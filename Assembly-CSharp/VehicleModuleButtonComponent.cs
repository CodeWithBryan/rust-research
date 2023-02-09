using System;
using UnityEngine;

// Token: 0x0200047B RID: 1147
public abstract class VehicleModuleButtonComponent : MonoBehaviour
{
	// Token: 0x06002557 RID: 9559
	public abstract void ServerUse(BasePlayer player, BaseVehicleModule parentModule);

	// Token: 0x04001DEC RID: 7660
	public string interactionColliderName = "MyCollider";

	// Token: 0x04001DED RID: 7661
	public SoundDefinition pressSoundDef;
}
