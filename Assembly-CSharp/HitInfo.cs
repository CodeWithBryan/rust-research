using System;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020004F1 RID: 1265
public class HitInfo
{
	// Token: 0x06002816 RID: 10262 RVA: 0x000F57A5 File Offset: 0x000F39A5
	public bool IsProjectile()
	{
		return this.ProjectileID != 0;
	}

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06002817 RID: 10263 RVA: 0x000F57B0 File Offset: 0x000F39B0
	public global::BasePlayer InitiatorPlayer
	{
		get
		{
			if (!this.Initiator)
			{
				return null;
			}
			return this.Initiator.ToPlayer();
		}
	}

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06002818 RID: 10264 RVA: 0x000F57CC File Offset: 0x000F39CC
	public Vector3 attackNormal
	{
		get
		{
			return (this.PointEnd - this.PointStart).normalized;
		}
	}

	// Token: 0x1700032D RID: 813
	// (get) Token: 0x06002819 RID: 10265 RVA: 0x000F57F2 File Offset: 0x000F39F2
	public bool hasDamage
	{
		get
		{
			return this.damageTypes.Total() > 0f;
		}
	}

	// Token: 0x0600281A RID: 10266 RVA: 0x000F5806 File Offset: 0x000F3A06
	public HitInfo()
	{
	}

	// Token: 0x0600281B RID: 10267 RVA: 0x000F583C File Offset: 0x000F3A3C
	public HitInfo(global::BaseEntity attacker, global::BaseEntity target, DamageType type, float damageAmount, Vector3 vhitPosition)
	{
		this.Initiator = attacker;
		this.HitEntity = target;
		this.HitPositionWorld = vhitPosition;
		if (attacker != null)
		{
			this.PointStart = attacker.transform.position;
		}
		this.damageTypes.Add(type, damageAmount);
	}

	// Token: 0x0600281C RID: 10268 RVA: 0x000F58B8 File Offset: 0x000F3AB8
	public HitInfo(global::BaseEntity attacker, global::BaseEntity target, DamageType type, float damageAmount) : this(attacker, target, type, damageAmount, target.transform.position)
	{
	}

	// Token: 0x0600281D RID: 10269 RVA: 0x000F58D0 File Offset: 0x000F3AD0
	public void LoadFromAttack(Attack attack, bool serverSide)
	{
		this.HitEntity = null;
		this.PointStart = attack.pointStart;
		this.PointEnd = attack.pointEnd;
		if (attack.hitID > 0U)
		{
			this.DidHit = true;
			if (serverSide)
			{
				this.HitEntity = (global::BaseNetworkable.serverEntities.Find(attack.hitID) as global::BaseEntity);
			}
			if (this.HitEntity)
			{
				this.HitBone = attack.hitBone;
				this.HitPart = attack.hitPartID;
			}
		}
		this.DidHit = true;
		this.HitPositionLocal = attack.hitPositionLocal;
		this.HitPositionWorld = attack.hitPositionWorld;
		this.HitNormalLocal = attack.hitNormalLocal.normalized;
		this.HitNormalWorld = attack.hitNormalWorld.normalized;
		this.HitMaterial = attack.hitMaterialID;
	}

	// Token: 0x1700032E RID: 814
	// (get) Token: 0x0600281E RID: 10270 RVA: 0x000F599C File Offset: 0x000F3B9C
	public bool isHeadshot
	{
		get
		{
			if (this.HitEntity == null)
			{
				return false;
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return false;
			}
			if (baseCombatEntity.skeletonProperties == null)
			{
				return false;
			}
			SkeletonProperties.BoneProperty boneProperty = baseCombatEntity.skeletonProperties.FindBone(this.HitBone);
			return boneProperty != null && boneProperty.area == HitArea.Head;
		}
	}

	// Token: 0x1700032F RID: 815
	// (get) Token: 0x0600281F RID: 10271 RVA: 0x000F5A00 File Offset: 0x000F3C00
	public Translate.Phrase bonePhrase
	{
		get
		{
			if (this.HitEntity == null)
			{
				return null;
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return null;
			}
			if (baseCombatEntity.skeletonProperties == null)
			{
				return null;
			}
			SkeletonProperties.BoneProperty boneProperty = baseCombatEntity.skeletonProperties.FindBone(this.HitBone);
			if (boneProperty == null)
			{
				return null;
			}
			return boneProperty.name;
		}
	}

	// Token: 0x17000330 RID: 816
	// (get) Token: 0x06002820 RID: 10272 RVA: 0x000F5A64 File Offset: 0x000F3C64
	public string boneName
	{
		get
		{
			Translate.Phrase bonePhrase = this.bonePhrase;
			if (bonePhrase != null)
			{
				return bonePhrase.english;
			}
			return "N/A";
		}
	}

	// Token: 0x17000331 RID: 817
	// (get) Token: 0x06002821 RID: 10273 RVA: 0x000F5A88 File Offset: 0x000F3C88
	public HitArea boneArea
	{
		get
		{
			if (this.HitEntity == null)
			{
				return (HitArea)(-1);
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return (HitArea)(-1);
			}
			return baseCombatEntity.SkeletonLookup(this.HitBone);
		}
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x000F5AC8 File Offset: 0x000F3CC8
	public Vector3 PositionOnRay(Vector3 position)
	{
		Ray ray = new Ray(this.PointStart, this.attackNormal);
		if (this.ProjectilePrefab == null)
		{
			return ray.ClosestPoint(position);
		}
		Sphere sphere = new Sphere(position, this.ProjectilePrefab.thickness);
		RaycastHit raycastHit;
		if (sphere.Trace(ray, out raycastHit, float.PositiveInfinity))
		{
			return raycastHit.point;
		}
		return position;
	}

	// Token: 0x06002823 RID: 10275 RVA: 0x000F5B2B File Offset: 0x000F3D2B
	public Vector3 HitPositionOnRay()
	{
		return this.PositionOnRay(this.HitPositionWorld);
	}

	// Token: 0x06002824 RID: 10276 RVA: 0x000F5B3C File Offset: 0x000F3D3C
	public bool IsNaNOrInfinity()
	{
		return this.PointStart.IsNaNOrInfinity() || this.PointEnd.IsNaNOrInfinity() || this.HitPositionWorld.IsNaNOrInfinity() || this.HitPositionLocal.IsNaNOrInfinity() || this.HitNormalWorld.IsNaNOrInfinity() || this.HitNormalLocal.IsNaNOrInfinity() || this.ProjectileVelocity.IsNaNOrInfinity() || float.IsNaN(this.ProjectileDistance) || float.IsInfinity(this.ProjectileDistance) || float.IsNaN(this.ProjectileIntegrity) || float.IsInfinity(this.ProjectileIntegrity) || float.IsNaN(this.ProjectileTravelTime) || float.IsInfinity(this.ProjectileTravelTime) || float.IsNaN(this.ProjectileTrajectoryMismatch) || float.IsInfinity(this.ProjectileTrajectoryMismatch);
	}

	// Token: 0x04002062 RID: 8290
	public global::BaseEntity Initiator;

	// Token: 0x04002063 RID: 8291
	public global::BaseEntity WeaponPrefab;

	// Token: 0x04002064 RID: 8292
	public AttackEntity Weapon;

	// Token: 0x04002065 RID: 8293
	public bool DoHitEffects = true;

	// Token: 0x04002066 RID: 8294
	public bool DoDecals = true;

	// Token: 0x04002067 RID: 8295
	public bool IsPredicting;

	// Token: 0x04002068 RID: 8296
	public bool UseProtection = true;

	// Token: 0x04002069 RID: 8297
	public Connection Predicted;

	// Token: 0x0400206A RID: 8298
	public bool DidHit;

	// Token: 0x0400206B RID: 8299
	public global::BaseEntity HitEntity;

	// Token: 0x0400206C RID: 8300
	public uint HitBone;

	// Token: 0x0400206D RID: 8301
	public uint HitPart;

	// Token: 0x0400206E RID: 8302
	public uint HitMaterial;

	// Token: 0x0400206F RID: 8303
	public Vector3 HitPositionWorld;

	// Token: 0x04002070 RID: 8304
	public Vector3 HitPositionLocal;

	// Token: 0x04002071 RID: 8305
	public Vector3 HitNormalWorld;

	// Token: 0x04002072 RID: 8306
	public Vector3 HitNormalLocal;

	// Token: 0x04002073 RID: 8307
	public Vector3 PointStart;

	// Token: 0x04002074 RID: 8308
	public Vector3 PointEnd;

	// Token: 0x04002075 RID: 8309
	public int ProjectileID;

	// Token: 0x04002076 RID: 8310
	public int ProjectileHits;

	// Token: 0x04002077 RID: 8311
	public float ProjectileDistance;

	// Token: 0x04002078 RID: 8312
	public float ProjectileIntegrity;

	// Token: 0x04002079 RID: 8313
	public float ProjectileTravelTime;

	// Token: 0x0400207A RID: 8314
	public float ProjectileTrajectoryMismatch;

	// Token: 0x0400207B RID: 8315
	public Vector3 ProjectileVelocity;

	// Token: 0x0400207C RID: 8316
	public Projectile ProjectilePrefab;

	// Token: 0x0400207D RID: 8317
	public PhysicMaterial material;

	// Token: 0x0400207E RID: 8318
	public DamageProperties damageProperties;

	// Token: 0x0400207F RID: 8319
	public DamageTypeList damageTypes = new DamageTypeList();

	// Token: 0x04002080 RID: 8320
	public bool CanGather;

	// Token: 0x04002081 RID: 8321
	public bool DidGather;

	// Token: 0x04002082 RID: 8322
	public float gatherScale = 1f;
}
