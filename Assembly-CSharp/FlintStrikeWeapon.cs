using System;

// Token: 0x0200038E RID: 910
public class FlintStrikeWeapon : BaseProjectile
{
	// Token: 0x06001FCA RID: 8138 RVA: 0x000D17CF File Offset: 0x000CF9CF
	public override RecoilProperties GetRecoil()
	{
		return this.strikeRecoil;
	}

	// Token: 0x040018FF RID: 6399
	public float successFraction = 0.5f;

	// Token: 0x04001900 RID: 6400
	public RecoilProperties strikeRecoil;
}
