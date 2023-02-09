using System;
using UnityEngine;

// Token: 0x02000946 RID: 2374
public class ViewmodelAspectOffset : MonoBehaviour
{
	// Token: 0x0400326B RID: 12907
	public Vector3 OffsetAmount = Vector3.zero;

	// Token: 0x0400326C RID: 12908
	[Tooltip("What aspect ratio should we start moving the viewmodel? 16:9 = 1.7, 21:9 = 2.3")]
	public float aspectCutoff = 2f;
}
