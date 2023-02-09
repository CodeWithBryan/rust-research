using System;
using ConVar;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public class ScarecrowNPC : NPCPlayer, IAISenses, IAIAttack, IThinker
{
	// Token: 0x0600196B RID: 6507 RVA: 0x000271CD File Offset: 0x000253CD
	public override float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x000271CD File Offset: 0x000253CD
	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x000271CD File Offset: 0x000253CD
	public override float MaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x0600196E RID: 6510 RVA: 0x000B76DA File Offset: 0x000B58DA
	// (set) Token: 0x0600196F RID: 6511 RVA: 0x000B76E2 File Offset: 0x000B58E2
	public ScarecrowBrain Brain { get; protected set; }

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x06001970 RID: 6512 RVA: 0x000B76EB File Offset: 0x000B58EB
	public override BaseNpc.AiStatistics.FamilyEnum Family
	{
		get
		{
			return BaseNpc.AiStatistics.FamilyEnum.Murderer;
		}
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x000B76EE File Offset: 0x000B58EE
	public override void ServerInit()
	{
		base.ServerInit();
		this.Brain = base.GetComponent<ScarecrowBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.Add(this);
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x000B5909 File Offset: 0x000B3B09
	internal override void DoServerDestroy()
	{
		AIThinkManager.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x000B526A File Offset: 0x000B346A
	public virtual void TryThink()
	{
		base.ServerThink_Internal();
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x000B7711 File Offset: 0x000B5911
	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		if (this.Brain.ShouldServerThink())
		{
			this.Brain.DoThink();
		}
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x000B7732 File Offset: 0x000B5932
	public override string Categorize()
	{
		return "Scarecrow";
	}

	// Token: 0x06001976 RID: 6518 RVA: 0x000B773C File Offset: 0x000B593C
	public override void EquipWeapon(bool skipDeployDelay = false)
	{
		base.EquipWeapon(skipDeployDelay);
		global::HeldEntity heldEntity = base.GetHeldEntity();
		Chainsaw chainsaw;
		if (heldEntity != null && (chainsaw = (heldEntity as Chainsaw)) != null)
		{
			chainsaw.ServerNPCStart();
		}
	}

	// Token: 0x06001977 RID: 6519 RVA: 0x000B7770 File Offset: 0x000B5970
	public float EngagementRange()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * this.Brain.AttackRangeMultiplier;
		}
		return this.Brain.SenseRange;
	}

	// Token: 0x06001978 RID: 6520 RVA: 0x000B77BF File Offset: 0x000B59BF
	public bool IsThreat(global::BaseEntity entity)
	{
		return this.IsTarget(entity);
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x00060ED8 File Offset: 0x0005F0D8
	public bool IsTarget(global::BaseEntity entity)
	{
		return entity is global::BasePlayer && !entity.IsNpc;
	}

	// Token: 0x0600197A RID: 6522 RVA: 0x00007074 File Offset: 0x00005274
	public bool IsFriendly(global::BaseEntity entity)
	{
		return false;
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x000B77C8 File Offset: 0x000B59C8
	public bool CanAttack(global::BaseEntity entity)
	{
		float num;
		global::BasePlayer basePlayer;
		return !(entity == null) && !this.NeedsToReload() && !this.IsOnCooldown() && this.IsTargetInRange(entity, out num) && !base.InSafeZone() && ((basePlayer = (entity as global::BasePlayer)) == null || !basePlayer.InSafeZone()) && this.CanSeeTarget(entity);
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x000B7829 File Offset: 0x000B5A29
	public bool IsTargetInRange(global::BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.transform.position);
		return dist <= this.EngagementRange();
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x00060F9C File Offset: 0x0005F19C
	public bool CanSeeTarget(global::BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x0600197E RID: 6526 RVA: 0x00007074 File Offset: 0x00005274
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06001980 RID: 6528 RVA: 0x000B7855 File Offset: 0x000B5A55
	public float CooldownDuration()
	{
		return this.BaseAttackRate;
	}

	// Token: 0x06001981 RID: 6529 RVA: 0x000B7860 File Offset: 0x000B5A60
	public bool IsOnCooldown()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		return !attackEntity || attackEntity.HasAttackCooldown();
	}

	// Token: 0x06001982 RID: 6530 RVA: 0x000B7884 File Offset: 0x000B5A84
	public bool StartAttacking(global::BaseEntity target)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		if (baseCombatEntity == null)
		{
			return false;
		}
		this.Attack(baseCombatEntity);
		return true;
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x000B78AC File Offset: 0x000B5AAC
	private void Attack(BaseCombatEntity target)
	{
		if (target == null)
		{
			return;
		}
		Vector3 vector = target.ServerPosition - this.ServerPosition;
		if (vector.magnitude > 0.001f)
		{
			this.ServerRotation = Quaternion.LookRotation(vector.normalized);
		}
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			attackEntity.ServerUse();
		}
	}

	// Token: 0x06001984 RID: 6532 RVA: 0x000059DD File Offset: 0x00003BDD
	public void StopAttacking()
	{
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x00061087 File Offset: 0x0005F287
	public float GetAmmoFraction()
	{
		return this.AmmoFractionRemaining();
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x0002A0CF File Offset: 0x000282CF
	public global::BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x000059DD File Offset: 0x00003BDD
	public void AttackTick(float delta, global::BaseEntity target, bool targetIsLOS)
	{
	}

	// Token: 0x06001988 RID: 6536 RVA: 0x00007074 File Offset: 0x00005274
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x06001989 RID: 6537 RVA: 0x000B790C File Offset: 0x000B5B0C
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse result;
		using (TimeWarning.New("Create corpse", 0))
		{
			string strCorpsePrefab = "assets/prefabs/npc/murderer/murderer_corpse.prefab";
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse(strCorpsePrefab) as NPCPlayerCorpse;
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
				npcplayerCorpse.playerName = "Scarecrow";
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				global::ItemContainer[] containers = npcplayerCorpse.containers;
				for (int i = 0; i < containers.Length; i++)
				{
					containers[i].Clear();
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
					{
						for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
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

	// Token: 0x0600198A RID: 6538 RVA: 0x000B7AB8 File Offset: 0x000B5CB8
	public override void Hurt(HitInfo info)
	{
		if (!info.isHeadshot)
		{
			if ((info.InitiatorPlayer != null && !info.InitiatorPlayer.IsNpc) || (info.InitiatorPlayer == null && info.Initiator != null && info.Initiator.IsNpc))
			{
				info.damageTypes.ScaleAll(Halloween.scarecrow_body_dmg_modifier);
			}
			else
			{
				info.damageTypes.ScaleAll(2f);
			}
		}
		base.Hurt(info);
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x000B60E8 File Offset: 0x000B42E8
	public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = this.inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}

	// Token: 0x040011FF RID: 4607
	public float BaseAttackRate = 2f;

	// Token: 0x04001200 RID: 4608
	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x04001201 RID: 4609
	public static float NextBeanCanAllowedTime;

	// Token: 0x04001202 RID: 4610
	public bool BlockClothingOnCorpse;

	// Token: 0x04001203 RID: 4611
	public bool RoamAroundHomePoint;
}
