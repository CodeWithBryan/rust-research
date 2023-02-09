using System;
using UnityEngine;

// Token: 0x0200020E RID: 526
[CreateAssetMenu(menuName = "Rust/Blended Engine Loop Definition")]
public class BlendedEngineLoopDefinition : ScriptableObject
{
	// Token: 0x040012F8 RID: 4856
	public BlendedEngineLoopDefinition.EngineLoopDefinition[] engineLoops;

	// Token: 0x040012F9 RID: 4857
	public float minRPM;

	// Token: 0x040012FA RID: 4858
	public float maxRPM;

	// Token: 0x040012FB RID: 4859
	public float RPMChangeRateUp = 0.5f;

	// Token: 0x040012FC RID: 4860
	public float RPMChangeRateDown = 0.2f;

	// Token: 0x02000C1C RID: 3100
	[Serializable]
	public class EngineLoopDefinition
	{
		// Token: 0x06004C2B RID: 19499 RVA: 0x00194DF6 File Offset: 0x00192FF6
		public float GetPitchForRPM(float targetRPM)
		{
			return targetRPM / this.RPM;
		}

		// Token: 0x040040B9 RID: 16569
		public SoundDefinition soundDefinition;

		// Token: 0x040040BA RID: 16570
		public float RPM;

		// Token: 0x040040BB RID: 16571
		public float startRPM;

		// Token: 0x040040BC RID: 16572
		public float startFullRPM;

		// Token: 0x040040BD RID: 16573
		public float stopFullRPM;

		// Token: 0x040040BE RID: 16574
		public float stopRPM;
	}
}
