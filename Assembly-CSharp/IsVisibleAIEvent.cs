using System;

// Token: 0x02000357 RID: 855
public class IsVisibleAIEvent : BaseAIEvent
{
	// Token: 0x06001EA8 RID: 7848 RVA: 0x000CE1EC File Offset: 0x000CC3EC
	public IsVisibleAIEvent() : base(AIEventType.IsVisible)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001EA9 RID: 7849 RVA: 0x000CE200 File Offset: 0x000CC400
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		if (baseEntity == null)
		{
			return;
		}
		if (!(base.Owner is IAIAttack))
		{
			return;
		}
		bool flag = senses.Memory.IsLOS(baseEntity);
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
