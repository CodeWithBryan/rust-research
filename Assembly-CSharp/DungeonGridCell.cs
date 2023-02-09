using System;
using UnityEngine;

// Token: 0x02000642 RID: 1602
public class DungeonGridCell : MonoBehaviour
{
	// Token: 0x06002DF8 RID: 11768 RVA: 0x00114158 File Offset: 0x00112358
	public bool ShouldAvoid(uint id)
	{
		GameObjectRef[] avoidNeighbours = this.AvoidNeighbours;
		for (int i = 0; i < avoidNeighbours.Length; i++)
		{
			if (avoidNeighbours[i].resourceID == id)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002DF9 RID: 11769 RVA: 0x00114188 File Offset: 0x00112388
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.DungeonGridCells.Add(this);
		}
	}

	// Token: 0x0400258C RID: 9612
	public DungeonGridConnectionType North;

	// Token: 0x0400258D RID: 9613
	public DungeonGridConnectionType South;

	// Token: 0x0400258E RID: 9614
	public DungeonGridConnectionType West;

	// Token: 0x0400258F RID: 9615
	public DungeonGridConnectionType East;

	// Token: 0x04002590 RID: 9616
	public DungeonGridConnectionVariant NorthVariant;

	// Token: 0x04002591 RID: 9617
	public DungeonGridConnectionVariant SouthVariant;

	// Token: 0x04002592 RID: 9618
	public DungeonGridConnectionVariant WestVariant;

	// Token: 0x04002593 RID: 9619
	public DungeonGridConnectionVariant EastVariant;

	// Token: 0x04002594 RID: 9620
	public GameObjectRef[] AvoidNeighbours;

	// Token: 0x04002595 RID: 9621
	public MeshRenderer[] MapRenderers;
}
