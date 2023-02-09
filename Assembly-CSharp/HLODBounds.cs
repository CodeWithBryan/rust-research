using System;
using UnityEngine;

// Token: 0x02000508 RID: 1288
public class HLODBounds : MonoBehaviour, IEditorComponent
{
	// Token: 0x0400209C RID: 8348
	[Tooltip("The bounds that this HLOD will cover. This should not overlap with any other HLODs")]
	public Bounds MeshBounds = new Bounds(Vector3.zero, new Vector3(50f, 25f, 50f));

	// Token: 0x0400209D RID: 8349
	[Tooltip("Assets created will use this prefix. Make sure multiple HLODS in a scene have different prefixes")]
	public string MeshPrefix = "root";

	// Token: 0x0400209E RID: 8350
	[Tooltip("The point from which to calculate the HLOD. Any RendererLODs that are visible at this distance will baked into the HLOD mesh")]
	public float CullDistance = 100f;

	// Token: 0x0400209F RID: 8351
	[Tooltip("If set, the lod will take over at this distance instead of the CullDistance (eg. we make a model based on what this area looks like at 200m but we actually want it take over rendering at 300m)")]
	public float OverrideLodDistance;

	// Token: 0x040020A0 RID: 8352
	[Tooltip("Any renderers below this height will considered culled even if they are visible from a distance. Good for underground areas")]
	public float CullBelowHeight;

	// Token: 0x040020A1 RID: 8353
	[Tooltip("Optimises the mesh produced by removing non-visible and small faces. Can turn it off during dev but should be on for final builds")]
	public bool ApplyMeshTrimming = true;

	// Token: 0x040020A2 RID: 8354
	public MeshTrimSettings Settings = MeshTrimSettings.Default;

	// Token: 0x040020A3 RID: 8355
	public RendererLOD DebugComponent;

	// Token: 0x040020A4 RID: 8356
	public bool ShowTrimSettings;
}
