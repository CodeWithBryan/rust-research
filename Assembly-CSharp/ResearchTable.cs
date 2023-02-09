using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B9 RID: 185
public class ResearchTable : StorageContainer
{
	// Token: 0x0600108F RID: 4239 RVA: 0x00087CC0 File Offset: 0x00085EC0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ResearchTable.OnRpcMessage", 0))
		{
			if (rpc == 3177710095U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoResearch ");
				}
				using (TimeWarning.New("DoResearch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3177710095U, "DoResearch", this, player, 3f))
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
							this.DoResearch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoResearch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x00087E28 File Offset: 0x00086028
	public override void ResetState()
	{
		base.ResetState();
		this.researchFinishedTime = 0f;
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x00087E3C File Offset: 0x0008603C
	public override int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		if (item.info.shortname == "scrap")
		{
			global::Item slot = base.inventory.GetSlot(1);
			if (slot == null)
			{
				return 1;
			}
			if (slot.amount < item.MaxStackable())
			{
				return 1;
			}
		}
		return base.GetIdealSlot(player, item);
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x0002782C File Offset: 0x00025A2C
	public bool IsResearching()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x00087E8A File Offset: 0x0008608A
	public int RarityMultiplier(Rarity rarity)
	{
		if (rarity == Rarity.Common)
		{
			return 20;
		}
		if (rarity == Rarity.Uncommon)
		{
			return 15;
		}
		if (rarity == Rarity.Rare)
		{
			return 10;
		}
		return 5;
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x00087EA4 File Offset: 0x000860A4
	public int GetBlueprintStacksize(global::Item sourceItem)
	{
		int result = this.RarityMultiplier(sourceItem.info.rarity);
		if (sourceItem.info.category == ItemCategory.Ammunition)
		{
			result = Mathf.FloorToInt((float)sourceItem.MaxStackable() / (float)sourceItem.info.Blueprint.amountToCreate) * 2;
		}
		return result;
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x00087EF4 File Offset: 0x000860F4
	public int ScrapForResearch(global::Item item)
	{
		int result = 0;
		if (item.info.rarity == Rarity.Common)
		{
			result = 20;
		}
		if (item.info.rarity == Rarity.Uncommon)
		{
			result = 75;
		}
		if (item.info.rarity == Rarity.Rare)
		{
			result = 125;
		}
		if (item.info.rarity == Rarity.VeryRare || item.info.rarity == Rarity.None)
		{
			result = 500;
		}
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(item.info);
		if (itemBlueprint != null && itemBlueprint.defaultBlueprint)
		{
			return ConVar.Server.defaultBlueprintResearchCost;
		}
		return result;
	}

	// Token: 0x06001096 RID: 4246 RVA: 0x00087F7C File Offset: 0x0008617C
	public static int ScrapForResearch(ItemDefinition info, global::ResearchTable.ResearchType type)
	{
		int num = 0;
		if (info.rarity == Rarity.Common)
		{
			num = 20;
		}
		if (info.rarity == Rarity.Uncommon)
		{
			num = 75;
		}
		if (info.rarity == Rarity.Rare)
		{
			num = 125;
		}
		if (info.rarity == Rarity.VeryRare || info.rarity == Rarity.None)
		{
			num = 500;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null)
		{
			BaseGameMode.ResearchCostResult scrapCostForResearch = activeGameMode.GetScrapCostForResearch(info, type);
			if (scrapCostForResearch.Scale != null)
			{
				num = Mathf.RoundToInt((float)num * scrapCostForResearch.Scale.Value);
			}
			else if (scrapCostForResearch.Amount != null)
			{
				num = scrapCostForResearch.Amount.Value;
			}
		}
		return num;
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x00088020 File Offset: 0x00086220
	public bool IsItemResearchable(global::Item item)
	{
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint((item.info.isRedirectOf != null) ? item.info.isRedirectOf : item.info);
		return (itemBlueprint != null && itemBlueprint.defaultBlueprint) || (!(itemBlueprint == null) && itemBlueprint.isResearchable);
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x00088080 File Offset: 0x00086280
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.ItemFilter);
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x000880A0 File Offset: 0x000862A0
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		return (targetSlot != 1 || !(item.info != this.researchResource)) && base.ItemFilter(item, targetSlot);
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x000880C3 File Offset: 0x000862C3
	public global::Item GetTargetItem()
	{
		return base.inventory.GetSlot(0);
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x000880D4 File Offset: 0x000862D4
	public global::Item GetScrapItem()
	{
		global::Item slot = base.inventory.GetSlot(1);
		if (slot == null || slot.info != this.researchResource)
		{
			return null;
		}
		return slot;
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x00088107 File Offset: 0x00086307
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.HasFlag(global::BaseEntity.Flags.On))
		{
			base.Invoke(new Action(this.ResearchAttemptFinished), this.researchDuration);
		}
		base.inventory.SetLocked(false);
	}

	// Token: 0x0600109D RID: 4253 RVA: 0x0008813C File Offset: 0x0008633C
	public override bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		this.user = player;
		return base.PlayerOpenLoot(player, "", true);
	}

	// Token: 0x0600109E RID: 4254 RVA: 0x00088152 File Offset: 0x00086352
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		this.user = null;
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x00088164 File Offset: 0x00086364
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void DoResearch(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsResearching())
		{
			return;
		}
		global::BasePlayer player = msg.player;
		global::Item targetItem = this.GetTargetItem();
		if (targetItem == null)
		{
			return;
		}
		if (targetItem.amount > 1)
		{
			return;
		}
		if (!this.IsItemResearchable(targetItem))
		{
			return;
		}
		targetItem.CollectedForCrafting(player);
		this.researchFinishedTime = UnityEngine.Time.realtimeSinceStartup + this.researchDuration;
		base.Invoke(new Action(this.ResearchAttemptFinished), this.researchDuration);
		base.inventory.SetLocked(true);
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		player.inventory.loot.SendImmediate();
		if (this.researchStartEffect.isValid)
		{
			Effect.server.Run(this.researchStartEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		msg.player.GiveAchievement("RESEARCH_ITEM");
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x0008823C File Offset: 0x0008643C
	public void ResearchAttemptFinished()
	{
		global::Item targetItem = this.GetTargetItem();
		global::Item scrapItem = this.GetScrapItem();
		if (targetItem != null && scrapItem != null)
		{
			int num = this.ScrapForResearch(targetItem);
			if (scrapItem.amount >= num)
			{
				if (scrapItem.amount == num)
				{
					base.inventory.Remove(scrapItem);
					scrapItem.RemoveFromContainer();
					scrapItem.Remove(0f);
				}
				else
				{
					scrapItem.UseItem(num);
				}
				base.inventory.Remove(targetItem);
				targetItem.Remove(0f);
				global::Item item = ItemManager.Create(ItemManager.blueprintBaseDef, 1, 0UL);
				item.blueprintTarget = ((targetItem.info.isRedirectOf != null) ? targetItem.info.isRedirectOf.itemid : targetItem.info.itemid);
				if (!item.MoveToContainer(base.inventory, 0, true, false, null, true))
				{
					item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
				if (this.researchSuccessEffect.isValid)
				{
					Effect.server.Run(this.researchSuccessEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
				}
			}
		}
		base.SendNetworkUpdateImmediate(false);
		if (this.user != null)
		{
			this.user.inventory.loot.SendImmediate();
		}
		this.EndResearch();
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x000059DD File Offset: 0x00003BDD
	public void CancelResearch()
	{
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x00088390 File Offset: 0x00086590
	public void EndResearch()
	{
		base.inventory.SetLocked(false);
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		this.researchFinishedTime = 0f;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (this.user != null)
		{
			this.user.inventory.loot.SendImmediate();
		}
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x000883E8 File Offset: 0x000865E8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.researchTable = Facepunch.Pool.Get<ProtoBuf.ResearchTable>();
		info.msg.researchTable.researchTimeLeft = this.researchFinishedTime - UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x0008841D File Offset: 0x0008661D
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.researchTable != null)
		{
			this.researchFinishedTime = UnityEngine.Time.realtimeSinceStartup + info.msg.researchTable.researchTimeLeft;
		}
	}

	// Token: 0x04000A5B RID: 2651
	[NonSerialized]
	public float researchFinishedTime;

	// Token: 0x04000A5C RID: 2652
	public float researchCostFraction = 1f;

	// Token: 0x04000A5D RID: 2653
	public float researchDuration = 10f;

	// Token: 0x04000A5E RID: 2654
	public int requiredPaper = 10;

	// Token: 0x04000A5F RID: 2655
	public GameObjectRef researchStartEffect;

	// Token: 0x04000A60 RID: 2656
	public GameObjectRef researchFailEffect;

	// Token: 0x04000A61 RID: 2657
	public GameObjectRef researchSuccessEffect;

	// Token: 0x04000A62 RID: 2658
	public ItemDefinition researchResource;

	// Token: 0x04000A63 RID: 2659
	private global::BasePlayer user;

	// Token: 0x02000BAC RID: 2988
	public enum ResearchType
	{
		// Token: 0x04003F21 RID: 16161
		ResearchTable,
		// Token: 0x04003F22 RID: 16162
		TechTree
	}
}
