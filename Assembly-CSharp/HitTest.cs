using System;
using UnityEngine;

// Token: 0x020004F0 RID: 1264
public class HitTest
{
	// Token: 0x06002812 RID: 10258 RVA: 0x000F5650 File Offset: 0x000F3850
	public Vector3 HitPointWorld()
	{
		if (this.HitEntity != null)
		{
			Transform transform = this.HitTransform;
			if (!transform)
			{
				transform = this.HitEntity.transform;
			}
			return transform.TransformPoint(this.HitPoint);
		}
		return this.HitPoint;
	}

	// Token: 0x06002813 RID: 10259 RVA: 0x000F569C File Offset: 0x000F389C
	public Vector3 HitNormalWorld()
	{
		if (this.HitEntity != null)
		{
			Transform transform = this.HitTransform;
			if (!transform)
			{
				transform = this.HitEntity.transform;
			}
			return transform.TransformDirection(this.HitNormal);
		}
		return this.HitNormal;
	}

	// Token: 0x06002814 RID: 10260 RVA: 0x000F56E8 File Offset: 0x000F38E8
	public void Clear()
	{
		this.type = HitTest.Type.Generic;
		this.AttackRay = default(Ray);
		this.Radius = 0f;
		this.Forgiveness = 0f;
		this.MaxDistance = 0f;
		this.RayHit = default(RaycastHit);
		this.MultiHit = false;
		this.BestHit = false;
		this.DidHit = false;
		this.damageProperties = null;
		this.gameObject = null;
		this.collider = null;
		this.ignoreEntity = null;
		this.HitEntity = null;
		this.HitPoint = default(Vector3);
		this.HitNormal = default(Vector3);
		this.HitDistance = 0f;
		this.HitTransform = null;
		this.HitPart = 0U;
		this.HitMaterial = null;
	}

	// Token: 0x0400204E RID: 8270
	public HitTest.Type type;

	// Token: 0x0400204F RID: 8271
	public Ray AttackRay;

	// Token: 0x04002050 RID: 8272
	public float Radius;

	// Token: 0x04002051 RID: 8273
	public float Forgiveness;

	// Token: 0x04002052 RID: 8274
	public float MaxDistance;

	// Token: 0x04002053 RID: 8275
	public RaycastHit RayHit;

	// Token: 0x04002054 RID: 8276
	public bool MultiHit;

	// Token: 0x04002055 RID: 8277
	public bool BestHit;

	// Token: 0x04002056 RID: 8278
	public bool DidHit;

	// Token: 0x04002057 RID: 8279
	public DamageProperties damageProperties;

	// Token: 0x04002058 RID: 8280
	public GameObject gameObject;

	// Token: 0x04002059 RID: 8281
	public Collider collider;

	// Token: 0x0400205A RID: 8282
	public BaseEntity ignoreEntity;

	// Token: 0x0400205B RID: 8283
	public BaseEntity HitEntity;

	// Token: 0x0400205C RID: 8284
	public Vector3 HitPoint;

	// Token: 0x0400205D RID: 8285
	public Vector3 HitNormal;

	// Token: 0x0400205E RID: 8286
	public float HitDistance;

	// Token: 0x0400205F RID: 8287
	public Transform HitTransform;

	// Token: 0x04002060 RID: 8288
	public uint HitPart;

	// Token: 0x04002061 RID: 8289
	public string HitMaterial;

	// Token: 0x02000CE4 RID: 3300
	public enum Type
	{
		// Token: 0x04004431 RID: 17457
		Generic,
		// Token: 0x04004432 RID: 17458
		ProjectileEffect,
		// Token: 0x04004433 RID: 17459
		Projectile,
		// Token: 0x04004434 RID: 17460
		MeleeAttack,
		// Token: 0x04004435 RID: 17461
		Use
	}
}
