using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008D RID: 141
public class LiquidWeapon : BaseLiquidVessel
{
	// Token: 0x06000D07 RID: 3335 RVA: 0x0006E104 File Offset: 0x0006C304
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LiquidWeapon.OnRpcMessage", 0))
		{
			if (rpc == 1600824953U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - PumpWater ");
				}
				using (TimeWarning.New("PumpWater", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1600824953U, "PumpWater", this, player))
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
							this.PumpWater(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in PumpWater");
					}
				}
				return true;
			}
			if (rpc == 3724096303U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartFiring ");
				}
				using (TimeWarning.New("StartFiring", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3724096303U, "StartFiring", this, player))
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
							this.StartFiring(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in StartFiring");
					}
				}
				return true;
			}
			if (rpc == 789289044U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StopFiring ");
				}
				using (TimeWarning.New("StopFiring", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(789289044U, "StopFiring", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							this.StopFiring();
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in StopFiring");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x0006E51C File Offset: 0x0006C71C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void StartFiring(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (this.OnCooldown())
		{
			return;
		}
		if (!this.RequiresPumping)
		{
			this.pressure = this.MaxPressure;
		}
		if (!this.CanFire(player))
		{
			return;
		}
		base.CancelInvoke("FireTick");
		base.InvokeRepeating("FireTick", 0f, this.FireRate);
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		this.StartCooldown(this.FireRate);
		if (base.isServer)
		{
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x0006E59D File Offset: 0x0006C79D
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void StopFiring()
	{
		base.CancelInvoke("FireTick");
		if (!this.RequiresPumping)
		{
			this.pressure = this.MaxPressure;
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (base.isServer)
		{
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x0006E5D8 File Offset: 0x0006C7D8
	private bool CanFire(global::BasePlayer player)
	{
		if (this.RequiresPumping && this.pressure < this.PressureLossPerTick)
		{
			return false;
		}
		if (player == null)
		{
			return false;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Open))
		{
			return false;
		}
		if (base.AmountHeld() <= 0)
		{
			return false;
		}
		if (!player.CanInteract())
		{
			return false;
		}
		if (!player.CanAttack() || player.IsRunning())
		{
			return false;
		}
		global::Item item = this.GetItem();
		return item != null && item.contents != null;
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x0006E64F File Offset: 0x0006C84F
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void PumpWater(global::BaseEntity.RPCMessage msg)
	{
		this.PumpWater();
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x0006E658 File Offset: 0x0006C858
	private void PumpWater()
	{
		if (base.GetOwnerPlayer() == null)
		{
			return;
		}
		if (this.OnCooldown())
		{
			return;
		}
		if (this.Firing())
		{
			return;
		}
		this.pressure += this.PressureGainedPerPump;
		this.pressure = Mathf.Min(this.pressure, this.MaxPressure);
		this.StartCooldown(this.PumpingBlockDuration);
		base.GetOwnerPlayer().SignalBroadcast(global::BaseEntity.Signal.Reload, null);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x0006E6D0 File Offset: 0x0006C8D0
	private void FireTick()
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!this.CanFire(ownerPlayer))
		{
			this.StopFiring();
			return;
		}
		int num = Mathf.Min(this.FireAmountML, base.AmountHeld());
		if (num == 0)
		{
			this.StopFiring();
			return;
		}
		base.LoseWater(num);
		float currentRange = this.CurrentRange;
		this.pressure -= this.PressureLossPerTick;
		if (this.pressure <= 0)
		{
			this.StopFiring();
		}
		Ray ray = ownerPlayer.eyes.BodyRay();
		Debug.DrawLine(ray.origin, ray.origin + ray.direction * currentRange, Color.blue, 1f);
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(ray, out raycastHit, currentRange, 1218652417))
		{
			this.DoSplash(ownerPlayer, raycastHit.point, ray.direction, num);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x0006E7AC File Offset: 0x0006C9AC
	private void DoSplash(global::BasePlayer attacker, Vector3 position, Vector3 direction, int amount)
	{
		global::Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		global::Item slot = item.contents.GetSlot(0);
		if (slot == null || slot.amount <= 0)
		{
			return;
		}
		if (slot.info == null)
		{
			return;
		}
		WaterBall.DoSplash(position, this.SplashRadius, slot.info, amount);
		DamageUtil.RadiusDamage(attacker, base.LookupPrefab(), position, this.MinDmgRadius, this.MaxDmgRadius, this.Damage, 131072, true);
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x0006E830 File Offset: 0x0006CA30
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		this.StopFiring();
	}

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000D10 RID: 3344 RVA: 0x0006E83E File Offset: 0x0006CA3E
	public float PressureFraction
	{
		get
		{
			return (float)this.pressure / (float)this.MaxPressure;
		}
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000D11 RID: 3345 RVA: 0x0006E84F File Offset: 0x0006CA4F
	public float MinimumPressureFraction
	{
		get
		{
			return (float)this.PressureGainedPerPump / (float)this.MaxPressure;
		}
	}

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000D12 RID: 3346 RVA: 0x0006E860 File Offset: 0x0006CA60
	public float CurrentRange
	{
		get
		{
			if (!this.UseFalloffCurve)
			{
				return this.MaxRange;
			}
			return this.MaxRange * this.FalloffCurve.Evaluate((float)(this.MaxPressure - this.pressure) / (float)this.MaxPressure);
		}
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x0006E899 File Offset: 0x0006CA99
	private void StartCooldown(float duration)
	{
		if (UnityEngine.Time.realtimeSinceStartup + duration > this.cooldownTime)
		{
			this.cooldownTime = UnityEngine.Time.realtimeSinceStartup + duration;
		}
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x0006E8B7 File Offset: 0x0006CAB7
	private bool OnCooldown()
	{
		return UnityEngine.Time.realtimeSinceStartup < this.cooldownTime;
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x0002782C File Offset: 0x00025A2C
	private bool Firing()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x0006E8C8 File Offset: 0x0006CAC8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Facepunch.Pool.Get<ProtoBuf.BaseProjectile>();
		info.msg.baseProjectile.primaryMagazine = Facepunch.Pool.Get<Magazine>();
		info.msg.baseProjectile.primaryMagazine.contents = this.pressure;
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x0006E91C File Offset: 0x0006CB1C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.pressure = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}

	// Token: 0x0400083B RID: 2107
	[Header("Liquid Weapon")]
	public float FireRate = 0.2f;

	// Token: 0x0400083C RID: 2108
	public float MaxRange = 10f;

	// Token: 0x0400083D RID: 2109
	public int FireAmountML = 100;

	// Token: 0x0400083E RID: 2110
	public int MaxPressure = 100;

	// Token: 0x0400083F RID: 2111
	public int PressureLossPerTick = 5;

	// Token: 0x04000840 RID: 2112
	public int PressureGainedPerPump = 25;

	// Token: 0x04000841 RID: 2113
	public float MinDmgRadius = 0.15f;

	// Token: 0x04000842 RID: 2114
	public float MaxDmgRadius = 0.15f;

	// Token: 0x04000843 RID: 2115
	public float SplashRadius = 2f;

	// Token: 0x04000844 RID: 2116
	public GameObjectRef ImpactSplashEffect;

	// Token: 0x04000845 RID: 2117
	public AnimationCurve PowerCurve;

	// Token: 0x04000846 RID: 2118
	public List<DamageTypeEntry> Damage;

	// Token: 0x04000847 RID: 2119
	public LiquidWeaponEffects EntityWeaponEffects;

	// Token: 0x04000848 RID: 2120
	public bool RequiresPumping;

	// Token: 0x04000849 RID: 2121
	public bool AutoPump;

	// Token: 0x0400084A RID: 2122
	public bool WaitForFillAnim;

	// Token: 0x0400084B RID: 2123
	public bool UseFalloffCurve;

	// Token: 0x0400084C RID: 2124
	public AnimationCurve FalloffCurve;

	// Token: 0x0400084D RID: 2125
	public float PumpingBlockDuration = 0.5f;

	// Token: 0x0400084E RID: 2126
	public float StartFillingBlockDuration = 2f;

	// Token: 0x0400084F RID: 2127
	public float StopFillingBlockDuration = 1f;

	// Token: 0x04000850 RID: 2128
	private float cooldownTime;

	// Token: 0x04000851 RID: 2129
	private int pressure;

	// Token: 0x04000852 RID: 2130
	public const string RadiationFightAchievement = "SUMMER_RADICAL";

	// Token: 0x04000853 RID: 2131
	public const string SoakedAchievement = "SUMMER_SOAKED";

	// Token: 0x04000854 RID: 2132
	public const string LiquidatorAchievement = "SUMMER_LIQUIDATOR";

	// Token: 0x04000855 RID: 2133
	public const string NoPressureAchievement = "SUMMER_NO_PRESSURE";
}
