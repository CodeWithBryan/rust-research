using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ConVar;
using Facepunch.Rcon;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Facepunch
{
	// Token: 0x02000AAA RID: 2730
	public class RCon
	{
		// Token: 0x06004153 RID: 16723 RVA: 0x0017F910 File Offset: 0x0017DB10
		public static void Initialize()
		{
			if (RCon.Port == 0)
			{
				RCon.Port = Server.port;
			}
			RCon.Password = CommandLine.GetSwitch("-rcon.password", CommandLine.GetSwitch("+rcon.password", ""));
			if (RCon.Password == "password")
			{
				return;
			}
			if (RCon.Password == "")
			{
				return;
			}
			Output.OnMessage += RCon.OnMessage;
			if (RCon.Web)
			{
				RCon.listenerNew = new Listener();
				if (!string.IsNullOrEmpty(RCon.Ip))
				{
					RCon.listenerNew.Address = RCon.Ip;
				}
				RCon.listenerNew.Password = RCon.Password;
				RCon.listenerNew.Port = RCon.Port;
				RCon.listenerNew.SslCertificate = CommandLine.GetSwitch("-rcon.ssl", null);
				RCon.listenerNew.SslCertificatePassword = CommandLine.GetSwitch("-rcon.sslpwd", null);
				RCon.listenerNew.OnMessage = delegate(IPAddress ip, int id, string msg)
				{
					Queue<RCon.Command> commands = RCon.Commands;
					lock (commands)
					{
						RCon.Command item = JsonConvert.DeserializeObject<RCon.Command>(msg);
						item.Ip = ip;
						item.ConnectionId = id;
						RCon.Commands.Enqueue(item);
					}
				};
				RCon.listenerNew.Start();
				Debug.Log("WebSocket RCon Started on " + RCon.Port);
				return;
			}
			RCon.listener = new RCon.RConListener();
			Debug.Log("RCon Started on " + RCon.Port);
			Debug.Log("Source style TCP Rcon is deprecated. Please switch to Websocket Rcon before it goes away.");
		}

		// Token: 0x06004154 RID: 16724 RVA: 0x0017FA72 File Offset: 0x0017DC72
		public static void Shutdown()
		{
			if (RCon.listenerNew != null)
			{
				RCon.listenerNew.Shutdown();
				RCon.listenerNew = null;
			}
			if (RCon.listener != null)
			{
				RCon.listener.Shutdown();
				RCon.listener = null;
			}
		}

		// Token: 0x06004155 RID: 16725 RVA: 0x0017FAA4 File Offset: 0x0017DCA4
		public static void Broadcast(RCon.LogType type, object obj)
		{
			if (RCon.listenerNew == null)
			{
				return;
			}
			string message = JsonConvert.SerializeObject(obj, Formatting.Indented);
			RCon.Broadcast(type, message);
		}

		// Token: 0x06004156 RID: 16726 RVA: 0x0017FAC8 File Offset: 0x0017DCC8
		public static void Broadcast(RCon.LogType type, string message)
		{
			if (RCon.listenerNew == null || string.IsNullOrWhiteSpace(message))
			{
				return;
			}
			RCon.Response response = default(RCon.Response);
			response.Identifier = -1;
			response.Message = message;
			response.Type = type;
			if (RCon.responseConnection < 0)
			{
				RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject(response, Formatting.Indented));
				return;
			}
			RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject(response, Formatting.Indented));
		}

		// Token: 0x06004157 RID: 16727 RVA: 0x0017FB40 File Offset: 0x0017DD40
		public static void Update()
		{
			Queue<RCon.Command> commands = RCon.Commands;
			lock (commands)
			{
				while (RCon.Commands.Count > 0)
				{
					RCon.OnCommand(RCon.Commands.Dequeue());
				}
			}
			if (RCon.listener == null)
			{
				return;
			}
			if (RCon.lastRunTime + 0.02f >= UnityEngine.Time.realtimeSinceStartup)
			{
				return;
			}
			RCon.lastRunTime = UnityEngine.Time.realtimeSinceStartup;
			try
			{
				RCon.bannedAddresses.RemoveAll((RCon.BannedAddresses x) => x.banTime < UnityEngine.Time.realtimeSinceStartup);
				RCon.listener.Cycle();
			}
			catch (Exception exception)
			{
				Debug.LogWarning("Rcon Exception");
				Debug.LogException(exception);
			}
		}

		// Token: 0x06004158 RID: 16728 RVA: 0x0017FC10 File Offset: 0x0017DE10
		public static void BanIP(IPAddress addr, float seconds)
		{
			RCon.bannedAddresses.RemoveAll((RCon.BannedAddresses x) => x.addr == addr);
			RCon.BannedAddresses bannedAddresses = default(RCon.BannedAddresses);
			bannedAddresses.addr = addr;
			bannedAddresses.banTime = UnityEngine.Time.realtimeSinceStartup + seconds;
		}

		// Token: 0x06004159 RID: 16729 RVA: 0x0017FC64 File Offset: 0x0017DE64
		public static bool IsBanned(IPAddress addr)
		{
			return RCon.bannedAddresses.Count((RCon.BannedAddresses x) => x.addr == addr && x.banTime > UnityEngine.Time.realtimeSinceStartup) > 0;
		}

		// Token: 0x0600415A RID: 16730 RVA: 0x0017FC98 File Offset: 0x0017DE98
		private static void OnCommand(RCon.Command cmd)
		{
			try
			{
				RCon.responseIdentifier = cmd.Identifier;
				RCon.responseConnection = cmd.ConnectionId;
				RCon.isInput = true;
				if (RCon.Print)
				{
					Debug.Log(string.Concat(new object[]
					{
						"[rcon] ",
						cmd.Ip,
						": ",
						cmd.Message
					}));
				}
				RCon.isInput = false;
				string text = ConsoleSystem.Run(ConsoleSystem.Option.Server.Quiet(), cmd.Message, Array.Empty<object>());
				if (text != null)
				{
					RCon.OnMessage(text, string.Empty, UnityEngine.LogType.Log);
				}
			}
			finally
			{
				RCon.responseIdentifier = 0;
				RCon.responseConnection = -1;
			}
		}

		// Token: 0x0600415B RID: 16731 RVA: 0x0017FD4C File Offset: 0x0017DF4C
		private static void OnMessage(string message, string stacktrace, UnityEngine.LogType type)
		{
			if (RCon.isInput)
			{
				return;
			}
			if (RCon.listenerNew != null)
			{
				RCon.Response response = default(RCon.Response);
				response.Identifier = RCon.responseIdentifier;
				response.Message = message;
				response.Stacktrace = stacktrace;
				response.Type = RCon.LogType.Generic;
				if (type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception)
				{
					response.Type = RCon.LogType.Error;
				}
				if (type == UnityEngine.LogType.Assert || type == UnityEngine.LogType.Warning)
				{
					response.Type = RCon.LogType.Warning;
				}
				if (RCon.responseConnection < 0)
				{
					RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject(response, Formatting.Indented));
					return;
				}
				RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject(response, Formatting.Indented));
			}
		}

		// Token: 0x04003A3D RID: 14909
		public static string Password = "";

		// Token: 0x04003A3E RID: 14910
		[ServerVar]
		public static int Port = 0;

		// Token: 0x04003A3F RID: 14911
		[ServerVar]
		public static string Ip = "";

		// Token: 0x04003A40 RID: 14912
		[ServerVar(Help = "If set to true, use websocket rcon. If set to false use legacy, source engine rcon.")]
		public static bool Web = true;

		// Token: 0x04003A41 RID: 14913
		[ServerVar(Help = "If true, rcon commands etc will be printed in the console")]
		public static bool Print = false;

		// Token: 0x04003A42 RID: 14914
		internal static RCon.RConListener listener = null;

		// Token: 0x04003A43 RID: 14915
		internal static Listener listenerNew = null;

		// Token: 0x04003A44 RID: 14916
		private static Queue<RCon.Command> Commands = new Queue<RCon.Command>();

		// Token: 0x04003A45 RID: 14917
		private static float lastRunTime = 0f;

		// Token: 0x04003A46 RID: 14918
		internal static List<RCon.BannedAddresses> bannedAddresses = new List<RCon.BannedAddresses>();

		// Token: 0x04003A47 RID: 14919
		private static int responseIdentifier;

		// Token: 0x04003A48 RID: 14920
		private static int responseConnection;

		// Token: 0x04003A49 RID: 14921
		private static bool isInput;

		// Token: 0x04003A4A RID: 14922
		internal static int SERVERDATA_AUTH = 3;

		// Token: 0x04003A4B RID: 14923
		internal static int SERVERDATA_EXECCOMMAND = 2;

		// Token: 0x04003A4C RID: 14924
		internal static int SERVERDATA_AUTH_RESPONSE = 2;

		// Token: 0x04003A4D RID: 14925
		internal static int SERVERDATA_RESPONSE_VALUE = 0;

		// Token: 0x04003A4E RID: 14926
		internal static int SERVERDATA_CONSOLE_LOG = 4;

		// Token: 0x04003A4F RID: 14927
		internal static int SERVERDATA_SWITCH_UTF8 = 5;

		// Token: 0x02000F04 RID: 3844
		public struct Command
		{
			// Token: 0x04004CDC RID: 19676
			public IPAddress Ip;

			// Token: 0x04004CDD RID: 19677
			public int ConnectionId;

			// Token: 0x04004CDE RID: 19678
			public string Name;

			// Token: 0x04004CDF RID: 19679
			public string Message;

			// Token: 0x04004CE0 RID: 19680
			public int Identifier;
		}

		// Token: 0x02000F05 RID: 3845
		public enum LogType
		{
			// Token: 0x04004CE2 RID: 19682
			Generic,
			// Token: 0x04004CE3 RID: 19683
			Error,
			// Token: 0x04004CE4 RID: 19684
			Warning,
			// Token: 0x04004CE5 RID: 19685
			Chat,
			// Token: 0x04004CE6 RID: 19686
			Report,
			// Token: 0x04004CE7 RID: 19687
			ClientPerf
		}

		// Token: 0x02000F06 RID: 3846
		public struct Response
		{
			// Token: 0x04004CE8 RID: 19688
			public string Message;

			// Token: 0x04004CE9 RID: 19689
			public int Identifier;

			// Token: 0x04004CEA RID: 19690
			[JsonConverter(typeof(StringEnumConverter))]
			public RCon.LogType Type;

			// Token: 0x04004CEB RID: 19691
			public string Stacktrace;
		}

		// Token: 0x02000F07 RID: 3847
		internal struct BannedAddresses
		{
			// Token: 0x04004CEC RID: 19692
			public IPAddress addr;

			// Token: 0x04004CED RID: 19693
			public float banTime;
		}

		// Token: 0x02000F08 RID: 3848
		internal class RConClient
		{
			// Token: 0x060051C9 RID: 20937 RVA: 0x001A53CF File Offset: 0x001A35CF
			internal RConClient(Socket cl)
			{
				this.socket = cl;
				this.socket.NoDelay = true;
				this.connectionName = this.socket.RemoteEndPoint.ToString();
			}

			// Token: 0x060051CA RID: 20938 RVA: 0x001A5407 File Offset: 0x001A3607
			internal bool IsValid()
			{
				return this.socket != null;
			}

			// Token: 0x060051CB RID: 20939 RVA: 0x001A5414 File Offset: 0x001A3614
			internal void Update()
			{
				if (this.socket == null)
				{
					return;
				}
				if (!this.socket.Connected)
				{
					this.Close("Disconnected");
					return;
				}
				int available = this.socket.Available;
				if (available < 14)
				{
					return;
				}
				if (available > 4096)
				{
					this.Close("overflow");
					return;
				}
				byte[] buffer = new byte[available];
				this.socket.Receive(buffer);
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer, false), this.utf8Mode ? Encoding.UTF8 : Encoding.ASCII))
				{
					int num = binaryReader.ReadInt32();
					if (available < num)
					{
						this.Close("invalid packet");
					}
					else
					{
						this.lastMessageID = binaryReader.ReadInt32();
						int type = binaryReader.ReadInt32();
						string msg = this.ReadNullTerminatedString(binaryReader);
						this.ReadNullTerminatedString(binaryReader);
						if (!this.HandleMessage(type, msg))
						{
							this.Close("invalid packet");
						}
						else
						{
							this.lastMessageID = -1;
						}
					}
				}
			}

			// Token: 0x060051CC RID: 20940 RVA: 0x001A5518 File Offset: 0x001A3718
			internal bool HandleMessage(int type, string msg)
			{
				if (!this.isAuthorised)
				{
					return this.HandleMessage_UnAuthed(type, msg);
				}
				if (type == RCon.SERVERDATA_SWITCH_UTF8)
				{
					this.utf8Mode = true;
					return true;
				}
				if (type == RCon.SERVERDATA_EXECCOMMAND)
				{
					Debug.Log("[RCON][" + this.connectionName + "] " + msg);
					this.runningConsoleCommand = true;
					ConsoleSystem.Run(ConsoleSystem.Option.Server, msg, Array.Empty<object>());
					this.runningConsoleCommand = false;
					this.Reply(-1, RCon.SERVERDATA_RESPONSE_VALUE, "");
					return true;
				}
				if (type == RCon.SERVERDATA_RESPONSE_VALUE)
				{
					this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
					return true;
				}
				Debug.Log(string.Concat(new object[]
				{
					"[RCON][",
					this.connectionName,
					"] Unhandled: ",
					this.lastMessageID,
					" -> ",
					type,
					" -> ",
					msg
				}));
				return false;
			}

			// Token: 0x060051CD RID: 20941 RVA: 0x001A5614 File Offset: 0x001A3814
			internal bool HandleMessage_UnAuthed(int type, string msg)
			{
				if (type != RCon.SERVERDATA_AUTH)
				{
					RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
					this.Close("Invalid Command - Not Authed");
					return false;
				}
				this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
				this.isAuthorised = (RCon.Password == msg);
				if (!this.isAuthorised)
				{
					this.Reply(-1, RCon.SERVERDATA_AUTH_RESPONSE, "");
					RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
					this.Close("Invalid Password");
					return true;
				}
				this.Reply(this.lastMessageID, RCon.SERVERDATA_AUTH_RESPONSE, "");
				Debug.Log("[RCON] Auth: " + this.connectionName);
				Output.OnMessage += this.Output_OnMessage;
				return true;
			}

			// Token: 0x060051CE RID: 20942 RVA: 0x001A5700 File Offset: 0x001A3900
			private void Output_OnMessage(string message, string stacktrace, UnityEngine.LogType type)
			{
				if (!this.isAuthorised)
				{
					return;
				}
				if (!this.IsValid())
				{
					return;
				}
				if (this.lastMessageID != -1 && this.runningConsoleCommand)
				{
					this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, message);
				}
				this.Reply(0, RCon.SERVERDATA_CONSOLE_LOG, message);
			}

			// Token: 0x060051CF RID: 20943 RVA: 0x001A5750 File Offset: 0x001A3950
			internal void Reply(int id, int type, string msg)
			{
				MemoryStream memoryStream = new MemoryStream(1024);
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					if (this.utf8Mode)
					{
						byte[] bytes = Encoding.UTF8.GetBytes(msg);
						int value = 10 + bytes.Length;
						binaryWriter.Write(value);
						binaryWriter.Write(id);
						binaryWriter.Write(type);
						binaryWriter.Write(bytes);
					}
					else
					{
						int value2 = 10 + msg.Length;
						binaryWriter.Write(value2);
						binaryWriter.Write(id);
						binaryWriter.Write(type);
						foreach (char c in msg)
						{
							binaryWriter.Write((sbyte)c);
						}
					}
					binaryWriter.Write(0);
					binaryWriter.Write(0);
					binaryWriter.Flush();
					try
					{
						this.socket.Send(memoryStream.GetBuffer(), (int)memoryStream.Position, SocketFlags.None);
					}
					catch (Exception arg)
					{
						Debug.LogWarning("Error sending rcon reply: " + arg);
						this.Close("Exception");
					}
				}
			}

			// Token: 0x060051D0 RID: 20944 RVA: 0x001A586C File Offset: 0x001A3A6C
			internal void Close(string strReasn)
			{
				Output.OnMessage -= this.Output_OnMessage;
				if (this.socket == null)
				{
					return;
				}
				Debug.Log("[RCON][" + this.connectionName + "] Disconnected: " + strReasn);
				this.socket.Close();
				this.socket = null;
			}

			// Token: 0x060051D1 RID: 20945 RVA: 0x001A58C0 File Offset: 0x001A3AC0
			internal string ReadNullTerminatedString(BinaryReader read)
			{
				string text = "";
				while (read.BaseStream.Position != read.BaseStream.Length)
				{
					char c = read.ReadChar();
					if (c == '\0')
					{
						return text;
					}
					text += c.ToString();
					if (text.Length > 8192)
					{
						return string.Empty;
					}
				}
				return "";
			}

			// Token: 0x04004CEE RID: 19694
			private Socket socket;

			// Token: 0x04004CEF RID: 19695
			private bool isAuthorised;

			// Token: 0x04004CF0 RID: 19696
			private string connectionName;

			// Token: 0x04004CF1 RID: 19697
			private int lastMessageID = -1;

			// Token: 0x04004CF2 RID: 19698
			private bool runningConsoleCommand;

			// Token: 0x04004CF3 RID: 19699
			private bool utf8Mode;
		}

		// Token: 0x02000F09 RID: 3849
		internal class RConListener
		{
			// Token: 0x060051D2 RID: 20946 RVA: 0x001A5920 File Offset: 0x001A3B20
			internal RConListener()
			{
				IPAddress any = IPAddress.Any;
				if (!IPAddress.TryParse(RCon.Ip, out any))
				{
					any = IPAddress.Any;
				}
				this.server = new TcpListener(any, RCon.Port);
				try
				{
					this.server.Start();
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Couldn't start RCON Listener: " + ex.Message);
					this.server = null;
				}
			}

			// Token: 0x060051D3 RID: 20947 RVA: 0x001A59A8 File Offset: 0x001A3BA8
			internal void Shutdown()
			{
				if (this.server != null)
				{
					this.server.Stop();
					this.server = null;
				}
			}

			// Token: 0x060051D4 RID: 20948 RVA: 0x001A59C4 File Offset: 0x001A3BC4
			internal void Cycle()
			{
				if (this.server == null)
				{
					return;
				}
				this.ProcessConnections();
				this.RemoveDeadClients();
				this.UpdateClients();
			}

			// Token: 0x060051D5 RID: 20949 RVA: 0x001A59E4 File Offset: 0x001A3BE4
			private void ProcessConnections()
			{
				if (!this.server.Pending())
				{
					return;
				}
				Socket socket = this.server.AcceptSocket();
				if (socket == null)
				{
					return;
				}
				IPEndPoint ipendPoint = socket.RemoteEndPoint as IPEndPoint;
				if (RCon.IsBanned(ipendPoint.Address))
				{
					Debug.Log("[RCON] Ignoring connection - banned. " + ipendPoint.Address.ToString());
					socket.Close();
					return;
				}
				this.clients.Add(new RCon.RConClient(socket));
			}

			// Token: 0x060051D6 RID: 20950 RVA: 0x001A5A5C File Offset: 0x001A3C5C
			private void UpdateClients()
			{
				foreach (RCon.RConClient rconClient in this.clients)
				{
					rconClient.Update();
				}
			}

			// Token: 0x060051D7 RID: 20951 RVA: 0x001A5AAC File Offset: 0x001A3CAC
			private void RemoveDeadClients()
			{
				this.clients.RemoveAll((RCon.RConClient x) => !x.IsValid());
			}

			// Token: 0x04004CF4 RID: 19700
			private TcpListener server;

			// Token: 0x04004CF5 RID: 19701
			private List<RCon.RConClient> clients = new List<RCon.RConClient>();
		}
	}
}
