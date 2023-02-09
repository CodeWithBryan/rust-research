using System;
using UnityEngine;

// Token: 0x02000220 RID: 544
[CreateAssetMenu(menuName = "Rust/Reverb Settings")]
public class ReverbSettings : ScriptableObject
{
	// Token: 0x04001387 RID: 4999
	[Range(-10000f, 0f)]
	public int room;

	// Token: 0x04001388 RID: 5000
	[Range(-10000f, 0f)]
	public int roomHF;

	// Token: 0x04001389 RID: 5001
	[Range(-10000f, 0f)]
	public int roomLF;

	// Token: 0x0400138A RID: 5002
	[Range(0.1f, 20f)]
	public float decayTime;

	// Token: 0x0400138B RID: 5003
	[Range(0.1f, 2f)]
	public float decayHFRatio;

	// Token: 0x0400138C RID: 5004
	[Range(-10000f, 1000f)]
	public int reflections;

	// Token: 0x0400138D RID: 5005
	[Range(0f, 0.3f)]
	public float reflectionsDelay;

	// Token: 0x0400138E RID: 5006
	[Range(-10000f, 2000f)]
	public int reverb;

	// Token: 0x0400138F RID: 5007
	[Range(0f, 0.1f)]
	public float reverbDelay;

	// Token: 0x04001390 RID: 5008
	[Range(1000f, 20000f)]
	public float HFReference;

	// Token: 0x04001391 RID: 5009
	[Range(20f, 1000f)]
	public float LFReference;

	// Token: 0x04001392 RID: 5010
	[Range(0f, 100f)]
	public float diffusion;

	// Token: 0x04001393 RID: 5011
	[Range(0f, 100f)]
	public float density;
}
