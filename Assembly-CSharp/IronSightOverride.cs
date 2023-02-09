using System;
using UnityEngine;

// Token: 0x02000939 RID: 2361
public class IronSightOverride : MonoBehaviour
{
	// Token: 0x04003237 RID: 12855
	public IronsightAimPoint aimPoint;

	// Token: 0x04003238 RID: 12856
	public float fieldOfViewOffset = -20f;

	// Token: 0x04003239 RID: 12857
	public float zoomFactor = -1f;

	// Token: 0x0400323A RID: 12858
	[Tooltip("If set to 1, the FOV is set to what this override is set to. If set to 0.5 it's half way between the weapon iconsights default and this scope.")]
	public float fovBias = 0.5f;
}
