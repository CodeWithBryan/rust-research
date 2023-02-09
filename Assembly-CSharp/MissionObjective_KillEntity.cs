using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020005EE RID: 1518
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Kill")]
public class MissionObjective_KillEntity : MissionObjective
{
	// Token: 0x06002C81 RID: 11393 RVA: 0x0010A291 File Offset: 0x00108491
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002C82 RID: 11394 RVA: 0x0010A440 File Offset: 0x00108640
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
		if (type == BaseMission.MissionEventType.KILL_ENTITY)
		{
			string[] array = this.targetPrefabIDs;
			int i = 0;
			while (i < array.Length)
			{
				if (array[i] == identifier)
				{
					instance.objectiveStatuses[index].genericInt1 += (int)amount;
					if (instance.objectiveStatuses[index].genericInt1 >= this.numToKill)
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

	// Token: 0x06002C83 RID: 11395 RVA: 0x0010A4D8 File Offset: 0x001086D8
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		if (this.shouldUpdateMissionLocation && base.IsStarted(index, instance) && Time.realtimeSinceStartup > this.nextLocationUpdateTime)
		{
			this.nextLocationUpdateTime = Time.realtimeSinceStartup + 1f;
			foreach (string s in this.targetPrefabIDs)
			{
				uint num = 0U;
				uint.TryParse(s, out num);
				List<BaseCombatEntity> list = Pool.GetList<BaseCombatEntity>();
				Vis.Entities<BaseCombatEntity>(assignee.transform.position, 20f, list, 133120, QueryTriggerInteraction.Collide);
				int num2 = -1;
				float num3 = float.PositiveInfinity;
				for (int j = 0; j < list.Count; j++)
				{
					BaseCombatEntity baseCombatEntity = list[j];
					if (baseCombatEntity.IsAlive() && baseCombatEntity.prefabID == num)
					{
						float num4 = Vector3.Distance(baseCombatEntity.transform.position, assignee.transform.position);
						if (num4 < num3)
						{
							num2 = j;
							num3 = num4;
						}
					}
				}
				if (num2 != -1)
				{
					instance.missionLocation = list[num2].transform.position;
					assignee.MissionDirty(true);
					Pool.FreeList<BaseCombatEntity>(ref list);
					break;
				}
				Pool.FreeList<BaseCombatEntity>(ref list);
			}
		}
		base.Think(index, instance, assignee, delta);
	}

	// Token: 0x04002447 RID: 9287
	public string[] targetPrefabIDs;

	// Token: 0x04002448 RID: 9288
	public int numToKill;

	// Token: 0x04002449 RID: 9289
	public bool shouldUpdateMissionLocation;

	// Token: 0x0400244A RID: 9290
	private float nextLocationUpdateTime;
}
