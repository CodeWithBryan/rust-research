using System;
using UnityEngine;

// Token: 0x0200046F RID: 1135
public class GlassPane : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04001DC4 RID: 7620
	public Renderer glassRendereer;

	// Token: 0x04001DC5 RID: 7621
	[SerializeField]
	private BaseVehicleModule module;

	// Token: 0x04001DC6 RID: 7622
	[SerializeField]
	private float showFullDamageAt = 0.75f;
}
