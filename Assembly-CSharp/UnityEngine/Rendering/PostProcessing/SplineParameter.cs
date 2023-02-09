using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A42 RID: 2626
	[Serializable]
	public sealed class SplineParameter : ParameterOverride<Spline>
	{
		// Token: 0x06003E18 RID: 15896 RVA: 0x0016D11B File Offset: 0x0016B31B
		protected internal override void OnEnable()
		{
			if (this.value != null)
			{
				this.value.Cache(int.MinValue);
			}
		}

		// Token: 0x06003E19 RID: 15897 RVA: 0x0016D135 File Offset: 0x0016B335
		internal override void SetValue(ParameterOverride parameter)
		{
			base.SetValue(parameter);
			if (this.value != null)
			{
				this.value.Cache(Time.renderedFrameCount);
			}
		}

		// Token: 0x06003E1A RID: 15898 RVA: 0x0016D158 File Offset: 0x0016B358
		public override void Interp(Spline from, Spline to, float t)
		{
			if (from == null || to == null)
			{
				base.Interp(from, to, t);
				return;
			}
			int renderedFrameCount = Time.renderedFrameCount;
			from.Cache(renderedFrameCount);
			to.Cache(renderedFrameCount);
			for (int i = 0; i < 128; i++)
			{
				float num = from.cachedData[i];
				float num2 = to.cachedData[i];
				this.value.cachedData[i] = num + (num2 - num) * t;
			}
		}
	}
}
