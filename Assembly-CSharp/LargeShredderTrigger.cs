using System;
using UnityEngine;

// Token: 0x02000415 RID: 1045
public class LargeShredderTrigger : TriggerBase
{
	// Token: 0x060022F8 RID: 8952 RVA: 0x000DEE10 File Offset: 0x000DD010
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

	// Token: 0x060022F9 RID: 8953 RVA: 0x000DEE6C File Offset: 0x000DD06C
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		this.shredder.OnEntityEnteredTrigger(ent);
	}

	// Token: 0x04001B61 RID: 7009
	public LargeShredder shredder;
}
