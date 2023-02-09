using System;

// Token: 0x02000372 RID: 882
public interface IAIAttack
{
	// Token: 0x06001F06 RID: 7942
	void AttackTick(float delta, BaseEntity target, bool targetIsLOS);

	// Token: 0x06001F07 RID: 7943
	BaseEntity GetBestTarget();

	// Token: 0x06001F08 RID: 7944
	bool CanAttack(BaseEntity entity);

	// Token: 0x06001F09 RID: 7945
	float EngagementRange();

	// Token: 0x06001F0A RID: 7946
	bool IsTargetInRange(BaseEntity entity, out float dist);

	// Token: 0x06001F0B RID: 7947
	bool CanSeeTarget(BaseEntity entity);

	// Token: 0x06001F0C RID: 7948
	float GetAmmoFraction();

	// Token: 0x06001F0D RID: 7949
	bool NeedsToReload();

	// Token: 0x06001F0E RID: 7950
	bool Reload();

	// Token: 0x06001F0F RID: 7951
	float CooldownDuration();

	// Token: 0x06001F10 RID: 7952
	bool IsOnCooldown();

	// Token: 0x06001F11 RID: 7953
	bool StartAttacking(BaseEntity entity);

	// Token: 0x06001F12 RID: 7954
	void StopAttacking();
}
