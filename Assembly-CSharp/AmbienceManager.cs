using System;
using System.Collections.Generic;

// Token: 0x0200020A RID: 522
public class AmbienceManager : SingletonComponent<AmbienceManager>, IClientComponent
{
	// Token: 0x040012E3 RID: 4835
	public List<AmbienceManager.EmitterTypeLimit> localEmitterLimits = new List<AmbienceManager.EmitterTypeLimit>();

	// Token: 0x040012E4 RID: 4836
	public AmbienceManager.EmitterTypeLimit catchallEmitterLimit = new AmbienceManager.EmitterTypeLimit();

	// Token: 0x040012E5 RID: 4837
	public int maxActiveLocalEmitters = 5;

	// Token: 0x040012E6 RID: 4838
	public int activeLocalEmitters;

	// Token: 0x040012E7 RID: 4839
	public List<AmbienceEmitter> cameraEmitters = new List<AmbienceEmitter>();

	// Token: 0x040012E8 RID: 4840
	public List<AmbienceEmitter> emittersInRange = new List<AmbienceEmitter>();

	// Token: 0x040012E9 RID: 4841
	public List<AmbienceEmitter> activeEmitters = new List<AmbienceEmitter>();

	// Token: 0x040012EA RID: 4842
	public float localEmitterRange = 30f;

	// Token: 0x040012EB RID: 4843
	public List<AmbienceZone> currentAmbienceZones = new List<AmbienceZone>();

	// Token: 0x040012EC RID: 4844
	public bool isUnderwater;

	// Token: 0x02000C1A RID: 3098
	[Serializable]
	public class EmitterTypeLimit
	{
		// Token: 0x040040B4 RID: 16564
		public List<AmbienceDefinitionList> ambience;

		// Token: 0x040040B5 RID: 16565
		public int limit = 1;

		// Token: 0x040040B6 RID: 16566
		public int active;
	}
}
