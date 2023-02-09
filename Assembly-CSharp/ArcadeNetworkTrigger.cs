using System;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class ArcadeNetworkTrigger : TriggerBase
{
	// Token: 0x06001616 RID: 5654 RVA: 0x000A8C70 File Offset: 0x000A6E70
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
		return baseEntity.gameObject;
	}
}
