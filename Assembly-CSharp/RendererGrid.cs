using System;

// Token: 0x02000502 RID: 1282
public class RendererGrid : SingletonComponent<RendererGrid>, IClientComponent
{
	// Token: 0x0400208E RID: 8334
	public static bool Paused;

	// Token: 0x0400208F RID: 8335
	public GameObjectRef BatchPrefab;

	// Token: 0x04002090 RID: 8336
	public float CellSize = 50f;

	// Token: 0x04002091 RID: 8337
	public float MaxMilliseconds = 0.1f;

	// Token: 0x04002092 RID: 8338
	public const float MinTimeBetweenRefreshes = 1f;
}
