using System;
using UnityEngine;

// Token: 0x02000495 RID: 1173
public abstract class VehicleChassisVisuals<T> : MonoBehaviour where T : BaseVehicle, VehicleChassisVisuals<T>.IClientWheelUser
{
	// Token: 0x02000CC3 RID: 3267
	public interface IClientWheelUser
	{
		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x06004D62 RID: 19810
		Vector3 Velocity { get; }

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x06004D63 RID: 19811
		float DriveWheelVelocity { get; }

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x06004D64 RID: 19812
		float SteerAngle { get; }

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x06004D65 RID: 19813
		float MaxSteerAngle { get; }

		// Token: 0x06004D66 RID: 19814
		float GetThrottleInput();
	}
}
