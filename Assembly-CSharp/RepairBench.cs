using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B8 RID: 184
public class RepairBench : StorageContainer
{
	// Token: 0x06001083 RID: 4227 RVA: 0x000870B0 File Offset: 0x000852B0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RepairBench.OnRpcMessage", 0))
		{
			if (rpc == 1942825351U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ChangeSkin ");
				}
				using (TimeWarning.New("ChangeSkin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1942825351U, "ChangeSkin", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ChangeSkin(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ChangeSkin");
					}
				}
				return true;
			}
			if (rpc == 1178348163U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RepairItem ");
				}
				using (TimeWarning.New("RepairItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1178348163U, "RepairItem", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RepairItem(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RepairItem");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x000873B0 File Offset: 0x000855B0
	public static float GetRepairFraction(Item itemToRepair)
	{
		return 1f - itemToRepair.condition / itemToRepair.maxCondition;
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x000873C5 File Offset: 0x000855C5
	public static float RepairCostFraction(Item itemToRepair)
	{
		return RepairBench.GetRepairFraction(itemToRepair) * 0.2f;
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x000873D4 File Offset: 0x000855D4
	public static void GetRepairCostList(ItemBlueprint bp, List<ItemAmount> allIngredients)
	{
		foreach (ItemAmount itemAmount in bp.ingredients)
		{
			allIngredients.Add(new ItemAmount(itemAmount.itemDef, itemAmount.amount));
		}
		RepairBench.StripComponentRepairCost(allIngredients);
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x00087440 File Offset: 0x00085640
	public static void StripComponentRepairCost(List<ItemAmount> allIngredients)
	{
		if (allIngredients == null)
		{
			return;
		}
		for (int i = 0; i < allIngredients.Count; i++)
		{
			ItemAmount itemAmount = allIngredients[i];
			if (itemAmount.itemDef.category == ItemCategory.Component)
			{
				if (itemAmount.itemDef.Blueprint != null)
				{
					bool flag = false;
					ItemAmount itemAmount2 = itemAmount.itemDef.Blueprint.ingredients[0];
					foreach (ItemAmount itemAmount3 in allIngredients)
					{
						if (itemAmount3.itemDef == itemAmount2.itemDef)
						{
							itemAmount3.amount += itemAmount2.amount * itemAmount.amount;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						allIngredients.Add(new ItemAmount(itemAmount2.itemDef, itemAmount2.amount * itemAmount.amount));
					}
				}
				allIngredients.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x0008754C File Offset: 0x0008574C
	public void debugprint(string toPrint)
	{
		if (Global.developer > 0)
		{
			Debug.LogWarning(toPrint);
		}
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x0008755C File Offset: 0x0008575C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ChangeSkin(BaseEntity.RPCMessage msg)
	{
		if (UnityEngine.Time.realtimeSinceStartup < this.nextSkinChangeTime)
		{
			return;
		}
		BasePlayer player = msg.player;
		int num = msg.read.Int32();
		Item slot = base.inventory.GetSlot(0);
		if (slot == null)
		{
			return;
		}
		bool flag = false;
		if (num != 0 && !flag && !player.blueprints.CheckSkinOwnership(num, player.userID))
		{
			this.debugprint("RepairBench.ChangeSkin player does not have item :" + num + ":");
			return;
		}
		ulong Skin = ItemDefinition.FindSkin(slot.info.itemid, num);
		if (Skin == slot.skin && slot.info.isRedirectOf == null)
		{
			this.debugprint(string.Concat(new object[]
			{
				"RepairBench.ChangeSkin cannot apply same skin twice : ",
				Skin,
				": ",
				slot.skin
			}));
			return;
		}
		this.nextSkinChangeTime = UnityEngine.Time.realtimeSinceStartup + 0.75f;
		ItemSkinDirectory.Skin skin = slot.info.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => (long)x.id == (long)Skin);
		if (slot.info.isRedirectOf != null)
		{
			Skin = ItemDefinition.FindSkin(slot.info.isRedirectOf.itemid, num);
			skin = slot.info.isRedirectOf.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => (long)x.id == (long)Skin);
		}
		ItemSkin itemSkin = (skin.id == 0) ? null : (skin.invItem as ItemSkin);
		if ((itemSkin && (itemSkin.Redirect != null || slot.info.isRedirectOf != null)) || (!itemSkin && slot.info.isRedirectOf != null))
		{
			ItemDefinition template = (itemSkin != null) ? itemSkin.Redirect : slot.info.isRedirectOf;
			bool flag2 = false;
			if (itemSkin != null && itemSkin.Redirect == null && slot.info.isRedirectOf != null)
			{
				template = slot.info.isRedirectOf;
				flag2 = (num != 0);
			}
			float condition = slot.condition;
			float maxCondition = slot.maxCondition;
			int amount = slot.amount;
			int contents = 0;
			ItemDefinition ammoType = null;
			BaseProjectile baseProjectile;
			if (slot.GetHeldEntity() != null && (baseProjectile = (slot.GetHeldEntity() as BaseProjectile)) != null && baseProjectile.primaryMagazine != null)
			{
				contents = baseProjectile.primaryMagazine.contents;
				ammoType = baseProjectile.primaryMagazine.ammoType;
			}
			List<Item> list = Facepunch.Pool.GetList<Item>();
			if (slot.contents != null && slot.contents.itemList != null && slot.contents.itemList.Count > 0)
			{
				foreach (Item item in slot.contents.itemList)
				{
					list.Add(item);
				}
				foreach (Item item2 in list)
				{
					item2.RemoveFromContainer();
				}
			}
			slot.Remove(0f);
			ItemManager.DoRemoves();
			Item item3 = ItemManager.Create(template, 1, 0UL);
			item3.MoveToContainer(base.inventory, 0, false, false, null, true);
			item3.maxCondition = maxCondition;
			item3.condition = condition;
			item3.amount = amount;
			BaseProjectile baseProjectile2;
			if (item3.GetHeldEntity() != null && (baseProjectile2 = (item3.GetHeldEntity() as BaseProjectile)) != null)
			{
				if (baseProjectile2.primaryMagazine != null)
				{
					baseProjectile2.primaryMagazine.contents = contents;
					baseProjectile2.primaryMagazine.ammoType = ammoType;
				}
				baseProjectile2.ForceModsChanged();
			}
			if (list.Count > 0 && item3.contents != null)
			{
				foreach (Item item4 in list)
				{
					item4.MoveToContainer(item3.contents, -1, true, false, null, true);
				}
			}
			Facepunch.Pool.FreeList<Item>(ref list);
			if (flag2)
			{
				this.ApplySkinToItem(item3, Skin);
			}
			Analytics.Server.SkinUsed(item3.info.shortname, num);
		}
		else
		{
			this.ApplySkinToItem(slot, Skin);
			Analytics.Server.SkinUsed(slot.info.shortname, num);
		}
		if (this.skinchangeEffect.isValid)
		{
			Effect.server.Run(this.skinchangeEffect.resourcePath, this, 0U, new Vector3(0f, 1.5f, 0f), Vector3.zero, null, false);
		}
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x00087A34 File Offset: 0x00085C34
	private void ApplySkinToItem(Item item, ulong Skin)
	{
		item.skin = Skin;
		item.MarkDirty();
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity != null)
		{
			heldEntity.skinID = Skin;
			heldEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x00087A6C File Offset: 0x00085C6C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RepairItem(BaseEntity.RPCMessage msg)
	{
		Item slot = base.inventory.GetSlot(0);
		BasePlayer player = msg.player;
		RepairBench.RepairAnItem(slot, player, this, this.maxConditionLostOnRepair, true);
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x00007074 File Offset: 0x00005274
	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		return 0;
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x00087A9C File Offset: 0x00085C9C
	public static void RepairAnItem(Item itemToRepair, BasePlayer player, BaseEntity repairBenchEntity, float maxConditionLostOnRepair, bool mustKnowBlueprint)
	{
		if (itemToRepair == null)
		{
			return;
		}
		ItemDefinition info = itemToRepair.info;
		ItemBlueprint component = info.GetComponent<ItemBlueprint>();
		if (!component)
		{
			return;
		}
		if (!info.condition.repairable)
		{
			return;
		}
		if (itemToRepair.condition == itemToRepair.maxCondition)
		{
			return;
		}
		if (mustKnowBlueprint)
		{
			ItemDefinition itemDefinition = (info.isRedirectOf != null) ? info.isRedirectOf : info;
			if (!player.blueprints.HasUnlocked(itemDefinition) && (!(itemDefinition.Blueprint != null) || itemDefinition.Blueprint.isResearchable))
			{
				return;
			}
		}
		float num = RepairBench.RepairCostFraction(itemToRepair);
		bool flag = false;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		RepairBench.GetRepairCostList(component, list);
		foreach (ItemAmount itemAmount in list)
		{
			if (itemAmount.itemDef.category != ItemCategory.Component)
			{
				int amount = player.inventory.GetAmount(itemAmount.itemDef.itemid);
				if (Mathf.CeilToInt(itemAmount.amount * num) > amount)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			Facepunch.Pool.FreeList<ItemAmount>(ref list);
			return;
		}
		foreach (ItemAmount itemAmount2 in list)
		{
			if (itemAmount2.itemDef.category != ItemCategory.Component)
			{
				int amount2 = Mathf.CeilToInt(itemAmount2.amount * num);
				player.inventory.Take(null, itemAmount2.itemid, amount2);
			}
		}
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		itemToRepair.DoRepair(maxConditionLostOnRepair);
		if (Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Item repaired! condition : ",
				itemToRepair.condition,
				"/",
				itemToRepair.maxCondition
			}));
		}
		Effect.server.Run("assets/bundled/prefabs/fx/repairbench/itemrepair.prefab", repairBenchEntity, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x04000A57 RID: 2647
	public float maxConditionLostOnRepair = 0.2f;

	// Token: 0x04000A58 RID: 2648
	public GameObjectRef skinchangeEffect;

	// Token: 0x04000A59 RID: 2649
	public const float REPAIR_COST_FRACTION = 0.2f;

	// Token: 0x04000A5A RID: 2650
	private float nextSkinChangeTime;
}
