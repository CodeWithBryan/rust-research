using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E7 RID: 2023
public class CraftingQueue : SingletonComponent<CraftingQueue>
{
	// Token: 0x04002CD8 RID: 11480
	public GameObject queueContainer;

	// Token: 0x04002CD9 RID: 11481
	public GameObject queueItemPrefab;

	// Token: 0x04002CDA RID: 11482
	private ScrollRect scrollRect;
}
