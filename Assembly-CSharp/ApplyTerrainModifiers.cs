using System;
using UnityEngine;

// Token: 0x020006C0 RID: 1728
public class ApplyTerrainModifiers : MonoBehaviour
{
	// Token: 0x0600307D RID: 12413 RVA: 0x0012AC78 File Offset: 0x00128E78
	protected void Awake()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		TerrainModifier[] modifiers = null;
		if (component.isServer)
		{
			modifiers = PrefabAttribute.server.FindAll<TerrainModifier>(component.prefabID);
		}
		base.transform.ApplyTerrainModifiers(modifiers);
		GameManager.Destroy(this, 0f);
	}
}
