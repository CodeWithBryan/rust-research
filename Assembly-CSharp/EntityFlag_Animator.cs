using System;
using UnityEngine;

// Token: 0x020003A4 RID: 932
public class EntityFlag_Animator : EntityFlag_Toggle
{
	// Token: 0x04001922 RID: 6434
	public Animator TargetAnimator;

	// Token: 0x04001923 RID: 6435
	public string ParamName = string.Empty;

	// Token: 0x04001924 RID: 6436
	public EntityFlag_Animator.AnimatorMode AnimationMode;

	// Token: 0x04001925 RID: 6437
	public float FloatOnState;

	// Token: 0x04001926 RID: 6438
	public float FloatOffState;

	// Token: 0x04001927 RID: 6439
	public int IntegerOnState;

	// Token: 0x04001928 RID: 6440
	public int IntegerOffState;

	// Token: 0x02000C6B RID: 3179
	public enum AnimatorMode
	{
		// Token: 0x0400423C RID: 16956
		Bool,
		// Token: 0x0400423D RID: 16957
		Float,
		// Token: 0x0400423E RID: 16958
		Trigger,
		// Token: 0x0400423F RID: 16959
		Integer
	}
}
