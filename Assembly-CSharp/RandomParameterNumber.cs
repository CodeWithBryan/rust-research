using System;
using UnityEngine;

// Token: 0x0200093D RID: 2365
public class RandomParameterNumber : StateMachineBehaviour
{
	// Token: 0x0600382A RID: 14378 RVA: 0x0014C3E8 File Offset: 0x0014A5E8
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int num = UnityEngine.Random.Range(this.min, this.max);
		int num2 = 0;
		while (this.last == num && this.preventRepetition && num2 < 100)
		{
			num = UnityEngine.Random.Range(this.min, this.max);
			num2++;
		}
		animator.SetInteger(this.parameterName, num);
		this.last = num;
	}

	// Token: 0x0400324C RID: 12876
	public string parameterName;

	// Token: 0x0400324D RID: 12877
	public int min;

	// Token: 0x0400324E RID: 12878
	public int max;

	// Token: 0x0400324F RID: 12879
	public bool preventRepetition;

	// Token: 0x04003250 RID: 12880
	private int last;
}
