using System;
using UnityEngine;

// Token: 0x02000471 RID: 1137
[Serializable]
public class ModularCarCentralLockingSwitch : VehicleModuleButtonComponent
{
	// Token: 0x06002529 RID: 9513 RVA: 0x000E93E8 File Offset: 0x000E75E8
	public override void ServerUse(BasePlayer player, BaseVehicleModule parentModule)
	{
		ModularCar modularCar;
		if ((modularCar = (parentModule.Vehicle as ModularCar)) != null)
		{
			modularCar.CarLock.ToggleCentralLocking();
		}
	}

	// Token: 0x04001DC9 RID: 7625
	public Transform centralLockingSwitch;

	// Token: 0x04001DCA RID: 7626
	public Vector3 switchOffPos;

	// Token: 0x04001DCB RID: 7627
	public Vector3 switchOnPos;
}
