using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SilentOrbit.ProtocolBuffers;
using UnityEngine;

namespace Network
{
	// Token: 0x02000008 RID: 8
	public class Connection
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002741 File Offset: 0x00000941
		public TimeSpan RecordTimeElapsed
		{
			get
			{
				if (this.recordTime == null)
				{
					return TimeSpan.Zero;
				}
				return this.recordTime.Elapsed;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000029 RID: 41 RVA: 0x0000275C File Offset: 0x0000095C
		public string RecordFilename
		{
			get
			{
				return this.recordFilename;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002764 File Offset: 0x00000964
		public int RecordFilesize
		{
			get
			{
				return (int)this.recordStream.Length;
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002774 File Offset: 0x00000974
		public bool StartRecording(string targetFilename, IDemoHeader header)
		{
			if (this.recordStream != null)
			{
				return false;
			}
			this.recordFilename = targetFilename;
			this.recordHeader = header;
			this.recordStream = new MemoryStream();
			this.recordWriter = new BinaryWriter(this.recordStream);
			this.recordTime = Stopwatch.StartNew();
			return true;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000027C4 File Offset: 0x000009C4
		public void StopRecording()
		{
			if (this.recordStream == null)
			{
				return;
			}
			if (this.recordHeader != null)
			{
				Directory.CreateDirectory(Path.GetDirectoryName(this.recordFilename));
				using (FileStream fileStream = new FileStream(this.recordFilename, FileMode.Create))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
					{
						this.recordHeader.Length = (long)this.recordTime.Elapsed.TotalMilliseconds;
						this.recordHeader.Write(binaryWriter);
						this.recordStream.WriteTo(fileStream);
					}
				}
				this.recordHeader = null;
			}
			this.recordTime = null;
			this.recordWriter.Close();
			this.recordWriter = null;
			this.recordStream.Dispose();
			this.recordStream = null;
			this.recordFilename = null;
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000028AC File Offset: 0x00000AAC
		public bool IsRecording
		{
			get
			{
				return this.recordStream != null;
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000028B8 File Offset: 0x00000AB8
		public void RecordPacket(byte packetId, IProto proto)
		{
			if (!this.IsRecording)
			{
				return;
			}
			Connection.reusableStream.SetLength(0L);
			proto.WriteToStream(Connection.reusableStream);
			byte[] buffer = Connection.reusableStream.GetBuffer();
			int num = (int)Connection.reusableStream.Length;
			this.recordWriter.Write(num + 1);
			this.recordWriter.Write((long)this.recordTime.Elapsed.TotalMilliseconds);
			this.recordWriter.Write(packetId + 140);
			this.recordWriter.Write(buffer, 0, num);
			this.recordWriter.Write('\0');
			this.recordWriter.Write('\0');
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002964 File Offset: 0x00000B64
		public void RecordPacket(Stream stream)
		{
			if (!this.IsRecording)
			{
				return;
			}
			byte[] buffer = Connection.reusableStream.GetBuffer();
			int num = (int)stream.Length;
			long position = stream.Position;
			stream.Position = 0L;
			stream.Read(buffer, 0, num);
			stream.Position = position;
			this.recordWriter.Write(num);
			this.recordWriter.Write((long)this.recordTime.Elapsed.TotalMilliseconds);
			this.recordWriter.Write(buffer, 0, num);
			this.recordWriter.Write('\0');
			this.recordWriter.Write('\0');
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002A00 File Offset: 0x00000C00
		public string IPAddressWithoutPort()
		{
			int num = this.ipaddress.LastIndexOf(':');
			if (num != -1)
			{
				return this.ipaddress.Substring(0, num);
			}
			return this.ipaddress;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002A33 File Offset: 0x00000C33
		public virtual void OnDisconnected()
		{
			this.player = null;
			this.guid = 0UL;
			this.ResetPacketsPerSecond();
			this.hasRequestedWorld = false;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002A51 File Offset: 0x00000C51
		public bool isAuthenticated
		{
			get
			{
				return this.authStatus == "ok";
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002A64 File Offset: 0x00000C64
		public void ResetPacketsPerSecond()
		{
			for (int i = 0; i < this.packetsPerSecond.Length; i++)
			{
				this.packetsPerSecond[i].Reset();
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002A95 File Offset: 0x00000C95
		public void AddPacketsPerSecond(Message.Type message)
		{
			this.AddPacketsPerSecond((int)message);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002A9E File Offset: 0x00000C9E
		public void AddPacketsPerSecond(int index = 0)
		{
			if (index < 0 || index >= this.packetsPerSecond.Length)
			{
				return;
			}
			this.packetsPerSecond[index].Increment();
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002AC1 File Offset: 0x00000CC1
		public ulong GetPacketsPerSecond(Message.Type message)
		{
			return this.GetPacketsPerSecond((int)message);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002ACA File Offset: 0x00000CCA
		public ulong GetPacketsPerSecond(int index = 0)
		{
			if (index < 0 || index >= this.packetsPerSecond.Length)
			{
				return 0UL;
			}
			return this.packetsPerSecond[index].Calculate();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002AEF File Offset: 0x00000CEF
		public float GetSecondsConnected()
		{
			return (float)(TimeEx.realtimeSinceStartup - this.connectionTime);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002AFE File Offset: 0x00000CFE
		public override string ToString()
		{
			return string.Format("{0}/{1}/{2}", this.ipaddress, this.userid, this.username);
		}

		// Token: 0x04000015 RID: 21
		private static MemoryStream reusableStream = new MemoryStream(1048576);

		// Token: 0x04000016 RID: 22
		protected MemoryStream recordStream;

		// Token: 0x04000017 RID: 23
		protected BinaryWriter recordWriter;

		// Token: 0x04000018 RID: 24
		protected Stopwatch recordTime;

		// Token: 0x04000019 RID: 25
		protected string recordFilename;

		// Token: 0x0400001A RID: 26
		protected IDemoHeader recordHeader;

		// Token: 0x0400001B RID: 27
		public Connection.State state;

		// Token: 0x0400001C RID: 28
		public bool active;

		// Token: 0x0400001D RID: 29
		public bool connected;

		// Token: 0x0400001E RID: 30
		public uint authLevel;

		// Token: 0x0400001F RID: 31
		public uint encryptionLevel;

		// Token: 0x04000020 RID: 32
		public bool trusted;

		// Token: 0x04000021 RID: 33
		public bool rejected;

		// Token: 0x04000022 RID: 34
		public string authStatus;

		// Token: 0x04000023 RID: 35
		public byte[] token;

		// Token: 0x04000024 RID: 36
		public bool hasRequestedWorld;

		// Token: 0x04000025 RID: 37
		public ulong guid;

		// Token: 0x04000026 RID: 38
		public ulong userid;

		// Token: 0x04000027 RID: 39
		public ulong ownerid;

		// Token: 0x04000028 RID: 40
		public string username;

		// Token: 0x04000029 RID: 41
		public string os;

		// Token: 0x0400002A RID: 42
		public uint protocol;

		// Token: 0x0400002B RID: 43
		private TimeAverageValueData[] packetsPerSecond = new TimeAverageValueData[26];

		// Token: 0x0400002C RID: 44
		public double connectionTime;

		// Token: 0x0400002D RID: 45
		public string ipaddress;

		// Token: 0x0400002E RID: 46
		public MonoBehaviour player;

		// Token: 0x0400002F RID: 47
		public Connection.Validation validate;

		// Token: 0x04000030 RID: 48
		public Connection.ClientInfo info = new Connection.ClientInfo();

		// Token: 0x02000021 RID: 33
		public enum State
		{
			// Token: 0x040000A1 RID: 161
			Unconnected,
			// Token: 0x040000A2 RID: 162
			Connecting,
			// Token: 0x040000A3 RID: 163
			InQueue,
			// Token: 0x040000A4 RID: 164
			Welcoming,
			// Token: 0x040000A5 RID: 165
			Connected,
			// Token: 0x040000A6 RID: 166
			Disconnected
		}

		// Token: 0x02000022 RID: 34
		public struct Validation
		{
			// Token: 0x040000A7 RID: 167
			public uint entityUpdates;
		}

		// Token: 0x02000023 RID: 35
		public class ClientInfo
		{
			// Token: 0x06000133 RID: 307 RVA: 0x00004C5D File Offset: 0x00002E5D
			public void Set(string k, string v)
			{
				this.info[k] = v;
			}

			// Token: 0x06000134 RID: 308 RVA: 0x00004C6C File Offset: 0x00002E6C
			public string GetString(string k, string def = "")
			{
				string result;
				if (this.info.TryGetValue(k, out result))
				{
					return result;
				}
				return def;
			}

			// Token: 0x06000135 RID: 309 RVA: 0x00004C8C File Offset: 0x00002E8C
			public float GetFloat(string k, float def = 0f)
			{
				string @string = this.GetString(k, null);
				if (@string == null)
				{
					return def;
				}
				float result;
				if (float.TryParse(@string, out result))
				{
					return result;
				}
				return def;
			}

			// Token: 0x06000136 RID: 310 RVA: 0x00004CB4 File Offset: 0x00002EB4
			public int GetInt(string k, int def = 0)
			{
				string @string = this.GetString(k, null);
				if (@string == null)
				{
					return def;
				}
				int result;
				if (int.TryParse(@string, out result))
				{
					return result;
				}
				return def;
			}

			// Token: 0x06000137 RID: 311 RVA: 0x00004CDC File Offset: 0x00002EDC
			public bool GetBool(string k, bool def = false)
			{
				string @string = this.GetString(k, null);
				if (@string == null)
				{
					return def;
				}
				bool result;
				if (bool.TryParse(@string, out result))
				{
					return result;
				}
				return def;
			}

			// Token: 0x040000A8 RID: 168
			public Dictionary<string, string> info = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}
	}
}
