using System;
using UnityEngine;

// Token: 0x020005E5 RID: 1509
public interface IMissionProvider
{
	// Token: 0x06002C52 RID: 11346
	uint ProviderID();

	// Token: 0x06002C53 RID: 11347
	Vector3 ProviderPosition();

	// Token: 0x06002C54 RID: 11348
	BaseEntity Entity();
}
