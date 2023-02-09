using System;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000EA RID: 234
public class Workbench : StorageContainer
{
	// Token: 0x0600144F RID: 5199 RVA: 0x000A0B40 File Offset: 0x0009ED40
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Workbench.OnRpcMessage", 0))
		{
			if (rpc == 2308794761U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_BeginExperiment ");
				}
				using (TimeWarning.New("RPC_BeginExperiment", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2308794761U, "RPC_BeginExperiment", this, player, 3f))
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
							this.RPC_BeginExperiment(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_BeginExperiment");
					}
				}
				return true;
			}
			if (rpc == 4127240744U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TechTreeUnlock ");
				}
				using (TimeWarning.New("RPC_TechTreeUnlock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4127240744U, "RPC_TechTreeUnlock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_TechTreeUnlock(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_TechTreeUnlock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x000A0E40 File Offset: 0x0009F040
	public int GetScrapForExperiment()
	{
		if (this.Workbenchlevel == 1)
		{
			return 75;
		}
		if (this.Workbenchlevel == 2)
		{
			return 300;
		}
		if (this.Workbenchlevel == 3)
		{
			return 1000;
		}
		Debug.LogWarning("GetScrapForExperiment fucked up big time.");
		return 0;
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x0002782C File Offset: 0x00025A2C
	public bool IsWorking()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06001452 RID: 5202 RVA: 0x000A0E77 File Offset: 0x0009F077
	public override bool CanPickup(global::BasePlayer player)
	{
		return this.children.Count == 0 && base.CanPickup(player);
	}

	// Token: 0x06001453 RID: 5203 RVA: 0x000A0E90 File Offset: 0x0009F090
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_TechTreeUnlock(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		int num = msg.read.Int32();
		TechTreeData.NodeInstance byID = this.techTree.GetByID(num);
		if (byID == null)
		{
			Debug.Log("Node for unlock not found :" + num);
			return;
		}
		if (this.techTree.PlayerCanUnlock(player, byID))
		{
			if (byID.IsGroup())
			{
				foreach (int id in byID.outputs)
				{
					TechTreeData.NodeInstance byID2 = this.techTree.GetByID(id);
					if (byID2 != null && byID2.itemDef != null)
					{
						player.blueprints.Unlock(byID2.itemDef);
					}
				}
				Debug.Log("Player unlocked group :" + byID.groupName);
				return;
			}
			if (byID.itemDef != null)
			{
				int num2 = global::ResearchTable.ScrapForResearch(byID.itemDef, global::ResearchTable.ResearchType.TechTree);
				int itemid = ItemManager.FindItemDefinition("scrap").itemid;
				if (player.inventory.GetAmount(itemid) >= num2)
				{
					player.inventory.Take(null, itemid, num2);
					player.blueprints.Unlock(byID.itemDef);
				}
			}
		}
	}

	// Token: 0x06001454 RID: 5204 RVA: 0x000A0FDC File Offset: 0x0009F1DC
	public static ItemDefinition GetBlueprintTemplate()
	{
		if (Workbench.blueprintBaseDef == null)
		{
			Workbench.blueprintBaseDef = ItemManager.FindItemDefinition("blueprintbase");
		}
		return Workbench.blueprintBaseDef;
	}

	// Token: 0x06001455 RID: 5205 RVA: 0x000A1000 File Offset: 0x0009F200
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_BeginExperiment(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (this.IsWorking())
		{
			return;
		}
		PersistantPlayer persistantPlayerInfo = player.PersistantPlayerInfo;
		int num = UnityEngine.Random.Range(0, this.experimentalItems.subSpawn.Length);
		for (int i = 0; i < this.experimentalItems.subSpawn.Length; i++)
		{
			int num2 = i + num;
			if (num2 >= this.experimentalItems.subSpawn.Length)
			{
				num2 -= this.experimentalItems.subSpawn.Length;
			}
			ItemDefinition itemDef = this.experimentalItems.subSpawn[num2].category.items[0].itemDef;
			if (itemDef.Blueprint && !itemDef.Blueprint.defaultBlueprint && itemDef.Blueprint.userCraftable && itemDef.Blueprint.isResearchable && !itemDef.Blueprint.NeedsSteamItem && !itemDef.Blueprint.NeedsSteamDLC && !persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid))
			{
				this.pendingBlueprint = itemDef;
				break;
			}
		}
		if (this.pendingBlueprint == null)
		{
			player.ChatMessage("You have already unlocked everything for this workbench tier.");
			return;
		}
		global::Item slot = base.inventory.GetSlot(0);
		if (slot != null)
		{
			if (!slot.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
			player.inventory.loot.SendImmediate();
		}
		if (this.experimentStartEffect.isValid)
		{
			Effect.server.Run(this.experimentStartEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.inventory.SetLocked(true);
		base.CancelInvoke(new Action(this.ExperimentComplete));
		base.Invoke(new Action(this.ExperimentComplete), 5f);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x000A1204 File Offset: 0x0009F404
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x000A120D File Offset: 0x0009F40D
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		base.CancelInvoke(new Action(this.ExperimentComplete));
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x000A1228 File Offset: 0x0009F428
	public int GetAvailableExperimentResources()
	{
		global::Item experimentResourceItem = this.GetExperimentResourceItem();
		if (experimentResourceItem == null || experimentResourceItem.info != this.experimentResource)
		{
			return 0;
		}
		return experimentResourceItem.amount;
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x000A125A File Offset: 0x0009F45A
	public global::Item GetExperimentResourceItem()
	{
		return base.inventory.GetSlot(1);
	}

	// Token: 0x0600145A RID: 5210 RVA: 0x000A1268 File Offset: 0x0009F468
	public void ExperimentComplete()
	{
		global::Item experimentResourceItem = this.GetExperimentResourceItem();
		int scrapForExperiment = this.GetScrapForExperiment();
		if (this.pendingBlueprint == null)
		{
			Debug.LogWarning("Pending blueprint was null!");
		}
		if (experimentResourceItem != null && experimentResourceItem.amount >= scrapForExperiment && this.pendingBlueprint != null)
		{
			experimentResourceItem.UseItem(scrapForExperiment);
			global::Item item = ItemManager.Create(Workbench.GetBlueprintTemplate(), 1, 0UL);
			item.blueprintTarget = this.pendingBlueprint.itemid;
			this.creatingBlueprint = true;
			if (!item.MoveToContainer(base.inventory, 0, true, false, null, true))
			{
				item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
			this.creatingBlueprint = false;
			if (this.experimentSuccessEffect.isValid)
			{
				Effect.server.Run(this.experimentSuccessEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
			}
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		this.pendingBlueprint = null;
		base.inventory.SetLocked(false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x000A1370 File Offset: 0x0009F570
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (base.inventory != null)
		{
			base.inventory.SetLocked(false);
		}
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x00088080 File Offset: 0x00086280
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.ItemFilter);
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x000A1396 File Offset: 0x0009F596
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		return (targetSlot == 1 && item.info == this.experimentResource) || (targetSlot == 0 && this.creatingBlueprint);
	}

	// Token: 0x0600145E RID: 5214 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x04000CEA RID: 3306
	public const int blueprintSlot = 0;

	// Token: 0x04000CEB RID: 3307
	public const int experimentSlot = 1;

	// Token: 0x04000CEC RID: 3308
	public bool Static;

	// Token: 0x04000CED RID: 3309
	public int Workbenchlevel;

	// Token: 0x04000CEE RID: 3310
	public LootSpawn experimentalItems;

	// Token: 0x04000CEF RID: 3311
	public GameObjectRef experimentStartEffect;

	// Token: 0x04000CF0 RID: 3312
	public GameObjectRef experimentSuccessEffect;

	// Token: 0x04000CF1 RID: 3313
	public ItemDefinition experimentResource;

	// Token: 0x04000CF2 RID: 3314
	public TechTreeData techTree;

	// Token: 0x04000CF3 RID: 3315
	public bool supportsIndustrialCrafter;

	// Token: 0x04000CF4 RID: 3316
	public static ItemDefinition blueprintBaseDef;

	// Token: 0x04000CF5 RID: 3317
	private ItemDefinition pendingBlueprint;

	// Token: 0x04000CF6 RID: 3318
	private bool creatingBlueprint;
}
