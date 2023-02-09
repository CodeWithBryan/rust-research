using System;

// Token: 0x02000545 RID: 1349
public interface ISpawnPointUser
{
	// Token: 0x06002914 RID: 10516
	void ObjectSpawned(SpawnPointInstance instance);

	// Token: 0x06002915 RID: 10517
	void ObjectRetired(SpawnPointInstance instance);
}
