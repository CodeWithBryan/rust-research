using System;
using UnityEngine;

// Token: 0x02000593 RID: 1427
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Natural Bloom and Dirty Lens")]
public class NaturalBloomAndDirtyLens : MonoBehaviour
{
	// Token: 0x04002282 RID: 8834
	public Shader shader;

	// Token: 0x04002283 RID: 8835
	public Texture2D lensDirtTexture;

	// Token: 0x04002284 RID: 8836
	public float range = 10000f;

	// Token: 0x04002285 RID: 8837
	public float cutoff = 1f;

	// Token: 0x04002286 RID: 8838
	[Range(0f, 1f)]
	public float bloomIntensity = 0.05f;

	// Token: 0x04002287 RID: 8839
	[Range(0f, 1f)]
	public float lensDirtIntensity = 0.05f;

	// Token: 0x04002288 RID: 8840
	[Range(0f, 4f)]
	public float spread = 1f;

	// Token: 0x04002289 RID: 8841
	[Range(0f, 4f)]
	public int iterations = 1;

	// Token: 0x0400228A RID: 8842
	[Range(1f, 10f)]
	public int mips = 6;

	// Token: 0x0400228B RID: 8843
	public float[] mipWeights = new float[]
	{
		0.5f,
		0.6f,
		0.6f,
		0.45f,
		0.35f,
		0.23f
	};

	// Token: 0x0400228C RID: 8844
	public bool highPrecision;

	// Token: 0x0400228D RID: 8845
	public bool downscaleSource;

	// Token: 0x0400228E RID: 8846
	public bool debug;

	// Token: 0x0400228F RID: 8847
	public bool temporalFilter;

	// Token: 0x04002290 RID: 8848
	[Range(0.01f, 1f)]
	public float temporalFilterWeight = 0.75f;
}
