using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A54 RID: 2644
	public class HableCurve
	{
		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06003E89 RID: 16009 RVA: 0x0016F21A File Offset: 0x0016D41A
		// (set) Token: 0x06003E8A RID: 16010 RVA: 0x0016F222 File Offset: 0x0016D422
		public float whitePoint { get; private set; }

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x06003E8B RID: 16011 RVA: 0x0016F22B File Offset: 0x0016D42B
		// (set) Token: 0x06003E8C RID: 16012 RVA: 0x0016F233 File Offset: 0x0016D433
		public float inverseWhitePoint { get; private set; }

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x06003E8D RID: 16013 RVA: 0x0016F23C File Offset: 0x0016D43C
		// (set) Token: 0x06003E8E RID: 16014 RVA: 0x0016F244 File Offset: 0x0016D444
		internal float x0 { get; private set; }

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x06003E8F RID: 16015 RVA: 0x0016F24D File Offset: 0x0016D44D
		// (set) Token: 0x06003E90 RID: 16016 RVA: 0x0016F255 File Offset: 0x0016D455
		internal float x1 { get; private set; }

		// Token: 0x06003E91 RID: 16017 RVA: 0x0016F260 File Offset: 0x0016D460
		public HableCurve()
		{
			for (int i = 0; i < 3; i++)
			{
				this.m_Segments[i] = new HableCurve.Segment();
			}
			this.uniforms = new HableCurve.Uniforms(this);
		}

		// Token: 0x06003E92 RID: 16018 RVA: 0x0016F2A4 File Offset: 0x0016D4A4
		public float Eval(float x)
		{
			float num = x * this.inverseWhitePoint;
			int num2 = (num < this.x0) ? 0 : ((num < this.x1) ? 1 : 2);
			return this.m_Segments[num2].Eval(num);
		}

		// Token: 0x06003E93 RID: 16019 RVA: 0x0016F2E4 File Offset: 0x0016D4E4
		public void Init(float toeStrength, float toeLength, float shoulderStrength, float shoulderLength, float shoulderAngle, float gamma)
		{
			HableCurve.DirectParams directParams = default(HableCurve.DirectParams);
			toeLength = Mathf.Pow(Mathf.Clamp01(toeLength), 2.2f);
			toeStrength = Mathf.Clamp01(toeStrength);
			shoulderAngle = Mathf.Clamp01(shoulderAngle);
			shoulderStrength = Mathf.Clamp(shoulderStrength, 1E-05f, 0.99999f);
			shoulderLength = Mathf.Max(0f, shoulderLength);
			gamma = Mathf.Max(1E-05f, gamma);
			float num = toeLength * 0.5f;
			float num2 = (1f - toeStrength) * num;
			float num3 = 1f - num2;
			float num4 = num + num3;
			float num5 = (1f - shoulderStrength) * num3;
			float x = num + num5;
			float y = num2 + num5;
			float num6 = RuntimeUtilities.Exp2(shoulderLength) - 1f;
			float w = num4 + num6;
			directParams.x0 = num;
			directParams.y0 = num2;
			directParams.x1 = x;
			directParams.y1 = y;
			directParams.W = w;
			directParams.gamma = gamma;
			directParams.overshootX = directParams.W * 2f * shoulderAngle * shoulderLength;
			directParams.overshootY = 0.5f * shoulderAngle * shoulderLength;
			this.InitSegments(directParams);
		}

		// Token: 0x06003E94 RID: 16020 RVA: 0x0016F3F8 File Offset: 0x0016D5F8
		private void InitSegments(HableCurve.DirectParams srcParams)
		{
			HableCurve.DirectParams directParams = srcParams;
			this.whitePoint = srcParams.W;
			this.inverseWhitePoint = 1f / srcParams.W;
			directParams.W = 1f;
			directParams.x0 /= srcParams.W;
			directParams.x1 /= srcParams.W;
			directParams.overshootX = srcParams.overshootX / srcParams.W;
			float num;
			float num2;
			this.AsSlopeIntercept(out num, out num2, directParams.x0, directParams.x1, directParams.y0, directParams.y1);
			float gamma = srcParams.gamma;
			HableCurve.Segment segment = this.m_Segments[1];
			segment.offsetX = -(num2 / num);
			segment.offsetY = 0f;
			segment.scaleX = 1f;
			segment.scaleY = 1f;
			segment.lnA = gamma * Mathf.Log(num);
			segment.B = gamma;
			float m = this.EvalDerivativeLinearGamma(num, num2, gamma, directParams.x0);
			float m2 = this.EvalDerivativeLinearGamma(num, num2, gamma, directParams.x1);
			directParams.y0 = Mathf.Max(1E-05f, Mathf.Pow(directParams.y0, directParams.gamma));
			directParams.y1 = Mathf.Max(1E-05f, Mathf.Pow(directParams.y1, directParams.gamma));
			directParams.overshootY = Mathf.Pow(1f + directParams.overshootY, directParams.gamma) - 1f;
			this.x0 = directParams.x0;
			this.x1 = directParams.x1;
			HableCurve.Segment segment2 = this.m_Segments[0];
			segment2.offsetX = 0f;
			segment2.offsetY = 0f;
			segment2.scaleX = 1f;
			segment2.scaleY = 1f;
			float lnA;
			float b;
			this.SolveAB(out lnA, out b, directParams.x0, directParams.y0, m);
			segment2.lnA = lnA;
			segment2.B = b;
			HableCurve.Segment segment3 = this.m_Segments[2];
			float x = 1f + directParams.overshootX - directParams.x1;
			float y = 1f + directParams.overshootY - directParams.y1;
			float lnA2;
			float b2;
			this.SolveAB(out lnA2, out b2, x, y, m2);
			segment3.offsetX = 1f + directParams.overshootX;
			segment3.offsetY = 1f + directParams.overshootY;
			segment3.scaleX = -1f;
			segment3.scaleY = -1f;
			segment3.lnA = lnA2;
			segment3.B = b2;
			float num3 = this.m_Segments[2].Eval(1f);
			float num4 = 1f / num3;
			this.m_Segments[0].offsetY *= num4;
			this.m_Segments[0].scaleY *= num4;
			this.m_Segments[1].offsetY *= num4;
			this.m_Segments[1].scaleY *= num4;
			this.m_Segments[2].offsetY *= num4;
			this.m_Segments[2].scaleY *= num4;
		}

		// Token: 0x06003E95 RID: 16021 RVA: 0x0016F711 File Offset: 0x0016D911
		private void SolveAB(out float lnA, out float B, float x0, float y0, float m)
		{
			B = m * x0 / y0;
			lnA = Mathf.Log(y0) - B * Mathf.Log(x0);
		}

		// Token: 0x06003E96 RID: 16022 RVA: 0x0016F730 File Offset: 0x0016D930
		private void AsSlopeIntercept(out float m, out float b, float x0, float x1, float y0, float y1)
		{
			float num = y1 - y0;
			float num2 = x1 - x0;
			if (num2 == 0f)
			{
				m = 1f;
			}
			else
			{
				m = num / num2;
			}
			b = y0 - x0 * m;
		}

		// Token: 0x06003E97 RID: 16023 RVA: 0x0016F767 File Offset: 0x0016D967
		private float EvalDerivativeLinearGamma(float m, float b, float g, float x)
		{
			return g * m * Mathf.Pow(m * x + b, g - 1f);
		}

		// Token: 0x04003793 RID: 14227
		private readonly HableCurve.Segment[] m_Segments = new HableCurve.Segment[3];

		// Token: 0x04003794 RID: 14228
		public readonly HableCurve.Uniforms uniforms;

		// Token: 0x02000ED3 RID: 3795
		private class Segment
		{
			// Token: 0x06005144 RID: 20804 RVA: 0x001A4308 File Offset: 0x001A2508
			public float Eval(float x)
			{
				float num = (x - this.offsetX) * this.scaleX;
				float num2 = 0f;
				if (num > 0f)
				{
					num2 = Mathf.Exp(this.lnA + this.B * Mathf.Log(num));
				}
				return num2 * this.scaleY + this.offsetY;
			}

			// Token: 0x04004C49 RID: 19529
			public float offsetX;

			// Token: 0x04004C4A RID: 19530
			public float offsetY;

			// Token: 0x04004C4B RID: 19531
			public float scaleX;

			// Token: 0x04004C4C RID: 19532
			public float scaleY;

			// Token: 0x04004C4D RID: 19533
			public float lnA;

			// Token: 0x04004C4E RID: 19534
			public float B;
		}

		// Token: 0x02000ED4 RID: 3796
		private struct DirectParams
		{
			// Token: 0x04004C4F RID: 19535
			internal float x0;

			// Token: 0x04004C50 RID: 19536
			internal float y0;

			// Token: 0x04004C51 RID: 19537
			internal float x1;

			// Token: 0x04004C52 RID: 19538
			internal float y1;

			// Token: 0x04004C53 RID: 19539
			internal float W;

			// Token: 0x04004C54 RID: 19540
			internal float overshootX;

			// Token: 0x04004C55 RID: 19541
			internal float overshootY;

			// Token: 0x04004C56 RID: 19542
			internal float gamma;
		}

		// Token: 0x02000ED5 RID: 3797
		public class Uniforms
		{
			// Token: 0x06005146 RID: 20806 RVA: 0x001A435C File Offset: 0x001A255C
			internal Uniforms(HableCurve parent)
			{
				this.parent = parent;
			}

			// Token: 0x170006AB RID: 1707
			// (get) Token: 0x06005147 RID: 20807 RVA: 0x001A436B File Offset: 0x001A256B
			public Vector4 curve
			{
				get
				{
					return new Vector4(this.parent.inverseWhitePoint, this.parent.x0, this.parent.x1, 0f);
				}
			}

			// Token: 0x170006AC RID: 1708
			// (get) Token: 0x06005148 RID: 20808 RVA: 0x001A4398 File Offset: 0x001A2598
			public Vector4 toeSegmentA
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[0];
					return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
				}
			}

			// Token: 0x170006AD RID: 1709
			// (get) Token: 0x06005149 RID: 20809 RVA: 0x001A43D0 File Offset: 0x001A25D0
			public Vector4 toeSegmentB
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[0];
					return new Vector4(segment.lnA, segment.B, 0f, 0f);
				}
			}

			// Token: 0x170006AE RID: 1710
			// (get) Token: 0x0600514A RID: 20810 RVA: 0x001A4408 File Offset: 0x001A2608
			public Vector4 midSegmentA
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[1];
					return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
				}
			}

			// Token: 0x170006AF RID: 1711
			// (get) Token: 0x0600514B RID: 20811 RVA: 0x001A4440 File Offset: 0x001A2640
			public Vector4 midSegmentB
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[1];
					return new Vector4(segment.lnA, segment.B, 0f, 0f);
				}
			}

			// Token: 0x170006B0 RID: 1712
			// (get) Token: 0x0600514C RID: 20812 RVA: 0x001A4478 File Offset: 0x001A2678
			public Vector4 shoSegmentA
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[2];
					return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
				}
			}

			// Token: 0x170006B1 RID: 1713
			// (get) Token: 0x0600514D RID: 20813 RVA: 0x001A44B0 File Offset: 0x001A26B0
			public Vector4 shoSegmentB
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[2];
					return new Vector4(segment.lnA, segment.B, 0f, 0f);
				}
			}

			// Token: 0x04004C57 RID: 19543
			private HableCurve parent;
		}
	}
}
