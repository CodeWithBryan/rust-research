using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000AE7 RID: 2791
	[CreateAssetMenu(fileName = "Vehicle Module Engine Items", menuName = "Rust/Vehicles/Module Engine Items")]
	public class VehicleModuleEngineItems : ScriptableObject
	{
		// Token: 0x06004324 RID: 17188 RVA: 0x00186388 File Offset: 0x00184588
		public bool TryGetItem(int tier, EngineStorage.EngineItemTypes type, out ItemModEngineItem output)
		{
			List<ItemModEngineItem> list = Pool.GetList<ItemModEngineItem>();
			bool result = false;
			output = null;
			foreach (ItemModEngineItem itemModEngineItem in this.engineItems)
			{
				if (itemModEngineItem.tier == tier && itemModEngineItem.engineItemType == type)
				{
					list.Add(itemModEngineItem);
				}
			}
			if (list.Count > 0)
			{
				output = list.GetRandom<ItemModEngineItem>();
				result = true;
			}
			Pool.FreeList<ItemModEngineItem>(ref list);
			return result;
		}

		// Token: 0x04003BB6 RID: 15286
		[SerializeField]
		private ItemModEngineItem[] engineItems;
	}
}
