using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x0200098C RID: 2444
	[Serializable]
	public class PredicationPreset
	{
		// Token: 0x04003481 RID: 13441
		[Min(0.0001f)]
		public float Threshold = 0.01f;

		// Token: 0x04003482 RID: 13442
		[Range(1f, 5f)]
		public float Scale = 2f;

		// Token: 0x04003483 RID: 13443
		[Range(0f, 1f)]
		public float Strength = 0.4f;
	}
}
