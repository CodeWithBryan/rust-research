using System;
using System.Collections.Generic;
using ProtoBuf;

// Token: 0x02000369 RID: 873
public class AIStateContainer
{
	// Token: 0x17000259 RID: 601
	// (get) Token: 0x06001EE9 RID: 7913 RVA: 0x000CEC08 File Offset: 0x000CCE08
	// (set) Token: 0x06001EEA RID: 7914 RVA: 0x000CEC10 File Offset: 0x000CCE10
	public int ID { get; private set; }

	// Token: 0x1700025A RID: 602
	// (get) Token: 0x06001EEB RID: 7915 RVA: 0x000CEC19 File Offset: 0x000CCE19
	// (set) Token: 0x06001EEC RID: 7916 RVA: 0x000CEC21 File Offset: 0x000CCE21
	public AIState State { get; private set; }

	// Token: 0x1700025B RID: 603
	// (get) Token: 0x06001EED RID: 7917 RVA: 0x000CEC2A File Offset: 0x000CCE2A
	// (set) Token: 0x06001EEE RID: 7918 RVA: 0x000CEC32 File Offset: 0x000CCE32
	public int InputMemorySlot { get; private set; } = -1;

	// Token: 0x06001EEF RID: 7919 RVA: 0x000CEC3C File Offset: 0x000CCE3C
	public void Init(ProtoBuf.AIStateContainer container, global::BaseEntity owner)
	{
		this.ID = container.id;
		this.State = (AIState)container.state;
		this.InputMemorySlot = container.inputMemorySlot;
		this.Events = new List<BaseAIEvent>();
		if (container.events == null)
		{
			return;
		}
		foreach (AIEventData aieventData in container.events)
		{
			BaseAIEvent baseAIEvent = BaseAIEvent.CreateEvent((AIEventType)aieventData.eventType);
			baseAIEvent.Init(aieventData, owner);
			baseAIEvent.Reset();
			this.Events.Add(baseAIEvent);
		}
	}

	// Token: 0x06001EF0 RID: 7920 RVA: 0x000CECE8 File Offset: 0x000CCEE8
	public ProtoBuf.AIStateContainer ToProto()
	{
		ProtoBuf.AIStateContainer aistateContainer = new ProtoBuf.AIStateContainer();
		aistateContainer.id = this.ID;
		aistateContainer.state = (int)this.State;
		aistateContainer.events = new List<AIEventData>();
		aistateContainer.inputMemorySlot = this.InputMemorySlot;
		foreach (BaseAIEvent baseAIEvent in this.Events)
		{
			aistateContainer.events.Add(baseAIEvent.ToProto());
		}
		return aistateContainer;
	}

	// Token: 0x0400185C RID: 6236
	public List<BaseAIEvent> Events;
}
