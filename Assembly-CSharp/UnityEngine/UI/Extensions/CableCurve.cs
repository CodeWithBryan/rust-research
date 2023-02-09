using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009FA RID: 2554
	[Serializable]
	public class CableCurve
	{
		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06003CCB RID: 15563 RVA: 0x00163478 File Offset: 0x00161678
		// (set) Token: 0x06003CCC RID: 15564 RVA: 0x00163480 File Offset: 0x00161680
		public bool regenPoints
		{
			get
			{
				return this.m_regen;
			}
			set
			{
				this.m_regen = value;
			}
		}

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06003CCD RID: 15565 RVA: 0x00163489 File Offset: 0x00161689
		// (set) Token: 0x06003CCE RID: 15566 RVA: 0x00163491 File Offset: 0x00161691
		public Vector2 start
		{
			get
			{
				return this.m_start;
			}
			set
			{
				if (value != this.m_start)
				{
					this.m_regen = true;
				}
				this.m_start = value;
			}
		}

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06003CCF RID: 15567 RVA: 0x001634AF File Offset: 0x001616AF
		// (set) Token: 0x06003CD0 RID: 15568 RVA: 0x001634B7 File Offset: 0x001616B7
		public Vector2 end
		{
			get
			{
				return this.m_end;
			}
			set
			{
				if (value != this.m_end)
				{
					this.m_regen = true;
				}
				this.m_end = value;
			}
		}

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x06003CD1 RID: 15569 RVA: 0x001634D5 File Offset: 0x001616D5
		// (set) Token: 0x06003CD2 RID: 15570 RVA: 0x001634DD File Offset: 0x001616DD
		public float slack
		{
			get
			{
				return this.m_slack;
			}
			set
			{
				if (value != this.m_slack)
				{
					this.m_regen = true;
				}
				this.m_slack = Mathf.Max(0f, value);
			}
		}

		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x06003CD3 RID: 15571 RVA: 0x00163500 File Offset: 0x00161700
		// (set) Token: 0x06003CD4 RID: 15572 RVA: 0x00163508 File Offset: 0x00161708
		public int steps
		{
			get
			{
				return this.m_steps;
			}
			set
			{
				if (value != this.m_steps)
				{
					this.m_regen = true;
				}
				this.m_steps = Mathf.Max(2, value);
			}
		}

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06003CD5 RID: 15573 RVA: 0x00163528 File Offset: 0x00161728
		public Vector2 midPoint
		{
			get
			{
				Vector2 result = Vector2.zero;
				if (this.m_steps == 2)
				{
					return (this.points[0] + this.points[1]) * 0.5f;
				}
				if (this.m_steps > 2)
				{
					int num = this.m_steps / 2;
					if (this.m_steps % 2 == 0)
					{
						result = (this.points[num] + this.points[num + 1]) * 0.5f;
					}
					else
					{
						result = this.points[num];
					}
				}
				return result;
			}
		}

		// Token: 0x06003CD6 RID: 15574 RVA: 0x001635C4 File Offset: 0x001617C4
		public CableCurve()
		{
			this.points = CableCurve.emptyCurve;
			this.m_start = Vector2.up;
			this.m_end = Vector2.up + Vector2.right;
			this.m_slack = 0.5f;
			this.m_steps = 20;
			this.m_regen = true;
		}

		// Token: 0x06003CD7 RID: 15575 RVA: 0x0016361C File Offset: 0x0016181C
		public CableCurve(Vector2[] inputPoints)
		{
			this.points = inputPoints;
			this.m_start = inputPoints[0];
			this.m_end = inputPoints[1];
			this.m_slack = 0.5f;
			this.m_steps = 20;
			this.m_regen = true;
		}

		// Token: 0x06003CD8 RID: 15576 RVA: 0x0016366C File Offset: 0x0016186C
		public CableCurve(List<Vector2> inputPoints)
		{
			this.points = inputPoints.ToArray();
			this.m_start = inputPoints[0];
			this.m_end = inputPoints[1];
			this.m_slack = 0.5f;
			this.m_steps = 20;
			this.m_regen = true;
		}

		// Token: 0x06003CD9 RID: 15577 RVA: 0x001636C0 File Offset: 0x001618C0
		public CableCurve(CableCurve v)
		{
			this.points = v.Points();
			this.m_start = v.start;
			this.m_end = v.end;
			this.m_slack = v.slack;
			this.m_steps = v.steps;
			this.m_regen = v.regenPoints;
		}

		// Token: 0x06003CDA RID: 15578 RVA: 0x0016371C File Offset: 0x0016191C
		public Vector2[] Points()
		{
			if (!this.m_regen)
			{
				return this.points;
			}
			if (this.m_steps < 2)
			{
				return CableCurve.emptyCurve;
			}
			float num = Vector2.Distance(this.m_end, this.m_start);
			float num2 = Vector2.Distance(new Vector2(this.m_end.x, this.m_start.y), this.m_start);
			float num3 = num + Mathf.Max(0.0001f, this.m_slack);
			float num4 = 0f;
			float y = this.m_start.y;
			float num5 = num2;
			float y2 = this.end.y;
			if (num5 - num4 == 0f)
			{
				return CableCurve.emptyCurve;
			}
			float num6 = Mathf.Sqrt(Mathf.Pow(num3, 2f) - Mathf.Pow(y2 - y, 2f)) / (num5 - num4);
			int num7 = 30;
			int num8 = 0;
			int num9 = num7 * 10;
			bool flag = false;
			float num10 = 0f;
			float num11 = 100f;
			for (int i = 0; i < num7; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					num8++;
					float num12 = num10 + num11;
					float num13 = (float)Math.Sinh((double)num12) / num12;
					if (!float.IsInfinity(num13))
					{
						if (num13 == num6)
						{
							flag = true;
							num10 = num12;
							break;
						}
						if (num13 > num6)
						{
							break;
						}
						num10 = num12;
						if (num8 > num9)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
				num11 *= 0.1f;
			}
			float num14 = (num5 - num4) / 2f / num10;
			float num15 = (num4 + num5 - num14 * Mathf.Log((num3 + y2 - y) / (num3 - y2 + y))) / 2f;
			float num16 = (y2 + y - num3 * (float)Math.Cosh((double)num10) / (float)Math.Sinh((double)num10)) / 2f;
			this.points = new Vector2[this.m_steps];
			float num17 = (float)(this.m_steps - 1);
			for (int k = 0; k < this.m_steps; k++)
			{
				float num18 = (float)k / num17;
				Vector2 zero = Vector2.zero;
				zero.x = Mathf.Lerp(this.start.x, this.end.x, num18);
				zero.y = num14 * (float)Math.Cosh((double)((num18 * num2 - num15) / num14)) + num16;
				this.points[k] = zero;
			}
			this.m_regen = false;
			return this.points;
		}

		// Token: 0x040035F6 RID: 13814
		[SerializeField]
		private Vector2 m_start;

		// Token: 0x040035F7 RID: 13815
		[SerializeField]
		private Vector2 m_end;

		// Token: 0x040035F8 RID: 13816
		[SerializeField]
		private float m_slack;

		// Token: 0x040035F9 RID: 13817
		[SerializeField]
		private int m_steps;

		// Token: 0x040035FA RID: 13818
		[SerializeField]
		private bool m_regen;

		// Token: 0x040035FB RID: 13819
		private static Vector2[] emptyCurve = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 0f)
		};

		// Token: 0x040035FC RID: 13820
		[SerializeField]
		private Vector2[] points;
	}
}
