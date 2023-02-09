using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200003C RID: 60
public class BaseMelee : AttackEntity
{
	// Token: 0x060003DB RID: 987 RVA: 0x000300E4 File Offset: 0x0002E2E4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseMelee.OnRpcMessage", 0))
		{
			if (rpc == 3168282921U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CLProject ");
				}
				using (TimeWarning.New("CLProject", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3168282921U, "CLProject", this, player))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3168282921U, "CLProject", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CLProject(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in CLProject");
					}
				}
				return true;
			}
			if (rpc == 4088326849U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - PlayerAttack ");
				}
				using (TimeWarning.New("PlayerAttack", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(4088326849U, "PlayerAttack", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.PlayerAttack(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in PlayerAttack");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003DC RID: 988 RVA: 0x000303F4 File Offset: 0x0002E5F4
	public override Vector3 GetInheritedVelocity(global::BasePlayer player, Vector3 direction)
	{
		return player.GetInheritedThrowVelocity(direction);
	}

	// Token: 0x060003DD RID: 989 RVA: 0x00030400 File Offset: 0x0002E600
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void CLProject(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return;
		}
		if (player == null)
		{
			return;
		}
		if (player.IsHeadUnderwater())
		{
			return;
		}
		if (!this.canThrowAsProjectile)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Not throwable (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "not_throwable");
			return;
		}
		global::Item item = this.GetItem();
		if (item == null)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Item not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "item_missing");
			return;
		}
		ItemModProjectile component = item.info.GetComponent<ItemModProjectile>();
		if (component == null)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "mod_missing");
			return;
		}
		ProjectileShoot projectileShoot = ProjectileShoot.Deserialize(msg.read);
		if (projectileShoot.projectiles.Count != 1)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Projectile count mismatch (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "count_mismatch");
			return;
		}
		player.CleanupExpiredProjectiles();
		foreach (ProjectileShoot.Projectile projectile in projectileShoot.projectiles)
		{
			if (player.HasFiredProjectile(projectile.projectileID))
			{
				global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Duplicate ID (" + projectile.projectileID + ")");
				player.stats.combat.LogInvalid(player, this, "duplicate_id");
			}
			else if (base.ValidateEyePos(player, projectile.startPos))
			{
				player.NoteFiredProjectile(projectile.projectileID, projectile.startPos, projectile.startVel, this, item.info, item);
				Effect effect = new Effect();
				effect.Init(Effect.Type.Projectile, projectile.startPos, projectile.startVel, msg.connection);
				effect.scale = 1f;
				effect.pooledString = component.projectileObject.resourcePath;
				effect.number = projectile.seed;
				EffectNetwork.Send(effect);
			}
		}
		if (projectileShoot != null)
		{
			projectileShoot.Dispose();
		}
		item.SetParent(null);
		if (this.canAiHearIt)
		{
			float num = 0f;
			if (component.projectileObject != null)
			{
				GameObject gameObject = component.projectileObject.Get();
				if (gameObject != null)
				{
					Projectile component2 = gameObject.GetComponent<Projectile>();
					if (component2 != null)
					{
						foreach (DamageTypeEntry damageTypeEntry in component2.damageTypes)
						{
							num += damageTypeEntry.amount;
						}
					}
				}
			}
			if (player != null)
			{
				Sense.Stimulate(new Sensation
				{
					Type = SensationType.ThrownWeapon,
					Position = player.transform.position,
					Radius = 50f,
					DamagePotential = num,
					InitiatorPlayer = player,
					Initiator = player
				});
			}
		}
	}

	// Token: 0x060003DE RID: 990 RVA: 0x0003075C File Offset: 0x0002E95C
	public override void GetAttackStats(HitInfo info)
	{
		info.damageTypes.Add(this.damageTypes);
		info.CanGather = this.gathering.Any();
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00030780 File Offset: 0x0002E980
	public virtual void DoAttackShared(HitInfo info)
	{
		this.GetAttackStats(info);
		if (info.HitEntity != null)
		{
			using (TimeWarning.New("OnAttacked", 50))
			{
				info.HitEntity.OnAttacked(info);
			}
		}
		if (info.DoHitEffects)
		{
			if (base.isServer)
			{
				using (TimeWarning.New("ImpactEffect", 20))
				{
					Effect.server.ImpactEffect(info);
					goto IL_88;
				}
			}
			using (TimeWarning.New("ImpactEffect", 20))
			{
				Effect.client.ImpactEffect(info);
			}
		}
		IL_88:
		if (base.isServer && !base.IsDestroyed)
		{
			using (TimeWarning.New("UpdateItemCondition", 50))
			{
				this.UpdateItemCondition(info);
			}
			base.StartAttackCooldown(this.repeatDelay);
		}
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00030888 File Offset: 0x0002EA88
	public ResourceDispenser.GatherPropertyEntry GetGatherInfoFromIndex(ResourceDispenser.GatherType index)
	{
		return this.gathering.GetFromIndex(index);
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool CanHit(HitTest info)
	{
		return true;
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00030898 File Offset: 0x0002EA98
	public float TotalDamage()
	{
		float num = 0f;
		foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
		{
			if (damageTypeEntry.amount > 0f)
			{
				num += damageTypeEntry.amount;
			}
		}
		return num;
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x00030904 File Offset: 0x0002EB04
	public bool IsItemBroken()
	{
		global::Item ownerItem = base.GetOwnerItem();
		return ownerItem == null || ownerItem.isBroken;
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00030924 File Offset: 0x0002EB24
	public void LoseCondition(float amount)
	{
		global::Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(amount);
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x000062DD File Offset: 0x000044DD
	public virtual float GetConditionLoss()
	{
		return 1f;
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x00030944 File Offset: 0x0002EB44
	public void UpdateItemCondition(HitInfo info)
	{
		global::Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null || !ownerItem.hasCondition)
		{
			return;
		}
		if (info != null && info.DidHit && !info.DidGather)
		{
			float num = this.GetConditionLoss();
			float num2 = 0f;
			foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
			{
				if (damageTypeEntry.amount > 0f)
				{
					num2 += Mathf.Clamp(damageTypeEntry.amount - info.damageTypes.Get(damageTypeEntry.type), 0f, damageTypeEntry.amount);
				}
			}
			num += num2 * 0.2f;
			ownerItem.LoseCondition(num);
		}
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x00030A1C File Offset: 0x0002EC1C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void PlayerAttack(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return;
		}
		using (TimeWarning.New("PlayerAttack", 50))
		{
			using (PlayerAttack playerAttack = ProtoBuf.PlayerAttack.Deserialize(msg.read))
			{
				if (playerAttack != null)
				{
					HitInfo hitInfo = Facepunch.Pool.Get<HitInfo>();
					hitInfo.LoadFromAttack(playerAttack.attack, true);
					hitInfo.Initiator = player;
					hitInfo.Weapon = this;
					hitInfo.WeaponPrefab = this;
					hitInfo.Predicted = msg.connection;
					hitInfo.damageProperties = this.damageProperties;
					if (hitInfo.IsNaNOrInfinity())
					{
						string shortPrefabName = base.ShortPrefabName;
						global::AntiHack.Log(player, AntiHackType.MeleeHack, "Contains NaN (" + shortPrefabName + ")");
						player.stats.combat.LogInvalid(hitInfo, "melee_nan");
					}
					else
					{
						global::BaseEntity hitEntity = hitInfo.HitEntity;
						global::BasePlayer basePlayer = hitInfo.HitEntity as global::BasePlayer;
						bool flag = basePlayer != null;
						bool flag2 = flag && basePlayer.IsSleeping();
						bool flag3 = flag && basePlayer.IsWounded();
						bool flag4 = flag && basePlayer.isMounted;
						bool flag5 = flag && basePlayer.HasParent();
						bool flag6 = hitEntity != null;
						bool flag7 = flag6 && hitEntity.IsNpc;
						if (ConVar.AntiHack.melee_protection > 0)
						{
							bool flag8 = true;
							float num = 1f + ConVar.AntiHack.melee_forgiveness;
							float melee_clientframes = ConVar.AntiHack.melee_clientframes;
							float melee_serverframes = ConVar.AntiHack.melee_serverframes;
							float num2 = melee_clientframes / 60f;
							float num3 = melee_serverframes * Mathx.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime, UnityEngine.Time.fixedDeltaTime);
							float num4 = (player.desyncTimeClamped + num2 + num3) * num;
							int layerMask = ConVar.AntiHack.melee_terraincheck ? 10551296 : 2162688;
							if (flag && hitInfo.boneArea == (HitArea)(-1))
							{
								string shortPrefabName2 = base.ShortPrefabName;
								string shortPrefabName3 = basePlayer.ShortPrefabName;
								global::AntiHack.Log(player, AntiHackType.MeleeHack, string.Concat(new object[]
								{
									"Bone is invalid  (",
									shortPrefabName2,
									" on ",
									shortPrefabName3,
									" bone ",
									hitInfo.HitBone,
									")"
								}));
								player.stats.combat.LogInvalid(hitInfo, "melee_bone");
								flag8 = false;
							}
							if (ConVar.AntiHack.melee_protection >= 2)
							{
								if (flag6)
								{
									float num5 = hitEntity.MaxVelocity() + hitEntity.GetParentVelocity().magnitude;
									float num6 = hitEntity.BoundsPadding() + num4 * num5;
									float num7 = hitEntity.Distance(hitInfo.HitPositionWorld);
									if (num7 > num6)
									{
										string shortPrefabName4 = base.ShortPrefabName;
										string shortPrefabName5 = hitEntity.ShortPrefabName;
										global::AntiHack.Log(player, AntiHackType.MeleeHack, string.Concat(new object[]
										{
											"Entity too far away (",
											shortPrefabName4,
											" on ",
											shortPrefabName5,
											" with ",
											num7,
											"m > ",
											num6,
											"m in ",
											num4,
											"s)"
										}));
										player.stats.combat.LogInvalid(hitInfo, "melee_target");
										flag8 = false;
									}
								}
								if (ConVar.AntiHack.melee_protection >= 4 && flag8 && flag && !flag7 && !flag2 && !flag3 && !flag4 && !flag5)
								{
									float magnitude = basePlayer.GetParentVelocity().magnitude;
									float num8 = basePlayer.BoundsPadding() + num4 * magnitude + ConVar.AntiHack.tickhistoryforgiveness;
									float num9 = basePlayer.tickHistory.Distance(basePlayer, hitInfo.HitPositionWorld);
									if (num9 > num8)
									{
										string shortPrefabName6 = base.ShortPrefabName;
										string shortPrefabName7 = basePlayer.ShortPrefabName;
										global::AntiHack.Log(player, AntiHackType.ProjectileHack, string.Concat(new object[]
										{
											"Player too far away (",
											shortPrefabName6,
											" on ",
											shortPrefabName7,
											" with ",
											num9,
											"m > ",
											num8,
											"m in ",
											num4,
											"s)"
										}));
										player.stats.combat.LogInvalid(hitInfo, "player_distance");
										flag8 = false;
									}
								}
							}
							if (ConVar.AntiHack.melee_protection >= 1)
							{
								if (ConVar.AntiHack.melee_protection >= 4)
								{
									float magnitude2 = player.GetParentVelocity().magnitude;
									float num10 = player.BoundsPadding() + num4 * magnitude2 + num * this.maxDistance;
									float num11 = player.tickHistory.Distance(player, hitInfo.HitPositionWorld);
									if (num11 > num10)
									{
										string shortPrefabName8 = base.ShortPrefabName;
										string text = flag6 ? hitEntity.ShortPrefabName : "world";
										global::AntiHack.Log(player, AntiHackType.MeleeHack, string.Concat(new object[]
										{
											"Initiator too far away (",
											shortPrefabName8,
											" on ",
											text,
											" with ",
											num11,
											"m > ",
											num10,
											"m in ",
											num4,
											"s)"
										}));
										player.stats.combat.LogInvalid(hitInfo, "melee_initiator");
										flag8 = false;
									}
								}
								else
								{
									float num12 = player.MaxVelocity() + player.GetParentVelocity().magnitude;
									float num13 = player.BoundsPadding() + num4 * num12 + num * this.maxDistance;
									float num14 = player.Distance(hitInfo.HitPositionWorld);
									if (num14 > num13)
									{
										string shortPrefabName9 = base.ShortPrefabName;
										string text2 = flag6 ? hitEntity.ShortPrefabName : "world";
										global::AntiHack.Log(player, AntiHackType.MeleeHack, string.Concat(new object[]
										{
											"Initiator too far away (",
											shortPrefabName9,
											" on ",
											text2,
											" with ",
											num14,
											"m > ",
											num13,
											"m in ",
											num4,
											"s)"
										}));
										player.stats.combat.LogInvalid(hitInfo, "melee_initiator");
										flag8 = false;
									}
								}
							}
							if (ConVar.AntiHack.melee_protection >= 3)
							{
								if (flag6)
								{
									Vector3 pointStart = hitInfo.PointStart;
									Vector3 hitPositionWorld = hitInfo.HitPositionWorld;
									Vector3 center = player.eyes.center;
									Vector3 position = player.eyes.position;
									Vector3 vector = pointStart;
									Vector3 vector2 = hitInfo.PositionOnRay(hitPositionWorld) + hitInfo.HitNormalWorld.normalized * 0.001f;
									Vector3 vector3 = hitPositionWorld;
									bool flag9 = GamePhysics.LineOfSight(center, position, layerMask, null) && GamePhysics.LineOfSight(position, vector, layerMask, null) && GamePhysics.LineOfSight(vector, vector2, layerMask, null) && GamePhysics.LineOfSight(vector2, vector3, layerMask, hitEntity);
									if (!flag9)
									{
										player.stats.Add("hit_" + hitEntity.Categorize() + "_indirect_los", 1, global::Stats.Server);
									}
									else
									{
										player.stats.Add("hit_" + hitEntity.Categorize() + "_direct_los", 1, global::Stats.Server);
									}
									if (!flag9)
									{
										string shortPrefabName10 = base.ShortPrefabName;
										string shortPrefabName11 = hitEntity.ShortPrefabName;
										global::AntiHack.Log(player, AntiHackType.MeleeHack, string.Concat(new object[]
										{
											"Line of sight (",
											shortPrefabName10,
											" on ",
											shortPrefabName11,
											") ",
											center,
											" ",
											position,
											" ",
											vector,
											" ",
											vector2,
											" ",
											vector3
										}));
										player.stats.combat.LogInvalid(hitInfo, "melee_los");
										flag8 = false;
									}
								}
								if (flag8 && flag && !flag7)
								{
									Vector3 hitPositionWorld2 = hitInfo.HitPositionWorld;
									Vector3 position2 = basePlayer.eyes.position;
									Vector3 vector4 = basePlayer.CenterPoint();
									float melee_losforgiveness = ConVar.AntiHack.melee_losforgiveness;
									bool flag10 = GamePhysics.LineOfSight(hitPositionWorld2, position2, layerMask, 0f, melee_losforgiveness, null) && GamePhysics.LineOfSight(position2, hitPositionWorld2, layerMask, melee_losforgiveness, 0f, null);
									if (!flag10)
									{
										flag10 = (GamePhysics.LineOfSight(hitPositionWorld2, vector4, layerMask, 0f, melee_losforgiveness, null) && GamePhysics.LineOfSight(vector4, hitPositionWorld2, layerMask, melee_losforgiveness, 0f, null));
									}
									if (!flag10)
									{
										string shortPrefabName12 = base.ShortPrefabName;
										string shortPrefabName13 = basePlayer.ShortPrefabName;
										global::AntiHack.Log(player, AntiHackType.MeleeHack, string.Concat(new object[]
										{
											"Line of sight (",
											shortPrefabName12,
											" on ",
											shortPrefabName13,
											") ",
											hitPositionWorld2,
											" ",
											position2,
											" or ",
											hitPositionWorld2,
											" ",
											vector4
										}));
										player.stats.combat.LogInvalid(hitInfo, "melee_los");
										flag8 = false;
									}
								}
							}
							if (!flag8)
							{
								global::AntiHack.AddViolation(player, AntiHackType.MeleeHack, ConVar.AntiHack.melee_penalty);
								return;
							}
						}
						player.metabolism.UseHeart(this.heartStress * 0.2f);
						using (TimeWarning.New("DoAttackShared", 50))
						{
							this.DoAttackShared(hitInfo);
						}
					}
				}
			}
		}
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool CanBeUsedInWater()
	{
		return true;
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x000313C8 File Offset: 0x0002F5C8
	public virtual string GetStrikeEffectPath(string materialName)
	{
		for (int i = 0; i < this.materialStrikeFX.Count; i++)
		{
			if (this.materialStrikeFX[i].materialName == materialName && this.materialStrikeFX[i].fx.isValid)
			{
				return this.materialStrikeFX[i].fx.resourcePath;
			}
		}
		return this.strikeFX.resourcePath;
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x00031440 File Offset: 0x0002F640
	public override void ServerUse()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.HasAttackCooldown())
		{
			return;
		}
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		base.StartAttackCooldown(this.repeatDelay * 2f);
		ownerPlayer.SignalBroadcast(global::BaseEntity.Signal.Attack, string.Empty, null);
		if (this.swingEffect.isValid)
		{
			Effect.server.Run(this.swingEffect.resourcePath, base.transform.position, Vector3.forward, ownerPlayer.net.connection, false);
		}
		if (base.IsInvoking(new Action(this.ServerUse_Strike)))
		{
			base.CancelInvoke(new Action(this.ServerUse_Strike));
		}
		base.Invoke(new Action(this.ServerUse_Strike), this.aiStrikeDelay);
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ServerUse_OnHit(HitInfo info)
	{
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x00031508 File Offset: 0x0002F708
	public void ServerUse_Strike()
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		Vector3 position = ownerPlayer.eyes.position;
		Vector3 vector = ownerPlayer.eyes.BodyForward();
		for (int i = 0; i < 2; i++)
		{
			List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
			GamePhysics.TraceAll(new Ray(position - vector * ((i == 0) ? 0f : 0.2f), vector), (i == 0) ? 0f : this.attackRadius, list, this.effectiveRange + 0.2f, 1219701521, QueryTriggerInteraction.UseGlobal, null);
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				RaycastHit hit = list[j];
				global::BaseEntity entity = hit.GetEntity();
				if (!(entity == null) && (!(entity != null) || (!(entity == ownerPlayer) && !entity.EqualNetID(ownerPlayer))) && (!(entity != null) || !entity.isClient) && !(entity.Categorize() == ownerPlayer.Categorize()))
				{
					float num = 0f;
					foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
					{
						num += damageTypeEntry.amount;
					}
					entity.OnAttacked(new HitInfo(ownerPlayer, entity, DamageType.Slash, num * this.npcDamageScale));
					HitInfo hitInfo = Facepunch.Pool.Get<HitInfo>();
					hitInfo.HitEntity = entity;
					hitInfo.HitPositionWorld = hit.point;
					hitInfo.HitNormalWorld = -vector;
					if (entity is BaseNpc || entity is global::BasePlayer)
					{
						hitInfo.HitMaterial = StringPool.Get("Flesh");
					}
					else
					{
						hitInfo.HitMaterial = StringPool.Get((hit.GetCollider().sharedMaterial != null) ? hit.GetCollider().sharedMaterial.GetName() : "generic");
					}
					this.ServerUse_OnHit(hitInfo);
					Effect.server.ImpactEffect(hitInfo);
					Facepunch.Pool.Free<HitInfo>(ref hitInfo);
					flag = true;
					if (!(entity != null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
				}
			}
			Facepunch.Pool.FreeList<RaycastHit>(ref list);
			if (flag)
			{
				break;
			}
		}
	}

	// Token: 0x04000304 RID: 772
	[Header("Throwing")]
	public bool canThrowAsProjectile;

	// Token: 0x04000305 RID: 773
	public bool canAiHearIt;

	// Token: 0x04000306 RID: 774
	public bool onlyThrowAsProjectile;

	// Token: 0x04000307 RID: 775
	[Header("Melee")]
	public DamageProperties damageProperties;

	// Token: 0x04000308 RID: 776
	public List<DamageTypeEntry> damageTypes;

	// Token: 0x04000309 RID: 777
	public float maxDistance = 1.5f;

	// Token: 0x0400030A RID: 778
	public float attackRadius = 0.3f;

	// Token: 0x0400030B RID: 779
	public bool isAutomatic = true;

	// Token: 0x0400030C RID: 780
	public bool blockSprintOnAttack = true;

	// Token: 0x0400030D RID: 781
	[Header("Effects")]
	public GameObjectRef strikeFX;

	// Token: 0x0400030E RID: 782
	public bool useStandardHitEffects = true;

	// Token: 0x0400030F RID: 783
	[Header("NPCUsage")]
	public float aiStrikeDelay = 0.2f;

	// Token: 0x04000310 RID: 784
	public GameObjectRef swingEffect;

	// Token: 0x04000311 RID: 785
	public List<BaseMelee.MaterialFX> materialStrikeFX = new List<BaseMelee.MaterialFX>();

	// Token: 0x04000312 RID: 786
	[Header("Other")]
	[Range(0f, 1f)]
	public float heartStress = 0.5f;

	// Token: 0x04000313 RID: 787
	public ResourceDispenser.GatherProperties gathering;

	// Token: 0x02000B4A RID: 2890
	[Serializable]
	public class MaterialFX
	{
		// Token: 0x04003D61 RID: 15713
		public string materialName;

		// Token: 0x04003D62 RID: 15714
		public GameObjectRef fx;
	}
}
