using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI
{
	// Token: 0x02000ACC RID: 2764
	public class SteamInventoryCrafting : MonoBehaviour
	{
		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x060042C5 RID: 17093 RVA: 0x001853CE File Offset: 0x001835CE
		// (set) Token: 0x060042C6 RID: 17094 RVA: 0x001853D6 File Offset: 0x001835D6
		public IPlayerItemDefinition ResultItem { get; private set; }

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x060042C7 RID: 17095 RVA: 0x001853DF File Offset: 0x001835DF
		// (set) Token: 0x060042C8 RID: 17096 RVA: 0x001853E7 File Offset: 0x001835E7
		public Coroutine MarketCoroutine { get; private set; }

		// Token: 0x04003B1B RID: 15131
		public GameObject Container;

		// Token: 0x04003B1C RID: 15132
		public Button ConvertToItem;

		// Token: 0x04003B1D RID: 15133
		public TextMeshProUGUI WoodAmount;

		// Token: 0x04003B1E RID: 15134
		public TextMeshProUGUI ClothAmount;

		// Token: 0x04003B1F RID: 15135
		public TextMeshProUGUI MetalAmount;

		// Token: 0x04003B20 RID: 15136
		public int SelectedCount;

		// Token: 0x04003B21 RID: 15137
		public TextMeshProUGUI InfoText;

		// Token: 0x04003B24 RID: 15140
		public SteamInventoryCrateOpen CraftModal;

		// Token: 0x04003B25 RID: 15141
		public GameObject CraftingContainer;

		// Token: 0x04003B26 RID: 15142
		public GameObject CraftingButton;

		// Token: 0x04003B27 RID: 15143
		public SteamInventoryNewItem NewItemModal;
	}
}
