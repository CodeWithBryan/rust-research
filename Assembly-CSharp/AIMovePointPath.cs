using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C9 RID: 457
public class AIMovePointPath : MonoBehaviour
{
	// Token: 0x0600184B RID: 6219 RVA: 0x000B3A02 File Offset: 0x000B1C02
	public void Clear()
	{
		this.Points.Clear();
	}

	// Token: 0x0600184C RID: 6220 RVA: 0x000B3A0F File Offset: 0x000B1C0F
	public void AddPoint(AIMovePoint point)
	{
		this.Points.Add(point);
	}

	// Token: 0x0600184D RID: 6221 RVA: 0x000B3A1D File Offset: 0x000B1C1D
	public AIMovePoint FindNearestPoint(Vector3 position)
	{
		return this.Points[this.FindNearestPointIndex(position)];
	}

	// Token: 0x0600184E RID: 6222 RVA: 0x000B3A34 File Offset: 0x000B1C34
	public int FindNearestPointIndex(Vector3 position)
	{
		float num = float.MaxValue;
		int result = 0;
		int num2 = 0;
		foreach (AIMovePoint aimovePoint in this.Points)
		{
			float num3 = Vector3.SqrMagnitude(position - aimovePoint.transform.position);
			if (num3 < num)
			{
				num = num3;
				result = num2;
			}
			num2++;
		}
		return result;
	}

	// Token: 0x0600184F RID: 6223 RVA: 0x000B3AB4 File Offset: 0x000B1CB4
	public AIMovePoint GetPointAtIndex(int index)
	{
		if (index < 0 || index >= this.Points.Count)
		{
			return null;
		}
		return this.Points[index];
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x000B3AD8 File Offset: 0x000B1CD8
	public int GetNextPointIndex(int currentPointIndex, ref AIMovePointPath.PathDirection pathDirection)
	{
		int num = currentPointIndex + ((pathDirection == AIMovePointPath.PathDirection.Forwards) ? 1 : -1);
		if (num < 0)
		{
			if (this.LoopMode == AIMovePointPath.Mode.Loop)
			{
				num = this.Points.Count - 1;
			}
			else
			{
				num = 1;
				pathDirection = AIMovePointPath.PathDirection.Forwards;
			}
		}
		else if (num >= this.Points.Count)
		{
			if (this.LoopMode == AIMovePointPath.Mode.Loop)
			{
				num = 0;
			}
			else
			{
				num = this.Points.Count - 2;
				pathDirection = AIMovePointPath.PathDirection.Backwards;
			}
		}
		return num;
	}

	// Token: 0x06001851 RID: 6225 RVA: 0x000B3B40 File Offset: 0x000B1D40
	private void OnDrawGizmos()
	{
		Color color = Gizmos.color;
		Gizmos.color = this.DebugPathColor;
		int num = -1;
		foreach (AIMovePoint aimovePoint in this.Points)
		{
			num++;
			if (!(aimovePoint == null))
			{
				if (num + 1 < this.Points.Count)
				{
					Gizmos.DrawLine(aimovePoint.transform.position, this.Points[num + 1].transform.position);
				}
				else if (this.LoopMode == AIMovePointPath.Mode.Loop)
				{
					Gizmos.DrawLine(aimovePoint.transform.position, this.Points[0].transform.position);
				}
			}
		}
		Gizmos.color = color;
	}

	// Token: 0x06001852 RID: 6226 RVA: 0x000B3C20 File Offset: 0x000B1E20
	private void OnDrawGizmosSelected()
	{
		if (this.Points == null)
		{
			return;
		}
		foreach (AIMovePoint aimovePoint in this.Points)
		{
			aimovePoint.DrawLookAtPoints();
		}
	}

	// Token: 0x06001853 RID: 6227 RVA: 0x000B3C7C File Offset: 0x000B1E7C
	[ContextMenu("Add Child Points")]
	public void AddChildPoints()
	{
		this.Points = new List<AIMovePoint>();
		this.Points.AddRange(base.GetComponentsInChildren<AIMovePoint>());
	}

	// Token: 0x0400117B RID: 4475
	public Color DebugPathColor = Color.green;

	// Token: 0x0400117C RID: 4476
	public AIMovePointPath.Mode LoopMode;

	// Token: 0x0400117D RID: 4477
	public List<AIMovePoint> Points = new List<AIMovePoint>();

	// Token: 0x02000BEE RID: 3054
	public enum Mode
	{
		// Token: 0x0400403B RID: 16443
		Loop,
		// Token: 0x0400403C RID: 16444
		Reverse
	}

	// Token: 0x02000BEF RID: 3055
	public enum PathDirection
	{
		// Token: 0x0400403E RID: 16446
		Forwards,
		// Token: 0x0400403F RID: 16447
		Backwards
	}
}
