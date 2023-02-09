using System;
using UnityEngine;

// Token: 0x020005E8 RID: 1512
[CreateAssetMenu(menuName = "Rust/Missions/MoveMission")]
public class MoveMission : BaseMission
{
	// Token: 0x06002C64 RID: 11364 RVA: 0x0010A010 File Offset: 0x00108210
	public override void MissionStart(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
		onUnitSphere.y = 0f;
		onUnitSphere.Normalize();
		Vector3 vector = assignee.transform.position + onUnitSphere * UnityEngine.Random.Range(this.minDistForMovePoint, this.maxDistForMovePoint);
		float b = vector.y;
		float a = vector.y;
		if (TerrainMeta.WaterMap != null)
		{
			a = TerrainMeta.WaterMap.GetHeight(vector);
		}
		if (TerrainMeta.HeightMap != null)
		{
			b = TerrainMeta.HeightMap.GetHeight(vector);
		}
		vector.y = Mathf.Max(a, b);
		instance.missionLocation = vector;
		base.MissionStart(instance, assignee);
	}

	// Token: 0x06002C65 RID: 11365 RVA: 0x0010A0BB File Offset: 0x001082BB
	public override void MissionEnded(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		base.MissionEnded(instance, assignee);
	}

	// Token: 0x06002C66 RID: 11366 RVA: 0x0010A0C5 File Offset: 0x001082C5
	public override Sprite GetIcon(BaseMission.MissionInstance instance)
	{
		if (instance.status != BaseMission.MissionStatus.Accomplished)
		{
			return this.icon;
		}
		return this.providerIcon;
	}

	// Token: 0x06002C67 RID: 11367 RVA: 0x0010A0E0 File Offset: 0x001082E0
	public override void Think(BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		float num = Vector3.Distance(instance.missionLocation, assignee.transform.position);
		if (instance.status == BaseMission.MissionStatus.Active && num <= this.minDistFromLocation)
		{
			this.MissionSuccess(instance, assignee);
			BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(instance.providerID);
			if (baseNetworkable)
			{
				instance.missionLocation = baseNetworkable.transform.position;
			}
			return;
		}
		if (instance.status == BaseMission.MissionStatus.Accomplished)
		{
			float num2 = this.minDistFromLocation;
		}
		base.Think(instance, assignee, delta);
	}

	// Token: 0x0400243A RID: 9274
	public float minDistForMovePoint = 20f;

	// Token: 0x0400243B RID: 9275
	public float maxDistForMovePoint = 25f;

	// Token: 0x0400243C RID: 9276
	private float minDistFromLocation = 3f;
}
