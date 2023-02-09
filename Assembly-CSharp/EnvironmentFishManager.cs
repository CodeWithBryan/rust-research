using System;
using System.Collections.Generic;

// Token: 0x020001ED RID: 493
public class EnvironmentFishManager : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04001250 RID: 4688
	public EnvironmentFishManager.FishTypeInstance[] fishTypes;

	// Token: 0x02000C13 RID: 3091
	[Serializable]
	public class FishTypeInstance
	{
		// Token: 0x04004084 RID: 16516
		public GameObjectRef prefab;

		// Token: 0x04004085 RID: 16517
		public bool shouldSchool;

		// Token: 0x04004086 RID: 16518
		public float populationScale;

		// Token: 0x04004087 RID: 16519
		public bool freshwater;

		// Token: 0x04004088 RID: 16520
		public bool seawater = true;

		// Token: 0x04004089 RID: 16521
		public float minDepth = 3f;

		// Token: 0x0400408A RID: 16522
		public float maxDepth = 100f;

		// Token: 0x0400408B RID: 16523
		public List<EnvironmentFish> activeFish;

		// Token: 0x0400408C RID: 16524
		public List<EnvironmentFish> sleeping;
	}
}
