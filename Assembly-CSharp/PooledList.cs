using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x02000918 RID: 2328
public class PooledList<T>
{
	// Token: 0x0600377B RID: 14203 RVA: 0x00149562 File Offset: 0x00147762
	public void Alloc()
	{
		if (this.data == null)
		{
			this.data = Pool.GetList<T>();
		}
	}

	// Token: 0x0600377C RID: 14204 RVA: 0x00149577 File Offset: 0x00147777
	public void Free()
	{
		if (this.data != null)
		{
			Pool.FreeList<T>(ref this.data);
		}
	}

	// Token: 0x0600377D RID: 14205 RVA: 0x0014958C File Offset: 0x0014778C
	public void Clear()
	{
		if (this.data != null)
		{
			this.data.Clear();
		}
	}

	// Token: 0x040031CB RID: 12747
	public List<T> data;
}
