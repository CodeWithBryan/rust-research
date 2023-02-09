using System;
using UnityEngine;

// Token: 0x020003F0 RID: 1008
public interface IIgniteable
{
	// Token: 0x060021E8 RID: 8680
	void Ignite(Vector3 fromPos);

	// Token: 0x060021E9 RID: 8681
	bool CanIgnite();
}
