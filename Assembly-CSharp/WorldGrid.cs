using System;

// Token: 0x0200058A RID: 1418
public class WorldGrid : SingletonComponent<WorldGrid>, IClientComponent
{
	// Token: 0x04002257 RID: 8791
	public static bool Paused;

	// Token: 0x04002258 RID: 8792
	public float CellSize = 50f;

	// Token: 0x04002259 RID: 8793
	public float MaxMilliseconds = 0.1f;

	// Token: 0x0400225A RID: 8794
	public const float MaxRefreshDistance = 500f;

	// Token: 0x0400225B RID: 8795
	public const float MinTimeBetweenRefreshes = 1f;
}
