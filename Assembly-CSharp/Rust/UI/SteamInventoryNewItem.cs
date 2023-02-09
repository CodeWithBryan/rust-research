using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000AD1 RID: 2769
	public class SteamInventoryNewItem : MonoBehaviour
	{
		// Token: 0x060042CF RID: 17103 RVA: 0x00185454 File Offset: 0x00183654
		public async Task Open(IPlayerItem item)
		{
			base.gameObject.SetActive(true);
			base.GetComponentInChildren<SteamInventoryItem>().Setup(item);
			while (this && base.gameObject.activeSelf)
			{
				await Task.Delay(100);
			}
		}
	}
}
