using System;
using UnityEngine;

// Token: 0x0200031F RID: 799
public class EmissionToggle : MonoBehaviour, IClientComponent
{
	// Token: 0x04001754 RID: 5972
	private Color emissionColor;

	// Token: 0x04001755 RID: 5973
	public Renderer[] targetRenderers;

	// Token: 0x04001756 RID: 5974
	public int materialIndex = -1;
}
