using System;
using UnityEngine;

// Token: 0x020003E4 RID: 996
public class NPCAutoTurret : AutoTurret
{
	// Token: 0x060021BA RID: 8634 RVA: 0x000D8B20 File Offset: 0x000D6D20
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetOnline();
		base.SetPeacekeepermode(true);
	}

	// Token: 0x060021BB RID: 8635 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool HasAmmo()
	{
		return true;
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x00007074 File Offset: 0x00005274
	public override bool CheckPeekers()
	{
		return false;
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x000D8B35 File Offset: 0x000D6D35
	public override float TargetScanRate()
	{
		return 1.25f;
	}

	// Token: 0x060021BE RID: 8638 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return true;
	}

	// Token: 0x060021BF RID: 8639 RVA: 0x0006BB18 File Offset: 0x00069D18
	public override float GetMaxAngleForEngagement()
	{
		return 15f;
	}

	// Token: 0x060021C0 RID: 8640 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool HasFallbackWeapon()
	{
		return true;
	}

	// Token: 0x060021C1 RID: 8641 RVA: 0x000D8B3C File Offset: 0x000D6D3C
	public override Transform GetCenterMuzzle()
	{
		return this.centerMuzzle;
	}

	// Token: 0x060021C2 RID: 8642 RVA: 0x000D8B44 File Offset: 0x000D6D44
	public override void FireGun(Vector3 targetPos, float aimCone, Transform muzzleToUse = null, BaseCombatEntity target = null)
	{
		muzzleToUse = this.muzzleRight;
		base.FireGun(targetPos, aimCone, muzzleToUse, target);
	}

	// Token: 0x060021C3 RID: 8643 RVA: 0x000D8B59 File Offset: 0x000D6D59
	protected override bool Ignore(BasePlayer player)
	{
		return player is ScientistNPC || player is BanditGuard;
	}

	// Token: 0x060021C4 RID: 8644 RVA: 0x000D8B70 File Offset: 0x000D6D70
	public override bool IsEntityHostile(BaseCombatEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (basePlayer != null)
		{
			if (basePlayer.IsNpc)
			{
				return !(basePlayer is ScientistNPC) && !(basePlayer is BanditGuard) && !(basePlayer is NPCShopKeeper) && (!(basePlayer is BasePet) || base.IsEntityHostile(basePlayer));
			}
			if (basePlayer.IsSleeping() && basePlayer.secondsSleeping >= NPCAutoTurret.sleeperhostiledelay)
			{
				return true;
			}
		}
		return base.IsEntityHostile(ent);
	}

	// Token: 0x04001A1A RID: 6682
	public Transform centerMuzzle;

	// Token: 0x04001A1B RID: 6683
	public Transform muzzleLeft;

	// Token: 0x04001A1C RID: 6684
	public Transform muzzleRight;

	// Token: 0x04001A1D RID: 6685
	private bool useLeftMuzzle;

	// Token: 0x04001A1E RID: 6686
	[ServerVar(Help = "How many seconds until a sleeping player is considered hostile")]
	public static float sleeperhostiledelay = 1200f;
}
