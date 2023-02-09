using System;
using UnityEngine;

// Token: 0x0200024B RID: 587
public class ModelConditionTest_RoofTop : ModelConditionTest
{
	// Token: 0x06001B4E RID: 6990 RVA: 0x000BEBFC File Offset: 0x000BCDFC
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, -1.5f, 3f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001B4F RID: 6991 RVA: 0x000BEC50 File Offset: 0x000BCE50
	public override bool DoTest(BaseEntity ent)
	{
		bool flag = false;
		bool flag2 = false;
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofTop.sockets_top_right);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			if (entityLink.connections[i].name.EndsWith("sockets/neighbour/3"))
			{
				flag = true;
				break;
			}
		}
		EntityLink entityLink2 = ent.FindLink(ModelConditionTest_RoofTop.sockets_top_left);
		if (entityLink2 == null)
		{
			return false;
		}
		for (int j = 0; j < entityLink2.connections.Count; j++)
		{
			if (entityLink2.connections[j].name.EndsWith("sockets/neighbour/4"))
			{
				flag2 = true;
				break;
			}
		}
		return !flag || !flag2;
	}

	// Token: 0x0400145C RID: 5212
	private const string roof_square = "roof/";

	// Token: 0x0400145D RID: 5213
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x0400145E RID: 5214
	private const string socket_bot_right = "sockets/neighbour/3";

	// Token: 0x0400145F RID: 5215
	private const string socket_bot_left = "sockets/neighbour/4";

	// Token: 0x04001460 RID: 5216
	private const string socket_top_right = "sockets/neighbour/5";

	// Token: 0x04001461 RID: 5217
	private const string socket_top_left = "sockets/neighbour/6";

	// Token: 0x04001462 RID: 5218
	private static string[] sockets_top_right = new string[]
	{
		"roof/sockets/neighbour/5",
		"roof.triangle/sockets/neighbour/5"
	};

	// Token: 0x04001463 RID: 5219
	private static string[] sockets_top_left = new string[]
	{
		"roof/sockets/neighbour/6",
		"roof.triangle/sockets/neighbour/6"
	};
}
