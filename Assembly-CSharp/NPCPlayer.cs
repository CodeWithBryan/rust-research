using System;
using System.Collections;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001E3 RID: 483
public class NPCPlayer : BasePlayer
{
	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x0600192F RID: 6447 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x06001930 RID: 6448 RVA: 0x000B6BB2 File Offset: 0x000B4DB2
	// (set) Token: 0x06001931 RID: 6449 RVA: 0x000B6BBA File Offset: 0x000B4DBA
	public virtual bool IsDormant
	{
		get
		{
			return this._isDormant;
		}
		set
		{
			this._isDormant = value;
			bool isDormant = this._isDormant;
		}
	}

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x06001932 RID: 6450 RVA: 0x000292BC File Offset: 0x000274BC
	protected override float PositionTickRate
	{
		get
		{
			return 0.1f;
		}
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsLoadBalanced()
	{
		return false;
	}

	// Token: 0x06001934 RID: 6452 RVA: 0x000B6BCC File Offset: 0x000B4DCC
	public override void ServerInit()
	{
		if (base.isClient)
		{
			return;
		}
		this.spawnPos = this.GetPosition();
		this.randomOffset = UnityEngine.Random.Range(0f, 1f);
		base.ServerInit();
		this.UpdateNetworkGroup();
		this.EquipLoadout(this.loadouts);
		if (!this.IsLoadBalanced())
		{
			base.InvokeRepeating(new Action(this.ServerThink_Internal), 0f, 0.1f);
			this.lastThinkTime = UnityEngine.Time.time;
		}
		base.Invoke(new Action(this.EquipTest), 0.25f);
		this.finalDestination = base.transform.position;
		if (this.NavAgent == null)
		{
			this.NavAgent = base.GetComponent<NavMeshAgent>();
		}
		if (this.NavAgent)
		{
			this.NavAgent.updateRotation = false;
			this.NavAgent.updatePosition = false;
			if (!this.LegacyNavigation)
			{
				base.transform.gameObject.GetComponent<BaseNavigator>().Init(this, this.NavAgent);
			}
		}
		base.InvokeRandomized(new Action(this.TickMovement), 1f, this.PositionTickRate, this.PositionTickRate * 0.1f);
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x000B6CFE File Offset: 0x000B4EFE
	public void EquipLoadout(PlayerInventoryProperties[] loads)
	{
		if (loads == null)
		{
			return;
		}
		if (loads.Length == 0)
		{
			return;
		}
		loads[UnityEngine.Random.Range(0, loads.Length)].GiveToPlayer(this);
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x00032960 File Offset: 0x00030B60
	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		this.ServerPosition = BaseNpc.GetNewNavPosWithVelocity(this, velocity);
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x000B6D1C File Offset: 0x000B4F1C
	public void RandomMove()
	{
		float d = 8f;
		Vector2 vector = UnityEngine.Random.insideUnitCircle * d;
		this.SetDestination(this.spawnPos + new Vector3(vector.x, 0f, vector.y));
	}

	// Token: 0x06001938 RID: 6456 RVA: 0x000B6D62 File Offset: 0x000B4F62
	public virtual void SetDestination(Vector3 newDestination)
	{
		this.finalDestination = newDestination;
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x000B6D6B File Offset: 0x000B4F6B
	public AttackEntity GetAttackEntity()
	{
		return base.GetHeldEntity() as AttackEntity;
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x000B6D78 File Offset: 0x000B4F78
	public BaseProjectile GetGun()
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity == null)
		{
			return null;
		}
		BaseProjectile baseProjectile = attackEntity as BaseProjectile;
		if (baseProjectile)
		{
			return baseProjectile;
		}
		return null;
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x000B6DB0 File Offset: 0x000B4FB0
	public virtual float AmmoFractionRemaining()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.AmmoFraction();
		}
		return 0f;
	}

	// Token: 0x0600193C RID: 6460 RVA: 0x000B6DD8 File Offset: 0x000B4FD8
	public virtual bool IsReloading()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		return attackEntity && attackEntity.ServerIsReloading();
	}

	// Token: 0x0600193D RID: 6461 RVA: 0x000B6DFC File Offset: 0x000B4FFC
	public virtual void AttemptReload()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		if (attackEntity == null)
		{
			return;
		}
		if (attackEntity.CanReload())
		{
			attackEntity.ServerReload();
		}
	}

	// Token: 0x0600193E RID: 6462 RVA: 0x000B6E28 File Offset: 0x000B5028
	public virtual bool ShotTest(float targetDist)
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity == null)
		{
			return false;
		}
		BaseProjectile baseProjectile = attackEntity as BaseProjectile;
		if (baseProjectile)
		{
			if (baseProjectile.primaryMagazine.contents <= 0)
			{
				baseProjectile.ServerReload();
				return false;
			}
			if (baseProjectile.NextAttackTime > UnityEngine.Time.time)
			{
				return false;
			}
		}
		if (Mathf.Approximately(attackEntity.attackLengthMin, -1f))
		{
			attackEntity.ServerUse(this.damageScale, null);
			this.lastGunShotTime = UnityEngine.Time.time;
			return true;
		}
		if (base.IsInvoking(new Action(this.TriggerDown)))
		{
			return true;
		}
		if (UnityEngine.Time.time < this.nextTriggerTime)
		{
			return true;
		}
		base.InvokeRepeating(new Action(this.TriggerDown), 0f, 0.01f);
		if (targetDist <= this.shortRange)
		{
			this.triggerEndTime = UnityEngine.Time.time + UnityEngine.Random.Range(attackEntity.attackLengthMin, attackEntity.attackLengthMax * this.attackLengthMaxShortRangeScale);
		}
		else
		{
			this.triggerEndTime = UnityEngine.Time.time + UnityEngine.Random.Range(attackEntity.attackLengthMin, attackEntity.attackLengthMax);
		}
		this.TriggerDown();
		return true;
	}

	// Token: 0x0600193F RID: 6463 RVA: 0x000062DD File Offset: 0x000044DD
	public virtual float GetAimConeScale()
	{
		return 1f;
	}

	// Token: 0x06001940 RID: 6464 RVA: 0x000B6F45 File Offset: 0x000B5145
	public void CancelBurst(float delay = 0.2f)
	{
		if (this.triggerEndTime > UnityEngine.Time.time + delay)
		{
			this.triggerEndTime = UnityEngine.Time.time + delay;
		}
	}

	// Token: 0x06001941 RID: 6465 RVA: 0x000B6F64 File Offset: 0x000B5164
	public bool MeleeAttack()
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity == null)
		{
			return false;
		}
		BaseMelee baseMelee = attackEntity as BaseMelee;
		if (baseMelee == null)
		{
			return false;
		}
		baseMelee.ServerUse(this.damageScale, null);
		return true;
	}

	// Token: 0x06001942 RID: 6466 RVA: 0x000B6FA8 File Offset: 0x000B51A8
	public virtual void TriggerDown()
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity != null)
		{
			attackEntity.ServerUse(this.damageScale, null);
		}
		this.lastGunShotTime = UnityEngine.Time.time;
		if (UnityEngine.Time.time > this.triggerEndTime)
		{
			base.CancelInvoke(new Action(this.TriggerDown));
			this.nextTriggerTime = UnityEngine.Time.time + ((attackEntity != null) ? attackEntity.attackSpacing : 1f);
		}
	}

	// Token: 0x06001943 RID: 6467 RVA: 0x000B7024 File Offset: 0x000B5224
	public virtual void EquipWeapon(bool skipDeployDelay = false)
	{
		if (this.inventory == null || this.inventory.containerBelt == null)
		{
			return;
		}
		Item slot = this.inventory.containerBelt.GetSlot(0);
		if (slot != null)
		{
			base.UpdateActiveItem(this.inventory.containerBelt.GetSlot(0).uid);
			BaseEntity heldEntity = slot.GetHeldEntity();
			if (heldEntity != null)
			{
				AttackEntity component = heldEntity.GetComponent<AttackEntity>();
				if (component != null)
				{
					if (skipDeployDelay)
					{
						component.ResetAttackCooldown();
					}
					component.TopUpAmmo();
				}
			}
		}
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x000B70AF File Offset: 0x000B52AF
	public void EquipTest()
	{
		this.EquipWeapon(true);
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x000B70B8 File Offset: 0x000B52B8
	internal void ServerThink_Internal()
	{
		float delta = UnityEngine.Time.time - this.lastThinkTime;
		this.ServerThink(delta);
		this.lastThinkTime = UnityEngine.Time.time;
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x000B70E4 File Offset: 0x000B52E4
	public virtual void ServerThink(float delta)
	{
		this.TickAi(delta);
	}

	// Token: 0x06001947 RID: 6471 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void Resume()
	{
	}

	// Token: 0x06001948 RID: 6472 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsNavRunning()
	{
		return false;
	}

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x06001949 RID: 6473 RVA: 0x000B70ED File Offset: 0x000B52ED
	public virtual bool IsOnNavMeshLink
	{
		get
		{
			return this.IsNavRunning() && this.NavAgent.isOnOffMeshLink;
		}
	}

	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x0600194A RID: 6474 RVA: 0x000B7104 File Offset: 0x000B5304
	public virtual bool HasPath
	{
		get
		{
			return this.IsNavRunning() && this.NavAgent.hasPath;
		}
	}

	// Token: 0x0600194B RID: 6475 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void TickAi(float delta)
	{
	}

	// Token: 0x0600194C RID: 6476 RVA: 0x000B711C File Offset: 0x000B531C
	public void TickMovement()
	{
		float delta = UnityEngine.Time.realtimeSinceStartup - this.lastMovementTickTime;
		this.lastMovementTickTime = UnityEngine.Time.realtimeSinceStartup;
		this.MovementUpdate(delta);
	}

	// Token: 0x0600194D RID: 6477 RVA: 0x000B7148 File Offset: 0x000B5348
	public override float GetNetworkTime()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastPositionUpdateTime > this.PositionTickRate * 2f)
		{
			return UnityEngine.Time.time;
		}
		return this.lastPositionUpdateTime;
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x000B7170 File Offset: 0x000B5370
	public virtual void MovementUpdate(float delta)
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		if (base.isClient)
		{
			return;
		}
		if (!this.IsAlive() || base.IsWounded() || (!base.isMounted && !this.IsNavRunning()))
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			if (this.IsNavRunning())
			{
				this.NavAgent.destination = this.ServerPosition;
			}
			return;
		}
		Vector3 moveToPosition = base.transform.position;
		if (this.HasPath)
		{
			moveToPosition = this.NavAgent.nextPosition;
		}
		if (!this.ValidateNextPosition(ref moveToPosition))
		{
			return;
		}
		this.UpdateSpeed(delta);
		this.UpdatePositionAndRotation(moveToPosition);
	}

	// Token: 0x0600194F RID: 6479 RVA: 0x000B7214 File Offset: 0x000B5414
	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		if (!ValidBounds.Test(moveToPosition) && base.transform != null && !base.IsDestroyed)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Invalid NavAgent Position: ",
				this,
				" ",
				moveToPosition.ToString(),
				" (destroying)"
			}));
			base.Kill(BaseNetworkable.DestroyMode.None);
			return false;
		}
		return true;
	}

	// Token: 0x06001950 RID: 6480 RVA: 0x000B728C File Offset: 0x000B548C
	private void UpdateSpeed(float delta)
	{
		float b = this.DesiredMoveSpeed();
		this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, b, delta * 8f);
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x000B72C3 File Offset: 0x000B54C3
	protected virtual void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		this.lastPositionUpdateTime = UnityEngine.Time.time;
		this.ServerPosition = moveToPosition;
		this.SetAimDirection(this.GetAimDirection());
	}

	// Token: 0x06001952 RID: 6482 RVA: 0x000299AB File Offset: 0x00027BAB
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06001953 RID: 6483 RVA: 0x000B72E4 File Offset: 0x000B54E4
	public virtual float DesiredMoveSpeed()
	{
		float running = Mathf.Sin(UnityEngine.Time.time + this.randomOffset);
		return base.GetSpeed(running, 0f, 0f);
	}

	// Token: 0x06001954 RID: 6484 RVA: 0x00007074 File Offset: 0x00005274
	public override bool EligibleForWounding(HitInfo info)
	{
		return false;
	}

	// Token: 0x06001955 RID: 6485 RVA: 0x000B7314 File Offset: 0x000B5514
	public virtual Vector3 GetAimDirection()
	{
		if (Vector3Ex.Distance2D(this.finalDestination, this.GetPosition()) >= 1f)
		{
			Vector3 normalized = (this.finalDestination - this.GetPosition()).normalized;
			return new Vector3(normalized.x, 0f, normalized.z);
		}
		return this.eyes.BodyForward();
	}

	// Token: 0x06001956 RID: 6486 RVA: 0x000B7378 File Offset: 0x000B5578
	public virtual void SetAimDirection(Vector3 newAim)
	{
		if (newAim == Vector3.zero)
		{
			return;
		}
		AttackEntity attackEntity = this.GetAttackEntity();
		if (attackEntity)
		{
			newAim = attackEntity.ModifyAIAim(newAim, 1f);
		}
		this.eyes.rotation = Quaternion.LookRotation(newAim, Vector3.up);
		this.viewAngles = this.eyes.rotation.eulerAngles;
		this.ServerRotation = this.eyes.rotation;
		this.lastPositionUpdateTime = UnityEngine.Time.time;
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x000B73FC File Offset: 0x000B55FC
	public bool TryUseThrownWeapon(BaseEntity target, float attackRate)
	{
		if (this.HasThrownItemCooldown())
		{
			return false;
		}
		Item item = this.FindThrownWeapon();
		if (item == null)
		{
			this.lastThrowTime = UnityEngine.Time.time;
			return false;
		}
		return this.TryUseThrownWeapon(item, target, attackRate);
	}

	// Token: 0x06001958 RID: 6488 RVA: 0x000B7434 File Offset: 0x000B5634
	public bool TryUseThrownWeapon(Item item, BaseEntity target, float attackRate)
	{
		if (this.HasThrownItemCooldown())
		{
			return false;
		}
		float num = Vector3.Distance(target.transform.position, base.transform.position);
		if (num <= 2f || num >= 20f)
		{
			return false;
		}
		Vector3 position = target.transform.position;
		if (!base.IsVisible(base.CenterPoint(), position, float.PositiveInfinity))
		{
			return false;
		}
		if (this.UseThrownWeapon(item, target))
		{
			if (this is ScarecrowNPC)
			{
				ScarecrowNPC.NextBeanCanAllowedTime = UnityEngine.Time.time + Halloween.scarecrow_throw_beancan_global_delay;
			}
			this.lastThrowTime = UnityEngine.Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x06001959 RID: 6489 RVA: 0x000B74CD File Offset: 0x000B56CD
	public bool HasThrownItemCooldown()
	{
		return UnityEngine.Time.time - this.lastThrowTime < 10f;
	}

	// Token: 0x0600195A RID: 6490 RVA: 0x000B74E4 File Offset: 0x000B56E4
	protected bool UseThrownWeapon(Item item, BaseEntity target)
	{
		base.UpdateActiveItem(item.uid);
		ThrownWeapon thrownWeapon = base.GetActiveItem().GetHeldEntity() as ThrownWeapon;
		if (thrownWeapon == null)
		{
			return false;
		}
		base.StartCoroutine(this.DoThrow(thrownWeapon, target));
		return true;
	}

	// Token: 0x0600195B RID: 6491 RVA: 0x000B7529 File Offset: 0x000B5729
	private IEnumerator DoThrow(ThrownWeapon thrownWeapon, BaseEntity target)
	{
		this.modelState.aiming = true;
		yield return new WaitForSeconds(1.5f);
		this.SetAimDirection(Vector3Ex.Direction(target.transform.position, base.transform.position));
		thrownWeapon.ResetAttackCooldown();
		thrownWeapon.ServerThrow(target.transform.position);
		this.modelState.aiming = false;
		base.Invoke(new Action(this.EquipTest), 0.5f);
		yield break;
	}

	// Token: 0x0600195C RID: 6492 RVA: 0x000B7548 File Offset: 0x000B5748
	public Item FindThrownWeapon()
	{
		if (this.inventory == null || this.inventory.containerBelt == null)
		{
			return null;
		}
		for (int i = 0; i < this.inventory.containerBelt.capacity; i++)
		{
			Item slot = this.inventory.containerBelt.GetSlot(i);
			if (slot != null && slot.GetHeldEntity() as ThrownWeapon != null)
			{
				return slot;
			}
		}
		return null;
	}

	// Token: 0x040011E9 RID: 4585
	public AIInformationZone VirtualInfoZone;

	// Token: 0x040011EA RID: 4586
	public Vector3 finalDestination;

	// Token: 0x040011EB RID: 4587
	[NonSerialized]
	private float randomOffset;

	// Token: 0x040011EC RID: 4588
	[NonSerialized]
	private Vector3 spawnPos;

	// Token: 0x040011ED RID: 4589
	public PlayerInventoryProperties[] loadouts;

	// Token: 0x040011EE RID: 4590
	public LayerMask movementMask = 429990145;

	// Token: 0x040011EF RID: 4591
	public bool LegacyNavigation = true;

	// Token: 0x040011F0 RID: 4592
	public NavMeshAgent NavAgent;

	// Token: 0x040011F1 RID: 4593
	public float damageScale = 1f;

	// Token: 0x040011F2 RID: 4594
	public float shortRange = 10f;

	// Token: 0x040011F3 RID: 4595
	public float attackLengthMaxShortRangeScale = 1f;

	// Token: 0x040011F4 RID: 4596
	private bool _isDormant;

	// Token: 0x040011F5 RID: 4597
	protected float lastGunShotTime;

	// Token: 0x040011F6 RID: 4598
	private float triggerEndTime;

	// Token: 0x040011F7 RID: 4599
	protected float nextTriggerTime;

	// Token: 0x040011F8 RID: 4600
	private float lastThinkTime;

	// Token: 0x040011F9 RID: 4601
	private float lastPositionUpdateTime;

	// Token: 0x040011FA RID: 4602
	private float lastMovementTickTime;

	// Token: 0x040011FB RID: 4603
	private Vector3 lastPos;

	// Token: 0x040011FC RID: 4604
	private float lastThrowTime;
}
