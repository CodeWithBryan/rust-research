using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200001D RID: 29
public class WaterBaseNavGenTest : MonoBehaviour
{
	// Token: 0x0600009E RID: 158 RVA: 0x0000554C File Offset: 0x0000374C
	[ContextMenu("Nav Gen")]
	public void NavGen()
	{
		DungeonNavmesh dungeonNavmesh = base.gameObject.AddComponent<DungeonNavmesh>();
		dungeonNavmesh.NavmeshResolutionModifier = 0.3f;
		dungeonNavmesh.NavMeshCollectGeometry = NavMeshCollectGeometry.PhysicsColliders;
		dungeonNavmesh.LayerMask = 65537;
		this.co = dungeonNavmesh.UpdateNavMeshAndWait();
		base.StartCoroutine(this.co);
	}

	// Token: 0x040000B2 RID: 178
	private IEnumerator co;
}
