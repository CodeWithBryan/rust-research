using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200034F RID: 847
public class ChanceAIEvent : BaseAIEvent
{
	// Token: 0x1700024C RID: 588
	// (get) Token: 0x06001E86 RID: 7814 RVA: 0x000CDD8C File Offset: 0x000CBF8C
	// (set) Token: 0x06001E87 RID: 7815 RVA: 0x000CDD94 File Offset: 0x000CBF94
	public float Chance { get; set; }

	// Token: 0x06001E88 RID: 7816 RVA: 0x000CDD9D File Offset: 0x000CBF9D
	public ChanceAIEvent() : base(AIEventType.Chance)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001E89 RID: 7817 RVA: 0x000CDDAE File Offset: 0x000CBFAE
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		this.Chance = data.chanceData.value;
	}

	// Token: 0x06001E8A RID: 7818 RVA: 0x000CDDC9 File Offset: 0x000CBFC9
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.chanceData = new ChanceAIEventData();
		aieventData.chanceData.value = this.Chance;
		return aieventData;
	}

	// Token: 0x06001E8B RID: 7819 RVA: 0x000CDDF0 File Offset: 0x000CBFF0
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		bool flag = UnityEngine.Random.Range(0f, 1f) <= this.Chance;
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
