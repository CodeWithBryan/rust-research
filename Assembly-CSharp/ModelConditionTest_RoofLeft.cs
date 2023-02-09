using System;
using UnityEngine;

// Token: 0x02000249 RID: 585
public class ModelConditionTest_RoofLeft : ModelConditionTest
{
	// Token: 0x17000211 RID: 529
	// (get) Token: 0x06001B42 RID: 6978 RVA: 0x000BE7CB File Offset: 0x000BC9CB
	private bool IsConvex
	{
		get
		{
			return this.angle > (ModelConditionTest_RoofLeft.AngleType)10;
		}
	}

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x06001B43 RID: 6979 RVA: 0x000BE7D7 File Offset: 0x000BC9D7
	private bool IsConcave
	{
		get
		{
			return this.angle < (ModelConditionTest_RoofLeft.AngleType)(-10);
		}
	}

	// Token: 0x06001B44 RID: 6980 RVA: 0x000BE7E4 File Offset: 0x000BC9E4
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(3f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	// Token: 0x06001B45 RID: 6981 RVA: 0x000BE838 File Offset: 0x000BCA38
	public override bool DoTest(BaseEntity ent)
	{
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if (buildingBlock == null)
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink(ModelConditionTest_RoofLeft.sockets_left);
		if (entityLink == null)
		{
			return false;
		}
		if (this.angle == ModelConditionTest_RoofLeft.AngleType.None)
		{
			for (int i = 0; i < entityLink.connections.Count; i++)
			{
				if (entityLink.connections[i].name.EndsWith("sockets/neighbour/3"))
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
			if (entityLink2.name.EndsWith("sockets/neighbour/3") && (this.shape != ModelConditionTest_RoofLeft.ShapeType.Square || entityLink2.name.StartsWith("roof/")) && (this.shape != ModelConditionTest_RoofLeft.ShapeType.Triangle || entityLink2.name.StartsWith("roof.triangle/")))
			{
				BuildingBlock buildingBlock2 = entityLink2.owner as BuildingBlock;
				if (!(buildingBlock2 == null) && buildingBlock2.grade == buildingBlock.grade)
				{
					int num = (int)this.angle;
					float num2 = Vector3.SignedAngle(ent.transform.forward, buildingBlock2.transform.forward, Vector3.up);
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

	// Token: 0x0400144E RID: 5198
	public ModelConditionTest_RoofLeft.AngleType angle = ModelConditionTest_RoofLeft.AngleType.None;

	// Token: 0x0400144F RID: 5199
	public ModelConditionTest_RoofLeft.ShapeType shape = ModelConditionTest_RoofLeft.ShapeType.Any;

	// Token: 0x04001450 RID: 5200
	private const string roof_square = "roof/";

	// Token: 0x04001451 RID: 5201
	private const string roof_triangle = "roof.triangle/";

	// Token: 0x04001452 RID: 5202
	private const string socket_right = "sockets/neighbour/3";

	// Token: 0x04001453 RID: 5203
	private const string socket_left = "sockets/neighbour/4";

	// Token: 0x04001454 RID: 5204
	private static string[] sockets_left = new string[]
	{
		"roof/sockets/neighbour/4",
		"roof.triangle/sockets/neighbour/4"
	};

	// Token: 0x02000C35 RID: 3125
	public enum AngleType
	{
		// Token: 0x0400414E RID: 16718
		None = -1,
		// Token: 0x0400414F RID: 16719
		Straight,
		// Token: 0x04004150 RID: 16720
		Convex60 = 60,
		// Token: 0x04004151 RID: 16721
		Convex90 = 90,
		// Token: 0x04004152 RID: 16722
		Convex120 = 120,
		// Token: 0x04004153 RID: 16723
		Concave30 = -30,
		// Token: 0x04004154 RID: 16724
		Concave60 = -60,
		// Token: 0x04004155 RID: 16725
		Concave90 = -90,
		// Token: 0x04004156 RID: 16726
		Concave120 = -120
	}

	// Token: 0x02000C36 RID: 3126
	public enum ShapeType
	{
		// Token: 0x04004158 RID: 16728
		Any = -1,
		// Token: 0x04004159 RID: 16729
		Square,
		// Token: 0x0400415A RID: 16730
		Triangle
	}
}
