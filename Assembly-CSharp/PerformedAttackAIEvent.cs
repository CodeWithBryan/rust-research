using System;
using UnityEngine;

// Token: 0x0200035A RID: 858
public class PerformedAttackAIEvent : BaseAIEvent
{
	// Token: 0x06001EAE RID: 7854 RVA: 0x000CE34C File Offset: 0x000CC54C
	public PerformedAttackAIEvent() : base(AIEventType.PerformedAttack)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001EAF RID: 7855 RVA: 0x000CE367 File Offset: 0x000CC567
	public override void Reset()
	{
		base.Reset();
		this.lastExecuteTime = Time.time;
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x000CE37C File Offset: 0x000CC57C
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		this.combatEntity = (memory.Entity.Get(base.InputEntityMemorySlot) as BaseCombatEntity);
		float num = this.lastExecuteTime;
		this.lastExecuteTime = Time.time;
		if (this.combatEntity == null)
		{
			return;
		}
		if (this.combatEntity.lastDealtDamageTime < num)
		{
			base.Result = base.Inverted;
			return;
		}
		if (this.combatEntity.lastDealtDamageTo == null)
		{
			return;
		}
		if (this.combatEntity.lastDealtDamageTo == this.combatEntity)
		{
			return;
		}
		BasePlayer basePlayer = this.combatEntity as BasePlayer;
		if (basePlayer != null)
		{
			if (basePlayer == memory.Entity.Get(5) && basePlayer.lastDealtDamageTo == base.Owner)
			{
				return;
			}
			if (basePlayer == memory.Entity.Get(5) && (basePlayer.lastDealtDamageTo.gameObject.layer == 21 || basePlayer.lastDealtDamageTo.gameObject.layer == 8))
			{
				return;
			}
		}
		if (base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(this.combatEntity.lastDealtDamageTo, base.OutputEntityMemorySlot);
		}
		base.Result = !base.Inverted;
	}

	// Token: 0x0400184E RID: 6222
	protected float lastExecuteTime = float.NegativeInfinity;

	// Token: 0x0400184F RID: 6223
	private BaseCombatEntity combatEntity;
}
