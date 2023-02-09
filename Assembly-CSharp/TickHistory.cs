using System;
using UnityEngine;

// Token: 0x0200091D RID: 2333
public class TickHistory
{
	// Token: 0x17000436 RID: 1078
	// (get) Token: 0x06003798 RID: 14232 RVA: 0x00149DA2 File Offset: 0x00147FA2
	public int Count
	{
		get
		{
			return this.points.Count;
		}
	}

	// Token: 0x06003799 RID: 14233 RVA: 0x00149DAF File Offset: 0x00147FAF
	public void Reset()
	{
		this.points.Clear();
	}

	// Token: 0x0600379A RID: 14234 RVA: 0x00149DBC File Offset: 0x00147FBC
	public void Reset(Vector3 point)
	{
		this.Reset();
		this.AddPoint(point, -1);
	}

	// Token: 0x0600379B RID: 14235 RVA: 0x00149DCC File Offset: 0x00147FCC
	public float Distance(BasePlayer player, Vector3 point)
	{
		if (this.points.Count == 0)
		{
			return player.Distance(point);
		}
		Vector3 position = player.transform.position;
		Quaternion rotation = player.transform.rotation;
		Bounds bounds = player.bounds;
		Matrix4x4 tickHistoryMatrix = player.tickHistoryMatrix;
		float num = float.MaxValue;
		for (int i = 0; i < this.points.Count; i++)
		{
			Vector3 point2 = tickHistoryMatrix.MultiplyPoint3x4(this.points[i]);
			Vector3 point3 = (i == this.points.Count - 1) ? position : tickHistoryMatrix.MultiplyPoint3x4(this.points[i + 1]);
			Line line = new Line(point2, point3);
			Vector3 position2 = line.ClosestPoint(point);
			OBB obb = new OBB(position2, rotation, bounds);
			num = Mathf.Min(num, obb.Distance(point));
		}
		return num;
	}

	// Token: 0x0600379C RID: 14236 RVA: 0x00149EAD File Offset: 0x001480AD
	public void AddPoint(Vector3 point, int limit = -1)
	{
		while (limit > 0 && this.points.Count >= limit)
		{
			this.points.PopFront();
		}
		this.points.PushBack(point);
	}

	// Token: 0x0600379D RID: 14237 RVA: 0x00149EDC File Offset: 0x001480DC
	public void TransformEntries(Matrix4x4 matrix)
	{
		for (int i = 0; i < this.points.Count; i++)
		{
			Vector3 vector = this.points[i];
			vector = matrix.MultiplyPoint3x4(vector);
			this.points[i] = vector;
		}
	}

	// Token: 0x040031D3 RID: 12755
	private Deque<Vector3> points = new Deque<Vector3>(8);
}
