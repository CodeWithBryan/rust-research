using System;
using UnityEngine;

// Token: 0x02000472 RID: 1138
public class ModularCarChassisVisuals : VehicleChassisVisuals<ModularCar>, IClientComponent
{
	// Token: 0x04001DCC RID: 7628
	public Transform frontAxle;

	// Token: 0x04001DCD RID: 7629
	public Transform rearAxle;

	// Token: 0x04001DCE RID: 7630
	public ModularCarChassisVisuals.Steering steering;

	// Token: 0x04001DCF RID: 7631
	public ModularCarChassisVisuals.LookAtTarget transmission;

	// Token: 0x02000CAF RID: 3247
	[Serializable]
	public class Steering
	{
		// Token: 0x04004372 RID: 17266
		public Transform steerL;

		// Token: 0x04004373 RID: 17267
		public Transform steerR;

		// Token: 0x04004374 RID: 17268
		public ModularCarChassisVisuals.LookAtTarget steerRodL;

		// Token: 0x04004375 RID: 17269
		public ModularCarChassisVisuals.LookAtTarget steerRodR;

		// Token: 0x04004376 RID: 17270
		public ModularCarChassisVisuals.LookAtTarget steeringArm;
	}

	// Token: 0x02000CB0 RID: 3248
	[Serializable]
	public class LookAtTarget
	{
		// Token: 0x04004377 RID: 17271
		public Transform aim;

		// Token: 0x04004378 RID: 17272
		public Transform target;

		// Token: 0x04004379 RID: 17273
		public Vector3 angleAdjust;
	}
}
