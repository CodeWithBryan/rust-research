using System;

// Token: 0x0200038D RID: 909
public class CrossbowWeapon : BaseProjectile
{
	// Token: 0x06001FC7 RID: 8135 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ForceSendMagazine(BaseNetworkable.SaveInfo saveInfo)
	{
		return true;
	}

	// Token: 0x06001FC8 RID: 8136 RVA: 0x000D17C6 File Offset: 0x000CF9C6
	public override void DidAttackServerside()
	{
		base.SendNetworkUpdateImmediate(false);
	}
}
