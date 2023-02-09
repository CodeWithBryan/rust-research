using System;

// Token: 0x020006EC RID: 1772
[Serializable]
public struct SubsurfaceScatteringParams
{
	// Token: 0x04002819 RID: 10265
	public bool enabled;

	// Token: 0x0400281A RID: 10266
	public SubsurfaceScatteringParams.Quality quality;

	// Token: 0x0400281B RID: 10267
	public bool halfResolution;

	// Token: 0x0400281C RID: 10268
	public float radiusScale;

	// Token: 0x0400281D RID: 10269
	public static SubsurfaceScatteringParams Default = new SubsurfaceScatteringParams
	{
		enabled = true,
		quality = SubsurfaceScatteringParams.Quality.Medium,
		halfResolution = true,
		radiusScale = 1f
	};

	// Token: 0x02000DDF RID: 3551
	public enum Quality
	{
		// Token: 0x04004832 RID: 18482
		Low,
		// Token: 0x04004833 RID: 18483
		Medium,
		// Token: 0x04004834 RID: 18484
		High
	}
}
