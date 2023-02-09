using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x02000070 RID: 112
public class FlameThrower : AttackEntity
{
	// Token: 0x06000AA3 RID: 2723 RVA: 0x0005F90C File Offset: 0x0005DB0C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FlameThrower.OnRpcMessage", 0))
		{
			if (rpc == 3381353917U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoReload ");
				}
				using (TimeWarning.New("DoReload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3381353917U, "DoReload", this, player))
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
							this.DoReload(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoReload");
					}
				}
				return true;
			}
			if (rpc == 3749570935U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetFiring ");
				}
				using (TimeWarning.New("SetFiring", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3749570935U, "SetFiring", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage firing = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetFiring(firing);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SetFiring");
					}
				}
				return true;
			}
			if (rpc == 1057268396U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TogglePilotLight ");
				}
				using (TimeWarning.New("TogglePilotLight", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1057268396U, "TogglePilotLight", this, player))
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
							this.TogglePilotLight(msg3);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in TogglePilotLight");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x0005FD58 File Offset: 0x0005DF58
	private bool IsWeaponBusy()
	{
		return UnityEngine.Time.realtimeSinceStartup < this.nextReadyTime;
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x0005FD67 File Offset: 0x0005DF67
	private void SetBusyFor(float dur)
	{
		this.nextReadyTime = UnityEngine.Time.realtimeSinceStartup + dur;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x0005FD76 File Offset: 0x0005DF76
	private void ClearBusy()
	{
		this.nextReadyTime = UnityEngine.Time.realtimeSinceStartup - 1f;
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x0005FD8C File Offset: 0x0005DF8C
	public void ReduceAmmo(float firingTime)
	{
		this.ammoRemainder += this.fuelPerSec * firingTime;
		if (this.ammoRemainder >= 1f)
		{
			int num = Mathf.FloorToInt(this.ammoRemainder);
			this.ammoRemainder -= (float)num;
			if (this.ammoRemainder >= 1f)
			{
				num++;
				this.ammoRemainder -= 1f;
			}
			this.ammo -= num;
			if (this.ammo <= 0)
			{
				this.ammo = 0;
			}
		}
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x0005FE17 File Offset: 0x0005E017
	public void PilotLightToggle_Shared()
	{
		base.SetFlag(global::BaseEntity.Flags.On, !base.HasFlag(global::BaseEntity.Flags.On), false, true);
		if (base.isServer)
		{
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x0002782C File Offset: 0x00025A2C
	public bool IsPilotOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x000028BF File Offset: 0x00000ABF
	public bool IsFlameOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.OnFire);
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x0005FE3B File Offset: 0x0005E03B
	public bool HasAmmo()
	{
		return this.GetAmmo() != null;
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x0005FE48 File Offset: 0x0005E048
	public global::Item GetAmmo()
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		global::Item item = ownerPlayer.inventory.containerMain.FindItemsByItemName(this.fuelType.shortname);
		if (item == null)
		{
			item = ownerPlayer.inventory.containerBelt.FindItemsByItemName(this.fuelType.shortname);
		}
		return item;
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x0005FEA4 File Offset: 0x0005E0A4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.ammo = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x0005FEF2 File Offset: 0x0005E0F2
	public override void CollectedForCrafting(global::Item item, global::BasePlayer crafter)
	{
		this.ServerCommand(item, "unload_ammo", crafter);
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x0005FF04 File Offset: 0x0005E104
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Facepunch.Pool.Get<ProtoBuf.BaseProjectile>();
		info.msg.baseProjectile.primaryMagazine = Facepunch.Pool.Get<Magazine>();
		info.msg.baseProjectile.primaryMagazine.contents = this.ammo;
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x0005FF58 File Offset: 0x0005E158
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void SetFiring(global::BaseEntity.RPCMessage msg)
	{
		bool flameState = msg.read.Bit();
		this.SetFlameState(flameState);
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0005FF78 File Offset: 0x0005E178
	public override void ServerUse()
	{
		if (base.IsOnFire())
		{
			return;
		}
		this.SetFlameState(true);
		base.Invoke(new Action(this.StopFlameState), 0.2f);
		base.ServerUse();
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x0005FFA7 File Offset: 0x0005E1A7
	public override void TopUpAmmo()
	{
		this.ammo = this.maxAmmo;
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0005FFB5 File Offset: 0x0005E1B5
	public override float AmmoFraction()
	{
		return (float)this.ammo / (float)this.maxAmmo;
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x0005FFC6 File Offset: 0x0005E1C6
	public override bool ServerIsReloading()
	{
		return UnityEngine.Time.time < this.lastReloadTime + this.reloadDuration;
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0005FFDC File Offset: 0x0005E1DC
	public override bool CanReload()
	{
		return this.ammo < this.maxAmmo;
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0005FFEC File Offset: 0x0005E1EC
	public override void ServerReload()
	{
		if (this.ServerIsReloading())
		{
			return;
		}
		this.lastReloadTime = UnityEngine.Time.time;
		base.StartAttackCooldown(this.reloadDuration);
		base.GetOwnerPlayer().SignalBroadcast(global::BaseEntity.Signal.Reload, null);
		this.ammo = this.maxAmmo;
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x00060027 File Offset: 0x0005E227
	public void StopFlameState()
	{
		this.SetFlameState(false);
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00060030 File Offset: 0x0005E230
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void DoReload(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		global::Item item;
		while (this.ammo < this.maxAmmo && (item = this.GetAmmo()) != null && item.amount > 0)
		{
			int num = Mathf.Min(this.maxAmmo - this.ammo, item.amount);
			this.ammo += num;
			item.UseItem(num);
		}
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x000600C0 File Offset: 0x0005E2C0
	public void SetFlameState(bool wantsOn)
	{
		if (wantsOn)
		{
			this.ammo--;
			if (this.ammo < 0)
			{
				this.ammo = 0;
			}
		}
		if (wantsOn && this.ammo <= 0)
		{
			wantsOn = false;
		}
		base.SetFlag(global::BaseEntity.Flags.OnFire, wantsOn, false, true);
		if (this.IsFlameOn())
		{
			this.nextFlameTime = UnityEngine.Time.realtimeSinceStartup + 1f;
			this.lastFlameTick = UnityEngine.Time.realtimeSinceStartup;
			base.InvokeRepeating(new Action(this.FlameTick), this.tickRate, this.tickRate);
			return;
		}
		base.CancelInvoke(new Action(this.FlameTick));
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x0006015E File Offset: 0x0005E35E
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void TogglePilotLight(global::BaseEntity.RPCMessage msg)
	{
		this.PilotLightToggle_Shared();
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x00060166 File Offset: 0x0005E366
	public override void OnHeldChanged()
	{
		this.SetFlameState(false);
		base.OnHeldChanged();
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00060178 File Offset: 0x0005E378
	public void FlameTick()
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastFlameTick;
		this.lastFlameTick = UnityEngine.Time.realtimeSinceStartup;
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		this.ReduceAmmo(num);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		Ray ray = ownerPlayer.eyes.BodyRay();
		Vector3 origin = ray.origin;
		RaycastHit raycastHit;
		bool flag = UnityEngine.Physics.SphereCast(ray, 0.3f, out raycastHit, this.flameRange, 1218652417);
		if (!flag)
		{
			raycastHit.point = origin + ray.direction * this.flameRange;
		}
		float num2 = ownerPlayer.IsNpc ? this.npcDamageScale : 1f;
		float amount = this.damagePerSec[0].amount;
		this.damagePerSec[0].amount = amount * num * num2;
		DamageUtil.RadiusDamage(ownerPlayer, base.LookupPrefab(), raycastHit.point - ray.direction * 0.1f, this.flameRadius * 0.5f, this.flameRadius, this.damagePerSec, 2279681, true);
		this.damagePerSec[0].amount = amount;
		if (flag && UnityEngine.Time.realtimeSinceStartup >= this.nextFlameTime && raycastHit.distance > 1.1f)
		{
			this.nextFlameTime = UnityEngine.Time.realtimeSinceStartup + 0.45f;
			Vector3 point = raycastHit.point;
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, point - ray.direction * 0.25f, default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.creatorEntity = ownerPlayer;
				baseEntity.Spawn();
			}
		}
		if (this.ammo == 0)
		{
			this.SetFlameState(false);
		}
		global::Item ownerItem = base.GetOwnerItem();
		if (ownerItem != null)
		{
			ownerItem.LoseCondition(num);
		}
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x00060358 File Offset: 0x0005E558
	public override void ServerCommand(global::Item item, string command, global::BasePlayer player)
	{
		if (item == null)
		{
			return;
		}
		if (command == "unload_ammo")
		{
			int num = this.ammo;
			if (num > 0)
			{
				this.ammo = 0;
				base.SendNetworkUpdateImmediate(false);
				global::Item item2 = ItemManager.Create(this.fuelType, num, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
				{
					item2.Drop(player.eyes.position, player.eyes.BodyForward() * 2f, default(Quaternion));
				}
			}
		}
	}

	// Token: 0x040006E6 RID: 1766
	[Header("Flame Thrower")]
	public int maxAmmo = 100;

	// Token: 0x040006E7 RID: 1767
	public int ammo = 100;

	// Token: 0x040006E8 RID: 1768
	public ItemDefinition fuelType;

	// Token: 0x040006E9 RID: 1769
	public float timeSinceLastAttack;

	// Token: 0x040006EA RID: 1770
	[FormerlySerializedAs("nextAttackTime")]
	public float nextReadyTime;

	// Token: 0x040006EB RID: 1771
	public float flameRange = 10f;

	// Token: 0x040006EC RID: 1772
	public float flameRadius = 2.5f;

	// Token: 0x040006ED RID: 1773
	public ParticleSystem[] flameEffects;

	// Token: 0x040006EE RID: 1774
	public FlameJet jet;

	// Token: 0x040006EF RID: 1775
	public GameObjectRef fireballPrefab;

	// Token: 0x040006F0 RID: 1776
	public List<DamageTypeEntry> damagePerSec;

	// Token: 0x040006F1 RID: 1777
	public SoundDefinition flameStart3P;

	// Token: 0x040006F2 RID: 1778
	public SoundDefinition flameLoop3P;

	// Token: 0x040006F3 RID: 1779
	public SoundDefinition flameStop3P;

	// Token: 0x040006F4 RID: 1780
	public SoundDefinition pilotLoopSoundDef;

	// Token: 0x040006F5 RID: 1781
	private float tickRate = 0.25f;

	// Token: 0x040006F6 RID: 1782
	private float lastFlameTick;

	// Token: 0x040006F7 RID: 1783
	public float fuelPerSec;

	// Token: 0x040006F8 RID: 1784
	private float ammoRemainder;

	// Token: 0x040006F9 RID: 1785
	public float reloadDuration = 3.5f;

	// Token: 0x040006FA RID: 1786
	private float lastReloadTime = -10f;

	// Token: 0x040006FB RID: 1787
	private float nextFlameTime;
}
