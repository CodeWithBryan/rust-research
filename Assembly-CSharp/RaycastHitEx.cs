using System;
using Rust;
using UnityEngine;

// Token: 0x020008F3 RID: 2291
public static class RaycastHitEx
{
	// Token: 0x060036A4 RID: 13988 RVA: 0x00145305 File Offset: 0x00143505
	public static Transform GetTransform(this RaycastHit hit)
	{
		return hit.transform;
	}

	// Token: 0x060036A5 RID: 13989 RVA: 0x0014530E File Offset: 0x0014350E
	public static Rigidbody GetRigidbody(this RaycastHit hit)
	{
		return hit.rigidbody;
	}

	// Token: 0x060036A6 RID: 13990 RVA: 0x00145317 File Offset: 0x00143517
	public static Collider GetCollider(this RaycastHit hit)
	{
		return hit.collider;
	}

	// Token: 0x060036A7 RID: 13991 RVA: 0x00145320 File Offset: 0x00143520
	public static BaseEntity GetEntity(this RaycastHit hit)
	{
		return hit.collider.ToBaseEntity();
	}

	// Token: 0x060036A8 RID: 13992 RVA: 0x0014532E File Offset: 0x0014352E
	public static bool IsOnLayer(this RaycastHit hit, Layer rustLayer)
	{
		return hit.collider != null && hit.collider.gameObject.IsOnLayer(rustLayer);
	}

	// Token: 0x060036A9 RID: 13993 RVA: 0x00145353 File Offset: 0x00143553
	public static bool IsOnLayer(this RaycastHit hit, int layer)
	{
		return hit.collider != null && hit.collider.gameObject.IsOnLayer(layer);
	}
}
