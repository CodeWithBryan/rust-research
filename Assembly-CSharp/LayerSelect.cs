using System;
using UnityEngine;

// Token: 0x02000930 RID: 2352
[Serializable]
public struct LayerSelect
{
	// Token: 0x06003806 RID: 14342 RVA: 0x0014BD54 File Offset: 0x00149F54
	public LayerSelect(int layer)
	{
		this.layer = layer;
	}

	// Token: 0x06003807 RID: 14343 RVA: 0x0014BD5D File Offset: 0x00149F5D
	public static implicit operator int(LayerSelect layer)
	{
		return layer.layer;
	}

	// Token: 0x06003808 RID: 14344 RVA: 0x0014BD65 File Offset: 0x00149F65
	public static implicit operator LayerSelect(int layer)
	{
		return new LayerSelect(layer);
	}

	// Token: 0x1700043A RID: 1082
	// (get) Token: 0x06003809 RID: 14345 RVA: 0x0014BD6D File Offset: 0x00149F6D
	public int Mask
	{
		get
		{
			return 1 << this.layer;
		}
	}

	// Token: 0x1700043B RID: 1083
	// (get) Token: 0x0600380A RID: 14346 RVA: 0x0014BD7A File Offset: 0x00149F7A
	public string Name
	{
		get
		{
			return LayerMask.LayerToName(this.layer);
		}
	}

	// Token: 0x0400321B RID: 12827
	[SerializeField]
	private int layer;
}
