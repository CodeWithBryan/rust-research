using System;
using ProtoBuf;

// Token: 0x02000362 RID: 866
public class TimeSinceThreatAIEvent : BaseAIEvent
{
	// Token: 0x17000255 RID: 597
	// (get) Token: 0x06001ECD RID: 7885 RVA: 0x000CE98A File Offset: 0x000CCB8A
	// (set) Token: 0x06001ECE RID: 7886 RVA: 0x000CE992 File Offset: 0x000CCB92
	public float Value { get; private set; }

	// Token: 0x06001ECF RID: 7887 RVA: 0x000CE99B File Offset: 0x000CCB9B
	public TimeSinceThreatAIEvent() : base(AIEventType.TimeSinceThreat)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001ED0 RID: 7888 RVA: 0x000CE9AC File Offset: 0x000CCBAC
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TimeSinceThreatAIEventData timeSinceThreatData = data.timeSinceThreatData;
		this.Value = timeSinceThreatData.value;
	}

	// Token: 0x06001ED1 RID: 7889 RVA: 0x000CE9D4 File Offset: 0x000CCBD4
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.timeSinceThreatData = new TimeSinceThreatAIEventData();
		aieventData.timeSinceThreatData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06001ED2 RID: 7890 RVA: 0x000CE9F8 File Offset: 0x000CCBF8
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (base.Inverted)
		{
			base.Result = (senses.TimeSinceThreat < this.Value);
			return;
		}
		base.Result = (senses.TimeSinceThreat >= this.Value);
	}
}
