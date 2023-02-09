using System;
using UnityEngine;

// Token: 0x020001EC RID: 492
public class EnvironmentFish : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04001247 RID: 4679
	public Animator animator;

	// Token: 0x04001248 RID: 4680
	public float minSpeed;

	// Token: 0x04001249 RID: 4681
	public float maxSpeed;

	// Token: 0x0400124A RID: 4682
	public float idealDepth;

	// Token: 0x0400124B RID: 4683
	public float minTurnSpeed = 0.5f;

	// Token: 0x0400124C RID: 4684
	public float maxTurnSpeed = 180f;

	// Token: 0x0400124D RID: 4685
	public Vector3 destination;

	// Token: 0x0400124E RID: 4686
	public Vector3 spawnPos;

	// Token: 0x0400124F RID: 4687
	public Vector3 idealLocalScale = Vector3.one;
}
