using System;
using UnityEngine;

namespace Sonar
{
	// Token: 0x020009D7 RID: 2519
	public class SubmarineSonar : FacepunchBehaviour
	{
		// Token: 0x04003544 RID: 13636
		[SerializeField]
		private float range = 100f;

		// Token: 0x04003545 RID: 13637
		[SerializeField]
		private ParticleSystem sonarPS;

		// Token: 0x04003546 RID: 13638
		[SerializeField]
		private ParticleSystem blipPS;

		// Token: 0x04003547 RID: 13639
		[SerializeField]
		private SonarObject us;

		// Token: 0x04003548 RID: 13640
		[SerializeField]
		private Color greenBlip;

		// Token: 0x04003549 RID: 13641
		[SerializeField]
		private Color redBlip;

		// Token: 0x0400354A RID: 13642
		[SerializeField]
		private Color whiteBlip;

		// Token: 0x0400354B RID: 13643
		[SerializeField]
		private SubmarineAudio submarineAudio;
	}
}
