using System;
using UnityEngine;

// Token: 0x0200034C RID: 844
public class AttackedAIEvent : BaseAIEvent
{
	// Token: 0x06001E5F RID: 7775 RVA: 0x000CD7EB File Offset: 0x000CB9EB
	public AttackedAIEvent() : base(AIEventType.Attacked)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001E60 RID: 7776 RVA: 0x000CD806 File Offset: 0x000CBA06
	public override void Reset()
	{
		base.Reset();
		this.lastExecuteTime = Time.time;
	}

	// Token: 0x06001E61 RID: 7777 RVA: 0x000CD81C File Offset: 0x000CBA1C
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		this.combatEntity = (memory.Entity.Get(base.InputEntityMemorySlot) as BaseCombatEntity);
		float num = this.lastExecuteTime;
		this.lastExecuteTime = Time.time;
		if (this.combatEntity == null)
		{
			return;
		}
		if (this.combatEntity.lastAttackedTime >= num)
		{
			if (this.combatEntity.lastAttacker == null)
			{
				return;
			}
			if (this.combatEntity.lastAttacker == this.combatEntity)
			{
				return;
			}
			BasePlayer basePlayer = this.combatEntity.lastAttacker as BasePlayer;
			if (basePlayer != null && basePlayer == memory.Entity.Get(5) && basePlayer.lastDealtDamageTo == base.Owner)
			{
				return;
			}
			if (base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Set(this.combatEntity.lastAttacker, base.OutputEntityMemorySlot);
			}
			base.Result = !base.Inverted;
		}
	}

	// Token: 0x0400183A RID: 6202
	protected float lastExecuteTime = float.NegativeInfinity;

	// Token: 0x0400183B RID: 6203
	private BaseCombatEntity combatEntity;
}
