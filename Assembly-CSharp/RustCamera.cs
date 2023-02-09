using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020002C6 RID: 710
public abstract class RustCamera<T> : SingletonComponent<T> where T : RustCamera<T>
{
	// Token: 0x04001649 RID: 5705
	[SerializeField]
	private AmplifyOcclusionEffect ssao;

	// Token: 0x0400164A RID: 5706
	[SerializeField]
	private SEScreenSpaceShadows contactShadows;

	// Token: 0x0400164B RID: 5707
	[SerializeField]
	private VisualizeTexelDensity visualizeTexelDensity;

	// Token: 0x0400164C RID: 5708
	[SerializeField]
	private EnvironmentVolumePropertiesCollection environmentVolumeProperties;

	// Token: 0x0400164D RID: 5709
	[SerializeField]
	private PostProcessLayer post;

	// Token: 0x0400164E RID: 5710
	[SerializeField]
	private PostProcessVolume baseEffectVolume;
}
