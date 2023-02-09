using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000AE6 RID: 2790
	[Serializable]
	public class ModularVehicleSocket
	{
		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x0600431E RID: 17182 RVA: 0x001862D4 File Offset: 0x001844D4
		public Vector3 WorldPosition
		{
			get
			{
				return this.socketTransform.position;
			}
		}

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x0600431F RID: 17183 RVA: 0x001862E1 File Offset: 0x001844E1
		public Quaternion WorldRotation
		{
			get
			{
				return this.socketTransform.rotation;
			}
		}

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x06004320 RID: 17184 RVA: 0x001862EE File Offset: 0x001844EE
		public ModularVehicleSocket.SocketWheelType WheelType
		{
			get
			{
				return this.wheelType;
			}
		}

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x06004321 RID: 17185 RVA: 0x001862F6 File Offset: 0x001844F6
		public ModularVehicleSocket.SocketLocationType LocationType
		{
			get
			{
				return this.locationType;
			}
		}

		// Token: 0x06004322 RID: 17186 RVA: 0x00186300 File Offset: 0x00184500
		public bool ShouldBeActive(ConditionalSocketSettings modelSettings)
		{
			bool flag = true;
			if (modelSettings.restrictOnLocation)
			{
				ConditionalSocketSettings.LocationCondition locationRestriction = modelSettings.locationRestriction;
				switch (this.LocationType)
				{
				case ModularVehicleSocket.SocketLocationType.Middle:
					flag = (locationRestriction == ConditionalSocketSettings.LocationCondition.Middle || locationRestriction == ConditionalSocketSettings.LocationCondition.NotFront || locationRestriction == ConditionalSocketSettings.LocationCondition.NotBack);
					break;
				case ModularVehicleSocket.SocketLocationType.Front:
					flag = (locationRestriction == ConditionalSocketSettings.LocationCondition.Front || locationRestriction == ConditionalSocketSettings.LocationCondition.NotBack || locationRestriction == ConditionalSocketSettings.LocationCondition.NotMiddle);
					break;
				case ModularVehicleSocket.SocketLocationType.Back:
					flag = (locationRestriction == ConditionalSocketSettings.LocationCondition.Back || locationRestriction == ConditionalSocketSettings.LocationCondition.NotFront || locationRestriction == ConditionalSocketSettings.LocationCondition.NotMiddle);
					break;
				}
			}
			if (flag && modelSettings.restrictOnWheel)
			{
				flag = (this.WheelType == modelSettings.wheelRestriction);
			}
			return flag;
		}

		// Token: 0x04003BB3 RID: 15283
		[SerializeField]
		private Transform socketTransform;

		// Token: 0x04003BB4 RID: 15284
		[SerializeField]
		private ModularVehicleSocket.SocketWheelType wheelType;

		// Token: 0x04003BB5 RID: 15285
		[SerializeField]
		private ModularVehicleSocket.SocketLocationType locationType;

		// Token: 0x02000F2E RID: 3886
		public enum SocketWheelType
		{
			// Token: 0x04004D92 RID: 19858
			NoWheel,
			// Token: 0x04004D93 RID: 19859
			ForwardWheel,
			// Token: 0x04004D94 RID: 19860
			BackWheel
		}

		// Token: 0x02000F2F RID: 3887
		public enum SocketLocationType
		{
			// Token: 0x04004D96 RID: 19862
			Middle,
			// Token: 0x04004D97 RID: 19863
			Front,
			// Token: 0x04004D98 RID: 19864
			Back
		}
	}
}
