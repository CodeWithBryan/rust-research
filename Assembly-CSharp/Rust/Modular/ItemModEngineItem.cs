using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000AE9 RID: 2793
	public class ItemModEngineItem : ItemMod
	{
		// Token: 0x04003BBE RID: 15294
		public EngineStorage.EngineItemTypes engineItemType;

		// Token: 0x04003BBF RID: 15295
		[Range(1f, 3f)]
		public int tier = 1;
	}
}
