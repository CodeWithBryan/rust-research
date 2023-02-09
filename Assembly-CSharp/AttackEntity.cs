using System;
using ConVar;
using UnityEngine;

// Token: 0x0200038A RID: 906
public class AttackEntity : HeldEntity
{
	// Token: 0x06001FAF RID: 8111 RVA: 0x00029180 File Offset: 0x00027380
	public virtual Vector3 GetInheritedVelocity(BasePlayer player, Vector3 direction)
	{
		return Vector3.zero;
	}

	// Token: 0x06001FB0 RID: 8112 RVA: 0x00026FFC File Offset: 0x000251FC
	public virtual float AmmoFraction()
	{
		return 0f;
	}

	// Token: 0x06001FB1 RID: 8113 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool CanReload()
	{
		return false;
	}

	// Token: 0x06001FB2 RID: 8114 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool ServerIsReloading()
	{
		return false;
	}

	// Token: 0x06001FB3 RID: 8115 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ServerReload()
	{
	}

	// Token: 0x06001FB4 RID: 8116 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void TopUpAmmo()
	{
	}

	// Token: 0x06001FB5 RID: 8117 RVA: 0x0003421C File Offset: 0x0003241C
	public virtual Vector3 ModifyAIAim(Vector3 eulerInput, float swayModifier = 1f)
	{
		return eulerInput;
	}

	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06001FB6 RID: 8118 RVA: 0x000D0E7F File Offset: 0x000CF07F
	public float NextAttackTime
	{
		get
		{
			return this.nextAttackTime;
		}
	}

	// Token: 0x06001FB7 RID: 8119 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void GetAttackStats(HitInfo info)
	{
	}

	// Token: 0x06001FB8 RID: 8120 RVA: 0x000D0E87 File Offset: 0x000CF087
	protected void StartAttackCooldownRaw(float cooldown)
	{
		this.nextAttackTime = UnityEngine.Time.time + cooldown;
	}

	// Token: 0x06001FB9 RID: 8121 RVA: 0x000D0E96 File Offset: 0x000CF096
	protected void StartAttackCooldown(float cooldown)
	{
		this.nextAttackTime = this.CalculateCooldownTime(this.nextAttackTime, cooldown, true);
	}

	// Token: 0x06001FBA RID: 8122 RVA: 0x000D0EAC File Offset: 0x000CF0AC
	public void ResetAttackCooldown()
	{
		this.nextAttackTime = float.NegativeInfinity;
	}

	// Token: 0x06001FBB RID: 8123 RVA: 0x000D0EB9 File Offset: 0x000CF0B9
	public bool HasAttackCooldown()
	{
		return UnityEngine.Time.time < this.nextAttackTime;
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x000D0EC8 File Offset: 0x000CF0C8
	protected float GetAttackCooldown()
	{
		return Mathf.Max(this.nextAttackTime - UnityEngine.Time.time, 0f);
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x000D0EE0 File Offset: 0x000CF0E0
	protected float GetAttackIdle()
	{
		return Mathf.Max(UnityEngine.Time.time - this.nextAttackTime, 0f);
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x000D0EF8 File Offset: 0x000CF0F8
	protected float CalculateCooldownTime(float nextTime, float cooldown, bool catchup)
	{
		float time = UnityEngine.Time.time;
		float num = 0f;
		if (base.isServer)
		{
			BasePlayer ownerPlayer = base.GetOwnerPlayer();
			num += 0.1f;
			num += cooldown * 0.1f;
			num += (ownerPlayer ? ownerPlayer.desyncTimeClamped : 0.1f);
			num += Mathf.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime);
		}
		if (nextTime < 0f)
		{
			nextTime = Mathf.Max(0f, time + cooldown - num);
		}
		else if (time - nextTime <= num)
		{
			nextTime = Mathf.Min(nextTime + cooldown, time + cooldown);
		}
		else
		{
			nextTime = Mathf.Max(nextTime + cooldown, time + cooldown - num);
		}
		return nextTime;
	}

	// Token: 0x06001FBF RID: 8127 RVA: 0x000D0F9C File Offset: 0x000CF19C
	protected bool VerifyClientRPC(BasePlayer player)
	{
		if (player == null)
		{
			Debug.LogWarning("Received RPC from null player");
			return false;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Owner not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "owner_missing");
			return false;
		}
		if (ownerPlayer != player)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player mismatch (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_mismatch");
			return false;
		}
		if (player.IsDead())
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player dead (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_dead");
			return false;
		}
		if (player.IsWounded())
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player down (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_down");
			return false;
		}
		if (player.IsSleeping())
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player sleeping (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_sleeping");
			return false;
		}
		if (player.desyncTimeRaw > ConVar.AntiHack.maxdesync)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, string.Concat(new object[]
			{
				"Player stalled (",
				base.ShortPrefabName,
				" with ",
				player.desyncTimeRaw,
				"s)"
			}));
			player.stats.combat.LogInvalid(player, this, "player_stalled");
			return false;
		}
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Item not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "item_missing");
			return false;
		}
		if (ownerItem.isBroken)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Item broken (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "item_broken");
			return false;
		}
		return true;
	}

	// Token: 0x06001FC0 RID: 8128 RVA: 0x000D11DC File Offset: 0x000CF3DC
	protected virtual bool VerifyClientAttack(BasePlayer player)
	{
		if (!this.VerifyClientRPC(player))
		{
			return false;
		}
		if (this.HasAttackCooldown())
		{
			global::AntiHack.Log(player, AntiHackType.CooldownHack, string.Concat(new object[]
			{
				"T-",
				this.GetAttackCooldown(),
				"s (",
				base.ShortPrefabName,
				")"
			}));
			player.stats.combat.LogInvalid(player, this, "attack_cooldown");
			return false;
		}
		return true;
	}

	// Token: 0x06001FC1 RID: 8129 RVA: 0x000D1258 File Offset: 0x000CF458
	protected bool ValidateEyePos(BasePlayer player, Vector3 eyePos)
	{
		bool flag = true;
		if (eyePos.IsNaNOrInfinity())
		{
			string shortPrefabName = base.ShortPrefabName;
			global::AntiHack.Log(player, AntiHackType.EyeHack, "Contains NaN (" + shortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "eye_nan");
			flag = false;
		}
		if (ConVar.AntiHack.eye_protection > 0)
		{
			float num = 1f + ConVar.AntiHack.eye_forgiveness;
			float eye_clientframes = ConVar.AntiHack.eye_clientframes;
			float eye_serverframes = ConVar.AntiHack.eye_serverframes;
			float num2 = eye_clientframes / 60f;
			float num3 = eye_serverframes * Mathx.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime, UnityEngine.Time.fixedDeltaTime);
			float num4 = (player.desyncTimeClamped + num2 + num3) * num;
			int layerMask = ConVar.AntiHack.eye_terraincheck ? 10551296 : 2162688;
			if (ConVar.AntiHack.eye_protection >= 1)
			{
				float num5 = player.MaxVelocity() + player.GetParentVelocity().magnitude;
				float num6 = player.BoundsPadding() + num4 * num5;
				float num7 = Vector3.Distance(player.eyes.position, eyePos);
				if (num7 > num6)
				{
					string shortPrefabName2 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[]
					{
						"Distance (",
						shortPrefabName2,
						" on attack with ",
						num7,
						"m > ",
						num6,
						"m)"
					}));
					player.stats.combat.LogInvalid(player, this, "eye_distance");
					flag = false;
				}
			}
			if (ConVar.AntiHack.eye_protection >= 3)
			{
				float num8 = Mathf.Abs(player.GetMountVelocity().y + player.GetParentVelocity().y);
				float num9 = player.BoundsPadding() + num4 * num8 + player.GetJumpHeight();
				float num10 = Mathf.Abs(player.eyes.position.y - eyePos.y);
				if (num10 > num9)
				{
					string shortPrefabName3 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[]
					{
						"Altitude (",
						shortPrefabName3,
						" on attack with ",
						num10,
						"m > ",
						num9,
						"m)"
					}));
					player.stats.combat.LogInvalid(player, this, "eye_altitude");
					flag = false;
				}
			}
			if (ConVar.AntiHack.eye_protection >= 2)
			{
				Vector3 center = player.eyes.center;
				Vector3 position = player.eyes.position;
				if (!GamePhysics.LineOfSightRadius(center, position, layerMask, ConVar.AntiHack.eye_losradius, null) || !GamePhysics.LineOfSightRadius(position, eyePos, layerMask, ConVar.AntiHack.eye_losradius, null))
				{
					string shortPrefabName4 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[]
					{
						"Line of sight (",
						shortPrefabName4,
						" on attack) ",
						center,
						" ",
						position,
						" ",
						eyePos
					}));
					player.stats.combat.LogInvalid(player, this, "eye_los");
					flag = false;
				}
			}
			if (ConVar.AntiHack.eye_protection >= 4 && !player.HasParent())
			{
				Vector3 position2 = player.eyes.position;
				float num11 = Vector3.Distance(position2, eyePos);
				if (num11 > ConVar.AntiHack.eye_noclip_cutoff)
				{
					if (global::AntiHack.TestNoClipping(player, position2, eyePos, player.NoClipRadius(ConVar.AntiHack.eye_noclip_margin), ConVar.AntiHack.eye_noclip_backtracking, ConVar.AntiHack.noclip_protection >= 2, false, null))
					{
						string shortPrefabName5 = base.ShortPrefabName;
						global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[]
						{
							"NoClip (",
							shortPrefabName5,
							" on attack) ",
							position2,
							" ",
							eyePos
						}));
						player.stats.combat.LogInvalid(player, this, "eye_noclip");
						flag = false;
					}
				}
				else if (num11 > 0.01f && global::AntiHack.TestNoClipping(player, position2, eyePos, 0.01f, ConVar.AntiHack.eye_noclip_backtracking, ConVar.AntiHack.noclip_protection >= 2, false, null))
				{
					string shortPrefabName6 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[]
					{
						"NoClip (",
						shortPrefabName6,
						" on attack) ",
						position2,
						" ",
						eyePos
					}));
					player.stats.combat.LogInvalid(player, this, "eye_noclip");
					flag = false;
				}
			}
			if (!flag)
			{
				global::AntiHack.AddViolation(player, AntiHackType.EyeHack, ConVar.AntiHack.eye_penalty);
			}
			else if (ConVar.AntiHack.eye_protection >= 5 && !player.HasParent() && !player.isMounted)
			{
				player.eyeHistory.PushBack(eyePos);
			}
		}
		return flag;
	}

	// Token: 0x06001FC2 RID: 8130 RVA: 0x000D16F3 File Offset: 0x000CF8F3
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		this.StartAttackCooldown(this.deployDelay * 0.9f);
	}

	// Token: 0x040018E7 RID: 6375
	[Header("Attack Entity")]
	public float deployDelay = 1f;

	// Token: 0x040018E8 RID: 6376
	public float repeatDelay = 0.5f;

	// Token: 0x040018E9 RID: 6377
	public float animationDelay;

	// Token: 0x040018EA RID: 6378
	[Header("NPCUsage")]
	public float effectiveRange = 1f;

	// Token: 0x040018EB RID: 6379
	public float npcDamageScale = 1f;

	// Token: 0x040018EC RID: 6380
	public float attackLengthMin = -1f;

	// Token: 0x040018ED RID: 6381
	public float attackLengthMax = -1f;

	// Token: 0x040018EE RID: 6382
	public float attackSpacing;

	// Token: 0x040018EF RID: 6383
	public float aiAimSwayOffset;

	// Token: 0x040018F0 RID: 6384
	public float aiAimCone;

	// Token: 0x040018F1 RID: 6385
	public bool aiOnlyInRange;

	// Token: 0x040018F2 RID: 6386
	public float CloseRangeAddition;

	// Token: 0x040018F3 RID: 6387
	public float MediumRangeAddition;

	// Token: 0x040018F4 RID: 6388
	public float LongRangeAddition;

	// Token: 0x040018F5 RID: 6389
	public bool CanUseAtMediumRange = true;

	// Token: 0x040018F6 RID: 6390
	public bool CanUseAtLongRange = true;

	// Token: 0x040018F7 RID: 6391
	public SoundDefinition[] reloadSounds;

	// Token: 0x040018F8 RID: 6392
	public SoundDefinition thirdPersonMeleeSound;

	// Token: 0x040018F9 RID: 6393
	[Header("Recoil Compensation")]
	public float recoilCompDelayOverride;

	// Token: 0x040018FA RID: 6394
	public bool wantsRecoilComp;

	// Token: 0x040018FB RID: 6395
	private float nextAttackTime = float.NegativeInfinity;
}
