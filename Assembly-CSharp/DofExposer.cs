using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020002E8 RID: 744
[ExecuteInEditMode]
public class DofExposer : SingletonComponent<DofExposer>
{
	// Token: 0x040016B3 RID: 5811
	public PostProcessVolume PostVolume;

	// Token: 0x040016B4 RID: 5812
	public bool DofEnabled;

	// Token: 0x040016B5 RID: 5813
	public float FocalLength = 15.24f;

	// Token: 0x040016B6 RID: 5814
	public float Blur = 2f;

	// Token: 0x040016B7 RID: 5815
	public float FocalAperture = 13.16f;

	// Token: 0x040016B8 RID: 5816
	public bool debug;
}
