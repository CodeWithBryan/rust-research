using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000098 RID: 152
public class MixingTable : StorageContainer
{
	// Token: 0x06000DA5 RID: 3493 RVA: 0x00072BF8 File Offset: 0x00070DF8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MixingTable.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4167839872U, "SVSwitch", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SVSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000DA6 RID: 3494 RVA: 0x00072D60 File Offset: 0x00070F60
	// (set) Token: 0x06000DA7 RID: 3495 RVA: 0x00072D68 File Offset: 0x00070F68
	public float RemainingMixTime { get; private set; }

	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000DA8 RID: 3496 RVA: 0x00072D71 File Offset: 0x00070F71
	// (set) Token: 0x06000DA9 RID: 3497 RVA: 0x00072D79 File Offset: 0x00070F79
	public float TotalMixTime { get; private set; }

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000DAA RID: 3498 RVA: 0x00072D82 File Offset: 0x00070F82
	// (set) Token: 0x06000DAB RID: 3499 RVA: 0x00072D8A File Offset: 0x00070F8A
	public global::BasePlayer MixStartingPlayer { get; private set; }

	// Token: 0x06000DAC RID: 3500 RVA: 0x00072D94 File Offset: 0x00070F94
	public override void ServerInit()
	{
		base.ServerInit();
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
		base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		RecipeDictionary.CacheRecipes(this.Recipes);
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x00072DF4 File Offset: 0x00070FF4
	private bool CanAcceptItem(global::Item item, int targetSlot)
	{
		if (item == null)
		{
			return false;
		}
		if (!this.OnlyAcceptValidIngredients)
		{
			return true;
		}
		if (this.GetItemWaterAmount(item) > 0)
		{
			item = item.contents.itemList[0];
		}
		return item.info == this.currentProductionItem || RecipeDictionary.ValidIngredientForARecipe(item, this.Recipes);
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x00072E4E File Offset: 0x0007104E
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		if (base.IsOn())
		{
			this.StopMixing();
		}
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x00072E64 File Offset: 0x00071064
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void SVSwitch(global::BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (flag == base.IsOn())
		{
			return;
		}
		if (msg.player == null)
		{
			return;
		}
		if (flag)
		{
			this.StartMixing(msg.player);
			return;
		}
		this.StopMixing();
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x00072EAC File Offset: 0x000710AC
	private void StartMixing(global::BasePlayer player)
	{
		if (base.IsOn())
		{
			return;
		}
		if (!this.CanStartMixing(player))
		{
			return;
		}
		this.MixStartingPlayer = player;
		bool flag;
		List<global::Item> orderedContainerItems = this.GetOrderedContainerItems(base.inventory, out flag);
		int num;
		this.currentRecipe = RecipeDictionary.GetMatchingRecipeAndQuantity(this.Recipes, orderedContainerItems, out num);
		this.currentQuantity = num;
		if (this.currentRecipe == null || !flag)
		{
			return;
		}
		if (this.currentRecipe.RequiresBlueprint && this.currentRecipe.ProducedItem != null && !player.blueprints.HasUnlocked(this.currentRecipe.ProducedItem))
		{
			return;
		}
		if (base.isServer)
		{
			this.lastTickTimestamp = UnityEngine.Time.realtimeSinceStartup;
		}
		this.RemainingMixTime = this.currentRecipe.MixingDuration * (float)this.currentQuantity;
		this.TotalMixTime = this.RemainingMixTime;
		this.ReturnExcessItems(orderedContainerItems, player);
		if (this.RemainingMixTime == 0f)
		{
			this.ProduceItem(this.currentRecipe, this.currentQuantity);
			return;
		}
		base.InvokeRepeating(new Action(this.TickMix), 1f, 1f);
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x00003A54 File Offset: 0x00001C54
	protected virtual bool CanStartMixing(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x00072FD8 File Offset: 0x000711D8
	public void StopMixing()
	{
		this.currentRecipe = null;
		this.currentQuantity = 0;
		this.RemainingMixTime = 0f;
		base.CancelInvoke(new Action(this.TickMix));
		if (!base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x0007302C File Offset: 0x0007122C
	private void TickMix()
	{
		if (this.currentRecipe == null)
		{
			this.StopMixing();
			return;
		}
		if (base.isServer)
		{
			this.lastTickTimestamp = UnityEngine.Time.realtimeSinceStartup;
			this.RemainingMixTime -= 1f;
		}
		base.SendNetworkUpdateImmediate(false);
		if (this.RemainingMixTime <= 0f)
		{
			this.ProduceItem(this.currentRecipe, this.currentQuantity);
		}
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x00073099 File Offset: 0x00071299
	private void ProduceItem(Recipe recipe, int quantity)
	{
		this.StopMixing();
		this.ConsumeInventory(recipe, quantity);
		this.CreateRecipeItems(recipe, quantity);
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x000730B4 File Offset: 0x000712B4
	private void ConsumeInventory(Recipe recipe, int quantity)
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item item = base.inventory.GetSlot(i);
			if (item != null)
			{
				if (this.GetItemWaterAmount(item) > 0)
				{
					item = item.contents.itemList[0];
				}
				int num = recipe.Ingredients[i].Count * quantity;
				if (num > 0)
				{
					item.UseItem(num);
				}
			}
		}
		ItemManager.DoRemoves();
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x00073128 File Offset: 0x00071328
	private void ReturnExcessItems(List<global::Item> orderedContainerItems, global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (this.currentRecipe == null)
		{
			return;
		}
		if (orderedContainerItems == null)
		{
			return;
		}
		if (orderedContainerItems.Count != this.currentRecipe.Ingredients.Length)
		{
			return;
		}
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot == null)
			{
				break;
			}
			int num = slot.amount - this.currentRecipe.Ingredients[i].Count * this.currentQuantity;
			if (num > 0)
			{
				global::Item item = slot.SplitItem(num);
				if (!item.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true) && !item.MoveToContainer(player.inventory.containerBelt, -1, true, false, null, true))
				{
					item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity, default(Quaternion));
				}
			}
		}
		ItemManager.DoRemoves();
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x00073224 File Offset: 0x00071424
	protected virtual void CreateRecipeItems(Recipe recipe, int quantity)
	{
		if (recipe == null)
		{
			return;
		}
		if (recipe.ProducedItem == null)
		{
			return;
		}
		int num = quantity * recipe.ProducedItemCount;
		int stackable = recipe.ProducedItem.stackable;
		int num2 = Mathf.CeilToInt((float)num / (float)stackable);
		this.currentProductionItem = recipe.ProducedItem;
		for (int i = 0; i < num2; i++)
		{
			int num3 = (num > stackable) ? stackable : num;
			global::Item item = ItemManager.Create(recipe.ProducedItem, num3, 0UL);
			if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
			{
				item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity, default(Quaternion));
			}
			num -= num3;
			if (num <= 0)
			{
				break;
			}
		}
		this.currentProductionItem = null;
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x000732EC File Offset: 0x000714EC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.mixingTable = Facepunch.Pool.Get<ProtoBuf.MixingTable>();
		if (info.forDisk)
		{
			info.msg.mixingTable.remainingMixTime = this.RemainingMixTime;
		}
		else
		{
			info.msg.mixingTable.remainingMixTime = this.RemainingMixTime - Mathf.Max(UnityEngine.Time.realtimeSinceStartup - this.lastTickTimestamp, 0f);
		}
		info.msg.mixingTable.totalMixTime = this.TotalMixTime;
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x00073374 File Offset: 0x00071574
	private int GetItemWaterAmount(global::Item item)
	{
		if (item == null)
		{
			return 0;
		}
		if (item.contents != null && item.contents.capacity == 1 && item.contents.allowedContents == global::ItemContainer.ContentsType.Liquid && item.contents.itemList.Count > 0)
		{
			return item.contents.itemList[0].amount;
		}
		return 0;
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x000733D8 File Offset: 0x000715D8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.mixingTable == null)
		{
			return;
		}
		this.RemainingMixTime = info.msg.mixingTable.remainingMixTime;
		this.TotalMixTime = info.msg.mixingTable.totalMixTime;
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x00073428 File Offset: 0x00071628
	public List<global::Item> GetOrderedContainerItems(global::ItemContainer container, out bool itemsAreContiguous)
	{
		itemsAreContiguous = true;
		if (container == null)
		{
			return null;
		}
		if (container.itemList == null)
		{
			return null;
		}
		if (container.itemList.Count == 0)
		{
			return null;
		}
		this.inventoryItems.Clear();
		bool flag = false;
		for (int i = 0; i < container.capacity; i++)
		{
			global::Item item = container.GetSlot(i);
			if (item != null && flag)
			{
				itemsAreContiguous = false;
				break;
			}
			if (item == null)
			{
				flag = true;
			}
			else
			{
				if (this.GetItemWaterAmount(item) > 0)
				{
					item = item.contents.itemList[0];
				}
				this.inventoryItems.Add(item);
			}
		}
		return this.inventoryItems;
	}

	// Token: 0x040008DC RID: 2268
	public GameObject Particles;

	// Token: 0x040008DD RID: 2269
	public RecipeList Recipes;

	// Token: 0x040008DE RID: 2270
	public bool OnlyAcceptValidIngredients;

	// Token: 0x040008E1 RID: 2273
	private float lastTickTimestamp;

	// Token: 0x040008E2 RID: 2274
	private List<global::Item> inventoryItems = new List<global::Item>();

	// Token: 0x040008E4 RID: 2276
	private const float mixTickInterval = 1f;

	// Token: 0x040008E5 RID: 2277
	private Recipe currentRecipe;

	// Token: 0x040008E6 RID: 2278
	private int currentQuantity;

	// Token: 0x040008E7 RID: 2279
	protected ItemDefinition currentProductionItem;
}
