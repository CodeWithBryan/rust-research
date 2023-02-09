using System;

// Token: 0x0200035E RID: 862
public class StateFinishedAIEvent : BaseAIEvent
{
	// Token: 0x06001EBB RID: 7867 RVA: 0x000CE662 File Offset: 0x000CC862
	public StateFinishedAIEvent() : base(AIEventType.StateFinished)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001EBC RID: 7868 RVA: 0x000CE672 File Offset: 0x000CC872
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (stateStatus == StateStatus.Finished)
		{
			base.Result = !base.Inverted;
		}
	}
}
