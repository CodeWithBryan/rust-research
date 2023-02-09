using System;
using System.Collections;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001DC RID: 476
public class HumanNPC : NPCPlayer, IAISenses, IAIAttack, IThinker
{
	// Token: 0x060018E0 RID: 6368 RVA: 0x000271CD File Offset: 0x000253CD
	public override float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x060018E1 RID: 6369 RVA: 0x000271CD File Offset: 0x000253CD
	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x060018E2 RID: 6370 RVA: 0x000271CD File Offset: 0x000253CD
	public override float MaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x060018E3 RID: 6371 RVA: 0x000B58D5 File Offset: 0x000B3AD5
	// (set) Token: 0x060018E4 RID: 6372 RVA: 0x000B58DD File Offset: 0x000B3ADD
	public ScientistBrain Brain { get; private set; }

	// Token: 0x060018E5 RID: 6373 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsLoadBalanced()
	{
		return true;
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x000B58E6 File Offset: 0x000B3AE6
	public override void ServerInit()
	{
		base.ServerInit();
		this.Brain = base.GetComponent<ScientistBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.Add(this);
	}

	// Token: 0x060018E7 RID: 6375 RVA: 0x000B5909 File Offset: 0x000B3B09
	internal override void DoServerDestroy()
	{
		AIThinkManager.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x060018E8 RID: 6376 RVA: 0x000B5917 File Offset: 0x000B3B17
	public void LightCheck()
	{
		if ((TOD_Sky.Instance.IsNight && !this.lightsOn) || (TOD_Sky.Instance.IsDay && this.lightsOn))
		{
			base.LightToggle(true);
			this.lightsOn = !this.lightsOn;
		}
	}

	// Token: 0x060018E9 RID: 6377 RVA: 0x000B5957 File Offset: 0x000B3B57
	public override float GetAimConeScale()
	{
		return this.aimConeScale;
	}

	// Token: 0x060018EA RID: 6378 RVA: 0x000B595F File Offset: 0x000B3B5F
	public override void EquipWeapon(bool skipDeployDelay = false)
	{
		base.EquipWeapon(skipDeployDelay);
	}

	// Token: 0x060018EB RID: 6379 RVA: 0x000B5968 File Offset: 0x000B3B68
	public override void DismountObject()
	{
		base.DismountObject();
		this.lastDismountTime = Time.time;
	}

	// Token: 0x060018EC RID: 6380 RVA: 0x000B597B File Offset: 0x000B3B7B
	public bool RecentlyDismounted()
	{
		return Time.time < this.lastDismountTime + 10f;
	}

	// Token: 0x060018ED RID: 6381 RVA: 0x000B5990 File Offset: 0x000B3B90
	public virtual float GetIdealDistanceFromTarget()
	{
		return Mathf.Max(5f, this.EngagementRange() * 0.75f);
	}

	// Token: 0x060018EE RID: 6382 RVA: 0x000B59A8 File Offset: 0x000B3BA8
	public AIInformationZone GetInformationZone(Vector3 pos)
	{
		if (this.VirtualInfoZone != null)
		{
			return this.VirtualInfoZone;
		}
		if (this.cachedInfoZone == null || Time.time > this.nextZoneSearchTime)
		{
			this.cachedInfoZone = AIInformationZone.GetForPoint(pos, true);
			this.nextZoneSearchTime = Time.time + 5f;
		}
		return this.cachedInfoZone;
	}

	// Token: 0x060018EF RID: 6383 RVA: 0x000B5A0C File Offset: 0x000B3C0C
	public float EngagementRange()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * this.Brain.AttackRangeMultiplier;
		}
		return this.Brain.SenseRange;
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x000B5A5B File Offset: 0x000B3C5B
	public void SetDucked(bool flag)
	{
		this.modelState.ducked = flag;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x000B526A File Offset: 0x000B346A
	public virtual void TryThink()
	{
		base.ServerThink_Internal();
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x000B5A70 File Offset: 0x000B3C70
	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		if (this.Brain.ShouldServerThink())
		{
			this.Brain.DoThink();
		}
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x000B5A94 File Offset: 0x000B3C94
	public void TickAttack(float delta, BaseCombatEntity target, bool targetIsLOS)
	{
		if (target == null)
		{
			return;
		}
		float num = Vector3.Dot(this.eyes.BodyForward(), (target.CenterPoint() - this.eyes.position).normalized);
		if (targetIsLOS)
		{
			if (num > 0.2f)
			{
				this.targetAimedDuration += delta;
			}
		}
		else
		{
			if (num < 0.5f)
			{
				this.targetAimedDuration = 0f;
			}
			base.CancelBurst(0.2f);
		}
		if (this.targetAimedDuration >= 0.2f && targetIsLOS)
		{
			bool flag = false;
			float num2 = 0f;
			if (this != null)
			{
				flag = ((IAIAttack)this).IsTargetInRange(target, out num2);
			}
			else
			{
				AttackEntity attackEntity = base.GetAttackEntity();
				if (attackEntity)
				{
					num2 = ((target != null) ? Vector3.Distance(base.transform.position, target.transform.position) : -1f);
					flag = (num2 < attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f));
				}
			}
			if (flag)
			{
				this.ShotTest(num2);
				return;
			}
		}
		else
		{
			base.CancelBurst(0.2f);
		}
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x000B5BBC File Offset: 0x000B3DBC
	public override void Hurt(HitInfo info)
	{
		if (base.isMounted)
		{
			info.damageTypes.ScaleAll(0.1f);
		}
		base.Hurt(info);
		global::BaseEntity initiator = info.Initiator;
		if (initiator != null && !initiator.EqualNetID(this))
		{
			this.Brain.Senses.Memory.SetKnown(initiator, this, null);
		}
	}

	// Token: 0x060018F5 RID: 6389 RVA: 0x000B5C19 File Offset: 0x000B3E19
	public float GetAimSwayScalar()
	{
		return 1f - Mathf.InverseLerp(1f, 3f, Time.time - this.lastGunShotTime);
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x000B5C3C File Offset: 0x000B3E3C
	public override Vector3 GetAimDirection()
	{
		if (this.Brain != null && this.Brain.Navigator != null && this.Brain.Navigator.IsOverridingFacingDirection)
		{
			return this.Brain.Navigator.FacingDirectionOverride;
		}
		return base.GetAimDirection();
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x000B5C94 File Offset: 0x000B3E94
	public override void SetAimDirection(Vector3 newAim)
	{
		if (newAim == Vector3.zero)
		{
			return;
		}
		float num = Time.time - this.lastAimSetTime;
		this.lastAimSetTime = Time.time;
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			newAim = attackEntity.ModifyAIAim(newAim, this.GetAimSwayScalar());
		}
		if (base.isMounted)
		{
			BaseMountable mounted = base.GetMounted();
			Vector3 eulerAngles = mounted.transform.eulerAngles;
			Quaternion rotation = Quaternion.Euler(Quaternion.LookRotation(newAim, mounted.transform.up).eulerAngles);
			Vector3 vector = Quaternion.LookRotation(base.transform.InverseTransformDirection(rotation * Vector3.forward), base.transform.up).eulerAngles;
			vector = BaseMountable.ConvertVector(vector);
			Quaternion rotation2 = Quaternion.Euler(Mathf.Clamp(vector.x, mounted.pitchClamp.x, mounted.pitchClamp.y), Mathf.Clamp(vector.y, mounted.yawClamp.x, mounted.yawClamp.y), eulerAngles.z);
			newAim = BaseMountable.ConvertVector(Quaternion.LookRotation(base.transform.TransformDirection(rotation2 * Vector3.forward), base.transform.up).eulerAngles);
		}
		else
		{
			global::BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity)
			{
				Vector3 vector2 = parentEntity.transform.InverseTransformDirection(newAim);
				Vector3 forward = new Vector3(newAim.x, vector2.y, newAim.z);
				this.eyes.rotation = Quaternion.Lerp(this.eyes.rotation, Quaternion.LookRotation(forward, parentEntity.transform.up), num * 25f);
				this.viewAngles = this.eyes.bodyRotation.eulerAngles;
				this.ServerRotation = this.eyes.bodyRotation;
				return;
			}
		}
		this.eyes.rotation = (base.isMounted ? Quaternion.Slerp(this.eyes.rotation, Quaternion.Euler(newAim), num * 70f) : Quaternion.Lerp(this.eyes.rotation, Quaternion.LookRotation(newAim, base.transform.up), num * 25f));
		this.viewAngles = this.eyes.rotation.eulerAngles;
		this.ServerRotation = this.eyes.rotation;
	}

	// Token: 0x060018F8 RID: 6392 RVA: 0x000B5F0F File Offset: 0x000B410F
	public void SetStationaryAimPoint(Vector3 aimAt)
	{
		this.aimOverridePosition = aimAt;
	}

	// Token: 0x060018F9 RID: 6393 RVA: 0x000B5F18 File Offset: 0x000B4118
	public void ClearStationaryAimPoint()
	{
		this.aimOverridePosition = Vector3.zero;
	}

	// Token: 0x060018FA RID: 6394 RVA: 0x00007074 File Offset: 0x00005274
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x060018FB RID: 6395 RVA: 0x000B5F28 File Offset: 0x000B4128
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse result;
		using (TimeWarning.New("Create corpse", 0))
		{
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse("assets/prefabs/npc/scientist/scientist_corpse.prefab") as NPCPlayerCorpse;
			if (npcplayerCorpse)
			{
				npcplayerCorpse.transform.position = npcplayerCorpse.transform.position + Vector3.down * this.NavAgent.baseOffset;
				npcplayerCorpse.SetLootableIn(2f);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved5, base.HasPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash), false, true);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				npcplayerCorpse.TakeFrom(new global::ItemContainer[]
				{
					this.inventory.containerMain,
					this.inventory.containerWear,
					this.inventory.containerBelt
				});
				npcplayerCorpse.playerName = this.OverrideCorpseName();
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				npcplayerCorpse.TakeChildren(this);
				for (int i = 0; i < npcplayerCorpse.containers.Length; i++)
				{
					global::ItemContainer itemContainer = npcplayerCorpse.containers[i];
					if (i != 1)
					{
						itemContainer.Clear();
					}
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
					{
						for (int k = 0; k < lootSpawnSlot.numberToSpawn; k++)
						{
							if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
							{
								lootSpawnSlot.definition.SpawnIntoContainer(npcplayerCorpse.containers[0]);
							}
						}
					}
				}
			}
			result = npcplayerCorpse;
		}
		return result;
	}

	// Token: 0x060018FC RID: 6396 RVA: 0x000B60E0 File Offset: 0x000B42E0
	protected virtual string OverrideCorpseName()
	{
		return base.displayName;
	}

	// Token: 0x060018FD RID: 6397 RVA: 0x000B60E8 File Offset: 0x000B42E8
	public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = this.inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}

	// Token: 0x060018FE RID: 6398 RVA: 0x000B611E File Offset: 0x000B431E
	public bool IsThreat(global::BaseEntity entity)
	{
		return this.IsTarget(entity);
	}

	// Token: 0x060018FF RID: 6399 RVA: 0x000B6127 File Offset: 0x000B4327
	public bool IsTarget(global::BaseEntity entity)
	{
		return (entity is global::BasePlayer && !entity.IsNpc) || entity is BasePet || entity is ScarecrowNPC;
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x000B6150 File Offset: 0x000B4350
	public bool IsFriendly(global::BaseEntity entity)
	{
		return !(entity == null) && entity.prefabID == this.prefabID;
	}

	// Token: 0x06001901 RID: 6401 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool CanAttack(global::BaseEntity entity)
	{
		return true;
	}

	// Token: 0x06001902 RID: 6402 RVA: 0x000B616B File Offset: 0x000B436B
	public bool IsTargetInRange(global::BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.transform.position);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06001903 RID: 6403 RVA: 0x000B6198 File Offset: 0x000B4398
	public bool CanSeeTarget(global::BaseEntity entity)
	{
		global::BasePlayer basePlayer = entity as global::BasePlayer;
		if (basePlayer == null)
		{
			return true;
		}
		if (this.AdditionalLosBlockingLayer == 0)
		{
			return base.IsPlayerVisibleToUs(basePlayer, 1218519041);
		}
		return base.IsPlayerVisibleToUs(basePlayer, 1218519041 | 1 << this.AdditionalLosBlockingLayer);
	}

	// Token: 0x06001904 RID: 6404 RVA: 0x00007074 File Offset: 0x00005274
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x000B61E4 File Offset: 0x000B43E4
	public float CooldownDuration()
	{
		return 5f;
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x00007074 File Offset: 0x00005274
	public bool IsOnCooldown()
	{
		return false;
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool StartAttacking(global::BaseEntity entity)
	{
		return true;
	}

	// Token: 0x06001909 RID: 6409 RVA: 0x000059DD File Offset: 0x00003BDD
	public void StopAttacking()
	{
	}

	// Token: 0x0600190A RID: 6410 RVA: 0x00061087 File Offset: 0x0005F287
	public float GetAmmoFraction()
	{
		return this.AmmoFractionRemaining();
	}

	// Token: 0x0600190B RID: 6411 RVA: 0x000B61EC File Offset: 0x000B43EC
	public global::BaseEntity GetBestTarget()
	{
		global::BaseEntity result = null;
		float num = -1f;
		foreach (global::BaseEntity baseEntity in this.Brain.Senses.Players)
		{
			if (!(baseEntity == null) && baseEntity.Health() > 0f)
			{
				float value = Vector3.Distance(baseEntity.transform.position, base.transform.position);
				float num2 = 1f - Mathf.InverseLerp(1f, this.Brain.SenseRange, value);
				float value2 = Vector3.Dot((baseEntity.transform.position - this.eyes.position).normalized, this.eyes.BodyForward());
				num2 += Mathf.InverseLerp(this.Brain.VisionCone, 1f, value2) / 2f;
				num2 += (this.Brain.Senses.Memory.IsLOS(baseEntity) ? 2f : 0f);
				if (num2 > num)
				{
					result = baseEntity;
					num = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x000B6334 File Offset: 0x000B4534
	public void AttackTick(float delta, global::BaseEntity target, bool targetIsLOS)
	{
		BaseCombatEntity target2 = target as BaseCombatEntity;
		this.TickAttack(delta, target2, targetIsLOS);
	}

	// Token: 0x0600190D RID: 6413 RVA: 0x000B6351 File Offset: 0x000B4551
	public void UseHealingItem(global::Item item)
	{
		base.StartCoroutine(this.Heal(item));
	}

	// Token: 0x0600190E RID: 6414 RVA: 0x000B6361 File Offset: 0x000B4561
	private IEnumerator Heal(global::Item item)
	{
		base.UpdateActiveItem(item.uid);
		global::Item activeItem = base.GetActiveItem();
		MedicalTool heldItem = activeItem.GetHeldEntity() as MedicalTool;
		if (heldItem == null)
		{
			yield break;
		}
		yield return new WaitForSeconds(1f);
		heldItem.ServerUse();
		this.Heal(this.MaxHealth());
		yield return new WaitForSeconds(2f);
		this.EquipWeapon(false);
		yield break;
	}

	// Token: 0x0600190F RID: 6415 RVA: 0x000B6378 File Offset: 0x000B4578
	public global::Item FindHealingItem()
	{
		if (this.Brain == null)
		{
			return null;
		}
		if (!this.Brain.CanUseHealingItems)
		{
			return null;
		}
		if (this.inventory == null || this.inventory.containerBelt == null)
		{
			return null;
		}
		for (int i = 0; i < this.inventory.containerBelt.capacity; i++)
		{
			global::Item slot = this.inventory.containerBelt.GetSlot(i);
			if (slot != null && slot.amount > 1 && slot.GetHeldEntity() as MedicalTool != null)
			{
				return slot;
			}
		}
		return null;
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsOnGround()
	{
		return true;
	}

	// Token: 0x040011D5 RID: 4565
	[Header("LOS")]
	public int AdditionalLosBlockingLayer;

	// Token: 0x040011D6 RID: 4566
	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x040011D7 RID: 4567
	[Header("Damage")]
	public float aimConeScale = 2f;

	// Token: 0x040011D8 RID: 4568
	public float lastDismountTime;

	// Token: 0x040011DA RID: 4570
	[NonSerialized]
	protected bool lightsOn;

	// Token: 0x040011DB RID: 4571
	private float nextZoneSearchTime;

	// Token: 0x040011DC RID: 4572
	private AIInformationZone cachedInfoZone;

	// Token: 0x040011DD RID: 4573
	private float targetAimedDuration;

	// Token: 0x040011DE RID: 4574
	private float lastAimSetTime;

	// Token: 0x040011DF RID: 4575
	private Vector3 aimOverridePosition = Vector3.zero;
}
