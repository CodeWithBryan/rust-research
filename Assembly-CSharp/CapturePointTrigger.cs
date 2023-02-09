using System;
using UnityEngine;

// Token: 0x020004E4 RID: 1252
public class CapturePointTrigger : TriggerBase
{
	// Token: 0x060027D0 RID: 10192 RVA: 0x000F3DAC File Offset: 0x000F1FAC
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
		if (baseEntity.isClient)
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
