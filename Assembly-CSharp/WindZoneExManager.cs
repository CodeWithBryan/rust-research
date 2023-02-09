using System;
using UnityEngine;

// Token: 0x0200070A RID: 1802
[ExecuteInEditMode]
[RequireComponent(typeof(WindZone))]
public class WindZoneExManager : MonoBehaviour
{
	// Token: 0x0400287E RID: 10366
	public float maxAccumMain = 4f;

	// Token: 0x0400287F RID: 10367
	public float maxAccumTurbulence = 4f;

	// Token: 0x04002880 RID: 10368
	public float globalMainScale = 1f;

	// Token: 0x04002881 RID: 10369
	public float globalTurbulenceScale = 1f;

	// Token: 0x04002882 RID: 10370
	public Transform testPosition;

	// Token: 0x02000DE8 RID: 3560
	private enum TestMode
	{
		// Token: 0x0400485A RID: 18522
		Disabled,
		// Token: 0x0400485B RID: 18523
		Low
	}
}
