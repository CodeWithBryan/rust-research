using System;

// Token: 0x0200082D RID: 2093
public class VehicleModuleInformationPanel : ItemInformationPanel
{
	// Token: 0x04002E7E RID: 11902
	public ItemStatValue socketsDisplay;

	// Token: 0x04002E7F RID: 11903
	public ItemStatValue hpDisplay;

	// Token: 0x02000E2C RID: 3628
	public interface IVehicleModuleInfo
	{
		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x06005002 RID: 20482
		int SocketsTaken { get; }
	}
}
