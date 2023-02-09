using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005BB RID: 1467
public class ItemModContainer : ItemMod
{
	// Token: 0x06002BA5 RID: 11173 RVA: 0x0010696C File Offset: 0x00104B6C
	public override void OnItemCreated(Item item)
	{
		if (!item.isServer)
		{
			return;
		}
		if (this.capacity <= 0)
		{
			return;
		}
		if (item.contents != null)
		{
			if (this.validItemWhitelist != null && this.validItemWhitelist.Length != 0)
			{
				item.contents.canAcceptItem = new Func<Item, int, bool>(this.CanAcceptItem);
			}
			return;
		}
		item.contents = new ItemContainer();
		item.contents.flags = this.containerFlags;
		item.contents.allowedContents = ((this.onlyAllowedContents == (ItemContainer.ContentsType)0) ? ItemContainer.ContentsType.Generic : this.onlyAllowedContents);
		this.SetAllowedItems(item.contents);
		item.contents.availableSlots = this.availableSlots;
		if ((this.validItemWhitelist != null && this.validItemWhitelist.Length != 0) || this.ForceAcceptItemCheck)
		{
			item.contents.canAcceptItem = new Func<Item, int, bool>(this.CanAcceptItem);
		}
		item.contents.ServerInitialize(item, this.capacity);
		item.contents.maxStackSize = this.maxStackSize;
		item.contents.GiveUID();
	}

	// Token: 0x06002BA6 RID: 11174 RVA: 0x00106A72 File Offset: 0x00104C72
	protected virtual void SetAllowedItems(ItemContainer container)
	{
		container.SetOnlyAllowedItem(this.onlyAllowedItemType);
	}

	// Token: 0x17000367 RID: 871
	// (get) Token: 0x06002BA7 RID: 11175 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual bool ForceAcceptItemCheck
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002BA8 RID: 11176 RVA: 0x00106A80 File Offset: 0x00104C80
	protected virtual bool CanAcceptItem(Item item, int count)
	{
		ItemDefinition[] array = this.validItemWhitelist;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].itemid == item.info.itemid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002BA9 RID: 11177 RVA: 0x00106ABC File Offset: 0x00104CBC
	public override void OnVirginItem(Item item)
	{
		base.OnVirginItem(item);
		foreach (ItemAmount itemAmount in this.defaultContents)
		{
			Item item2 = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL);
			if (item2 != null)
			{
				item2.MoveToContainer(item.contents, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x06002BAA RID: 11178 RVA: 0x00106B3C File Offset: 0x00104D3C
	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		if (item.contents == null)
		{
			return;
		}
		for (int i = item.contents.itemList.Count - 1; i >= 0; i--)
		{
			Item item2 = item.contents.itemList[i];
			if (!item2.MoveToContainer(crafter.inventory.containerMain, -1, true, false, null, true))
			{
				item2.Drop(crafter.GetDropPosition(), crafter.GetDropVelocity(), default(Quaternion));
			}
		}
	}

	// Token: 0x0400236A RID: 9066
	public int capacity = 6;

	// Token: 0x0400236B RID: 9067
	public int maxStackSize;

	// Token: 0x0400236C RID: 9068
	[InspectorFlags]
	public ItemContainer.Flag containerFlags;

	// Token: 0x0400236D RID: 9069
	public ItemContainer.ContentsType onlyAllowedContents = ItemContainer.ContentsType.Generic;

	// Token: 0x0400236E RID: 9070
	public ItemDefinition onlyAllowedItemType;

	// Token: 0x0400236F RID: 9071
	public List<ItemSlot> availableSlots = new List<ItemSlot>();

	// Token: 0x04002370 RID: 9072
	public ItemDefinition[] validItemWhitelist = new ItemDefinition[0];

	// Token: 0x04002371 RID: 9073
	public bool openInDeployed = true;

	// Token: 0x04002372 RID: 9074
	public bool openInInventory = true;

	// Token: 0x04002373 RID: 9075
	public List<ItemAmount> defaultContents = new List<ItemAmount>();
}
