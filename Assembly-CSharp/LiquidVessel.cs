using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008C RID: 140
public class LiquidVessel : HeldEntity
{
	// Token: 0x06000CFD RID: 3325 RVA: 0x0006DD44 File Offset: 0x0006BF44
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LiquidVessel.OnRpcMessage", 0))
		{
			if (rpc == 4034725537U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoEmpty ");
				}
				using (TimeWarning.New("DoEmpty", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(4034725537U, "DoEmpty", this, player))
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
							this.DoEmpty(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoEmpty");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x0006DEA8 File Offset: 0x0006C0A8
	public bool CanDrink()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		if (!ownerPlayer.metabolism.CanConsume())
		{
			return false;
		}
		Item item = this.GetItem();
		return item != null && item.contents != null && item.contents.itemList != null && item.contents.itemList.Count != 0;
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x0006DF10 File Offset: 0x0006C110
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void DoEmpty(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		if (!msg.player.metabolism.CanConsume())
		{
			return;
		}
		using (List<Item>.Enumerator enumerator = item.contents.itemList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				enumerator.Current.UseItem(50);
			}
		}
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x0006DFA0 File Offset: 0x0006C1A0
	public void AddLiquid(ItemDefinition liquidType, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		Item item = this.GetItem();
		Item item2 = item.contents.GetSlot(0);
		ItemModContainer component = item.info.GetComponent<ItemModContainer>();
		if (item2 == null)
		{
			Item item3 = ItemManager.Create(liquidType, amount, 0UL);
			if (item3 != null)
			{
				item3.MoveToContainer(item.contents, -1, true, false, null, true);
				return;
			}
		}
		else
		{
			int num = Mathf.Clamp(item2.amount + amount, 0, component.maxStackSize);
			ItemDefinition itemDefinition = WaterResource.Merge(item2.info, liquidType);
			if (itemDefinition != item2.info)
			{
				item2.Remove(0f);
				item2 = ItemManager.Create(itemDefinition, num, 0UL);
				item2.MoveToContainer(item.contents, -1, true, false, null, true);
			}
			else
			{
				item2.amount = num;
			}
			item2.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x0006E06C File Offset: 0x0006C26C
	public bool CanFillHere(Vector3 pos)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		return ownerPlayer && (double)ownerPlayer.WaterFactor() > 0.05;
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x0006E0A0 File Offset: 0x0006C2A0
	public int AmountHeld()
	{
		Item slot = this.GetItem().contents.GetSlot(0);
		if (slot == null)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x0006E0CA File Offset: 0x0006C2CA
	public float HeldFraction()
	{
		return (float)this.AmountHeld() / (float)this.MaxHoldable();
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x0006E0DB File Offset: 0x0006C2DB
	public bool IsFull()
	{
		return this.HeldFraction() >= 1f;
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x0006E0ED File Offset: 0x0006C2ED
	public int MaxHoldable()
	{
		return this.GetItem().info.GetComponent<ItemModContainer>().maxStackSize;
	}
}
