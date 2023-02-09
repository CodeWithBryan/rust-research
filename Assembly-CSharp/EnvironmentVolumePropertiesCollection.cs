using System;
using UnityEngine;

// Token: 0x020006F7 RID: 1783
[CreateAssetMenu(menuName = "Rust/Environment Volume Properties Collection")]
public class EnvironmentVolumePropertiesCollection : ScriptableObject
{
	// Token: 0x04002831 RID: 10289
	public float TransitionSpeed = 1f;

	// Token: 0x04002832 RID: 10290
	[Horizontal(1, 0)]
	public EnvironmentVolumePropertiesCollection.EnvironmentMultiplier[] ReflectionMultipliers;

	// Token: 0x04002833 RID: 10291
	public float DefaultReflectionMultiplier = 1f;

	// Token: 0x04002834 RID: 10292
	[Horizontal(1, 0)]
	public EnvironmentVolumePropertiesCollection.EnvironmentMultiplier[] AmbientMultipliers;

	// Token: 0x04002835 RID: 10293
	public float DefaultAmbientMultiplier = 1f;

	// Token: 0x04002836 RID: 10294
	public EnvironmentVolumePropertiesCollection.OceanParameters OceanOverrides;

	// Token: 0x02000DE1 RID: 3553
	[Serializable]
	public class EnvironmentMultiplier
	{
		// Token: 0x04004837 RID: 18487
		public EnvironmentType Type;

		// Token: 0x04004838 RID: 18488
		public float Multiplier;
	}

	// Token: 0x02000DE2 RID: 3554
	[Serializable]
	public class OceanParameters
	{
		// Token: 0x04004839 RID: 18489
		public AnimationCurve TransitionCurve = AnimationCurve.Linear(0f, 0f, 40f, 1f);

		// Token: 0x0400483A RID: 18490
		[Range(0f, 1f)]
		public float DirectionalLightMultiplier = 0.25f;

		// Token: 0x0400483B RID: 18491
		[Range(0f, 1f)]
		public float AmbientLightMultiplier;

		// Token: 0x0400483C RID: 18492
		[Range(0f, 1f)]
		public float ReflectionMultiplier = 1f;

		// Token: 0x0400483D RID: 18493
		[Range(0f, 1f)]
		public float SunMeshBrightnessMultiplier = 1f;

		// Token: 0x0400483E RID: 18494
		[Range(0f, 1f)]
		public float MoonMeshBrightnessMultiplier = 1f;

		// Token: 0x0400483F RID: 18495
		[Range(0f, 1f)]
		public float AtmosphereBrightnessMultiplier = 1f;

		// Token: 0x04004840 RID: 18496
		[Range(0f, 1f)]
		public float LightColorMultiplier = 1f;

		// Token: 0x04004841 RID: 18497
		public Color LightColor = Color.black;

		// Token: 0x04004842 RID: 18498
		[Range(0f, 1f)]
		public float SunRayColorMultiplier = 1f;

		// Token: 0x04004843 RID: 18499
		public Color SunRayColor = Color.black;

		// Token: 0x04004844 RID: 18500
		[Range(0f, 1f)]
		public float MoonRayColorMultiplier = 1f;

		// Token: 0x04004845 RID: 18501
		public Color MoonRayColor = Color.black;
	}
}
