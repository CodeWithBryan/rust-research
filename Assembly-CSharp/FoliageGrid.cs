using System;
using UnityEngine.Rendering;

// Token: 0x020004DF RID: 1247
public class FoliageGrid : SingletonComponent<FoliageGrid>, IClientComponent
{
	// Token: 0x04001FE7 RID: 8167
	public static bool Paused;

	// Token: 0x04001FE8 RID: 8168
	public GameObjectRef BatchPrefab;

	// Token: 0x04001FE9 RID: 8169
	public float CellSize = 50f;

	// Token: 0x04001FEA RID: 8170
	public LayerSelect FoliageLayer = 0;

	// Token: 0x04001FEB RID: 8171
	public ShadowCastingMode FoliageShadows;
}
