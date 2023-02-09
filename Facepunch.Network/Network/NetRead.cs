using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using UnityEngine;

namespace Network
{
	// Token: 0x02000011 RID: 17
	public class NetRead : Stream, Pool.IPooled
	{
		// Token: 0x06000077 RID: 119 RVA: 0x000030E1 File Offset: 0x000012E1
		public void EnterPool()
		{
			this.connection = null;
			if (this.Data != null)
			{
				ArrayPool<byte>.Shared.Return(this.Data, false);
				this.Data = null;
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00002209 File Offset: 0x00000409
		public void LeavePool()
		{
		}

		// Token: 0x06000079 RID: 121 RVA: 0x0000310A File Offset: 0x0000130A
		public bool Start(ulong guid, string ipaddress, IntPtr data, int length)
		{
			this.connection = null;
			this.guid = guid;
			this.ipaddress = ipaddress;
			return this.Init(data, length);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000312A File Offset: 0x0000132A
		public bool Start(Connection connection, IntPtr data, int length)
		{
			this.connection = connection;
			this.guid = connection.guid;
			this.ipaddress = connection.ipaddress;
			return this.Init(data, length);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00003153 File Offset: 0x00001353
		public bool Start(Connection connection, ulong guid, IntPtr data, int length)
		{
			this.connection = connection;
			this.guid = guid;
			this.ipaddress = connection.ipaddress;
			return this.Init(data, length);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00003178 File Offset: 0x00001378
		public bool Start(Connection connection, ulong guid, string ipaddress, IntPtr data, int length)
		{
			this.connection = connection;
			this.guid = guid;
			this.ipaddress = ipaddress;
			return this.Init(data, length);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000319C File Offset: 0x0000139C
		private unsafe bool Init(IntPtr data, int length)
		{
			if (length > 6291456)
			{
				throw new Exception(string.Format("Packet was too large (max is {0})", 6291456));
			}
			this.EnsureCapacity(length);
			this.SetLength((long)length);
			this.Position = 0L;
			byte[] array;
			byte* destination;
			if ((array = this.Data) == null || array.Length == 0)
			{
				destination = null;
			}
			else
			{
				destination = &array[0];
			}
			Buffer.MemoryCopy((void*)data, (void*)destination, (long)this.Data.Length, (long)length);
			array = null;
			return true;
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00003219 File Offset: 0x00001419
		public int Unread
		{
			get
			{
				return (int)(this.Length - this.Position);
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00003229 File Offset: 0x00001429
		public string String(int maxLength = 256)
		{
			return this.StringInternal(maxLength, false);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003233 File Offset: 0x00001433
		public string StringMultiLine(int maxLength = 2048)
		{
			return this.StringInternal(maxLength, true);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003240 File Offset: 0x00001440
		private string StringInternal(int maxLength, bool allowNewLine)
		{
			int num = this.BytesWithSize(NetRead.byteBuffer, 8388608U);
			if (num <= 0)
			{
				return string.Empty;
			}
			int num2 = Encoding.UTF8.GetChars(NetRead.byteBuffer, 0, num, NetRead.charBuffer, 0);
			if (num2 > maxLength)
			{
				num2 = maxLength;
			}
			for (int i = 0; i < num2; i++)
			{
				char c = NetRead.charBuffer[i];
				if (char.IsControl(c) && (!allowNewLine || c != '\n'))
				{
					NetRead.charBuffer[i] = ' ';
				}
			}
			return new string(NetRead.charBuffer, 0, num2);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000032C0 File Offset: 0x000014C0
		public string StringRaw(uint maxLength = 8388608U)
		{
			int num = this.BytesWithSize(NetRead.byteBuffer, maxLength);
			if (num <= 0)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(NetRead.byteBuffer, 0, num);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000032F8 File Offset: 0x000014F8
		public bool TemporaryBytesWithSize(out byte[] buffer, out int size)
		{
			buffer = NetRead.byteBuffer;
			size = 0;
			uint num = this.UInt32();
			if (num == 0U)
			{
				return false;
			}
			if ((ulong)num > (ulong)((long)NetRead.byteBuffer.Length))
			{
				return false;
			}
			size = this.Read(NetRead.byteBuffer, 0, (int)num);
			return (long)size == (long)((ulong)num);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003342 File Offset: 0x00001542
		public uint EntityID()
		{
			return this.UInt32();
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003342 File Offset: 0x00001542
		public uint GroupID()
		{
			return this.UInt32();
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000334C File Offset: 0x0000154C
		public int BytesWithSize(byte[] buffer, uint maxLength = 4294967295U)
		{
			uint num = this.UInt32();
			if (num == 0U)
			{
				return 0;
			}
			if ((ulong)num > (ulong)((long)buffer.Length) || num > maxLength)
			{
				return -1;
			}
			if ((long)this.Read(buffer, 0, (int)num) != (long)((ulong)num))
			{
				return -1;
			}
			return (int)num;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00003384 File Offset: 0x00001584
		public byte[] BytesWithSize(uint maxSize = 10485760U)
		{
			uint num = this.UInt32();
			if (num == 0U)
			{
				return null;
			}
			if (num > maxSize)
			{
				return null;
			}
			byte[] array = new byte[num];
			if ((long)this.Read(array, 0, (int)num) != (long)((ulong)num))
			{
				return null;
			}
			return array;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000033BB File Offset: 0x000015BB
		public override int ReadByte()
		{
			if ((long)this._position == this.Length)
			{
				return -1;
			}
			int result = (int)this.Data[this._position];
			this._position++;
			return result;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000033EC File Offset: 0x000015EC
		private void EnsureCapacity(int spaceRequired)
		{
			if (this.Data == null)
			{
				int num = (spaceRequired <= 16384) ? 16384 : spaceRequired;
				int num2 = Mathf.NextPowerOfTwo(num);
				if (num2 > 8388608)
				{
					throw new Exception(string.Format("Preventing NetWrite buffer from growing too large (requiredLength={0})", num));
				}
				this.Data = ArrayPool<byte>.Shared.Rent(num2);
				return;
			}
			else
			{
				if (this.Data.Length - this._position >= spaceRequired)
				{
					return;
				}
				int num3 = this._position + spaceRequired;
				int num4 = Mathf.NextPowerOfTwo(Math.Max(num3, this.Data.Length));
				if (num4 > 8388608)
				{
					throw new Exception(string.Format("Preventing NetWrite buffer from growing too large (requiredLength={0})", num3));
				}
				byte[] array = ArrayPool<byte>.Shared.Rent(num4);
				Buffer.BlockCopy(this.Data, 0, array, 0, this._length);
				ArrayPool<byte>.Shared.Return(this.Data, false);
				this.Data = array;
				return;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00002ED8 File Offset: 0x000010D8
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600008B RID: 139 RVA: 0x000021A0 File Offset: 0x000003A0
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000034D2 File Offset: 0x000016D2
		public override void SetLength(long value)
		{
			this._length = (int)value;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000034DC File Offset: 0x000016DC
		public byte PacketID()
		{
			return this.Read<byte>();
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000034E4 File Offset: 0x000016E4
		public byte PeekPacketID()
		{
			return this.Peek<byte>();
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000034EC File Offset: 0x000016EC
		public bool Bit()
		{
			return this.UInt8() > 0;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000034DC File Offset: 0x000016DC
		public byte UInt8()
		{
			return this.Read<byte>();
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000034F7 File Offset: 0x000016F7
		public ushort UInt16()
		{
			return this.Read<ushort>();
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000034FF File Offset: 0x000016FF
		public uint UInt32()
		{
			return this.Read<uint>();
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00003507 File Offset: 0x00001707
		public ulong UInt64()
		{
			return this.Read<ulong>();
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000350F File Offset: 0x0000170F
		public sbyte Int8()
		{
			return this.Read<sbyte>();
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00003517 File Offset: 0x00001717
		public short Int16()
		{
			return this.Read<short>();
		}

		// Token: 0x06000096 RID: 150 RVA: 0x0000351F File Offset: 0x0000171F
		public int Int32()
		{
			return this.Read<int>();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003527 File Offset: 0x00001727
		public long Int64()
		{
			return this.Read<long>();
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000352F File Offset: 0x0000172F
		public float Float()
		{
			return this.Read<float>();
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00003537 File Offset: 0x00001737
		public double Double()
		{
			return this.Read<double>();
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000353F File Offset: 0x0000173F
		public Vector3 Vector3()
		{
			return this.Read<Vector3>();
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00003547 File Offset: 0x00001747
		public Quaternion Quaternion()
		{
			return this.Read<Quaternion>();
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000354F File Offset: 0x0000174F
		public Ray Ray()
		{
			return this.Read<Ray>();
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00003557 File Offset: 0x00001757
		public Color Color()
		{
			return this.Read<Color>();
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00003560 File Offset: 0x00001760
		public unsafe override int Read(byte[] buffer, int offset, int count)
		{
			if (this._position + count > this._length)
			{
				count = this._length - this._position;
			}
			byte[] array;
			byte* ptr;
			if ((array = this.Data) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			fixed (byte[] array2 = buffer)
			{
				byte* ptr2;
				if (buffer == null || array2.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array2[0];
				}
				Buffer.MemoryCopy((void*)(ptr + this._position), (void*)(ptr2 + offset), (long)count, (long)count);
			}
			array = null;
			this._position += count;
			return count;
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600009F RID: 159 RVA: 0x000035E8 File Offset: 0x000017E8
		public override long Length
		{
			get
			{
				return (long)this._length;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x000035F1 File Offset: 0x000017F1
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x000035FA File Offset: 0x000017FA
		public override long Position
		{
			get
			{
				return (long)this._position;
			}
			set
			{
				this._position = (int)value;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x00002ED8 File Offset: 0x000010D8
		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00003604 File Offset: 0x00001804
		public override long Seek(long offset, SeekOrigin origin)
		{
			if (origin == SeekOrigin.Current)
			{
				this._position += (int)offset;
				return (long)this._position;
			}
			throw new NotImplementedException();
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00003628 File Offset: 0x00001828
		public T Read<[IsUnmanaged] T>() where T : struct, ValueType
		{
			if (this.Unread < sizeof(T))
			{
				return default(T);
			}
			T result = this.Data.ReadUnsafe(this._position);
			this._position += sizeof(T);
			return result;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00003674 File Offset: 0x00001874
		public T Peek<[IsUnmanaged] T>() where T : struct, ValueType
		{
			if (this.Unread < sizeof(T))
			{
				return default(T);
			}
			return this.Data.ReadUnsafe(this._position);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000036AA File Offset: 0x000018AA
		public override void Flush()
		{
			throw new NotImplementedException();
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000036AA File Offset: 0x000018AA
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000036AA File Offset: 0x000018AA
		public override void WriteByte(byte value)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0400004B RID: 75
		public const int MaxPacketSize = 6291456;

		// Token: 0x0400004C RID: 76
		public const int MaxBufferSize = 8388608;

		// Token: 0x0400004D RID: 77
		public byte[] Data;

		// Token: 0x0400004E RID: 78
		private int _length;

		// Token: 0x0400004F RID: 79
		private int _position;

		// Token: 0x04000050 RID: 80
		public ulong guid;

		// Token: 0x04000051 RID: 81
		public string ipaddress;

		// Token: 0x04000052 RID: 82
		public Connection connection;

		// Token: 0x04000053 RID: 83
		private const int bufferSize = 8388608;

		// Token: 0x04000054 RID: 84
		private static byte[] byteBuffer = new byte[8388608];

		// Token: 0x04000055 RID: 85
		private static char[] charBuffer = new char[8388608];
	}
}
