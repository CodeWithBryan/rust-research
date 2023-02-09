using System;
using UnityEngine;

// Token: 0x0200047E RID: 1150
public class VehicleLight : MonoBehaviour, IClientComponent
{
	// Token: 0x04001E03 RID: 7683
	public bool IsBrake;

	// Token: 0x04001E04 RID: 7684
	public GameObject toggleObject;

	// Token: 0x04001E05 RID: 7685
	public VehicleLight.LightRenderer[] renderers;

	// Token: 0x04001E06 RID: 7686
	[ColorUsage(true, true)]
	public Color lightOnColour;

	// Token: 0x04001E07 RID: 7687
	[ColorUsage(true, true)]
	public Color brakesOnColour;

	// Token: 0x02000CB4 RID: 3252
	[Serializable]
	public class LightRenderer
	{
		// Token: 0x04004387 RID: 17287
		public Renderer renderer;

		// Token: 0x04004388 RID: 17288
		public int matIndex;
	}
}
