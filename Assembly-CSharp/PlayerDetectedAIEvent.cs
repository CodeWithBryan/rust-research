using System;
using ProtoBuf;

// Token: 0x0200035B RID: 859
public class PlayerDetectedAIEvent : BaseAIEvent
{
	// Token: 0x17000251 RID: 593
	// (get) Token: 0x06001EB1 RID: 7857 RVA: 0x000CE4C4 File Offset: 0x000CC6C4
	// (set) Token: 0x06001EB2 RID: 7858 RVA: 0x000CE4CC File Offset: 0x000CC6CC
	public float Range { get; set; }

	// Token: 0x06001EB3 RID: 7859 RVA: 0x000CE4D5 File Offset: 0x000CC6D5
	public PlayerDetectedAIEvent() : base(AIEventType.PlayerDetected)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Slow;
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x000CE4E8 File Offset: 0x000CC6E8
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		PlayerDetectedAIEventData playerDetectedData = data.playerDetectedData;
		this.Range = playerDetectedData.range;
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x000CE510 File Offset: 0x000CC710
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.playerDetectedData = new PlayerDetectedAIEventData();
		aieventData.playerDetectedData.range = this.Range;
		return aieventData;
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x000CE534 File Offset: 0x000CC734
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = false;
		global::BaseEntity nearestPlayer = senses.GetNearestPlayer(this.Range);
		if (base.Inverted)
		{
			if (nearestPlayer == null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = (nearestPlayer == null);
			return;
		}
		if (nearestPlayer != null && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(nearestPlayer, base.OutputEntityMemorySlot);
		}
		base.Result = (nearestPlayer != null);
	}
}
