using System;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class NpcWalkAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x040014E8 RID: 5352
	public Vector3 HipFudge = new Vector3(-90f, 0f, 90f);

	// Token: 0x040014E9 RID: 5353
	public BaseNpc Npc;

	// Token: 0x040014EA RID: 5354
	public Animator Animator;

	// Token: 0x040014EB RID: 5355
	public Transform HipBone;

	// Token: 0x040014EC RID: 5356
	public Transform LookBone;

	// Token: 0x040014ED RID: 5357
	public bool UpdateWalkSpeed = true;

	// Token: 0x040014EE RID: 5358
	public bool UpdateFacingDirection = true;

	// Token: 0x040014EF RID: 5359
	public bool UpdateGroundNormal = true;

	// Token: 0x040014F0 RID: 5360
	public Transform alignmentRoot;

	// Token: 0x040014F1 RID: 5361
	public bool LaggyAss = true;

	// Token: 0x040014F2 RID: 5362
	public bool LookAtTarget;

	// Token: 0x040014F3 RID: 5363
	public float MaxLaggyAssRotation = 70f;

	// Token: 0x040014F4 RID: 5364
	public float MaxWalkAnimSpeed = 25f;

	// Token: 0x040014F5 RID: 5365
	public bool UseDirectionBlending;

	// Token: 0x040014F6 RID: 5366
	public bool useTurnPosing;

	// Token: 0x040014F7 RID: 5367
	public float turnPoseScale = 0.5f;

	// Token: 0x040014F8 RID: 5368
	public float laggyAssLerpScale = 15f;

	// Token: 0x040014F9 RID: 5369
	public bool skeletonChainInverted;
}
