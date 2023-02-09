using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A55 RID: 2645
	public static class HaltonSeq
	{
		// Token: 0x06003E98 RID: 16024 RVA: 0x0016F780 File Offset: 0x0016D980
		public static float Get(int index, int radix)
		{
			float num = 0f;
			float num2 = 1f / (float)radix;
			while (index > 0)
			{
				num += (float)(index % radix) * num2;
				index /= radix;
				num2 /= (float)radix;
			}
			return num;
		}
	}
}
