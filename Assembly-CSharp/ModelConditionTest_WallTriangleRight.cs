using System;
using UnityEngine;

// Token: 0x02000253 RID: 595
public class ModelConditionTest_WallTriangleRight : ModelConditionTest
{
	// Token: 0x06001B66 RID: 7014 RVA: 0x000BF264 File Offset: 0x000BD464
	public static bool CheckCondition(BaseEntity ent)
	{
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/wall-female"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/1"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/2"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/3"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/4"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/stability/2"))
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink("wall/sockets/neighbour/1");
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock = entityLink.connections[i].owner as BuildingBlock;
			if (!(buildingBlock == null))
			{
				if (buildingBlock.blockDefinition.info.name.token == "roof" && Vector3.Angle(ent.transform.forward, -buildingBlock.transform.forward) < 10f)
				{
					return true;
				}
				if (buildingBlock.blockDefinition.info.name.token == "roof_triangle" && Vector3.Angle(ent.transform.forward, -buildingBlock.transform.forward) < 40f)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x000BF3B0 File Offset: 0x000BD5B0
	private static bool CheckSocketOccupied(BaseEntity ent, string socket)
	{
		EntityLink entityLink = ent.FindLink(socket);
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x000BF3D3 File Offset: 0x000BD5D3
	public override bool DoTest(BaseEntity ent)
	{
		return ModelConditionTest_WallTriangleRight.CheckCondition(ent);
	}

	// Token: 0x04001475 RID: 5237
	private const string socket_1 = "wall/sockets/wall-female";

	// Token: 0x04001476 RID: 5238
	private const string socket_2 = "wall/sockets/floor-female/1";

	// Token: 0x04001477 RID: 5239
	private const string socket_3 = "wall/sockets/floor-female/2";

	// Token: 0x04001478 RID: 5240
	private const string socket_4 = "wall/sockets/floor-female/3";

	// Token: 0x04001479 RID: 5241
	private const string socket_5 = "wall/sockets/floor-female/4";

	// Token: 0x0400147A RID: 5242
	private const string socket_6 = "wall/sockets/stability/2";

	// Token: 0x0400147B RID: 5243
	private const string socket = "wall/sockets/neighbour/1";
}
