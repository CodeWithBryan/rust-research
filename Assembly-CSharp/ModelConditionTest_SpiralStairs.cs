using System;
using UnityEngine;

// Token: 0x0200024D RID: 589
public class ModelConditionTest_SpiralStairs : ModelConditionTest
{
	// Token: 0x06001B54 RID: 6996 RVA: 0x000BED68 File Offset: 0x000BCF68
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, 2.35f, 0f), new Vector3(3f, 1.5f, 3f));
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x000BEDBC File Offset: 0x000BCFBC
	public override bool DoTest(BaseEntity ent)
	{
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if (buildingBlock == null)
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink(ModelConditionTest_SpiralStairs.stairs_sockets_female);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock2 = entityLink.connections[i].owner as BuildingBlock;
			if (!(buildingBlock2 == null) && buildingBlock2.grade == buildingBlock.grade)
			{
				return false;
			}
		}
		EntityLink entityLink2 = ent.FindLink(ModelConditionTest_SpiralStairs.floor_sockets_female);
		return entityLink2 == null || entityLink2.IsEmpty();
	}

	// Token: 0x04001465 RID: 5221
	private const string stairs_socket_female = "sockets/stairs-female/1";

	// Token: 0x04001466 RID: 5222
	private static string[] stairs_sockets_female = new string[]
	{
		"block.stair.spiral/sockets/stairs-female/1",
		"block.stair.spiral.triangle/sockets/stairs-female/1"
	};

	// Token: 0x04001467 RID: 5223
	private const string floor_socket_female = "sockets/floor-female/1";

	// Token: 0x04001468 RID: 5224
	private static string[] floor_sockets_female = new string[]
	{
		"block.stair.spiral/sockets/floor-female/1",
		"block.stair.spiral.triangle/sockets/floor-female/1"
	};
}
