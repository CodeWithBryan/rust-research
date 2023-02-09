using System;
using ProtoBuf;

// Token: 0x02000368 RID: 872
public class TirednessAboveAIEvent : BaseAIEvent
{
	// Token: 0x17000258 RID: 600
	// (get) Token: 0x06001EE3 RID: 7907 RVA: 0x000CEB49 File Offset: 0x000CCD49
	// (set) Token: 0x06001EE4 RID: 7908 RVA: 0x000CEB51 File Offset: 0x000CCD51
	public float Value { get; private set; }

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000CEB5A File Offset: 0x000CCD5A
	public TirednessAboveAIEvent() : base(AIEventType.TirednessAbove)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x000CEB6C File Offset: 0x000CCD6C
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TirednessAboveAIEventData tirednessAboveData = data.tirednessAboveData;
		this.Value = tirednessAboveData.value;
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x000CEB94 File Offset: 0x000CCD94
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.tirednessAboveData = new TirednessAboveAIEventData();
		aieventData.tirednessAboveData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x000CEBB8 File Offset: 0x000CCDB8
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAITirednessAbove iaitirednessAbove = base.Owner as IAITirednessAbove;
		if (iaitirednessAbove == null)
		{
			return;
		}
		bool flag = iaitirednessAbove.IsTirednessAbove(this.Value);
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
