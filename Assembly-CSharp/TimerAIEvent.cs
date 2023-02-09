using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000363 RID: 867
public class TimerAIEvent : BaseAIEvent
{
	// Token: 0x17000256 RID: 598
	// (get) Token: 0x06001ED3 RID: 7891 RVA: 0x000CEA45 File Offset: 0x000CCC45
	// (set) Token: 0x06001ED4 RID: 7892 RVA: 0x000CEA4D File Offset: 0x000CCC4D
	public float DurationMin { get; set; }

	// Token: 0x17000257 RID: 599
	// (get) Token: 0x06001ED5 RID: 7893 RVA: 0x000CEA56 File Offset: 0x000CCC56
	// (set) Token: 0x06001ED6 RID: 7894 RVA: 0x000CEA5E File Offset: 0x000CCC5E
	public float DurationMax { get; set; }

	// Token: 0x06001ED7 RID: 7895 RVA: 0x000CEA67 File Offset: 0x000CCC67
	public TimerAIEvent() : base(AIEventType.Timer)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001ED8 RID: 7896 RVA: 0x000CEA78 File Offset: 0x000CCC78
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TimerAIEventData timerData = data.timerData;
		this.DurationMin = timerData.duration;
		this.DurationMax = timerData.durationMax;
	}

	// Token: 0x06001ED9 RID: 7897 RVA: 0x000CEAAC File Offset: 0x000CCCAC
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.timerData = new TimerAIEventData();
		aieventData.timerData.duration = this.DurationMin;
		aieventData.timerData.durationMax = this.DurationMax;
		return aieventData;
	}

	// Token: 0x06001EDA RID: 7898 RVA: 0x000CEAE1 File Offset: 0x000CCCE1
	public override void Reset()
	{
		base.Reset();
		this.currentDuration = UnityEngine.Random.Range(this.DurationMin, this.DurationMax);
		this.elapsedDuration = 0f;
	}

	// Token: 0x06001EDB RID: 7899 RVA: 0x000CEB0B File Offset: 0x000CCD0B
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		this.elapsedDuration += this.deltaTime;
		if (this.elapsedDuration >= this.currentDuration)
		{
			base.Result = !base.Inverted;
		}
	}

	// Token: 0x04001857 RID: 6231
	protected float currentDuration;

	// Token: 0x04001858 RID: 6232
	protected float elapsedDuration;
}
