using System;
using UnityEngine;

// Token: 0x0200041E RID: 1054
public class PlayerBelt
{
	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x0600230C RID: 8972 RVA: 0x000DF37C File Offset: 0x000DD57C
	public static int MaxBeltSlots
	{
		get
		{
			return 6;
		}
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x000DF37F File Offset: 0x000DD57F
	public PlayerBelt(BasePlayer player)
	{
		this.player = player;
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x000DF390 File Offset: 0x000DD590
	public void DropActive(Vector3 position, Vector3 velocity)
	{
		Item activeItem = this.player.GetActiveItem();
		if (activeItem == null)
		{
			return;
		}
		using (TimeWarning.New("PlayerBelt.DropActive", 0))
		{
			activeItem.Drop(position, velocity, default(Quaternion));
			this.player.svActiveItemID = 0U;
			this.player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000DF400 File Offset: 0x000DD600
	public Item GetItemInSlot(int slot)
	{
		if (this.player == null)
		{
			return null;
		}
		if (this.player.inventory == null)
		{
			return null;
		}
		if (this.player.inventory.containerBelt == null)
		{
			return null;
		}
		return this.player.inventory.containerBelt.GetSlot(slot);
	}

	// Token: 0x04001B94 RID: 7060
	public static int SelectedSlot = -1;

	// Token: 0x04001B95 RID: 7061
	protected BasePlayer player;
}
