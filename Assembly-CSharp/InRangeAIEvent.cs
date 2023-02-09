using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000355 RID: 853
public class InRangeAIEvent : BaseAIEvent
{
	// Token: 0x1700024F RID: 591
	// (get) Token: 0x06001E9C RID: 7836 RVA: 0x000CE047 File Offset: 0x000CC247
	// (set) Token: 0x06001E9D RID: 7837 RVA: 0x000CE04F File Offset: 0x000CC24F
	public float Range { get; set; }

	// Token: 0x06001E9E RID: 7838 RVA: 0x000CE058 File Offset: 0x000CC258
	public InRangeAIEvent() : base(AIEventType.InRange)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x000CE068 File Offset: 0x000CC268
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		InRangeAIEventData inRangeData = data.inRangeData;
		this.Range = inRangeData.range;
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x000CE090 File Offset: 0x000CC290
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.inRangeData = new InRangeAIEventData();
		aieventData.inRangeData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06001EA1 RID: 7841 RVA: 0x000CE0B4 File Offset: 0x000CC2B4
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		global::BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		base.Result = false;
		if (baseEntity == null)
		{
			return;
		}
		bool flag = Vector3Ex.Distance2D(base.Owner.transform.position, baseEntity.transform.position) <= this.Range;
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
