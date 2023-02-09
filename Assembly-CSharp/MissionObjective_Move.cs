using System;
using UnityEngine;

// Token: 0x020005EF RID: 1519
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Move")]
public class MissionObjective_Move : MissionObjective
{
	// Token: 0x06002C85 RID: 11397 RVA: 0x0010A60F File Offset: 0x0010880F
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
		instance.missionLocation = instance.GetMissionPoint(this.positionName, playerFor);
		playerFor.MissionDirty(true);
	}

	// Token: 0x06002C86 RID: 11398 RVA: 0x0010A634 File Offset: 0x00108834
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
		if (!this.ShouldThink(index, instance))
		{
			return;
		}
		Vector3 missionPoint = instance.GetMissionPoint(this.positionName, assignee);
		if ((this.use2D ? Vector3Ex.Distance2D(missionPoint, assignee.transform.position) : Vector3.Distance(missionPoint, assignee.transform.position)) <= this.distForCompletion)
		{
			this.CompleteObjective(index, instance, assignee);
			assignee.MissionDirty(true);
		}
	}

	// Token: 0x0400244B RID: 9291
	public string positionName = "default";

	// Token: 0x0400244C RID: 9292
	public float distForCompletion = 3f;

	// Token: 0x0400244D RID: 9293
	public bool use2D;
}
