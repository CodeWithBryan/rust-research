using System;
using UnityEngine;

// Token: 0x02000478 RID: 1144
public class ModularCarSeat : MouseSteerableSeat
{
	// Token: 0x06002550 RID: 9552 RVA: 0x000E9A20 File Offset: 0x000E7C20
	public override bool CanSwapToThis(BasePlayer player)
	{
		if (this.associatedSeatingModule.DoorsAreLockable)
		{
			ModularCar modularCar = this.associatedSeatingModule.Vehicle as ModularCar;
			if (modularCar != null)
			{
				return modularCar.PlayerCanUseThis(player, ModularCarCodeLock.LockType.Door);
			}
		}
		return true;
	}

	// Token: 0x06002551 RID: 9553 RVA: 0x000E9A5E File Offset: 0x000E7C5E
	public override float GetComfort()
	{
		return this.providesComfort;
	}

	// Token: 0x04001DE2 RID: 7650
	[SerializeField]
	private Vector3 leftFootIKPos;

	// Token: 0x04001DE3 RID: 7651
	[SerializeField]
	private Vector3 rightFootIKPos;

	// Token: 0x04001DE4 RID: 7652
	[SerializeField]
	private Vector3 leftHandIKPos;

	// Token: 0x04001DE5 RID: 7653
	[SerializeField]
	private Vector3 rightHandIKPos;

	// Token: 0x04001DE6 RID: 7654
	public float providesComfort;

	// Token: 0x04001DE7 RID: 7655
	public VehicleModuleSeating associatedSeatingModule;
}
