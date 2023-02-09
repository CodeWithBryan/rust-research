using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class GunTrap : StorageContainer
{
	// Token: 0x06000B79 RID: 2937 RVA: 0x00064750 File Offset: 0x00062950
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("GunTrap.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x00064790 File Offset: 0x00062990
	public override string Categorize()
	{
		return "GunTrap";
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x00064798 File Offset: 0x00062998
	public bool UseAmmo()
	{
		foreach (Item item in base.inventory.itemList)
		{
			if (item.info == this.ammoType && item.amount > 0)
			{
				item.UseItem(1);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x00064814 File Offset: 0x00062A14
	public void FireWeapon()
	{
		if (!this.UseAmmo())
		{
			return;
		}
		Effect.server.Run(this.gun_fire_effect.resourcePath, this, StringPool.Get(this.muzzlePos.gameObject.name), Vector3.zero, Vector3.zero, null, false);
		for (int i = 0; i < this.numPellets; i++)
		{
			this.FireBullet();
		}
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x00064874 File Offset: 0x00062A74
	public void FireBullet()
	{
		float damageAmount = 10f;
		Vector3 vector = this.muzzlePos.transform.position - this.muzzlePos.forward * 0.25f;
		Vector3 forward = this.muzzlePos.transform.forward;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection((float)this.aimCone, forward, true);
		Vector3 arg = vector + modifiedAimConeDirection * 300f;
		base.ClientRPC<Vector3>(null, "CLIENT_FireGun", arg);
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		int layerMask = 1219701505;
		GamePhysics.TraceAll(new Ray(vector, modifiedAimConeDirection), 0.1f, list, 300f, layerMask, QueryTriggerInteraction.UseGlobal, null);
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit hit = list[i];
			BaseEntity entity = hit.GetEntity();
			if (!(entity != null) || (!(entity == this) && !entity.EqualNetID(this)))
			{
				if (entity as BaseCombatEntity != null)
				{
					HitInfo info = new HitInfo(this, entity, DamageType.Bullet, damageAmount, hit.point);
					entity.OnAttacked(info);
					if (entity is BasePlayer || entity is BaseNpc)
					{
						Effect.server.ImpactEffect(new HitInfo
						{
							HitPositionWorld = hit.point,
							HitNormalWorld = -hit.normal,
							HitMaterial = StringPool.Get("Flesh")
						});
					}
				}
				if (!(entity != null) || entity.ShouldBlockProjectiles())
				{
					arg = hit.point;
					return;
				}
			}
		}
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x00064A00 File Offset: 0x00062C00
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.TriggerCheck), UnityEngine.Random.Range(0f, 1f), 0.5f, 0.1f);
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x00064A33 File Offset: 0x00062C33
	public void TriggerCheck()
	{
		if (this.CheckTrigger())
		{
			this.FireWeapon();
		}
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x00064A44 File Offset: 0x00062C44
	public bool CheckTrigger()
	{
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		HashSet<BaseEntity> entityContents = this.trigger.entityContents;
		bool flag = false;
		if (entityContents != null)
		{
			foreach (BaseEntity baseEntity in entityContents)
			{
				BasePlayer component = baseEntity.GetComponent<BasePlayer>();
				if (!component.IsSleeping() && component.IsAlive() && !component.IsBuildingAuthed())
				{
					list.Clear();
					GamePhysics.TraceAll(new Ray(component.eyes.position, (this.GetEyePosition() - component.eyes.position).normalized), 0f, list, 9f, 1218519297, QueryTriggerInteraction.UseGlobal, null);
					for (int i = 0; i < list.Count; i++)
					{
						BaseEntity entity = list[i].GetEntity();
						if (entity != null && (entity == this || entity.EqualNetID(this)))
						{
							flag = true;
							break;
						}
						if (!(entity != null) || entity.ShouldBlockProjectiles())
						{
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return flag;
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x00020A80 File Offset: 0x0001EC80
	public bool IsTriggered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x00064B8C File Offset: 0x00062D8C
	public Vector3 GetEyePosition()
	{
		return this.eyeTransform.position;
	}

	// Token: 0x04000752 RID: 1874
	public GameObjectRef gun_fire_effect;

	// Token: 0x04000753 RID: 1875
	public GameObjectRef bulletEffect;

	// Token: 0x04000754 RID: 1876
	public GameObjectRef triggeredEffect;

	// Token: 0x04000755 RID: 1877
	public Transform muzzlePos;

	// Token: 0x04000756 RID: 1878
	public Transform eyeTransform;

	// Token: 0x04000757 RID: 1879
	public int numPellets = 15;

	// Token: 0x04000758 RID: 1880
	public int aimCone = 30;

	// Token: 0x04000759 RID: 1881
	public float sensorRadius = 1.25f;

	// Token: 0x0400075A RID: 1882
	public ItemDefinition ammoType;

	// Token: 0x0400075B RID: 1883
	public TargetTrigger trigger;

	// Token: 0x02000B87 RID: 2951
	public static class GunTrapFlags
	{
		// Token: 0x04003EA0 RID: 16032
		public const BaseEntity.Flags Triggered = BaseEntity.Flags.Reserved1;
	}
}
