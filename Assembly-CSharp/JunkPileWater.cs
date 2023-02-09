using System;
using UnityEngine;

// Token: 0x0200015E RID: 350
public class JunkPileWater : JunkPile
{
	// Token: 0x06001663 RID: 5731 RVA: 0x000AA2EC File Offset: 0x000A84EC
	public override void Spawn()
	{
		Vector3 position = base.transform.position;
		position.y = TerrainMeta.WaterMap.GetHeight(base.transform.position);
		base.transform.position = position;
		base.Spawn();
		this.baseRotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x06001664 RID: 5732 RVA: 0x000AA360 File Offset: 0x000A8560
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateMovement();
	}

	// Token: 0x06001665 RID: 5733 RVA: 0x000AA374 File Offset: 0x000A8574
	public void UpdateMovement()
	{
		if (this.nextPlayerCheck <= 0f)
		{
			this.nextPlayerCheck = UnityEngine.Random.Range(0.5f, 1f);
			JunkPileWater.junkpileWaterWorkQueue.Add(this);
		}
		if (!this.isSinking && this.hasPlayersNearby)
		{
			float height = WaterSystem.GetHeight(base.transform.position);
			base.transform.position = new Vector3(base.transform.position.x, height, base.transform.position.z);
			if (this.buoyancyPoints != null && this.buoyancyPoints.Length >= 3)
			{
				Vector3 position = base.transform.position;
				Vector3 localPosition = this.buoyancyPoints[0].localPosition;
				Vector3 localPosition2 = this.buoyancyPoints[1].localPosition;
				Vector3 localPosition3 = this.buoyancyPoints[2].localPosition;
				Vector3 vector = localPosition + position;
				Vector3 vector2 = localPosition2 + position;
				Vector3 vector3 = localPosition3 + position;
				vector.y = WaterSystem.GetHeight(vector);
				vector2.y = WaterSystem.GetHeight(vector2);
				vector3.y = WaterSystem.GetHeight(vector3);
				Vector3 position2 = new Vector3(position.x, vector.y - localPosition.y, position.z);
				Vector3 rhs = vector2 - vector;
				Vector3 vector4 = Vector3.Cross(vector3 - vector, rhs);
				Vector3 eulerAngles = Quaternion.LookRotation(new Vector3(vector4.x, vector4.z, vector4.y)).eulerAngles;
				Quaternion lhs = Quaternion.Euler(-eulerAngles.x, 0f, -eulerAngles.y);
				if (this.first)
				{
					this.baseRotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
					this.first = false;
				}
				base.transform.SetPositionAndRotation(position2, lhs * this.baseRotation);
			}
		}
	}

	// Token: 0x06001666 RID: 5734 RVA: 0x000AA57D File Offset: 0x000A877D
	public void UpdateNearbyPlayers()
	{
		this.hasPlayersNearby = BaseNetworkable.HasCloseConnections(base.transform.position, 16f);
	}

	// Token: 0x04000F4D RID: 3917
	public static JunkPileWater.JunkpileWaterWorkQueue junkpileWaterWorkQueue = new JunkPileWater.JunkpileWaterWorkQueue();

	// Token: 0x04000F4E RID: 3918
	[ServerVar]
	[Help("How many milliseconds to budget for processing life story updates per frame")]
	public static float framebudgetms = 0.25f;

	// Token: 0x04000F4F RID: 3919
	public Transform[] buoyancyPoints;

	// Token: 0x04000F50 RID: 3920
	public bool debugDraw;

	// Token: 0x04000F51 RID: 3921
	private Quaternion baseRotation = Quaternion.identity;

	// Token: 0x04000F52 RID: 3922
	private bool first = true;

	// Token: 0x04000F53 RID: 3923
	private TimeUntil nextPlayerCheck;

	// Token: 0x04000F54 RID: 3924
	private bool hasPlayersNearby;

	// Token: 0x02000BDE RID: 3038
	public class JunkpileWaterWorkQueue : ObjectWorkQueue<JunkPileWater>
	{
		// Token: 0x06004B58 RID: 19288 RVA: 0x00191C35 File Offset: 0x0018FE35
		protected override void RunJob(JunkPileWater entity)
		{
			if (this.ShouldAdd(entity))
			{
				entity.UpdateNearbyPlayers();
			}
		}

		// Token: 0x06004B59 RID: 19289 RVA: 0x00191C46 File Offset: 0x0018FE46
		protected override bool ShouldAdd(JunkPileWater entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}
}
