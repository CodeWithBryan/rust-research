using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x0200059F RID: 1439
public class ItemBlueprint : MonoBehaviour
{
	// Token: 0x17000356 RID: 854
	// (get) Token: 0x06002B3E RID: 11070 RVA: 0x0010599C File Offset: 0x00103B9C
	public ItemDefinition targetItem
	{
		get
		{
			return base.GetComponent<ItemDefinition>();
		}
	}

	// Token: 0x17000357 RID: 855
	// (get) Token: 0x06002B3F RID: 11071 RVA: 0x001059A4 File Offset: 0x00103BA4
	public bool NeedsSteamDLC
	{
		get
		{
			return this.targetItem.steamDlc != null;
		}
	}

	// Token: 0x040022E6 RID: 8934
	public List<ItemAmount> ingredients = new List<ItemAmount>();

	// Token: 0x040022E7 RID: 8935
	public List<ItemDefinition> additionalUnlocks = new List<ItemDefinition>();

	// Token: 0x040022E8 RID: 8936
	public bool defaultBlueprint;

	// Token: 0x040022E9 RID: 8937
	public bool userCraftable = true;

	// Token: 0x040022EA RID: 8938
	public bool isResearchable = true;

	// Token: 0x040022EB RID: 8939
	public Rarity rarity;

	// Token: 0x040022EC RID: 8940
	[Header("Workbench")]
	public int workbenchLevelRequired;

	// Token: 0x040022ED RID: 8941
	[Header("Scrap")]
	public int scrapRequired;

	// Token: 0x040022EE RID: 8942
	public int scrapFromRecycle;

	// Token: 0x040022EF RID: 8943
	[Header("Unlocking")]
	[Tooltip("This item won't show anywhere unless you have the corresponding SteamItem in your inventory - which is defined on the ItemDefinition")]
	public bool NeedsSteamItem;

	// Token: 0x040022F0 RID: 8944
	public int blueprintStackSize = -1;

	// Token: 0x040022F1 RID: 8945
	public float time = 1f;

	// Token: 0x040022F2 RID: 8946
	public int amountToCreate = 1;

	// Token: 0x040022F3 RID: 8947
	public string UnlockAchievment;

	// Token: 0x040022F4 RID: 8948
	public string RecycleStat;
}
