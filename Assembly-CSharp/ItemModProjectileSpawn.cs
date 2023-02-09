using System;
using ConVar;
using UnityEngine;

// Token: 0x020005CC RID: 1484
public class ItemModProjectileSpawn : ItemModProjectile
{
	// Token: 0x06002BE4 RID: 11236 RVA: 0x00107884 File Offset: 0x00105A84
	public override void ServerProjectileHit(HitInfo info)
	{
		for (int i = 0; i < this.numToCreateChances; i++)
		{
			if (this.createOnImpact.isValid && UnityEngine.Random.Range(0f, 1f) < this.createOnImpactChance)
			{
				Vector3 hitPositionWorld = info.HitPositionWorld;
				Vector3 pointStart = info.PointStart;
				Vector3 normalized = info.ProjectileVelocity.normalized;
				Vector3 normalized2 = info.HitNormalWorld.normalized;
				Vector3 vector = hitPositionWorld - normalized * 0.1f;
				Quaternion rotation = Quaternion.LookRotation(-normalized);
				int layerMask = ConVar.AntiHack.projectile_terraincheck ? 10551296 : 2162688;
				if (GamePhysics.LineOfSight(pointStart, vector, layerMask, null))
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(this.createOnImpact.resourcePath, default(Vector3), default(Quaternion), true);
					if (baseEntity)
					{
						baseEntity.transform.position = vector;
						baseEntity.transform.rotation = rotation;
						baseEntity.Spawn();
						if (this.spreadAngle > 0f)
						{
							Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(this.spreadAngle, normalized2, true);
							baseEntity.SetVelocity(modifiedAimConeDirection * UnityEngine.Random.Range(1f, 3f));
						}
					}
				}
			}
		}
		base.ServerProjectileHit(info);
	}

	// Token: 0x040023B4 RID: 9140
	public float createOnImpactChance;

	// Token: 0x040023B5 RID: 9141
	public GameObjectRef createOnImpact = new GameObjectRef();

	// Token: 0x040023B6 RID: 9142
	public float spreadAngle = 30f;

	// Token: 0x040023B7 RID: 9143
	public float spreadVelocityMin = 1f;

	// Token: 0x040023B8 RID: 9144
	public float spreadVelocityMax = 3f;

	// Token: 0x040023B9 RID: 9145
	public int numToCreateChances = 1;
}
