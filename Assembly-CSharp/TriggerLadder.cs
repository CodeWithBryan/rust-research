using System;
using UnityEngine;

// Token: 0x0200055D RID: 1373
public class TriggerLadder : TriggerBase
{
	// Token: 0x060029C3 RID: 10691 RVA: 0x000FD590 File Offset: 0x000FB790
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity as BasePlayer == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
