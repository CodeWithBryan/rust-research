using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000091 RID: 145
public class Mailbox : StorageContainer
{
	// Token: 0x06000D5B RID: 3419 RVA: 0x000705D4 File Offset: 0x0006E7D4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Mailbox.OnRpcMessage", 0))
		{
			if (rpc == 131727457U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Submit ");
				}
				using (TimeWarning.New("RPC_Submit", 0))
				{
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
							this.RPC_Submit(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Submit");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000D5C RID: 3420 RVA: 0x000706F8 File Offset: 0x0006E8F8
	public int mailInputSlot
	{
		get
		{
			return this.inventorySlots - 1;
		}
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x00070702 File Offset: 0x0006E902
	public virtual bool PlayerIsOwner(BasePlayer player)
	{
		return player.CanBuild();
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x0007070A File Offset: 0x0006E90A
	public bool IsFull()
	{
		return this.shouldMarkAsFull && base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x00070721 File Offset: 0x0006E921
	public void MarkFull(bool full)
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, this.shouldMarkAsFull && full, false, true);
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x00070738 File Offset: 0x0006E938
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		return base.PlayerOpenLoot(player, this.PlayerIsOwner(player) ? this.ownerPanel : panelToOpen, true);
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x00070754 File Offset: 0x0006E954
	public override bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		if (panelName == this.ownerPanel)
		{
			return this.PlayerIsOwner(player) && base.CanOpenLootPanel(player, panelName);
		}
		return this.HasFreeSpace() || !this.shouldMarkAsFull;
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x0007078B File Offset: 0x0006E98B
	private bool HasFreeSpace()
	{
		return this.GetFreeSlot() != -1;
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x0007079C File Offset: 0x0006E99C
	private int GetFreeSlot()
	{
		for (int i = 0; i < this.mailInputSlot; i++)
		{
			if (base.inventory.GetSlot(i) == null)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x000707CB File Offset: 0x0006E9CB
	public virtual bool MoveItemToStorage(Item item)
	{
		item.RemoveFromContainer();
		return item.MoveToContainer(base.inventory, -1, true, false, null, true);
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x000707EC File Offset: 0x0006E9EC
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		if (this.autoSubmitWhenClosed)
		{
			this.SubmitInputItems(player);
		}
		if (this.IsFull())
		{
			Item slot = base.inventory.GetSlot(this.mailInputSlot);
			if (slot != null)
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
		}
		base.PlayerStoppedLooting(player);
		if (this.PlayerIsOwner(player))
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x0007085C File Offset: 0x0006EA5C
	[BaseEntity.RPC_Server]
	public void RPC_Submit(BaseEntity.RPCMessage msg)
	{
		if (this.IsFull())
		{
			return;
		}
		BasePlayer player = msg.player;
		this.SubmitInputItems(player);
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x00070880 File Offset: 0x0006EA80
	public void SubmitInputItems(BasePlayer fromPlayer)
	{
		Item slot = base.inventory.GetSlot(this.mailInputSlot);
		if (this.IsFull())
		{
			return;
		}
		if (slot != null)
		{
			if (this.MoveItemToStorage(slot))
			{
				if (slot.position != this.mailInputSlot)
				{
					Effect.server.Run(this.mailDropSound.resourcePath, this.GetDropPosition(), default(Vector3), null, false);
					if (fromPlayer != null && !this.PlayerIsOwner(fromPlayer))
					{
						base.SetFlag(BaseEntity.Flags.On, true, false, true);
						return;
					}
				}
			}
			else
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
		}
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x0007091C File Offset: 0x0006EB1C
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		this.MarkFull(!this.HasFreeSpace());
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x00070938 File Offset: 0x0006EB38
	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		bool flag = this.PlayerIsOwner(player);
		if (!flag)
		{
			flag = (item == base.inventory.GetSlot(this.mailInputSlot));
		}
		return flag && base.CanMoveFrom(player, item);
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x00070974 File Offset: 0x0006EB74
	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (this.allowedItems == null || this.allowedItems.Length == 0)
		{
			return base.ItemFilter(item, targetSlot);
		}
		foreach (ItemDefinition y in this.allowedItems)
		{
			if (item.info == y)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x000709C5 File Offset: 0x0006EBC5
	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		if (player == null || this.PlayerIsOwner(player))
		{
			return -1;
		}
		return this.mailInputSlot;
	}

	// Token: 0x040008A0 RID: 2208
	public string ownerPanel;

	// Token: 0x040008A1 RID: 2209
	public GameObjectRef mailDropSound;

	// Token: 0x040008A2 RID: 2210
	public ItemDefinition[] allowedItems;

	// Token: 0x040008A3 RID: 2211
	public bool autoSubmitWhenClosed;

	// Token: 0x040008A4 RID: 2212
	public bool shouldMarkAsFull;
}
