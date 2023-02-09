using System;
using UnityEngine;

// Token: 0x02000736 RID: 1846
public class SteamInventoryItem : ScriptableObject
{
	// Token: 0x170003F6 RID: 1014
	// (get) Token: 0x060032FD RID: 13053 RVA: 0x0013ADC0 File Offset: 0x00138FC0
	public ItemDefinition itemDefinition
	{
		get
		{
			return ItemManager.FindItemDefinition(this.itemname);
		}
	}

	// Token: 0x060032FE RID: 13054 RVA: 0x0013ADCD File Offset: 0x00138FCD
	public virtual bool HasUnlocked(ulong playerId)
	{
		return this.DlcItem != null && this.DlcItem.HasLicense(playerId);
	}

	// Token: 0x04002974 RID: 10612
	public int id;

	// Token: 0x04002975 RID: 10613
	public Sprite icon;

	// Token: 0x04002976 RID: 10614
	public Translate.Phrase displayName;

	// Token: 0x04002977 RID: 10615
	public Translate.Phrase displayDescription;

	// Token: 0x04002978 RID: 10616
	[Header("Steam Inventory")]
	public SteamInventoryItem.Category category;

	// Token: 0x04002979 RID: 10617
	public SteamInventoryItem.SubCategory subcategory;

	// Token: 0x0400297A RID: 10618
	public SteamInventoryCategory steamCategory;

	// Token: 0x0400297B RID: 10619
	public bool isLimitedTimeOffer = true;

	// Token: 0x0400297C RID: 10620
	[Tooltip("Stop this item being broken down into cloth etc")]
	public bool PreventBreakingDown;

	// Token: 0x0400297D RID: 10621
	[Header("Meta")]
	public string itemname;

	// Token: 0x0400297E RID: 10622
	public ulong workshopID;

	// Token: 0x0400297F RID: 10623
	public SteamDLCItem DlcItem;

	// Token: 0x04002980 RID: 10624
	[Tooltip("Does nothing currently")]
	public bool forceCraftableItemDesc;

	// Token: 0x02000E14 RID: 3604
	public enum Category
	{
		// Token: 0x0400491E RID: 18718
		None,
		// Token: 0x0400491F RID: 18719
		Clothing,
		// Token: 0x04004920 RID: 18720
		Weapon,
		// Token: 0x04004921 RID: 18721
		Decoration,
		// Token: 0x04004922 RID: 18722
		Crate,
		// Token: 0x04004923 RID: 18723
		Resource
	}

	// Token: 0x02000E15 RID: 3605
	public enum SubCategory
	{
		// Token: 0x04004925 RID: 18725
		None,
		// Token: 0x04004926 RID: 18726
		Shirt,
		// Token: 0x04004927 RID: 18727
		Pants,
		// Token: 0x04004928 RID: 18728
		Jacket,
		// Token: 0x04004929 RID: 18729
		Hat,
		// Token: 0x0400492A RID: 18730
		Mask,
		// Token: 0x0400492B RID: 18731
		Footwear,
		// Token: 0x0400492C RID: 18732
		Weapon,
		// Token: 0x0400492D RID: 18733
		Misc,
		// Token: 0x0400492E RID: 18734
		Crate,
		// Token: 0x0400492F RID: 18735
		Resource,
		// Token: 0x04004930 RID: 18736
		CrateUncraftable
	}
}
