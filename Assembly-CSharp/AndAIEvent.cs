using System;

// Token: 0x0200034A RID: 842
public class AndAIEvent : BaseAIEvent
{
	// Token: 0x06001E5B RID: 7771 RVA: 0x000CD75A File Offset: 0x000CB95A
	public AndAIEvent() : base(AIEventType.And)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Normal;
	}

	// Token: 0x06001E5C RID: 7772 RVA: 0x000CD76B File Offset: 0x000CB96B
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
	}
}
