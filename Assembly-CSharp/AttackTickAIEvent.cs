using System;

// Token: 0x0200034B RID: 843
public class AttackTickAIEvent : BaseAIEvent
{
	// Token: 0x06001E5D RID: 7773 RVA: 0x000CD774 File Offset: 0x000CB974
	public AttackTickAIEvent() : base(AIEventType.AttackTick)
	{
		base.Rate = BaseAIEvent.ExecuteRate.VeryFast;
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x000CD788 File Offset: 0x000CB988
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		IAIAttack iaiattack = base.Owner as IAIAttack;
		if (iaiattack == null)
		{
			return;
		}
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		iaiattack.AttackTick(this.deltaTime, baseEntity, senses.Memory.IsLOS(baseEntity));
		base.Result = !base.Inverted;
	}
}
