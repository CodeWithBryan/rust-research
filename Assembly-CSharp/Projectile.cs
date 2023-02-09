using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020002C1 RID: 705
public class Projectile : BaseMonoBehaviour
{
	// Token: 0x06001CA4 RID: 7332 RVA: 0x000C49C0 File Offset: 0x000C2BC0
	public void CalculateDamage(HitInfo info, Projectile.Modifier mod, float scale)
	{
		float num = this.damageMultipliers.Lerp(mod.distanceOffset + mod.distanceScale * this.damageDistances.x, mod.distanceOffset + mod.distanceScale * this.damageDistances.y, info.ProjectileDistance);
		float num2 = scale * (mod.damageOffset + mod.damageScale * num);
		foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
		{
			info.damageTypes.Add(damageTypeEntry.type, damageTypeEntry.amount * num2);
		}
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				" Projectile damage: ",
				info.damageTypes.Total(),
				" (scalar=",
				num2,
				")"
			}));
		}
	}

	// Token: 0x06001CA5 RID: 7333 RVA: 0x000C4AC8 File Offset: 0x000C2CC8
	public static uint FleshMaterialID()
	{
		if (Projectile._fleshMaterialID == 0U)
		{
			Projectile._fleshMaterialID = StringPool.Get("flesh");
		}
		return Projectile._fleshMaterialID;
	}

	// Token: 0x06001CA6 RID: 7334 RVA: 0x000C4AE5 File Offset: 0x000C2CE5
	public static uint WaterMaterialID()
	{
		if (Projectile._waterMaterialID == 0U)
		{
			Projectile._waterMaterialID = StringPool.Get("Water");
		}
		return Projectile._waterMaterialID;
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x000C4B02 File Offset: 0x000C2D02
	public static bool IsWaterMaterial(string hitMaterial)
	{
		if (Projectile.cachedWaterString == 0U)
		{
			Projectile.cachedWaterString = StringPool.Get("Water");
		}
		return StringPool.Get(hitMaterial) == Projectile.cachedWaterString;
	}

	// Token: 0x06001CA8 RID: 7336 RVA: 0x000C4B2C File Offset: 0x000C2D2C
	public static bool ShouldStopProjectile(RaycastHit hit)
	{
		BaseEntity entity = hit.GetEntity();
		return !(entity != null) || entity.ShouldBlockProjectiles();
	}

	// Token: 0x0400160C RID: 5644
	public const float moveDeltaTime = 0.03125f;

	// Token: 0x0400160D RID: 5645
	public const float lifeTime = 8f;

	// Token: 0x0400160E RID: 5646
	[Header("Attributes")]
	public Vector3 initialVelocity;

	// Token: 0x0400160F RID: 5647
	public float drag;

	// Token: 0x04001610 RID: 5648
	public float gravityModifier = 1f;

	// Token: 0x04001611 RID: 5649
	public float thickness;

	// Token: 0x04001612 RID: 5650
	[Tooltip("This projectile will raycast for this many units, and then become a projectile. This is typically done for bullets.")]
	public float initialDistance;

	// Token: 0x04001613 RID: 5651
	[Header("Impact Rules")]
	public bool remainInWorld;

	// Token: 0x04001614 RID: 5652
	[Range(0f, 1f)]
	public float stickProbability = 1f;

	// Token: 0x04001615 RID: 5653
	[Range(0f, 1f)]
	public float breakProbability;

	// Token: 0x04001616 RID: 5654
	[Range(0f, 1f)]
	public float conditionLoss;

	// Token: 0x04001617 RID: 5655
	[Range(0f, 1f)]
	public float ricochetChance;

	// Token: 0x04001618 RID: 5656
	public float penetrationPower = 1f;

	// Token: 0x04001619 RID: 5657
	[Header("Damage")]
	public DamageProperties damageProperties;

	// Token: 0x0400161A RID: 5658
	[Horizontal(2, -1)]
	public MinMax damageDistances = new MinMax(10f, 100f);

	// Token: 0x0400161B RID: 5659
	[Horizontal(2, -1)]
	public MinMax damageMultipliers = new MinMax(1f, 0.8f);

	// Token: 0x0400161C RID: 5660
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	// Token: 0x0400161D RID: 5661
	[Header("Rendering")]
	public ScaleRenderer rendererToScale;

	// Token: 0x0400161E RID: 5662
	public ScaleRenderer firstPersonRenderer;

	// Token: 0x0400161F RID: 5663
	public bool createDecals = true;

	// Token: 0x04001620 RID: 5664
	[Header("Effects")]
	public bool doDefaultHitEffects = true;

	// Token: 0x04001621 RID: 5665
	[Header("Audio")]
	public SoundDefinition flybySound;

	// Token: 0x04001622 RID: 5666
	public float flybySoundDistance = 7f;

	// Token: 0x04001623 RID: 5667
	public SoundDefinition closeFlybySound;

	// Token: 0x04001624 RID: 5668
	public float closeFlybyDistance = 3f;

	// Token: 0x04001625 RID: 5669
	[Header("Tumble")]
	public float tumbleSpeed;

	// Token: 0x04001626 RID: 5670
	public Vector3 tumbleAxis = Vector3.right;

	// Token: 0x04001627 RID: 5671
	[Header("Swim")]
	public Vector3 swimScale;

	// Token: 0x04001628 RID: 5672
	public Vector3 swimSpeed;

	// Token: 0x04001629 RID: 5673
	[NonSerialized]
	public BasePlayer owner;

	// Token: 0x0400162A RID: 5674
	[NonSerialized]
	public AttackEntity sourceWeaponPrefab;

	// Token: 0x0400162B RID: 5675
	[NonSerialized]
	public Projectile sourceProjectilePrefab;

	// Token: 0x0400162C RID: 5676
	[NonSerialized]
	public ItemModProjectile mod;

	// Token: 0x0400162D RID: 5677
	[NonSerialized]
	public int projectileID;

	// Token: 0x0400162E RID: 5678
	[NonSerialized]
	public int seed;

	// Token: 0x0400162F RID: 5679
	[NonSerialized]
	public bool clientsideEffect;

	// Token: 0x04001630 RID: 5680
	[NonSerialized]
	public bool clientsideAttack;

	// Token: 0x04001631 RID: 5681
	[NonSerialized]
	public float integrity = 1f;

	// Token: 0x04001632 RID: 5682
	[NonSerialized]
	public float maxDistance = float.PositiveInfinity;

	// Token: 0x04001633 RID: 5683
	[NonSerialized]
	public Projectile.Modifier modifier = Projectile.Modifier.Default;

	// Token: 0x04001634 RID: 5684
	[NonSerialized]
	public bool invisible;

	// Token: 0x04001635 RID: 5685
	private static uint _fleshMaterialID;

	// Token: 0x04001636 RID: 5686
	private static uint _waterMaterialID;

	// Token: 0x04001637 RID: 5687
	private static uint cachedWaterString;

	// Token: 0x02000C47 RID: 3143
	public struct Modifier
	{
		// Token: 0x0400419C RID: 16796
		public float damageScale;

		// Token: 0x0400419D RID: 16797
		public float damageOffset;

		// Token: 0x0400419E RID: 16798
		public float distanceScale;

		// Token: 0x0400419F RID: 16799
		public float distanceOffset;

		// Token: 0x040041A0 RID: 16800
		public static Projectile.Modifier Default = new Projectile.Modifier
		{
			damageScale = 1f,
			damageOffset = 0f,
			distanceScale = 1f,
			distanceOffset = 0f
		};
	}
}
