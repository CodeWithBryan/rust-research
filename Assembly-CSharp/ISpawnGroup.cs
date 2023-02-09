using System;

// Token: 0x02000544 RID: 1348
public interface ISpawnGroup
{
	// Token: 0x0600290F RID: 10511
	void Clear();

	// Token: 0x06002910 RID: 10512
	void Fill();

	// Token: 0x06002911 RID: 10513
	void SpawnInitial();

	// Token: 0x06002912 RID: 10514
	void SpawnRepeating();

	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06002913 RID: 10515
	int currentPopulation { get; }
}
