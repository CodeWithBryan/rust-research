using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004C9 RID: 1225
public class CullingVolume : MonoBehaviour, IClientComponent
{
	// Token: 0x04001FAE RID: 8110
	[Tooltip("Override occludee root from children of this object (default) to children of any other object.")]
	public GameObject OccludeeRoot;

	// Token: 0x04001FAF RID: 8111
	[Tooltip("Invert visibility. False will show occludes. True will hide them.")]
	public bool Invert;

	// Token: 0x04001FB0 RID: 8112
	[Tooltip("A portal in the culling volume chain does not toggle objects visible, it merely signals the non-portal volumes to hide their occludees.")]
	public bool Portal;

	// Token: 0x04001FB1 RID: 8113
	[Tooltip("Secondary culling volumes, connected to this one, that will get signaled when this trigger is activated.")]
	public List<CullingVolume> Connections = new List<CullingVolume>();
}
