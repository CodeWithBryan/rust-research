using System;
using UnityEngine;

// Token: 0x02000558 RID: 1368
public class TriggerEnsnare : TriggerBase
{
	// Token: 0x060029AC RID: 10668 RVA: 0x000FCE6C File Offset: 0x000FB06C
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
		return baseEntity.gameObject;
	}

	// Token: 0x040021B7 RID: 8631
	public bool blockHands = true;
}
