using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

// Token: 0x02000672 RID: 1650
public abstract class TerrainMap<T> : TerrainMap where T : struct
{
	// Token: 0x06002ED8 RID: 11992 RVA: 0x00118E85 File Offset: 0x00117085
	public void Push()
	{
		if (this.src != this.dst)
		{
			return;
		}
		this.dst = (T[])this.src.Clone();
	}

	// Token: 0x06002ED9 RID: 11993 RVA: 0x00118EAC File Offset: 0x001170AC
	public void Pop()
	{
		if (this.src == this.dst)
		{
			return;
		}
		Array.Copy(this.dst, this.src, this.src.Length);
		this.dst = this.src;
	}

	// Token: 0x06002EDA RID: 11994 RVA: 0x00118EE2 File Offset: 0x001170E2
	public IEnumerable<T> ToEnumerable()
	{
		return this.src.Cast<T>();
	}

	// Token: 0x06002EDB RID: 11995 RVA: 0x00118EEF File Offset: 0x001170EF
	public int BytesPerElement()
	{
		return Marshal.SizeOf(typeof(T));
	}

	// Token: 0x06002EDC RID: 11996 RVA: 0x00118F00 File Offset: 0x00117100
	public long GetMemoryUsage()
	{
		return (long)this.BytesPerElement() * (long)this.src.Length;
	}

	// Token: 0x06002EDD RID: 11997 RVA: 0x00118F14 File Offset: 0x00117114
	public byte[] ToByteArray()
	{
		byte[] array = new byte[this.BytesPerElement() * this.src.Length];
		Buffer.BlockCopy(this.src, 0, array, 0, array.Length);
		return array;
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x00118F48 File Offset: 0x00117148
	public void FromByteArray(byte[] dat)
	{
		Buffer.BlockCopy(dat, 0, this.dst, 0, dat.Length);
	}

	// Token: 0x0400262C RID: 9772
	internal T[] src;

	// Token: 0x0400262D RID: 9773
	internal T[] dst;
}
