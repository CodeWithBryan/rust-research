using System;
using UnityEngine;

// Token: 0x020005EA RID: 1514
public class MissionObjective : ScriptableObject
{
	// Token: 0x06002C6A RID: 11370 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void MissionStarted(int index, BaseMission.MissionInstance instance)
	{
	}

	// Token: 0x06002C6B RID: 11371 RVA: 0x0010A18D File Offset: 0x0010838D
	public virtual void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		instance.objectiveStatuses[index].started = true;
		playerFor.MissionDirty(true);
	}

	// Token: 0x06002C6C RID: 11372 RVA: 0x0010A1A4 File Offset: 0x001083A4
	public bool IsStarted(int index, BaseMission.MissionInstance instance)
	{
		return instance.objectiveStatuses[index].started;
	}

	// Token: 0x06002C6D RID: 11373 RVA: 0x0010A1B3 File Offset: 0x001083B3
	public bool CanProgress(int index, BaseMission.MissionInstance instance)
	{
		return !instance.GetMission().objectives[index].onlyProgressIfStarted || this.IsStarted(index, instance);
	}

	// Token: 0x06002C6E RID: 11374 RVA: 0x0010A1D4 File Offset: 0x001083D4
	public bool ShouldObjectiveStart(int index, BaseMission.MissionInstance instance)
	{
		foreach (int num in instance.GetMission().objectives[index].startAfterCompletedObjectives)
		{
			if (!instance.objectiveStatuses[num].completed && !instance.objectiveStatuses[num].failed)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002C6F RID: 11375 RVA: 0x0010A227 File Offset: 0x00108427
	public bool IsCompleted(int index, BaseMission.MissionInstance instance)
	{
		return instance.objectiveStatuses[index].completed || instance.objectiveStatuses[index].failed;
	}

	// Token: 0x06002C70 RID: 11376 RVA: 0x0010A247 File Offset: 0x00108447
	public virtual bool ShouldThink(int index, BaseMission.MissionInstance instance)
	{
		return !this.IsCompleted(index, instance);
	}

	// Token: 0x06002C71 RID: 11377 RVA: 0x0010A254 File Offset: 0x00108454
	public virtual void CompleteObjective(int index, BaseMission.MissionInstance instance, BasePlayer playerFor)
	{
		instance.objectiveStatuses[index].completed = true;
		instance.GetMission().OnObjectiveCompleted(index, instance, playerFor);
	}

	// Token: 0x06002C72 RID: 11378 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, string identifier, float amount)
	{
	}

	// Token: 0x06002C73 RID: 11379 RVA: 0x0010A272 File Offset: 0x00108472
	public virtual void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		if (this.ShouldObjectiveStart(index, instance) && !this.IsStarted(index, instance))
		{
			this.ObjectiveStarted(assignee, index, instance);
		}
	}
}
