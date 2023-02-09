using System;
using ProtoBuf;

// Token: 0x02000350 RID: 848
public class HealthBelowAIEvent : BaseAIEvent
{
	// Token: 0x1700024D RID: 589
	// (get) Token: 0x06001E8C RID: 7820 RVA: 0x000CDE3E File Offset: 0x000CC03E
	// (set) Token: 0x06001E8D RID: 7821 RVA: 0x000CDE46 File Offset: 0x000CC046
	public float HealthFraction { get; set; }

	// Token: 0x06001E8E RID: 7822 RVA: 0x000CDE4F File Offset: 0x000CC04F
	public HealthBelowAIEvent() : base(AIEventType.HealthBelow)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001E8F RID: 7823 RVA: 0x000CDE60 File Offset: 0x000CC060
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		HealthBelowAIEventData healthBelowData = data.healthBelowData;
		this.HealthFraction = healthBelowData.healthFraction;
	}

	// Token: 0x06001E90 RID: 7824 RVA: 0x000CDE88 File Offset: 0x000CC088
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.healthBelowData = new HealthBelowAIEventData();
		aieventData.healthBelowData.healthFraction = this.HealthFraction;
		return aieventData;
	}

	// Token: 0x06001E91 RID: 7825 RVA: 0x000CDEAC File Offset: 0x000CC0AC
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		this.combatEntity = (memory.Entity.Get(base.InputEntityMemorySlot) as BaseCombatEntity);
		if (this.combatEntity == null)
		{
			return;
		}
		bool flag = this.combatEntity.healthFraction < this.HealthFraction;
		if (base.Inverted)
		{
			base.Result = !flag;
			return;
		}
		base.Result = flag;
	}

	// Token: 0x0400184A RID: 6218
	private BaseCombatEntity combatEntity;
}
