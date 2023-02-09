using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000286 RID: 646
public class WaterOverlay : MonoBehaviour, IClientComponent
{
	// Token: 0x04001526 RID: 5414
	public PostProcessVolume postProcessVolume;

	// Token: 0x04001527 RID: 5415
	public WaterOverlay.EffectParams adminParams = WaterOverlay.EffectParams.DefaultAdmin;

	// Token: 0x04001528 RID: 5416
	public WaterOverlay.EffectParams gogglesParams = WaterOverlay.EffectParams.DefaultGoggles;

	// Token: 0x04001529 RID: 5417
	public WaterOverlay.EffectParams submarineParams = WaterOverlay.EffectParams.DefaultSubmarine;

	// Token: 0x0400152A RID: 5418
	public WaterOverlay.EffectParams underwaterLabParams = WaterOverlay.EffectParams.DefaultUnderwaterLab;

	// Token: 0x0400152B RID: 5419
	public Material[] UnderwaterFogMaterials;

	// Token: 0x02000C3D RID: 3133
	[Serializable]
	public struct EffectParams
	{
		// Token: 0x04004172 RID: 16754
		public float scatterCoefficient;

		// Token: 0x04004173 RID: 16755
		public bool blur;

		// Token: 0x04004174 RID: 16756
		public float blurDistance;

		// Token: 0x04004175 RID: 16757
		public float blurSize;

		// Token: 0x04004176 RID: 16758
		public int blurIterations;

		// Token: 0x04004177 RID: 16759
		public bool wiggle;

		// Token: 0x04004178 RID: 16760
		public float wiggleSpeed;

		// Token: 0x04004179 RID: 16761
		public bool chromaticAberration;

		// Token: 0x0400417A RID: 16762
		public bool godRays;

		// Token: 0x0400417B RID: 16763
		public static WaterOverlay.EffectParams DefaultAdmin = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.025f,
			blur = false,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = false,
			wiggleSpeed = 0f,
			chromaticAberration = true,
			godRays = false
		};

		// Token: 0x0400417C RID: 16764
		public static WaterOverlay.EffectParams DefaultGoggles = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.05f,
			blur = true,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = true,
			wiggleSpeed = 2f,
			chromaticAberration = true,
			godRays = true
		};

		// Token: 0x0400417D RID: 16765
		public static WaterOverlay.EffectParams DefaultSubmarine = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.025f,
			blur = false,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = false,
			wiggleSpeed = 0f,
			chromaticAberration = false,
			godRays = false
		};

		// Token: 0x0400417E RID: 16766
		public static WaterOverlay.EffectParams DefaultUnderwaterLab = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.005f,
			blur = false,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = false,
			wiggleSpeed = 0f,
			chromaticAberration = true,
			godRays = false
		};
	}
}
