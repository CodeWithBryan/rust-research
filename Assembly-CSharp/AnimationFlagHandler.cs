using System;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class AnimationFlagHandler : MonoBehaviour
{
	// Token: 0x06001646 RID: 5702 RVA: 0x000A98B8 File Offset: 0x000A7AB8
	public void SetBoolTrue(string name)
	{
		this.animator.SetBool(name, true);
	}

	// Token: 0x06001647 RID: 5703 RVA: 0x000A98C7 File Offset: 0x000A7AC7
	public void SetBoolFalse(string name)
	{
		this.animator.SetBool(name, false);
	}

	// Token: 0x04000F22 RID: 3874
	public Animator animator;
}
