using System;
using Facepunch.Extend;
using TMPro;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000ACF RID: 2767
	public class SteamInventoryItem : MonoBehaviour
	{
		// Token: 0x060042CC RID: 17100 RVA: 0x001853F0 File Offset: 0x001835F0
		public bool Setup(IPlayerItem item)
		{
			this.Item = item;
			if (item.GetDefinition() == null)
			{
				return false;
			}
			base.transform.FindChildRecursive("ItemName").GetComponent<TextMeshProUGUI>().text = item.GetDefinition().Name;
			return this.Image.Load(item.GetDefinition().IconUrl);
		}

		// Token: 0x04003B31 RID: 15153
		public IPlayerItem Item;

		// Token: 0x04003B32 RID: 15154
		public HttpImage Image;
	}
}
