using System;
using ProtoBuf;

// Token: 0x0200034E RID: 846
public class BestTargetDetectedAIEvent : BaseAIEvent
{
	// Token: 0x06001E83 RID: 7811 RVA: 0x000CDCD7 File Offset: 0x000CBED7
	public BestTargetDetectedAIEvent() : base(AIEventType.BestTargetDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Normal;
	}

	// Token: 0x06001E84 RID: 7812 RVA: 0x000CDCE8 File Offset: 0x000CBEE8
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x000CDCF4 File Offset: 0x000CBEF4
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		global::BaseEntity bestTarget = iaiattack.GetBestTarget();
		if (base.Inverted)
		{
			if (bestTarget == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = (bestTarget == null);
			return;
		}
		if (bestTarget != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(bestTarget, base.OutputEntityMemorySlot);
		}
		base.Result = (bestTarget != null);
	}
}
