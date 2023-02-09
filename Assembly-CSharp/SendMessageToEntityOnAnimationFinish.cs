using System;
using UnityEngine;

// Token: 0x0200070E RID: 1806
public class SendMessageToEntityOnAnimationFinish : StateMachineBehaviour
{
	// Token: 0x060031E6 RID: 12774 RVA: 0x001322CC File Offset: 0x001304CC
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (0f + this.repeatRate > Time.time)
		{
			return;
		}
		if (animator.IsInTransition(layerIndex))
		{
			return;
		}
		if (stateInfo.normalizedTime < 1f)
		{
			return;
		}
		for (int i = 0; i < animator.layerCount; i++)
		{
			if (i != layerIndex)
			{
				if (animator.IsInTransition(i))
				{
					return;
				}
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
				if (currentAnimatorStateInfo.speed > 0f && currentAnimatorStateInfo.normalizedTime < 1f)
				{
					return;
				}
			}
		}
		BaseEntity baseEntity = animator.gameObject.ToBaseEntity();
		if (baseEntity)
		{
			baseEntity.SendMessage(this.messageToSendToEntity);
		}
	}

	// Token: 0x04002899 RID: 10393
	public string messageToSendToEntity;

	// Token: 0x0400289A RID: 10394
	public float repeatRate = 0.1f;

	// Token: 0x0400289B RID: 10395
	private const float lastMessageSent = 0f;
}
