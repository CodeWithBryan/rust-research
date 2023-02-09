using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Epic.OnlineServices.AntiCheatCommon;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000042 RID: 66
public class BaseProjectile : AttackEntity
{
	// Token: 0x060006A2 RID: 1698 RVA: 0x00044938 File Offset: 0x00042B38
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseProjectile.OnRpcMessage", 0))
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
			if (rpc == 1720368164U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Reload ");
				}
				using (TimeWarning.New("Reload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1720368164U, "Reload", this, player))
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
							this.Reload(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Reload");
					}
				}
				return true;
			}
			if (rpc == 240404208U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerFractionalReloadInsert ");
				}
				using (TimeWarning.New("ServerFractionalReloadInsert", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(240404208U, "ServerFractionalReloadInsert", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerFractionalReloadInsert(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in ServerFractionalReloadInsert");
					}
				}
				return true;
			}
			if (rpc == 555589155U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartReload ");
				}
				using (TimeWarning.New("StartReload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(555589155U, "StartReload", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.StartReload(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in StartReload");
					}
				}
				return true;
			}
			if (rpc == 1918419884U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SwitchAmmoTo ");
				}
				using (TimeWarning.New("SwitchAmmoTo", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1918419884U, "SwitchAmmoTo", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SwitchAmmoTo(msg6);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in SwitchAmmoTo");
					}
				}
				return true;
			}
			if (rpc == 3327286961U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ToggleFireMode ");
				}
				using (TimeWarning.New("ToggleFireMode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3327286961U, "ToggleFireMode", this, player, 2UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3327286961U, "ToggleFireMode", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ToggleFireMode(msg7);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in ToggleFireMode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x060006A3 RID: 1699 RVA: 0x000451BC File Offset: 0x000433BC
	public RecoilProperties recoilProperties
	{
		get
		{
			if (!(this.recoil == null))
			{
				return this.recoil.GetRecoil();
			}
			return null;
		}
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x000451D9 File Offset: 0x000433D9
	public override Vector3 GetInheritedVelocity(global::BasePlayer player, Vector3 direction)
	{
		return player.GetInheritedProjectileVelocity(direction);
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x000451E2 File Offset: 0x000433E2
	public virtual float GetDamageScale(bool getMax = false)
	{
		return this.damageScale;
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x000451EA File Offset: 0x000433EA
	public virtual float GetDistanceScale(bool getMax = false)
	{
		return this.distanceScale;
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x000451F2 File Offset: 0x000433F2
	public virtual float GetProjectileVelocityScale(bool getMax = false)
	{
		return this.projectileVelocityScale;
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x000451FA File Offset: 0x000433FA
	protected void StartReloadCooldown(float cooldown)
	{
		this.nextReloadTime = base.CalculateCooldownTime(this.nextReloadTime, cooldown, false);
		this.startReloadTime = this.nextReloadTime - cooldown;
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x0004521E File Offset: 0x0004341E
	protected void ResetReloadCooldown()
	{
		this.nextReloadTime = float.NegativeInfinity;
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x0004522B File Offset: 0x0004342B
	protected bool HasReloadCooldown()
	{
		return UnityEngine.Time.time < this.nextReloadTime;
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x0004523A File Offset: 0x0004343A
	protected float GetReloadCooldown()
	{
		return Mathf.Max(this.nextReloadTime - UnityEngine.Time.time, 0f);
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x00045252 File Offset: 0x00043452
	protected float GetReloadIdle()
	{
		return Mathf.Max(UnityEngine.Time.time - this.nextReloadTime, 0f);
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x0004526C File Offset: 0x0004346C
	private void OnDrawGizmos()
	{
		if (!base.isClient)
		{
			return;
		}
		if (this.MuzzlePoint != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(this.MuzzlePoint.position, this.MuzzlePoint.position + this.MuzzlePoint.forward * 10f);
			global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
			if (ownerPlayer)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(this.MuzzlePoint.position, this.MuzzlePoint.position + ownerPlayer.eyes.rotation * Vector3.forward * 10f);
			}
		}
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x0004532A File Offset: 0x0004352A
	public virtual RecoilProperties GetRecoil()
	{
		return this.recoilProperties;
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x060006AF RID: 1711 RVA: 0x00045332 File Offset: 0x00043532
	public bool isSemiAuto
	{
		get
		{
			return !this.automatic;
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x060006B0 RID: 1712 RVA: 0x0004533D File Offset: 0x0004353D
	public override bool IsUsableByTurret
	{
		get
		{
			return this.usableByTurret;
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x060006B1 RID: 1713 RVA: 0x00045345 File Offset: 0x00043545
	public override Transform MuzzleTransform
	{
		get
		{
			return this.MuzzlePoint;
		}
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void DidAttackServerside()
	{
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x0004534D File Offset: 0x0004354D
	public override bool ServerIsReloading()
	{
		return UnityEngine.Time.time < this.lastReloadTime + this.reloadTime;
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x00045363 File Offset: 0x00043563
	public override bool CanReload()
	{
		return this.primaryMagazine.contents < this.primaryMagazine.capacity;
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x0004537D File Offset: 0x0004357D
	public override float AmmoFraction()
	{
		return (float)this.primaryMagazine.contents / (float)this.primaryMagazine.capacity;
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x00045398 File Offset: 0x00043598
	public override void TopUpAmmo()
	{
		this.primaryMagazine.contents = this.primaryMagazine.capacity;
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x000453B0 File Offset: 0x000435B0
	public override void ServerReload()
	{
		if (this.ServerIsReloading())
		{
			return;
		}
		this.lastReloadTime = UnityEngine.Time.time;
		base.StartAttackCooldown(this.reloadTime);
		base.GetOwnerPlayer().SignalBroadcast(global::BaseEntity.Signal.Reload, null);
		this.primaryMagazine.contents = this.primaryMagazine.capacity;
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x00045400 File Offset: 0x00043600
	public override Vector3 ModifyAIAim(Vector3 eulerInput, float swayModifier = 1f)
	{
		bool flag = false;
		float num = UnityEngine.Time.time * (this.aimSwaySpeed * 1f + this.aiAimSwayOffset);
		float num2 = Mathf.Sin(UnityEngine.Time.time * 2f);
		float num3 = (num2 < 0f) ? (1f - Mathf.Clamp(Mathf.Abs(num2) / 1f, 0f, 1f)) : 1f;
		float num4 = flag ? 0.6f : 1f;
		float num5 = (this.aimSway * 1f + this.aiAimSwayOffset) * num4 * num3 * swayModifier;
		eulerInput.y += (Mathf.PerlinNoise(num, num) - 0.5f) * num5 * UnityEngine.Time.deltaTime;
		eulerInput.x += (Mathf.PerlinNoise(num + 0.1f, num + 0.2f) - 0.5f) * num5 * UnityEngine.Time.deltaTime;
		return eulerInput;
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x000454E4 File Offset: 0x000436E4
	public float GetAIAimcone()
	{
		NPCPlayer npcplayer = base.GetOwnerPlayer() as NPCPlayer;
		if (npcplayer)
		{
			return npcplayer.GetAimConeScale() * this.aiAimCone;
		}
		return this.aiAimCone;
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x0002EE04 File Offset: 0x0002D004
	public override void ServerUse()
	{
		this.ServerUse(1f, null);
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x0004551C File Offset: 0x0004371C
	public override void ServerUse(float damageModifier, Transform originOverride = null)
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
		bool flag = ownerPlayer != null;
		if (this.primaryMagazine.contents <= 0)
		{
			base.SignalBroadcast(global::BaseEntity.Signal.DryFire, null);
			base.StartAttackCooldownRaw(1f);
			return;
		}
		this.primaryMagazine.contents--;
		if (this.primaryMagazine.contents < 0)
		{
			this.primaryMagazine.contents = 0;
		}
		bool flag2 = flag && ownerPlayer.IsNpc;
		if (flag2 && (ownerPlayer.isMounted || ownerPlayer.GetParentEntity() != null))
		{
			NPCPlayer npcplayer = ownerPlayer as NPCPlayer;
			if (npcplayer != null)
			{
				npcplayer.SetAimDirection(npcplayer.GetAimDirection());
			}
		}
		base.StartAttackCooldownRaw(this.repeatDelay);
		Vector3 vector = flag ? ownerPlayer.eyes.position : this.MuzzlePoint.transform.position;
		Vector3 inputVec = this.MuzzlePoint.transform.forward;
		if (originOverride != null)
		{
			vector = originOverride.position;
			inputVec = originOverride.forward;
		}
		ItemModProjectile component = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>();
		base.SignalBroadcast(global::BaseEntity.Signal.Attack, string.Empty, null);
		Projectile component2 = component.projectileObject.Get().GetComponent<Projectile>();
		global::BaseEntity baseEntity = null;
		if (flag)
		{
			inputVec = ownerPlayer.eyes.BodyForward();
		}
		for (int i = 0; i < component.numProjectiles; i++)
		{
			Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(component.projectileSpread + this.GetAimCone() + this.GetAIAimcone() * 1f, inputVec, true);
			List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
			GamePhysics.TraceAll(new Ray(vector, modifiedAimConeDirection), 0f, list, 300f, 1219701505, QueryTriggerInteraction.UseGlobal, null);
			for (int j = 0; j < list.Count; j++)
			{
				RaycastHit hit = list[j];
				global::BaseEntity entity = hit.GetEntity();
				if ((!(entity != null) || (!(entity == this) && !entity.EqualNetID(this))) && (!(entity != null) || !entity.isClient))
				{
					ColliderInfo component3 = hit.collider.GetComponent<ColliderInfo>();
					if (!(component3 != null) || component3.HasFlag(ColliderInfo.Flags.Shootable))
					{
						BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
						if ((!(entity != null) || !entity.IsNpc || !flag2 || baseCombatEntity.GetFaction() == BaseCombatEntity.Faction.Horror || entity is BasePet) && baseCombatEntity != null && (baseEntity == null || entity == baseEntity || entity.EqualNetID(baseEntity)))
						{
							HitInfo hitInfo = new HitInfo();
							this.AssignInitiator(hitInfo);
							hitInfo.Weapon = this;
							hitInfo.WeaponPrefab = base.gameManager.FindPrefab(base.PrefabName).GetComponent<AttackEntity>();
							hitInfo.IsPredicting = false;
							hitInfo.DoHitEffects = component2.doDefaultHitEffects;
							hitInfo.DidHit = true;
							hitInfo.ProjectileVelocity = modifiedAimConeDirection * 300f;
							hitInfo.PointStart = this.MuzzlePoint.position;
							hitInfo.PointEnd = hit.point;
							hitInfo.HitPositionWorld = hit.point;
							hitInfo.HitNormalWorld = hit.normal;
							hitInfo.HitEntity = entity;
							hitInfo.UseProtection = true;
							component2.CalculateDamage(hitInfo, this.GetProjectileModifier(), 1f);
							hitInfo.damageTypes.ScaleAll(this.GetDamageScale(false) * damageModifier * (flag2 ? this.npcDamageScale : this.turretDamageScale));
							baseCombatEntity.OnAttacked(hitInfo);
							component.ServerProjectileHit(hitInfo);
							if (entity is global::BasePlayer || entity is BaseNpc)
							{
								hitInfo.HitPositionLocal = entity.transform.InverseTransformPoint(hitInfo.HitPositionWorld);
								hitInfo.HitNormalLocal = entity.transform.InverseTransformDirection(hitInfo.HitNormalWorld);
								hitInfo.HitMaterial = StringPool.Get("Flesh");
								Effect.server.ImpactEffect(hitInfo);
							}
						}
						if (!(entity != null) || entity.ShouldBlockProjectiles())
						{
							break;
						}
					}
				}
			}
			Facepunch.Pool.FreeList<RaycastHit>(ref list);
			Vector3 b = (flag && ownerPlayer.isMounted) ? (modifiedAimConeDirection * 6f) : Vector3.zero;
			this.CreateProjectileEffectClientside(component.projectileObject.resourcePath, vector + b, modifiedAimConeDirection * component.projectileVelocity, UnityEngine.Random.Range(1, 100), null, this.IsSilenced(), true);
		}
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x000459B1 File Offset: 0x00043BB1
	private void AssignInitiator(HitInfo info)
	{
		info.Initiator = base.GetOwnerPlayer();
		if (info.Initiator == null)
		{
			info.Initiator = base.GetParentEntity();
		}
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x000459D9 File Offset: 0x00043BD9
	public override void ServerInit()
	{
		base.ServerInit();
		this.primaryMagazine.ServerInit();
		base.Invoke(new Action(this.DelayedModSetup), 0.1f);
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x00045A04 File Offset: 0x00043C04
	public void DelayedModSetup()
	{
		if (this.modsChangedInitialized)
		{
			return;
		}
		global::Item cachedItem = base.GetCachedItem();
		if (cachedItem != null && cachedItem.contents != null)
		{
			global::ItemContainer contents = cachedItem.contents;
			contents.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(contents.onItemAddedRemoved, new Action<global::Item, bool>(this.ModsChanged));
			this.modsChangedInitialized = true;
		}
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x00045A5C File Offset: 0x00043C5C
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			global::Item cachedItem = base.GetCachedItem();
			if (cachedItem != null && cachedItem.contents != null)
			{
				global::ItemContainer contents = cachedItem.contents;
				contents.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Remove(contents.onItemAddedRemoved, new Action<global::Item, bool>(this.ModsChanged));
				this.modsChangedInitialized = false;
			}
		}
		base.DestroyShared();
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00045AB7 File Offset: 0x00043CB7
	public void ModsChanged(global::Item item, bool added)
	{
		base.Invoke(new Action(this.DelayedModsChanged), 0.1f);
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x00045AD0 File Offset: 0x00043CD0
	public void ForceModsChanged()
	{
		base.Invoke(new Action(this.DelayedModSetup), 0f);
		base.Invoke(new Action(this.DelayedModsChanged), 0.2f);
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00045B00 File Offset: 0x00043D00
	public void DelayedModsChanged()
	{
		int num = Mathf.CeilToInt(ProjectileWeaponMod.Mult(this, (ProjectileWeaponMod x) => x.magazineCapacity, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * (float)this.primaryMagazine.definition.builtInSize);
		if (num == this.primaryMagazine.capacity)
		{
			return;
		}
		if (this.primaryMagazine.contents > 0 && this.primaryMagazine.contents > num)
		{
			ItemDefinition ammoType = this.primaryMagazine.ammoType;
			int contents = this.primaryMagazine.contents;
			global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
			global::ItemContainer itemContainer = null;
			if (ownerPlayer != null)
			{
				itemContainer = ownerPlayer.inventory.containerMain;
			}
			else if (base.GetCachedItem() != null)
			{
				itemContainer = base.GetCachedItem().parent;
			}
			this.primaryMagazine.contents = 0;
			if (itemContainer != null)
			{
				global::Item item = ItemManager.Create(this.primaryMagazine.ammoType, contents, 0UL);
				if (!item.MoveToContainer(itemContainer, -1, true, false, null, true))
				{
					Vector3 vPos = base.transform.position;
					if (itemContainer.entityOwner != null)
					{
						vPos = itemContainer.entityOwner.transform.position + Vector3.up * 0.25f;
					}
					item.Drop(vPos, Vector3.up * 5f, default(Quaternion));
				}
			}
		}
		this.primaryMagazine.capacity = num;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00045C98 File Offset: 0x00043E98
	public override void ServerCommand(global::Item item, string command, global::BasePlayer player)
	{
		if (item == null)
		{
			return;
		}
		if (command == "unload_ammo" && !this.HasReloadCooldown())
		{
			this.UnloadAmmo(item, player);
		}
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x00045CBC File Offset: 0x00043EBC
	public void UnloadAmmo(global::Item item, global::BasePlayer player)
	{
		global::BaseProjectile component = item.GetHeldEntity().GetComponent<global::BaseProjectile>();
		if (!component.canUnloadAmmo)
		{
			return;
		}
		if (component)
		{
			int contents = component.primaryMagazine.contents;
			if (contents > 0)
			{
				component.primaryMagazine.contents = 0;
				base.SendNetworkUpdateImmediate(false);
				global::Item item2 = ItemManager.Create(component.primaryMagazine.ammoType, contents, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
				{
					item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
				}
			}
		}
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x00045D51 File Offset: 0x00043F51
	public override void CollectedForCrafting(global::Item item, global::BasePlayer crafter)
	{
		if (crafter == null || item == null)
		{
			return;
		}
		this.UnloadAmmo(item, crafter);
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x00045D68 File Offset: 0x00043F68
	public override void ReturnedFromCancelledCraft(global::Item item, global::BasePlayer crafter)
	{
		if (crafter == null || item == null)
		{
			return;
		}
		global::BaseProjectile component = item.GetHeldEntity().GetComponent<global::BaseProjectile>();
		if (component)
		{
			component.primaryMagazine.contents = 0;
		}
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x00045DA2 File Offset: 0x00043FA2
	public override void SetLightsOn(bool isOn)
	{
		base.SetLightsOn(isOn);
		this.UpdateAttachmentsState();
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00045DB4 File Offset: 0x00043FB4
	public void UpdateAttachmentsState()
	{
		global::BaseEntity.Flags flags = this.flags;
		bool b = this.ShouldLightsBeOn();
		if (this.children != null)
		{
			foreach (ProjectileWeaponMod projectileWeaponMod in from ProjectileWeaponMod x in this.children
			where x != null && x.isLight
			select x)
			{
				projectileWeaponMod.SetFlag(global::BaseEntity.Flags.On, b, false, true);
			}
		}
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x00045E44 File Offset: 0x00044044
	private bool ShouldLightsBeOn()
	{
		return base.LightsOn() && (base.IsDeployed() || this.parentEntity.Get(base.isServer) is global::AutoTurret);
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x00045E74 File Offset: 0x00044074
	protected override void OnChildRemoved(global::BaseEntity child)
	{
		base.OnChildRemoved(child);
		ProjectileWeaponMod projectileWeaponMod;
		if ((projectileWeaponMod = (child as ProjectileWeaponMod)) != null && projectileWeaponMod.isLight)
		{
			child.SetFlag(global::BaseEntity.Flags.On, false, false, true);
			this.SetLightsOn(false);
		}
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool CanAiAttack()
	{
		return true;
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x00045EAC File Offset: 0x000440AC
	public virtual float GetAimCone()
	{
		uint num = 0U;
		foreach (global::BaseEntity baseEntity in this.children)
		{
			num += baseEntity.net.ID;
			num = (uint)((int)num + baseEntity.flags);
		}
		uint num2 = CRC.Compute32(0U, num);
		if (num2 != this.cachedModHash)
		{
			this.sightAimConeScale = ProjectileWeaponMod.Mult(this, (ProjectileWeaponMod x) => x.sightAimCone, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
			this.sightAimConeOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.sightAimCone, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
			this.hipAimConeScale = ProjectileWeaponMod.Mult(this, (ProjectileWeaponMod x) => x.hipAimCone, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
			this.hipAimConeOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.hipAimCone, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
			this.cachedModHash = num2;
		}
		float num3 = this.aimCone;
		num3 *= (this.UsingInternalBurstMode() ? this.internalBurstAimConeScale : 1f);
		if (this.recoilProperties != null && this.recoilProperties.overrideAimconeWithCurve && this.primaryMagazine.capacity > 0)
		{
			num3 += this.recoilProperties.aimconeCurve.Evaluate((float)this.numShotsFired / (float)this.primaryMagazine.capacity % 1f) * this.recoilProperties.aimconeCurveScale;
			this.aimconePenalty = 0f;
		}
		if (this.aiming || base.isServer)
		{
			return (num3 + this.aimconePenalty + this.stancePenalty * this.stancePenaltyScale) * this.sightAimConeScale + this.sightAimConeOffset;
		}
		return (num3 + this.aimconePenalty + this.stancePenalty * this.stancePenaltyScale) * this.sightAimConeScale + this.sightAimConeOffset + this.hipAimCone * this.hipAimConeScale + this.hipAimConeOffset;
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x00046168 File Offset: 0x00044368
	public float ScaleRepeatDelay(float delay)
	{
		float num = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.repeatDelay, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
		float num2 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.repeatDelay, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		float num3 = this.UsingInternalBurstMode() ? this.internalBurstFireRateScale : 1f;
		return delay * num * num3 + num2;
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x00046228 File Offset: 0x00044428
	public Projectile.Modifier GetProjectileModifier()
	{
		Projectile.Modifier result = default(Projectile.Modifier);
		result.damageOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.projectileDamage, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		result.damageScale = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.projectileDamage, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * this.GetDamageScale(false);
		result.distanceOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.projectileDistance, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		result.distanceScale = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.projectileDistance, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * this.GetDistanceScale(false);
		return result;
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x00046390 File Offset: 0x00044590
	public bool UsingBurstMode()
	{
		if (this.IsBurstDisabled())
		{
			return false;
		}
		if (this.isBurstWeapon)
		{
			return true;
		}
		if (this.children != null)
		{
			return (from ProjectileWeaponMod x in this.children
			where x != null && x.burstCount > 0
			select x).FirstOrDefault<ProjectileWeaponMod>() != null;
		}
		return false;
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x000463F5 File Offset: 0x000445F5
	public bool UsingInternalBurstMode()
	{
		return !this.IsBurstDisabled() && this.isBurstWeapon;
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00046408 File Offset: 0x00044608
	public bool IsBurstEligable()
	{
		if (this.isBurstWeapon)
		{
			return true;
		}
		if (this.children != null)
		{
			return (from ProjectileWeaponMod x in this.children
			where x != null && x.burstCount > 0
			select x).FirstOrDefault<ProjectileWeaponMod>() != null;
		}
		return false;
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00046463 File Offset: 0x00044663
	public float TimeBetweenBursts()
	{
		return this.repeatDelay * 3f;
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x00046474 File Offset: 0x00044674
	public float GetReloadDuration()
	{
		if (this.fractionalReload)
		{
			int num = Mathf.Min(this.primaryMagazine.capacity - this.primaryMagazine.contents, this.GetAvailableAmmo());
			return this.reloadStartDuration + this.reloadEndDuration + this.reloadFractionDuration * (float)num;
		}
		return this.reloadTime;
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x000464CC File Offset: 0x000446CC
	public int GetAvailableAmmo()
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return this.primaryMagazine.capacity;
		}
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		ownerPlayer.inventory.FindAmmo(list, this.primaryMagazine.definition.ammoTypes);
		int num = 0;
		if (list.Count != 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				global::Item item = list[i];
				if (item.info == this.primaryMagazine.ammoType)
				{
					num += item.amount;
				}
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
		return num;
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00046566 File Offset: 0x00044766
	public bool IsBurstDisabled()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved6) == this.defaultOn;
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x0004657C File Offset: 0x0004477C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	[global::BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	private void ToggleFireMode(global::BaseEntity.RPCMessage msg)
	{
		if (!this.canChangeFireModes)
		{
			return;
		}
		if (!this.IsBurstEligable())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, !base.HasFlag(global::BaseEntity.Flags.Reserved6), false, true);
		base.SendNetworkUpdate_Flags();
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer.IsNpc && ownerPlayer.IsConnected)
		{
			ownerPlayer.ShowToast(GameTip.Styles.Blue_Short, this.IsBurstDisabled() ? this.Toast_BurstDisabled : this.Toast_BurstEnabled, Array.Empty<string>());
		}
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x000465F8 File Offset: 0x000447F8
	protected virtual void ReloadMagazine(int desiredAmount = -1)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		this.primaryMagazine.Reload(ownerPlayer, desiredAmount, true);
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00046640 File Offset: 0x00044840
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void SwitchAmmoTo(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		int num = msg.read.Int32();
		if (num == this.primaryMagazine.ammoType.itemid)
		{
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(num);
		if (itemDefinition == null)
		{
			return;
		}
		ItemModProjectile component = itemDefinition.GetComponent<ItemModProjectile>();
		if (!component || !component.IsAmmo(this.primaryMagazine.definition.ammoTypes))
		{
			return;
		}
		if (this.primaryMagazine.contents > 0)
		{
			ownerPlayer.GiveItem(ItemManager.CreateByItemID(this.primaryMagazine.ammoType.itemid, this.primaryMagazine.contents, 0UL), global::BaseEntity.GiveItemReason.Generic);
			this.primaryMagazine.contents = 0;
		}
		this.primaryMagazine.ammoType = itemDefinition;
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00046721 File Offset: 0x00044921
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		this.reloadStarted = false;
		this.reloadFinished = false;
		this.fractionalInsertCounter = 0;
		this.UpdateAttachmentsState();
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00046744 File Offset: 0x00044944
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void StartReload(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!base.VerifyClientRPC(player))
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		this.reloadFinished = false;
		this.reloadStarted = true;
		this.fractionalInsertCounter = 0;
		if (this.CanRefundAmmo)
		{
			this.primaryMagazine.SwitchAmmoTypesIfNeeded(player);
		}
		this.StartReloadCooldown(this.GetReloadDuration());
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x060006DB RID: 1755 RVA: 0x00003A54 File Offset: 0x00001C54
	protected virtual bool CanRefundAmmo
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x000467AC File Offset: 0x000449AC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void ServerFractionalReloadInsert(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!base.VerifyClientRPC(player))
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (!this.fractionalReload)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload not allowed (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_type");
			return;
		}
		if (!this.reloadStarted)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload request skipped (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_skip");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (this.GetReloadIdle() > 3f)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, string.Concat(new object[]
			{
				"T+",
				this.GetReloadIdle(),
				"s (",
				base.ShortPrefabName,
				")"
			}));
			player.stats.combat.LogInvalid(player, this, "reload_time");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (UnityEngine.Time.time < this.startReloadTime + this.reloadStartDuration)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload too early (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_fraction_too_early");
			this.reloadStarted = false;
			this.reloadFinished = false;
		}
		if (UnityEngine.Time.time < this.startReloadTime + this.reloadStartDuration + (float)this.fractionalInsertCounter * this.reloadFractionDuration)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload rate too high (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_fraction_rate");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		this.fractionalInsertCounter++;
		if (this.primaryMagazine.contents < this.primaryMagazine.capacity)
		{
			this.ReloadMagazine(1);
		}
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x000469C0 File Offset: 0x00044BC0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void Reload(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!base.VerifyClientRPC(player))
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (!this.reloadStarted)
		{
			global::AntiHack.Log(player, AntiHackType.ReloadHack, "Request skipped (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_skip");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (!this.fractionalReload)
		{
			if (this.GetReloadCooldown() > 1f)
			{
				global::AntiHack.Log(player, AntiHackType.ReloadHack, string.Concat(new object[]
				{
					"T-",
					this.GetReloadCooldown(),
					"s (",
					base.ShortPrefabName,
					")"
				}));
				player.stats.combat.LogInvalid(player, this, "reload_time");
				this.reloadStarted = false;
				this.reloadFinished = false;
				return;
			}
			if (this.GetReloadIdle() > 1.5f)
			{
				global::AntiHack.Log(player, AntiHackType.ReloadHack, string.Concat(new object[]
				{
					"T+",
					this.GetReloadIdle(),
					"s (",
					base.ShortPrefabName,
					")"
				}));
				player.stats.combat.LogInvalid(player, this, "reload_time");
				this.reloadStarted = false;
				this.reloadFinished = false;
				return;
			}
		}
		if (this.fractionalReload)
		{
			this.ResetReloadCooldown();
		}
		this.reloadStarted = false;
		this.reloadFinished = true;
		if (!this.fractionalReload)
		{
			this.ReloadMagazine(-1);
		}
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x00046B5C File Offset: 0x00044D5C
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
		if (this.reloadFinished && this.HasReloadCooldown())
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Reloading (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_cooldown");
			return;
		}
		this.reloadStarted = false;
		this.reloadFinished = false;
		if (this.primaryMagazine.contents <= 0 && !this.UsingInfiniteAmmoCheat)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Magazine empty (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "ammo_missing");
			return;
		}
		ItemDefinition primaryMagazineAmmo = this.PrimaryMagazineAmmo;
		ProjectileShoot projectileShoot = ProjectileShoot.Deserialize(msg.read);
		if (primaryMagazineAmmo.itemid != projectileShoot.ammoType)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Ammo mismatch (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "ammo_mismatch");
			return;
		}
		if (!this.UsingInfiniteAmmoCheat)
		{
			this.primaryMagazine.contents--;
		}
		ItemModProjectile component = primaryMagazineAmmo.GetComponent<ItemModProjectile>();
		if (component == null)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "mod_missing");
			return;
		}
		if (projectileShoot.projectiles.Count > component.numProjectiles)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Count mismatch (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "count_mismatch");
			return;
		}
		if (player.InGesture)
		{
			return;
		}
		base.SignalBroadcast(global::BaseEntity.Signal.Attack, string.Empty, msg.connection);
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
				player.NoteFiredProjectile(projectile.projectileID, projectile.startPos, projectile.startVel, this, primaryMagazineAmmo, null);
				this.CreateProjectileEffectClientside(component.projectileObject.resourcePath, projectile.startPos, projectile.startVel, projectile.seed, msg.connection, this.IsSilenced(), false);
			}
		}
		player.MakeNoise(player.transform.position, BaseCombatEntity.ActionVolume.Loud);
		player.stats.Add(component.category + "_fired", projectileShoot.projectiles.Count<ProjectileShoot.Projectile>(), (global::Stats)5);
		player.LifeStoryShotFired(this);
		base.StartAttackCooldown(this.ScaleRepeatDelay(this.repeatDelay) + this.animationDelay);
		player.MarkHostileFor(60f);
		this.UpdateItemCondition();
		this.DidAttackServerside();
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
		float num2 = this.NoiseRadius;
		if (this.IsSilenced())
		{
			num2 *= AI.npc_gun_noise_silencer_modifier;
		}
		Sense.Stimulate(new Sensation
		{
			Type = SensationType.Gunshot,
			Position = player.transform.position,
			Radius = num2,
			DamagePotential = num,
			InitiatorPlayer = player,
			Initiator = player
		});
		if (EACServer.CanSendAnalytics && player.net.connection != null)
		{
			using (TimeWarning.New("EAC.LogPlayerShooting", 0))
			{
				Vector3 networkPosition = player.GetNetworkPosition();
				Quaternion networkRotation = player.GetNetworkRotation();
				global::Item item = this.GetItem();
				string str = (item != null) ? item.info.shortname : "unknown";
				LogPlayerUseWeaponOptions logPlayerUseWeaponOptions = default(LogPlayerUseWeaponOptions);
				logPlayerUseWeaponOptions.UseWeaponData = new LogPlayerUseWeaponData?(new LogPlayerUseWeaponData
				{
					PlayerHandle = EACServer.GetClient(player.net.connection),
					PlayerPosition = new Vec3f?(new Vec3f
					{
						x = networkPosition.x,
						y = networkPosition.y,
						z = networkPosition.z
					}),
					PlayerViewRotation = new Quat?(new Quat
					{
						w = networkRotation.w,
						x = networkRotation.x,
						y = networkRotation.y,
						z = networkRotation.z
					}),
					WeaponName = str
				});
				EACServer.Interface.LogPlayerUseWeapon(ref logPlayerUseWeaponOptions);
			}
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x060006DF RID: 1759 RVA: 0x000470F8 File Offset: 0x000452F8
	protected virtual ItemDefinition PrimaryMagazineAmmo
	{
		get
		{
			return this.primaryMagazine.ammoType;
		}
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00047108 File Offset: 0x00045308
	private void CreateProjectileEffectClientside(string prefabName, Vector3 pos, Vector3 velocity, int seed, Connection sourceConnection, bool silenced = false, bool forceClientsideEffects = false)
	{
		Effect effect = global::BaseProjectile.reusableInstance;
		effect.Clear();
		effect.Init(Effect.Type.Projectile, pos, velocity, sourceConnection);
		effect.scale = (silenced ? 0f : 1f);
		if (forceClientsideEffects)
		{
			effect.scale = 2f;
		}
		effect.pooledString = prefabName;
		effect.number = seed;
		EffectNetwork.Send(effect);
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x00047168 File Offset: 0x00045368
	public void UpdateItemCondition()
	{
		global::Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		float barrelConditionLoss = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>().barrelConditionLoss;
		float num = 0.25f;
		ownerItem.LoseCondition(num + barrelConditionLoss);
		if (ownerItem.contents != null && ownerItem.contents.itemList != null)
		{
			for (int i = ownerItem.contents.itemList.Count - 1; i >= 0; i--)
			{
				global::Item item = ownerItem.contents.itemList[i];
				if (item != null)
				{
					item.LoseCondition(num + barrelConditionLoss);
				}
			}
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x000471F8 File Offset: 0x000453F8
	public bool IsSilenced()
	{
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				ProjectileWeaponMod projectileWeaponMod = baseEntity as ProjectileWeaponMod;
				if (projectileWeaponMod != null && projectileWeaponMod.isSilencer && !projectileWeaponMod.IsBroken())
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x060006E3 RID: 1763 RVA: 0x00007074 File Offset: 0x00005274
	private bool UsingInfiniteAmmoCheat
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00047274 File Offset: 0x00045474
	public override bool CanUseNetworkCache(Connection sendingTo)
	{
		Connection ownerConnection = base.GetOwnerConnection();
		return sendingTo == null || ownerConnection == null || sendingTo != ownerConnection;
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x00047298 File Offset: 0x00045498
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Facepunch.Pool.Get<ProtoBuf.BaseProjectile>();
		if (info.forDisk || info.SendingTo(base.GetOwnerConnection()) || this.ForceSendMagazine(info))
		{
			info.msg.baseProjectile.primaryMagazine = this.primaryMagazine.Save();
		}
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x000472F8 File Offset: 0x000454F8
	public virtual bool ForceSendMagazine(global::BaseNetworkable.SaveInfo saveInfo)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer && ownerPlayer.IsBeingSpectated)
		{
			foreach (global::BaseEntity baseEntity in ownerPlayer.children)
			{
				if (baseEntity.net != null && baseEntity.net.connection == saveInfo.forConnection)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00047380 File Offset: 0x00045580
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.primaryMagazine.Load(info.msg.baseProjectile.primaryMagazine);
		}
	}

	// Token: 0x04000442 RID: 1090
	[Header("NPC Info")]
	public float NoiseRadius = 100f;

	// Token: 0x04000443 RID: 1091
	[Header("Projectile")]
	public float damageScale = 1f;

	// Token: 0x04000444 RID: 1092
	public float distanceScale = 1f;

	// Token: 0x04000445 RID: 1093
	public float projectileVelocityScale = 1f;

	// Token: 0x04000446 RID: 1094
	public bool automatic;

	// Token: 0x04000447 RID: 1095
	public bool usableByTurret = true;

	// Token: 0x04000448 RID: 1096
	[Tooltip("Final damage is scaled by this amount before being applied to a target when this weapon is mounted to a turret")]
	public float turretDamageScale = 0.35f;

	// Token: 0x04000449 RID: 1097
	[Header("Effects")]
	public GameObjectRef attackFX;

	// Token: 0x0400044A RID: 1098
	public GameObjectRef silencedAttack;

	// Token: 0x0400044B RID: 1099
	public GameObjectRef muzzleBrakeAttack;

	// Token: 0x0400044C RID: 1100
	public Transform MuzzlePoint;

	// Token: 0x0400044D RID: 1101
	[Header("Reloading")]
	public float reloadTime = 1f;

	// Token: 0x0400044E RID: 1102
	public bool canUnloadAmmo = true;

	// Token: 0x0400044F RID: 1103
	public global::BaseProjectile.Magazine primaryMagazine;

	// Token: 0x04000450 RID: 1104
	public bool fractionalReload;

	// Token: 0x04000451 RID: 1105
	public float reloadStartDuration;

	// Token: 0x04000452 RID: 1106
	public float reloadFractionDuration;

	// Token: 0x04000453 RID: 1107
	public float reloadEndDuration;

	// Token: 0x04000454 RID: 1108
	[Header("Recoil")]
	public float aimSway = 3f;

	// Token: 0x04000455 RID: 1109
	public float aimSwaySpeed = 1f;

	// Token: 0x04000456 RID: 1110
	public RecoilProperties recoil;

	// Token: 0x04000457 RID: 1111
	[Header("Aim Cone")]
	public AnimationCurve aimconeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04000458 RID: 1112
	public float aimCone;

	// Token: 0x04000459 RID: 1113
	public float hipAimCone = 1.8f;

	// Token: 0x0400045A RID: 1114
	public float aimconePenaltyPerShot;

	// Token: 0x0400045B RID: 1115
	public float aimConePenaltyMax;

	// Token: 0x0400045C RID: 1116
	public float aimconePenaltyRecoverTime = 0.1f;

	// Token: 0x0400045D RID: 1117
	public float aimconePenaltyRecoverDelay = 0.1f;

	// Token: 0x0400045E RID: 1118
	public float stancePenaltyScale = 1f;

	// Token: 0x0400045F RID: 1119
	[Header("Iconsights")]
	public bool hasADS = true;

	// Token: 0x04000460 RID: 1120
	public bool noAimingWhileCycling;

	// Token: 0x04000461 RID: 1121
	public bool manualCycle;

	// Token: 0x04000462 RID: 1122
	[NonSerialized]
	protected bool needsCycle;

	// Token: 0x04000463 RID: 1123
	[NonSerialized]
	protected bool isCycling;

	// Token: 0x04000464 RID: 1124
	[NonSerialized]
	public bool aiming;

	// Token: 0x04000465 RID: 1125
	[Header("ViewModel")]
	public bool useEmptyAmmoState;

	// Token: 0x04000466 RID: 1126
	[Header("Burst Information")]
	public bool isBurstWeapon;

	// Token: 0x04000467 RID: 1127
	public bool canChangeFireModes = true;

	// Token: 0x04000468 RID: 1128
	public bool defaultOn = true;

	// Token: 0x04000469 RID: 1129
	public float internalBurstRecoilScale = 0.8f;

	// Token: 0x0400046A RID: 1130
	public float internalBurstFireRateScale = 0.8f;

	// Token: 0x0400046B RID: 1131
	public float internalBurstAimConeScale = 0.8f;

	// Token: 0x0400046C RID: 1132
	public Translate.Phrase Toast_BurstDisabled = new Translate.Phrase("burst_disabled", "Burst Disabled");

	// Token: 0x0400046D RID: 1133
	public Translate.Phrase Toast_BurstEnabled = new Translate.Phrase("burst enabled", "Burst Enabled");

	// Token: 0x0400046E RID: 1134
	public float resetDuration = 0.3f;

	// Token: 0x0400046F RID: 1135
	public int numShotsFired;

	// Token: 0x04000470 RID: 1136
	[NonSerialized]
	private float nextReloadTime = float.NegativeInfinity;

	// Token: 0x04000471 RID: 1137
	[NonSerialized]
	private float startReloadTime = float.NegativeInfinity;

	// Token: 0x04000472 RID: 1138
	private float lastReloadTime = -10f;

	// Token: 0x04000473 RID: 1139
	private bool modsChangedInitialized;

	// Token: 0x04000474 RID: 1140
	private float stancePenalty;

	// Token: 0x04000475 RID: 1141
	private float aimconePenalty;

	// Token: 0x04000476 RID: 1142
	private uint cachedModHash;

	// Token: 0x04000477 RID: 1143
	private float sightAimConeScale = 1f;

	// Token: 0x04000478 RID: 1144
	private float sightAimConeOffset;

	// Token: 0x04000479 RID: 1145
	private float hipAimConeScale = 1f;

	// Token: 0x0400047A RID: 1146
	private float hipAimConeOffset;

	// Token: 0x0400047B RID: 1147
	protected bool reloadStarted;

	// Token: 0x0400047C RID: 1148
	protected bool reloadFinished;

	// Token: 0x0400047D RID: 1149
	private int fractionalInsertCounter;

	// Token: 0x0400047E RID: 1150
	private static readonly Effect reusableInstance = new Effect();

	// Token: 0x02000B6B RID: 2923
	[Serializable]
	public class Magazine
	{
		// Token: 0x06004A89 RID: 19081 RVA: 0x001905B6 File Offset: 0x0018E7B6
		public void ServerInit()
		{
			if (this.definition.builtInSize > 0)
			{
				this.capacity = this.definition.builtInSize;
			}
		}

		// Token: 0x06004A8A RID: 19082 RVA: 0x001905D8 File Offset: 0x0018E7D8
		public ProtoBuf.Magazine Save()
		{
			ProtoBuf.Magazine magazine = Facepunch.Pool.Get<ProtoBuf.Magazine>();
			if (this.ammoType == null)
			{
				magazine.capacity = this.capacity;
				magazine.contents = 0;
				magazine.ammoType = 0;
			}
			else
			{
				magazine.capacity = this.capacity;
				magazine.contents = this.contents;
				magazine.ammoType = this.ammoType.itemid;
			}
			return magazine;
		}

		// Token: 0x06004A8B RID: 19083 RVA: 0x0019063F File Offset: 0x0018E83F
		public void Load(ProtoBuf.Magazine mag)
		{
			this.contents = mag.contents;
			this.capacity = mag.capacity;
			this.ammoType = ItemManager.FindItemDefinition(mag.ammoType);
		}

		// Token: 0x06004A8C RID: 19084 RVA: 0x0019066A File Offset: 0x0018E86A
		public bool CanReload(global::BasePlayer owner)
		{
			return this.contents < this.capacity && owner.inventory.HasAmmo(this.definition.ammoTypes);
		}

		// Token: 0x06004A8D RID: 19085 RVA: 0x00190692 File Offset: 0x0018E892
		public bool CanAiReload(global::BasePlayer owner)
		{
			return this.contents < this.capacity;
		}

		// Token: 0x06004A8E RID: 19086 RVA: 0x001906A8 File Offset: 0x0018E8A8
		public void SwitchAmmoTypesIfNeeded(global::BasePlayer owner)
		{
			List<global::Item> list = owner.inventory.FindItemIDs(this.ammoType.itemid).ToList<global::Item>();
			if (list.Count == 0)
			{
				List<global::Item> list2 = new List<global::Item>();
				owner.inventory.FindAmmo(list2, this.definition.ammoTypes);
				if (list2.Count == 0)
				{
					return;
				}
				list = owner.inventory.FindItemIDs(list2[0].info.itemid).ToList<global::Item>();
				if (list == null || list.Count == 0)
				{
					return;
				}
				if (this.contents > 0)
				{
					owner.GiveItem(ItemManager.CreateByItemID(this.ammoType.itemid, this.contents, 0UL), global::BaseEntity.GiveItemReason.Generic);
					this.contents = 0;
				}
				this.ammoType = list[0].info;
			}
		}

		// Token: 0x06004A8F RID: 19087 RVA: 0x00190774 File Offset: 0x0018E974
		public bool Reload(global::BasePlayer owner, int desiredAmount = -1, bool canRefundAmmo = true)
		{
			List<global::Item> list = owner.inventory.FindItemIDs(this.ammoType.itemid).ToList<global::Item>();
			if (list.Count == 0)
			{
				List<global::Item> list2 = new List<global::Item>();
				owner.inventory.FindAmmo(list2, this.definition.ammoTypes);
				if (list2.Count == 0)
				{
					return false;
				}
				list = owner.inventory.FindItemIDs(list2[0].info.itemid).ToList<global::Item>();
				if (list == null || list.Count == 0)
				{
					return false;
				}
				if (this.contents > 0)
				{
					if (canRefundAmmo)
					{
						owner.GiveItem(ItemManager.CreateByItemID(this.ammoType.itemid, this.contents, 0UL), global::BaseEntity.GiveItemReason.Generic);
					}
					this.contents = 0;
				}
				this.ammoType = list[0].info;
			}
			int num = desiredAmount;
			if (num == -1)
			{
				num = this.capacity - this.contents;
			}
			foreach (global::Item item in list)
			{
				int amount = item.amount;
				int num2 = Mathf.Min(num, item.amount);
				item.UseItem(num2);
				this.contents += num2;
				num -= num2;
				if (num <= 0)
				{
					break;
				}
			}
			return false;
		}

		// Token: 0x04003E40 RID: 15936
		public global::BaseProjectile.Magazine.Definition definition;

		// Token: 0x04003E41 RID: 15937
		public int capacity;

		// Token: 0x04003E42 RID: 15938
		public int contents;

		// Token: 0x04003E43 RID: 15939
		[ItemSelector(ItemCategory.All)]
		public ItemDefinition ammoType;

		// Token: 0x02000F5D RID: 3933
		[Serializable]
		public struct Definition
		{
			// Token: 0x04004DFF RID: 19967
			[Tooltip("Set to 0 to not use inbuilt mag")]
			public int builtInSize;

			// Token: 0x04004E00 RID: 19968
			[Tooltip("If using inbuilt mag, will accept these types of ammo")]
			[global::InspectorFlags]
			public AmmoTypes ammoTypes;
		}
	}

	// Token: 0x02000B6C RID: 2924
	public static class BaseProjectileFlags
	{
		// Token: 0x04003E44 RID: 15940
		public const global::BaseEntity.Flags BurstToggle = global::BaseEntity.Flags.Reserved6;
	}
}
