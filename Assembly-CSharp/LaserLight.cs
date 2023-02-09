using System;
using UnityEngine;

// Token: 0x0200037F RID: 895
public class LaserLight : AudioVisualisationEntity
{
	// Token: 0x06001F42 RID: 8002 RVA: 0x000CF596 File Offset: 0x000CD796
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
	}

	// Token: 0x040018A5 RID: 6309
	public Animator LaserAnimator;

	// Token: 0x040018A6 RID: 6310
	public LineRenderer[] LineRenderers;

	// Token: 0x040018A7 RID: 6311
	public MeshRenderer[] DotRenderers;

	// Token: 0x040018A8 RID: 6312
	public MeshRenderer FlareRenderer;

	// Token: 0x040018A9 RID: 6313
	public Light[] LightSources;

	// Token: 0x040018AA RID: 6314
	public LaserLight.ColourSetting RedSettings;

	// Token: 0x040018AB RID: 6315
	public LaserLight.ColourSetting GreenSettings;

	// Token: 0x040018AC RID: 6316
	public LaserLight.ColourSetting BlueSettings;

	// Token: 0x040018AD RID: 6317
	public LaserLight.ColourSetting YellowSettings;

	// Token: 0x040018AE RID: 6318
	public LaserLight.ColourSetting PinkSettings;

	// Token: 0x02000C63 RID: 3171
	[Serializable]
	public struct ColourSetting
	{
		// Token: 0x04004229 RID: 16937
		public Color PointLightColour;

		// Token: 0x0400422A RID: 16938
		public Material LaserMaterial;

		// Token: 0x0400422B RID: 16939
		public Color DotColour;

		// Token: 0x0400422C RID: 16940
		public Color FlareColour;
	}
}
