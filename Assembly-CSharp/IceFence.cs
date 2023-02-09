using System;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class IceFence : GraveyardFence
{
	// Token: 0x060016C7 RID: 5831 RVA: 0x000ABFD8 File Offset: 0x000AA1D8
	public int GetStyleFromID()
	{
		uint id = this.net.ID;
		return SeedRandom.Range(ref id, 0, this.styles.Length);
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x000AC001 File Offset: 0x000AA201
	public override void ServerInit()
	{
		base.ServerInit();
		this.InitStyle();
		this.UpdatePillars();
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x000AC015 File Offset: 0x000AA215
	public void InitStyle()
	{
		if (this.init)
		{
			return;
		}
		this.SetStyle(this.GetStyleFromID());
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x000AC02C File Offset: 0x000AA22C
	public void SetStyle(int style)
	{
		GameObject[] array = this.styles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		this.styles[style].gameObject.SetActive(true);
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x000AC06F File Offset: 0x000AA26F
	public override void UpdatePillars()
	{
		base.UpdatePillars();
	}

	// Token: 0x04000FD3 RID: 4051
	public GameObject[] styles;

	// Token: 0x04000FD4 RID: 4052
	private bool init;

	// Token: 0x04000FD5 RID: 4053
	public AdaptMeshToTerrain snowMesh;
}
