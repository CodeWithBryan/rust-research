using System;
using UnityEngine;

// Token: 0x02000567 RID: 1383
public class TriggerParentExclusion : TriggerBase, IServerComponent
{
	// Token: 0x060029ED RID: 10733 RVA: 0x000FDF48 File Offset: 0x000FC148
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
