using System;

// Token: 0x0200050C RID: 1292
public class LODGrid : SingletonComponent<LODGrid>, IClientComponent
{
	// Token: 0x040020A8 RID: 8360
	public static bool Paused = false;

	// Token: 0x040020A9 RID: 8361
	public float CellSize = 50f;

	// Token: 0x040020AA RID: 8362
	public float MaxMilliseconds = 0.1f;

	// Token: 0x040020AB RID: 8363
	public const float MaxRefreshDistance = 500f;

	// Token: 0x040020AC RID: 8364
	public static float TreeMeshDistance = 500f;

	// Token: 0x040020AD RID: 8365
	public const float MinTimeBetweenRefreshes = 1f;
}
