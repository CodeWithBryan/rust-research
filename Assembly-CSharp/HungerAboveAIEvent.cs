using System;
using ProtoBuf;

// Token: 0x02000352 RID: 850
public class HungerAboveAIEvent : BaseAIEvent
{
	// Token: 0x1700024E RID: 590
	// (get) Token: 0x06001E93 RID: 7827 RVA: 0x000CDF1E File Offset: 0x000CC11E
	// (set) Token: 0x06001E94 RID: 7828 RVA: 0x000CDF26 File Offset: 0x000CC126
	public float Value { get; private set; }

	// Token: 0x06001E95 RID: 7829 RVA: 0x000CDF2F File Offset: 0x000CC12F
	public HungerAboveAIEvent() : base(AIEventType.HungerAbove)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x000CDF40 File Offset: 0x000CC140
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		HungerAboveAIEventData hungerAboveData = data.hungerAboveData;
		this.Value = hungerAboveData.value;
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x000CDF68 File Offset: 0x000CC168
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.hungerAboveData = new HungerAboveAIEventData();
		aieventData.hungerAboveData.value = this.Value;
		return aieventData;
	}

	// Token: 0x06001E98 RID: 7832 RVA: 0x000CDF8C File Offset: 0x000CC18C
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		IAIHungerAbove iaihungerAbove = base.Owner as IAIHungerAbove;
		if (iaihungerAbove == null)
		{
			base.Result = false;
			return;
		}
		bool flag = iaihungerAbove.IsHungerAbove(this.Value);
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}
}
