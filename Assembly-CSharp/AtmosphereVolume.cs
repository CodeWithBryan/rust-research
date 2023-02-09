using System;
using UnityEngine;

// Token: 0x020006E3 RID: 1763
[ExecuteInEditMode]
public class AtmosphereVolume : MonoBehaviour
{
	// Token: 0x04002802 RID: 10242
	public float MaxVisibleDistance = 750f;

	// Token: 0x04002803 RID: 10243
	public float BoundsAttenuationDecay = 5f;

	// Token: 0x04002804 RID: 10244
	public FogSettings FogSettings;
}
