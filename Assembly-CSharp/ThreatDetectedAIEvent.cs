using System;
using ProtoBuf;

// Token: 0x02000361 RID: 865
public class ThreatDetectedAIEvent : BaseAIEvent
{
	// Token: 0x17000254 RID: 596
	// (get) Token: 0x06001EC7 RID: 7879 RVA: 0x000CE88E File Offset: 0x000CCA8E
	// (set) Token: 0x06001EC8 RID: 7880 RVA: 0x000CE896 File Offset: 0x000CCA96
	public float Range { get; set; }

	// Token: 0x06001EC9 RID: 7881 RVA: 0x000CE89F File Offset: 0x000CCA9F
	public ThreatDetectedAIEvent() : base(AIEventType.ThreatDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x06001ECA RID: 7882 RVA: 0x000CE8B0 File Offset: 0x000CCAB0
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		ThreatDetectedAIEventData threatDetectedData = data.threatDetectedData;
		this.Range = threatDetectedData.range;
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x000CE8D8 File Offset: 0x000CCAD8
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.threatDetectedData = new ThreatDetectedAIEventData();
		aieventData.threatDetectedData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x000CE8FC File Offset: 0x000CCAFC
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		global::BaseEntity nearestThreat = senses.GetNearestThreat(this.Range);
		if (base.Inverted)
		{
			if (nearestThreat == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = (nearestThreat == null);
			return;
		}
		if (nearestThreat != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(nearestThreat, base.OutputEntityMemorySlot);
		}
		base.Result = (nearestThreat != null);
	}
}
