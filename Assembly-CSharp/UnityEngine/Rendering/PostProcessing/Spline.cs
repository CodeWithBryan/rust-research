using System;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5C RID: 2652
	[Serializable]
	public sealed class Spline
	{
		// Token: 0x06003EE7 RID: 16103 RVA: 0x00171278 File Offset: 0x0016F478
		public Spline(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds)
		{
			Assert.IsNotNull<AnimationCurve>(curve);
			this.curve = curve;
			this.m_ZeroValue = zeroValue;
			this.m_Loop = loop;
			this.m_Range = bounds.magnitude;
			this.cachedData = new float[128];
		}

		// Token: 0x06003EE8 RID: 16104 RVA: 0x001712CC File Offset: 0x0016F4CC
		public void Cache(int frame)
		{
			if (frame == this.frameCount)
			{
				return;
			}
			int length = this.curve.length;
			if (this.m_Loop && length > 1)
			{
				if (this.m_InternalLoopingCurve == null)
				{
					this.m_InternalLoopingCurve = new AnimationCurve();
				}
				Keyframe key = this.curve[length - 1];
				key.time -= this.m_Range;
				Keyframe key2 = this.curve[0];
				key2.time += this.m_Range;
				this.m_InternalLoopingCurve.keys = this.curve.keys;
				this.m_InternalLoopingCurve.AddKey(key);
				this.m_InternalLoopingCurve.AddKey(key2);
			}
			for (int i = 0; i < 128; i++)
			{
				this.cachedData[i] = this.Evaluate((float)i * 0.0078125f, length);
			}
			this.frameCount = Time.renderedFrameCount;
		}

		// Token: 0x06003EE9 RID: 16105 RVA: 0x001713BA File Offset: 0x0016F5BA
		public float Evaluate(float t, int length)
		{
			if (length == 0)
			{
				return this.m_ZeroValue;
			}
			if (!this.m_Loop || length == 1)
			{
				return this.curve.Evaluate(t);
			}
			return this.m_InternalLoopingCurve.Evaluate(t);
		}

		// Token: 0x06003EEA RID: 16106 RVA: 0x001713EB File Offset: 0x0016F5EB
		public float Evaluate(float t)
		{
			return this.Evaluate(t, this.curve.length);
		}

		// Token: 0x06003EEB RID: 16107 RVA: 0x001713FF File Offset: 0x0016F5FF
		public override int GetHashCode()
		{
			return 17 * 23 + this.curve.GetHashCode();
		}

		// Token: 0x04003830 RID: 14384
		public const int k_Precision = 128;

		// Token: 0x04003831 RID: 14385
		public const float k_Step = 0.0078125f;

		// Token: 0x04003832 RID: 14386
		public AnimationCurve curve;

		// Token: 0x04003833 RID: 14387
		[SerializeField]
		private bool m_Loop;

		// Token: 0x04003834 RID: 14388
		[SerializeField]
		private float m_ZeroValue;

		// Token: 0x04003835 RID: 14389
		[SerializeField]
		private float m_Range;

		// Token: 0x04003836 RID: 14390
		private AnimationCurve m_InternalLoopingCurve;

		// Token: 0x04003837 RID: 14391
		private int frameCount = -1;

		// Token: 0x04003838 RID: 14392
		public float[] cachedData;
	}
}
