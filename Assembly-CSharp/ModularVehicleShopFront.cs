using System;
using UnityEngine;

// Token: 0x02000479 RID: 1145
public class ModularVehicleShopFront : ShopFront
{
	// Token: 0x06002553 RID: 9555 RVA: 0x000E9A6E File Offset: 0x000E7C6E
	public override bool CanBeLooted(BasePlayer player)
	{
		return this.WithinUseDistance(player) && base.CanBeLooted(player);
	}

	// Token: 0x06002554 RID: 9556 RVA: 0x000E9A82 File Offset: 0x000E7C82
	private bool WithinUseDistance(BasePlayer player)
	{
		return base.Distance(player.eyes.position) <= this.maxUseDistance;
	}

	// Token: 0x04001DE8 RID: 7656
	[SerializeField]
	private float maxUseDistance = 1.5f;
}
