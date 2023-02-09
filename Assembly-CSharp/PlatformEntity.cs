using System;
using UnityEngine;

// Token: 0x0200041C RID: 1052
public class PlatformEntity : BaseEntity
{
	// Token: 0x06002308 RID: 8968 RVA: 0x000DF218 File Offset: 0x000DD418
	protected void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (this.targetPosition == Vector3.zero || Vector3.Distance(base.transform.position, this.targetPosition) < 0.01f)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * 10f;
			this.targetPosition = base.transform.position + new Vector3(vector.x, 0f, vector.y);
			if (TerrainMeta.HeightMap != null && TerrainMeta.WaterMap != null)
			{
				float height = TerrainMeta.HeightMap.GetHeight(this.targetPosition);
				float height2 = TerrainMeta.WaterMap.GetHeight(this.targetPosition);
				this.targetPosition.y = Mathf.Max(height, height2) + 1f;
			}
			this.targetRotation = Quaternion.LookRotation(this.targetPosition - base.transform.position);
		}
		base.transform.SetPositionAndRotation(Vector3.MoveTowards(base.transform.position, this.targetPosition, Time.fixedDeltaTime * 1f), Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, Time.fixedDeltaTime * 10f));
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x000058B6 File Offset: 0x00003AB6
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x04001B8F RID: 7055
	private const float movementSpeed = 1f;

	// Token: 0x04001B90 RID: 7056
	private const float rotationSpeed = 10f;

	// Token: 0x04001B91 RID: 7057
	private const float radius = 10f;

	// Token: 0x04001B92 RID: 7058
	private Vector3 targetPosition = Vector3.zero;

	// Token: 0x04001B93 RID: 7059
	private Quaternion targetRotation = Quaternion.identity;
}
