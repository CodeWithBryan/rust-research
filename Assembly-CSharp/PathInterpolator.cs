using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000917 RID: 2327
public class PathInterpolator
{
	// Token: 0x17000428 RID: 1064
	// (get) Token: 0x0600375E RID: 14174 RVA: 0x00148CAB File Offset: 0x00146EAB
	// (set) Token: 0x0600375F RID: 14175 RVA: 0x00148CB3 File Offset: 0x00146EB3
	public int MinIndex { get; set; }

	// Token: 0x17000429 RID: 1065
	// (get) Token: 0x06003760 RID: 14176 RVA: 0x00148CBC File Offset: 0x00146EBC
	// (set) Token: 0x06003761 RID: 14177 RVA: 0x00148CC4 File Offset: 0x00146EC4
	public int MaxIndex { get; set; }

	// Token: 0x1700042A RID: 1066
	// (get) Token: 0x06003762 RID: 14178 RVA: 0x00148CCD File Offset: 0x00146ECD
	// (set) Token: 0x06003763 RID: 14179 RVA: 0x00148CD5 File Offset: 0x00146ED5
	public virtual float Length { get; private set; }

	// Token: 0x1700042B RID: 1067
	// (get) Token: 0x06003764 RID: 14180 RVA: 0x00148CDE File Offset: 0x00146EDE
	// (set) Token: 0x06003765 RID: 14181 RVA: 0x00148CE6 File Offset: 0x00146EE6
	public virtual float StepSize { get; private set; }

	// Token: 0x1700042C RID: 1068
	// (get) Token: 0x06003766 RID: 14182 RVA: 0x00148CEF File Offset: 0x00146EEF
	// (set) Token: 0x06003767 RID: 14183 RVA: 0x00148CF7 File Offset: 0x00146EF7
	public bool Circular { get; private set; }

	// Token: 0x1700042D RID: 1069
	// (get) Token: 0x06003768 RID: 14184 RVA: 0x00007074 File Offset: 0x00005274
	public int DefaultMinIndex
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x1700042E RID: 1070
	// (get) Token: 0x06003769 RID: 14185 RVA: 0x00148D00 File Offset: 0x00146F00
	public int DefaultMaxIndex
	{
		get
		{
			return this.Points.Length - 1;
		}
	}

	// Token: 0x1700042F RID: 1071
	// (get) Token: 0x0600376A RID: 14186 RVA: 0x00148D0C File Offset: 0x00146F0C
	public float StartOffset
	{
		get
		{
			return this.Length * (float)(this.MinIndex - this.DefaultMinIndex) / (float)(this.DefaultMaxIndex - this.DefaultMinIndex);
		}
	}

	// Token: 0x17000430 RID: 1072
	// (get) Token: 0x0600376B RID: 14187 RVA: 0x00148D32 File Offset: 0x00146F32
	public float EndOffset
	{
		get
		{
			return this.Length * (float)(this.DefaultMaxIndex - this.MaxIndex) / (float)(this.DefaultMaxIndex - this.DefaultMinIndex);
		}
	}

	// Token: 0x0600376C RID: 14188 RVA: 0x00148D58 File Offset: 0x00146F58
	public PathInterpolator(Vector3[] points)
	{
		if (points.Length < 2)
		{
			throw new ArgumentException("Point list too short.");
		}
		this.Points = points;
		this.MinIndex = this.DefaultMinIndex;
		this.MaxIndex = this.DefaultMaxIndex;
		this.Circular = (Vector3.Distance(points[0], points[points.Length - 1]) < 0.1f);
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x00148DC0 File Offset: 0x00146FC0
	public PathInterpolator(Vector3[] points, Vector3[] tangents) : this(points)
	{
		if (tangents.Length != points.Length)
		{
			throw new ArgumentException(string.Concat(new object[]
			{
				"Points and tangents lengths must match. Points: ",
				points.Length,
				" Tangents: ",
				tangents.Length
			}));
		}
		this.Tangents = tangents;
		this.RecalculateLength();
		this.initialized = true;
	}

	// Token: 0x0600376E RID: 14190 RVA: 0x00148E28 File Offset: 0x00147028
	public void RecalculateTangents()
	{
		if (this.Tangents == null || this.Tangents.Length != this.Points.Length)
		{
			this.Tangents = new Vector3[this.Points.Length];
		}
		for (int i = 0; i < this.Points.Length; i++)
		{
			int num = i - 1;
			int num2 = i + 1;
			if (num < 0)
			{
				num = (this.Circular ? (this.Points.Length - 2) : 0);
			}
			if (num2 > this.Points.Length - 1)
			{
				num2 = (this.Circular ? 1 : (this.Points.Length - 1));
			}
			Vector3 b = this.Points[num];
			Vector3 a = this.Points[num2];
			this.Tangents[i] = (a - b).normalized;
		}
		this.RecalculateLength();
		this.initialized = true;
	}

	// Token: 0x0600376F RID: 14191 RVA: 0x00148F08 File Offset: 0x00147108
	public void RecalculateLength()
	{
		float num = 0f;
		for (int i = 0; i < this.Points.Length - 1; i++)
		{
			Vector3 b = this.Points[i];
			Vector3 a = this.Points[i + 1];
			num += (a - b).magnitude;
		}
		this.Length = num;
		this.StepSize = num / (float)this.Points.Length;
	}

	// Token: 0x06003770 RID: 14192 RVA: 0x00148F78 File Offset: 0x00147178
	public void Resample(float distance)
	{
		float num = 0f;
		for (int i = 0; i < this.Points.Length - 1; i++)
		{
			Vector3 b = this.Points[i];
			Vector3 a = this.Points[i + 1];
			num += (a - b).magnitude;
		}
		int num2 = Mathf.RoundToInt(num / distance);
		if (num2 < 2)
		{
			return;
		}
		distance = num / (float)(num2 - 1);
		List<Vector3> list = new List<Vector3>(num2);
		float num3 = 0f;
		for (int j = 0; j < this.Points.Length - 1; j++)
		{
			int num4 = j;
			int num5 = j + 1;
			Vector3 vector = this.Points[num4];
			Vector3 vector2 = this.Points[num5];
			float num6 = (vector2 - vector).magnitude;
			if (num4 == 0)
			{
				list.Add(vector);
			}
			while (num3 + num6 > distance)
			{
				float num7 = distance - num3;
				float t = num7 / num6;
				Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
				list.Add(vector3);
				vector = vector3;
				num3 = 0f;
				num6 -= num7;
			}
			num3 += num6;
			if (num5 == this.Points.Length - 1 && num3 > distance * 0.5f)
			{
				list.Add(vector2);
			}
		}
		if (list.Count < 2)
		{
			return;
		}
		this.Points = list.ToArray();
		this.MinIndex = this.DefaultMinIndex;
		this.MaxIndex = this.DefaultMaxIndex;
		this.initialized = false;
	}

	// Token: 0x06003771 RID: 14193 RVA: 0x001490F8 File Offset: 0x001472F8
	public void Smoothen(int iterations, Func<int, float> filter = null)
	{
		this.Smoothen(iterations, Vector3.one, filter);
	}

	// Token: 0x06003772 RID: 14194 RVA: 0x00149108 File Offset: 0x00147308
	public void Smoothen(int iterations, Vector3 multipliers, Func<int, float> filter = null)
	{
		for (int i = 0; i < iterations; i++)
		{
			for (int j = this.MinIndex + (this.Circular ? 0 : 1); j <= this.MaxIndex - 1; j += 2)
			{
				this.SmoothenIndex(j, multipliers, filter);
			}
			for (int k = this.MinIndex + (this.Circular ? 1 : 2); k <= this.MaxIndex - 1; k += 2)
			{
				this.SmoothenIndex(k, multipliers, filter);
			}
		}
		this.initialized = false;
	}

	// Token: 0x06003773 RID: 14195 RVA: 0x00149184 File Offset: 0x00147384
	private void SmoothenIndex(int i, Vector3 multipliers, Func<int, float> filter = null)
	{
		int num = i - 1;
		int num2 = i + 1;
		if (i == 0)
		{
			num = this.Points.Length - 2;
		}
		Vector3 a = this.Points[num];
		Vector3 vector = this.Points[i];
		Vector3 b = this.Points[num2];
		Vector3 vector2 = (a + vector + vector + b) * 0.25f;
		if (filter != null)
		{
			multipliers *= filter(i);
		}
		if (multipliers != Vector3.one)
		{
			vector2.x = Mathf.LerpUnclamped(vector.x, vector2.x, multipliers.x);
			vector2.y = Mathf.LerpUnclamped(vector.y, vector2.y, multipliers.y);
			vector2.z = Mathf.LerpUnclamped(vector.z, vector2.z, multipliers.z);
		}
		this.Points[i] = vector2;
		if (i == 0)
		{
			this.Points[this.Points.Length - 1] = this.Points[0];
		}
	}

	// Token: 0x06003774 RID: 14196 RVA: 0x0014929D File Offset: 0x0014749D
	public Vector3 GetStartPoint()
	{
		return this.Points[this.MinIndex];
	}

	// Token: 0x06003775 RID: 14197 RVA: 0x001492B0 File Offset: 0x001474B0
	public Vector3 GetEndPoint()
	{
		return this.Points[this.MaxIndex];
	}

	// Token: 0x06003776 RID: 14198 RVA: 0x001492C3 File Offset: 0x001474C3
	public Vector3 GetStartTangent()
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return this.Tangents[this.MinIndex];
	}

	// Token: 0x06003777 RID: 14199 RVA: 0x001492E9 File Offset: 0x001474E9
	public Vector3 GetEndTangent()
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return this.Tangents[this.MaxIndex];
	}

	// Token: 0x06003778 RID: 14200 RVA: 0x00149310 File Offset: 0x00147510
	public Vector3 GetPoint(float distance)
	{
		if (this.Length == 0f)
		{
			return this.GetStartPoint();
		}
		float num = distance / this.Length * (float)(this.Points.Length - 1);
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartPoint();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndPoint();
		}
		Vector3 a = this.Points[num2];
		Vector3 b = this.Points[num2 + 1];
		float t = num - (float)num2;
		return Vector3.Lerp(a, b, t);
	}

	// Token: 0x06003779 RID: 14201 RVA: 0x00149394 File Offset: 0x00147594
	public virtual Vector3 GetTangent(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		if (this.Length == 0f)
		{
			return this.GetStartPoint();
		}
		float num = distance / this.Length * (float)(this.Tangents.Length - 1);
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartTangent();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndTangent();
		}
		Vector3 a = this.Tangents[num2];
		Vector3 b = this.Tangents[num2 + 1];
		float t = num - (float)num2;
		return Vector3.Slerp(a, b, t);
	}

	// Token: 0x0600377A RID: 14202 RVA: 0x0014942C File Offset: 0x0014762C
	public virtual Vector3 GetPointCubicHermite(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		if (this.Length == 0f)
		{
			return this.GetStartPoint();
		}
		float num = distance / this.Length * (float)(this.Points.Length - 1);
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartPoint();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndPoint();
		}
		Vector3 a = this.Points[num2];
		Vector3 a2 = this.Points[num2 + 1];
		Vector3 a3 = this.Tangents[num2] * this.StepSize;
		Vector3 a4 = this.Tangents[num2 + 1] * this.StepSize;
		float num3 = num - (float)num2;
		float num4 = num3 * num3;
		float num5 = num3 * num4;
		return (2f * num5 - 3f * num4 + 1f) * a + (num5 - 2f * num4 + num3) * a3 + (-2f * num5 + 3f * num4) * a2 + (num5 - num4) * a4;
	}

	// Token: 0x040031C3 RID: 12739
	public Vector3[] Points;

	// Token: 0x040031C4 RID: 12740
	public Vector3[] Tangents;

	// Token: 0x040031CA RID: 12746
	protected bool initialized;
}
