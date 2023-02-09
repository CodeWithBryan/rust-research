using System;
using System.Collections;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class FrankensteinPet : BasePet, IAISenses, IAIAttack
{
	// Token: 0x06000AD6 RID: 2774 RVA: 0x00060D80 File Offset: 0x0005EF80
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FrankensteinPet.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x00060DC0 File Offset: 0x0005EFC0
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		base.InvokeRandomized(new Action(this.TickDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x00060DFC File Offset: 0x0005EFFC
	public IEnumerator DelayEquipWeapon(ItemDefinition item, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (this.inventory == null)
		{
			yield break;
		}
		if (this.inventory.containerBelt == null)
		{
			yield break;
		}
		if (item == null)
		{
			yield break;
		}
		this.inventory.GiveItem(ItemManager.Create(item, 1, 0UL), this.inventory.containerBelt);
		this.EquipWeapon(false);
		yield break;
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x00060E1C File Offset: 0x0005F01C
	private void TickDecay()
	{
		BasePlayer basePlayer = BasePlayer.FindByID(base.OwnerID);
		if (basePlayer != null && !basePlayer.IsSleeping())
		{
			return;
		}
		if (base.healthFraction <= 0f || base.IsDestroyed)
		{
			return;
		}
		float num = 1f / FrankensteinPet.decayminutes;
		float amount = this.MaxHealth() * num;
		base.Hurt(amount, DamageType.Decay, this, false);
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x00060E80 File Offset: 0x0005F080
	public float EngagementRange()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * base.Brain.AttackRangeMultiplier;
		}
		return base.Brain.SenseRange;
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x00060ECF File Offset: 0x0005F0CF
	public bool IsThreat(BaseEntity entity)
	{
		return this.IsTarget(entity);
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x00060ED8 File Offset: 0x0005F0D8
	public bool IsTarget(BaseEntity entity)
	{
		return entity is BasePlayer && !entity.IsNpc;
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x00007074 File Offset: 0x00005274
	public bool IsFriendly(BaseEntity entity)
	{
		return false;
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x00060EF0 File Offset: 0x0005F0F0
	public bool CanAttack(BaseEntity entity)
	{
		float num;
		BasePlayer basePlayer;
		return !(entity == null) && entity.gameObject.layer != 21 && entity.gameObject.layer != 8 && !this.NeedsToReload() && !this.IsOnCooldown() && this.IsTargetInRange(entity, out num) && !base.InSafeZone() && ((basePlayer = (entity as BasePlayer)) == null || !basePlayer.InSafeZone()) && this.CanSeeTarget(entity);
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x00060F70 File Offset: 0x0005F170
	public bool IsTargetInRange(BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.transform.position);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x00060F9C File Offset: 0x0005F19C
	public bool CanSeeTarget(BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x00007074 File Offset: 0x00005274
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x00060FC5 File Offset: 0x0005F1C5
	public float CooldownDuration()
	{
		return this.BaseAttackRate;
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x00060FCD File Offset: 0x0005F1CD
	public bool IsOnCooldown()
	{
		return Time.realtimeSinceStartup < this.nextAttackTime;
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x00060FDC File Offset: 0x0005F1DC
	public bool StartAttacking(BaseEntity target)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		if (baseCombatEntity == null)
		{
			return false;
		}
		this.Attack(baseCombatEntity);
		return true;
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x00061004 File Offset: 0x0005F204
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
		target.Hurt(this.BaseAttackDamge, this.AttackDamageType, this, true);
		base.SignalBroadcast(BaseEntity.Signal.Attack, null);
		base.ClientRPC(null, "OnAttack");
		this.nextAttackTime = Time.realtimeSinceStartup + this.CooldownDuration();
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x000059DD File Offset: 0x00003BDD
	public void StopAttacking()
	{
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x00061087 File Offset: 0x0005F287
	public float GetAmmoFraction()
	{
		return this.AmmoFractionRemaining();
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0002A0CF File Offset: 0x000282CF
	public BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x000059DD File Offset: 0x00003BDD
	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x00007074 File Offset: 0x00005274
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x00061090 File Offset: 0x0005F290
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse result;
		using (TimeWarning.New("Create corpse", 0))
		{
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse("assets/rust.ai/agents/NPCPlayer/pet/frankensteinpet_corpse.prefab") as NPCPlayerCorpse;
			if (npcplayerCorpse)
			{
				npcplayerCorpse.transform.position = npcplayerCorpse.transform.position + Vector3.down * this.NavAgent.baseOffset;
				npcplayerCorpse.SetLootableIn(2f);
				npcplayerCorpse.SetFlag(BaseEntity.Flags.Reserved5, base.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
				npcplayerCorpse.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
				npcplayerCorpse.TakeFrom(new ItemContainer[]
				{
					this.inventory.containerMain,
					this.inventory.containerWear,
					this.inventory.containerBelt
				});
				npcplayerCorpse.playerName = this.OverrideCorpseName();
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				ItemContainer[] containers = npcplayerCorpse.containers;
				for (int i = 0; i < containers.Length; i++)
				{
					containers[i].Clear();
				}
			}
			result = npcplayerCorpse;
		}
		return result;
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x000611B8 File Offset: 0x0005F3B8
	protected virtual string OverrideCorpseName()
	{
		return "Frankenstein";
	}

	// Token: 0x04000717 RID: 1815
	[Header("Frankenstein")]
	[ServerVar(Help = "How long before a Frankenstein Pet dies un controlled and not asleep on table")]
	public static float decayminutes = 180f;

	// Token: 0x04000718 RID: 1816
	[Header("Audio")]
	public SoundDefinition AttackVocalSFX;

	// Token: 0x04000719 RID: 1817
	private float nextAttackTime;
}
