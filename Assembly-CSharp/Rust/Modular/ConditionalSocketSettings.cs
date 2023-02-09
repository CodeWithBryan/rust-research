using System;

namespace Rust.Modular
{
	// Token: 0x02000AE0 RID: 2784
	[Serializable]
	public class ConditionalSocketSettings
	{
		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x060042E5 RID: 17125 RVA: 0x001856CB File Offset: 0x001838CB
		public bool HasSocketRestrictions
		{
			get
			{
				return this.restrictOnLocation || this.restrictOnWheel;
			}
		}

		// Token: 0x04003B8A RID: 15242
		public bool restrictOnLocation;

		// Token: 0x04003B8B RID: 15243
		public ConditionalSocketSettings.LocationCondition locationRestriction;

		// Token: 0x04003B8C RID: 15244
		public bool restrictOnWheel;

		// Token: 0x04003B8D RID: 15245
		public ModularVehicleSocket.SocketWheelType wheelRestriction;

		// Token: 0x02000F2B RID: 3883
		public enum LocationCondition
		{
			// Token: 0x04004D83 RID: 19843
			Middle,
			// Token: 0x04004D84 RID: 19844
			Front,
			// Token: 0x04004D85 RID: 19845
			Back,
			// Token: 0x04004D86 RID: 19846
			NotMiddle,
			// Token: 0x04004D87 RID: 19847
			NotFront,
			// Token: 0x04004D88 RID: 19848
			NotBack
		}
	}
}
