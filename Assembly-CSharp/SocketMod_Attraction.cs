using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000259 RID: 601
public class SocketMod_Attraction : SocketMod
{
	// Token: 0x06001B7E RID: 7038 RVA: 0x000BF6A0 File Offset: 0x000BD8A0
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
		Gizmos.DrawSphere(Vector3.zero, this.outerRadius);
		Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
		Gizmos.DrawSphere(Vector3.zero, this.innerRadius);
	}

	// Token: 0x06001B7F RID: 7039 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool DoCheck(Construction.Placement place)
	{
		return true;
	}

	// Token: 0x06001B80 RID: 7040 RVA: 0x000BF71C File Offset: 0x000BD91C
	public override void ModifyPlacement(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(vector, this.outerRadius * 2f, list, -1, QueryTriggerInteraction.Collide);
		Vector3 position = Vector3.zero;
		float num = float.MaxValue;
		Vector3 position2 = place.position;
		Quaternion rotation = Quaternion.identity;
		foreach (BaseEntity baseEntity in list)
		{
			if (baseEntity.isServer == this.isServer)
			{
				AttractionPoint[] array = this.prefabAttribute.FindAll<AttractionPoint>(baseEntity.prefabID);
				if (array != null)
				{
					foreach (AttractionPoint attractionPoint in array)
					{
						if (!(attractionPoint.groupName != this.groupName))
						{
							Vector3 a = baseEntity.transform.position + baseEntity.transform.rotation * attractionPoint.worldPosition;
							float magnitude = (a - vector).magnitude;
							if (this.ignoreRotationForRadiusCheck)
							{
								Vector3 vector2 = baseEntity.transform.TransformPoint(Vector3.LerpUnclamped(Vector3.zero, attractionPoint.worldPosition.WithY(0f), 2f));
								float num2 = Vector3.Distance(vector2, position2);
								if (num2 < num)
								{
									num = num2;
									position = vector2;
									rotation = baseEntity.transform.rotation;
								}
							}
							if (magnitude <= this.outerRadius)
							{
								Quaternion b = QuaternionEx.LookRotationWithOffset(this.worldPosition, a - place.position, Vector3.up);
								float num3 = Mathf.InverseLerp(this.outerRadius, this.innerRadius, magnitude);
								if (this.lockRotation)
								{
									num3 = 1f;
								}
								if (this.lockRotation)
								{
									Vector3 vector3 = place.rotation.eulerAngles;
									vector3 -= new Vector3(vector3.x % 90f, vector3.y % 90f, vector3.z % 90f);
									place.rotation = Quaternion.Euler(vector3 + baseEntity.transform.eulerAngles);
								}
								else
								{
									place.rotation = Quaternion.Lerp(place.rotation, b, num3);
								}
								vector = place.position + place.rotation * this.worldPosition;
								Vector3 a2 = a - vector;
								place.position += a2 * num3;
							}
						}
					}
				}
			}
		}
		if (num < 3.4028235E+38f && this.ignoreRotationForRadiusCheck)
		{
			place.position = position;
			place.rotation = rotation;
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	// Token: 0x04001485 RID: 5253
	public float outerRadius = 1f;

	// Token: 0x04001486 RID: 5254
	public float innerRadius = 0.1f;

	// Token: 0x04001487 RID: 5255
	public string groupName = "wallbottom";

	// Token: 0x04001488 RID: 5256
	public bool lockRotation;

	// Token: 0x04001489 RID: 5257
	public bool ignoreRotationForRadiusCheck;
}
