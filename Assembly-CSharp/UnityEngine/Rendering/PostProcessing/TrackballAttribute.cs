using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A02 RID: 2562
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class TrackballAttribute : Attribute
	{
		// Token: 0x06003D3F RID: 15679 RVA: 0x00165C08 File Offset: 0x00163E08
		public TrackballAttribute(TrackballAttribute.Mode mode)
		{
			this.mode = mode;
		}

		// Token: 0x0400364B RID: 13899
		public readonly TrackballAttribute.Mode mode;

		// Token: 0x02000EBD RID: 3773
		public enum Mode
		{
			// Token: 0x04004BC3 RID: 19395
			None,
			// Token: 0x04004BC4 RID: 19396
			Lift,
			// Token: 0x04004BC5 RID: 19397
			Gamma,
			// Token: 0x04004BC6 RID: 19398
			Gain
		}
	}
}
