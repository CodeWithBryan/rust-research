using System;
using ProtoBuf;

// Token: 0x0200035F RID: 863
public class TargetDetectedAIEvent : BaseAIEvent
{
	// Token: 0x17000252 RID: 594
	// (get) Token: 0x06001EBD RID: 7869 RVA: 0x000CE693 File Offset: 0x000CC893
	// (set) Token: 0x06001EBE RID: 7870 RVA: 0x000CE69B File Offset: 0x000CC89B
	public float Range { get; set; }

	// Token: 0x06001EBF RID: 7871 RVA: 0x000CE6A4 File Offset: 0x000CC8A4
	public TargetDetectedAIEvent() : base(AIEventType.TargetDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x000CE6B8 File Offset: 0x000CC8B8
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TargetDetectedAIEventData targetDetectedData = data.targetDetectedData;
		this.Range = targetDetectedData.range;
	}

	// Token: 0x06001EC1 RID: 7873 RVA: 0x000CE6E0 File Offset: 0x000CC8E0
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.targetDetectedData = new TargetDetectedAIEventData();
		aieventData.targetDetectedData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06001EC2 RID: 7874 RVA: 0x000CE704 File Offset: 0x000CC904
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		global::BaseEntity nearestTarget = senses.GetNearestTarget(this.Range);
		if (base.Inverted)
		{
			if (nearestTarget == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = (nearestTarget == null);
			return;
		}
		if (nearestTarget != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(nearestTarget, base.OutputEntityMemorySlot);
		}
		base.Result = (nearestTarget != null);
	}
}
