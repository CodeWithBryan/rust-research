using System;

// Token: 0x02000354 RID: 852
public class InAttackRangeAIEvent : BaseAIEvent
{
	// Token: 0x06001E9A RID: 7834 RVA: 0x000CDFD7 File Offset: 0x000CC1D7
	public InAttackRangeAIEvent() : base(AIEventType.InAttackRange)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001E9B RID: 7835 RVA: 0x000CDFE8 File Offset: 0x000CC1E8
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		base.Result = false;
		if (baseEntity == null)
		{
			return;
		}
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		float num;
		bool flag = iaiattack.IsTargetInRange(baseEntity, out num);
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
