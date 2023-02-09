using System;
using UnityEngine;

// Token: 0x02000379 RID: 889
public class DiscoFloor : AudioVisualisationEntity
{
	// Token: 0x0400188E RID: 6286
	public float GradientDuration = 3f;

	// Token: 0x0400188F RID: 6287
	public float VolumeSensitivityMultiplier = 3f;

	// Token: 0x04001890 RID: 6288
	public float BaseSpeed;

	// Token: 0x04001891 RID: 6289
	public Light[] LightSources;

	// Token: 0x04001892 RID: 6290
	public DiscoFloorMesh FloorMesh;
}
