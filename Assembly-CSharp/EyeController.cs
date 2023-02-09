using System;
using UnityEngine;

// Token: 0x02000321 RID: 801
public class EyeController : MonoBehaviour
{
	// Token: 0x04001763 RID: 5987
	public const float MaxLookDot = 0.8f;

	// Token: 0x04001764 RID: 5988
	public bool debug;

	// Token: 0x04001765 RID: 5989
	public Transform LeftEye;

	// Token: 0x04001766 RID: 5990
	public Transform RightEye;

	// Token: 0x04001767 RID: 5991
	public Transform EyeTransform;

	// Token: 0x04001768 RID: 5992
	public Vector3 Fudge = new Vector3(0f, 90f, 0f);

	// Token: 0x04001769 RID: 5993
	public Vector3 FlickerRange;

	// Token: 0x0400176A RID: 5994
	private Transform Focus;

	// Token: 0x0400176B RID: 5995
	private float FocusUpdateTime;
}
