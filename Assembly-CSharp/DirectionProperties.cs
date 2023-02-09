using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class DirectionProperties : PrefabAttribute
{
	// Token: 0x06001B2A RID: 6954 RVA: 0x000BE13E File Offset: 0x000BC33E
	protected override Type GetIndexedType()
	{
		return typeof(DirectionProperties);
	}

	// Token: 0x06001B2B RID: 6955 RVA: 0x000BE14C File Offset: 0x000BC34C
	public bool IsWeakspot(Transform tx, HitInfo info)
	{
		if (this.bounds.size == Vector3.zero)
		{
			return false;
		}
		BasePlayer initiatorPlayer = info.InitiatorPlayer;
		if (initiatorPlayer == null)
		{
			return false;
		}
		BaseEntity hitEntity = info.HitEntity;
		if (hitEntity == null)
		{
			return false;
		}
		Matrix4x4 worldToLocalMatrix = tx.worldToLocalMatrix;
		Vector3 b = worldToLocalMatrix.MultiplyPoint3x4(info.PointStart) - this.worldPosition;
		float num = this.worldForward.DotDegrees(b);
		Vector3 target = worldToLocalMatrix.MultiplyPoint3x4(info.HitPositionWorld);
		OBB obb = new OBB(this.worldPosition, this.worldRotation, this.bounds);
		Vector3 position = initiatorPlayer.eyes.position;
		WeakpointProperties[] array = PrefabAttribute.server.FindAll<WeakpointProperties>(hitEntity.prefabID);
		if (array != null && array.Length != 0)
		{
			bool flag = false;
			foreach (WeakpointProperties weakpointProperties in array)
			{
				if ((!weakpointProperties.BlockWhenRoofAttached || this.CheckWeakpointRoof(hitEntity)) && this.IsWeakspotVisible(hitEntity, position, tx.TransformPoint(weakpointProperties.worldPosition)))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		else if (!this.IsWeakspotVisible(hitEntity, position, tx.TransformPoint(obb.position)))
		{
			return false;
		}
		return num > 100f && obb.Contains(target);
	}

	// Token: 0x06001B2C RID: 6956 RVA: 0x000BE298 File Offset: 0x000BC498
	private bool CheckWeakpointRoof(BaseEntity hitEntity)
	{
		foreach (EntityLink entityLink in hitEntity.GetEntityLinks(true))
		{
			if (entityLink.socket is NeighbourSocket)
			{
				using (List<EntityLink>.Enumerator enumerator2 = entityLink.connections.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						BuildingBlock buildingBlock;
						if ((buildingBlock = (enumerator2.Current.owner as BuildingBlock)) != null && (buildingBlock.ShortPrefabName == "roof" || buildingBlock.ShortPrefabName == "roof.triangle"))
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06001B2D RID: 6957 RVA: 0x000BE36C File Offset: 0x000BC56C
	private bool IsWeakspotVisible(BaseEntity hitEntity, Vector3 playerEyes, Vector3 weakspotPos)
	{
		return hitEntity.IsVisible(playerEyes, weakspotPos, float.PositiveInfinity);
	}

	// Token: 0x04001438 RID: 5176
	private const float radius = 200f;

	// Token: 0x04001439 RID: 5177
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	// Token: 0x0400143A RID: 5178
	public ProtectionProperties extraProtection;
}
