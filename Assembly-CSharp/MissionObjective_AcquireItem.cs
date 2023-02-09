using System;
using UnityEngine;

// Token: 0x020005EB RID: 1515
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/AcquireItem")]
public class MissionObjective_AcquireItem : MissionObjective
{
	// Token: 0x06002C75 RID: 11381 RVA: 0x0010A291 File Offset: 0x00108491
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002C76 RID: 11382 RVA: 0x0010A29C File Offset: 0x0010849C
	public override void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, string identifier, float amount)
	{
		base.ProcessMissionEvent(playerFor, instance, index, type, identifier, amount);
		if (base.IsCompleted(index, instance))
		{
			return;
		}
		if (!base.CanProgress(index, instance))
		{
			return;
		}
		if (type == BaseMission.MissionEventType.ACQUIRE_ITEM)
		{
			if (this.itemShortname == identifier)
			{
				instance.objectiveStatuses[index].genericInt1 += (int)amount;
			}
			if (instance.objectiveStatuses[index].genericInt1 >= this.targetItemAmount)
			{
				this.CompleteObjective(index, instance, playerFor);
				playerFor.MissionDirty(true);
			}
		}
	}

	// Token: 0x06002C77 RID: 11383 RVA: 0x0010A31E File Offset: 0x0010851E
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
	}

	// Token: 0x04002442 RID: 9282
	public string itemShortname;

	// Token: 0x04002443 RID: 9283
	public int targetItemAmount;
}
