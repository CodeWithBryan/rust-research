using System;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class LootPanelVendingMachine : LootPanel, IVendingMachineInterface
{
	// Token: 0x04000E26 RID: 3622
	public GameObjectRef sellOrderPrefab;

	// Token: 0x04000E27 RID: 3623
	public GameObject sellOrderContainer;

	// Token: 0x04000E28 RID: 3624
	public GameObject busyOverlayPrefab;

	// Token: 0x04000E29 RID: 3625
	private GameObject busyOverlayInstance;
}
