using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using UnityEngine;

namespace Network
{
	// Token: 0x02000012 RID: 18
	public class NetWrite : Stream, Pool.IPooled
	{
		// Token: 0x060000AB RID: 171 RVA: 0x000036D9 File Offset: 0x000018D9
		public void EnterPool()
		{
			this.peer = null;
			this.connections.Clear();
			if (this.Data != null)
			{
				ArrayPool<byte>.Shared.Return(this.Data, false);
				this.Data = null;
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00002209 File Offset: 0x00000409
		public void LeavePool()
		{
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000370D File Offset: 0x0000190D
		public bool Start(BaseNetwork peer)
		{
			this.peer = peer;
			this.connections.Clear();
			this._position = 0;
			this._length = 0;
			return true;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00003730 File Offset: 0x00001930
		public void Send(SendInfo info)
		{
			this.method = info.method;
			this.channel = info.channel;
			this.priority = info.priority;
			if (info.connections != null)
			{
				this.connections.AddRange(info.connections);
			}
			if (info.connection != null)
			{
				this.connections.Add(info.connection);
			}
			if (BaseNetwork.Multithreading)
			{
				this.peer.EnqueueWrite(this);
				return;
			}
			this.peer.ProcessWrite(this);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000037B3 File Offset: 0x000019B3
		public byte PeekPacketID()
		{
			if (this._length <= 0)
			{
				return 0;
			}
			return this.Data[0];
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000037C8 File Offset: 0x000019C8
		public void PacketID(Message.Type val)
		{
			byte val2 = (byte)(val + 140);
			this.UInt8(val2);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000037E7 File Offset: 0x000019E7
		public void UInt8(byte val)
		{
			this.Write<byte>(val);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000037F1 File Offset: 0x000019F1
		public void UInt16(ushort val)
		{
			this.Write<ushort>(val);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000037FB File Offset: 0x000019FB
		public void UInt32(uint val)
		{
			this.Write<uint>(val);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00003805 File Offset: 0x00001A05
		public void UInt64(ulong val)
		{
			this.Write<ulong>(val);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000380F File Offset: 0x00001A0F
		public void Int8(sbyte val)
		{
			this.Write<sbyte>(val);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00003819 File Offset: 0x00001A19
		public void Int16(short val)
		{
			this.Write<short>(val);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00003823 File Offset: 0x00001A23
		public void Int32(int val)
		{
			this.Write<int>(val);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x0000382D File Offset: 0x00001A2D
		public void Int64(long val)
		{
			this.Write<long>(val);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00003838 File Offset: 0x00001A38
		public void Bool(bool val)
		{
			byte b = val ? 1 : 0;
			this.Write<byte>(b);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00003855 File Offset: 0x00001A55
		public void Float(float val)
		{
			this.Write<float>(val);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000385F File Offset: 0x00001A5F
		public void Double(double val)
		{
			this.Write<double>(val);
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00003869 File Offset: 0x00001A69
		public void Bytes(byte[] val)
		{
			this.Write(val, 0, val.Length);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00003878 File Offset: 0x00001A78
		public void String(string val)
		{
			if (string.IsNullOrEmpty(val))
			{
				this.BytesWithSize(null);
				return;
			}
			if (NetWrite.stringBuffer.Capacity < val.Length * 8)
			{
				NetWrite.stringBuffer.Capacity = val.Length * 8;
			}
			NetWrite.stringBuffer.Position = 0L;
			NetWrite.stringBuffer.SetLength((long)NetWrite.stringBuffer.Capacity);
			int bytes = Encoding.UTF8.GetBytes(val, 0, val.Length, NetWrite.stringBuffer.GetBuffer(), 0);
			NetWrite.stringBuffer.SetLength((long)bytes);
			this.BytesWithSize(NetWrite.stringBuffer);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00003911 File Offset: 0x00001B11
		public void Vector3(in Vector3 obj)
		{
			this.Float(obj.x);
			this.Float(obj.y);
			this.Float(obj.z);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00003937 File Offset: 0x00001B37
		public void Quaternion(in Quaternion obj)
		{
			this.Float(obj.x);
			this.Float(obj.y);
			this.Float(obj.z);
			this.Float(obj.w);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x0000396C File Offset: 0x00001B6C
		public void Ray(in Ray obj)
		{
			Ray ray = obj;
			Vector3 vector = ray.origin;
			this.Vector3(vector);
			ray = obj;
			vector = ray.direction;
			this.Vector3(vector);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000039A7 File Offset: 0x00001BA7
		public void Color(in Color obj)
		{
			this.Float(obj.r);
			this.Float(obj.g);
			this.Float(obj.b);
			this.Float(obj.a);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000039D9 File Offset: 0x00001BD9
		public void EntityID(uint id)
		{
			this.UInt32(id);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000039D9 File Offset: 0x00001BD9
		public void GroupID(uint id)
		{
			this.UInt32(id);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000039E2 File Offset: 0x00001BE2
		public void BytesWithSize(MemoryStream val)
		{
			if (val == null || val.Length == 0L)
			{
				this.UInt32(0U);
				return;
			}
			this.BytesWithSize(val.GetBuffer(), (int)val.Length);
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00003A0A File Offset: 0x00001C0A
		public void BytesWithSize(byte[] b)
		{
			this.BytesWithSize(b, b.Length);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00003A18 File Offset: 0x00001C18
		public void BytesWithSize(byte[] b, int length)
		{
			if (b == null || b.Length == 0 || length == 0)
			{
				this.UInt32(0U);
				return;
			}
			if (length > 10485760)
			{
				this.UInt32(0U);
				Debug.LogError("BytesWithSize: Too big " + length);
				return;
			}
			this.UInt32((uint)length);
			this.Write(b, 0, length);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00003A6C File Offset: 0x00001C6C
		private void Write<[IsUnmanaged] T>(in T val) where T : struct, ValueType
		{
			int num = sizeof(T);
			this.EnsureCapacity(num);
			this.Data.WriteUnsafe(val, this._position);
			this._position += num;
			if (this._position > this._length)
			{
				this._length = this._position;
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00003AC4 File Offset: 0x00001CC4
		private void EnsureCapacity(int spaceRequired)
		{
			if (this.Data == null)
			{
				int num = (spaceRequired <= 16384) ? 16384 : spaceRequired;
				int num2 = Mathf.NextPowerOfTwo(num);
				if (num2 > 4194304)
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
				if (num4 > 4194304)
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

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00002ED8 File Offset: 0x000010D8
		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00002ED8 File Offset: 0x000010D8
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00002ED8 File Offset: 0x000010D8
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00003BAA File Offset: 0x00001DAA
		public override long Length
		{
			get
			{
				return (long)this._length;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00003BB3 File Offset: 0x00001DB3
		// (set) Token: 0x060000CE RID: 206 RVA: 0x00003BBC File Offset: 0x00001DBC
		public override long Position
		{
			get
			{
				return (long)this._position;
			}
			set
			{
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00002209 File Offset: 0x00000409
		public override void Flush()
		{
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00003BC8 File Offset: 0x00001DC8
		public override int Read(byte[] buffer, int offset, int count)
		{
			int max = this._length - this._position;
			int num = Mathf.Clamp(count, 0, max);
			Buffer.BlockCopy(this.Data, this._position, buffer, offset, count);
			this._position += num;
			return num;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00003C10 File Offset: 0x00001E10
		public override int ReadByte()
		{
			if (this._position >= this._length)
			{
				return -1;
			}
			byte[] data = this.Data;
			int position = this._position;
			this._position = position + 1;
			return data[position];
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00003C48 File Offset: 0x00001E48
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.EnsureCapacity(count);
			Buffer.BlockCopy(buffer, offset, this.Data, this._position, count);
			this._position += count;
			if (this._position > this._length)
			{
				this._length = this._position;
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00003C98 File Offset: 0x00001E98
		public override void WriteByte(byte value)
		{
			this.UInt8(value);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00003CA4 File Offset: 0x00001EA4
		public override long Seek(long offset, SeekOrigin origin)
		{
			int num;
			switch (origin)
			{
			default:
				num = (int)offset;
				break;
			case SeekOrigin.Current:
				num = this._position + (int)offset;
				break;
			case SeekOrigin.End:
				num = this._length + (int)offset;
				break;
			}
			if (num < 0 || num > this._length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			this._position = num;
			return (long)this._position;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000036AA File Offset: 0x000018AA
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000056 RID: 86
		private static MemoryStream stringBuffer = new MemoryStream();

		// Token: 0x04000057 RID: 87
		private BaseNetwork peer;

		// Token: 0x04000058 RID: 88
		public byte[] Data;

		// Token: 0x04000059 RID: 89
		private int _position;

		// Token: 0x0400005A RID: 90
		private int _length;

		// Token: 0x0400005B RID: 91
		public SendMethod method;

		// Token: 0x0400005C RID: 92
		public sbyte channel;

		// Token: 0x0400005D RID: 93
		public Priority priority;

		// Token: 0x0400005E RID: 94
		public List<Connection> connections = new List<Connection>();
	}
}
