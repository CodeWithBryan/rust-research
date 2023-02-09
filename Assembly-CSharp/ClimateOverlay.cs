using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200027B RID: 635
public class ClimateOverlay : MonoBehaviour
{
	// Token: 0x0400150D RID: 5389
	[Range(0f, 1f)]
	public float blendingSpeed = 1f;

	// Token: 0x0400150E RID: 5390
	public PostProcessVolume[] biomeVolumes;
}
