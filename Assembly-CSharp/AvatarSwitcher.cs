using System;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class AvatarSwitcher : StateMachineBehaviour
{
	// Token: 0x06001BCE RID: 7118 RVA: 0x000C11E9 File Offset: 0x000BF3E9
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (this.ToApply != null)
		{
			animator.avatar = this.ToApply;
			animator.Play(stateInfo.shortNameHash, layerIndex);
		}
	}

	// Token: 0x040014DE RID: 5342
	public Avatar ToApply;
}
