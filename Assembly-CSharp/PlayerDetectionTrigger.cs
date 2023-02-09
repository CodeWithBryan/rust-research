using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class PlayerDetectionTrigger : TriggerBase
{
	// Token: 0x06001554 RID: 5460 RVA: 0x000A6224 File Offset: 0x000A4424
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

	// Token: 0x06001555 RID: 5461 RVA: 0x000A6267 File Offset: 0x000A4467
	internal override void OnObjects()
	{
		base.OnObjects();
		this.myDetector.OnObjects();
	}

	// Token: 0x06001556 RID: 5462 RVA: 0x000A627A File Offset: 0x000A447A
	internal override void OnEmpty()
	{
		base.OnEmpty();
		this.myDetector.OnEmpty();
	}

	// Token: 0x04000DC6 RID: 3526
	public BaseDetector myDetector;
}
