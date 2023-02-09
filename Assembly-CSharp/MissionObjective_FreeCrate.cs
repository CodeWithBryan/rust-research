using System;
using UnityEngine;

// Token: 0x020005EC RID: 1516
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/FreeCrate")]
public class MissionObjective_FreeCrate : MissionObjective
{
	// Token: 0x06002C79 RID: 11385 RVA: 0x0010A291 File Offset: 0x00108491
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002C7A RID: 11386 RVA: 0x0010A334 File Offset: 0x00108534
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
		if (type == BaseMission.MissionEventType.FREE_CRATE)
		{
			instance.objectiveStatuses[index].genericInt1 += (int)amount;
			if (instance.objectiveStatuses[index].genericInt1 >= this.targetAmount)
			{
				this.CompleteObjective(index, instance, playerFor);
				playerFor.MissionDirty(true);
			}
		}
	}

	// Token: 0x06002C7B RID: 11387 RVA: 0x0010A31E File Offset: 0x0010851E
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
	}

	// Token: 0x04002444 RID: 9284
	public int targetAmount;
}
