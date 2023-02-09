using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class AIInformationCell
{
	// Token: 0x170001CE RID: 462
	// (get) Token: 0x06001808 RID: 6152 RVA: 0x000B1DCC File Offset: 0x000AFFCC
	public int X { get; }

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x06001809 RID: 6153 RVA: 0x000B1DD4 File Offset: 0x000AFFD4
	public int Z { get; }

	// Token: 0x0600180A RID: 6154 RVA: 0x000B1DDC File Offset: 0x000AFFDC
	public AIInformationCell(Bounds bounds, GameObject root, int x, int z)
	{
		this.BoundingBox = bounds;
		this.X = x;
		this.Z = z;
		this.MovePoints.Init(bounds, root);
		this.CoverPoints.Init(bounds, root);
	}

	// Token: 0x0600180B RID: 6155 RVA: 0x000B1E40 File Offset: 0x000B0040
	public void DebugDraw(Color color, bool points, float scale = 1f)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawWireCube(this.BoundingBox.center, this.BoundingBox.size * scale);
		Gizmos.color = color2;
		if (points)
		{
			foreach (AIMovePoint aimovePoint in this.MovePoints.Items)
			{
				Gizmos.DrawLine(this.BoundingBox.center, aimovePoint.transform.position);
			}
			foreach (AICoverPoint aicoverPoint in this.CoverPoints.Items)
			{
				Gizmos.DrawLine(this.BoundingBox.center, aicoverPoint.transform.position);
			}
		}
	}

	// Token: 0x04001149 RID: 4425
	public Bounds BoundingBox;

	// Token: 0x0400114A RID: 4426
	public List<AIInformationCell> NeighbourCells = new List<AIInformationCell>();

	// Token: 0x0400114B RID: 4427
	public AIInformationCellContents<AIMovePoint> MovePoints = new AIInformationCellContents<AIMovePoint>();

	// Token: 0x0400114C RID: 4428
	public AIInformationCellContents<AICoverPoint> CoverPoints = new AIInformationCellContents<AICoverPoint>();
}
