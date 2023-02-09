using System;
using UnityEngine;

// Token: 0x020005C2 RID: 1474
public class ItemModFishable : ItemMod
{
	// Token: 0x04002385 RID: 9093
	public bool CanBeFished = true;

	// Token: 0x04002386 RID: 9094
	[Header("Catching Behaviour")]
	public float StrainModifier = 1f;

	// Token: 0x04002387 RID: 9095
	public float MoveMultiplier = 1f;

	// Token: 0x04002388 RID: 9096
	public float ReelInSpeedMultiplier = 1f;

	// Token: 0x04002389 RID: 9097
	public float CatchWaitTimeMultiplier = 1f;

	// Token: 0x0400238A RID: 9098
	[Header("Catch Criteria")]
	public float MinimumBaitLevel;

	// Token: 0x0400238B RID: 9099
	public float MaximumBaitLevel;

	// Token: 0x0400238C RID: 9100
	public float MinimumWaterDepth;

	// Token: 0x0400238D RID: 9101
	public float MaximumWaterDepth;

	// Token: 0x0400238E RID: 9102
	[InspectorFlags]
	public WaterBody.FishingTag RequiredTag;

	// Token: 0x0400238F RID: 9103
	[Range(0f, 1f)]
	public float Chance;

	// Token: 0x04002390 RID: 9104
	public string SteamStatName;
}
