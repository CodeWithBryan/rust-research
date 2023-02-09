using System;
using UnityEngine;

// Token: 0x02000404 RID: 1028
public class TorpedoServerProjectile : ServerProjectile
{
	// Token: 0x1700029F RID: 671
	// (get) Token: 0x06002294 RID: 8852 RVA: 0x00007074 File Offset: 0x00005274
	public override bool HasRangeLimit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x06002295 RID: 8853 RVA: 0x000DD3A0 File Offset: 0x000DB5A0
	protected override int mask
	{
		get
		{
			return 1236478721;
		}
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x000DD3A8 File Offset: 0x000DB5A8
	public override bool DoMovement()
	{
		if (!base.DoMovement())
		{
			return false;
		}
		float num = WaterLevel.GetWaterInfo(base.transform.position, true, null, false).surfaceLevel - base.transform.position.y;
		if (num < -1f)
		{
			this.gravityModifier = 1f;
		}
		else if (num <= this.minWaterDepth)
		{
			Vector3 currentVelocity = base.CurrentVelocity;
			currentVelocity.y = 0f;
			base.CurrentVelocity = currentVelocity;
			this.gravityModifier = 0.1f;
		}
		else if (num > this.minWaterDepth + 0.3f && num <= this.minWaterDepth + 0.7f)
		{
			this.gravityModifier = -0.1f;
		}
		else
		{
			this.gravityModifier = Mathf.Clamp(base.CurrentVelocity.y, -0.1f, 0.1f);
		}
		return true;
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x000DD47C File Offset: 0x000DB67C
	public override void InitializeVelocity(Vector3 overrideVel)
	{
		base.InitializeVelocity(overrideVel);
		float value = WaterLevel.GetWaterInfo(base.transform.position, true, null, false).surfaceLevel - base.transform.position.y;
		float t = Mathf.InverseLerp(this.shallowWaterCutoff, this.shallowWaterCutoff + 2f, value);
		float maxAngle = Mathf.Lerp(this.shallowWaterInaccuracy, this.deepWaterInaccuracy, t);
		this.initialVelocity = this.initialVelocity.GetWithInaccuracy(maxAngle);
		base.CurrentVelocity = this.initialVelocity;
	}

	// Token: 0x04001B0B RID: 6923
	[Tooltip("Make sure to leave some allowance for waves, which affect the true depth.")]
	[SerializeField]
	private float minWaterDepth = 0.5f;

	// Token: 0x04001B0C RID: 6924
	[SerializeField]
	private float shallowWaterInaccuracy;

	// Token: 0x04001B0D RID: 6925
	[SerializeField]
	private float deepWaterInaccuracy;

	// Token: 0x04001B0E RID: 6926
	[SerializeField]
	private float shallowWaterCutoff = 2f;
}
