using System;
using UnityEngine;

// Token: 0x02000452 RID: 1106
public class CrushTrigger : TriggerHurt
{
	// Token: 0x06002441 RID: 9281 RVA: 0x000E5A54 File Offset: 0x000E3C54
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
		if (!this.includeNPCs && baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002442 RID: 9282 RVA: 0x000E5AA9 File Offset: 0x000E3CA9
	protected override bool CanHurt(BaseCombatEntity ent)
	{
		return (!this.requireCentreBelowPosition || ent.CenterPoint().y <= base.transform.position.y) && base.CanHurt(ent);
	}

	// Token: 0x04001CE8 RID: 7400
	public bool includeNPCs = true;

	// Token: 0x04001CE9 RID: 7401
	public bool requireCentreBelowPosition;
}
