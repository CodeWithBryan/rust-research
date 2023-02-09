using System;

// Token: 0x020005E4 RID: 1508
public interface IMissionEntityListener
{
	// Token: 0x06002C50 RID: 11344
	void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance);

	// Token: 0x06002C51 RID: 11345
	void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance);
}
