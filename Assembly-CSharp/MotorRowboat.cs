using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009E RID: 158
public class MotorRowboat : BaseBoat
{
	// Token: 0x06000E96 RID: 3734 RVA: 0x00079ED0 File Offset: 0x000780D0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MotorRowboat.OnRpcMessage", 0))
		{
			if (rpc == 1873751172U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_EngineToggle ");
				}
				using (TimeWarning.New("RPC_EngineToggle", 0))
				{
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
							this.RPC_EngineToggle(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_EngineToggle");
					}
				}
				return true;
			}
			if (rpc == 1851540757U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenFuel ");
				}
				using (TimeWarning.New("RPC_OpenFuel", 0))
				{
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
							this.RPC_OpenFuel(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_OpenFuel");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x0007A130 File Offset: 0x00078330
	public override void InitShared()
	{
		this.fuelSystem = new EntityFuelSystem(base.isServer, this.fuelStoragePrefab, this.children, true);
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x0007A150 File Offset: 0x00078350
	public override void ServerInit()
	{
		base.ServerInit();
		this.timeSinceLastUsedFuel = 0f;
		base.InvokeRandomized(new Action(this.BoatDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x0007A1A0 File Offset: 0x000783A0
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (this.isSpawned)
			{
				this.fuelSystem.CheckNewChild(child);
			}
			if (child.prefabID == this.storageUnitPrefab.GetEntity().prefabID)
			{
				this.storageUnitInstance.Set((StorageContainer)child);
			}
		}
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x0007A1F9 File Offset: 0x000783F9
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot && this.storageUnitInstance.IsValid(base.isServer))
		{
			this.storageUnitInstance.Get(base.isServer).DropItems(null);
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x0007A232 File Offset: 0x00078432
	public override EntityFuelSystem GetFuelSystem()
	{
		return this.fuelSystem;
	}

	// Token: 0x06000E9C RID: 3740 RVA: 0x0004A2EE File Offset: 0x000484EE
	public override int StartingFuelUnits()
	{
		return 50;
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x0007A23A File Offset: 0x0007843A
	public void BoatDecay()
	{
		if (this.dying)
		{
			return;
		}
		BaseBoat.WaterVehicleDecay(this, 60f, this.timeSinceLastUsedFuel, MotorRowboat.outsidedecayminutes, MotorRowboat.deepwaterdecayminutes);
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x0007A268 File Offset: 0x00078468
	protected override void DoPushAction(global::BasePlayer player)
	{
		if (base.IsFlipped())
		{
			ref Vector3 ptr = base.transform.InverseTransformPoint(player.transform.position);
			float num = 4f;
			if (ptr.x > 0f)
			{
				num = -num;
			}
			this.rigidBody.AddRelativeTorque(Vector3.forward * num, ForceMode.VelocityChange);
			this.rigidBody.AddForce(Vector3.up * 4f, ForceMode.VelocityChange);
			this.startedFlip = 0f;
			base.InvokeRepeatingFixedTime(new Action(this.FlipMonitor));
		}
		else
		{
			Vector3 a = Vector3Ex.Direction2D(player.transform.position, base.transform.position);
			Vector3 vector = Vector3Ex.Direction2D(player.transform.position + player.eyes.BodyForward() * 3f, player.transform.position);
			vector = (Vector3.up * 0.1f + vector).normalized;
			Vector3 position = base.transform.position + a * 2f;
			float num2 = 3f;
			float value = Vector3.Dot(base.transform.forward, vector);
			num2 += Mathf.InverseLerp(0.8f, 1f, value) * 3f;
			this.rigidBody.AddForceAtPosition(vector * num2, position, ForceMode.VelocityChange);
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved5))
		{
			if (this.pushWaterEffect.isValid)
			{
				Effect.server.Run(this.pushWaterEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
			}
		}
		else if (this.pushLandEffect.isValid)
		{
			Effect.server.Run(this.pushLandEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		base.WakeUp();
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x0007A448 File Offset: 0x00078648
	private void FlipMonitor()
	{
		float num = Vector3.Dot(Vector3.up, base.transform.up);
		this.rigidBody.angularVelocity = Vector3.Lerp(this.rigidBody.angularVelocity, Vector3.zero, UnityEngine.Time.fixedDeltaTime * 8f * num);
		if (this.startedFlip > 3f)
		{
			base.CancelInvokeFixedTime(new Action(this.FlipMonitor));
		}
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x0007A4BC File Offset: 0x000786BC
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!base.IsDriver(player))
		{
			return;
		}
		this.fuelSystem.LootFuel(player);
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x0007A4F0 File Offset: 0x000786F0
	[global::BaseEntity.RPC_Server]
	public void RPC_EngineToggle(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (base.InDryDock())
		{
			flag = false;
		}
		if (!base.IsDriver(player))
		{
			return;
		}
		if (flag == this.EngineOn())
		{
			return;
		}
		this.EngineToggle(flag);
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x0007A53F File Offset: 0x0007873F
	public void EngineToggle(bool wantsOn)
	{
		if (!this.fuelSystem.HasFuel(true))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved1, wantsOn, false, true);
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x0007A560 File Offset: 0x00078760
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Invoke(new Action(this.CheckInvalidBoat), 1f);
		if (base.health <= 0f)
		{
			base.Invoke(new Action(this.ActualDeath), vehicle.boat_corpse_seconds);
			this.buoyancy.buoyancyScale = 0f;
			this.dying = true;
		}
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x0007A5C8 File Offset: 0x000787C8
	public void CheckInvalidBoat()
	{
		if (!this.fuelSystem.fuelStorageInstance.IsValid(base.isServer) || !this.storageUnitInstance.IsValid(base.isServer))
		{
			Debug.Log("Destroying invalid boat ");
			base.Invoke(new Action(this.ActualDeath), 1f);
			return;
		}
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x0007A622 File Offset: 0x00078822
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x00020A80 File Offset: 0x0001EC80
	public override bool EngineOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x0007A62C File Offset: 0x0007882C
	public float TimeSinceDriver()
	{
		return UnityEngine.Time.time - this.lastHadDriverTime;
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0007A63A File Offset: 0x0007883A
	public override void DriverInput(InputState inputState, global::BasePlayer player)
	{
		base.DriverInput(inputState, player);
		this.lastHadDriverTime = UnityEngine.Time.time;
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x0007A650 File Offset: 0x00078850
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		float num = this.TimeSinceDriver();
		if (num > 15f)
		{
			this.steering += Mathf.InverseLerp(15f, 30f, num);
			this.steering = Mathf.Clamp(-1f, 1f, this.steering);
			if (num > 75f)
			{
				this.gasPedal = 0f;
			}
		}
		this.SetFlags();
		this.UpdateDrag();
		if (this.dying)
		{
			this.buoyancy.buoyancyScale = Mathf.Lerp(this.buoyancy.buoyancyScale, 0f, UnityEngine.Time.fixedDeltaTime * 0.1f);
		}
		else
		{
			float num2 = 1f;
			float value = this.rigidBody.velocity.Magnitude2D();
			float num3 = Mathf.InverseLerp(1f, 10f, value) * 0.5f * base.healthFraction;
			if (!this.EngineOn())
			{
				num3 = 0f;
			}
			float num4 = 1f - 0.3f * (1f - base.healthFraction);
			this.buoyancy.buoyancyScale = (num2 + num3) * num4;
		}
		if (this.EngineOn())
		{
			float num5 = base.HasFlag(global::BaseEntity.Flags.Reserved2) ? 1f : 0.0333f;
			this.fuelSystem.TryUseFuel(UnityEngine.Time.fixedDeltaTime * num5, this.fuelPerSec);
			this.timeSinceLastUsedFuel = 0f;
		}
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x0007A7BC File Offset: 0x000789BC
	private void SetFlags()
	{
		using (TimeWarning.New("SetFlag", 0))
		{
			bool b = this.EngineOn() && !base.IsFlipped() && base.healthFraction > 0f && this.fuelSystem.HasFuel(false) && this.TimeSinceDriver() < 75f;
			global::BaseEntity.Flags flags = this.flags;
			base.SetFlag(global::BaseEntity.Flags.Reserved3, this.steering > 0f, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved4, this.steering < 0f, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved1, b, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved2, this.EngineOn() && this.gasPedal != 0f, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved5, this.buoyancy.submergedFraction > 0.85f, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved6, this.fuelSystem.HasFuel(false), false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved8, base.RecentlyPushed, false, false);
			if (flags != this.flags)
			{
				base.Invoke(new Action(base.SendNetworkUpdate_Flags), 0f);
			}
		}
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x0007A914 File Offset: 0x00078B14
	protected override bool DetermineIfStationary()
	{
		return base.GetLocalVelocity().sqrMagnitude < 0.5f && !this.AnyMounted();
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x0007A944 File Offset: 0x00078B44
	public override void SeatClippedWorld(BaseMountable mountable)
	{
		global::BasePlayer mounted = mountable.GetMounted();
		if (mounted == null)
		{
			return;
		}
		if (base.IsDriver(mounted))
		{
			this.steering = 0f;
			this.gasPedal = 0f;
		}
		float num = Mathf.InverseLerp(4f, 20f, this.rigidBody.velocity.magnitude);
		if (num > 0f)
		{
			mounted.Hurt(num * 100f, DamageType.Blunt, this, false);
		}
		if (mounted != null && mounted.isMounted)
		{
			base.SeatClippedWorld(mountable);
		}
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x0007A9D8 File Offset: 0x00078BD8
	public void UpdateDrag()
	{
		float value = this.rigidBody.velocity.SqrMagnitude2D();
		float num = Mathf.InverseLerp(0f, 2f, value);
		this.rigidBody.angularDrag = this.angularDragBase + this.angularDragVelocity * num;
		this.rigidBody.drag = this.landDrag + this.waterDrag * Mathf.InverseLerp(0f, 1f, this.buoyancy.submergedFraction);
		if (this.offAxisDrag > 0f)
		{
			float value2 = Vector3.Dot(base.transform.forward, this.rigidBody.velocity.normalized);
			float num2 = Mathf.InverseLerp(0.98f, 0.92f, value2);
			this.rigidBody.drag += num2 * this.offAxisDrag * this.buoyancy.submergedFraction;
		}
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x0007AAC0 File Offset: 0x00078CC0
	public override void OnKilled(HitInfo info)
	{
		if (this.dying)
		{
			return;
		}
		this.dying = true;
		this.repair.enabled = false;
		base.Invoke(new Action(this.DismountAllPlayers), 10f);
		base.Invoke(new Action(this.ActualDeath), vehicle.boat_corpse_seconds);
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x00026D90 File Offset: 0x00024F90
	public void ActualDeath()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x0007AB18 File Offset: 0x00078D18
	public override bool MountEligable(global::BasePlayer player)
	{
		return !this.dying && (this.rigidBody.velocity.magnitude < 5f || !base.HasDriver()) && base.MountEligable(player);
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x0007AB5C File Offset: 0x00078D5C
	public override bool HasValidDismountPosition(global::BasePlayer player)
	{
		if (base.GetWorldVelocity().magnitude <= 4f)
		{
			foreach (Transform transform in this.stationaryDismounts)
			{
				if (this.ValidDismountPosition(player, transform.transform.position))
				{
					return true;
				}
			}
		}
		return base.HasValidDismountPosition(player);
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x0007ABB4 File Offset: 0x00078DB4
	public override bool GetDismountPosition(global::BasePlayer player, out Vector3 res)
	{
		if (this.rigidBody.velocity.magnitude <= 4f)
		{
			List<Vector3> list = Facepunch.Pool.GetList<Vector3>();
			foreach (Transform transform in this.stationaryDismounts)
			{
				if (this.ValidDismountPosition(player, transform.transform.position))
				{
					list.Add(transform.transform.position);
				}
			}
			if (list.Count > 0)
			{
				Vector3 pos = player.transform.position;
				list.Sort((Vector3 a, Vector3 b) => Vector3.Distance(a, pos).CompareTo(Vector3.Distance(b, pos)));
				res = list[0];
				Facepunch.Pool.FreeList<Vector3>(ref list);
				return true;
			}
			Facepunch.Pool.FreeList<Vector3>(ref list);
		}
		return base.GetDismountPosition(player, out res);
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x0007AC80 File Offset: 0x00078E80
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.motorBoat = Facepunch.Pool.Get<Motorboat>();
		info.msg.motorBoat.storageid = this.storageUnitInstance.uid;
		info.msg.motorBoat.fuelStorageID = this.fuelSystem.fuelStorageInstance.uid;
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x0007ACE0 File Offset: 0x00078EE0
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && base.IsStationary() && (pusher.WaterFactor() <= 0.6f || base.IsFlipped()) && (base.IsFlipped() || !pusher.IsStandingOnEntity(this, 8192)) && Vector3.Distance(pusher.transform.position, base.transform.position) <= 5f && !this.dying && (!pusher.isMounted && pusher.IsOnGround() && base.healthFraction > 0f) && this.ShowPushMenu(pusher);
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x0007AD83 File Offset: 0x00078F83
	private bool ShowPushMenu(global::BasePlayer player)
	{
		return (base.IsFlipped() || !player.IsStandingOnEntity(this, 8192)) && base.IsStationary() && (player.WaterFactor() <= 0.6f || base.IsFlipped());
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x0007ADBC File Offset: 0x00078FBC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.motorBoat != null)
		{
			this.fuelSystem.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
			this.storageUnitInstance.uid = info.msg.motorBoat.storageid;
		}
	}

	// Token: 0x04000969 RID: 2409
	[Header("Audio")]
	public BlendedSoundLoops engineLoops;

	// Token: 0x0400096A RID: 2410
	public BlendedSoundLoops waterLoops;

	// Token: 0x0400096B RID: 2411
	public SoundDefinition engineStartSoundDef;

	// Token: 0x0400096C RID: 2412
	public SoundDefinition engineStopSoundDef;

	// Token: 0x0400096D RID: 2413
	public SoundDefinition movementSplashAccentSoundDef;

	// Token: 0x0400096E RID: 2414
	public SoundDefinition engineSteerSoundDef;

	// Token: 0x0400096F RID: 2415
	public GameObjectRef pushLandEffect;

	// Token: 0x04000970 RID: 2416
	public GameObjectRef pushWaterEffect;

	// Token: 0x04000971 RID: 2417
	public float waterSpeedDivisor = 10f;

	// Token: 0x04000972 RID: 2418
	public float turnPitchModScale = -0.25f;

	// Token: 0x04000973 RID: 2419
	public float tiltPitchModScale = 0.3f;

	// Token: 0x04000974 RID: 2420
	public float splashAccentFrequencyMin = 1f;

	// Token: 0x04000975 RID: 2421
	public float splashAccentFrequencyMax = 10f;

	// Token: 0x04000976 RID: 2422
	protected const global::BaseEntity.Flags Flag_EngineOn = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000977 RID: 2423
	protected const global::BaseEntity.Flags Flag_ThrottleOn = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000978 RID: 2424
	protected const global::BaseEntity.Flags Flag_TurnLeft = global::BaseEntity.Flags.Reserved3;

	// Token: 0x04000979 RID: 2425
	protected const global::BaseEntity.Flags Flag_TurnRight = global::BaseEntity.Flags.Reserved4;

	// Token: 0x0400097A RID: 2426
	protected const global::BaseEntity.Flags Flag_Submerged = global::BaseEntity.Flags.Reserved5;

	// Token: 0x0400097B RID: 2427
	protected const global::BaseEntity.Flags Flag_HasFuel = global::BaseEntity.Flags.Reserved6;

	// Token: 0x0400097C RID: 2428
	protected const global::BaseEntity.Flags Flag_RecentlyPushed = global::BaseEntity.Flags.Reserved8;

	// Token: 0x0400097D RID: 2429
	private const float submergeFractionMinimum = 0.85f;

	// Token: 0x0400097E RID: 2430
	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	// Token: 0x0400097F RID: 2431
	public float fuelPerSec;

	// Token: 0x04000980 RID: 2432
	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	// Token: 0x04000981 RID: 2433
	public EntityRef<StorageContainer> storageUnitInstance;

	// Token: 0x04000982 RID: 2434
	[Header("Effects")]
	public Transform boatRear;

	// Token: 0x04000983 RID: 2435
	public ParticleSystemContainer wakeEffect;

	// Token: 0x04000984 RID: 2436
	public ParticleSystemContainer engineEffectIdle;

	// Token: 0x04000985 RID: 2437
	public ParticleSystemContainer engineEffectThrottle;

	// Token: 0x04000986 RID: 2438
	public Projector causticsProjector;

	// Token: 0x04000987 RID: 2439
	public Transform causticsDepthTest;

	// Token: 0x04000988 RID: 2440
	public Transform engineLeftHandPosition;

	// Token: 0x04000989 RID: 2441
	public Transform engineRotate;

	// Token: 0x0400098A RID: 2442
	public Transform propellerRotate;

	// Token: 0x0400098B RID: 2443
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 1f;

	// Token: 0x0400098C RID: 2444
	[ServerVar(Help = "How long before a boat loses all its health while outside. If it's in deep water, deepwaterdecayminutes is used")]
	public static float outsidedecayminutes = 180f;

	// Token: 0x0400098D RID: 2445
	[ServerVar(Help = "How long before a boat loses all its health while in deep water")]
	public static float deepwaterdecayminutes = 120f;

	// Token: 0x0400098E RID: 2446
	protected EntityFuelSystem fuelSystem;

	// Token: 0x0400098F RID: 2447
	private TimeSince timeSinceLastUsedFuel;

	// Token: 0x04000990 RID: 2448
	public Transform[] stationaryDismounts;

	// Token: 0x04000991 RID: 2449
	public Collider mainCollider;

	// Token: 0x04000992 RID: 2450
	public float angularDragBase = 0.5f;

	// Token: 0x04000993 RID: 2451
	public float angularDragVelocity = 0.5f;

	// Token: 0x04000994 RID: 2452
	public float landDrag = 0.2f;

	// Token: 0x04000995 RID: 2453
	public float waterDrag = 0.8f;

	// Token: 0x04000996 RID: 2454
	public float offAxisDrag = 1f;

	// Token: 0x04000997 RID: 2455
	public float offAxisDot = 0.25f;

	// Token: 0x04000998 RID: 2456
	private const float DECAY_TICK_TIME = 60f;

	// Token: 0x04000999 RID: 2457
	private TimeSince startedFlip;

	// Token: 0x0400099A RID: 2458
	private float lastHadDriverTime;

	// Token: 0x0400099B RID: 2459
	private bool dying;

	// Token: 0x0400099C RID: 2460
	private const float maxVelForStationaryDismount = 4f;
}
