using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009B RID: 155
public class ModularCar : BaseModularVehicle, TakeCollisionDamage.ICanRestoreVelocity, CarPhysics<global::ModularCar>.ICar, IVehicleLockUser, VehicleChassisVisuals<global::ModularCar>.IClientWheelUser
{
	// Token: 0x06000E09 RID: 3593 RVA: 0x0007635C File Offset: 0x0007455C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ModularCar.OnRpcMessage", 0))
		{
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
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenFuel(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_OpenFuel");
					}
				}
				return true;
			}
			if (rpc == 1382140449U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenFuelWithKeycode ");
				}
				using (TimeWarning.New("RPC_OpenFuelWithKeycode", 0))
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
							this.RPC_OpenFuelWithKeycode(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_OpenFuelWithKeycode");
					}
				}
				return true;
			}
			if (rpc == 2818660542U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TryMountWithKeycode ");
				}
				using (TimeWarning.New("RPC_TryMountWithKeycode", 0))
				{
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
							this.RPC_TryMountWithKeycode(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_TryMountWithKeycode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000E0A RID: 3594 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool AlwaysAllowBradleyTargeting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000E0B RID: 3595 RVA: 0x000766C8 File Offset: 0x000748C8
	public VehicleTerrainHandler.Surface OnSurface
	{
		get
		{
			if (this.serverTerrainHandler == null)
			{
				return VehicleTerrainHandler.Surface.Default;
			}
			return this.serverTerrainHandler.OnSurface;
		}
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x000766E0 File Offset: 0x000748E0
	public override void ServerInit()
	{
		base.ServerInit();
		this.carPhysics = new CarPhysics<global::ModularCar>(this, base.transform, this.rigidBody, this.carSettings);
		this.serverTerrainHandler = new VehicleTerrainHandler(this);
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnPreassignedModules();
		}
		this.lastEngineOnTime = UnityEngine.Time.realtimeSinceStartup;
		global::ModularCar.allCarsList.Add(this);
		this.collisionCheckBounds = new Bounds(this.mainChassisCollider.center, new Vector3(this.mainChassisCollider.size.x - 0.5f, 0.05f, this.mainChassisCollider.size.z - 0.5f));
		base.InvokeRandomized(new Action(this.UpdateClients), 0f, 0.15f, 0.02f);
		base.InvokeRandomized(new Action(this.DecayTick), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x000767D9 File Offset: 0x000749D9
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		global::ModularCar.allCarsList.Remove(this);
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x000767ED File Offset: 0x000749ED
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.CarLock.PostServerLoad();
		if (this.IsDead())
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x00076810 File Offset: 0x00074A10
	public float GetSteerInput()
	{
		float num = 0f;
		BufferList<global::ModularCar.DriverSeatInputs> values = this.driverSeatInputs.Values;
		for (int i = 0; i < values.Count; i++)
		{
			num += values[i].steerInput;
		}
		return Mathf.Clamp(num, -1f, 1f);
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x00076860 File Offset: 0x00074A60
	public bool GetSteerModInput()
	{
		BufferList<global::ModularCar.DriverSeatInputs> values = this.driverSeatInputs.Values;
		for (int i = 0; i < values.Count; i++)
		{
			if (values[i].steerMod)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x0007689C File Offset: 0x00074A9C
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		float speed = base.GetSpeed();
		this.carPhysics.FixedUpdate(UnityEngine.Time.fixedDeltaTime, speed);
		this.engineController.CheckEngineState();
		this.hurtTriggerFront.gameObject.SetActive(speed > this.hurtTriggerMinSpeed);
		this.hurtTriggerRear.gameObject.SetActive(speed < -this.hurtTriggerMinSpeed);
		this.serverTerrainHandler.FixedUpdate();
		float num = Mathf.Abs(speed);
		if (this.lastPosWasBad || num > 15f)
		{
			if (GamePhysics.CheckOBB(new OBB(this.mainChassisCollider.transform, this.collisionCheckBounds), 1218511105, QueryTriggerInteraction.Ignore))
			{
				this.rigidBody.position = this.lastGoodPos;
				this.rigidBody.rotation = this.lastGoodRot;
				base.transform.position = this.lastGoodPos;
				base.transform.rotation = this.lastGoodRot;
				this.rigidBody.velocity = Vector3.zero;
				this.rigidBody.angularVelocity = Vector3.zero;
				this.lastPosWasBad = true;
			}
			else
			{
				this.lastGoodPos = this.rigidBody.position;
				this.lastGoodRot = this.rigidBody.rotation;
				this.lastPosWasBad = false;
			}
		}
		else
		{
			this.lastGoodPos = this.rigidBody.position;
			this.lastGoodRot = this.rigidBody.rotation;
			this.lastPosWasBad = false;
		}
		if (base.IsMoving())
		{
			Vector3 commultiplier = this.GetCOMMultiplier();
			if (commultiplier != this.prevCOMMultiplier)
			{
				this.rigidBody.centerOfMass = Vector3.Scale(this.realLocalCOM, commultiplier);
				this.prevCOMMultiplier = commultiplier;
			}
		}
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x00076A4C File Offset: 0x00074C4C
	protected override bool DetermineIfStationary()
	{
		bool result = this.rigidBody.position == this.prevPosition && this.rigidBody.rotation == this.prevRotation;
		this.prevPosition = this.rigidBody.position;
		this.prevRotation = this.rigidBody.rotation;
		return result;
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x00076AAC File Offset: 0x00074CAC
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		global::BaseVehicle.MountPointInfo playerSeatInfo = base.GetPlayerSeatInfo(player);
		if (playerSeatInfo == null || !playerSeatInfo.isDriver)
		{
			return;
		}
		if (!this.driverSeatInputs.Contains(playerSeatInfo.mountable))
		{
			this.driverSeatInputs.Add(playerSeatInfo.mountable, new global::ModularCar.DriverSeatInputs());
		}
		global::ModularCar.DriverSeatInputs driverSeatInputs = this.driverSeatInputs[playerSeatInfo.mountable];
		if (inputState.IsDown(BUTTON.DUCK))
		{
			driverSeatInputs.steerInput += inputState.MouseDelta().x * 0.1f;
		}
		else
		{
			driverSeatInputs.steerInput = 0f;
			if (inputState.IsDown(BUTTON.LEFT))
			{
				driverSeatInputs.steerInput = -1f;
			}
			else if (inputState.IsDown(BUTTON.RIGHT))
			{
				driverSeatInputs.steerInput = 1f;
			}
		}
		driverSeatInputs.steerMod = inputState.IsDown(BUTTON.SPRINT);
		float num = 0f;
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			num = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			num = -1f;
		}
		driverSeatInputs.throttleInput = 0f;
		driverSeatInputs.brakeInput = 0f;
		if (base.GetSpeed() > 3f && num < -0.1f)
		{
			driverSeatInputs.throttleInput = 0f;
			driverSeatInputs.brakeInput = -num;
		}
		else
		{
			driverSeatInputs.throttleInput = num;
			driverSeatInputs.brakeInput = 0f;
		}
		for (int i = 0; i < base.NumAttachedModules; i++)
		{
			base.AttachedModuleEntities[i].PlayerServerInput(inputState, player);
		}
		if (this.engineController.IsOff && ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD))))
		{
			this.engineController.TryStartEngine(player);
		}
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x00076C54 File Offset: 0x00074E54
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		global::ModularCar.DriverSeatInputs driverSeatInputs;
		if (this.driverSeatInputs.TryGetValue(seat, out driverSeatInputs))
		{
			this.driverSeatInputs.Remove(seat);
		}
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			if (baseVehicleModule != null)
			{
				baseVehicleModule.OnPlayerDismountedVehicle(player);
			}
		}
		this.CarLock.CheckEnableCentralLocking();
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x00076CE0 File Offset: 0x00074EE0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.modularCar = Facepunch.Pool.Get<ProtoBuf.ModularCar>();
		info.msg.modularCar.steerAngle = this.SteerAngle;
		info.msg.modularCar.driveWheelVel = this.DriveWheelVelocity;
		info.msg.modularCar.throttleInput = this.GetThrottleInput();
		info.msg.modularCar.brakeInput = this.GetBrakeInput();
		info.msg.modularCar.fuelStorageID = this.GetFuelSystem().fuelStorageInstance.uid;
		info.msg.modularCar.fuelFraction = this.GetFuelFraction();
		this.CarLock.Save(info);
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x00076DA0 File Offset: 0x00074FA0
	public override void Hurt(HitInfo info)
	{
		if (!this.IsDead() && info.damageTypes.Get(DamageType.Decay) == 0f)
		{
			this.PropagateDamageToModules(info, 0.5f / (float)base.NumAttachedModules, 0.9f / (float)base.NumAttachedModules, null);
		}
		base.Hurt(info);
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x00076DF2 File Offset: 0x00074FF2
	public void TickFuel(float fuelUsedPerSecond)
	{
		this.engineController.TickFuel(fuelUsedPerSecond);
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x00076E04 File Offset: 0x00075004
	public override bool MountEligable(global::BasePlayer player)
	{
		if (!base.MountEligable(player))
		{
			return false;
		}
		ModularCarSeat modularCarSeat = base.GetIdealMountPointFor(player) as ModularCarSeat;
		return (modularCarSeat != null && !modularCarSeat.associatedSeatingModule.DoorsAreLockable) || this.PlayerCanUseThis(player, ModularCarCodeLock.LockType.Door);
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x00076E49 File Offset: 0x00075049
	public override bool IsComplete()
	{
		return this.HasAnyEngines() && base.HasDriverMountPoints() && !this.IsDead();
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x00076E68 File Offset: 0x00075068
	public void DoDecayDamage(float damage)
	{
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			if (!baseVehicleModule.IsDestroyed)
			{
				baseVehicleModule.Hurt(damage, DamageType.Decay, null, true);
			}
		}
		if (!base.HasAnyModules)
		{
			base.Hurt(damage, DamageType.Decay, null, true);
		}
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x00076EDC File Offset: 0x000750DC
	public float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].GetAdjustedDriveForce(absSpeed, topSpeed);
		}
		return this.RollOffDriveForce(num);
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x00076F24 File Offset: 0x00075124
	public bool HasAnyEngines()
	{
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			if (base.AttachedModuleEntities[i].HasAnEngine)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x00076F5D File Offset: 0x0007515D
	public bool HasAnyWorkingEngines()
	{
		return this.GetMaxDriveForce() > 0f;
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x00076F6C File Offset: 0x0007516C
	public override bool MeetsEngineRequirements()
	{
		return this.HasAnyWorkingEngines() && base.HasDriver();
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x00076F80 File Offset: 0x00075180
	public override void OnEngineStartFailed()
	{
		bool arg = !this.HasAnyWorkingEngines() || this.engineController.IsWaterlogged();
		base.ClientRPC<bool>(null, "EngineStartFailed", arg);
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x00076FB1 File Offset: 0x000751B1
	public CarWheel[] GetWheels()
	{
		if (this.wheels == null)
		{
			this.wheels = new CarWheel[]
			{
				this.wheelFL,
				this.wheelFR,
				this.wheelRL,
				this.wheelRR
			};
		}
		return this.wheels;
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x00076FF4 File Offset: 0x000751F4
	public float GetWheelsMidPos()
	{
		return (this.wheels[0].wheelCollider.transform.localPosition.z - this.wheels[2].wheelCollider.transform.localPosition.z) * 0.5f;
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x00077040 File Offset: 0x00075240
	public override bool AdminFixUp(int tier)
	{
		if (!base.AdminFixUp(tier))
		{
			return false;
		}
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			baseVehicleModule.AdminFixUp(tier);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x000770A8 File Offset: 0x000752A8
	public override void ModuleHurt(BaseVehicleModule hurtModule, HitInfo info)
	{
		if (this.IsDead())
		{
			if (this.timeSinceDeath > 1f)
			{
				for (int i = 0; i < info.damageTypes.types.Length; i++)
				{
					this.deathDamageCounter += info.damageTypes.types[i];
				}
			}
			if (this.deathDamageCounter > 600f && !base.IsDestroyed)
			{
				base.Kill(global::BaseNetworkable.DestroyMode.Gib);
				return;
			}
		}
		else if (hurtModule.PropagateDamage && info.damageTypes.Get(DamageType.Decay) == 0f)
		{
			this.PropagateDamageToModules(info, 0.15f, 0.4f, hurtModule);
		}
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x0007714C File Offset: 0x0007534C
	private void PropagateDamageToModules(HitInfo info, float minPropagationPercent, float maxPropagationPercent, BaseVehicleModule ignoreModule)
	{
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			if (!(baseVehicleModule == ignoreModule) && baseVehicleModule.Health() > 0f)
			{
				if (this.IsDead())
				{
					break;
				}
				float num = UnityEngine.Random.Range(minPropagationPercent, maxPropagationPercent);
				for (int i = 0; i < info.damageTypes.types.Length; i++)
				{
					float num2 = info.damageTypes.types[i];
					if (num2 > 0f)
					{
						baseVehicleModule.AcceptPropagatedDamage(num2 * num, (DamageType)i, info.Initiator, info.UseProtection);
					}
					if (this.IsDead())
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x00077218 File Offset: 0x00075418
	public override void ModuleReachedZeroHealth()
	{
		if (this.IsDead())
		{
			return;
		}
		bool flag = true;
		using (List<BaseVehicleModule>.Enumerator enumerator = base.AttachedModuleEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.health > 0f)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			this.Die(null);
		}
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x00077288 File Offset: 0x00075488
	public override void OnKilled(HitInfo info)
	{
		this.DismountAllPlayers();
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			baseVehicleModule.repair.enabled = false;
		}
		if (this.CarLock != null)
		{
			this.CarLock.RemoveLock();
		}
		this.timeSinceDeath = 0f;
		if (!vehicle.carwrecks)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.Gib);
			return;
		}
		if (!base.HasAnyModules)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.Gib);
			return;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x00077330 File Offset: 0x00075530
	public void RemoveLock()
	{
		this.CarLock.RemoveLock();
	}

	// Token: 0x06000E28 RID: 3624 RVA: 0x00077340 File Offset: 0x00075540
	public void RestoreVelocity(Vector3 vel)
	{
		if (this.rigidBody.velocity.sqrMagnitude < vel.sqrMagnitude)
		{
			vel.y = this.rigidBody.velocity.y;
			this.rigidBody.velocity = vel;
		}
	}

	// Token: 0x06000E29 RID: 3625 RVA: 0x0007738C File Offset: 0x0007558C
	protected override Vector3 GetCOMMultiplier()
	{
		if (this.carPhysics == null || !this.carPhysics.IsGrounded() || !base.IsOn())
		{
			return this.airbourneCOMMultiplier;
		}
		return this.groundedCOMMultiplier;
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x000773BC File Offset: 0x000755BC
	private void UpdateClients()
	{
		if (base.HasDriver())
		{
			int num = (int)((byte)((this.GetThrottleInput() + 1f) * 7f));
			byte b = (byte)(this.GetBrakeInput() * 15f);
			byte arg = (byte)(num + ((int)b << 4));
			byte arg2 = (byte)(this.GetFuelFraction() * 255f);
			base.ClientRPC<float, byte, float, byte>(null, "ModularCarUpdate", this.SteerAngle, arg, this.DriveWheelVelocity, arg2);
		}
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x00077420 File Offset: 0x00075620
	private void DecayTick()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (base.IsOn() || this.immuneToDecay)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastEngineOnTime + 600f)
		{
			return;
		}
		float num = 1f;
		if (this.IsDead())
		{
			int num2 = Mathf.Max(1, base.AttachedModuleEntities.Count);
			num /= 5f * (float)num2;
			this.DoDecayDamage(600f * num);
			return;
		}
		num /= global::ModularCar.outsidedecayminutes;
		if (!this.IsOutside())
		{
			num *= 0.1f;
		}
		float num3;
		if (!base.HasAnyModules)
		{
			num3 = this.MaxHealth();
		}
		else
		{
			num3 = base.AttachedModuleEntities.Max((BaseVehicleModule module) => module.MaxHealth());
		}
		this.DoDecayDamage(num3 * num);
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x000774F4 File Offset: 0x000756F4
	protected override void DoCollisionDamage(global::BaseEntity hitEntity, float damage)
	{
		if (hitEntity == null)
		{
			return;
		}
		BaseVehicleModule baseVehicleModule;
		if ((baseVehicleModule = (hitEntity as BaseVehicleModule)) != null)
		{
			baseVehicleModule.Hurt(damage, DamageType.Collision, this, false);
			return;
		}
		if (hitEntity == this)
		{
			if (!base.HasAnyModules)
			{
				base.Hurt(damage, DamageType.Collision, this, false);
				return;
			}
			float amount = damage / (float)base.NumAttachedModules;
			foreach (BaseVehicleModule baseVehicleModule2 in base.AttachedModuleEntities)
			{
				baseVehicleModule2.AcceptPropagatedDamage(amount, DamageType.Collision, this, false);
			}
		}
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x00077590 File Offset: 0x00075790
	private void SpawnPreassignedModules()
	{
		if (!this.spawnSettings.useSpawnSettings)
		{
			return;
		}
		if (this.spawnSettings.configurationOptions.IsNullOrEmpty<ModularCarPresetConfig>())
		{
			return;
		}
		ModularCarPresetConfig modularCarPresetConfig = this.spawnSettings.configurationOptions[UnityEngine.Random.Range(0, this.spawnSettings.configurationOptions.Length)];
		for (int i = 0; i < modularCarPresetConfig.socketItemDefs.Length; i++)
		{
			ItemModVehicleModule itemModVehicleModule = modularCarPresetConfig.socketItemDefs[i];
			if (itemModVehicleModule != null && base.Inventory.SocketsAreFree(i, itemModVehicleModule.socketsTaken, null))
			{
				itemModVehicleModule.doNonUserSpawn = true;
				global::Item item = ItemManager.Create(itemModVehicleModule.GetComponent<ItemDefinition>(), 1, 0UL);
				float num = UnityEngine.Random.Range(this.spawnSettings.minStartHealthPercent, this.spawnSettings.maxStartHealthPercent);
				item.condition = item.maxCondition * num;
				if (!base.TryAddModule(item))
				{
					item.Remove(0f);
				}
			}
		}
		base.Invoke(new Action(this.HandleAdminBonus), 0f);
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x0007768C File Offset: 0x0007588C
	private void HandleAdminBonus()
	{
		switch (this.spawnSettings.adminBonus)
		{
		case global::ModularCar.SpawnSettings.AdminBonus.T1PlusFuel:
			this.AdminFixUp(1);
			return;
		case global::ModularCar.SpawnSettings.AdminBonus.T2PlusFuel:
			this.AdminFixUp(2);
			return;
		case global::ModularCar.SpawnSettings.AdminBonus.T3PlusFuel:
			this.AdminFixUp(3);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x000776D4 File Offset: 0x000758D4
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		this.GetFuelSystem().LootFuel(player);
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x00077708 File Offset: 0x00075908
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuelWithKeycode(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string codeEntered = msg.read.String(256);
		if (!this.CarLock.TryOpenWithCode(player, codeEntered))
		{
			base.ClientRPC(null, "CodeEntryFailed");
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		this.GetFuelSystem().LootFuel(player);
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x0007776C File Offset: 0x0007596C
	[global::BaseEntity.RPC_Server]
	public void RPC_TryMountWithKeycode(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string codeEntered = msg.read.String(256);
		if (this.CarLock.TryOpenWithCode(player, codeEntered))
		{
			base.WantsMount(player);
			return;
		}
		base.ClientRPC(null, "CodeEntryFailed");
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x000777C0 File Offset: 0x000759C0
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			VehicleModuleSeating vehicleModuleSeating;
			if (baseVehicleModule.HasSeating && (vehicleModuleSeating = (baseVehicleModule as VehicleModuleSeating)) != null && vehicleModuleSeating.IsOnThisModule(player))
			{
				baseVehicleModule.ScaleDamageForPlayer(player, info);
			}
		}
	}

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000E33 RID: 3635 RVA: 0x00077838 File Offset: 0x00075A38
	public override float DriveWheelVelocity
	{
		get
		{
			if (base.isServer)
			{
				return this.carPhysics.DriveWheelVelocity;
			}
			return 0f;
		}
	}

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000E34 RID: 3636 RVA: 0x00077853 File Offset: 0x00075A53
	public float DriveWheelSlip
	{
		get
		{
			if (base.isServer)
			{
				return this.carPhysics.DriveWheelSlip;
			}
			return 0f;
		}
	}

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000E35 RID: 3637 RVA: 0x0007786E File Offset: 0x00075A6E
	public float SteerAngle
	{
		get
		{
			if (base.isServer)
			{
				return this.carPhysics.SteerAngle;
			}
			return 0f;
		}
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000E36 RID: 3638 RVA: 0x0004D717 File Offset: 0x0004B917
	public ItemDefinition AssociatedItemDef
	{
		get
		{
			return this.repair.itemTarget;
		}
	}

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000E37 RID: 3639 RVA: 0x00077889 File Offset: 0x00075A89
	public float MaxSteerAngle
	{
		get
		{
			return this.carSettings.maxSteerAngle;
		}
	}

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000E38 RID: 3640 RVA: 0x00077896 File Offset: 0x00075A96
	public override bool IsLockable
	{
		get
		{
			return this.CarLock.HasALock;
		}
	}

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000E39 RID: 3641 RVA: 0x000778A3 File Offset: 0x00075AA3
	// (set) Token: 0x06000E3A RID: 3642 RVA: 0x000778AB File Offset: 0x00075AAB
	public ModularCarCodeLock CarLock { get; private set; }

	// Token: 0x06000E3B RID: 3643 RVA: 0x000778B4 File Offset: 0x00075AB4
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		this.damageShowingRenderers = base.GetComponentsInChildren<MeshRenderer>();
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x000778D1 File Offset: 0x00075AD1
	public override void InitShared()
	{
		base.InitShared();
		if (this.CarLock == null)
		{
			this.CarLock = new ModularCarCodeLock(this, base.isServer);
		}
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x000778F3 File Offset: 0x00075AF3
	public override float MaxHealth()
	{
		return this.AssociatedItemDef.condition.max;
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x000778F3 File Offset: 0x00075AF3
	public override float StartHealth()
	{
		return this.AssociatedItemDef.condition.max;
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x00077908 File Offset: 0x00075B08
	public float TotalHealth()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].Health();
		}
		return num;
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x00077948 File Offset: 0x00075B48
	public float TotalMaxHealth()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].MaxHealth();
		}
		return num;
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x00077988 File Offset: 0x00075B88
	public override float GetMaxForwardSpeed()
	{
		float num = this.GetMaxDriveForce() / base.TotalMass * 30f;
		return Mathf.Pow(0.9945f, num) * num;
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x000779B8 File Offset: 0x00075BB8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.modularCar != null)
		{
			this.engineController.FuelSystem.fuelStorageInstance.uid = info.msg.modularCar.fuelStorageID;
			this.cachedFuelFraction = info.msg.modularCar.fuelFraction;
			bool hasALock = this.CarLock.HasALock;
			this.CarLock.Load(info);
			if (this.CarLock.HasALock != hasALock)
			{
				for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
				{
					base.AttachedModuleEntities[i].RefreshConditionals(true);
				}
			}
		}
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x00077A65 File Offset: 0x00075C65
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old == next)
		{
			return;
		}
		this.RefreshEngineState();
	}

	// Token: 0x06000E44 RID: 3652 RVA: 0x00077A7C File Offset: 0x00075C7C
	public override float GetThrottleInput()
	{
		if (base.isServer)
		{
			float num = 0f;
			BufferList<global::ModularCar.DriverSeatInputs> values = this.driverSeatInputs.Values;
			for (int i = 0; i < values.Count; i++)
			{
				num += values[i].throttleInput;
			}
			return Mathf.Clamp(num, -1f, 1f);
		}
		return 0f;
	}

	// Token: 0x06000E45 RID: 3653 RVA: 0x00077ADC File Offset: 0x00075CDC
	public override float GetBrakeInput()
	{
		if (base.isServer)
		{
			float num = 0f;
			BufferList<global::ModularCar.DriverSeatInputs> values = this.driverSeatInputs.Values;
			for (int i = 0; i < values.Count; i++)
			{
				num += values[i].brakeInput;
			}
			return Mathf.Clamp01(num);
		}
		return 0f;
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x00077B30 File Offset: 0x00075D30
	public float GetMaxDriveForce()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].GetMaxDriveForce();
		}
		return this.RollOffDriveForce(num);
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x00077B74 File Offset: 0x00075D74
	public float GetFuelFraction()
	{
		if (base.isServer)
		{
			return this.engineController.FuelSystem.GetFuelFraction();
		}
		return this.cachedFuelFraction;
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x00077B95 File Offset: 0x00075D95
	public bool PlayerHasUnlockPermission(global::BasePlayer player)
	{
		return this.CarLock.HasLockPermission(player);
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x00077BA3 File Offset: 0x00075DA3
	public bool KeycodeEntryBlocked(global::BasePlayer player)
	{
		return this.CarLock.CodeEntryBlocked(player);
	}

	// Token: 0x06000E4A RID: 3658 RVA: 0x00077BB1 File Offset: 0x00075DB1
	public override bool PlayerCanUseThis(global::BasePlayer player, ModularCarCodeLock.LockType lockType)
	{
		return this.CarLock.PlayerCanUseThis(player, lockType);
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x00077BC0 File Offset: 0x00075DC0
	public bool PlayerCanDestroyLock(global::BasePlayer player, BaseVehicleModule viaModule)
	{
		return this.CarLock.PlayerCanDestroyLock(viaModule);
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x00077BCE File Offset: 0x00075DCE
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return !(player == null) && (this.PlayerIsMounted(player) || (this.PlayerCanUseThis(player, ModularCarCodeLock.LockType.General) && !base.IsOn()));
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x00077BFB File Offset: 0x00075DFB
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && (!pusher.InSafeZone() || this.CarLock.HasLockPermission(pusher));
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x00077C24 File Offset: 0x00075E24
	protected bool RefreshEngineState()
	{
		if (this.lastSetEngineState == base.CurEngineState)
		{
			return false;
		}
		if (base.isServer && base.CurEngineState == VehicleEngineController<GroundVehicle>.EngineState.Off)
		{
			this.lastEngineOnTime = UnityEngine.Time.time;
		}
		foreach (BaseVehicleModule baseVehicleModule in base.AttachedModuleEntities)
		{
			baseVehicleModule.OnEngineStateChanged(this.lastSetEngineState, base.CurEngineState);
		}
		if (base.isServer && GameInfo.HasAchievements && base.NumMounted() >= 5)
		{
			foreach (global::BaseVehicle.MountPointInfo mountPointInfo in base.allMountPoints)
			{
				if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() != null)
				{
					mountPointInfo.mountable.GetMounted().GiveAchievement("BATTLE_BUS");
				}
			}
		}
		this.lastSetEngineState = base.CurEngineState;
		return true;
	}

	// Token: 0x06000E4F RID: 3663 RVA: 0x00077D44 File Offset: 0x00075F44
	private float RollOffDriveForce(float driveForce)
	{
		return Mathf.Pow(0.9999175f, driveForce) * driveForce;
	}

	// Token: 0x06000E50 RID: 3664 RVA: 0x00077D53 File Offset: 0x00075F53
	private void RefreshChassisProtectionState()
	{
		if (base.HasAnyModules)
		{
			this.baseProtection = this.immortalProtection;
			if (base.isServer)
			{
				base.SetHealth(this.MaxHealth());
				return;
			}
		}
		else
		{
			this.baseProtection = this.mortalProtection;
		}
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x00077D8A File Offset: 0x00075F8A
	protected override void ModuleEntityAdded(BaseVehicleModule addedModule)
	{
		base.ModuleEntityAdded(addedModule);
		this.RefreshChassisProtectionState();
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x00077D99 File Offset: 0x00075F99
	protected override void ModuleEntityRemoved(BaseVehicleModule removedModule)
	{
		base.ModuleEntityRemoved(removedModule);
		this.RefreshChassisProtectionState();
	}

	// Token: 0x0400091B RID: 2331
	public static HashSet<global::ModularCar> allCarsList = new HashSet<global::ModularCar>();

	// Token: 0x0400091C RID: 2332
	private readonly ListDictionary<BaseMountable, global::ModularCar.DriverSeatInputs> driverSeatInputs = new ListDictionary<BaseMountable, global::ModularCar.DriverSeatInputs>();

	// Token: 0x0400091D RID: 2333
	private CarPhysics<global::ModularCar> carPhysics;

	// Token: 0x0400091E RID: 2334
	private VehicleTerrainHandler serverTerrainHandler;

	// Token: 0x0400091F RID: 2335
	private CarWheel[] wheels;

	// Token: 0x04000920 RID: 2336
	private float lastEngineOnTime;

	// Token: 0x04000921 RID: 2337
	private const float DECAY_TICK_TIME = 60f;

	// Token: 0x04000922 RID: 2338
	private const float INSIDE_DECAY_MULTIPLIER = 0.1f;

	// Token: 0x04000923 RID: 2339
	private const float CORPSE_DECAY_MINUTES = 5f;

	// Token: 0x04000924 RID: 2340
	private Vector3 prevPosition;

	// Token: 0x04000925 RID: 2341
	private Quaternion prevRotation;

	// Token: 0x04000926 RID: 2342
	private Bounds collisionCheckBounds;

	// Token: 0x04000927 RID: 2343
	private Vector3 lastGoodPos;

	// Token: 0x04000928 RID: 2344
	private Quaternion lastGoodRot;

	// Token: 0x04000929 RID: 2345
	private bool lastPosWasBad;

	// Token: 0x0400092A RID: 2346
	private float deathDamageCounter;

	// Token: 0x0400092B RID: 2347
	private const float DAMAGE_TO_GIB = 600f;

	// Token: 0x0400092C RID: 2348
	private TimeSince timeSinceDeath;

	// Token: 0x0400092D RID: 2349
	private const float IMMUNE_TIME = 1f;

	// Token: 0x0400092E RID: 2350
	protected readonly Vector3 groundedCOMMultiplier = new Vector3(0.25f, 0.3f, 0.25f);

	// Token: 0x0400092F RID: 2351
	protected readonly Vector3 airbourneCOMMultiplier = new Vector3(0.25f, 0.75f, 0.25f);

	// Token: 0x04000930 RID: 2352
	private Vector3 prevCOMMultiplier;

	// Token: 0x04000931 RID: 2353
	[Header("Modular Car")]
	public ModularCarChassisVisuals chassisVisuals;

	// Token: 0x04000932 RID: 2354
	public VisualCarWheel wheelFL;

	// Token: 0x04000933 RID: 2355
	public VisualCarWheel wheelFR;

	// Token: 0x04000934 RID: 2356
	public VisualCarWheel wheelRL;

	// Token: 0x04000935 RID: 2357
	public VisualCarWheel wheelRR;

	// Token: 0x04000936 RID: 2358
	[SerializeField]
	private CarSettings carSettings;

	// Token: 0x04000937 RID: 2359
	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	// Token: 0x04000938 RID: 2360
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	// Token: 0x04000939 RID: 2361
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	// Token: 0x0400093A RID: 2362
	[SerializeField]
	private ProtectionProperties immortalProtection;

	// Token: 0x0400093B RID: 2363
	[SerializeField]
	private ProtectionProperties mortalProtection;

	// Token: 0x0400093C RID: 2364
	[SerializeField]
	private BoxCollider mainChassisCollider;

	// Token: 0x0400093D RID: 2365
	[SerializeField]
	private global::ModularCar.SpawnSettings spawnSettings;

	// Token: 0x0400093E RID: 2366
	[SerializeField]
	[HideInInspector]
	private MeshRenderer[] damageShowingRenderers;

	// Token: 0x0400093F RID: 2367
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 3f;

	// Token: 0x04000940 RID: 2368
	[ServerVar(Help = "How many minutes before a ModularCar loses all its health while outside")]
	public static float outsidedecayminutes = 864f;

	// Token: 0x04000941 RID: 2369
	public const BUTTON RapidSteerButton = BUTTON.SPRINT;

	// Token: 0x04000943 RID: 2371
	private VehicleEngineController<GroundVehicle>.EngineState lastSetEngineState;

	// Token: 0x04000944 RID: 2372
	private float cachedFuelFraction;

	// Token: 0x02000B9B RID: 2971
	private class DriverSeatInputs
	{
		// Token: 0x04003EE8 RID: 16104
		public float steerInput;

		// Token: 0x04003EE9 RID: 16105
		public bool steerMod;

		// Token: 0x04003EEA RID: 16106
		public float brakeInput;

		// Token: 0x04003EEB RID: 16107
		public float throttleInput;
	}

	// Token: 0x02000B9C RID: 2972
	[Serializable]
	public class SpawnSettings
	{
		// Token: 0x04003EEC RID: 16108
		[Tooltip("Must be true to use any of these settings.")]
		public bool useSpawnSettings;

		// Token: 0x04003EED RID: 16109
		[Tooltip("Specify a list of possible module configurations that'll automatically spawn with this vehicle.")]
		public ModularCarPresetConfig[] configurationOptions;

		// Token: 0x04003EEE RID: 16110
		[Tooltip("Min health % at spawn for any modules that spawn with this chassis.")]
		public float minStartHealthPercent = 0.15f;

		// Token: 0x04003EEF RID: 16111
		[Tooltip("Max health  % at spawn for any modules that spawn with this chassis.")]
		public float maxStartHealthPercent = 0.5f;

		// Token: 0x04003EF0 RID: 16112
		public global::ModularCar.SpawnSettings.AdminBonus adminBonus;

		// Token: 0x02000F61 RID: 3937
		public enum AdminBonus
		{
			// Token: 0x04004E0C RID: 19980
			None,
			// Token: 0x04004E0D RID: 19981
			T1PlusFuel,
			// Token: 0x04004E0E RID: 19982
			T2PlusFuel,
			// Token: 0x04004E0F RID: 19983
			T3PlusFuel
		}
	}
}
