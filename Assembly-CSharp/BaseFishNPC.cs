using System;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class BaseFishNPC : BaseNpc, IAIAttack, IAISenses, IThinker
{
	// Token: 0x06001A59 RID: 6745 RVA: 0x000BACB4 File Offset: 0x000B8EB4
	public override void ServerInit()
	{
		base.ServerInit();
		this.brain = base.GetComponent<FishBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.AddAnimal(this);
	}

	// Token: 0x06001A5A RID: 6746 RVA: 0x000BA9F2 File Offset: 0x000B8BF2
	internal override void DoServerDestroy()
	{
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.RemoveAnimal(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001A5B RID: 6747 RVA: 0x000BACD7 File Offset: 0x000B8ED7
	public virtual void TryThink()
	{
		if (this.brain.ShouldServerThink())
		{
			this.brain.DoThink();
		}
	}

	// Token: 0x06001A5C RID: 6748 RVA: 0x000BACF4 File Offset: 0x000B8EF4
	public bool CanAttack(BaseEntity entity)
	{
		float num;
		return !this.IsOnCooldown() && this.IsTargetInRange(entity, out num) && this.CanSeeTarget(entity);
	}

	// Token: 0x06001A5D RID: 6749 RVA: 0x00007074 File Offset: 0x00005274
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06001A5E RID: 6750 RVA: 0x000BAD24 File Offset: 0x000B8F24
	public float EngagementRange()
	{
		return this.AttackRange * this.brain.AttackRangeMultiplier;
	}

	// Token: 0x06001A5F RID: 6751 RVA: 0x000BAD38 File Offset: 0x000B8F38
	public bool IsTargetInRange(BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.AttackPosition);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x00060F9C File Offset: 0x0005F19C
	public bool CanSeeTarget(BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x000BAD60 File Offset: 0x000B8F60
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

	// Token: 0x06001A63 RID: 6755 RVA: 0x000059DD File Offset: 0x00003BDD
	public void StopAttacking()
	{
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x000338E5 File Offset: 0x00031AE5
	public float CooldownDuration()
	{
		return this.AttackRate;
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x000BABAB File Offset: 0x000B8DAB
	public bool IsOnCooldown()
	{
		return !base.AttackReady();
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x000BAD88 File Offset: 0x000B8F88
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

	// Token: 0x06001A67 RID: 6759 RVA: 0x000BADF0 File Offset: 0x000B8FF0
	public bool IsTarget(BaseEntity entity)
	{
		BaseNpc baseNpc = entity as BaseNpc;
		return (!(baseNpc != null) || baseNpc.Stats.Family != this.Stats.Family) && !this.IsThreat(entity);
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x000B6150 File Offset: 0x000B4350
	public bool IsFriendly(BaseEntity entity)
	{
		return !(entity == null) && entity.prefabID == this.prefabID;
	}

	// Token: 0x06001A69 RID: 6761 RVA: 0x000062DD File Offset: 0x000044DD
	public float GetAmmoFraction()
	{
		return 1f;
	}

	// Token: 0x06001A6A RID: 6762 RVA: 0x0002A0CF File Offset: 0x000282CF
	public BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06001A6B RID: 6763 RVA: 0x000059DD File Offset: 0x00003BDD
	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
	}

	// Token: 0x0400129C RID: 4764
	protected FishBrain brain;
}
