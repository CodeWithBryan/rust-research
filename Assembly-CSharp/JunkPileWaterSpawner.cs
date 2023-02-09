using System;

// Token: 0x0200015F RID: 351
public class JunkPileWaterSpawner : SpawnGroup
{
	// Token: 0x06001669 RID: 5737 RVA: 0x000AA5CA File Offset: 0x000A87CA
	protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
		base.PostSpawnProcess(entity, spawnPoint);
		if (this.attachToParent != null)
		{
			entity.SetParent(this.attachToParent, true, false);
		}
	}

	// Token: 0x04000F55 RID: 3925
	public BaseEntity attachToParent;
}
