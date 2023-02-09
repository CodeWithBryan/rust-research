using System;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class ModelConditionTest_WallTriangleLeft : ModelConditionTest
{
	// Token: 0x06001B62 RID: 7010 RVA: 0x000BF0F4 File Offset: 0x000BD2F4
	public static bool CheckCondition(BaseEntity ent)
	{
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/wall-female"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/1"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/2"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/3"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/floor-female/4"))
		{
			return false;
		}
		if (ModelConditionTest_WallTriangleLeft.CheckSocketOccupied(ent, "wall/sockets/stability/1"))
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
				if (buildingBlock.blockDefinition.info.name.token == "roof" && Vector3.Angle(ent.transform.forward, buildingBlock.transform.forward) < 10f)
				{
					return true;
				}
				if (buildingBlock.blockDefinition.info.name.token == "roof_triangle" && Vector3.Angle(ent.transform.forward, buildingBlock.transform.forward) < 40f)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001B63 RID: 7011 RVA: 0x000BF238 File Offset: 0x000BD438
	private static bool CheckSocketOccupied(BaseEntity ent, string socket)
	{
		EntityLink entityLink = ent.FindLink(socket);
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x06001B64 RID: 7012 RVA: 0x000BF25B File Offset: 0x000BD45B
	public override bool DoTest(BaseEntity ent)
	{
		return ModelConditionTest_WallTriangleLeft.CheckCondition(ent);
	}

	// Token: 0x0400146E RID: 5230
	private const string socket_1 = "wall/sockets/wall-female";

	// Token: 0x0400146F RID: 5231
	private const string socket_2 = "wall/sockets/floor-female/1";

	// Token: 0x04001470 RID: 5232
	private const string socket_3 = "wall/sockets/floor-female/2";

	// Token: 0x04001471 RID: 5233
	private const string socket_4 = "wall/sockets/floor-female/3";

	// Token: 0x04001472 RID: 5234
	private const string socket_5 = "wall/sockets/floor-female/4";

	// Token: 0x04001473 RID: 5235
	private const string socket_6 = "wall/sockets/stability/1";

	// Token: 0x04001474 RID: 5236
	private const string socket = "wall/sockets/neighbour/1";
}
