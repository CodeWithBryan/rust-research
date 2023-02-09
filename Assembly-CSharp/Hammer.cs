using System;

// Token: 0x02000391 RID: 913
public class Hammer : BaseMelee
{
	// Token: 0x06001FCE RID: 8142 RVA: 0x000D17F2 File Offset: 0x000CF9F2
	public override bool CanHit(HitTest info)
	{
		return !(info.HitEntity == null) && !(info.HitEntity is BasePlayer) && info.HitEntity is BaseCombatEntity;
	}

	// Token: 0x06001FCF RID: 8143 RVA: 0x000D1824 File Offset: 0x000CFA24
	public override void DoAttackShared(HitInfo info)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		BaseCombatEntity baseCombatEntity = info.HitEntity as BaseCombatEntity;
		if (baseCombatEntity != null && ownerPlayer != null && base.isServer)
		{
			using (TimeWarning.New("DoRepair", 50))
			{
				baseCombatEntity.DoRepair(ownerPlayer);
			}
		}
		info.DoDecals = false;
		if (base.isServer)
		{
			Effect.server.ImpactEffect(info);
			return;
		}
		Effect.client.ImpactEffect(info);
	}
}
