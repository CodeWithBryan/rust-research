using System;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class BaseAnimalNPC : BaseNpc, IAIAttack, IAITirednessAbove, IAISleep, IAIHungerAbove, IAISenses, IThinker
{
	// Token: 0x06001A3E RID: 6718 RVA: 0x000BA9CF File Offset: 0x000B8BCF
	public override void ServerInit()
	{
		base.ServerInit();
		this.brain = base.GetComponent<AnimalBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.AddAnimal(this);
	}

	// Token: 0x06001A3F RID: 6719 RVA: 0x000BA9F2 File Offset: 0x000B8BF2
	internal override void DoServerDestroy()
	{
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.RemoveAnimal(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001A40 RID: 6720 RVA: 0x000BAA09 File Offset: 0x000B8C09
	public virtual void TryThink()
	{
		if (this.brain.ShouldServerThink())
		{
			this.brain.DoThink();
		}
	}

	// Token: 0x06001A41 RID: 6721 RVA: 0x000BAA24 File Offset: 0x000B8C24
	public override void OnKilled(HitInfo hitInfo = null)
	{
		if (hitInfo != null)
		{
			BasePlayer initiatorPlayer = hitInfo.InitiatorPlayer;
			if (initiatorPlayer != null)
			{
				initiatorPlayer.GiveAchievement("KILL_ANIMAL");
				if (!string.IsNullOrEmpty(this.deathStatName))
				{
					initiatorPlayer.stats.Add(this.deathStatName, 1, (Stats)5);
					initiatorPlayer.stats.Save(false);
				}
				initiatorPlayer.LifeStoryKill(this);
			}
		}
		base.OnKilled(null);
	}

	// Token: 0x06001A42 RID: 6722 RVA: 0x000BAA89 File Offset: 0x000B8C89
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (base.isServer && info.InitiatorPlayer && !info.damageTypes.IsMeleeType())
		{
			info.InitiatorPlayer.LifeStoryShotHit(info.Weapon);
		}
	}

	// Token: 0x06001A43 RID: 6723 RVA: 0x000BAAC5 File Offset: 0x000B8CC5
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001A44 RID: 6724 RVA: 0x000BAAD4 File Offset: 0x000B8CD4
	public bool CanAttack(BaseEntity entity)
	{
		if (entity == null)
		{
			return false;
		}
		if (this.NeedsToReload())
		{
			return false;
		}
		if (this.IsOnCooldown())
		{
			return false;
		}
		float num;
		if (!this.IsTargetInRange(entity, out num))
		{
			return false;
		}
		if (!this.CanSeeTarget(entity))
		{
			return false;
		}
		BasePlayer basePlayer = entity as BasePlayer;
		BaseVehicle baseVehicle = (basePlayer != null) ? basePlayer.GetMountedVehicle() : null;
		return !(baseVehicle != null) || !(baseVehicle is BaseModularVehicle);
	}

	// Token: 0x06001A45 RID: 6725 RVA: 0x00007074 File Offset: 0x00005274
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06001A46 RID: 6726 RVA: 0x000BAB46 File Offset: 0x000B8D46
	public float EngagementRange()
	{
		return this.AttackRange * this.brain.AttackRangeMultiplier;
	}

	// Token: 0x06001A47 RID: 6727 RVA: 0x000BAB5A File Offset: 0x000B8D5A
	public bool IsTargetInRange(BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.AttackPosition);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06001A48 RID: 6728 RVA: 0x00060F9C File Offset: 0x0005F19C
	public bool CanSeeTarget(BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x06001A49 RID: 6729 RVA: 0x00027647 File Offset: 0x00025847
	public bool Reload()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001A4A RID: 6730 RVA: 0x000BAB84 File Offset: 0x000B8D84
	public bool StartAttacking(BaseEntity target)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		if (baseCombatEntity == null)
		{
			return false;
		}
		base.Attack(baseCombatEntity);
		return true;
	}

	// Token: 0x06001A4B RID: 6731 RVA: 0x000059DD File Offset: 0x00003BDD
	public void StopAttacking()
	{
	}

	// Token: 0x06001A4C RID: 6732 RVA: 0x000338E5 File Offset: 0x00031AE5
	public float CooldownDuration()
	{
		return this.AttackRate;
	}

	// Token: 0x06001A4D RID: 6733 RVA: 0x000BABAB File Offset: 0x000B8DAB
	public bool IsOnCooldown()
	{
		return !base.AttackReady();
	}

	// Token: 0x06001A4E RID: 6734 RVA: 0x000BABB6 File Offset: 0x000B8DB6
	public bool IsTirednessAbove(float value)
	{
		return 1f - this.Sleep > value;
	}

	// Token: 0x06001A4F RID: 6735 RVA: 0x000BABC7 File Offset: 0x000B8DC7
	public void StartSleeping()
	{
		base.SetFact(BaseNpc.Facts.IsSleeping, 1, true, true);
	}

	// Token: 0x06001A50 RID: 6736 RVA: 0x000BABD3 File Offset: 0x000B8DD3
	public void StopSleeping()
	{
		base.SetFact(BaseNpc.Facts.IsSleeping, 0, true, true);
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x000BABDF File Offset: 0x000B8DDF
	public bool IsHungerAbove(float value)
	{
		return 1f - this.Energy.Level > value;
	}

	// Token: 0x06001A52 RID: 6738 RVA: 0x000BABF8 File Offset: 0x000B8DF8
	public bool IsThreat(BaseEntity entity)
	{
		BaseNpc baseNpc = entity as BaseNpc;
		if (baseNpc != null)
		{
			return baseNpc.Stats.Family != this.Stats.Family && base.IsAfraidOf(baseNpc.Stats.Family);
		}
		BasePlayer basePlayer = entity as BasePlayer;
		return basePlayer != null && base.IsAfraidOf(basePlayer.Family);
	}

	// Token: 0x06001A53 RID: 6739 RVA: 0x000BAC60 File Offset: 0x000B8E60
	public bool IsTarget(BaseEntity entity)
	{
		BaseNpc baseNpc = entity as BaseNpc;
		return (!(baseNpc != null) || baseNpc.Stats.Family != this.Stats.Family) && !this.IsThreat(entity);
	}

	// Token: 0x06001A54 RID: 6740 RVA: 0x000B6150 File Offset: 0x000B4350
	public bool IsFriendly(BaseEntity entity)
	{
		return !(entity == null) && entity.prefabID == this.prefabID;
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x000062DD File Offset: 0x000044DD
	public float GetAmmoFraction()
	{
		return 1f;
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x0002A0CF File Offset: 0x000282CF
	public BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06001A57 RID: 6743 RVA: 0x000059DD File Offset: 0x00003BDD
	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
	}

	// Token: 0x0400129A RID: 4762
	public string deathStatName = "";

	// Token: 0x0400129B RID: 4763
	protected AnimalBrain brain;
}
