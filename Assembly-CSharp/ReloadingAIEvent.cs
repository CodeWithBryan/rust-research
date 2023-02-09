using System;

// Token: 0x0200035C RID: 860
public class ReloadingAIEvent : BaseAIEvent
{
	// Token: 0x06001EB7 RID: 7863 RVA: 0x000CE5BD File Offset: 0x000CC7BD
	public ReloadingAIEvent() : base(AIEventType.Reloading)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x000CE5D0 File Offset: 0x000CC7D0
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		base.Result = false;
		NPCPlayer npcplayer = baseEntity as NPCPlayer;
		if (npcplayer == null)
		{
			return;
		}
		bool flag = npcplayer.IsReloading();
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
