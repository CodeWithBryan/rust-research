using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200034D RID: 845
public class BaseAIEvent
{
	// Token: 0x1700023F RID: 575
	// (get) Token: 0x06001E62 RID: 7778 RVA: 0x000CD925 File Offset: 0x000CBB25
	// (set) Token: 0x06001E63 RID: 7779 RVA: 0x000CD92D File Offset: 0x000CBB2D
	public AIEventType EventType { get; private set; }

	// Token: 0x17000240 RID: 576
	// (get) Token: 0x06001E64 RID: 7780 RVA: 0x000CD936 File Offset: 0x000CBB36
	// (set) Token: 0x06001E65 RID: 7781 RVA: 0x000CD93E File Offset: 0x000CBB3E
	public int TriggerStateContainerID { get; private set; } = -1;

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x06001E66 RID: 7782 RVA: 0x000CD947 File Offset: 0x000CBB47
	// (set) Token: 0x06001E67 RID: 7783 RVA: 0x000CD94F File Offset: 0x000CBB4F
	public BaseAIEvent.ExecuteRate Rate { get; protected set; } = BaseAIEvent.ExecuteRate.Normal;

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x06001E68 RID: 7784 RVA: 0x000CD958 File Offset: 0x000CBB58
	public float ExecutionRate
	{
		get
		{
			switch (this.Rate)
			{
			case BaseAIEvent.ExecuteRate.Slow:
				return 1f;
			case BaseAIEvent.ExecuteRate.Normal:
				return 0.5f;
			case BaseAIEvent.ExecuteRate.Fast:
				return 0.25f;
			case BaseAIEvent.ExecuteRate.VeryFast:
				return 0.1f;
			default:
				return 0.5f;
			}
		}
	}

	// Token: 0x17000243 RID: 579
	// (get) Token: 0x06001E69 RID: 7785 RVA: 0x000CD9A1 File Offset: 0x000CBBA1
	// (set) Token: 0x06001E6A RID: 7786 RVA: 0x000CD9A9 File Offset: 0x000CBBA9
	public bool ShouldExecute { get; protected set; }

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x06001E6B RID: 7787 RVA: 0x000CD9B2 File Offset: 0x000CBBB2
	// (set) Token: 0x06001E6C RID: 7788 RVA: 0x000CD9BA File Offset: 0x000CBBBA
	public bool Result { get; protected set; }

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x06001E6D RID: 7789 RVA: 0x000CD9C3 File Offset: 0x000CBBC3
	// (set) Token: 0x06001E6E RID: 7790 RVA: 0x000CD9CB File Offset: 0x000CBBCB
	public bool Inverted { get; private set; }

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x06001E6F RID: 7791 RVA: 0x000CD9D4 File Offset: 0x000CBBD4
	// (set) Token: 0x06001E70 RID: 7792 RVA: 0x000CD9DC File Offset: 0x000CBBDC
	public int OutputEntityMemorySlot { get; protected set; } = -1;

	// Token: 0x17000247 RID: 583
	// (get) Token: 0x06001E71 RID: 7793 RVA: 0x000CD9E5 File Offset: 0x000CBBE5
	public bool ShouldSetOutputEntityMemory
	{
		get
		{
			return this.OutputEntityMemorySlot > -1;
		}
	}

	// Token: 0x17000248 RID: 584
	// (get) Token: 0x06001E72 RID: 7794 RVA: 0x000CD9F0 File Offset: 0x000CBBF0
	// (set) Token: 0x06001E73 RID: 7795 RVA: 0x000CD9F8 File Offset: 0x000CBBF8
	public int InputEntityMemorySlot { get; protected set; } = -1;

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x06001E74 RID: 7796 RVA: 0x000CDA01 File Offset: 0x000CBC01
	// (set) Token: 0x06001E75 RID: 7797 RVA: 0x000CDA09 File Offset: 0x000CBC09
	public int ID { get; protected set; }

	// Token: 0x1700024A RID: 586
	// (get) Token: 0x06001E76 RID: 7798 RVA: 0x000CDA12 File Offset: 0x000CBC12
	// (set) Token: 0x06001E77 RID: 7799 RVA: 0x000CDA1A File Offset: 0x000CBC1A
	public global::BaseEntity Owner { get; private set; }

	// Token: 0x1700024B RID: 587
	// (get) Token: 0x06001E78 RID: 7800 RVA: 0x000CDA23 File Offset: 0x000CBC23
	public bool HasValidTriggerState
	{
		get
		{
			return this.TriggerStateContainerID != -1;
		}
	}

	// Token: 0x06001E79 RID: 7801 RVA: 0x000CDA31 File Offset: 0x000CBC31
	public BaseAIEvent(AIEventType type)
	{
		this.EventType = type;
	}

	// Token: 0x06001E7A RID: 7802 RVA: 0x000CDA5C File Offset: 0x000CBC5C
	public virtual void Init(AIEventData data, global::BaseEntity owner)
	{
		this.Init(data.triggerStateContainer, data.id, owner, data.inputMemorySlot, data.outputMemorySlot, data.inverted);
	}

	// Token: 0x06001E7B RID: 7803 RVA: 0x000CDA83 File Offset: 0x000CBC83
	public virtual void Init(int triggerStateContainer, int id, global::BaseEntity owner, int inputMemorySlot, int outputMemorySlot, bool inverted)
	{
		this.TriggerStateContainerID = triggerStateContainer;
		this.ID = id;
		this.Owner = owner;
		this.InputEntityMemorySlot = inputMemorySlot;
		this.OutputEntityMemorySlot = outputMemorySlot;
		this.Inverted = inverted;
	}

	// Token: 0x06001E7C RID: 7804 RVA: 0x000CDAB4 File Offset: 0x000CBCB4
	public virtual AIEventData ToProto()
	{
		return new AIEventData
		{
			id = this.ID,
			eventType = (int)this.EventType,
			triggerStateContainer = this.TriggerStateContainerID,
			outputMemorySlot = this.OutputEntityMemorySlot,
			inputMemorySlot = this.InputEntityMemorySlot,
			inverted = this.Inverted
		};
	}

	// Token: 0x06001E7D RID: 7805 RVA: 0x000CDB0E File Offset: 0x000CBD0E
	public virtual void Reset()
	{
		this.executeTimer = 0f;
		this.deltaTime = 0f;
		this.Result = false;
	}

	// Token: 0x06001E7E RID: 7806 RVA: 0x000CDB30 File Offset: 0x000CBD30
	public void Tick(float deltaTime, IAIEventListener listener)
	{
		this.deltaTime += deltaTime;
		this.executeTimer += deltaTime;
		float executionRate = this.ExecutionRate;
		if (this.executeTimer >= executionRate)
		{
			this.executeTimer = 0f;
			this.ShouldExecute = true;
			return;
		}
		this.ShouldExecute = false;
	}

	// Token: 0x06001E7F RID: 7807 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
	}

	// Token: 0x06001E80 RID: 7808 RVA: 0x000CDB83 File Offset: 0x000CBD83
	public virtual void PostExecute()
	{
		this.deltaTime = 0f;
	}

	// Token: 0x06001E81 RID: 7809 RVA: 0x000CDB90 File Offset: 0x000CBD90
	public void TriggerStateChange(IAIEventListener listener, int sourceEventID)
	{
		listener.EventTriggeredStateChange(this.TriggerStateContainerID, sourceEventID);
	}

	// Token: 0x06001E82 RID: 7810 RVA: 0x000CDBA0 File Offset: 0x000CBDA0
	public static BaseAIEvent CreateEvent(AIEventType eventType)
	{
		switch (eventType)
		{
		case AIEventType.Timer:
			return new TimerAIEvent();
		case AIEventType.PlayerDetected:
			return new PlayerDetectedAIEvent();
		case AIEventType.StateError:
			return new StateErrorAIEvent();
		case AIEventType.Attacked:
			return new AttackedAIEvent();
		case AIEventType.StateFinished:
			return new StateFinishedAIEvent();
		case AIEventType.InAttackRange:
			return new InAttackRangeAIEvent();
		case AIEventType.HealthBelow:
			return new HealthBelowAIEvent();
		case AIEventType.InRange:
			return new InRangeAIEvent();
		case AIEventType.PerformedAttack:
			return new PerformedAttackAIEvent();
		case AIEventType.TirednessAbove:
			return new TirednessAboveAIEvent();
		case AIEventType.HungerAbove:
			return new HungerAboveAIEvent();
		case AIEventType.ThreatDetected:
			return new ThreatDetectedAIEvent();
		case AIEventType.TargetDetected:
			return new TargetDetectedAIEvent();
		case AIEventType.AmmoBelow:
			return new AmmoBelowAIEvent();
		case AIEventType.BestTargetDetected:
			return new BestTargetDetectedAIEvent();
		case AIEventType.IsVisible:
			return new IsVisibleAIEvent();
		case AIEventType.AttackTick:
			return new AttackTickAIEvent();
		case AIEventType.IsMounted:
			return new IsMountedAIEvent();
		case AIEventType.And:
			return new AndAIEvent();
		case AIEventType.Chance:
			return new ChanceAIEvent();
		case AIEventType.TargetLost:
			return new TargetLostAIEvent();
		case AIEventType.TimeSinceThreat:
			return new TimeSinceThreatAIEvent();
		case AIEventType.OnPositionMemorySet:
			return new OnPositionMemorySetAIEvent();
		case AIEventType.AggressionTimer:
			return new AggressionTimerAIEvent();
		case AIEventType.Reloading:
			return new ReloadingAIEvent();
		case AIEventType.InRangeOfHome:
			return new InRangeOfHomeAIEvent();
		default:
			Debug.LogWarning("No case for " + eventType + " event in BaseAIEvent.CreateEvent()!");
			return null;
		}
	}

	// Token: 0x04001846 RID: 6214
	private float executeTimer;

	// Token: 0x04001847 RID: 6215
	protected float deltaTime;

	// Token: 0x02000C60 RID: 3168
	public enum ExecuteRate
	{
		// Token: 0x0400421B RID: 16923
		Slow,
		// Token: 0x0400421C RID: 16924
		Normal,
		// Token: 0x0400421D RID: 16925
		Fast,
		// Token: 0x0400421E RID: 16926
		VeryFast
	}
}
