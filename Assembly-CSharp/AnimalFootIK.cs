using System;
using UnityEngine;

// Token: 0x0200026F RID: 623
public class AnimalFootIK : MonoBehaviour
{
	// Token: 0x06001BC9 RID: 7113 RVA: 0x000C1060 File Offset: 0x000BF260
	public bool GroundSample(Vector3 origin, out RaycastHit hit)
	{
		return Physics.Raycast(origin + Vector3.up * 0.5f, Vector3.down, out hit, 1f, 455155969);
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x000059DD File Offset: 0x00003BDD
	public void Start()
	{
	}

	// Token: 0x06001BCB RID: 7115 RVA: 0x000C1091 File Offset: 0x000BF291
	public AvatarIKGoal GoalFromIndex(int index)
	{
		if (index == 0)
		{
			return AvatarIKGoal.LeftHand;
		}
		if (index == 1)
		{
			return AvatarIKGoal.RightHand;
		}
		if (index == 2)
		{
			return AvatarIKGoal.LeftFoot;
		}
		if (index == 3)
		{
			return AvatarIKGoal.RightFoot;
		}
		return AvatarIKGoal.LeftHand;
	}

	// Token: 0x06001BCC RID: 7116 RVA: 0x000C10AC File Offset: 0x000BF2AC
	private void OnAnimatorIK(int layerIndex)
	{
		Debug.Log("animal ik!");
		for (int i = 0; i < 4; i++)
		{
			Transform transform = this.Feet[i];
			AvatarIKGoal goal = this.GoalFromIndex(i);
			Vector3 up = Vector3.up;
			Vector3 vector = transform.transform.position;
			float value = this.animator.GetIKPositionWeight(goal);
			RaycastHit raycastHit;
			if (this.GroundSample(transform.transform.position - Vector3.down * this.actualFootOffset, out raycastHit))
			{
				Vector3 normal = raycastHit.normal;
				vector = raycastHit.point;
				float value2 = Vector3.Distance(transform.transform.position - Vector3.down * this.actualFootOffset, vector);
				value = 1f - Mathf.InverseLerp(this.minWeightDistance, this.maxWeightDistance, value2);
				this.animator.SetIKPosition(goal, vector + Vector3.up * this.actualFootOffset);
			}
			else
			{
				value = 0f;
			}
			this.animator.SetIKPositionWeight(goal, value);
		}
	}

	// Token: 0x040014D9 RID: 5337
	public Transform[] Feet;

	// Token: 0x040014DA RID: 5338
	public Animator animator;

	// Token: 0x040014DB RID: 5339
	public float maxWeightDistance = 0.1f;

	// Token: 0x040014DC RID: 5340
	public float minWeightDistance = 0.025f;

	// Token: 0x040014DD RID: 5341
	public float actualFootOffset = 0.01f;
}
