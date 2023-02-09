using System;
using UnityEngine;

// Token: 0x02000397 RID: 919
public class Speargun : CrossbowWeapon
{
	// Token: 0x06001FD7 RID: 8151 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ForceSendMagazine(BaseNetworkable.SaveInfo saveInfo)
	{
		return true;
	}

	// Token: 0x06001FD8 RID: 8152 RVA: 0x000D1910 File Offset: 0x000CFB10
	protected override bool VerifyClientAttack(BasePlayer player)
	{
		return player.WaterFactor() >= 1f && base.VerifyClientAttack(player);
	}

	// Token: 0x06001FD9 RID: 8153 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool CanBeUsedInWater()
	{
		return true;
	}

	// Token: 0x04001908 RID: 6408
	public GameObject worldAmmoModel;
}
