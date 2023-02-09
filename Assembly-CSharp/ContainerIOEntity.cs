using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005C RID: 92
public class ContainerIOEntity : global::IOEntity, IItemContainerEntity, IIdealSlotEntity, LootPanel.IHasLootPanel, IContainerSounds
{
	// Token: 0x06000997 RID: 2455 RVA: 0x00059710 File Offset: 0x00057910
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ContainerIOEntity.OnRpcMessage", 0))
		{
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLoot ");
				}
				using (TimeWarning.New("RPC_OpenLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(331989034U, "RPC_OpenLoot", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenLoot(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06000998 RID: 2456 RVA: 0x00059878 File Offset: 0x00057A78
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.panelTitle;
		}
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000999 RID: 2457 RVA: 0x00059880 File Offset: 0x00057A80
	// (set) Token: 0x0600099A RID: 2458 RVA: 0x00059888 File Offset: 0x00057A88
	public global::ItemContainer inventory { get; private set; }

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x0600099B RID: 2459 RVA: 0x00059891 File Offset: 0x00057A91
	public Transform Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x0600099C RID: 2460 RVA: 0x00059899 File Offset: 0x00057A99
	public bool DropsLoot
	{
		get
		{
			return this.dropsLoot;
		}
	}

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x0600099D RID: 2461 RVA: 0x000598A1 File Offset: 0x00057AA1
	public bool DropFloats
	{
		get
		{
			return this.dropFloats;
		}
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x000598A9 File Offset: 0x00057AA9
	public override bool CanPickup(global::BasePlayer player)
	{
		return (!this.pickup.requireEmptyInv || this.inventory == null || this.inventory.itemList.Count == 0) && base.CanPickup(player);
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x000598DB File Offset: 0x00057ADB
	public override void ServerInit()
	{
		if (this.inventory == null)
		{
			this.CreateInventory(true);
			this.OnInventoryFirstCreated(this.inventory);
		}
		base.ServerInit();
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x000598FE File Offset: 0x00057AFE
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x0005990D File Offset: 0x00057B0D
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.inventory != null && this.inventory.uid == 0U)
		{
			this.inventory.GiveUID();
		}
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00059940 File Offset: 0x00057B40
	public void CreateInventory(bool giveUID)
	{
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.allowedContents = ((this.allowedContents == (global::ItemContainer.ContentsType)0) ? global::ItemContainer.ContentsType.Generic : this.allowedContents);
		this.inventory.SetOnlyAllowedItem(this.onlyAllowedItem);
		this.inventory.maxStackSize = this.maxStackSize;
		this.inventory.ServerInitialize(null, this.numSlots);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		this.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		this.inventory.onDirty += this.OnInventoryDirty;
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x000599F4 File Offset: 0x00057BF4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnInventoryFirstCreated(global::ItemContainer container)
	{
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnItemAddedOrRemoved(global::Item item, bool added)
	{
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnInventoryDirty()
	{
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x00059A59 File Offset: 0x00057C59
	public override void OnKilled(HitInfo info)
	{
		this.DropItems(null);
		base.OnKilled(info);
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x00059A69 File Offset: 0x00057C69
	public void DropItems(global::BaseEntity initiator = null)
	{
		StorageContainer.DropItems(this, initiator);
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x00059A74 File Offset: 0x00057C74
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage rpc)
	{
		if (this.inventory == null)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		this.PlayerOpenLoot(player, this.lootPanelName, true);
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x00059AB4 File Offset: 0x00057CB4
	public virtual bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (this.needsBuildingPrivilegeToUse && !player.CanBuild())
		{
			return false;
		}
		if (this.onlyOneUser && base.IsOpen())
		{
			player.ChatMessage("Already in use");
			return false;
		}
		if (panelToOpen == "")
		{
			panelToOpen = this.lootPanelName;
		}
		if (player.inventory.loot.StartLootingEntity(this, doPositionChecks))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			player.inventory.loot.AddContainer(this.inventory);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", this.lootPanelName);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return true;
		}
		return false;
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00059B66 File Offset: 0x00057D66
	public virtual void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00007074 File Offset: 0x00005274
	public bool ShouldDropItemsIndividually()
	{
		return false;
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x00040DA9 File Offset: 0x0003EFA9
	public virtual int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		return -1;
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x00007074 File Offset: 0x00005274
	public virtual uint GetIdealContainer(global::BasePlayer player, global::Item item)
	{
		return 0U;
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void DropBonusItems(global::BaseEntity initiator, global::ItemContainer container)
	{
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x00059B79 File Offset: 0x00057D79
	public bool OccupiedCheck(global::BasePlayer player = null)
	{
		return (player != null && player.inventory.loot.entitySource == this) || !this.onlyOneUser || !base.IsOpen();
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x00059BB4 File Offset: 0x00057DB4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				this.inventory.capacity = this.numSlots;
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x04000659 RID: 1625
	public ItemDefinition onlyAllowedItem;

	// Token: 0x0400065A RID: 1626
	public global::ItemContainer.ContentsType allowedContents = global::ItemContainer.ContentsType.Generic;

	// Token: 0x0400065B RID: 1627
	public int maxStackSize = 1;

	// Token: 0x0400065C RID: 1628
	public int numSlots;

	// Token: 0x0400065D RID: 1629
	public string lootPanelName = "generic";

	// Token: 0x0400065E RID: 1630
	public Translate.Phrase panelTitle = new Translate.Phrase("loot", "Loot");

	// Token: 0x0400065F RID: 1631
	public bool needsBuildingPrivilegeToUse;

	// Token: 0x04000660 RID: 1632
	public bool isLootable = true;

	// Token: 0x04000661 RID: 1633
	public bool dropsLoot;

	// Token: 0x04000662 RID: 1634
	public bool dropFloats;

	// Token: 0x04000663 RID: 1635
	public bool onlyOneUser;

	// Token: 0x04000664 RID: 1636
	public SoundDefinition openSound;

	// Token: 0x04000665 RID: 1637
	public SoundDefinition closeSound;
}
