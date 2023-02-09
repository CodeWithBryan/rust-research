using System;
using UnityEngine;

// Token: 0x02000951 RID: 2385
[AddComponentMenu("")]
public class AmplifyOcclusionBase : MonoBehaviour
{
	// Token: 0x0400328F RID: 12943
	[Header("Ambient Occlusion")]
	public AmplifyOcclusionBase.ApplicationMethod ApplyMethod;

	// Token: 0x04003290 RID: 12944
	[Tooltip("Number of samples per pass.")]
	public AmplifyOcclusionBase.SampleCountLevel SampleCount = AmplifyOcclusionBase.SampleCountLevel.Medium;

	// Token: 0x04003291 RID: 12945
	public AmplifyOcclusionBase.PerPixelNormalSource PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.Camera;

	// Token: 0x04003292 RID: 12946
	[Tooltip("Final applied intensity of the occlusion effect.")]
	[Range(0f, 1f)]
	public float Intensity = 1f;

	// Token: 0x04003293 RID: 12947
	public Color Tint = Color.black;

	// Token: 0x04003294 RID: 12948
	[Tooltip("Radius spread of the occlusion.")]
	[Range(0f, 32f)]
	public float Radius = 2f;

	// Token: 0x04003295 RID: 12949
	[Tooltip("Max sampling range in pixels.")]
	[Range(32f, 1024f)]
	[NonSerialized]
	public int PixelRadiusLimit = 512;

	// Token: 0x04003296 RID: 12950
	[Tooltip("Occlusion contribution amount on relation to radius.")]
	[Range(0f, 2f)]
	[NonSerialized]
	public float RadiusIntensity = 1f;

	// Token: 0x04003297 RID: 12951
	[Tooltip("Power exponent attenuation of the occlusion.")]
	[Range(0f, 16f)]
	public float PowerExponent = 1.8f;

	// Token: 0x04003298 RID: 12952
	[Tooltip("Controls the initial occlusion contribution offset.")]
	[Range(0f, 0.99f)]
	public float Bias = 0.05f;

	// Token: 0x04003299 RID: 12953
	[Tooltip("Controls the thickness occlusion contribution.")]
	[Range(0f, 1f)]
	public float Thickness = 1f;

	// Token: 0x0400329A RID: 12954
	[Tooltip("Compute the Occlusion and Blur at half of the resolution.")]
	public bool Downsample = true;

	// Token: 0x0400329B RID: 12955
	[Header("Distance Fade")]
	[Tooltip("Control parameters at faraway.")]
	public bool FadeEnabled;

	// Token: 0x0400329C RID: 12956
	[Tooltip("Distance in Unity unities that start to fade.")]
	public float FadeStart = 100f;

	// Token: 0x0400329D RID: 12957
	[Tooltip("Length distance to performe the transition.")]
	public float FadeLength = 50f;

	// Token: 0x0400329E RID: 12958
	[Tooltip("Final Intensity parameter.")]
	[Range(0f, 1f)]
	public float FadeToIntensity;

	// Token: 0x0400329F RID: 12959
	public Color FadeToTint = Color.black;

	// Token: 0x040032A0 RID: 12960
	[Tooltip("Final Radius parameter.")]
	[Range(0f, 32f)]
	public float FadeToRadius = 2f;

	// Token: 0x040032A1 RID: 12961
	[Tooltip("Final PowerExponent parameter.")]
	[Range(0f, 16f)]
	public float FadeToPowerExponent = 1.8f;

	// Token: 0x040032A2 RID: 12962
	[Tooltip("Final Thickness parameter.")]
	[Range(0f, 1f)]
	public float FadeToThickness = 1f;

	// Token: 0x040032A3 RID: 12963
	[Header("Bilateral Blur")]
	public bool BlurEnabled = true;

	// Token: 0x040032A4 RID: 12964
	[Tooltip("Radius in screen pixels.")]
	[Range(1f, 4f)]
	public int BlurRadius = 3;

	// Token: 0x040032A5 RID: 12965
	[Tooltip("Number of times that the Blur will repeat.")]
	[Range(1f, 4f)]
	public int BlurPasses = 1;

	// Token: 0x040032A6 RID: 12966
	[Tooltip("0 - Blured, 1 - Sharpened.")]
	[Range(0f, 20f)]
	public float BlurSharpness = 10f;

	// Token: 0x040032A7 RID: 12967
	[Header("Temporal Filter")]
	[Tooltip("Accumulates the effect over the time.")]
	public bool FilterEnabled = true;

	// Token: 0x040032A8 RID: 12968
	[Tooltip("Controls the accumulation decayment. 0 - Faster update, more flicker. 1 - Slow update (ghosting on moving objects), less flicker.")]
	[Range(0f, 1f)]
	public float FilterBlending = 0.5f;

	// Token: 0x040032A9 RID: 12969
	[Tooltip("Controls the discard sensibility based on the motion of the scene and objects. 0 - Discard less, reuse more (more ghost effect). 1 - Discard more, reuse less (less ghost effect).")]
	[Range(0f, 1f)]
	public float FilterResponse = 0.5f;

	// Token: 0x040032AA RID: 12970
	[Tooltip("Enables directional variations.")]
	[NonSerialized]
	public bool TemporalDirections = true;

	// Token: 0x040032AB RID: 12971
	[Tooltip("Enables offset variations.")]
	[NonSerialized]
	public bool TemporalOffsets = true;

	// Token: 0x040032AC RID: 12972
	[Tooltip("Reduces ghosting effect near the objects's edges while moving.")]
	[NonSerialized]
	public bool TemporalDilation;

	// Token: 0x040032AD RID: 12973
	[Tooltip("Uses the object movement information for calc new areas of occlusion.")]
	[NonSerialized]
	public bool UseMotionVectors = true;

	// Token: 0x02000E6C RID: 3692
	public enum ApplicationMethod
	{
		// Token: 0x04004A63 RID: 19043
		PostEffect,
		// Token: 0x04004A64 RID: 19044
		Deferred,
		// Token: 0x04004A65 RID: 19045
		Debug
	}

	// Token: 0x02000E6D RID: 3693
	public enum PerPixelNormalSource
	{
		// Token: 0x04004A67 RID: 19047
		None,
		// Token: 0x04004A68 RID: 19048
		Camera,
		// Token: 0x04004A69 RID: 19049
		GBuffer,
		// Token: 0x04004A6A RID: 19050
		GBufferOctaEncoded
	}

	// Token: 0x02000E6E RID: 3694
	public enum SampleCountLevel
	{
		// Token: 0x04004A6C RID: 19052
		Low,
		// Token: 0x04004A6D RID: 19053
		Medium,
		// Token: 0x04004A6E RID: 19054
		High,
		// Token: 0x04004A6F RID: 19055
		VeryHigh
	}
}
