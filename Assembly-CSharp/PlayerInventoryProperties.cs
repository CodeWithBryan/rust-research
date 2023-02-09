using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200072B RID: 1835
[CreateAssetMenu(menuName = "Rust/Player Inventory Properties")]
public class PlayerInventoryProperties : ScriptableObject
{
	// Token: 0x060032D8 RID: 13016 RVA: 0x0013A3F4 File Offset: 0x001385F4
	public void GiveToPlayer(BasePlayer player)
	{
		PlayerInventoryProperties.<>c__DisplayClass7_0 CS$<>8__locals1;
		CS$<>8__locals1.player = player;
		if (CS$<>8__locals1.player == null)
		{
			return;
		}
		CS$<>8__locals1.player.inventory.Strip();
		if (this.giveBase != null)
		{
			this.giveBase.GiveToPlayer(CS$<>8__locals1.player);
		}
		foreach (PlayerInventoryProperties.ItemAmountSkinned toCreate in this.belt)
		{
			PlayerInventoryProperties.<GiveToPlayer>g__CreateItem|7_0(toCreate, CS$<>8__locals1.player.inventory.containerBelt, ref CS$<>8__locals1);
		}
		foreach (PlayerInventoryProperties.ItemAmountSkinned toCreate2 in this.main)
		{
			PlayerInventoryProperties.<GiveToPlayer>g__CreateItem|7_0(toCreate2, CS$<>8__locals1.player.inventory.containerMain, ref CS$<>8__locals1);
		}
		foreach (PlayerInventoryProperties.ItemAmountSkinned toCreate3 in this.wear)
		{
			PlayerInventoryProperties.<GiveToPlayer>g__CreateItem|7_0(toCreate3, CS$<>8__locals1.player.inventory.containerWear, ref CS$<>8__locals1);
		}
	}

	// Token: 0x060032DA RID: 13018 RVA: 0x0013A550 File Offset: 0x00138750
	[CompilerGenerated]
	internal static void <GiveToPlayer>g__CreateItem|7_0(PlayerInventoryProperties.ItemAmountSkinned toCreate, ItemContainer destination, ref PlayerInventoryProperties.<>c__DisplayClass7_0 A_2)
	{
		Item item;
		if (toCreate.blueprint)
		{
			item = ItemManager.Create(ItemManager.blueprintBaseDef, 1, 0UL);
			item.blueprintTarget = ((toCreate.itemDef.isRedirectOf != null) ? toCreate.itemDef.isRedirectOf.itemid : toCreate.itemDef.itemid);
		}
		else
		{
			item = ItemManager.Create(toCreate.itemDef, (int)toCreate.amount, toCreate.skinOverride);
		}
		A_2.player.inventory.GiveItem(item, destination);
	}

	// Token: 0x04002931 RID: 10545
	public string niceName;

	// Token: 0x04002932 RID: 10546
	public int order = 100;

	// Token: 0x04002933 RID: 10547
	public List<PlayerInventoryProperties.ItemAmountSkinned> belt;

	// Token: 0x04002934 RID: 10548
	public List<PlayerInventoryProperties.ItemAmountSkinned> main;

	// Token: 0x04002935 RID: 10549
	public List<PlayerInventoryProperties.ItemAmountSkinned> wear;

	// Token: 0x04002936 RID: 10550
	public PlayerInventoryProperties giveBase;

	// Token: 0x02000E0E RID: 3598
	[Serializable]
	public class ItemAmountSkinned : ItemAmount
	{
		// Token: 0x06004FD7 RID: 20439 RVA: 0x001A0741 File Offset: 0x0019E941
		public ItemAmountSkinned() : base(null, 0f)
		{
		}

		// Token: 0x040048DD RID: 18653
		public ulong skinOverride;

		// Token: 0x040048DE RID: 18654
		public bool blueprint;
	}
}
