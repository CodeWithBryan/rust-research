using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class TriggerVehiclePush : TriggerBase, IServerComponent
{
	// Token: 0x060000CB RID: 203 RVA: 0x0000636C File Offset: 0x0000456C
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
		if (baseEntity is BuildingBlock)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x060000CC RID: 204 RVA: 0x000063B9 File Offset: 0x000045B9
	public int ContentsCount
	{
		get
		{
			HashSet<BaseEntity> entityContents = this.entityContents;
			if (entityContents == null)
			{
				return 0;
			}
			return entityContents.Count;
		}
	}

	// Token: 0x060000CD RID: 205 RVA: 0x000063CC File Offset: 0x000045CC
	public void FixedUpdate()
	{
		if (this.thisEntity == null)
		{
			return;
		}
		if (this.entityContents == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		foreach (BaseEntity baseEntity in this.entityContents)
		{
			if (baseEntity.IsValid() && !baseEntity.EqualNetID(this.thisEntity))
			{
				Rigidbody rigidbody = baseEntity.GetComponent<Rigidbody>();
				if (rigidbody == null && this.allowParentRigidbody)
				{
					rigidbody = baseEntity.GetComponentInParent<Rigidbody>();
				}
				if (rigidbody && !rigidbody.isKinematic)
				{
					float value = Vector3Ex.Distance2D(this.useRigidbodyPosition ? rigidbody.transform.position : baseEntity.transform.position, base.transform.position);
					float d = 1f - Mathf.InverseLerp(this.minRadius, this.maxRadius, value);
					float num = 1f - Mathf.InverseLerp(this.minRadius - 1f, this.minRadius, value);
					Vector3 vector = baseEntity.ClosestPoint(position);
					Vector3 vector2 = Vector3Ex.Direction2D(vector, position);
					vector2 = Vector3Ex.Direction2D(this.useCentreOfMass ? rigidbody.worldCenterOfMass : vector, position);
					if (this.snapToAxis)
					{
						Vector3 from = base.transform.InverseTransformDirection(vector2);
						if (Vector3.Angle(from, this.axisToSnapTo) < Vector3.Angle(from, -this.axisToSnapTo))
						{
							vector2 = base.transform.TransformDirection(this.axisToSnapTo);
						}
						else
						{
							vector2 = -base.transform.TransformDirection(this.axisToSnapTo);
						}
					}
					rigidbody.AddForceAtPosition(vector2 * this.maxPushVelocity * d, vector, ForceMode.Acceleration);
					if (num > 0f)
					{
						rigidbody.AddForceAtPosition(vector2 * 1f * num, vector, ForceMode.VelocityChange);
					}
				}
			}
		}
	}

	// Token: 0x060000CE RID: 206 RVA: 0x000065E4 File Offset: 0x000047E4
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.minRadius);
		Gizmos.color = new Color(0.5f, 0f, 0f, 1f);
		Gizmos.DrawWireSphere(base.transform.position, this.maxRadius);
		if (this.snapToAxis)
		{
			Gizmos.color = Color.cyan;
			Vector3 b = base.transform.TransformDirection(this.axisToSnapTo);
			Gizmos.DrawLine(base.transform.position + b, base.transform.position - b);
		}
	}

	// Token: 0x040000F8 RID: 248
	public BaseEntity thisEntity;

	// Token: 0x040000F9 RID: 249
	public float maxPushVelocity = 10f;

	// Token: 0x040000FA RID: 250
	public float minRadius;

	// Token: 0x040000FB RID: 251
	public float maxRadius;

	// Token: 0x040000FC RID: 252
	public bool snapToAxis;

	// Token: 0x040000FD RID: 253
	public Vector3 axisToSnapTo = Vector3.right;

	// Token: 0x040000FE RID: 254
	public bool allowParentRigidbody;

	// Token: 0x040000FF RID: 255
	public bool useRigidbodyPosition;

	// Token: 0x04000100 RID: 256
	public bool useCentreOfMass;
}
