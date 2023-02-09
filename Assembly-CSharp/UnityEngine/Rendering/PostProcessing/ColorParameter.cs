using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A3E RID: 2622
	[Serializable]
	public sealed class ColorParameter : ParameterOverride<Color>
	{
		// Token: 0x06003E09 RID: 15881 RVA: 0x0016CEB8 File Offset: 0x0016B0B8
		public override void Interp(Color from, Color to, float t)
		{
			this.value.r = from.r + (to.r - from.r) * t;
			this.value.g = from.g + (to.g - from.g) * t;
			this.value.b = from.b + (to.b - from.b) * t;
			this.value.a = from.a + (to.a - from.a) * t;
		}

		// Token: 0x06003E0A RID: 15882 RVA: 0x0016CF49 File Offset: 0x0016B149
		public static implicit operator Vector4(ColorParameter prop)
		{
			return prop.value;
		}
	}
}
