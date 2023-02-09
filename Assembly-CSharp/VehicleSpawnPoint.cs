using System;
using UnityEngine;

// Token: 0x0200054E RID: 1358
public class VehicleSpawnPoint : SpaceCheckingSpawnPoint
{
	// Token: 0x06002950 RID: 10576 RVA: 0x000FAC7A File Offset: 0x000F8E7A
	public override void ObjectSpawned(SpawnPointInstance instance)
	{
		base.ObjectSpawned(instance);
		VehicleSpawnPoint.AddStartingFuel(instance.gameObject.ToBaseEntity() as BaseVehicle);
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x000FAC98 File Offset: 0x000F8E98
	public static void AddStartingFuel(BaseVehicle vehicle)
	{
		if (vehicle == null)
		{
			return;
		}
		EntityFuelSystem fuelSystem = vehicle.GetFuelSystem();
		if (fuelSystem != null)
		{
			fuelSystem.AddStartingFuel((float)vehicle.StartingFuelUnits());
		}
	}
}
