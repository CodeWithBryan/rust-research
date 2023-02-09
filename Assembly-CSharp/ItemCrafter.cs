using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000598 RID: 1432
public class ItemCrafter : EntityComponent<global::BasePlayer>
{
	// Token: 0x06002A9E RID: 10910 RVA: 0x00101AE4 File Offset: 0x000FFCE4
	public void AddContainer(global::ItemContainer container)
	{
		this.containers.Add(container);
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x00101AF4 File Offset: 0x000FFCF4
	public static float GetScaledDuration(ItemBlueprint bp, float workbenchLevel)
	{
		float num = workbenchLevel - (float)bp.workbenchLevelRequired;
		if (num == 1f)
		{
			return bp.time * 0.5f;
		}
		if (num >= 2f)
		{
			return bp.time * 0.25f;
		}
		return bp.time;
	}

	// Token: 0x06002AA0 RID: 10912 RVA: 0x00101B3C File Offset: 0x000FFD3C
	public void ServerUpdate(float delta)
	{
		if (this.queue.Count == 0)
		{
			return;
		}
		ItemCraftTask value = this.queue.First.Value;
		if (value.cancelled)
		{
			value.owner.Command("note.craft_done", new object[]
			{
				value.taskUID,
				0
			});
			this.queue.RemoveFirst();
			return;
		}
		float currentCraftLevel = value.owner.currentCraftLevel;
		if (value.endTime > UnityEngine.Time.realtimeSinceStartup)
		{
			return;
		}
		if (value.endTime == 0f)
		{
			float scaledDuration = ItemCrafter.GetScaledDuration(value.blueprint, currentCraftLevel);
			value.endTime = UnityEngine.Time.realtimeSinceStartup + scaledDuration;
			if (value.owner != null)
			{
				value.owner.Command("note.craft_start", new object[]
				{
					value.taskUID,
					scaledDuration,
					value.amount
				});
				if (value.owner.IsAdmin && Craft.instant)
				{
					value.endTime = UnityEngine.Time.realtimeSinceStartup + 1f;
				}
			}
			return;
		}
		this.FinishCrafting(value);
		if (value.amount <= 0)
		{
			this.queue.RemoveFirst();
			return;
		}
		value.endTime = 0f;
	}

	// Token: 0x06002AA1 RID: 10913 RVA: 0x00101C84 File Offset: 0x000FFE84
	private void CollectIngredient(int item, int amount, List<global::Item> collect)
	{
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			amount -= itemContainer.Take(collect, item, amount);
			if (amount <= 0)
			{
				break;
			}
		}
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x00101CE4 File Offset: 0x000FFEE4
	private void CollectIngredients(ItemBlueprint bp, ItemCraftTask task, int amount = 1, global::BasePlayer player = null)
	{
		List<global::Item> list = new List<global::Item>();
		foreach (ItemAmount itemAmount in bp.ingredients)
		{
			this.CollectIngredient(itemAmount.itemid, (int)itemAmount.amount * amount, list);
		}
		task.potentialOwners = new List<ulong>();
		foreach (global::Item item in list)
		{
			item.CollectedForCrafting(player);
			if (!task.potentialOwners.Contains(player.userID))
			{
				task.potentialOwners.Add(player.userID);
			}
		}
		task.takenItems = list;
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x00101DC0 File Offset: 0x000FFFC0
	public bool CraftItem(ItemBlueprint bp, global::BasePlayer owner, ProtoBuf.Item.InstanceData instanceData = null, int amount = 1, int skinID = 0, global::Item fromTempBlueprint = null, bool free = false)
	{
		if (!this.CanCraft(bp, amount, free))
		{
			return false;
		}
		this.taskUID++;
		ItemCraftTask itemCraftTask = Facepunch.Pool.Get<ItemCraftTask>();
		itemCraftTask.blueprint = bp;
		if (!free)
		{
			this.CollectIngredients(bp, itemCraftTask, amount, owner);
		}
		itemCraftTask.endTime = 0f;
		itemCraftTask.taskUID = this.taskUID;
		itemCraftTask.owner = owner;
		itemCraftTask.instanceData = instanceData;
		if (itemCraftTask.instanceData != null)
		{
			itemCraftTask.instanceData.ShouldPool = false;
		}
		itemCraftTask.amount = amount;
		itemCraftTask.skinID = skinID;
		if (fromTempBlueprint != null && itemCraftTask.takenItems != null)
		{
			fromTempBlueprint.RemoveFromContainer();
			itemCraftTask.takenItems.Add(fromTempBlueprint);
			itemCraftTask.conditionScale = 0.5f;
		}
		this.queue.AddLast(itemCraftTask);
		if (itemCraftTask.owner != null)
		{
			itemCraftTask.owner.Command("note.craft_add", new object[]
			{
				itemCraftTask.taskUID,
				itemCraftTask.blueprint.targetItem.itemid,
				amount,
				itemCraftTask.skinID
			});
		}
		return true;
	}

	// Token: 0x06002AA4 RID: 10916 RVA: 0x00101EEC File Offset: 0x001000EC
	private void FinishCrafting(ItemCraftTask task)
	{
		task.amount--;
		task.numCrafted++;
		ulong skin = ItemDefinition.FindSkin(task.blueprint.targetItem.itemid, task.skinID);
		global::Item item = ItemManager.CreateByItemID(task.blueprint.targetItem.itemid, 1, skin);
		item.amount = task.blueprint.amountToCreate;
		int amount = item.amount;
		if (item.hasCondition && task.conditionScale != 1f)
		{
			item.maxCondition *= task.conditionScale;
			item.condition = item.maxCondition;
		}
		item.OnVirginSpawn();
		foreach (ItemAmount itemAmount in task.blueprint.ingredients)
		{
			int num = (int)itemAmount.amount;
			if (task.takenItems != null)
			{
				foreach (global::Item item2 in task.takenItems)
				{
					if (item2.info == itemAmount.itemDef)
					{
						int num2 = Mathf.Min(item2.amount, num);
						item2.UseItem(num);
						num -= num2;
					}
				}
			}
		}
		Analytics.Server.Crafting(task.blueprint.targetItem.shortname, task.skinID);
		task.owner.Command("note.craft_done", new object[]
		{
			task.taskUID,
			1,
			task.amount
		});
		if (task.instanceData != null)
		{
			item.instanceData = task.instanceData;
		}
		if (!string.IsNullOrEmpty(task.blueprint.UnlockAchievment))
		{
			task.owner.GiveAchievement(task.blueprint.UnlockAchievment);
		}
		if (task.owner.inventory.GiveItem(item, null))
		{
			task.owner.Command("note.inv", new object[]
			{
				item.info.itemid,
				amount
			});
			return;
		}
		global::ItemContainer itemContainer = this.containers.First<global::ItemContainer>();
		task.owner.Command("note.inv", new object[]
		{
			item.info.itemid,
			amount
		});
		task.owner.Command("note.inv", new object[]
		{
			item.info.itemid,
			-item.amount
		});
		item.Drop(itemContainer.dropPosition, itemContainer.dropVelocity, default(Quaternion));
	}

	// Token: 0x06002AA5 RID: 10917 RVA: 0x001021DC File Offset: 0x001003DC
	public bool CancelTask(int iID, bool ReturnItems)
	{
		if (this.queue.Count == 0)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = this.queue.FirstOrDefault((ItemCraftTask x) => x.taskUID == iID && !x.cancelled);
		if (itemCraftTask == null)
		{
			return false;
		}
		itemCraftTask.cancelled = true;
		if (itemCraftTask.owner == null)
		{
			return true;
		}
		itemCraftTask.owner.Command("note.craft_done", new object[]
		{
			itemCraftTask.taskUID,
			0
		});
		if (itemCraftTask.takenItems != null && itemCraftTask.takenItems.Count > 0 && ReturnItems)
		{
			foreach (global::Item item in itemCraftTask.takenItems)
			{
				if (item != null && item.amount > 0)
				{
					if (item.IsBlueprint() && item.blueprintTargetDef == itemCraftTask.blueprint.targetItem)
					{
						item.UseItem(itemCraftTask.numCrafted);
					}
					if (item.amount > 0 && !item.MoveToContainer(itemCraftTask.owner.inventory.containerMain, -1, true, false, null, true))
					{
						item.Drop(itemCraftTask.owner.inventory.containerMain.dropPosition + UnityEngine.Random.value * Vector3.down + UnityEngine.Random.insideUnitSphere, itemCraftTask.owner.inventory.containerMain.dropVelocity, default(Quaternion));
						itemCraftTask.owner.Command("note.inv", new object[]
						{
							item.info.itemid,
							-item.amount
						});
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x001023D4 File Offset: 0x001005D4
	public bool CancelBlueprint(int itemid)
	{
		if (this.queue.Count == 0)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = this.queue.FirstOrDefault((ItemCraftTask x) => x.blueprint.targetItem.itemid == itemid && !x.cancelled);
		return itemCraftTask != null && this.CancelTask(itemCraftTask.taskUID, true);
	}

	// Token: 0x06002AA7 RID: 10919 RVA: 0x00102428 File Offset: 0x00100628
	public void CancelAll(bool returnItems)
	{
		foreach (ItemCraftTask itemCraftTask in this.queue)
		{
			this.CancelTask(itemCraftTask.taskUID, returnItems);
		}
	}

	// Token: 0x06002AA8 RID: 10920 RVA: 0x00102484 File Offset: 0x00100684
	private bool DoesHaveUsableItem(int item, int iAmount)
	{
		int num = 0;
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			num += itemContainer.GetAmount(item, true);
		}
		return num >= iAmount;
	}

	// Token: 0x06002AA9 RID: 10921 RVA: 0x001024E4 File Offset: 0x001006E4
	public bool CanCraft(ItemBlueprint bp, int amount = 1, bool free = false)
	{
		float num = (float)amount / (float)bp.targetItem.craftingStackable;
		foreach (ItemCraftTask itemCraftTask in this.queue)
		{
			if (!itemCraftTask.cancelled)
			{
				num += (float)itemCraftTask.amount / (float)itemCraftTask.blueprint.targetItem.craftingStackable;
			}
		}
		if (num > 8f)
		{
			return false;
		}
		if (amount < 1 || amount > bp.targetItem.craftingStackable)
		{
			return false;
		}
		foreach (ItemAmount itemAmount in bp.ingredients)
		{
			if (!this.DoesHaveUsableItem(itemAmount.itemid, (int)itemAmount.amount * amount))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002AAA RID: 10922 RVA: 0x001025E0 File Offset: 0x001007E0
	public bool CanCraft(ItemDefinition def, int amount = 1, bool free = false)
	{
		ItemBlueprint component = def.GetComponent<ItemBlueprint>();
		return this.CanCraft(component, amount, free);
	}

	// Token: 0x06002AAB RID: 10923 RVA: 0x00102604 File Offset: 0x00100804
	public bool FastTrackTask(int taskID)
	{
		if (this.queue.Count == 0)
		{
			return false;
		}
		ItemCraftTask value = this.queue.First.Value;
		if (value == null)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = this.queue.FirstOrDefault((ItemCraftTask x) => x.taskUID == taskID && !x.cancelled);
		if (itemCraftTask == null)
		{
			return false;
		}
		if (itemCraftTask == value)
		{
			return false;
		}
		value.endTime = 0f;
		this.queue.Remove(itemCraftTask);
		this.queue.AddFirst(itemCraftTask);
		itemCraftTask.owner.Command("note.craft_fasttracked", new object[]
		{
			taskID
		});
		return true;
	}

	// Token: 0x040022B0 RID: 8880
	public List<global::ItemContainer> containers = new List<global::ItemContainer>();

	// Token: 0x040022B1 RID: 8881
	public LinkedList<ItemCraftTask> queue = new LinkedList<ItemCraftTask>();

	// Token: 0x040022B2 RID: 8882
	public int taskUID;
}
