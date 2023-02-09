using System;
using UnityEngine;

// Token: 0x02000936 RID: 2358
public class AlternateAttack : StateMachineBehaviour
{
	// Token: 0x06003819 RID: 14361 RVA: 0x0014BE70 File Offset: 0x0014A070
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.random)
		{
			string stateName = this.targetTransitions[UnityEngine.Random.Range(0, this.targetTransitions.Length)];
			animator.Play(stateName, layerIndex, 0f);
			return;
		}
		int integer = animator.GetInteger("lastAttack");
		string stateName2 = this.targetTransitions[integer % this.targetTransitions.Length];
		animator.Play(stateName2, layerIndex, 0f);
		if (!this.dontIncrement)
		{
			animator.SetInteger("lastAttack", integer + 1);
		}
	}

	// Token: 0x04003220 RID: 12832
	public bool random;

	// Token: 0x04003221 RID: 12833
	public bool dontIncrement;

	// Token: 0x04003222 RID: 12834
	public string[] targetTransitions;
}
