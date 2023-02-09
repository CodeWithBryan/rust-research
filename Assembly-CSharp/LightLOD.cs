using System;
using UnityEngine;

// Token: 0x020008CD RID: 2253
public class LightLOD : MonoBehaviour, ILOD, IClientComponent
{
	// Token: 0x06003636 RID: 13878 RVA: 0x000C335A File Offset: 0x000C155A
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}

	// Token: 0x0400312D RID: 12589
	public float DistanceBias;

	// Token: 0x0400312E RID: 12590
	public bool ToggleLight;

	// Token: 0x0400312F RID: 12591
	public bool ToggleShadows = true;
}
