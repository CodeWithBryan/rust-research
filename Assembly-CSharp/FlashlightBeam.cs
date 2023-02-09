using System;
using UnityEngine;

// Token: 0x020001B4 RID: 436
public class FlashlightBeam : MonoBehaviour, IClientComponent
{
	// Token: 0x04001107 RID: 4359
	public Vector2 scrollDir;

	// Token: 0x04001108 RID: 4360
	public Vector3 localEndPoint = new Vector3(0f, 0f, 2f);

	// Token: 0x04001109 RID: 4361
	public LineRenderer beamRenderer;
}
