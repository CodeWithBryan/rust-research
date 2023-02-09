using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x0200055C RID: 1372
public class TriggerHurtNotChild : TriggerBase, IServerComponent, IHurtTrigger
{
	// Token: 0x060029B9 RID: 10681 RVA: 0x000FD0E8 File Offset: 0x000FB2E8
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		if (this.ignoreNPC && baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x060029BA RID: 10682 RVA: 0x000FD13D File Offset: 0x000FB33D
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), 0f, 1f / this.DamageTickRate);
	}

	// Token: 0x060029BB RID: 10683 RVA: 0x000FD164 File Offset: 0x000FB364
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent != null && this.DamageDelay > 0f)
		{
			if (this.entryTimes == null)
			{
				this.entryTimes = new Dictionary<BaseEntity, float>();
			}
			this.entryTimes.Add(ent, Time.time);
		}
	}

	// Token: 0x060029BC RID: 10684 RVA: 0x000FD1B2 File Offset: 0x000FB3B2
	internal override void OnEntityLeave(BaseEntity ent)
	{
		if (ent != null && this.entryTimes != null)
		{
			this.entryTimes.Remove(ent);
		}
		base.OnEntityLeave(ent);
	}

	// Token: 0x060029BD RID: 10685 RVA: 0x000FD1D9 File Offset: 0x000FB3D9
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x060029BE RID: 10686 RVA: 0x000FD1ED File Offset: 0x000FB3ED
	protected void OnEnable()
	{
		this.timeSinceAcivation = 0f;
		this.hurtTiggerUser = (this.SourceEntity as TriggerHurtNotChild.IHurtTriggerUser);
	}

	// Token: 0x060029BF RID: 10687 RVA: 0x000FD210 File Offset: 0x000FB410
	public new void OnDisable()
	{
		base.CancelInvoke(new Action(this.OnTick));
		base.OnDisable();
	}

	// Token: 0x060029C0 RID: 10688 RVA: 0x000FD22C File Offset: 0x000FB42C
	private bool IsInterested(BaseEntity ent)
	{
		if (this.timeSinceAcivation < this.activationDelay)
		{
			return false;
		}
		BasePlayer basePlayer = ent.ToPlayer();
		if (basePlayer != null)
		{
			if (basePlayer.isMounted)
			{
				BaseVehicle mountedVehicle = basePlayer.GetMountedVehicle();
				if (this.SourceEntity != null && mountedVehicle == this.SourceEntity)
				{
					return false;
				}
				if (this.ignoreAllVehicleMounted && mountedVehicle != null)
				{
					return false;
				}
			}
			if (this.SourceEntity != null && basePlayer.HasEntityInParents(this.SourceEntity))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060029C1 RID: 10689 RVA: 0x000FD2C0 File Offset: 0x000FB4C0
	private void OnTick()
	{
		if (this.entityContents.IsNullOrEmpty<BaseEntity>())
		{
			return;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		list.AddRange(this.entityContents);
		foreach (BaseEntity baseEntity in list)
		{
			float num;
			if (baseEntity.IsValid() && this.IsInterested(baseEntity) && (this.DamageDelay <= 0f || this.entryTimes == null || !this.entryTimes.TryGetValue(baseEntity, out num) || num + this.DamageDelay <= Time.time) && (!this.RequireUpAxis || Vector3.Dot(baseEntity.transform.up, base.transform.up) >= 0f))
			{
				float num2 = this.DamagePerSecond * 1f / this.DamageTickRate;
				if (this.UseSourceEntityDamageMultiplier && this.hurtTiggerUser != null)
				{
					num2 *= this.hurtTiggerUser.GetPlayerDamageMultiplier();
				}
				if (baseEntity.IsNpc)
				{
					num2 *= this.npcMultiplier;
				}
				if (baseEntity is ResourceEntity)
				{
					num2 *= this.resourceMultiplier;
				}
				Vector3 vector = baseEntity.transform.position + Vector3.up * 1f;
				HitInfo hitInfo = new HitInfo
				{
					DoHitEffects = true,
					HitEntity = baseEntity,
					HitPositionWorld = vector,
					HitPositionLocal = baseEntity.transform.InverseTransformPoint(vector),
					HitNormalWorld = Vector3.up,
					HitMaterial = ((baseEntity is BaseCombatEntity) ? StringPool.Get("Flesh") : 0U),
					WeaponPrefab = ((this.SourceEntity != null) ? this.SourceEntity.gameManager.FindPrefab(this.SourceEntity.prefabID).GetComponent<BaseEntity>() : null),
					Initiator = ((this.hurtTiggerUser != null) ? this.hurtTiggerUser.GetPlayerDamageInitiator() : null)
				};
				hitInfo.damageTypes = new DamageTypeList();
				hitInfo.damageTypes.Set(this.damageType, num2);
				baseEntity.OnAttacked(hitInfo);
				if (this.hurtTiggerUser != null)
				{
					this.hurtTiggerUser.OnHurtTriggerOccupant(baseEntity, this.damageType, num2);
				}
				if (this.triggerHitImpacts)
				{
					Effect.server.ImpactEffect(hitInfo);
				}
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		base.RemoveInvalidEntities();
	}

	// Token: 0x040021BF RID: 8639
	public float DamagePerSecond = 1f;

	// Token: 0x040021C0 RID: 8640
	public float DamageTickRate = 4f;

	// Token: 0x040021C1 RID: 8641
	public float DamageDelay;

	// Token: 0x040021C2 RID: 8642
	public DamageType damageType;

	// Token: 0x040021C3 RID: 8643
	public bool ignoreNPC = true;

	// Token: 0x040021C4 RID: 8644
	public float npcMultiplier = 1f;

	// Token: 0x040021C5 RID: 8645
	public float resourceMultiplier = 1f;

	// Token: 0x040021C6 RID: 8646
	public bool triggerHitImpacts = true;

	// Token: 0x040021C7 RID: 8647
	public bool RequireUpAxis;

	// Token: 0x040021C8 RID: 8648
	public BaseEntity SourceEntity;

	// Token: 0x040021C9 RID: 8649
	public bool UseSourceEntityDamageMultiplier = true;

	// Token: 0x040021CA RID: 8650
	public bool ignoreAllVehicleMounted;

	// Token: 0x040021CB RID: 8651
	public float activationDelay;

	// Token: 0x040021CC RID: 8652
	private Dictionary<BaseEntity, float> entryTimes;

	// Token: 0x040021CD RID: 8653
	private TimeSince timeSinceAcivation;

	// Token: 0x040021CE RID: 8654
	private TriggerHurtNotChild.IHurtTriggerUser hurtTiggerUser;

	// Token: 0x02000D03 RID: 3331
	public interface IHurtTriggerUser
	{
		// Token: 0x06004E07 RID: 19975
		BasePlayer GetPlayerDamageInitiator();

		// Token: 0x06004E08 RID: 19976
		float GetPlayerDamageMultiplier();

		// Token: 0x06004E09 RID: 19977
		void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal);
	}
}
