using System;
using Network;

// Token: 0x020000C8 RID: 200
public class SlotMachineStorage : StorageContainer
{
	// Token: 0x060011A8 RID: 4520 RVA: 0x0008EF08 File Offset: 0x0008D108
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SlotMachineStorage.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060011A9 RID: 4521 RVA: 0x0008EF48 File Offset: 0x0008D148
	public bool IsPlayerValid(BasePlayer player)
	{
		return player.isMounted && !(player.GetMounted() != base.GetParentEntity());
	}

	// Token: 0x060011AA RID: 4522 RVA: 0x0008EF68 File Offset: 0x0008D168
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		return this.IsPlayerValid(player) && base.PlayerOpenLoot(player, panelToOpen, true);
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x0008EF80 File Offset: 0x0008D180
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		Item slot = base.inventory.GetSlot(0);
		this.UpdateAmount((slot != null) ? slot.amount : 0);
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x0008EFB2 File Offset: 0x0008D1B2
	public void UpdateAmount(int amount)
	{
		if (this.Amount == amount)
		{
			return;
		}
		this.Amount = amount;
		(base.GetParentEntity() as SlotMachine).OnBettingScrapUpdated(amount);
		base.ClientRPC<int>(null, "RPC_UpdateAmount", this.Amount);
	}

	// Token: 0x060011AD RID: 4525 RVA: 0x0008EFE8 File Offset: 0x0008D1E8
	public override bool CanBeLooted(BasePlayer player)
	{
		return this.IsPlayerValid(player) && base.CanBeLooted(player);
	}

	// Token: 0x04000B0D RID: 2829
	public int Amount;
}
