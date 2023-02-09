using System;

// Token: 0x02000358 RID: 856
public class IsMountedAIEvent : BaseAIEvent
{
	// Token: 0x06001EAA RID: 7850 RVA: 0x000CE260 File Offset: 0x000CC460
	public IsMountedAIEvent() : base(AIEventType.IsMounted)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06001EAB RID: 7851 RVA: 0x000CE274 File Offset: 0x000CC474
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		IAIMounted iaimounted = memory.Entity.Get(base.InputEntityMemorySlot) as IAIMounted;
		base.Result = false;
		if (iaimounted == null)
		{
			return;
		}
		if (base.Inverted && !iaimounted.IsMounted())
		{
			base.Result = true;
		}
		if (!base.Inverted && iaimounted.IsMounted())
		{
			base.Result = true;
		}
		if (base.Result && base.ShouldSetOutputEntityMemory)
		{
			memory.Entity.Set(memory.Entity.Get(base.InputEntityMemorySlot), base.OutputEntityMemorySlot);
		}
	}
}
