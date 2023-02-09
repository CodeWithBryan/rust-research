using System;
using ProtoBuf;

// Token: 0x0200036A RID: 874
public class AggressionTimerAIEvent : BaseAIEvent
{
	// Token: 0x1700025C RID: 604
	// (get) Token: 0x06001EF2 RID: 7922 RVA: 0x000CED8B File Offset: 0x000CCF8B
	// (set) Token: 0x06001EF3 RID: 7923 RVA: 0x000CED93 File Offset: 0x000CCF93
	public float Value { get; private set; }

	// Token: 0x06001EF4 RID: 7924 RVA: 0x000CED9C File Offset: 0x000CCF9C
	public AggressionTimerAIEvent() : base(AIEventType.AggressionTimer)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001EF5 RID: 7925 RVA: 0x000CEDB0 File Offset: 0x000CCFB0
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		AggressionTimerAIEventData aggressionTimerData = data.aggressionTimerData;
		this.Value = aggressionTimerData.value;
	}

	// Token: 0x06001EF6 RID: 7926 RVA: 0x000CEDD8 File Offset: 0x000CCFD8
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.aggressionTimerData = new AggressionTimerAIEventData();
		aieventData.aggressionTimerData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06001EF7 RID: 7927 RVA: 0x000CEDFC File Offset: 0x000CCFFC
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (base.Inverted)
		{
			base.Result = (senses.TimeInAgressiveState < this.Value);
			return;
		}
		base.Result = (senses.TimeInAgressiveState >= this.Value);
	}
}
