using System;
using System.Collections.Generic;
using Rust;
using Rust.Ai;
using UnityEngine;

// Token: 0x0200043D RID: 1085
public class TimedExplosive : BaseEntity, ServerProjectile.IProjectileImpact
{
	// Token: 0x060023A9 RID: 9129 RVA: 0x000E1B1C File Offset: 0x000DFD1C
	public void SetDamageScale(float scale)
	{
		foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
		{
			damageTypeEntry.amount *= scale;
		}
	}

	// Token: 0x060023AA RID: 9130 RVA: 0x000058B6 File Offset: 0x00003AB6
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x060023AB RID: 9131 RVA: 0x000E1B74 File Offset: 0x000DFD74
	public override void ServerInit()
	{
		this.lastBounceTime = Time.time;
		base.ServerInit();
		this.SetFuse(this.GetRandomTimerTime());
		base.ReceiveCollisionMessages(true);
		if (this.waterCausesExplosion || this.AlwaysRunWaterCheck)
		{
			base.InvokeRepeating(new Action(this.WaterCheck), 0f, 0.5f);
		}
	}

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x060023AC RID: 9132 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual bool AlwaysRunWaterCheck
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x000E1BD2 File Offset: 0x000DFDD2
	public virtual void WaterCheck()
	{
		if (this.waterCausesExplosion && this.WaterFactor() >= 0.5f)
		{
			this.Explode();
		}
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x000E1BEF File Offset: 0x000DFDEF
	public virtual void SetFuse(float fuseLength)
	{
		if (base.isServer)
		{
			base.Invoke(new Action(this.Explode), fuseLength);
		}
	}

	// Token: 0x060023AF RID: 9135 RVA: 0x000E1C0D File Offset: 0x000DFE0D
	public virtual float GetRandomTimerTime()
	{
		return UnityEngine.Random.Range(this.timerAmountMin, this.timerAmountMax);
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x000E1C20 File Offset: 0x000DFE20
	public virtual void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		this.Explode();
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x000E1C28 File Offset: 0x000DFE28
	public virtual void Explode()
	{
		this.Explode(base.PivotPoint());
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x000E1C38 File Offset: 0x000DFE38
	public virtual void Explode(Vector3 explosionFxPos)
	{
		Collider component = base.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
		}
		bool flag = false;
		if (this.underwaterExplosionEffect.isValid)
		{
			flag = (WaterLevel.GetWaterDepth(base.transform.position, true, null) > 1f);
		}
		if (flag)
		{
			Effect.server.Run(this.underwaterExplosionEffect.resourcePath, explosionFxPos, this.explosionUsesForward ? base.transform.forward : Vector3.up, null, true);
		}
		else if (this.explosionEffect.isValid)
		{
			Effect.server.Run(this.explosionEffect.resourcePath, explosionFxPos, this.explosionUsesForward ? base.transform.forward : Vector3.up, null, true);
		}
		if (this.damageTypes.Count > 0)
		{
			if (this.onlyDamageParent)
			{
				DamageUtil.RadiusDamage(this.creatorEntity, base.LookupPrefab(), base.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 166144, true);
				BaseEntity parentEntity = base.GetParentEntity();
				BaseCombatEntity baseCombatEntity = parentEntity as BaseCombatEntity;
				while (baseCombatEntity == null && parentEntity != null && parentEntity.HasParent())
				{
					parentEntity = parentEntity.GetParentEntity();
					baseCombatEntity = (parentEntity as BaseCombatEntity);
				}
				if (baseCombatEntity)
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.Initiator = this.creatorEntity;
					hitInfo.WeaponPrefab = base.LookupPrefab();
					hitInfo.damageTypes.Add(this.damageTypes);
					baseCombatEntity.Hurt(hitInfo);
				}
				if (this.creatorEntity != null && this.damageTypes != null)
				{
					float num = 0f;
					foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
					{
						num += damageTypeEntry.amount;
					}
					Sense.Stimulate(new Sensation
					{
						Type = SensationType.Explosion,
						Position = this.creatorEntity.transform.position,
						Radius = this.explosionRadius * 17f,
						DamagePotential = num,
						InitiatorPlayer = (this.creatorEntity as BasePlayer),
						Initiator = this.creatorEntity
					});
				}
			}
			else
			{
				DamageUtil.RadiusDamage(this.creatorEntity, base.LookupPrefab(), base.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 1076005121, true);
				if (this.creatorEntity != null && this.damageTypes != null)
				{
					float num2 = 0f;
					foreach (DamageTypeEntry damageTypeEntry2 in this.damageTypes)
					{
						num2 += damageTypeEntry2.amount;
					}
					Sense.Stimulate(new Sensation
					{
						Type = SensationType.Explosion,
						Position = this.creatorEntity.transform.position,
						Radius = this.explosionRadius * 17f,
						DamagePotential = num2,
						InitiatorPlayer = (this.creatorEntity as BasePlayer),
						Initiator = this.creatorEntity
					});
				}
			}
		}
		if (base.IsDestroyed || base.HasFlag(BaseEntity.Flags.Broken))
		{
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060023B3 RID: 9139 RVA: 0x000E1FB8 File Offset: 0x000E01B8
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (this.canStick && !this.IsStuck())
		{
			bool flag = true;
			if (hitEntity)
			{
				flag = this.CanStickTo(hitEntity);
				if (!flag)
				{
					Collider component = base.GetComponent<Collider>();
					if (collision.collider != null && component != null)
					{
						Physics.IgnoreCollision(collision.collider, component);
					}
				}
			}
			if (flag)
			{
				this.DoCollisionStick(collision, hitEntity);
			}
		}
		if (this.explodeOnContact && !base.IsBusy())
		{
			this.SetMotionEnabled(false);
			base.SetFlag(BaseEntity.Flags.Busy, true, false, false);
			base.Invoke(new Action(this.Explode), 0.015f);
			return;
		}
		this.DoBounceEffect();
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x000E2063 File Offset: 0x000E0263
	public virtual bool CanStickTo(BaseEntity entity)
	{
		return entity.GetComponent<DecorDeployable>() == null;
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x000E2074 File Offset: 0x000E0274
	private void DoBounceEffect()
	{
		if (!this.bounceEffect.isValid)
		{
			return;
		}
		if (Time.time - this.lastBounceTime < 0.2f)
		{
			return;
		}
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component && component.velocity.magnitude < 1f)
		{
			return;
		}
		if (this.bounceEffect.isValid)
		{
			Effect.server.Run(this.bounceEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		this.lastBounceTime = Time.time;
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x000E2104 File Offset: 0x000E0304
	private void DoCollisionStick(Collision collision, BaseEntity ent)
	{
		ContactPoint contact = collision.GetContact(0);
		this.DoStick(contact.point, contact.normal, ent, collision.collider);
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x000E2134 File Offset: 0x000E0334
	public virtual void SetMotionEnabled(bool wantsMotion)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			if (this.initialCollisionDetectionMode == null)
			{
				this.initialCollisionDetectionMode = new CollisionDetectionMode?(component.collisionDetectionMode);
			}
			component.useGravity = wantsMotion;
			if (!wantsMotion)
			{
				component.collisionDetectionMode = CollisionDetectionMode.Discrete;
			}
			component.isKinematic = !wantsMotion;
			if (wantsMotion)
			{
				component.collisionDetectionMode = this.initialCollisionDetectionMode.Value;
			}
		}
	}

	// Token: 0x060023B8 RID: 9144 RVA: 0x000E21A0 File Offset: 0x000E03A0
	public bool IsStuck()
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component && !component.isKinematic)
		{
			return false;
		}
		Collider component2 = base.GetComponent<Collider>();
		return (!component2 || !component2.enabled) && this.parentEntity.IsValid(true);
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x000E21EC File Offset: 0x000E03EC
	public void DoStick(Vector3 position, Vector3 normal, BaseEntity ent, Collider collider)
	{
		if (ent == null)
		{
			return;
		}
		if (ent is TimedExplosive)
		{
			if (!ent.HasParent())
			{
				return;
			}
			position = ent.transform.position;
			ent = ent.parentEntity.Get(true);
		}
		this.SetMotionEnabled(false);
		this.SetCollisionEnabled(false);
		if (base.HasChild(ent))
		{
			return;
		}
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(normal, base.transform.up);
		if (collider != null)
		{
			base.SetParent(ent, ent.FindBoneID(collider.transform), true, false);
		}
		else
		{
			base.SetParent(ent, StringPool.closest, true, false);
		}
		if (this.stickEffect.isValid)
		{
			Effect.server.Run(this.stickEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		base.ReceiveCollisionMessages(false);
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x000E22D7 File Offset: 0x000E04D7
	private void UnStick()
	{
		if (!base.GetParentEntity())
		{
			return;
		}
		base.SetParent(null, true, true);
		this.SetMotionEnabled(true);
		this.SetCollisionEnabled(true);
		base.ReceiveCollisionMessages(true);
	}

	// Token: 0x060023BB RID: 9147 RVA: 0x000E2305 File Offset: 0x000E0505
	internal override void OnParentRemoved()
	{
		this.UnStick();
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x000E2310 File Offset: 0x000E0510
	public virtual void SetCollisionEnabled(bool wantsCollision)
	{
		Collider component = base.GetComponent<Collider>();
		if (component && component.enabled != wantsCollision)
		{
			component.enabled = wantsCollision;
		}
	}

	// Token: 0x04001C51 RID: 7249
	public float timerAmountMin = 10f;

	// Token: 0x04001C52 RID: 7250
	public float timerAmountMax = 20f;

	// Token: 0x04001C53 RID: 7251
	public float minExplosionRadius;

	// Token: 0x04001C54 RID: 7252
	public float explosionRadius = 10f;

	// Token: 0x04001C55 RID: 7253
	public bool explodeOnContact;

	// Token: 0x04001C56 RID: 7254
	public bool canStick;

	// Token: 0x04001C57 RID: 7255
	public bool onlyDamageParent;

	// Token: 0x04001C58 RID: 7256
	public GameObjectRef explosionEffect;

	// Token: 0x04001C59 RID: 7257
	[Tooltip("Optional: Will fall back to explosionEffect if not assigned.")]
	public GameObjectRef underwaterExplosionEffect;

	// Token: 0x04001C5A RID: 7258
	public GameObjectRef stickEffect;

	// Token: 0x04001C5B RID: 7259
	public GameObjectRef bounceEffect;

	// Token: 0x04001C5C RID: 7260
	public bool explosionUsesForward;

	// Token: 0x04001C5D RID: 7261
	public bool waterCausesExplosion;

	// Token: 0x04001C5E RID: 7262
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	// Token: 0x04001C5F RID: 7263
	[NonSerialized]
	private float lastBounceTime;

	// Token: 0x04001C60 RID: 7264
	private CollisionDetectionMode? initialCollisionDetectionMode;
}
