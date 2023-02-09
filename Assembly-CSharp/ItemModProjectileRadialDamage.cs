using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020005CB RID: 1483
public class ItemModProjectileRadialDamage : ItemModProjectileMod
{
	// Token: 0x06002BE2 RID: 11234 RVA: 0x001076A8 File Offset: 0x001058A8
	public override void ServerProjectileHit(HitInfo info)
	{
		if (this.effect.isValid)
		{
			Effect.server.Run(this.effect.resourcePath, info.HitPositionWorld, info.HitNormalWorld, null, false);
		}
		List<BaseCombatEntity> list = Pool.GetList<BaseCombatEntity>();
		List<BaseCombatEntity> list2 = Pool.GetList<BaseCombatEntity>();
		Vis.Entities<BaseCombatEntity>(info.HitPositionWorld, this.radius, list2, 1236478737, QueryTriggerInteraction.Collide);
		foreach (BaseCombatEntity baseCombatEntity in list2)
		{
			if (baseCombatEntity.isServer && !list.Contains(baseCombatEntity) && (!(baseCombatEntity == info.HitEntity) || !this.ignoreHitObject))
			{
				baseCombatEntity.CenterPoint();
				Vector3 a = baseCombatEntity.ClosestPoint(info.HitPositionWorld);
				float num = Vector3.Distance(a, info.HitPositionWorld) / this.radius;
				if (num <= 1f)
				{
					float num2 = 1f - num;
					if (baseCombatEntity.IsVisibleAndCanSee(info.HitPositionWorld - info.ProjectileVelocity.normalized * 0.1f, float.PositiveInfinity) && baseCombatEntity.IsVisibleAndCanSee(info.HitPositionWorld - (a - info.HitPositionWorld).normalized * 0.1f, float.PositiveInfinity))
					{
						list.Add(baseCombatEntity);
						baseCombatEntity.OnAttacked(new HitInfo(info.Initiator, baseCombatEntity, this.damage.type, this.damage.amount * num2));
					}
				}
			}
		}
		Pool.FreeList<BaseCombatEntity>(ref list);
		Pool.FreeList<BaseCombatEntity>(ref list2);
	}

	// Token: 0x040023B0 RID: 9136
	public float radius = 0.5f;

	// Token: 0x040023B1 RID: 9137
	public DamageTypeEntry damage;

	// Token: 0x040023B2 RID: 9138
	public GameObjectRef effect;

	// Token: 0x040023B3 RID: 9139
	public bool ignoreHitObject = true;
}
