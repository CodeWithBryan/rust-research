using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009F9 RID: 2553
	public class BezierPath
	{
		// Token: 0x06003CBE RID: 15550 RVA: 0x00162E31 File Offset: 0x00161031
		public BezierPath()
		{
			this.controlPoints = new List<Vector2>();
		}

		// Token: 0x06003CBF RID: 15551 RVA: 0x00162E62 File Offset: 0x00161062
		public void SetControlPoints(List<Vector2> newControlPoints)
		{
			this.controlPoints.Clear();
			this.controlPoints.AddRange(newControlPoints);
			this.curveCount = (this.controlPoints.Count - 1) / 3;
		}

		// Token: 0x06003CC0 RID: 15552 RVA: 0x00162E62 File Offset: 0x00161062
		public void SetControlPoints(Vector2[] newControlPoints)
		{
			this.controlPoints.Clear();
			this.controlPoints.AddRange(newControlPoints);
			this.curveCount = (this.controlPoints.Count - 1) / 3;
		}

		// Token: 0x06003CC1 RID: 15553 RVA: 0x00162E90 File Offset: 0x00161090
		public List<Vector2> GetControlPoints()
		{
			return this.controlPoints;
		}

		// Token: 0x06003CC2 RID: 15554 RVA: 0x00162E98 File Offset: 0x00161098
		public void Interpolate(List<Vector2> segmentPoints, float scale)
		{
			this.controlPoints.Clear();
			if (segmentPoints.Count < 2)
			{
				return;
			}
			for (int i = 0; i < segmentPoints.Count; i++)
			{
				if (i == 0)
				{
					Vector2 vector = segmentPoints[i];
					Vector2 a = segmentPoints[i + 1] - vector;
					Vector2 item = vector + scale * a;
					this.controlPoints.Add(vector);
					this.controlPoints.Add(item);
				}
				else if (i == segmentPoints.Count - 1)
				{
					Vector2 b = segmentPoints[i - 1];
					Vector2 vector2 = segmentPoints[i];
					Vector2 a2 = vector2 - b;
					Vector2 item2 = vector2 - scale * a2;
					this.controlPoints.Add(item2);
					this.controlPoints.Add(vector2);
				}
				else
				{
					Vector2 b2 = segmentPoints[i - 1];
					Vector2 vector3 = segmentPoints[i];
					Vector2 a3 = segmentPoints[i + 1];
					Vector2 normalized = (a3 - b2).normalized;
					Vector2 item3 = vector3 - scale * normalized * (vector3 - b2).magnitude;
					Vector2 item4 = vector3 + scale * normalized * (a3 - vector3).magnitude;
					this.controlPoints.Add(item3);
					this.controlPoints.Add(vector3);
					this.controlPoints.Add(item4);
				}
			}
			this.curveCount = (this.controlPoints.Count - 1) / 3;
		}

		// Token: 0x06003CC3 RID: 15555 RVA: 0x00163030 File Offset: 0x00161230
		public void SamplePoints(List<Vector2> sourcePoints, float minSqrDistance, float maxSqrDistance, float scale)
		{
			if (sourcePoints.Count < 2)
			{
				return;
			}
			Stack<Vector2> stack = new Stack<Vector2>();
			stack.Push(sourcePoints[0]);
			Vector2 vector = sourcePoints[1];
			for (int i = 2; i < sourcePoints.Count; i++)
			{
				if ((vector - sourcePoints[i]).sqrMagnitude > minSqrDistance && (stack.Peek() - sourcePoints[i]).sqrMagnitude > maxSqrDistance)
				{
					stack.Push(vector);
				}
				vector = sourcePoints[i];
			}
			Vector2 vector2 = stack.Pop();
			Vector2 vector3 = stack.Peek();
			Vector2 normalized = (vector3 - vector).normalized;
			float magnitude = (vector - vector2).magnitude;
			float magnitude2 = (vector2 - vector3).magnitude;
			vector2 += normalized * ((magnitude2 - magnitude) / 2f);
			stack.Push(vector2);
			stack.Push(vector);
			this.Interpolate(new List<Vector2>(stack), scale);
		}

		// Token: 0x06003CC4 RID: 15556 RVA: 0x00163138 File Offset: 0x00161338
		public Vector2 CalculateBezierPoint(int curveIndex, float t)
		{
			int num = curveIndex * 3;
			Vector2 p = this.controlPoints[num];
			Vector2 p2 = this.controlPoints[num + 1];
			Vector2 p3 = this.controlPoints[num + 2];
			Vector2 p4 = this.controlPoints[num + 3];
			return this.CalculateBezierPoint(t, p, p2, p3, p4);
		}

		// Token: 0x06003CC5 RID: 15557 RVA: 0x00163190 File Offset: 0x00161390
		public List<Vector2> GetDrawingPoints0()
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this.curveCount; i++)
			{
				if (i == 0)
				{
					list.Add(this.CalculateBezierPoint(i, 0f));
				}
				for (int j = 1; j <= this.SegmentsPerCurve; j++)
				{
					float t = (float)j / (float)this.SegmentsPerCurve;
					list.Add(this.CalculateBezierPoint(i, t));
				}
			}
			return list;
		}

		// Token: 0x06003CC6 RID: 15558 RVA: 0x001631F4 File Offset: 0x001613F4
		public List<Vector2> GetDrawingPoints1()
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this.controlPoints.Count - 3; i += 3)
			{
				Vector2 p = this.controlPoints[i];
				Vector2 p2 = this.controlPoints[i + 1];
				Vector2 p3 = this.controlPoints[i + 2];
				Vector2 p4 = this.controlPoints[i + 3];
				if (i == 0)
				{
					list.Add(this.CalculateBezierPoint(0f, p, p2, p3, p4));
				}
				for (int j = 1; j <= this.SegmentsPerCurve; j++)
				{
					float t = (float)j / (float)this.SegmentsPerCurve;
					list.Add(this.CalculateBezierPoint(t, p, p2, p3, p4));
				}
			}
			return list;
		}

		// Token: 0x06003CC7 RID: 15559 RVA: 0x001632B4 File Offset: 0x001614B4
		public List<Vector2> GetDrawingPoints2()
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this.curveCount; i++)
			{
				List<Vector2> list2 = this.FindDrawingPoints(i);
				if (i != 0)
				{
					list2.RemoveAt(0);
				}
				list.AddRange(list2);
			}
			return list;
		}

		// Token: 0x06003CC8 RID: 15560 RVA: 0x001632F4 File Offset: 0x001614F4
		private List<Vector2> FindDrawingPoints(int curveIndex)
		{
			List<Vector2> list = new List<Vector2>();
			Vector2 item = this.CalculateBezierPoint(curveIndex, 0f);
			Vector2 item2 = this.CalculateBezierPoint(curveIndex, 1f);
			list.Add(item);
			list.Add(item2);
			this.FindDrawingPoints(curveIndex, 0f, 1f, list, 1);
			return list;
		}

		// Token: 0x06003CC9 RID: 15561 RVA: 0x00163344 File Offset: 0x00161544
		private int FindDrawingPoints(int curveIndex, float t0, float t1, List<Vector2> pointList, int insertionIndex)
		{
			Vector2 a = this.CalculateBezierPoint(curveIndex, t0);
			Vector2 vector = this.CalculateBezierPoint(curveIndex, t1);
			if ((a - vector).sqrMagnitude < this.MINIMUM_SQR_DISTANCE)
			{
				return 0;
			}
			float num = (t0 + t1) / 2f;
			Vector2 vector2 = this.CalculateBezierPoint(curveIndex, num);
			Vector2 normalized = (a - vector2).normalized;
			Vector2 normalized2 = (vector - vector2).normalized;
			if (Vector2.Dot(normalized, normalized2) > this.DIVISION_THRESHOLD || Mathf.Abs(num - 0.5f) < 0.0001f)
			{
				int num2 = 0;
				num2 += this.FindDrawingPoints(curveIndex, t0, num, pointList, insertionIndex);
				pointList.Insert(insertionIndex + num2, vector2);
				num2++;
				return num2 + this.FindDrawingPoints(curveIndex, num, t1, pointList, insertionIndex + num2);
			}
			return 0;
		}

		// Token: 0x06003CCA RID: 15562 RVA: 0x00163418 File Offset: 0x00161618
		private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			float num = 1f - t;
			float num2 = t * t;
			float num3 = num * num;
			float d = num3 * num;
			float d2 = num2 * t;
			return d * p0 + 3f * num3 * t * p1 + 3f * num * num2 * p2 + d2 * p3;
		}

		// Token: 0x040035F1 RID: 13809
		public int SegmentsPerCurve = 10;

		// Token: 0x040035F2 RID: 13810
		public float MINIMUM_SQR_DISTANCE = 0.01f;

		// Token: 0x040035F3 RID: 13811
		public float DIVISION_THRESHOLD = -0.99f;

		// Token: 0x040035F4 RID: 13812
		private List<Vector2> controlPoints;

		// Token: 0x040035F5 RID: 13813
		private int curveCount;
	}
}
