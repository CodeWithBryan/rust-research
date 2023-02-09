using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000356 RID: 854
public class InRangeOfHomeAIEvent : BaseAIEvent
{
	// Token: 0x17000250 RID: 592
	// (get) Token: 0x06001EA2 RID: 7842 RVA: 0x000CE125 File Offset: 0x000CC325
	// (set) Token: 0x06001EA3 RID: 7843 RVA: 0x000CE12D File Offset: 0x000CC32D
	public float Range { get; set; }

	// Token: 0x06001EA4 RID: 7844 RVA: 0x000CE136 File Offset: 0x000CC336
	public InRangeOfHomeAIEvent() : base(AIEventType.InRangeOfHome)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x000CE148 File Offset: 0x000CC348
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		InRangeOfHomeAIEventData inRangeOfHomeData = data.inRangeOfHomeData;
		this.Range = inRangeOfHomeData.range;
	}

	// Token: 0x06001EA6 RID: 7846 RVA: 0x000CE170 File Offset: 0x000CC370
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.inRangeOfHomeData = new InRangeOfHomeAIEventData();
		aieventData.inRangeOfHomeData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06001EA7 RID: 7847 RVA: 0x000CE194 File Offset: 0x000CC394
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		Vector3 b = memory.Position.Get(4);
		base.Result = false;
		bool flag = Vector3Ex.Distance2D(base.Owner.transform.position, b) <= this.Range;
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
