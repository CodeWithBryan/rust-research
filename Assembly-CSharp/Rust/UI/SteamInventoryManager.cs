using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000AD0 RID: 2768
	public class SteamInventoryManager : SingletonComponent<SteamInventoryManager>
	{
		// Token: 0x04003B33 RID: 15155
		public GameObject inventoryItemPrefab;

		// Token: 0x04003B34 RID: 15156
		public GameObject inventoryCanvas;

		// Token: 0x04003B35 RID: 15157
		public GameObject missingItems;

		// Token: 0x04003B36 RID: 15158
		public SteamInventoryCrafting CraftControl;

		// Token: 0x04003B37 RID: 15159
		public List<GameObject> items;

		// Token: 0x04003B38 RID: 15160
		public GameObject LoadingOverlay;
	}
}
