using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000AEA RID: 2794
	public class ItemModVehicleChassis : ItemMod, VehicleModuleInformationPanel.IVehicleModuleInfo
	{
		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x0600432C RID: 17196 RVA: 0x0018650F File Offset: 0x0018470F
		public int SocketsTaken
		{
			get
			{
				return this.socketsTaken;
			}
		}

		// Token: 0x04003BC0 RID: 15296
		public GameObjectRef entityPrefab;

		// Token: 0x04003BC1 RID: 15297
		[Range(1f, 6f)]
		public int socketsTaken = 1;
	}
}
