using System;
using UnityEngine;

// Token: 0x02000481 RID: 1153
public class SnowmobileChassisVisuals : VehicleChassisVisuals<Snowmobile>, IClientComponent
{
	// Token: 0x04001E16 RID: 7702
	[SerializeField]
	private Animator animator;

	// Token: 0x04001E17 RID: 7703
	[SerializeField]
	private SnowmobileAudio audioScript;

	// Token: 0x04001E18 RID: 7704
	[SerializeField]
	private SnowmobileChassisVisuals.TreadRenderer[] treadRenderers;

	// Token: 0x04001E19 RID: 7705
	[SerializeField]
	private float treadSpeedMultiplier = 0.01f;

	// Token: 0x04001E1A RID: 7706
	[SerializeField]
	private bool flipRightSkiExtension;

	// Token: 0x04001E1B RID: 7707
	[SerializeField]
	private Transform leftSki;

	// Token: 0x04001E1C RID: 7708
	[SerializeField]
	private Transform leftSkiPistonIn;

	// Token: 0x04001E1D RID: 7709
	[SerializeField]
	private Transform leftSkiPistonOut;

	// Token: 0x04001E1E RID: 7710
	[SerializeField]
	private Transform rightSki;

	// Token: 0x04001E1F RID: 7711
	[SerializeField]
	private Transform rightSkiPistonIn;

	// Token: 0x04001E20 RID: 7712
	[SerializeField]
	private Transform rightSkiPistonOut;

	// Token: 0x04001E21 RID: 7713
	[SerializeField]
	private float skiVisualAdjust;

	// Token: 0x04001E22 RID: 7714
	[SerializeField]
	private float treadVisualAdjust;

	// Token: 0x04001E23 RID: 7715
	[SerializeField]
	private float skiVisualMaxExtension;

	// Token: 0x04001E24 RID: 7716
	[SerializeField]
	private float treadVisualMaxExtension;

	// Token: 0x04001E25 RID: 7717
	[SerializeField]
	private float wheelSizeVisualMultiplier = 1f;

	// Token: 0x02000CB5 RID: 3253
	[Serializable]
	private class TreadRenderer
	{
		// Token: 0x04004389 RID: 17289
		public Renderer renderer;

		// Token: 0x0400438A RID: 17290
		public int materialIndex;
	}
}
