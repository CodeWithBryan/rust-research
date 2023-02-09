using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A3B RID: 2619
	[Serializable]
	public sealed class FloatParameter : ParameterOverride<float>
	{
		// Token: 0x06003E04 RID: 15876 RVA: 0x0016CE84 File Offset: 0x0016B084
		public override void Interp(float from, float to, float t)
		{
			this.value = from + (to - from) * t;
		}
	}
}
