using System;
using UnityEngine;

// Token: 0x0200046C RID: 1132
public class TriggerMagnet : TriggerBase
{
	// Token: 0x060024E8 RID: 9448 RVA: 0x000E85C4 File Offset: 0x000E67C4
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
		if (!baseEntity.syncPosition)
		{
			return null;
		}
		if (!baseEntity.GetComponent<MagnetLiftable>())
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
