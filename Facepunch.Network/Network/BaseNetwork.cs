using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Facepunch;
using UnityEngine;

namespace Network
{
	// Token: 0x02000006 RID: 6
	public abstract class BaseNetwork
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8
		protected abstract float MaxReceiveTimeValue { get; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000009 RID: 9
		protected abstract int MaxReadQueueValue { get; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000A RID: 10
		protected abstract int MaxWriteQueueValue { get; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000B RID: 11
		protected abstract int MaxDecryptQueueValue { get; }

		// Token: 0x0600000C RID: 12 RVA: 0x00002060 File Offset: 0x00000260
		protected void MultithreadingInit()
		{
			if (this.readThread != null)
			{
				this.readThread.Abort();
				this.readThread = null;
			}
			if (this.writeThread != null)
			{
				this.writeThread.Abort();
				this.writeThread = null;
			}
			if (this.decryptThread != null)
			{
				this.decryptThread.Abort();
				this.decryptThread = null;
			}
			if (BaseNetwork.Multithreading)
			{
				this.readQueue = new ConcurrentQueue<NetRead>();
				this.writeQueue = new ConcurrentQueue<NetWrite>();
				this.decryptQueue = new ConcurrentQueue<NetRead>();
				this.mainThreadReset = new AutoResetEvent(false);
				this.readThreadReset = new AutoResetEvent(false);
				this.writeThreadReset = new AutoResetEvent(false);
				this.decryptThreadReset = new AutoResetEvent(false);
				this.readThread = new Thread(new ThreadStart(this.ReadThread));
				this.readThread.IsBackground = true;
				this.readThread.Start();
				this.writeThread = new Thread(new ThreadStart(this.WriteThread));
				this.writeThread.IsBackground = true;
				this.writeThread.Start();
				this.decryptThread = new Thread(new ThreadStart(this.DecryptThread));
				this.decryptThread.IsBackground = true;
				this.decryptThread.Start();
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000021A0 File Offset: 0x000003A0
		public virtual bool IsConnected()
		{
			return false;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021A0 File Offset: 0x000003A0
		protected virtual bool Receive()
		{
			return false;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000021A3 File Offset: 0x000003A3
		public void EnqueueWrite(NetWrite write)
		{
			if (this.writeQueue.Count >= this.MaxWriteQueueValue)
			{
				this.mainThreadReset.WaitOne(1000);
			}
			this.writeQueue.Enqueue(write);
			this.writeThreadReset.Set();
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000021E1 File Offset: 0x000003E1
		public void EnqueueRead(NetRead read)
		{
			this.readQueue.Enqueue(read);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000021EF File Offset: 0x000003EF
		public void EnqueueDecrypt(NetRead read)
		{
			this.decryptQueue.Enqueue(read);
			this.decryptThreadReset.Set();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002209 File Offset: 0x00000409
		public virtual void ProcessWrite(NetWrite write)
		{
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002209 File Offset: 0x00000409
		public virtual void ProcessRead(NetRead read)
		{
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000220B File Offset: 0x0000040B
		public void ProcessDecrypt(NetRead read)
		{
			this.Decrypt(read.connection, read);
			if (BaseNetwork.Multithreading)
			{
				this.EnqueueRead(read);
				return;
			}
			this.ProcessRead(read);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002230 File Offset: 0x00000430
		private void ReadThread()
		{
			while (this.IsConnected())
			{
				try
				{
					object obj = this.readLock;
					lock (obj)
					{
						this.ReadThreadCycle();
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
				if (this.decryptQueue.Count >= this.MaxDecryptQueueValue)
				{
					this.readThreadReset.WaitOne(1000);
				}
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000022B4 File Offset: 0x000004B4
		private void WriteThread()
		{
			while (this.IsConnected())
			{
				try
				{
					object obj = this.writeLock;
					lock (obj)
					{
						this.WriteThreadCycle();
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
				if (this.writeQueue.Count <= 0)
				{
					this.writeThreadReset.WaitOne(1000);
				}
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002334 File Offset: 0x00000534
		private void DecryptThread()
		{
			while (this.IsConnected())
			{
				try
				{
					this.DecryptThreadCycle();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
				if (this.readQueue.Count >= this.MaxReadQueueValue || this.decryptQueue.Count <= 0)
				{
					this.decryptThreadReset.WaitOne(1000);
				}
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000023A0 File Offset: 0x000005A0
		private void ReadThreadCycle()
		{
			this.readStopwatch.Restart();
			while (this.IsConnected() && this.decryptQueue.Count < this.MaxDecryptQueueValue && this.Receive() && this.readStopwatch.Elapsed.TotalMilliseconds <= (double)this.MaxReceiveTimeValue)
			{
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000023FC File Offset: 0x000005FC
		private void WriteThreadCycle()
		{
			this.writeStopwatch.Restart();
			NetWrite write;
			while (this.IsConnected() && this.writeQueue.TryDequeue(out write))
			{
				this.mainThreadReset.Set();
				this.ProcessWrite(write);
				if (this.writeStopwatch.Elapsed.TotalMilliseconds > (double)this.MaxReceiveTimeValue)
				{
					break;
				}
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000245C File Offset: 0x0000065C
		private void DecryptThreadCycle()
		{
			this.decryptStopwatch.Restart();
			NetRead read;
			while (this.IsConnected() && this.readQueue.Count < this.MaxReadQueueValue && this.decryptQueue.TryDequeue(out read))
			{
				this.readThreadReset.Set();
				this.ProcessDecrypt(read);
				if (this.decryptStopwatch.Elapsed.TotalMilliseconds > (double)this.MaxReceiveTimeValue)
				{
					break;
				}
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000024D0 File Offset: 0x000006D0
		public void Cycle()
		{
			if (BaseNetwork.Multithreading)
			{
				this.mainStopwatch.Restart();
				while (this.IsConnected())
				{
					NetRead read;
					if (!this.readQueue.TryDequeue(out read))
					{
						return;
					}
					this.decryptThreadReset.Set();
					this.ProcessRead(read);
					if (this.mainStopwatch.Elapsed.TotalMilliseconds > (double)this.MaxReceiveTimeValue)
					{
						return;
					}
				}
				return;
			}
			this.mainStopwatch.Restart();
			while (this.IsConnected() && this.Receive() && this.mainStopwatch.Elapsed.TotalMilliseconds <= (double)this.MaxReceiveTimeValue)
			{
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002571 File Offset: 0x00000771
		public NetWrite StartWrite()
		{
			NetWrite netWrite = Pool.Get<NetWrite>();
			netWrite.Start(this);
			return netWrite;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002580 File Offset: 0x00000780
		protected Message StartMessage(Message.Type type, NetRead read)
		{
			Message message = Pool.Get<Message>();
			message.peer = this;
			message.type = type;
			message.read = read;
			return message;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000259C File Offset: 0x0000079C
		public void Decrypt(Connection connection, NetRead read)
		{
			if (this.cryptography == null)
			{
				return;
			}
			if (connection == null)
			{
				return;
			}
			if (connection.encryptionLevel <= 0U)
			{
				return;
			}
			if (read.Length <= 1L)
			{
				return;
			}
			int num = (int)(read.PeekPacketID() - 140);
			if (num <= 0)
			{
				return;
			}
			if (num >= 25)
			{
				return;
			}
			if (!Message.EncryptionPerType[num])
			{
				return;
			}
			if (connection.encryptionLevel > 1U && read.Length >= 23L)
			{
				connection.trusted = ((read.Data[(int)(checked((IntPtr)(unchecked(read.Length - 17L))))] & 1) > 0);
			}
			ArraySegment<byte> arraySegment = new ArraySegment<byte>(read.Data, 1, (int)read.Length - 1);
			this.cryptography.Decrypt(connection, ref arraySegment);
			read.SetLength((long)(arraySegment.Offset + arraySegment.Count));
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000265C File Offset: 0x0000085C
		public ArraySegment<byte> Encrypt(Connection connection, NetWrite write)
		{
			ArraySegment<byte> arraySegment = new ArraySegment<byte>(write.Data, 1, (int)write.Length - 1);
			if (this.cryptography == null)
			{
				return arraySegment;
			}
			if (connection == null)
			{
				return arraySegment;
			}
			if (connection.encryptionLevel <= 0U)
			{
				return arraySegment;
			}
			if (write.Length <= 1L)
			{
				return arraySegment;
			}
			int num = (int)(write.PeekPacketID() - 140);
			if (num <= 0)
			{
				return arraySegment;
			}
			if (num >= 25)
			{
				return arraySegment;
			}
			if (!Message.EncryptionPerType[num])
			{
				return arraySegment;
			}
			return this.cryptography.EncryptCopy(connection, arraySegment);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000026D8 File Offset: 0x000008D8
		public void Record(Connection connection, Stream stream)
		{
			if (connection == null)
			{
				return;
			}
			connection.RecordPacket(stream);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000026E5 File Offset: 0x000008E5
		public virtual string GetDebug(Connection connection)
		{
			return null;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000026E8 File Offset: 0x000008E8
		public virtual ulong GetStat(Connection connection, BaseNetwork.StatTypeLong type)
		{
			return 0UL;
		}

		// Token: 0x04000003 RID: 3
		public static bool Multithreading;

		// Token: 0x04000004 RID: 4
		protected readonly object readLock = new object();

		// Token: 0x04000005 RID: 5
		protected readonly object writeLock = new object();

		// Token: 0x04000006 RID: 6
		private ConcurrentQueue<NetRead> readQueue;

		// Token: 0x04000007 RID: 7
		private ConcurrentQueue<NetWrite> writeQueue;

		// Token: 0x04000008 RID: 8
		private ConcurrentQueue<NetRead> decryptQueue;

		// Token: 0x04000009 RID: 9
		private AutoResetEvent mainThreadReset;

		// Token: 0x0400000A RID: 10
		private AutoResetEvent readThreadReset;

		// Token: 0x0400000B RID: 11
		private AutoResetEvent writeThreadReset;

		// Token: 0x0400000C RID: 12
		private AutoResetEvent decryptThreadReset;

		// Token: 0x0400000D RID: 13
		private Stopwatch mainStopwatch = new Stopwatch();

		// Token: 0x0400000E RID: 14
		private Stopwatch readStopwatch = new Stopwatch();

		// Token: 0x0400000F RID: 15
		private Stopwatch writeStopwatch = new Stopwatch();

		// Token: 0x04000010 RID: 16
		private Stopwatch decryptStopwatch = new Stopwatch();

		// Token: 0x04000011 RID: 17
		private Thread readThread;

		// Token: 0x04000012 RID: 18
		private Thread writeThread;

		// Token: 0x04000013 RID: 19
		private Thread decryptThread;

		// Token: 0x04000014 RID: 20
		public INetworkCryptography cryptography;

		// Token: 0x02000020 RID: 32
		public enum StatTypeLong
		{
			// Token: 0x04000095 RID: 149
			BytesSent,
			// Token: 0x04000096 RID: 150
			BytesSent_LastSecond,
			// Token: 0x04000097 RID: 151
			BytesReceived,
			// Token: 0x04000098 RID: 152
			BytesReceived_LastSecond,
			// Token: 0x04000099 RID: 153
			MessagesInSendBuffer,
			// Token: 0x0400009A RID: 154
			BytesInSendBuffer,
			// Token: 0x0400009B RID: 155
			MessagesInResendBuffer,
			// Token: 0x0400009C RID: 156
			BytesInResendBuffer,
			// Token: 0x0400009D RID: 157
			PacketLossAverage,
			// Token: 0x0400009E RID: 158
			PacketLossLastSecond,
			// Token: 0x0400009F RID: 159
			ThrottleBytes
		}
	}
}
