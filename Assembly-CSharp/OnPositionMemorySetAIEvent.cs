using System;

// Token: 0x02000359 RID: 857
public class OnPositionMemorySetAIEvent : BaseAIEvent
{
	// Token: 0x06001EAC RID: 7852 RVA: 0x000CE303 File Offset: 0x000CC503
	public OnPositionMemorySetAIEvent() : base(AIEventType.OnPositionMemorySet)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x000CE314 File Offset: 0x000CC514
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		if (memory.Position.GetTimeSinceSet(5) <= 0.5f)
		{
			base.Result = !base.Inverted;
			return;
		}
		base.Result = base.Inverted;
	}
}
