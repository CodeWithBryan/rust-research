using System;
using UnityEngine;

// Token: 0x02000360 RID: 864
public class TargetLostAIEvent : BaseAIEvent
{
	// Token: 0x17000253 RID: 595
	// (get) Token: 0x06001EC3 RID: 7875 RVA: 0x000CE792 File Offset: 0x000CC992
	// (set) Token: 0x06001EC4 RID: 7876 RVA: 0x000CE79A File Offset: 0x000CC99A
	public float Range { get; set; }

	// Token: 0x06001EC5 RID: 7877 RVA: 0x000CE7A3 File Offset: 0x000CC9A3
	public TargetLostAIEvent() : base(AIEventType.TargetLost)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001EC6 RID: 7878 RVA: 0x000CE7B4 File Offset: 0x000CC9B4
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		BaseEntity baseEntity = memory.Entity.Get(base.InputEntityMemorySlot);
		if (baseEntity == null)
		{
			base.Result = !base.Inverted;
			return;
		}
		if (Vector3.Distance(baseEntity.transform.position, base.Owner.transform.position) > senses.TargetLostRange)
		{
			base.Result = !base.Inverted;
			return;
		}
		BasePlayer basePlayer = baseEntity as BasePlayer;
		if (baseEntity.Health() <= 0f || (basePlayer != null && basePlayer.IsDead()))
		{
			base.Result = !base.Inverted;
			return;
		}
		if (senses.ignoreSafeZonePlayers && basePlayer != null && basePlayer.InSafeZone())
		{
			base.Result = !base.Inverted;
			return;
		}
	}
}
