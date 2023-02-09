using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000923 RID: 2339
[Serializable]
public class WorldSplineData
{
	// Token: 0x060037CC RID: 14284 RVA: 0x0014AA94 File Offset: 0x00148C94
	public WorldSplineData(WorldSpline worldSpline)
	{
		worldSpline.CheckValidity();
		this.LUTValues = new List<WorldSplineData.LUTEntry>();
		this.inputPoints = new Vector3[worldSpline.points.Length];
		worldSpline.points.CopyTo(this.inputPoints, 0);
		this.inputTangents = new Vector3[worldSpline.tangents.Length];
		worldSpline.tangents.CopyTo(this.inputTangents, 0);
		this.inputLUTInterval = worldSpline.lutInterval;
		this.maxPointsIndex = this.inputPoints.Length - 1;
		this.CreateLookupTable(worldSpline);
	}

	// Token: 0x060037CD RID: 14285 RVA: 0x0014AB25 File Offset: 0x00148D25
	public bool IsSameAs(WorldSpline worldSpline)
	{
		return this.inputPoints.SequenceEqual(worldSpline.points) && this.inputTangents.SequenceEqual(worldSpline.tangents) && this.inputLUTInterval == worldSpline.lutInterval;
	}

	// Token: 0x060037CE RID: 14286 RVA: 0x0014AB5D File Offset: 0x00148D5D
	public bool IsDifferentTo(WorldSpline worldSpline)
	{
		return !this.IsSameAs(worldSpline);
	}

	// Token: 0x060037CF RID: 14287 RVA: 0x0014AB69 File Offset: 0x00148D69
	public Vector3 GetStartPoint()
	{
		return this.inputPoints[0];
	}

	// Token: 0x060037D0 RID: 14288 RVA: 0x0014AB77 File Offset: 0x00148D77
	public Vector3 GetEndPoint()
	{
		return this.inputPoints[this.maxPointsIndex];
	}

	// Token: 0x060037D1 RID: 14289 RVA: 0x0014AB8A File Offset: 0x00148D8A
	public Vector3 GetStartTangent()
	{
		return this.inputTangents[0];
	}

	// Token: 0x060037D2 RID: 14290 RVA: 0x0014AB98 File Offset: 0x00148D98
	public Vector3 GetEndTangent()
	{
		return this.inputTangents[this.maxPointsIndex];
	}

	// Token: 0x060037D3 RID: 14291 RVA: 0x0014ABAC File Offset: 0x00148DAC
	public Vector3 GetPointCubicHermite(float distance)
	{
		Vector3 vector;
		return this.GetPointAndTangentCubicHermite(distance, out vector);
	}

	// Token: 0x060037D4 RID: 14292 RVA: 0x0014ABC4 File Offset: 0x00148DC4
	public Vector3 GetTangentCubicHermite(float distance)
	{
		Vector3 result;
		this.GetPointAndTangentCubicHermite(distance, out result);
		return result;
	}

	// Token: 0x060037D5 RID: 14293 RVA: 0x0014ABDC File Offset: 0x00148DDC
	public Vector3 GetPointAndTangentCubicHermite(float distance, out Vector3 tangent)
	{
		if (distance <= 0f)
		{
			tangent = this.GetStartTangent();
			return this.GetStartPoint();
		}
		if (distance >= this.Length)
		{
			tangent = this.GetEndTangent();
			return this.GetEndPoint();
		}
		int num = Mathf.FloorToInt(distance);
		if (this.LUTValues.Count > num)
		{
			int num2 = -1;
			while (num2 < 0 && (float)num > 0f)
			{
				WorldSplineData.LUTEntry lutentry = this.LUTValues[num];
				int num3 = 0;
				while (num3 < lutentry.points.Count && lutentry.points[num3].distance <= distance)
				{
					num2 = num3;
					num3++;
				}
				if (num2 < 0)
				{
					num--;
				}
			}
			float a;
			Vector3 vector;
			if (num2 < 0)
			{
				a = 0f;
				vector = this.GetStartPoint();
			}
			else
			{
				WorldSplineData.LUTEntry.LUTPoint lutpoint = this.LUTValues[num].points[num2];
				a = lutpoint.distance;
				vector = lutpoint.pos;
			}
			num2 = -1;
			while (num2 < 0 && num < this.LUTValues.Count)
			{
				WorldSplineData.LUTEntry lutentry2 = this.LUTValues[num];
				for (int i = 0; i < lutentry2.points.Count; i++)
				{
					if (lutentry2.points[i].distance > distance)
					{
						num2 = i;
						break;
					}
				}
				if (num2 < 0)
				{
					num++;
				}
			}
			float b;
			Vector3 vector2;
			if (num2 < 0)
			{
				b = this.Length;
				vector2 = this.GetEndPoint();
			}
			else
			{
				WorldSplineData.LUTEntry.LUTPoint lutpoint2 = this.LUTValues[num].points[num2];
				b = lutpoint2.distance;
				vector2 = lutpoint2.pos;
			}
			float t = Mathf.InverseLerp(a, b, distance);
			tangent = (vector2 - vector).normalized;
			return Vector3.Lerp(vector, vector2, t);
		}
		tangent = this.GetEndTangent();
		return this.GetEndPoint();
	}

	// Token: 0x060037D6 RID: 14294 RVA: 0x0014ADA8 File Offset: 0x00148FA8
	public void SetDefaultTangents(WorldSpline worldSpline)
	{
		PathInterpolator pathInterpolator = new PathInterpolator(worldSpline.points, worldSpline.tangents);
		pathInterpolator.RecalculateTangents();
		worldSpline.tangents = pathInterpolator.Tangents;
	}

	// Token: 0x060037D7 RID: 14295 RVA: 0x0014ADDC File Offset: 0x00148FDC
	public bool DetectSplineProblems(WorldSpline worldSpline)
	{
		bool result = false;
		Vector3 to = this.GetTangentCubicHermite(0f);
		for (float num = 0.05f; num <= this.Length; num += 0.05f)
		{
			Vector3 tangentCubicHermite = this.GetTangentCubicHermite(num);
			float num2 = Vector3.Angle(tangentCubicHermite, to);
			if (num2 > 5f)
			{
				if (worldSpline != null)
				{
					Vector3 dir;
					Vector3 pointAndTangentCubicHermiteWorld = worldSpline.GetPointAndTangentCubicHermiteWorld(num, out dir);
					Debug.DrawRay(pointAndTangentCubicHermiteWorld, dir, Color.red, 30f);
					Debug.DrawRay(pointAndTangentCubicHermiteWorld, Vector3.up, Color.red, 30f);
				}
				Debug.Log(string.Format("Spline may have a too-sharp bend at {0:P0}. Angle change: ", num / this.Length) + num2);
				result = true;
			}
			to = tangentCubicHermite;
		}
		return result;
	}

	// Token: 0x060037D8 RID: 14296 RVA: 0x0014AE90 File Offset: 0x00149090
	private void CreateLookupTable(WorldSpline worldSpline)
	{
		PathInterpolator pathInterpolator = new PathInterpolator(worldSpline.points, worldSpline.tangents);
		Vector3 b = pathInterpolator.GetPointCubicHermite(0f);
		this.Length = 0f;
		this.AddEntry(0f, this.GetStartPoint());
		Vector3 vector;
		for (float num = worldSpline.lutInterval; num < pathInterpolator.Length; num += worldSpline.lutInterval)
		{
			vector = pathInterpolator.GetPointCubicHermite(num);
			this.Length += Vector3.Distance(vector, b);
			this.AddEntry(this.Length, pathInterpolator.GetPointCubicHermite(num));
			b = vector;
		}
		vector = this.GetEndPoint();
		this.Length += Vector3.Distance(vector, b);
		this.AddEntry(this.Length, vector);
	}

	// Token: 0x060037D9 RID: 14297 RVA: 0x0014AF4C File Offset: 0x0014914C
	private void AddEntry(float distance, Vector3 pos)
	{
		int num = Mathf.FloorToInt(distance);
		if (this.LUTValues.Count < num + 1)
		{
			for (int i = this.LUTValues.Count; i < num + 1; i++)
			{
				this.LUTValues.Add(new WorldSplineData.LUTEntry());
			}
		}
		this.LUTValues[num].points.Add(new WorldSplineData.LUTEntry.LUTPoint(distance, pos));
	}

	// Token: 0x040031EA RID: 12778
	public Vector3[] inputPoints;

	// Token: 0x040031EB RID: 12779
	public Vector3[] inputTangents;

	// Token: 0x040031EC RID: 12780
	public float inputLUTInterval;

	// Token: 0x040031ED RID: 12781
	public List<WorldSplineData.LUTEntry> LUTValues;

	// Token: 0x040031EE RID: 12782
	public float Length;

	// Token: 0x040031EF RID: 12783
	[SerializeField]
	private int maxPointsIndex;

	// Token: 0x02000E66 RID: 3686
	[Serializable]
	public class LUTEntry
	{
		// Token: 0x04004A4A RID: 19018
		public List<WorldSplineData.LUTEntry.LUTPoint> points = new List<WorldSplineData.LUTEntry.LUTPoint>();

		// Token: 0x02000F6F RID: 3951
		[Serializable]
		public struct LUTPoint
		{
			// Token: 0x0600526E RID: 21102 RVA: 0x001A7231 File Offset: 0x001A5431
			public LUTPoint(float distance, Vector3 pos)
			{
				this.distance = distance;
				this.pos = pos;
			}

			// Token: 0x04004E47 RID: 20039
			public float distance;

			// Token: 0x04004E48 RID: 20040
			public Vector3 pos;
		}
	}
}
