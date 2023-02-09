using System;
using UnityEngine;

// Token: 0x0200018E RID: 398
public class HorseSpawner : VehicleSpawner
{
	// Token: 0x0600174D RID: 5965 RVA: 0x000AE838 File Offset: 0x000ACA38
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.RespawnHorse), UnityEngine.Random.Range(0f, 4f), this.respawnDelay, this.respawnDelayVariance);
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x000AE86D File Offset: 0x000ACA6D
	public override int GetOccupyLayer()
	{
		return 2048;
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x000AE874 File Offset: 0x000ACA74
	public void RespawnHorse()
	{
		if (base.GetVehicleOccupying() != null)
		{
			return;
		}
		BaseVehicle baseVehicle = base.SpawnVehicle(this.objectsToSpawn[0].prefabToSpawn.resourcePath, null);
		if (this.spawnForSale)
		{
			baseVehicle.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		}
	}

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x06001750 RID: 5968 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool LogAnalytics
	{
		get
		{
			return false;
		}
	}

	// Token: 0x04001055 RID: 4181
	public float respawnDelay = 10f;

	// Token: 0x04001056 RID: 4182
	public float respawnDelayVariance = 5f;

	// Token: 0x04001057 RID: 4183
	public bool spawnForSale = true;
}
