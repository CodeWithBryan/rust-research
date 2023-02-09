using System;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public class AIInformationGrid : MonoBehaviour
{
	// Token: 0x06001813 RID: 6163 RVA: 0x000B1FF4 File Offset: 0x000B01F4
	[ContextMenu("Init")]
	public void Init()
	{
		AIInformationZone component = base.GetComponent<AIInformationZone>();
		if (component == null)
		{
			Debug.LogWarning("Unable to Init AIInformationGrid, no AIInformationZone found!");
			return;
		}
		this.BoundingBox = component.bounds;
		this.BoundingBox.center = base.transform.position + component.bounds.center + new Vector3(0f, this.BoundingBox.extents.y, 0f);
		float num = this.BoundingBox.extents.x * 2f;
		float num2 = this.BoundingBox.extents.z * 2f;
		this.xCellCount = (int)Mathf.Ceil(num / (float)this.CellSize);
		this.zCellCount = (int)Mathf.Ceil(num2 / (float)this.CellSize);
		this.Cells = new AIInformationCell[this.xCellCount * this.zCellCount];
		Vector3 min = this.BoundingBox.min;
		this.origin = min;
		min.x = this.BoundingBox.min.x + (float)this.CellSize / 2f;
		min.z = this.BoundingBox.min.z + (float)this.CellSize / 2f;
		for (int i = 0; i < this.zCellCount; i++)
		{
			for (int j = 0; j < this.xCellCount; j++)
			{
				Vector3 center = min;
				Bounds bounds = new Bounds(center, new Vector3((float)this.CellSize, this.BoundingBox.extents.y * 2f, (float)this.CellSize));
				this.Cells[this.GetIndex(j, i)] = new AIInformationCell(bounds, base.gameObject, j, i);
				min.x += (float)this.CellSize;
			}
			min.x = this.BoundingBox.min.x + (float)this.CellSize / 2f;
			min.z += (float)this.CellSize;
		}
	}

	// Token: 0x06001814 RID: 6164 RVA: 0x000B220F File Offset: 0x000B040F
	private int GetIndex(int x, int z)
	{
		return z * this.xCellCount + x;
	}

	// Token: 0x06001815 RID: 6165 RVA: 0x000B221B File Offset: 0x000B041B
	public AIInformationCell CellAt(int x, int z)
	{
		return this.Cells[this.GetIndex(x, z)];
	}

	// Token: 0x06001816 RID: 6166 RVA: 0x000B222C File Offset: 0x000B042C
	public AIMovePoint[] GetMovePointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		pointCount = 0;
		int num;
		AIInformationCell[] cellsInRange = this.GetCellsInRange(position, maxRange, out num);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				if (cellsInRange[i] != null)
				{
					foreach (AIMovePoint aimovePoint in cellsInRange[i].MovePoints.Items)
					{
						this.movePointResults[pointCount] = aimovePoint;
						pointCount++;
					}
				}
			}
		}
		return this.movePointResults;
	}

	// Token: 0x06001817 RID: 6167 RVA: 0x000B22BC File Offset: 0x000B04BC
	public AICoverPoint[] GetCoverPointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		pointCount = 0;
		int num;
		AIInformationCell[] cellsInRange = this.GetCellsInRange(position, maxRange, out num);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				if (cellsInRange[i] != null)
				{
					foreach (AICoverPoint aicoverPoint in cellsInRange[i].CoverPoints.Items)
					{
						this.coverPointResults[pointCount] = aicoverPoint;
						pointCount++;
					}
				}
			}
		}
		return this.coverPointResults;
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x000B234C File Offset: 0x000B054C
	public AIInformationCell[] GetCellsInRange(Vector3 position, float maxRange, out int cellCount)
	{
		cellCount = 0;
		int num = (int)(maxRange / (float)this.CellSize);
		AIInformationCell cell = this.GetCell(position);
		if (cell == null)
		{
			return this.resultCells;
		}
		int num2 = Mathf.Max(cell.X - num, 0);
		int num3 = Mathf.Min(cell.X + num, this.xCellCount - 1);
		int num4 = Mathf.Max(cell.Z - num, 0);
		int num5 = Mathf.Min(cell.Z + num, this.zCellCount - 1);
		for (int i = num4; i <= num5; i++)
		{
			for (int j = num2; j <= num3; j++)
			{
				this.resultCells[cellCount] = this.CellAt(j, i);
				cellCount++;
				if (cellCount >= 512)
				{
					return this.resultCells;
				}
			}
		}
		return this.resultCells;
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x000B2418 File Offset: 0x000B0618
	public AIInformationCell GetCell(Vector3 position)
	{
		if (this.Cells == null)
		{
			return null;
		}
		Vector3 vector = position - this.origin;
		if (vector.x < 0f || vector.z < 0f)
		{
			return null;
		}
		int num = (int)(vector.x / (float)this.CellSize);
		int num2 = (int)(vector.z / (float)this.CellSize);
		if (num < 0 || num >= this.xCellCount)
		{
			return null;
		}
		if (num2 < 0 || num2 >= this.zCellCount)
		{
			return null;
		}
		return this.CellAt(num, num2);
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x000B249E File Offset: 0x000B069E
	public void OnDrawGizmos()
	{
		this.DebugDraw();
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x000B24A8 File Offset: 0x000B06A8
	public void DebugDraw()
	{
		if (this.Cells == null)
		{
			return;
		}
		foreach (AIInformationCell aiinformationCell in this.Cells)
		{
			if (aiinformationCell != null)
			{
				aiinformationCell.DebugDraw(Color.white, false, 1f);
			}
		}
	}

	// Token: 0x04001150 RID: 4432
	public int CellSize = 10;

	// Token: 0x04001151 RID: 4433
	public Bounds BoundingBox;

	// Token: 0x04001152 RID: 4434
	public AIInformationCell[] Cells;

	// Token: 0x04001153 RID: 4435
	private Vector3 origin;

	// Token: 0x04001154 RID: 4436
	private int xCellCount;

	// Token: 0x04001155 RID: 4437
	private int zCellCount;

	// Token: 0x04001156 RID: 4438
	private const int maxPointResults = 2048;

	// Token: 0x04001157 RID: 4439
	private AIMovePoint[] movePointResults = new AIMovePoint[2048];

	// Token: 0x04001158 RID: 4440
	private AICoverPoint[] coverPointResults = new AICoverPoint[2048];

	// Token: 0x04001159 RID: 4441
	private const int maxCellResults = 512;

	// Token: 0x0400115A RID: 4442
	private AIInformationCell[] resultCells = new AIInformationCell[512];
}
