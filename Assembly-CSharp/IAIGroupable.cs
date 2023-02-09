using System;
using UnityEngine;

// Token: 0x02000371 RID: 881
public interface IAIGroupable
{
	// Token: 0x06001EFF RID: 7935
	bool AddMember(IAIGroupable member);

	// Token: 0x06001F00 RID: 7936
	void RemoveMember(IAIGroupable member);

	// Token: 0x06001F01 RID: 7937
	void JoinGroup(IAIGroupable leader, BaseEntity leaderEntity);

	// Token: 0x06001F02 RID: 7938
	void SetGroupRoamRootPosition(Vector3 rootPos);

	// Token: 0x06001F03 RID: 7939
	bool InGroup();

	// Token: 0x06001F04 RID: 7940
	void LeaveGroup();

	// Token: 0x06001F05 RID: 7941
	void SetUngrouped();
}
