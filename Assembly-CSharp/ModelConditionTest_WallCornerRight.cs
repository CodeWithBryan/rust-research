using System;
using UnityEngine;

// Token: 0x02000251 RID: 593
public class ModelConditionTest_WallCornerRight : ModelConditionTest
{
	// Token: 0x06001B5F RID: 7007 RVA: 0x000BEFD0 File Offset: 0x000BD1D0
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink(ModelConditionTest_WallCornerRight.sockets);
		if (entityLink == null)
		{
			return false;
		}
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if (buildingBlock == null)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			EntityLink entityLink2 = entityLink.connections[i];
			BuildingBlock buildingBlock2 = entityLink2.owner as BuildingBlock;
			if (!(buildingBlock2 == null))
			{
				float num = Vector3.SignedAngle(ent.transform.forward, buildingBlock2.transform.forward, Vector3.up);
				if (entityLink2.name.EndsWith("sockets/stability/1"))
				{
					if (num < 10f || num > 100f)
					{
						return false;
					}
				}
				else
				{
					if (num < 10f && num > -10f)
					{
						return false;
					}
					if (num > 10f)
					{
						return false;
					}
					if (buildingBlock2.grade == buildingBlock.grade)
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0400146C RID: 5228
	private const string socket = "sockets/stability/1";

	// Token: 0x0400146D RID: 5229
	private static string[] sockets = new string[]
	{
		"wall/sockets/stability/1",
		"wall.half/sockets/stability/1",
		"wall.low/sockets/stability/1",
		"wall.doorway/sockets/stability/1",
		"wall.window/sockets/stability/1"
	};
}
