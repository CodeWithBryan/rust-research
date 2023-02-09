using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200096E RID: 2414
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Sonic Ether/SE Screen-Space Shadows")]
[ExecuteInEditMode]
public class SEScreenSpaceShadows : MonoBehaviour
{
	// Token: 0x04003382 RID: 13186
	private CommandBuffer blendShadowsCommandBuffer;

	// Token: 0x04003383 RID: 13187
	private CommandBuffer renderShadowsCommandBuffer;

	// Token: 0x04003384 RID: 13188
	private Camera attachedCamera;

	// Token: 0x04003385 RID: 13189
	public Light sun;

	// Token: 0x04003386 RID: 13190
	[Range(0f, 1f)]
	public float blendStrength = 1f;

	// Token: 0x04003387 RID: 13191
	[Range(0f, 1f)]
	public float accumulation = 0.9f;

	// Token: 0x04003388 RID: 13192
	[Range(0.1f, 5f)]
	public float lengthFade = 0.7f;

	// Token: 0x04003389 RID: 13193
	[Range(0.01f, 5f)]
	public float range = 0.7f;

	// Token: 0x0400338A RID: 13194
	[Range(0f, 1f)]
	public float zThickness = 0.1f;

	// Token: 0x0400338B RID: 13195
	[Range(2f, 92f)]
	public int samples = 32;

	// Token: 0x0400338C RID: 13196
	[Range(0.5f, 4f)]
	public float nearSampleQuality = 1.5f;

	// Token: 0x0400338D RID: 13197
	[Range(0f, 1f)]
	public float traceBias = 0.03f;

	// Token: 0x0400338E RID: 13198
	public bool stochasticSampling = true;

	// Token: 0x0400338F RID: 13199
	public bool leverageTemporalAA;

	// Token: 0x04003390 RID: 13200
	public bool bilateralBlur = true;

	// Token: 0x04003391 RID: 13201
	[Range(1f, 2f)]
	public int blurPasses = 1;

	// Token: 0x04003392 RID: 13202
	[Range(0.01f, 0.5f)]
	public float blurDepthTolerance = 0.1f;
}
