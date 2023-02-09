using System;
using UnityEngine;

// Token: 0x02000248 RID: 584
public class ModelConditionTest_RoofBottom : ModelConditionTest
{
	// Token: 0x06001B3E RID: 6974 RVA: 0x000BE68C File Offset: 0x000BC88C
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, -1.5f, 3f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001B3F RID: 6975 RVA: 0x000BE6E0 File Offset: 0x000BC8E0
	public override bool DoTest(BaseEntity ent)
	{
		bool flag = false;
		bool flag2 = false;
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofBottom.sockets_bot_right);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			if (entityLink.connections[i].name.EndsWith("sockets/neighbour/5"))
			{
				flag = true;
				break;
			}
		}
		EntityLink entityLink2 = ent.FindLink(ModelConditionTest_RoofBottom.sockets_bot_left);
		if (entityLink2 == null)
		{
			return false;
		}
		for (int j = 0; j < entityLink2.connections.Count; j++)
		{
			if (entityLink2.connections[j].name.EndsWith("sockets/neighbour/6"))
			{
				flag2 = true;
				break;
			}
		}
		return !flag || !flag2;
	}

	// Token: 0x04001446 RID: 5190
	private const string roof_square = "roof/";

	// Token: 0x04001447 RID: 5191
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x04001448 RID: 5192
	private const string socket_bot_right = "sockets/neighbour/3";

	// Token: 0x04001449 RID: 5193
	private const string socket_bot_left = "sockets/neighbour/4";

	// Token: 0x0400144A RID: 5194
	private const string socket_top_right = "sockets/neighbour/5";

	// Token: 0x0400144B RID: 5195
	private const string socket_top_left = "sockets/neighbour/6";

	// Token: 0x0400144C RID: 5196
	private static string[] sockets_bot_right = new string[]
	{
		"roof/sockets/neighbour/3",
		"roof.triangle/sockets/neighbour/3"
	};

	// Token: 0x0400144D RID: 5197
	private static string[] sockets_bot_left = new string[]
	{
		"roof/sockets/neighbour/4",
		"roof.triangle/sockets/neighbour/4"
	};
}
