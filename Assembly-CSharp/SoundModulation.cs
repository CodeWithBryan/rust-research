using System;
using UnityEngine;

// Token: 0x0200022D RID: 557
public class SoundModulation : MonoBehaviour, IClientComponent
{
	// Token: 0x040013E8 RID: 5096
	private const int parameterCount = 4;

	// Token: 0x02000C2D RID: 3117
	public enum Parameter
	{
		// Token: 0x0400411E RID: 16670
		Gain,
		// Token: 0x0400411F RID: 16671
		Pitch,
		// Token: 0x04004120 RID: 16672
		Spread,
		// Token: 0x04004121 RID: 16673
		MaxDistance
	}

	// Token: 0x02000C2E RID: 3118
	[Serializable]
	public class Modulator
	{
		// Token: 0x04004122 RID: 16674
		public SoundModulation.Parameter param;

		// Token: 0x04004123 RID: 16675
		public float value = 1f;
	}
}
