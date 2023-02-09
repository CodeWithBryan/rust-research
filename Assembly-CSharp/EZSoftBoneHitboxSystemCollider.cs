using System;
using EZhex1991.EZSoftBone;
using UnityEngine;

// Token: 0x020002DD RID: 733
[RequireComponent(typeof(HitboxSystem))]
public class EZSoftBoneHitboxSystemCollider : EZSoftBoneColliderBase, IClientComponent
{
	// Token: 0x06001D10 RID: 7440 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void Collide(ref Vector3 position, float spacing)
	{
	}

	// Token: 0x0400169A RID: 5786
	public float radius = 2f;
}
