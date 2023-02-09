using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000218 RID: 536
public class MusicChangeIntensity : MonoBehaviour
{
	// Token: 0x0400134E RID: 4942
	public float raiseTo;

	// Token: 0x0400134F RID: 4943
	public List<MusicChangeIntensity.DistanceIntensity> distanceIntensities = new List<MusicChangeIntensity.DistanceIntensity>();

	// Token: 0x04001350 RID: 4944
	public float tickInterval = 0.2f;

	// Token: 0x02000C24 RID: 3108
	[Serializable]
	public class DistanceIntensity
	{
		// Token: 0x040040EF RID: 16623
		public float distance = 60f;

		// Token: 0x040040F0 RID: 16624
		public float raiseTo;

		// Token: 0x040040F1 RID: 16625
		public bool forceStartMusicInSuppressedMusicZones;
	}
}
