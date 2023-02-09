using System;

// Token: 0x0200044D RID: 1101
public class KayakSeat : BaseVehicleSeat
{
	// Token: 0x0600241B RID: 9243 RVA: 0x000E3ED6 File Offset: 0x000E20D6
	public override void OnPlayerMounted()
	{
		base.OnPlayerMounted();
		if (this.VehicleParent() != null)
		{
			this.VehicleParent().OnPlayerMounted();
		}
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x000E3EF7 File Offset: 0x000E20F7
	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		if (this.VehicleParent() != null)
		{
			this.VehicleParent().OnPlayerDismounted(player);
		}
	}

	// Token: 0x04001CB2 RID: 7346
	public ItemDefinition PaddleItem;
}
