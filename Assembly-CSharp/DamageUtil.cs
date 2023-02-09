using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020002E4 RID: 740
public static class DamageUtil
{
	// Token: 0x06001D32 RID: 7474 RVA: 0x000C74E8 File Offset: 0x000C56E8
	public static void RadiusDamage(BaseEntity attackingPlayer, BaseEntity weaponPrefab, Vector3 pos, float minradius, float radius, List<DamageTypeEntry> damage, int layers, bool useLineOfSight)
	{
		using (TimeWarning.New("DamageUtil.RadiusDamage", 0))
		{
			List<HitInfo> list = Pool.GetList<HitInfo>();
			List<BaseEntity> list2 = Pool.GetList<BaseEntity>();
			List<BaseEntity> list3 = Pool.GetList<BaseEntity>();
			Vis.Entities<BaseEntity>(pos, radius, list3, layers, QueryTriggerInteraction.Collide);
			for (int i = 0; i < list3.Count; i++)
			{
				BaseEntity baseEntity = list3[i];
				if (baseEntity.isServer && !list2.Contains(baseEntity))
				{
					Vector3 vector = baseEntity.ClosestPoint(pos);
					float num = Mathf.Clamp01((Vector3.Distance(vector, pos) - minradius) / (radius - minradius));
					if (num <= 1f)
					{
						float amount = 1f - num;
						if (!useLineOfSight || baseEntity.IsVisible(pos, float.PositiveInfinity))
						{
							HitInfo hitInfo = new HitInfo();
							hitInfo.Initiator = attackingPlayer;
							hitInfo.WeaponPrefab = weaponPrefab;
							hitInfo.damageTypes.Add(damage);
							hitInfo.damageTypes.ScaleAll(amount);
							hitInfo.HitPositionWorld = vector;
							hitInfo.HitNormalWorld = (pos - vector).normalized;
							hitInfo.PointStart = pos;
							hitInfo.PointEnd = hitInfo.HitPositionWorld;
							list.Add(hitInfo);
							list2.Add(baseEntity);
						}
					}
				}
			}
			for (int j = 0; j < list2.Count; j++)
			{
				BaseEntity baseEntity2 = list2[j];
				HitInfo info = list[j];
				baseEntity2.OnAttacked(info);
			}
			Pool.FreeList<HitInfo>(ref list);
			Pool.FreeList<BaseEntity>(ref list2);
			Pool.FreeList<BaseEntity>(ref list3);
		}
	}
}
