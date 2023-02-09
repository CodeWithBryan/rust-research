using System;
using UnityEngine;

// Token: 0x020003E5 RID: 997
public class TargetTrigger : TriggerBase
{
	// Token: 0x060021C7 RID: 8647 RVA: 0x000D8BF8 File Offset: 0x000D6DF8
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
		if (this.losEyes != null && !baseEntity.IsVisible(this.losEyes.transform.position, baseEntity.CenterPoint(), float.PositiveInfinity))
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x04001A1F RID: 6687
	[Tooltip("If set, the entering object must have line of sight to this transform to be added, note this is only checked on entry")]
	public Transform losEyes;
}
