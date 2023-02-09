using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200091E RID: 2334
public class TickInterpolator
{
	// Token: 0x0600379F RID: 14239 RVA: 0x00149F36 File Offset: 0x00148136
	public void Reset()
	{
		this.index = 0;
		this.CurrentPoint = this.StartPoint;
	}

	// Token: 0x060037A0 RID: 14240 RVA: 0x00149F4C File Offset: 0x0014814C
	public void Reset(Vector3 point)
	{
		this.points.Clear();
		this.index = 0;
		this.Length = 0f;
		this.EndPoint = point;
		this.StartPoint = point;
		this.CurrentPoint = point;
	}

	// Token: 0x060037A1 RID: 14241 RVA: 0x00149F90 File Offset: 0x00148190
	public void AddPoint(Vector3 point)
	{
		TickInterpolator.Segment segment = new TickInterpolator.Segment(this.EndPoint, point);
		this.points.Add(segment);
		this.Length += segment.length;
		this.EndPoint = segment.point;
	}

	// Token: 0x060037A2 RID: 14242 RVA: 0x00149FD8 File Offset: 0x001481D8
	public bool MoveNext(float distance)
	{
		float num = 0f;
		while (num < distance && this.index < this.points.Count)
		{
			TickInterpolator.Segment segment = this.points[this.index];
			this.CurrentPoint = segment.point;
			num += segment.length;
			this.index++;
		}
		return num > 0f;
	}

	// Token: 0x060037A3 RID: 14243 RVA: 0x0014A041 File Offset: 0x00148241
	public bool HasNext()
	{
		return this.index < this.points.Count;
	}

	// Token: 0x060037A4 RID: 14244 RVA: 0x0014A058 File Offset: 0x00148258
	public void TransformEntries(Matrix4x4 matrix)
	{
		for (int i = 0; i < this.points.Count; i++)
		{
			TickInterpolator.Segment segment = this.points[i];
			segment.point = matrix.MultiplyPoint3x4(segment.point);
			this.points[i] = segment;
		}
		this.CurrentPoint = matrix.MultiplyPoint3x4(this.CurrentPoint);
		this.StartPoint = matrix.MultiplyPoint3x4(this.StartPoint);
		this.EndPoint = matrix.MultiplyPoint3x4(this.EndPoint);
	}

	// Token: 0x040031D4 RID: 12756
	private List<TickInterpolator.Segment> points = new List<TickInterpolator.Segment>();

	// Token: 0x040031D5 RID: 12757
	private int index;

	// Token: 0x040031D6 RID: 12758
	public float Length;

	// Token: 0x040031D7 RID: 12759
	public Vector3 CurrentPoint;

	// Token: 0x040031D8 RID: 12760
	public Vector3 StartPoint;

	// Token: 0x040031D9 RID: 12761
	public Vector3 EndPoint;

	// Token: 0x02000E64 RID: 3684
	private struct Segment
	{
		// Token: 0x06005079 RID: 20601 RVA: 0x001A1ADE File Offset: 0x0019FCDE
		public Segment(Vector3 a, Vector3 b)
		{
			this.point = b;
			this.length = Vector3.Distance(a, b);
		}

		// Token: 0x04004A46 RID: 19014
		public Vector3 point;

		// Token: 0x04004A47 RID: 19015
		public float length;
	}
}
