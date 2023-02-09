using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000315 RID: 789
public class Effect : EffectData
{
	// Token: 0x06001DC2 RID: 7618 RVA: 0x000CB046 File Offset: 0x000C9246
	public Effect()
	{
	}

	// Token: 0x06001DC3 RID: 7619 RVA: 0x000CB04E File Offset: 0x000C924E
	public Effect(string effectName, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		this.Init(Effect.Type.Generic, posWorld, normWorld, sourceConnection);
		this.pooledString = effectName;
	}

	// Token: 0x06001DC4 RID: 7620 RVA: 0x000CB068 File Offset: 0x000C9268
	public Effect(string effectName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		this.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
		this.pooledString = effectName;
	}

	// Token: 0x06001DC5 RID: 7621 RVA: 0x000CB088 File Offset: 0x000C9288
	public void Init(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		this.Clear();
		this.type = (uint)fxtype;
		this.attached = true;
		this.origin = posLocal;
		this.normal = normLocal;
		this.gameObject = null;
		this.Up = Vector3.zero;
		if (ent != null && !ent.IsValid())
		{
			Debug.LogWarning("Effect.Init - invalid entity");
		}
		this.entity = (ent.IsValid() ? ent.net.ID : 0U);
		this.source = ((sourceConnection != null) ? sourceConnection.userid : 0UL);
		this.bone = boneID;
	}

	// Token: 0x06001DC6 RID: 7622 RVA: 0x000CB120 File Offset: 0x000C9320
	public void Init(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		this.Clear();
		this.type = (uint)fxtype;
		this.attached = false;
		this.worldPos = posWorld;
		this.worldNrm = normWorld;
		this.gameObject = null;
		this.Up = Vector3.zero;
		this.entity = 0U;
		this.origin = this.worldPos;
		this.normal = this.worldNrm;
		this.bone = 0U;
		this.source = ((sourceConnection != null) ? sourceConnection.userid : 0UL);
	}

	// Token: 0x06001DC7 RID: 7623 RVA: 0x000CB19C File Offset: 0x000C939C
	public void Clear()
	{
		this.worldPos = Vector3.zero;
		this.worldNrm = Vector3.zero;
		this.attached = false;
		this.transform = null;
		this.gameObject = null;
		this.pooledString = null;
		this.broadcast = false;
	}

	// Token: 0x0400173E RID: 5950
	public Vector3 Up;

	// Token: 0x0400173F RID: 5951
	public Vector3 worldPos;

	// Token: 0x04001740 RID: 5952
	public Vector3 worldNrm;

	// Token: 0x04001741 RID: 5953
	public bool attached;

	// Token: 0x04001742 RID: 5954
	public Transform transform;

	// Token: 0x04001743 RID: 5955
	public GameObject gameObject;

	// Token: 0x04001744 RID: 5956
	public string pooledString;

	// Token: 0x04001745 RID: 5957
	public bool broadcast;

	// Token: 0x04001746 RID: 5958
	private static Effect reusableInstace = new Effect();

	// Token: 0x02000C59 RID: 3161
	public enum Type : uint
	{
		// Token: 0x04004201 RID: 16897
		Generic,
		// Token: 0x04004202 RID: 16898
		Projectile,
		// Token: 0x04004203 RID: 16899
		GenericGlobal
	}

	// Token: 0x02000C5A RID: 3162
	public static class client
	{
		// Token: 0x06004CB8 RID: 19640 RVA: 0x000059DD File Offset: 0x00003BDD
		public static void Run(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
		}

		// Token: 0x06004CB9 RID: 19641 RVA: 0x00195EBA File Offset: 0x001940BA
		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
			string.IsNullOrEmpty(strName);
		}

		// Token: 0x06004CBA RID: 19642 RVA: 0x000059DD File Offset: 0x00003BDD
		public static void Run(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Vector3 up = default(Vector3))
		{
		}

		// Token: 0x06004CBB RID: 19643 RVA: 0x00195EBA File Offset: 0x001940BA
		public static void Run(string strName, Vector3 posWorld = default(Vector3), Vector3 normWorld = default(Vector3), Vector3 up = default(Vector3), Effect.Type overrideType = Effect.Type.Generic)
		{
			string.IsNullOrEmpty(strName);
		}

		// Token: 0x06004CBC RID: 19644 RVA: 0x00195EBA File Offset: 0x001940BA
		public static void Run(string strName, GameObject obj)
		{
			string.IsNullOrEmpty(strName);
		}

		// Token: 0x06004CBD RID: 19645 RVA: 0x00195EC4 File Offset: 0x001940C4
		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			if (info.HitEntity.IsValid())
			{
				Effect.client.Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal + info.HitNormalLocal * 0.1f, info.HitNormalLocal);
				return;
			}
			Effect.client.Run(effectName, info.HitPositionWorld + info.HitNormalWorld * 0.1f, info.HitNormalWorld, default(Vector3), Effect.Type.Generic);
		}

		// Token: 0x06004CBE RID: 19646 RVA: 0x00195F44 File Offset: 0x00194144
		public static void ImpactEffect(HitInfo info)
		{
			if (!info.DoHitEffects)
			{
				return;
			}
			string materialName = StringPool.Get(info.HitMaterial);
			string strName = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), materialName);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), materialName);
			if (TerrainMeta.WaterMap != null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < TerrainMeta.WaterMap.GetHeight(info.HitPositionWorld) && WaterLevel.Test(info.HitPositionWorld, false, null))
			{
				return;
			}
			if (info.HitEntity.IsValid())
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					strName = impactEffect.resourcePath;
				}
				Effect.client.Run(strName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				if (info.DoDecals)
				{
					Effect.client.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				}
			}
			else
			{
				Effect.Type overrideType = Effect.Type.Generic;
				Effect.client.Run(strName, info.HitPositionWorld, info.HitNormalWorld, default(Vector3), overrideType);
				Effect.client.Run(decal, info.HitPositionWorld, info.HitNormalWorld, default(Vector3), overrideType);
			}
			if (info.WeaponPrefab)
			{
				BaseMelee baseMelee = info.WeaponPrefab as BaseMelee;
				if (baseMelee != null)
				{
					string strikeEffectPath = baseMelee.GetStrikeEffectPath(materialName);
					if (info.HitEntity.IsValid())
					{
						Effect.client.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
					}
					else
					{
						Effect.client.Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, default(Vector3), Effect.Type.Generic);
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}
	}

	// Token: 0x02000C5B RID: 3163
	public static class server
	{
		// Token: 0x06004CBF RID: 19647 RVA: 0x00196139 File Offset: 0x00194339
		public static void Run(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
			Effect.reusableInstace.Init(fxtype, ent, boneID, posLocal, normLocal, sourceConnection);
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004CC0 RID: 19648 RVA: 0x00196163 File Offset: 0x00194363
		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
			if (string.IsNullOrEmpty(strName))
			{
				return;
			}
			Effect.reusableInstace.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
			Effect.reusableInstace.pooledString = strName;
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004CC1 RID: 19649 RVA: 0x001961A1 File Offset: 0x001943A1
		public static void Run(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null, bool broadcast = false)
		{
			Effect.reusableInstace.Init(fxtype, posWorld, normWorld, sourceConnection);
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004CC2 RID: 19650 RVA: 0x001961C7 File Offset: 0x001943C7
		public static void Run(string strName, Vector3 posWorld = default(Vector3), Vector3 normWorld = default(Vector3), Connection sourceConnection = null, bool broadcast = false)
		{
			if (string.IsNullOrEmpty(strName))
			{
				return;
			}
			Effect.reusableInstace.Init(Effect.Type.Generic, posWorld, normWorld, sourceConnection);
			Effect.reusableInstace.pooledString = strName;
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004CC3 RID: 19651 RVA: 0x00196204 File Offset: 0x00194404
		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			if (info.HitEntity.IsValid())
			{
				Effect.server.Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
				return;
			}
			Effect.server.Run(effectName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
		}

		// Token: 0x06004CC4 RID: 19652 RVA: 0x00196260 File Offset: 0x00194460
		public static void ImpactEffect(HitInfo info)
		{
			if (!info.DoHitEffects)
			{
				return;
			}
			string materialName = StringPool.Get(info.HitMaterial);
			if (TerrainMeta.WaterMap != null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < TerrainMeta.WaterMap.GetHeight(info.HitPositionWorld) && WaterLevel.Test(info.HitPositionWorld, false, null))
			{
				return;
			}
			string strName = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), materialName);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), materialName);
			if (info.HitEntity.IsValid())
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					strName = impactEffect.resourcePath;
				}
				Bounds bounds = info.HitEntity.bounds;
				float num = info.HitEntity.BoundsPadding();
				bounds.extents += new Vector3(num, num, num);
				if (!bounds.Contains(info.HitPositionLocal))
				{
					BasePlayer initiatorPlayer = info.InitiatorPlayer;
					if (initiatorPlayer != null && initiatorPlayer.GetType() == typeof(BasePlayer))
					{
						float num2 = Mathf.Sqrt(bounds.SqrDistance(info.HitPositionLocal));
						AntiHack.Log(initiatorPlayer, AntiHackType.EffectHack, string.Format("Tried to run an impact effect outside of entity '{0}' bounds by {1}m", info.HitEntity.ShortPrefabName, num2));
					}
					return;
				}
				Effect.server.Run(strName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
				Effect.server.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
			}
			else
			{
				Effect.server.Run(strName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
				Effect.server.Run(decal, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
			}
			if (info.WeaponPrefab)
			{
				BaseMelee baseMelee = info.WeaponPrefab as BaseMelee;
				if (baseMelee != null)
				{
					string strikeEffectPath = baseMelee.GetStrikeEffectPath(materialName);
					if (info.HitEntity.IsValid())
					{
						Effect.server.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
					}
					else
					{
						Effect.server.Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}
	}
}
