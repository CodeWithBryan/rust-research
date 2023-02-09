using System;
using UnityEngine;

// Token: 0x0200011D RID: 285
[CreateAssetMenu(menuName = "Rust/NPC Vending Order")]
public class NPCVendingOrder : ScriptableObject
{
	// Token: 0x04000E0B RID: 3595
	public NPCVendingOrder.Entry[] orders;

	// Token: 0x02000BD5 RID: 3029
	[Serializable]
	public class Entry
	{
		// Token: 0x04003FD3 RID: 16339
		public ItemDefinition sellItem;

		// Token: 0x04003FD4 RID: 16340
		public int sellItemAmount;

		// Token: 0x04003FD5 RID: 16341
		public bool sellItemAsBP;

		// Token: 0x04003FD6 RID: 16342
		public ItemDefinition currencyItem;

		// Token: 0x04003FD7 RID: 16343
		public int currencyAmount;

		// Token: 0x04003FD8 RID: 16344
		public bool currencyAsBP;

		// Token: 0x04003FD9 RID: 16345
		[Tooltip("The higher this number, the more likely this will be chosen")]
		public int weight;

		// Token: 0x04003FDA RID: 16346
		public int refillAmount = 1;

		// Token: 0x04003FDB RID: 16347
		public float refillDelay = 10f;
	}
}
