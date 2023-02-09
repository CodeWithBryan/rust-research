using System;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class PlayerIdleAnimationRandomiser : StateMachineBehaviour
{
	// Token: 0x04000031 RID: 49
	public int MaxValue = 3;

	// Token: 0x04000032 RID: 50
	public static int Param_Random = Animator.StringToHash("Random Idle");

	// Token: 0x04000033 RID: 51
	private TimeSince lastRandomisation;
}
