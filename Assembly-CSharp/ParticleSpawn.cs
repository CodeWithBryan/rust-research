using System;
using UnityEngine;

// Token: 0x02000651 RID: 1617
public class ParticleSpawn : SingletonComponent<ParticleSpawn>, IClientComponent
{
	// Token: 0x1700038F RID: 911
	// (get) Token: 0x06002E15 RID: 11797 RVA: 0x00114805 File Offset: 0x00112A05
	// (set) Token: 0x06002E16 RID: 11798 RVA: 0x0011480D File Offset: 0x00112A0D
	public Vector3 Origin { get; private set; }

	// Token: 0x040025C3 RID: 9667
	public GameObjectRef[] Prefabs;

	// Token: 0x040025C4 RID: 9668
	public int PatchCount = 8;

	// Token: 0x040025C5 RID: 9669
	public int PatchSize = 100;
}
