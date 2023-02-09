using System;
using UnityEngine;

// Token: 0x0200024A RID: 586
public class ModelConditionTest_RoofRight : ModelConditionTest
{
	// Token: 0x17000213 RID: 531
	// (get) Token: 0x06001B48 RID: 6984 RVA: 0x000BE9E3 File Offset: 0x000BCBE3
	private bool IsConvex
	{
		get
		{
			return this.angle > (ModelConditionTest_RoofRight.AngleType)10;
		}
	}

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x06001B49 RID: 6985 RVA: 0x000BE9EF File Offset: 0x000BCBEF
	private bool IsConcave
	{
		get
		{
			return this.angle < (ModelConditionTest_RoofRight.AngleType)(-10);
		}
	}

	// Token: 0x06001B4A RID: 6986 RVA: 0x000BE9FC File Offset: 0x000BCBFC
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(-3f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001B4B RID: 6987 RVA: 0x000BEA50 File Offset: 0x000BCC50
	public override bool DoTest(BaseEntity ent)
	{
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if (buildingBlock == null)
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofRight.sockets_right);
		if (entityLink == null)
		{
			return false;
		}
		if (this.angle == ModelConditionTest_RoofRight.AngleType.None)
		{
			for (int i = 0; i < entityLink.connections.Count; i++)
			{
				if (entityLink.connections[i].name.EndsWith("sockets/neighbour/4"))
				{
					return false;
				}
			}
			return true;
		}
		if (entityLink.IsEmpty())
		{
			return false;
		}
		bool result = false;
		for (int j = 0; j < entityLink.connections.Count; j++)
		{
			EntityLink entityLink2 = entityLink.connections[j];
			if (entityLink2.name.EndsWith("sockets/neighbour/4") && (this.shape != ModelConditionTest_RoofRight.ShapeType.Square || entityLink2.name.StartsWith("roof/")) && (this.shape != ModelConditionTest_RoofRight.ShapeType.Triangle || entityLink2.name.StartsWith("roof.triangle/")))
			{
				BuildingBlock buildingBlock2 = entityLink2.owner as BuildingBlock;
				if (!(buildingBlock2 == null) && buildingBlock2.grade == buildingBlock.grade)
				{
					int num = (int)this.angle;
					float num2 = -Vector3.SignedAngle(ent.transform.forward, buildingBlock2.transform.forward, Vector3.up);
					if (num2 < (float)(num - 10))
					{
						if (this.IsConvex)
						{
							return false;
						}
					}
					else if (num2 > (float)(num + 10))
					{
						if (this.IsConvex)
						{
							return false;
						}
					}
					else
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x04001455 RID: 5205
	public ModelConditionTest_RoofRight.AngleType angle = ModelConditionTest_RoofRight.AngleType.None;

	// Token: 0x04001456 RID: 5206
	public ModelConditionTest_RoofRight.ShapeType shape = ModelConditionTest_RoofRight.ShapeType.Any;

	// Token: 0x04001457 RID: 5207
	private const string roof_square = "roof/";

	// Token: 0x04001458 RID: 5208
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x04001459 RID: 5209
	private const string socket_right = "sockets/neighbour/3";

	// Token: 0x0400145A RID: 5210
	private const string socket_left = "sockets/neighbour/4";

	// Token: 0x0400145B RID: 5211
	private static string[] sockets_right = new string[]
	{
		"roof/sockets/neighbour/3",
		"roof.triangle/sockets/neighbour/3"
	};

	// Token: 0x02000C37 RID: 3127
	public enum AngleType
	{
		// Token: 0x0400415C RID: 16732
		None = -1,
		// Token: 0x0400415D RID: 16733
		Straight,
		// Token: 0x0400415E RID: 16734
		Convex60 = 60,
		// Token: 0x0400415F RID: 16735
		Convex90 = 90,
		// Token: 0x04004160 RID: 16736
		Convex120 = 120,
		// Token: 0x04004161 RID: 16737
		Concave30 = -30,
		// Token: 0x04004162 RID: 16738
		Concave60 = -60,
		// Token: 0x04004163 RID: 16739
		Concave90 = -90,
		// Token: 0x04004164 RID: 16740
		Concave120 = -120
	}

	// Token: 0x02000C38 RID: 3128
	public enum ShapeType
	{
		// Token: 0x04004166 RID: 16742
		Any = -1,
		// Token: 0x04004167 RID: 16743
		Square,
		// Token: 0x04004168 RID: 16744
		Triangle
	}
}
