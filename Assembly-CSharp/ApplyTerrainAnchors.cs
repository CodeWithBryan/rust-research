using System;
using UnityEngine;

// Token: 0x0200065D RID: 1629
public class ApplyTerrainAnchors : MonoBehaviour
{
	// Token: 0x06002E32 RID: 11826 RVA: 0x001152AC File Offset: 0x001134AC
	protected void Awake()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		TerrainAnchor[] anchors = null;
		if (component.isServer)
		{
			anchors = PrefabAttribute.server.FindAll<TerrainAnchor>(component.prefabID);
		}
		base.transform.ApplyTerrainAnchors(anchors);
		GameManager.Destroy(this, 0f);
	}
}
