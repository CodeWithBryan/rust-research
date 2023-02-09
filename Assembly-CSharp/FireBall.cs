using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020003CD RID: 973
public class FireBall : BaseEntity, ISplashable
{
	// Token: 0x06002133 RID: 8499 RVA: 0x000D63CD File Offset: 0x000D45CD
	public void SetDelayedVelocity(Vector3 delayed)
	{
		if (this.delayedVelocity != Vector3.zero)
		{
			return;
		}
		this.delayedVelocity = delayed;
		base.Invoke(new Action(this.ApplyDelayedVelocity), 0.1f);
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x000D6400 File Offset: 0x000D4600
	private void ApplyDelayedVelocity()
	{
		this.SetVelocity(this.delayedVelocity);
		this.delayedVelocity = Vector3.zero;
	}

	// Token: 0x06002135 RID: 8501 RVA: 0x000D641C File Offset: 0x000D461C
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.Think), UnityEngine.Random.Range(0f, 1f), this.tickRate);
		float num = UnityEngine.Random.Range(this.lifeTimeMin, this.lifeTimeMax);
		float num2 = num * UnityEngine.Random.Range(0.9f, 1.1f);
		base.Invoke(new Action(this.Extinguish), num2);
		base.Invoke(new Action(this.TryToSpread), num * UnityEngine.Random.Range(0.3f, 0.5f));
		this.deathTime = Time.realtimeSinceStartup + num2;
		this.spawnTime = Time.realtimeSinceStartup;
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x000D64C8 File Offset: 0x000D46C8
	public float GetDeathTime()
	{
		return this.deathTime;
	}

	// Token: 0x06002137 RID: 8503 RVA: 0x000D64D0 File Offset: 0x000D46D0
	public void AddLife(float amountToAdd)
	{
		float time = Mathf.Clamp(this.GetDeathTime() + amountToAdd, 0f, this.MaxLifeTime());
		base.Invoke(new Action(this.Extinguish), time);
		this.deathTime = time;
	}

	// Token: 0x06002138 RID: 8504 RVA: 0x000D6510 File Offset: 0x000D4710
	public float MaxLifeTime()
	{
		return this.lifeTimeMax * 2.5f;
	}

	// Token: 0x06002139 RID: 8505 RVA: 0x000D6520 File Offset: 0x000D4720
	public float TimeLeft()
	{
		float num = this.deathTime - Time.realtimeSinceStartup;
		if (num < 0f)
		{
			num = 0f;
		}
		return num;
	}

	// Token: 0x0600213A RID: 8506 RVA: 0x000D654C File Offset: 0x000D474C
	public void TryToSpread()
	{
		float num = 0.9f - this.generation * 0.1f;
		if (UnityEngine.Random.Range(0f, 1f) >= num)
		{
			return;
		}
		if (this.spreadSubEntity.isValid)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.spreadSubEntity.resourcePath, default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.transform.position = base.transform.position + Vector3.up * 0.25f;
				baseEntity.Spawn();
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(45f, Vector3.up, true);
				baseEntity.creatorEntity = ((this.creatorEntity == null) ? baseEntity : this.creatorEntity);
				baseEntity.SetVelocity(modifiedAimConeDirection * UnityEngine.Random.Range(5f, 8f));
				baseEntity.SendMessage("SetGeneration", this.generation + 1f);
			}
		}
	}

	// Token: 0x0600213B RID: 8507 RVA: 0x000D665C File Offset: 0x000D485C
	public void SetGeneration(int gen)
	{
		this.generation = (float)gen;
	}

	// Token: 0x0600213C RID: 8508 RVA: 0x000D6668 File Offset: 0x000D4868
	public void Think()
	{
		if (!base.isServer)
		{
			return;
		}
		this.SetResting(Vector3.Distance(this.lastPos, base.transform.localPosition) < 0.25f);
		this.lastPos = base.transform.localPosition;
		if (this.IsResting())
		{
			this.DoRadialDamage();
		}
		if (this.WaterFactor() > 0.5f)
		{
			this.Extinguish();
		}
		if (this.wetness > this.waterToExtinguish)
		{
			this.Extinguish();
		}
	}

	// Token: 0x0600213D RID: 8509 RVA: 0x000D66E8 File Offset: 0x000D48E8
	public void DoRadialDamage()
	{
		List<Collider> list = Pool.GetList<Collider>();
		Vector3 position = base.transform.position + new Vector3(0f, this.radius * 0.75f, 0f);
		Vis.Colliders<Collider>(position, this.radius, list, this.AttackLayers, QueryTriggerInteraction.Collide);
		HitInfo hitInfo = new HitInfo();
		hitInfo.DoHitEffects = true;
		hitInfo.DidHit = true;
		hitInfo.HitBone = 0U;
		hitInfo.Initiator = ((this.creatorEntity == null) ? base.gameObject.ToBaseEntity() : this.creatorEntity);
		hitInfo.PointStart = base.transform.position;
		foreach (Collider collider in list)
		{
			if (!collider.isTrigger || (collider.gameObject.layer != 29 && collider.gameObject.layer != 18))
			{
				BaseCombatEntity baseCombatEntity = collider.gameObject.ToBaseEntity() as BaseCombatEntity;
				if (!(baseCombatEntity == null) && baseCombatEntity.isServer && baseCombatEntity.IsAlive() && (!this.ignoreNPC || !baseCombatEntity.IsNpc) && baseCombatEntity.IsVisible(position, float.PositiveInfinity))
				{
					if (baseCombatEntity is BasePlayer)
					{
						Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", baseCombatEntity, 0U, new Vector3(0f, 1f, 0f), Vector3.up, null, false);
					}
					hitInfo.PointEnd = baseCombatEntity.transform.position;
					hitInfo.HitPositionWorld = baseCombatEntity.transform.position;
					hitInfo.damageTypes.Set(DamageType.Heat, this.damagePerSecond * this.tickRate);
					baseCombatEntity.OnAttacked(hitInfo);
				}
			}
		}
		Pool.FreeList<Collider>(ref list);
	}

	// Token: 0x0600213E RID: 8510 RVA: 0x000D68E8 File Offset: 0x000D4AE8
	public bool CanMerge()
	{
		return this.canMerge && this.TimeLeft() < this.MaxLifeTime() * 0.8f;
	}

	// Token: 0x0600213F RID: 8511 RVA: 0x000D6908 File Offset: 0x000D4B08
	public float TimeAlive()
	{
		return Time.realtimeSinceStartup - this.spawnTime;
	}

	// Token: 0x06002140 RID: 8512 RVA: 0x000D6918 File Offset: 0x000D4B18
	public void SetResting(bool isResting)
	{
		if (isResting != this.IsResting() && isResting && this.TimeAlive() > 1f && this.CanMerge())
		{
			List<Collider> list = Pool.GetList<Collider>();
			Vis.Colliders<Collider>(base.transform.position, 0.5f, list, 512, QueryTriggerInteraction.Collide);
			foreach (Collider collider in list)
			{
				BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
				if (baseEntity)
				{
					FireBall fireBall = baseEntity.ToServer<FireBall>();
					if (fireBall && fireBall.CanMerge() && fireBall != this)
					{
						fireBall.Invoke(new Action(this.Extinguish), 1f);
						fireBall.canMerge = false;
						this.AddLife(fireBall.TimeLeft() * 0.25f);
					}
				}
			}
			Pool.FreeList<Collider>(ref list);
		}
		base.SetFlag(BaseEntity.Flags.OnFire, isResting, false, true);
	}

	// Token: 0x06002141 RID: 8513 RVA: 0x000D6A24 File Offset: 0x000D4C24
	public void Extinguish()
	{
		base.CancelInvoke(new Action(this.Extinguish));
		if (!base.IsDestroyed)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06002142 RID: 8514 RVA: 0x000D6A47 File Offset: 0x000D4C47
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed;
	}

	// Token: 0x06002143 RID: 8515 RVA: 0x000D6A52 File Offset: 0x000D4C52
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		this.wetness += amount;
		return amount;
	}

	// Token: 0x06002144 RID: 8516 RVA: 0x000028BF File Offset: 0x00000ABF
	public bool IsResting()
	{
		return base.HasFlag(BaseEntity.Flags.OnFire);
	}

	// Token: 0x06002145 RID: 8517 RVA: 0x000D6A63 File Offset: 0x000D4C63
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x04001990 RID: 6544
	public float lifeTimeMin = 20f;

	// Token: 0x04001991 RID: 6545
	public float lifeTimeMax = 40f;

	// Token: 0x04001992 RID: 6546
	public ParticleSystem[] movementSystems;

	// Token: 0x04001993 RID: 6547
	public ParticleSystem[] restingSystems;

	// Token: 0x04001994 RID: 6548
	[NonSerialized]
	public float generation;

	// Token: 0x04001995 RID: 6549
	public GameObjectRef spreadSubEntity;

	// Token: 0x04001996 RID: 6550
	public float tickRate = 0.5f;

	// Token: 0x04001997 RID: 6551
	public float damagePerSecond = 2f;

	// Token: 0x04001998 RID: 6552
	public float radius = 0.5f;

	// Token: 0x04001999 RID: 6553
	public int waterToExtinguish = 200;

	// Token: 0x0400199A RID: 6554
	public bool canMerge;

	// Token: 0x0400199B RID: 6555
	public LayerMask AttackLayers = 1219701521;

	// Token: 0x0400199C RID: 6556
	public bool ignoreNPC;

	// Token: 0x0400199D RID: 6557
	private Vector3 lastPos = Vector3.zero;

	// Token: 0x0400199E RID: 6558
	private float deathTime;

	// Token: 0x0400199F RID: 6559
	private int wetness;

	// Token: 0x040019A0 RID: 6560
	private float spawnTime;

	// Token: 0x040019A1 RID: 6561
	private Vector3 delayedVelocity;
}
