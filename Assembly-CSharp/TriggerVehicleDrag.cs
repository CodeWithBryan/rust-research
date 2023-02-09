﻿using System;
using UnityEngine;

// Token: 0x020002D2 RID: 722
public class TriggerVehicleDrag : TriggerBase, IServerComponent
{
	// Token: 0x06001CD1 RID: 7377 RVA: 0x000C5844 File Offset: 0x000C3A44
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
		if (this.losEyes != null)
		{
			if (this.entityContents != null && this.entityContents.Contains(baseEntity))
			{
				return baseEntity.gameObject;
			}
			if (!baseEntity.IsVisible(this.losEyes.transform.position, baseEntity.CenterPoint(), float.PositiveInfinity))
			{
				return null;
			}
		}
		return baseEntity.gameObject;
	}

	// Token: 0x0400167E RID: 5758
	[Tooltip("If set, the entering object must have line of sight to this transform to be added, note this is only checked on entry")]
	public Transform losEyes;

	// Token: 0x0400167F RID: 5759
	public float vehicleDrag;
}
