using System;
using UnityEngine;

// Token: 0x020005ED RID: 1517
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Harvest")]
public class MissionObjective_Harvest : MissionObjective
{
	// Token: 0x06002C7D RID: 11389 RVA: 0x0010A291 File Offset: 0x00108491
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002C7E RID: 11390 RVA: 0x0010A3A8 File Offset: 0x001085A8
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
		if (type == BaseMission.MissionEventType.HARVEST)
		{
			string[] array = this.itemShortnames;
			int i = 0;
			while (i < array.Length)
			{
				if (array[i] == identifier)
				{
					instance.objectiveStatuses[index].genericInt1 += (int)amount;
					if (instance.objectiveStatuses[index].genericInt1 >= this.targetItemAmount)
					{
						this.CompleteObjective(index, instance, playerFor);
						playerFor.MissionDirty(true);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x0010A31E File Offset: 0x0010851E
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
	}

	// Token: 0x04002445 RID: 9285
	public string[] itemShortnames;

	// Token: 0x04002446 RID: 9286
	public int targetItemAmount;
}
