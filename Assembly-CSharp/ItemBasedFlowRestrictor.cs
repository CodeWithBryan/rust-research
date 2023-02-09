using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000084 RID: 132
public class ItemBasedFlowRestrictor : global::IOEntity, IContainerSounds
{
	// Token: 0x06000C88 RID: 3208 RVA: 0x0006B200 File Offset: 0x00069400
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ItemBasedFlowRestrictor.OnRpcMessage", 0))
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

	// Token: 0x06000C89 RID: 3209 RVA: 0x0006B368 File Offset: 0x00069568
	public override void ResetIOState()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (this.inventory != null)
		{
			global::Item slot = this.inventory.GetSlot(0);
			if (slot != null)
			{
				slot.Drop(this.debugOrigin.transform.position + base.transform.forward * 0.5f, this.GetInheritedDropVelocity() + base.transform.forward * 2f, default(Quaternion));
			}
		}
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x0006B3F1 File Offset: 0x000695F1
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.HasFlag(global::BaseEntity.Flags.Reserved1))
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x0006B40C File Offset: 0x0006960C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		base.SetFlag(global::BaseEntity.Flags.On, this.IsPowered(), false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this.HasPassthroughItem(), false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, base.IsOn() && !base.HasFlag(global::BaseEntity.Flags.Reserved1), false, true);
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x0006B46C File Offset: 0x0006966C
	public virtual bool HasPassthroughItem()
	{
		if (this.inventory.itemList.Count <= 0)
		{
			return false;
		}
		global::Item slot = this.inventory.GetSlot(0);
		return slot != null && (this.passthroughItemConditionLossPerSec <= 0f || !slot.hasCondition || slot.conditionNormalized > 0f) && slot.info == this.passthroughItem;
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x0006B4DC File Offset: 0x000696DC
	public virtual void TickPassthroughItem()
	{
		if (this.inventory.itemList.Count <= 0)
		{
			return;
		}
		if (!base.HasFlag(global::BaseEntity.Flags.On))
		{
			return;
		}
		global::Item slot = this.inventory.GetSlot(0);
		if (slot != null && slot.hasCondition)
		{
			slot.LoseCondition(1f);
		}
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0006B52C File Offset: 0x0006972C
	public override void ServerInit()
	{
		if (this.inventory == null)
		{
			this.CreateInventory(true);
			this.OnInventoryFirstCreated(this.inventory);
		}
		base.InvokeRandomized(new Action(this.TickPassthroughItem), 1f, 1f, 0.015f);
		base.ServerInit();
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x0006B57C File Offset: 0x0006977C
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x0006B58C File Offset: 0x0006978C
	public void CreateInventory(bool giveUID)
	{
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.allowedContents = ((this.allowedContents == (global::ItemContainer.ContentsType)0) ? global::ItemContainer.ContentsType.Generic : this.allowedContents);
		this.inventory.SetOnlyAllowedItem(this.passthroughItem);
		this.inventory.maxStackSize = this.maxStackSize;
		this.inventory.ServerInitialize(null, this.numSlots);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		this.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x0006B628 File Offset: 0x00069828
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

	// Token: 0x06000C92 RID: 3218 RVA: 0x0006B690 File Offset: 0x00069890
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.storageBox != null)
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

	// Token: 0x06000C93 RID: 3219 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnInventoryFirstCreated(global::ItemContainer container)
	{
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x0006B6FB File Offset: 0x000698FB
	public virtual void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this.HasPassthroughItem(), false, true);
		this.MarkDirty();
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x0006B718 File Offset: 0x00069918
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
		if (player.inventory.loot.StartLootingEntity(this, true))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			player.inventory.loot.AddContainer(this.inventory);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", this.lootPanelName);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x000059DD File Offset: 0x00003BDD
	public void PlayerStoppedLooting(global::BasePlayer player)
	{
	}

	// Token: 0x040007F7 RID: 2039
	public ItemDefinition passthroughItem;

	// Token: 0x040007F8 RID: 2040
	public global::ItemContainer.ContentsType allowedContents = global::ItemContainer.ContentsType.Generic;

	// Token: 0x040007F9 RID: 2041
	public int maxStackSize = 1;

	// Token: 0x040007FA RID: 2042
	public int numSlots;

	// Token: 0x040007FB RID: 2043
	public string lootPanelName = "generic";

	// Token: 0x040007FC RID: 2044
	public const global::BaseEntity.Flags HasPassthrough = global::BaseEntity.Flags.Reserved1;

	// Token: 0x040007FD RID: 2045
	public const global::BaseEntity.Flags Sparks = global::BaseEntity.Flags.Reserved2;

	// Token: 0x040007FE RID: 2046
	public float passthroughItemConditionLossPerSec = 1f;

	// Token: 0x040007FF RID: 2047
	public SoundDefinition openSound;

	// Token: 0x04000800 RID: 2048
	public SoundDefinition closeSound;

	// Token: 0x04000801 RID: 2049
	private global::ItemContainer inventory;
}
