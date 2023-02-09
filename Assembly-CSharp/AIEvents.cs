using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000342 RID: 834
public class AIEvents
{
	// Token: 0x1700023D RID: 573
	// (get) Token: 0x06001E45 RID: 7749 RVA: 0x000CD23C File Offset: 0x000CB43C
	// (set) Token: 0x06001E46 RID: 7750 RVA: 0x000CD244 File Offset: 0x000CB444
	public int CurrentInputMemorySlot { get; private set; } = -1;

	// Token: 0x06001E47 RID: 7751 RVA: 0x000CD250 File Offset: 0x000CB450
	public void Init(IAIEventListener listener, AIStateContainer stateContainer, BaseEntity owner, AIBrainSenses senses)
	{
		this.CurrentInputMemorySlot = stateContainer.InputMemorySlot;
		this.eventListener = listener;
		this.RemoveAll();
		this.AddStateEvents(stateContainer.Events, owner);
		this.Memory.Entity.Set(owner, 4);
		this.senses = senses;
	}

	// Token: 0x06001E48 RID: 7752 RVA: 0x000CD29D File Offset: 0x000CB49D
	private void RemoveAll()
	{
		this.events.Clear();
	}

	// Token: 0x06001E49 RID: 7753 RVA: 0x000CD2AC File Offset: 0x000CB4AC
	private void AddStateEvents(List<BaseAIEvent> events, BaseEntity owner)
	{
		foreach (BaseAIEvent aiEvent in events)
		{
			this.Add(aiEvent);
		}
	}

	// Token: 0x06001E4A RID: 7754 RVA: 0x000CD2FC File Offset: 0x000CB4FC
	private void Add(BaseAIEvent aiEvent)
	{
		if (this.events.Contains(aiEvent))
		{
			Debug.LogWarning("Attempting to add duplicate AI event: " + aiEvent.EventType);
			return;
		}
		aiEvent.Reset();
		this.events.Add(aiEvent);
	}

	// Token: 0x06001E4B RID: 7755 RVA: 0x000CD33C File Offset: 0x000CB53C
	public void Tick(float deltaTime, StateStatus stateStatus)
	{
		foreach (BaseAIEvent baseAIEvent in this.events)
		{
			baseAIEvent.Tick(deltaTime, this.eventListener);
		}
		this.inBlock = false;
		this.currentEventIndex = 0;
		this.currentEventIndex = 0;
		while (this.currentEventIndex < this.events.Count)
		{
			BaseAIEvent baseAIEvent2 = this.events[this.currentEventIndex];
			BaseAIEvent baseAIEvent3 = (this.currentEventIndex < this.events.Count - 1) ? this.events[this.currentEventIndex + 1] : null;
			if (baseAIEvent3 != null && baseAIEvent3.EventType == AIEventType.And && !this.inBlock)
			{
				this.inBlock = true;
			}
			if (baseAIEvent2.EventType != AIEventType.And)
			{
				if (baseAIEvent2.ShouldExecute)
				{
					baseAIEvent2.Execute(this.Memory, this.senses, stateStatus);
					baseAIEvent2.PostExecute();
				}
				bool result = baseAIEvent2.Result;
				if (this.inBlock)
				{
					if (result)
					{
						if ((baseAIEvent3 != null && baseAIEvent3.EventType != AIEventType.And) || baseAIEvent3 == null)
						{
							this.inBlock = false;
							if (baseAIEvent2.HasValidTriggerState)
							{
								baseAIEvent2.TriggerStateChange(this.eventListener, baseAIEvent2.ID);
								return;
							}
						}
					}
					else
					{
						this.inBlock = false;
						this.currentEventIndex = this.FindNextEventBlock() - 1;
					}
				}
				else if (result && baseAIEvent2.HasValidTriggerState)
				{
					baseAIEvent2.TriggerStateChange(this.eventListener, baseAIEvent2.ID);
					return;
				}
			}
			this.currentEventIndex++;
		}
	}

	// Token: 0x06001E4C RID: 7756 RVA: 0x000CD4D4 File Offset: 0x000CB6D4
	private int FindNextEventBlock()
	{
		for (int i = this.currentEventIndex; i < this.events.Count; i++)
		{
			BaseAIEvent baseAIEvent = this.events[i];
			BaseAIEvent baseAIEvent2 = (i < this.events.Count - 1) ? this.events[i + 1] : null;
			if (baseAIEvent2 != null && baseAIEvent2.EventType != AIEventType.And && baseAIEvent.EventType != AIEventType.And)
			{
				return i + 1;
			}
		}
		return this.events.Count + 1;
	}

	// Token: 0x0400181C RID: 6172
	public AIMemory Memory = new AIMemory();

	// Token: 0x0400181E RID: 6174
	private List<BaseAIEvent> events = new List<BaseAIEvent>();

	// Token: 0x0400181F RID: 6175
	private IAIEventListener eventListener;

	// Token: 0x04001820 RID: 6176
	private AIBrainSenses senses;

	// Token: 0x04001821 RID: 6177
	private int currentEventIndex;

	// Token: 0x04001822 RID: 6178
	private bool inBlock;
}
