using System;
using UnityEngine;

// Token: 0x02000374 RID: 884
public interface IPet
{
	// Token: 0x06001F13 RID: 7955
	bool IsPet();

	// Token: 0x06001F14 RID: 7956
	void SetPetOwner(BasePlayer player);

	// Token: 0x06001F15 RID: 7957
	bool IsOwnedBy(BasePlayer player);

	// Token: 0x06001F16 RID: 7958
	bool IssuePetCommand(PetCommandType cmd, int param, Ray? ray);
}
