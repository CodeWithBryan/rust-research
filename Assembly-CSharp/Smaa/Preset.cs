using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x0200098D RID: 2445
	[Serializable]
	public class Preset
	{
		// Token: 0x04003484 RID: 13444
		public bool DiagDetection = true;

		// Token: 0x04003485 RID: 13445
		public bool CornerDetection = true;

		// Token: 0x04003486 RID: 13446
		[Range(0f, 0.5f)]
		public float Threshold = 0.1f;

		// Token: 0x04003487 RID: 13447
		[Min(0.0001f)]
		public float DepthThreshold = 0.01f;

		// Token: 0x04003488 RID: 13448
		[Range(0f, 112f)]
		public int MaxSearchSteps = 16;

		// Token: 0x04003489 RID: 13449
		[Range(0f, 20f)]
		public int MaxSearchStepsDiag = 8;

		// Token: 0x0400348A RID: 13450
		[Range(0f, 100f)]
		public int CornerRounding = 25;

		// Token: 0x0400348B RID: 13451
		[Min(0f)]
		public float LocalContrastAdaptationFactor = 2f;
	}
}
