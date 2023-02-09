using System;
using Facepunch;
using Rust.Demo;
using UnityEngine;

namespace Network
{
	// Token: 0x0200000F RID: 15
	public class DemoClient : Client, IDisposable
	{
		// Token: 0x0600006B RID: 107 RVA: 0x00002EA9 File Offset: 0x000010A9
		public DemoClient(Reader demoFile)
		{
			this.demoFile = demoFile;
			base.MultithreadingInit();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002EBE File Offset: 0x000010BE
		public virtual void Dispose()
		{
			Reader reader = this.demoFile;
			if (reader != null)
			{
				reader.Stop();
			}
			this.demoFile = null;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002ED8 File Offset: 0x000010D8
		public override bool IsConnected()
		{
			return true;
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00002ED8 File Offset: 0x000010D8
		public override bool IsPlaying
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00002EDB File Offset: 0x000010DB
		public bool PlayingFinished
		{
			get
			{
				return this.demoFile.IsFinished;
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00002EE8 File Offset: 0x000010E8
		public void UpdatePlayback(long frameTime)
		{
			if (this.PlayingFinished)
			{
				return;
			}
			this.demoFile.Progress(frameTime);
			while (!this.demoFile.IsFinished && this.PlaybackPacket())
			{
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00002F18 File Offset: 0x00001118
		private unsafe bool PlaybackPacket()
		{
			Packet packet = this.demoFile.ReadPacket();
			if (!packet.isValid)
			{
				return false;
			}
			byte[] array;
			byte* value;
			if ((array = packet.Data) == null || array.Length == 0)
			{
				value = null;
			}
			else
			{
				value = &array[0];
			}
			this.HandleMessage((IntPtr)((void*)value), packet.Size);
			array = null;
			return this.IsPlaying;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00002F74 File Offset: 0x00001174
		private void HandleMessage(IntPtr data, int size)
		{
			NetRead netRead = Pool.Get<NetRead>();
			netRead.Start(0UL, string.Empty, data, size);
			base.Decrypt(netRead.connection, netRead);
			byte b = netRead.PacketID();
			if (b < 140)
			{
				Pool.Free<NetRead>(ref netRead);
				return;
			}
			b -= 140;
			if (b > 25)
			{
				Debug.LogWarning("Invalid Packet (higher than " + Message.Type.ConsoleReplicatedVars + ")");
				this.Disconnect(string.Format("Invalid Packet ({0}) {1}b", b, size), true);
				Pool.Free<NetRead>(ref netRead);
				return;
			}
			Message message = base.StartMessage((Message.Type)b, netRead);
			if (this.callbackHandler != null)
			{
				try
				{
					using (TimeWarning.New("OnMessage", 0))
					{
						this.callbackHandler.OnNetworkMessage(message);
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					if (!this.IsPlaying)
					{
						this.Disconnect(ex.Message + "\n" + ex.StackTrace, true);
					}
				}
			}
			message.Clear();
			Pool.Free<Message>(ref message);
			Pool.Free<NetRead>(ref netRead);
		}

		// Token: 0x04000046 RID: 70
		protected Reader demoFile;
	}
}
