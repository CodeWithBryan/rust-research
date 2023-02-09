using System;
using UnityEngine;

// Token: 0x02000559 RID: 1369
public class TriggerForce : TriggerBase, IServerComponent
{
	// Token: 0x060029AE RID: 10670 RVA: 0x000FCEB4 File Offset: 0x000FB0B4
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

	// Token: 0x060029AF RID: 10671 RVA: 0x000FCEF8 File Offset: 0x000FB0F8
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		Vector3 vector = base.transform.TransformDirection(this.velocity);
		ent.ApplyInheritedVelocity(vector);
	}

	// Token: 0x060029B0 RID: 10672 RVA: 0x000FCF25 File Offset: 0x000FB125
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		ent.ApplyInheritedVelocity(Vector3.zero);
	}

	// Token: 0x060029B1 RID: 10673 RVA: 0x000FCF3C File Offset: 0x000FB13C
	protected void FixedUpdate()
	{
		if (this.entityContents != null)
		{
			Vector3 vector = base.transform.TransformDirection(this.velocity);
			foreach (BaseEntity baseEntity in this.entityContents)
			{
				if (baseEntity != null)
				{
					baseEntity.ApplyInheritedVelocity(vector);
				}
			}
		}
	}

	// Token: 0x040021B8 RID: 8632
	public const float GravityMultiplier = 0.1f;

	// Token: 0x040021B9 RID: 8633
	public const float VelocityLerp = 10f;

	// Token: 0x040021BA RID: 8634
	public const float AngularDrag = 10f;

	// Token: 0x040021BB RID: 8635
	public Vector3 velocity = Vector3.forward;
}
