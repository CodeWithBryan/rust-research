using System;
using UnityEngine;

// Token: 0x02000652 RID: 1618
public class PowerlineNode : MonoBehaviour
{
	// Token: 0x06002E18 RID: 11800 RVA: 0x0011482D File Offset: 0x00112A2D
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.AddWire(this);
		}
	}

	// Token: 0x040025C7 RID: 9671
	public GameObjectRef WirePrefab;

	// Token: 0x040025C8 RID: 9672
	public float MaxDistance = 50f;
}
