using System;
using UnityEngine;

// Token: 0x02000812 RID: 2066
public class LootAllButton : MonoBehaviour
{
	// Token: 0x04002DB0 RID: 11696
	public Func<Item, bool> Filter;

	// Token: 0x04002DB1 RID: 11697
	public OvenLootPanel inventoryGrid;
}
