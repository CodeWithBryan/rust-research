using System;

// Token: 0x0200017C RID: 380
public class SnowballGun : BaseProjectile
{
	// Token: 0x060016D2 RID: 5842 RVA: 0x000AC13C File Offset: 0x000AA33C
	protected override void ReloadMagazine(int desiredAmount = -1)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		desiredAmount = 1;
		this.primaryMagazine.Reload(ownerPlayer, desiredAmount, this.CanRefundAmmo);
		this.primaryMagazine.contents = this.primaryMagazine.capacity;
		this.primaryMagazine.ammoType = this.OverrideProjectile;
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x060016D3 RID: 5843 RVA: 0x000AC1B3 File Offset: 0x000AA3B3
	protected override ItemDefinition PrimaryMagazineAmmo
	{
		get
		{
			if (!(this.OverrideProjectile != null))
			{
				return base.PrimaryMagazineAmmo;
			}
			return this.OverrideProjectile;
		}
	}

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x060016D4 RID: 5844 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool CanRefundAmmo
	{
		get
		{
			return false;
		}
	}

	// Token: 0x04000FDB RID: 4059
	public ItemDefinition OverrideProjectile;
}
