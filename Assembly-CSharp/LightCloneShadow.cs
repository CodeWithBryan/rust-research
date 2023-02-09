using System;
using UnityEngine;

// Token: 0x02000700 RID: 1792
public class LightCloneShadow : MonoBehaviour
{
	// Token: 0x04002855 RID: 10325
	public bool cloneShadowMap;

	// Token: 0x04002856 RID: 10326
	public string shaderPropNameMap = "_MainLightShadowMap";

	// Token: 0x04002857 RID: 10327
	[Range(0f, 2f)]
	public int cloneShadowMapDownscale = 1;

	// Token: 0x04002858 RID: 10328
	public RenderTexture map;

	// Token: 0x04002859 RID: 10329
	public bool cloneShadowMask = true;

	// Token: 0x0400285A RID: 10330
	public string shaderPropNameMask = "_MainLightShadowMask";

	// Token: 0x0400285B RID: 10331
	[Range(0f, 2f)]
	public int cloneShadowMaskDownscale = 1;

	// Token: 0x0400285C RID: 10332
	public RenderTexture mask;
}
