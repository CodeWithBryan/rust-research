using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000442 RID: 1090
[CreateAssetMenu(menuName = "Rust/TrappableWildlife")]
[Serializable]
public class TrappableWildlife : ScriptableObject
{
	// Token: 0x04001C62 RID: 7266
	public GameObjectRef worldObject;

	// Token: 0x04001C63 RID: 7267
	public ItemDefinition inventoryObject;

	// Token: 0x04001C64 RID: 7268
	public int minToCatch;

	// Token: 0x04001C65 RID: 7269
	public int maxToCatch;

	// Token: 0x04001C66 RID: 7270
	public List<TrappableWildlife.BaitType> baitTypes;

	// Token: 0x04001C67 RID: 7271
	public int caloriesForInterest = 20;

	// Token: 0x04001C68 RID: 7272
	public float successRate = 1f;

	// Token: 0x04001C69 RID: 7273
	public float xpScale = 1f;

	// Token: 0x02000C9D RID: 3229
	[Serializable]
	public class BaitType
	{
		// Token: 0x04004339 RID: 17209
		public float successRate = 1f;

		// Token: 0x0400433A RID: 17210
		public ItemDefinition bait;

		// Token: 0x0400433B RID: 17211
		public int minForInterest = 1;

		// Token: 0x0400433C RID: 17212
		public int maxToConsume = 1;
	}
}
