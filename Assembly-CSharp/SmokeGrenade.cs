using System;
using System.Collections.Generic;
using Rust.Ai;
using UnityEngine;

// Token: 0x020003FF RID: 1023
public class SmokeGrenade : TimedExplosive
{
	// Token: 0x06002278 RID: 8824 RVA: 0x000DCDDB File Offset: 0x000DAFDB
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.CheckForWater), 1f, 1f);
	}

	// Token: 0x06002279 RID: 8825 RVA: 0x000DCE00 File Offset: 0x000DB000
	public override void Explode()
	{
		if (this.WaterFactor() >= 0.5f)
		{
			this.FinishUp();
			return;
		}
		if (base.IsOn())
		{
			return;
		}
		base.Invoke(new Action(this.FinishUp), this.smokeDuration);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		base.InvalidateNetworkCache();
		base.SendNetworkUpdateImmediate(false);
		SmokeGrenade.activeGrenades.Add(this);
		if (this.creatorEntity)
		{
			Sense.Stimulate(new Sensation
			{
				Type = SensationType.Explosion,
				Position = this.creatorEntity.transform.position,
				Radius = this.explosionRadius * 17f,
				DamagePotential = 0f,
				InitiatorPlayer = (this.creatorEntity as BasePlayer),
				Initiator = this.creatorEntity
			});
		}
	}

	// Token: 0x0600227A RID: 8826 RVA: 0x000DCEE6 File Offset: 0x000DB0E6
	public void CheckForWater()
	{
		if (this.WaterFactor() >= 0.5f)
		{
			this.FinishUp();
		}
	}

	// Token: 0x0600227B RID: 8827 RVA: 0x000DCEFB File Offset: 0x000DB0FB
	public void FinishUp()
	{
		if (this.killing)
		{
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
		this.killing = true;
	}

	// Token: 0x0600227C RID: 8828 RVA: 0x000DCF14 File Offset: 0x000DB114
	public override void DestroyShared()
	{
		SmokeGrenade.activeGrenades.Remove(this);
		base.DestroyShared();
	}

	// Token: 0x04001AF7 RID: 6903
	public float smokeDuration = 45f;

	// Token: 0x04001AF8 RID: 6904
	public GameObjectRef smokeEffectPrefab;

	// Token: 0x04001AF9 RID: 6905
	public GameObjectRef igniteSound;

	// Token: 0x04001AFA RID: 6906
	public SoundPlayer soundLoop;

	// Token: 0x04001AFB RID: 6907
	private GameObject smokeEffectInstance;

	// Token: 0x04001AFC RID: 6908
	public static List<SmokeGrenade> activeGrenades = new List<SmokeGrenade>();

	// Token: 0x04001AFD RID: 6909
	public float fieldMin = 5f;

	// Token: 0x04001AFE RID: 6910
	public float fieldMax = 8f;

	// Token: 0x04001AFF RID: 6911
	protected bool killing;
}
