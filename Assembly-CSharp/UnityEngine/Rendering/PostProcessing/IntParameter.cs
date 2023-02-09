using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A3C RID: 2620
	[Serializable]
	public sealed class IntParameter : ParameterOverride<int>
	{
		// Token: 0x06003E06 RID: 15878 RVA: 0x0016CE9B File Offset: 0x0016B09B
		public override void Interp(int from, int to, float t)
		{
			this.value = (int)((float)from + (float)(to - from) * t);
		}
	}
}
