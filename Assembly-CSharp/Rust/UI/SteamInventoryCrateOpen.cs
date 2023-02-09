using System;
using TMPro;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000ACD RID: 2765
	public class SteamInventoryCrateOpen : MonoBehaviour
	{
		// Token: 0x04003B28 RID: 15144
		public TextMeshProUGUI Name;

		// Token: 0x04003B29 RID: 15145
		public TextMeshProUGUI Requirements;

		// Token: 0x04003B2A RID: 15146
		public TextMeshProUGUI Label;

		// Token: 0x04003B2B RID: 15147
		public HttpImage IconImage;

		// Token: 0x04003B2C RID: 15148
		public GameObject ErrorPanel;

		// Token: 0x04003B2D RID: 15149
		public TextMeshProUGUI ErrorText;

		// Token: 0x04003B2E RID: 15150
		public GameObject CraftButton;

		// Token: 0x04003B2F RID: 15151
		public GameObject ProgressPanel;

		// Token: 0x04003B30 RID: 15152
		public SteamInventoryNewItem NewItemModal;
	}
}
