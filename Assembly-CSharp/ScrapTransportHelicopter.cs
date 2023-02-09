using System;
using Rust;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class ScrapTransportHelicopter : MiniCopter, TriggerHurtNotChild.IHurtTriggerUser
{
	// Token: 0x060000FD RID: 253 RVA: 0x00007316 File Offset: 0x00005516
	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		if (!base.isServer)
		{
			return;
		}
		base.Invoke(new Action(this.DelayedNetworking), 0.15f);
	}

	// Token: 0x060000FE RID: 254 RVA: 0x00007338 File Offset: 0x00005538
	public void DelayedNetworking()
	{
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060000FF RID: 255 RVA: 0x00007341 File Offset: 0x00005541
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
	}

	// Token: 0x06000100 RID: 256 RVA: 0x0000734C File Offset: 0x0000554C
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (GameInfo.HasAchievements && base.isServer && !old.HasFlag(BaseEntity.Flags.On) && next.HasFlag(BaseEntity.Flags.On) && base.GetDriver() != null)
		{
			int num = 0;
			foreach (BaseEntity baseEntity in this.children)
			{
				if (baseEntity.ToPlayer() != null)
				{
					num++;
				}
				BaseVehicleSeat baseVehicleSeat;
				if ((baseVehicleSeat = (baseEntity as BaseVehicleSeat)) != null && baseVehicleSeat.GetMounted() != null && baseVehicleSeat.GetMounted() != base.GetDriver())
				{
					num++;
				}
			}
			if (num >= 5)
			{
				base.GetDriver().GiveAchievement("RUST_AIR");
			}
		}
	}

	// Token: 0x06000101 RID: 257 RVA: 0x000066E9 File Offset: 0x000048E9
	public override int StartingFuelUnits()
	{
		return 100;
	}

	// Token: 0x06000102 RID: 258 RVA: 0x000062DD File Offset: 0x000044DD
	public float GetPlayerDamageMultiplier()
	{
		return 1f;
	}

	// Token: 0x06000103 RID: 259 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
	}

	// Token: 0x06000104 RID: 260 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool CanPushNow(BasePlayer pusher)
	{
		return false;
	}

	// Token: 0x0400013D RID: 317
	public Transform searchlightEye;

	// Token: 0x0400013E RID: 318
	public BoxCollider parentTriggerCollider;

	// Token: 0x0400013F RID: 319
	[Header("Damage Effects")]
	public ParticleSystemContainer tailDamageLight;

	// Token: 0x04000140 RID: 320
	public ParticleSystemContainer tailDamageHeavy;

	// Token: 0x04000141 RID: 321
	public ParticleSystemContainer mainEngineDamageLight;

	// Token: 0x04000142 RID: 322
	public ParticleSystemContainer mainEngineDamageHeavy;

	// Token: 0x04000143 RID: 323
	public ParticleSystemContainer cockpitSparks;

	// Token: 0x04000144 RID: 324
	public Transform tailDamageLightEffects;

	// Token: 0x04000145 RID: 325
	public Transform mainEngineDamageLightEffects;

	// Token: 0x04000146 RID: 326
	public SoundDefinition damagedFireSoundDef;

	// Token: 0x04000147 RID: 327
	public SoundDefinition damagedFireTailSoundDef;

	// Token: 0x04000148 RID: 328
	public SoundDefinition damagedSparksSoundDef;

	// Token: 0x04000149 RID: 329
	private Sound damagedFireSound;

	// Token: 0x0400014A RID: 330
	private Sound damagedFireTailSound;

	// Token: 0x0400014B RID: 331
	private Sound damagedSparksSound;

	// Token: 0x0400014C RID: 332
	public float pilotRotorScale = 1.5f;

	// Token: 0x0400014D RID: 333
	public float compassOffset;

	// Token: 0x0400014E RID: 334
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public new static float population;

	// Token: 0x0400014F RID: 335
	public const string PASSENGER_ACHIEVEMENT = "RUST_AIR";

	// Token: 0x04000150 RID: 336
	public const int PASSENGER_ACHIEVEMENT_REQ_COUNT = 5;
}
