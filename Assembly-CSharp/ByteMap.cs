using System;
using UnityEngine;

// Token: 0x02000620 RID: 1568
[Serializable]
public class ByteMap
{
	// Token: 0x06002CF6 RID: 11510 RVA: 0x0010DBA8 File Offset: 0x0010BDA8
	public ByteMap(int size, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		this.values = new byte[bytes * size * size];
	}

	// Token: 0x06002CF7 RID: 11511 RVA: 0x0010DBCE File Offset: 0x0010BDCE
	public ByteMap(int size, byte[] values, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		this.values = values;
	}

	// Token: 0x17000370 RID: 880
	// (get) Token: 0x06002CF8 RID: 11512 RVA: 0x0010DBEB File Offset: 0x0010BDEB
	public int Size
	{
		get
		{
			return this.size;
		}
	}

	// Token: 0x17000371 RID: 881
	public uint this[int x, int y]
	{
		get
		{
			int num = y * this.bytes * this.size + x * this.bytes;
			switch (this.bytes)
			{
			case 1:
				return (uint)this.values[num];
			case 2:
			{
				uint num2 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				return num2 << 8 | num3;
			}
			case 3:
			{
				uint num4 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				uint num5 = (uint)this.values[num + 2];
				return num4 << 16 | num3 << 8 | num5;
			}
			default:
			{
				uint num6 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				uint num5 = (uint)this.values[num + 2];
				uint num7 = (uint)this.values[num + 3];
				return num6 << 24 | num3 << 16 | num5 << 8 | num7;
			}
			}
		}
		set
		{
			int num = y * this.bytes * this.size + x * this.bytes;
			switch (this.bytes)
			{
			case 1:
				this.values[num] = (byte)(value & 255U);
				return;
			case 2:
				this.values[num] = (byte)(value >> 8 & 255U);
				this.values[num + 1] = (byte)(value & 255U);
				return;
			case 3:
				this.values[num] = (byte)(value >> 16 & 255U);
				this.values[num + 1] = (byte)(value >> 8 & 255U);
				this.values[num + 2] = (byte)(value & 255U);
				return;
			default:
				this.values[num] = (byte)(value >> 24 & 255U);
				this.values[num + 1] = (byte)(value >> 16 & 255U);
				this.values[num + 2] = (byte)(value >> 8 & 255U);
				this.values[num + 3] = (byte)(value & 255U);
				return;
			}
		}
	}

	// Token: 0x040024EE RID: 9454
	[SerializeField]
	private int size;

	// Token: 0x040024EF RID: 9455
	[SerializeField]
	private int bytes;

	// Token: 0x040024F0 RID: 9456
	[SerializeField]
	private byte[] values;
}
