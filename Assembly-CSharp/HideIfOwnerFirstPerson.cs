using System;
using UnityEngine;

// Token: 0x020003AB RID: 939
public class HideIfOwnerFirstPerson : EntityComponent<BaseEntity>, IClientComponent, IViewModeChanged
{
	// Token: 0x04001939 RID: 6457
	public GameObject[] disableGameObjects;

	// Token: 0x0400193A RID: 6458
	public bool worldModelEffect;
}
