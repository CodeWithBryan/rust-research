using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000034 RID: 52
public class BaseCombatEntity : global::BaseEntity
{
	// Token: 0x06000224 RID: 548 RVA: 0x00025F98 File Offset: 0x00024198
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseCombatEntity.OnRpcMessage", 0))
		{
			if (rpc == 1191093595U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PickupStart ");
				}
				using (TimeWarning.New("RPC_PickupStart", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1191093595U, "RPC_PickupStart", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_PickupStart(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_PickupStart");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x00026100 File Offset: 0x00024300
	protected virtual int GetPickupCount()
	{
		return this.pickup.itemCount;
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0002610D File Offset: 0x0002430D
	public virtual bool CanPickup(global::BasePlayer player)
	{
		return this.pickup.enabled && (!this.pickup.requireBuildingPrivilege || player.CanBuild()) && (!this.pickup.requireHammer || player.IsHoldingEntity<Hammer>());
	}

	// Token: 0x06000227 RID: 551 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnPickedUp(global::Item createdItem, global::BasePlayer player)
	{
	}

	// Token: 0x06000228 RID: 552 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnPickedUpPreItemMove(global::Item createdItem, global::BasePlayer player)
	{
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0002614C File Offset: 0x0002434C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_PickupStart(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanPickup(rpc.player))
		{
			return;
		}
		global::Item item = ItemManager.Create(this.pickup.itemTarget, this.GetPickupCount(), this.skinID);
		if (this.pickup.setConditionFromHealth && item.hasCondition)
		{
			item.conditionNormalized = Mathf.Clamp01(this.healthFraction - this.pickup.subtractCondition);
		}
		this.OnPickedUpPreItemMove(item, rpc.player);
		rpc.player.GiveItem(item, global::BaseEntity.GiveItemReason.PickedUp);
		this.OnPickedUp(item, rpc.player);
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600022A RID: 554 RVA: 0x000261F4 File Offset: 0x000243F4
	public virtual List<ItemAmount> BuildCost()
	{
		if (this.repair.itemTarget == null)
		{
			return null;
		}
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(this.repair.itemTarget);
		if (itemBlueprint == null)
		{
			return null;
		}
		return itemBlueprint.ingredients;
	}

	// Token: 0x0600022B RID: 555 RVA: 0x00026238 File Offset: 0x00024438
	public virtual float RepairCostFraction()
	{
		return 0.5f;
	}

	// Token: 0x0600022C RID: 556 RVA: 0x00026240 File Offset: 0x00024440
	public List<ItemAmount> RepairCost(float healthMissingFraction)
	{
		List<ItemAmount> list = this.BuildCost();
		if (list == null)
		{
			return null;
		}
		List<ItemAmount> list2 = new List<ItemAmount>();
		foreach (ItemAmount itemAmount in list)
		{
			list2.Add(new ItemAmount(itemAmount.itemDef, (float)Mathf.RoundToInt(itemAmount.amount * this.RepairCostFraction() * healthMissingFraction)));
		}
		RepairBench.StripComponentRepairCost(list2);
		return list2;
	}

	// Token: 0x0600022D RID: 557 RVA: 0x000262C8 File Offset: 0x000244C8
	public virtual void OnRepair()
	{
		Effect.server.Run(this.repair.repairEffect.isValid ? this.repair.repairEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x0600022E RID: 558 RVA: 0x00026306 File Offset: 0x00024506
	public virtual void OnRepairFinished()
	{
		Effect.server.Run(this.repair.repairFullEffect.isValid ? this.repair.repairFullEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair_full.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x0600022F RID: 559 RVA: 0x00026344 File Offset: 0x00024544
	public virtual void OnRepairFailed(global::BasePlayer player, string reason)
	{
		Effect.server.Run(this.repair.repairFailedEffect.isValid ? this.repair.repairFailedEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair_failed.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
		if (player != null && !string.IsNullOrEmpty(reason))
		{
			player.ChatMessage(reason);
		}
	}

	// Token: 0x06000230 RID: 560 RVA: 0x000263A8 File Offset: 0x000245A8
	public virtual void OnRepairFailedResources(global::BasePlayer player, List<ItemAmount> requirements)
	{
		Effect.server.Run(this.repair.repairFailedEffect.isValid ? this.repair.repairFailedEffect.resourcePath : "assets/bundled/prefabs/fx/build/repair_failed.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
		if (player != null)
		{
			using (ItemAmountList itemAmountList = ItemAmount.SerialiseList(requirements))
			{
				player.ClientRPCPlayer<ItemAmountList>(null, player, "Client_OnRepairFailedResources", itemAmountList);
			}
		}
	}

	// Token: 0x06000231 RID: 561 RVA: 0x0002642C File Offset: 0x0002462C
	public virtual void DoRepair(global::BasePlayer player)
	{
		if (!this.repair.enabled)
		{
			return;
		}
		float num = 30f;
		if (this.SecondsSinceAttacked <= num)
		{
			this.OnRepairFailed(player, string.Format("Unable to repair: Recently damaged. Repairable in: {0:N0}s.", num - this.SecondsSinceAttacked));
			return;
		}
		float num2 = this.MaxHealth() - this.Health();
		float num3 = num2 / this.MaxHealth();
		if (num2 <= 0f || num3 <= 0f)
		{
			this.OnRepairFailed(player, "Unable to repair: Not damaged.");
			return;
		}
		List<ItemAmount> list = this.RepairCost(num3);
		if (list == null)
		{
			return;
		}
		float num4 = list.Sum((ItemAmount x) => x.amount);
		if (num4 > 0f)
		{
			float num5 = list.Min((ItemAmount x) => Mathf.Clamp01((float)player.inventory.GetAmount(x.itemid) / x.amount));
			num5 = Mathf.Min(num5, 50f / num2);
			if (num5 <= 0f)
			{
				this.OnRepairFailedResources(player, list);
				return;
			}
			int num6 = 0;
			foreach (ItemAmount itemAmount in list)
			{
				int amount = Mathf.CeilToInt(num5 * itemAmount.amount);
				int num7 = player.inventory.Take(null, itemAmount.itemid, amount);
				if (num7 > 0)
				{
					num6 += num7;
					player.Command("note.inv", new object[]
					{
						itemAmount.itemid,
						num7 * -1
					});
				}
			}
			float num8 = (float)num6 / num4;
			this.health += num2 * num8;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		else
		{
			this.health += num2;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		if (this.Health() >= this.MaxHealth())
		{
			this.OnRepairFinished();
			return;
		}
		this.OnRepair();
	}

	// Token: 0x06000232 RID: 562 RVA: 0x0002663C File Offset: 0x0002483C
	public virtual void InitializeHealth(float newhealth, float newmax)
	{
		this._maxHealth = newmax;
		this._health = newhealth;
		this.lifestate = BaseCombatEntity.LifeState.Alive;
	}

	// Token: 0x06000233 RID: 563 RVA: 0x00026653 File Offset: 0x00024853
	public override void ServerInit()
	{
		this.propDirection = PrefabAttribute.server.FindAll<DirectionProperties>(this.prefabID);
		if (this.ResetLifeStateOnSpawn)
		{
			this.InitializeHealth(this.StartHealth(), this.StartMaxHealth());
			this.lifestate = BaseCombatEntity.LifeState.Alive;
		}
		base.ServerInit();
	}

	// Token: 0x06000234 RID: 564 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnHealthChanged(float oldvalue, float newvalue)
	{
	}

	// Token: 0x06000235 RID: 565 RVA: 0x00026692 File Offset: 0x00024892
	public void Hurt(float amount)
	{
		this.Hurt(Mathf.Abs(amount), DamageType.Generic, null, true);
	}

	// Token: 0x06000236 RID: 566 RVA: 0x000266A4 File Offset: 0x000248A4
	public void Hurt(float amount, DamageType type, global::BaseEntity attacker = null, bool useProtection = true)
	{
		using (TimeWarning.New("Hurt", 0))
		{
			this.Hurt(new HitInfo(attacker, this, type, amount, base.transform.position)
			{
				UseProtection = useProtection
			});
		}
	}

	// Token: 0x06000237 RID: 567 RVA: 0x00026700 File Offset: 0x00024900
	public virtual void Hurt(HitInfo info)
	{
		Assert.IsTrue(base.isServer, "This should be called serverside only");
		if (this.IsDead())
		{
			return;
		}
		using (TimeWarning.New("Hurt( HitInfo )", 50))
		{
			float health = this.health;
			this.ScaleDamage(info);
			if (info.PointStart != Vector3.zero)
			{
				for (int i = 0; i < this.propDirection.Length; i++)
				{
					if (!(this.propDirection[i].extraProtection == null) && !this.propDirection[i].IsWeakspot(base.transform, info))
					{
						this.propDirection[i].extraProtection.Scale(info.damageTypes, 1f);
					}
				}
			}
			info.damageTypes.Scale(DamageType.Arrow, ConVar.Server.arrowdamage);
			info.damageTypes.Scale(DamageType.Bullet, ConVar.Server.bulletdamage);
			info.damageTypes.Scale(DamageType.Slash, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Blunt, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Stab, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Bleeding, ConVar.Server.bleedingdamage);
			if (!(this is global::BasePlayer))
			{
				info.damageTypes.Scale(DamageType.Fun_Water, 0f);
			}
			this.DebugHurt(info);
			this.health = health - info.damageTypes.Total();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			if (ConVar.Global.developer > 1)
			{
				Debug.Log(string.Concat(new object[]
				{
					"[Combat]".PadRight(10),
					base.gameObject.name,
					" hurt ",
					info.damageTypes.GetMajorityDamageType(),
					"/",
					info.damageTypes.Total(),
					" - ",
					this.health.ToString("0"),
					" health left"
				}));
			}
			this.lastDamage = info.damageTypes.GetMajorityDamageType();
			this.lastAttacker = info.Initiator;
			if (this.lastAttacker != null)
			{
				BaseCombatEntity baseCombatEntity = this.lastAttacker as BaseCombatEntity;
				if (baseCombatEntity != null)
				{
					baseCombatEntity.lastDealtDamageTime = UnityEngine.Time.time;
					baseCombatEntity.lastDealtDamageTo = this;
				}
			}
			BaseCombatEntity baseCombatEntity2 = this.lastAttacker as BaseCombatEntity;
			if (this.markAttackerHostile && baseCombatEntity2 != null && baseCombatEntity2 != this)
			{
				baseCombatEntity2.MarkHostileFor(60f);
			}
			if (this.lastDamage.IsConsideredAnAttack())
			{
				this.lastAttackedTime = UnityEngine.Time.time;
				if (this.lastAttacker != null)
				{
					this.LastAttackedDir = (this.lastAttacker.transform.position - base.transform.position).normalized;
				}
			}
			if (this.Health() <= 0f)
			{
				this.Die(info);
			}
			global::BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (initiatorPlayer)
			{
				if (this.IsDead())
				{
					initiatorPlayer.stats.combat.LogAttack(info, "killed", health);
				}
				else
				{
					initiatorPlayer.stats.combat.LogAttack(info, "", health);
				}
			}
		}
	}

	// Token: 0x06000238 RID: 568 RVA: 0x00026A50 File Offset: 0x00024C50
	public virtual bool IsHostile()
	{
		return this.unHostileTime > UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06000239 RID: 569 RVA: 0x00026A60 File Offset: 0x00024C60
	public virtual void MarkHostileFor(float duration = 60f)
	{
		float b = UnityEngine.Time.realtimeSinceStartup + duration;
		this.unHostileTime = Mathf.Max(this.unHostileTime, b);
	}

	// Token: 0x0600023A RID: 570 RVA: 0x00026A88 File Offset: 0x00024C88
	private void DebugHurt(HitInfo info)
	{
		if (!ConVar.Vis.damage)
		{
			return;
		}
		if (info.PointStart != info.PointEnd)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.arrow", new object[]
			{
				60,
				Color.cyan,
				info.PointStart,
				info.PointEnd,
				0.1f
			});
			ConsoleNetwork.BroadcastToAllClients("ddraw.sphere", new object[]
			{
				60,
				Color.cyan,
				info.HitPositionWorld,
				0.01f
			});
		}
		string text = "";
		for (int i = 0; i < info.damageTypes.types.Length; i++)
		{
			float num = info.damageTypes.types[i];
			if (num != 0f)
			{
				string[] array = new string[5];
				array[0] = text;
				array[1] = " ";
				int num2 = 2;
				DamageType damageType = (DamageType)i;
				array[num2] = damageType.ToString().PadRight(10);
				array[3] = num.ToString("0.00");
				array[4] = "\n";
				text = string.Concat(array);
			}
		}
		string text2 = string.Concat(new object[]
		{
			"<color=lightblue>Damage:</color>".PadRight(10),
			info.damageTypes.Total().ToString("0.00"),
			"\n<color=lightblue>Health:</color>".PadRight(10),
			this.health.ToString("0.00"),
			" / ",
			(this.health - info.damageTypes.Total() <= 0f) ? "<color=red>" : "<color=green>",
			(this.health - info.damageTypes.Total()).ToString("0.00"),
			"</color>",
			"\n<color=lightblue>HitEnt:</color>".PadRight(10),
			this,
			"\n<color=lightblue>HitBone:</color>".PadRight(10),
			info.boneName,
			"\n<color=lightblue>Attacker:</color>".PadRight(10),
			info.Initiator,
			"\n<color=lightblue>WeaponPrefab:</color>".PadRight(10),
			info.WeaponPrefab,
			"\n<color=lightblue>Damages:</color>\n",
			text
		});
		ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[]
		{
			60,
			Color.white,
			info.HitPositionWorld,
			text2
		});
	}

	// Token: 0x0600023B RID: 571 RVA: 0x00026D24 File Offset: 0x00024F24
	public void SetHealth(float hp)
	{
		if (this.health == hp)
		{
			return;
		}
		this.health = hp;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600023C RID: 572 RVA: 0x00026D40 File Offset: 0x00024F40
	public virtual void Heal(float amount)
	{
		if (ConVar.Global.developer > 1)
		{
			Debug.Log("[Combat]".PadRight(10) + base.gameObject.name + " healed");
		}
		this.health = this._health + amount;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00026D90 File Offset: 0x00024F90
	public virtual void OnKilled(HitInfo info)
	{
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00026D9C File Offset: 0x00024F9C
	public virtual void Die(HitInfo info = null)
	{
		if (this.IsDead())
		{
			return;
		}
		if (ConVar.Global.developer > 1)
		{
			Debug.Log("[Combat]".PadRight(10) + base.gameObject.name + " died");
		}
		this.health = 0f;
		this.lifestate = BaseCombatEntity.LifeState.Dead;
		if (info != null && info.InitiatorPlayer)
		{
			global::BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (initiatorPlayer != null && initiatorPlayer.GetActiveMission() != -1 && !initiatorPlayer.IsNpc)
			{
				initiatorPlayer.ProcessMissionEvent(BaseMission.MissionEventType.KILL_ENTITY, this.prefabID.ToString(), 1f);
			}
		}
		using (TimeWarning.New("OnKilled", 0))
		{
			this.OnKilled(info);
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x00026E6C File Offset: 0x0002506C
	public void DieInstantly()
	{
		if (this.IsDead())
		{
			return;
		}
		if (ConVar.Global.developer > 1)
		{
			Debug.Log("[Combat]".PadRight(10) + base.gameObject.name + " died");
		}
		this.health = 0f;
		this.lifestate = BaseCombatEntity.LifeState.Dead;
		this.OnKilled(null);
	}

	// Token: 0x06000240 RID: 576 RVA: 0x00026ECC File Offset: 0x000250CC
	public void UpdateSurroundings()
	{
		global::StabilityEntity.updateSurroundingsQueue.Add(this.WorldSpaceBounds().ToBounds());
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000241 RID: 577 RVA: 0x00026EF1 File Offset: 0x000250F1
	public float TimeSinceLastNoise
	{
		get
		{
			return UnityEngine.Time.time - this.lastNoiseTime;
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000242 RID: 578 RVA: 0x00026EFF File Offset: 0x000250FF
	// (set) Token: 0x06000243 RID: 579 RVA: 0x00026F07 File Offset: 0x00025107
	public BaseCombatEntity.ActionVolume LastNoiseVolume { get; private set; }

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000244 RID: 580 RVA: 0x00026F10 File Offset: 0x00025110
	// (set) Token: 0x06000245 RID: 581 RVA: 0x00026F18 File Offset: 0x00025118
	public Vector3 LastNoisePosition { get; private set; }

	// Token: 0x06000246 RID: 582 RVA: 0x00026F21 File Offset: 0x00025121
	public void MakeNoise(Vector3 position, BaseCombatEntity.ActionVolume loudness)
	{
		this.LastNoisePosition = position;
		this.LastNoiseVolume = loudness;
		this.lastNoiseTime = UnityEngine.Time.time;
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00026F3C File Offset: 0x0002513C
	public bool CanLastNoiseBeHeard(Vector3 listenPosition, float listenRange)
	{
		return listenRange > 0f && Vector3.Distance(listenPosition, this.LastNoisePosition) <= listenRange;
	}

	// Token: 0x06000248 RID: 584 RVA: 0x00026F5A File Offset: 0x0002515A
	public virtual bool IsDead()
	{
		return this.lifestate == BaseCombatEntity.LifeState.Dead;
	}

	// Token: 0x06000249 RID: 585 RVA: 0x00026F65 File Offset: 0x00025165
	public virtual bool IsAlive()
	{
		return this.lifestate == BaseCombatEntity.LifeState.Alive;
	}

	// Token: 0x0600024A RID: 586 RVA: 0x00026F70 File Offset: 0x00025170
	public BaseCombatEntity.Faction GetFaction()
	{
		return this.faction;
	}

	// Token: 0x0600024B RID: 587 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsFriendly(BaseCombatEntity other)
	{
		return false;
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x0600024C RID: 588 RVA: 0x00026F78 File Offset: 0x00025178
	// (set) Token: 0x0600024D RID: 589 RVA: 0x00026F80 File Offset: 0x00025180
	public Vector3 LastAttackedDir { get; set; }

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x0600024E RID: 590 RVA: 0x00026F89 File Offset: 0x00025189
	public float SecondsSinceAttacked
	{
		get
		{
			return UnityEngine.Time.time - this.lastAttackedTime;
		}
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x0600024F RID: 591 RVA: 0x00026F97 File Offset: 0x00025197
	public float SecondsSinceDealtDamage
	{
		get
		{
			return UnityEngine.Time.time - this.lastDealtDamageTime;
		}
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x06000250 RID: 592 RVA: 0x00026FA5 File Offset: 0x000251A5
	public float healthFraction
	{
		get
		{
			return this.Health() / this.MaxHealth();
		}
	}

	// Token: 0x06000251 RID: 593 RVA: 0x00026FB4 File Offset: 0x000251B4
	public override void ResetState()
	{
		base.ResetState();
		this.health = this.MaxHealth();
		if (base.isServer)
		{
			this.lastAttackedTime = float.NegativeInfinity;
			this.lastDealtDamageTime = float.NegativeInfinity;
		}
	}

	// Token: 0x06000252 RID: 594 RVA: 0x00026FE6 File Offset: 0x000251E6
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			this.UpdateSurroundings();
		}
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00026FFC File Offset: 0x000251FC
	public virtual float GetThreatLevel()
	{
		return 0f;
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00027003 File Offset: 0x00025203
	public override float PenetrationResistance(HitInfo info)
	{
		if (!this.baseProtection)
		{
			return 100f;
		}
		return this.baseProtection.density;
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00027023 File Offset: 0x00025223
	public virtual void ScaleDamage(HitInfo info)
	{
		if (info.UseProtection && this.baseProtection != null)
		{
			this.baseProtection.Scale(info.damageTypes, 1f);
		}
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00027054 File Offset: 0x00025254
	public HitArea SkeletonLookup(uint boneID)
	{
		if (this.skeletonProperties == null)
		{
			return (HitArea)(-1);
		}
		SkeletonProperties.BoneProperty boneProperty = this.skeletonProperties.FindBone(boneID);
		if (boneProperty == null)
		{
			return (HitArea)(-1);
		}
		return boneProperty.area;
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0002708C File Offset: 0x0002528C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseCombat = Facepunch.Pool.Get<BaseCombat>();
		info.msg.baseCombat.state = (int)this.lifestate;
		info.msg.baseCombat.health = this.Health();
	}

	// Token: 0x06000258 RID: 600 RVA: 0x000270DC File Offset: 0x000252DC
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.Health() > this.MaxHealth())
		{
			this.health = this.MaxHealth();
		}
		if (float.IsNaN(this.Health()))
		{
			this.health = this.MaxHealth();
		}
	}

	// Token: 0x06000259 RID: 601 RVA: 0x00027118 File Offset: 0x00025318
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (base.isServer)
		{
			this.lifestate = BaseCombatEntity.LifeState.Alive;
		}
		if (info.msg.baseCombat != null)
		{
			this.lifestate = (BaseCombatEntity.LifeState)info.msg.baseCombat.state;
			this._health = info.msg.baseCombat.health;
		}
		base.Load(info);
	}

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600025A RID: 602 RVA: 0x00027174 File Offset: 0x00025374
	// (set) Token: 0x0600025B RID: 603 RVA: 0x0002717C File Offset: 0x0002537C
	public float health
	{
		get
		{
			return this._health;
		}
		set
		{
			float health = this._health;
			this._health = Mathf.Clamp(value, 0f, this.MaxHealth());
			if (base.isServer && this._health != health)
			{
				this.OnHealthChanged(health, this._health);
			}
		}
	}

	// Token: 0x0600025C RID: 604 RVA: 0x00027174 File Offset: 0x00025374
	public override float Health()
	{
		return this._health;
	}

	// Token: 0x0600025D RID: 605 RVA: 0x000271C5 File Offset: 0x000253C5
	public override float MaxHealth()
	{
		return this._maxHealth;
	}

	// Token: 0x0600025E RID: 606 RVA: 0x000271CD File Offset: 0x000253CD
	public virtual float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x0600025F RID: 607 RVA: 0x000271D5 File Offset: 0x000253D5
	public virtual float StartMaxHealth()
	{
		return this.StartHealth();
	}

	// Token: 0x06000260 RID: 608 RVA: 0x000271DD File Offset: 0x000253DD
	public void SetMaxHealth(float newMax)
	{
		this._maxHealth = newMax;
		this._health = Mathf.Min(this._health, newMax);
	}

	// Token: 0x06000261 RID: 609 RVA: 0x000271F8 File Offset: 0x000253F8
	public void DoHitNotify(HitInfo info)
	{
		using (TimeWarning.New("DoHitNotify", 0))
		{
			if (this.sendsHitNotification && !(info.Initiator == null) && info.Initiator is global::BasePlayer && !(this == info.Initiator))
			{
				if (!info.isHeadshot || !(info.HitEntity is global::BasePlayer))
				{
					if (UnityEngine.Time.frameCount != this.lastNotifyFrame)
					{
						this.lastNotifyFrame = UnityEngine.Time.frameCount;
						bool flag = info.Weapon is BaseMelee;
						if (base.isServer && (!flag || this.sendsMeleeHitNotification))
						{
							bool arg = info.Initiator.net.connection == info.Predicted;
							base.ClientRPCPlayerAndSpectators<bool>(null, info.Initiator as global::BasePlayer, "HitNotify", arg);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000262 RID: 610 RVA: 0x000272E8 File Offset: 0x000254E8
	public override void OnAttacked(HitInfo info)
	{
		using (TimeWarning.New("BaseCombatEntity.OnAttacked", 0))
		{
			if (!this.IsDead())
			{
				this.DoHitNotify(info);
			}
			if (base.isServer)
			{
				this.Hurt(info);
			}
		}
		base.OnAttacked(info);
	}

	// Token: 0x04000218 RID: 536
	private const float MAX_HEALTH_REPAIR = 50f;

	// Token: 0x04000219 RID: 537
	[NonSerialized]
	public DamageType lastDamage;

	// Token: 0x0400021A RID: 538
	[NonSerialized]
	public global::BaseEntity lastAttacker;

	// Token: 0x0400021B RID: 539
	public global::BaseEntity lastDealtDamageTo;

	// Token: 0x0400021C RID: 540
	[NonSerialized]
	public bool ResetLifeStateOnSpawn = true;

	// Token: 0x0400021D RID: 541
	protected DirectionProperties[] propDirection;

	// Token: 0x0400021E RID: 542
	protected float unHostileTime;

	// Token: 0x04000221 RID: 545
	private float lastNoiseTime;

	// Token: 0x04000222 RID: 546
	[Header("BaseCombatEntity")]
	public SkeletonProperties skeletonProperties;

	// Token: 0x04000223 RID: 547
	public ProtectionProperties baseProtection;

	// Token: 0x04000224 RID: 548
	public float startHealth;

	// Token: 0x04000225 RID: 549
	public BaseCombatEntity.Pickup pickup;

	// Token: 0x04000226 RID: 550
	public BaseCombatEntity.Repair repair;

	// Token: 0x04000227 RID: 551
	public bool ShowHealthInfo = true;

	// Token: 0x04000228 RID: 552
	public BaseCombatEntity.LifeState lifestate;

	// Token: 0x04000229 RID: 553
	public bool sendsHitNotification;

	// Token: 0x0400022A RID: 554
	public bool sendsMeleeHitNotification = true;

	// Token: 0x0400022B RID: 555
	public bool markAttackerHostile = true;

	// Token: 0x0400022C RID: 556
	protected float _health;

	// Token: 0x0400022D RID: 557
	protected float _maxHealth = 100f;

	// Token: 0x0400022E RID: 558
	public BaseCombatEntity.Faction faction;

	// Token: 0x0400022F RID: 559
	[NonSerialized]
	public float lastAttackedTime = float.NegativeInfinity;

	// Token: 0x04000231 RID: 561
	[NonSerialized]
	public float lastDealtDamageTime = float.NegativeInfinity;

	// Token: 0x04000232 RID: 562
	private int lastNotifyFrame;

	// Token: 0x02000B2B RID: 2859
	[Serializable]
	public struct Pickup
	{
		// Token: 0x04003CBA RID: 15546
		public bool enabled;

		// Token: 0x04003CBB RID: 15547
		[ItemSelector(ItemCategory.All)]
		public ItemDefinition itemTarget;

		// Token: 0x04003CBC RID: 15548
		public int itemCount;

		// Token: 0x04003CBD RID: 15549
		[Tooltip("Should we set the condition of the item based on the health of the picked up entity")]
		public bool setConditionFromHealth;

		// Token: 0x04003CBE RID: 15550
		[Tooltip("How much to reduce the item condition when picking up")]
		public float subtractCondition;

		// Token: 0x04003CBF RID: 15551
		[Tooltip("Must have building access to pick up")]
		public bool requireBuildingPrivilege;

		// Token: 0x04003CC0 RID: 15552
		[Tooltip("Must have hammer equipped to pick up")]
		public bool requireHammer;

		// Token: 0x04003CC1 RID: 15553
		[Tooltip("Inventory Must be empty (if applicable) to be picked up")]
		public bool requireEmptyInv;
	}

	// Token: 0x02000B2C RID: 2860
	[Serializable]
	public struct Repair
	{
		// Token: 0x04003CC2 RID: 15554
		public bool enabled;

		// Token: 0x04003CC3 RID: 15555
		[ItemSelector(ItemCategory.All)]
		public ItemDefinition itemTarget;

		// Token: 0x04003CC4 RID: 15556
		public GameObjectRef repairEffect;

		// Token: 0x04003CC5 RID: 15557
		public GameObjectRef repairFullEffect;

		// Token: 0x04003CC6 RID: 15558
		public GameObjectRef repairFailedEffect;
	}

	// Token: 0x02000B2D RID: 2861
	public enum ActionVolume
	{
		// Token: 0x04003CC8 RID: 15560
		Quiet,
		// Token: 0x04003CC9 RID: 15561
		Normal,
		// Token: 0x04003CCA RID: 15562
		Loud
	}

	// Token: 0x02000B2E RID: 2862
	public enum LifeState
	{
		// Token: 0x04003CCC RID: 15564
		Alive,
		// Token: 0x04003CCD RID: 15565
		Dead
	}

	// Token: 0x02000B2F RID: 2863
	[Serializable]
	public enum Faction
	{
		// Token: 0x04003CCF RID: 15567
		Default,
		// Token: 0x04003CD0 RID: 15568
		Player,
		// Token: 0x04003CD1 RID: 15569
		Bandit,
		// Token: 0x04003CD2 RID: 15570
		Scientist,
		// Token: 0x04003CD3 RID: 15571
		Horror
	}
}
