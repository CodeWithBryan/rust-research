using System;
using UnityEngine;

// Token: 0x0200093E RID: 2366
public class RandomParameterNumberFloat : StateMachineBehaviour
{
	// Token: 0x0600382C RID: 14380 RVA: 0x0014C44B File Offset: 0x0014A64B
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (string.IsNullOrEmpty(this.parameterName))
		{
			return;
		}
		animator.SetFloat(this.parameterName, Mathf.Floor(UnityEngine.Random.Range((float)this.min, (float)this.max + 0.5f)));
	}

	// Token: 0x04003251 RID: 12881
	public string parameterName;

	// Token: 0x04003252 RID: 12882
	public int min;

	// Token: 0x04003253 RID: 12883
	public int max;
}
