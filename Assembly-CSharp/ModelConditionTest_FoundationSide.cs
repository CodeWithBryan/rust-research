using System;
using UnityEngine;

// Token: 0x02000245 RID: 581
public class ModelConditionTest_FoundationSide : ModelConditionTest
{
	// Token: 0x06001B34 RID: 6964 RVA: 0x000BE3C4 File Offset: 0x000BC5C4
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(1.5f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001B35 RID: 6965 RVA: 0x000BE418 File Offset: 0x000BC618
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		Vector3 vector = this.worldRotation * Vector3.right;
		if (name.Contains("foundation.triangle"))
		{
			if (vector.z < -0.9f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/1";
			}
			if (vector.x < -0.1f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/2";
			}
			if (vector.x > 0.1f)
			{
				this.socket = "foundation.triangle/sockets/foundation-top/3";
				return;
			}
		}
		else
		{
			if (vector.z < -0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/1";
			}
			if (vector.z > 0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/3";
			}
			if (vector.x < -0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/2";
			}
			if (vector.x > 0.9f)
			{
				this.socket = "foundation/sockets/foundation-top/4";
			}
		}
	}

	// Token: 0x06001B36 RID: 6966 RVA: 0x000BE4EC File Offset: 0x000BC6EC
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink(this.socket);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock = entityLink.connections[i].owner as BuildingBlock;
			if (!(buildingBlock == null) && !(buildingBlock.blockDefinition.info.name.token == "foundation_steps"))
			{
				if (buildingBlock.grade == BuildingGrade.Enum.TopTier)
				{
					return false;
				}
				if (buildingBlock.grade == BuildingGrade.Enum.Metal)
				{
					return false;
				}
				if (buildingBlock.grade == BuildingGrade.Enum.Stone)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0400143C RID: 5180
	private const string square_south = "foundation/sockets/foundation-top/1";

	// Token: 0x0400143D RID: 5181
	private const string square_north = "foundation/sockets/foundation-top/3";

	// Token: 0x0400143E RID: 5182
	private const string square_west = "foundation/sockets/foundation-top/2";

	// Token: 0x0400143F RID: 5183
	private const string square_east = "foundation/sockets/foundation-top/4";

	// Token: 0x04001440 RID: 5184
	private const string triangle_south = "foundation.triangle/sockets/foundation-top/1";

	// Token: 0x04001441 RID: 5185
	private const string triangle_northwest = "foundation.triangle/sockets/foundation-top/2";

	// Token: 0x04001442 RID: 5186
	private const string triangle_northeast = "foundation.triangle/sockets/foundation-top/3";

	// Token: 0x04001443 RID: 5187
	private string socket = string.Empty;
}
