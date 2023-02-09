using System;
using UnityEngine;

// Token: 0x0200056E RID: 1390
public class TriggerSubmarineMoonpool : TriggerBase, IServerComponent
{
	// Token: 0x06002A13 RID: 10771 RVA: 0x000FE7A8 File Offset: 0x000FC9A8
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
		BaseSubmarine baseSubmarine;
		if (baseEntity.isServer && (baseSubmarine = (baseEntity as BaseSubmarine)) != null)
		{
			return baseSubmarine.gameObject;
		}
		return null;
	}

	// Token: 0x06002A14 RID: 10772 RVA: 0x000FE7F8 File Offset: 0x000FC9F8
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		BaseSubmarine baseSubmarine;
		if ((baseSubmarine = (ent as BaseSubmarine)) != null)
		{
			baseSubmarine.OnSurfacedInMoonpool();
		}
	}
}
