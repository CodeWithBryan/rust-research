using System;
using UnityEngine;

// Token: 0x0200046B RID: 1131
public class MagnetLiftable : EntityComponent<BaseEntity>
{
	// Token: 0x170002CC RID: 716
	// (get) Token: 0x060024E4 RID: 9444 RVA: 0x000E8597 File Offset: 0x000E6797
	// (set) Token: 0x060024E5 RID: 9445 RVA: 0x000E859F File Offset: 0x000E679F
	public BasePlayer associatedPlayer { get; private set; }

	// Token: 0x060024E6 RID: 9446 RVA: 0x000E85A8 File Offset: 0x000E67A8
	public virtual void SetMagnetized(bool wantsOn, BaseMagnet magnetSource, BasePlayer player)
	{
		this.associatedPlayer = player;
	}

	// Token: 0x04001D9F RID: 7583
	public ItemAmount[] shredResources;

	// Token: 0x04001DA0 RID: 7584
	public Vector3 shredDirection = Vector3.forward;
}
