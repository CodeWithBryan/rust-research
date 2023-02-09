using System;

// Token: 0x0200035D RID: 861
public class StateErrorAIEvent : BaseAIEvent
{
	// Token: 0x06001EB9 RID: 7865 RVA: 0x000CE621 File Offset: 0x000CC821
	public StateErrorAIEvent() : base(AIEventType.StateError)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x000CE631 File Offset: 0x000CC831
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (stateStatus == StateStatus.Error)
		{
			base.Result = !base.Inverted;
			return;
		}
		if (stateStatus == StateStatus.Running)
		{
			base.Result = base.Inverted;
		}
	}
}
