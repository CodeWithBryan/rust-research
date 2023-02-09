using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008E RID: 142
public class Locker : StorageContainer
{
	// Token: 0x06000D19 RID: 3353 RVA: 0x0006E9F8 File Offset: 0x0006CBF8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Locker.OnRpcMessage", 0))
		{
			if (rpc == 1799659668U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Equip ");
				}
				using (TimeWarning.New("RPC_Equip", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1799659668U, "RPC_Equip", this, player, 3f))
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
							this.RPC_Equip(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Equip");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x00020A80 File Offset: 0x0001EC80
	public bool IsEquipping()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x0006EB60 File Offset: 0x0006CD60
	private Locker.RowType GetRowType(int slot)
	{
		if (slot == -1)
		{
			return Locker.RowType.Clothing;
		}
		if (slot % 13 >= 7)
		{
			return Locker.RowType.Belt;
		}
		return Locker.RowType.Clothing;
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x0006EB72 File Offset: 0x0006CD72
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x0006EB88 File Offset: 0x0006CD88
	public void ClearEquipping()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x0006EB98 File Offset: 0x0006CD98
	public override bool ItemFilter(Item item, int targetSlot)
	{
		return base.ItemFilter(item, targetSlot) && (item.info.category == ItemCategory.Attire || this.GetRowType(targetSlot) == Locker.RowType.Belt);
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x0006EBC0 File Offset: 0x0006CDC0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Equip(BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		if (num < 0 || num >= 3)
		{
			return;
		}
		if (this.IsEquipping())
		{
			return;
		}
		BasePlayer player = msg.player;
		int num2 = num * 13;
		bool flag = false;
		for (int i = 0; i < player.inventory.containerWear.capacity; i++)
		{
			Item slot = player.inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				slot.RemoveFromContainer();
				this.clothingBuffer[i] = slot;
			}
		}
		for (int j = 0; j < 7; j++)
		{
			int num3 = num2 + j;
			Item slot2 = base.inventory.GetSlot(num3);
			Item item = this.clothingBuffer[j];
			if (slot2 != null)
			{
				flag = true;
				if (slot2.info.category != ItemCategory.Attire || !slot2.MoveToContainer(player.inventory.containerWear, j, true, false, null, true))
				{
					slot2.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
			if (item != null)
			{
				flag = true;
				if (item.info.category != ItemCategory.Attire || !item.MoveToContainer(base.inventory, num3, true, false, null, true))
				{
					item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
			this.clothingBuffer[j] = null;
		}
		for (int k = 0; k < 6; k++)
		{
			int num4 = num2 + k + 7;
			int iTargetPos = k;
			Item slot3 = base.inventory.GetSlot(num4);
			Item slot4 = player.inventory.containerBelt.GetSlot(k);
			if (slot4 != null)
			{
				slot4.RemoveFromContainer();
			}
			if (slot3 != null)
			{
				flag = true;
				if (!slot3.MoveToContainer(player.inventory.containerBelt, iTargetPos, true, false, null, true))
				{
					slot3.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
			if (slot4 != null)
			{
				flag = true;
				if (!slot4.MoveToContainer(base.inventory, num4, true, false, null, true))
				{
					slot4.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
		}
		if (flag)
		{
			Effect.server.Run(this.equipSound.resourcePath, player, StringPool.Get("spine3"), Vector3.zero, Vector3.zero, null, false);
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
			base.Invoke(new Action(this.ClearEquipping), 1.5f);
		}
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x0006EE30 File Offset: 0x0006D030
	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		int i = 0;
		while (i < this.inventorySlots)
		{
			Locker.RowType rowType = this.GetRowType(i);
			if (item.info.category == ItemCategory.Attire)
			{
				if (rowType == Locker.RowType.Clothing)
				{
					goto IL_23;
				}
			}
			else if (rowType == Locker.RowType.Belt)
			{
				goto IL_23;
			}
			IL_41:
			i++;
			continue;
			IL_23:
			if (!base.inventory.SlotTaken(item, i) && (rowType != Locker.RowType.Clothing || !this.DoesWearableConflictWithRow(item, i)))
			{
				return i;
			}
			goto IL_41;
		}
		return int.MinValue;
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x0006EE90 File Offset: 0x0006D090
	private bool DoesWearableConflictWithRow(Item item, int pos)
	{
		int num = pos / 13 * 13;
		ItemModWearable itemModWearable = item.info.ItemModWearable;
		if (itemModWearable == null)
		{
			return false;
		}
		for (int i = num; i < num + 7; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				ItemModWearable itemModWearable2 = slot.info.ItemModWearable;
				if (!(itemModWearable2 == null) && !itemModWearable2.CanExistWith(itemModWearable))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x0006EEFE File Offset: 0x0006D0FE
	public Vector2i GetIndustrialSlotRange(Vector3 localPosition)
	{
		if (localPosition.x < -0.3f)
		{
			return new Vector2i(26, 38);
		}
		if (localPosition.x > 0.3f)
		{
			return new Vector2i(0, 12);
		}
		return new Vector2i(13, 25);
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x0006EF36 File Offset: 0x0006D136
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !base.HasAttachedStorageAdaptor();
	}

	// Token: 0x04000856 RID: 2134
	public GameObjectRef equipSound;

	// Token: 0x04000857 RID: 2135
	private const int maxGearSets = 3;

	// Token: 0x04000858 RID: 2136
	private const int attireSize = 7;

	// Token: 0x04000859 RID: 2137
	private const int beltSize = 6;

	// Token: 0x0400085A RID: 2138
	private const int columnSize = 2;

	// Token: 0x0400085B RID: 2139
	private Item[] clothingBuffer = new Item[7];

	// Token: 0x0400085C RID: 2140
	private const int setSize = 13;

	// Token: 0x02000B95 RID: 2965
	private enum RowType
	{
		// Token: 0x04003ED7 RID: 16087
		Clothing,
		// Token: 0x04003ED8 RID: 16088
		Belt
	}

	// Token: 0x02000B96 RID: 2966
	public static class LockerFlags
	{
		// Token: 0x04003ED9 RID: 16089
		public const BaseEntity.Flags IsEquipping = BaseEntity.Flags.Reserved1;
	}
}
