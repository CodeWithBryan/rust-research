using System;
using UnityEngine;

// Token: 0x020002D1 RID: 721
public class TriggerMovement : TriggerBase, IClientComponent
{
	// Token: 0x0400167C RID: 5756
	[Tooltip("If set, the entering object must have line of sight to this transform to be added, note this is only checked on entry")]
	public Transform losEyes;

	// Token: 0x0400167D RID: 5757
	public BaseEntity.MovementModify movementModify;
}
