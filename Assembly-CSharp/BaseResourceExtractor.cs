using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020003DF RID: 991
public class BaseResourceExtractor : BaseCombatEntity
{
	// Token: 0x060021A1 RID: 8609 RVA: 0x000D7D8C File Offset: 0x000D5F8C
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		List<SurveyCrater> list = Pool.GetList<SurveyCrater>();
		Vis.Entities<SurveyCrater>(base.transform.position, 3f, list, 1, QueryTriggerInteraction.Collide);
		foreach (SurveyCrater surveyCrater in list)
		{
			if (surveyCrater.isServer)
			{
				surveyCrater.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<SurveyCrater>(ref list);
	}

	// Token: 0x04001A03 RID: 6659
	public bool canExtractLiquid;

	// Token: 0x04001A04 RID: 6660
	public bool canExtractSolid = true;
}
