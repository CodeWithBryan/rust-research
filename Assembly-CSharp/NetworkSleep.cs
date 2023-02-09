using System;
using UnityEngine;

// Token: 0x020008D6 RID: 2262
public class NetworkSleep : MonoBehaviour
{
	// Token: 0x0400314D RID: 12621
	public static int totalBehavioursDisabled;

	// Token: 0x0400314E RID: 12622
	public static int totalCollidersDisabled;

	// Token: 0x0400314F RID: 12623
	public Behaviour[] behaviours;

	// Token: 0x04003150 RID: 12624
	public Collider[] colliders;

	// Token: 0x04003151 RID: 12625
	internal int BehavioursDisabled;

	// Token: 0x04003152 RID: 12626
	internal int CollidersDisabled;
}
