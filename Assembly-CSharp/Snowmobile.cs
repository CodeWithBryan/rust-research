using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CB RID: 203
public class Snowmobile : GroundVehicle, CarPhysics<global::Snowmobile>.ICar, TriggerHurtNotChild.IHurtTriggerUser, VehicleChassisVisuals<global::Snowmobile>.IClientWheelUser, IPrefabPreProcess
{
	// Token: 0x060011CE RID: 4558 RVA: 0x0008F90C File Offset: 0x0008DB0C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Snowmobile.OnRpcMessage", 0))
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
			if (rpc == 924237371U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenItemStorage ");
				}
				using (TimeWarning.New("RPC_OpenItemStorage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(924237371U, "RPC_OpenItemStorage", this, player, 3f))
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
							this.RPC_OpenItemStorage(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_OpenItemStorage");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000179 RID: 377
	// (get) Token: 0x060011CF RID: 4559 RVA: 0x0008FBC0 File Offset: 0x0008DDC0
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

	// Token: 0x060011D0 RID: 4560 RVA: 0x0008FBD8 File Offset: 0x0008DDD8
	public override void ServerInit()
	{
		base.ServerInit();
		this.timeSinceLastUsed = 0f;
		this.rigidBody.centerOfMass = this.centreOfMassTransform.localPosition;
		this.rigidBody.inertiaTensor = new Vector3(450f, 200f, 200f);
		this.carPhysics = new CarPhysics<global::Snowmobile>(this, base.transform, this.rigidBody, this.carSettings);
		this.serverTerrainHandler = new VehicleTerrainHandler(this);
		base.InvokeRandomized(new Action(this.UpdateClients), 0f, 0.15f, 0.02f);
		base.InvokeRandomized(new Action(this.SnowmobileDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x0008FCA8 File Offset: 0x0008DEA8
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		float speed = base.GetSpeed();
		this.carPhysics.FixedUpdate(UnityEngine.Time.fixedDeltaTime, speed);
		this.serverTerrainHandler.FixedUpdate();
		if (base.IsOn())
		{
			float fuelPerSecond = Mathf.Lerp(this.idleFuelPerSec, this.maxFuelPerSec, Mathf.Abs(this.ThrottleInput));
			this.engineController.TickFuel(fuelPerSecond);
		}
		this.engineController.CheckEngineState();
		RaycastHit raycastHit;
		if (!this.carPhysics.IsGrounded() && UnityEngine.Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 10f, 1218511105, QueryTriggerInteraction.Ignore))
		{
			Vector3 vector = raycastHit.normal;
			Vector3 right = base.transform.right;
			right.y = 0f;
			vector = Vector3.ProjectOnPlane(vector, right);
			float num = Vector3.Angle(vector, Vector3.up);
			float angle = this.rigidBody.angularVelocity.magnitude * 57.29578f * this.airControlStability / this.airControlPower;
			if (num <= 45f)
			{
				Vector3 torque = Vector3.Cross(Quaternion.AngleAxis(angle, this.rigidBody.angularVelocity) * base.transform.up, vector) * this.airControlPower * this.airControlPower;
				this.rigidBody.AddTorque(torque);
			}
		}
		this.hurtTriggerFront.gameObject.SetActive(speed > this.hurtTriggerMinSpeed);
		this.hurtTriggerRear.gameObject.SetActive(speed < -this.hurtTriggerMinSpeed);
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x0008FE38 File Offset: 0x0008E038
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		this.timeSinceLastUsed = 0f;
		if (inputState.IsDown(BUTTON.DUCK))
		{
			this.SteerInput += inputState.MouseDelta().x * 0.1f;
		}
		else
		{
			this.SteerInput = 0f;
			if (inputState.IsDown(BUTTON.LEFT))
			{
				this.SteerInput = -1f;
			}
			else if (inputState.IsDown(BUTTON.RIGHT))
			{
				this.SteerInput = 1f;
			}
		}
		float num = 0f;
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			num = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			num = -1f;
		}
		this.ThrottleInput = 0f;
		this.BrakeInput = 0f;
		if (base.GetSpeed() > 3f && num < -0.1f)
		{
			this.ThrottleInput = 0f;
			this.BrakeInput = -num;
		}
		else
		{
			this.ThrottleInput = num;
			this.BrakeInput = 0f;
		}
		if (this.engineController.IsOff && ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD))))
		{
			this.engineController.TryStartEngine(player);
		}
	}

	// Token: 0x060011D3 RID: 4563 RVA: 0x0008FF74 File Offset: 0x0008E174
	public float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float maxDriveForce = this.GetMaxDriveForce();
		float bias = Mathf.Lerp(0.3f, 0.75f, this.GetPerformanceFraction());
		float num = MathEx.BiasedLerp(1f - absSpeed / topSpeed, bias);
		return maxDriveForce * num;
	}

	// Token: 0x060011D4 RID: 4564 RVA: 0x0008FFB0 File Offset: 0x0008E1B0
	public override float GetModifiedDrag()
	{
		float num = base.GetModifiedDrag();
		if (!global::Snowmobile.allTerrain)
		{
			VehicleTerrainHandler.Surface onSurface = this.serverTerrainHandler.OnSurface;
			if (this.serverTerrainHandler.IsGrounded && onSurface != VehicleTerrainHandler.Surface.Frictionless && onSurface != VehicleTerrainHandler.Surface.Sand && onSurface != VehicleTerrainHandler.Surface.Snow && onSurface != VehicleTerrainHandler.Surface.Ice)
			{
				float num2 = Mathf.Max(num, this.badTerrainDrag);
				if (num2 <= this.prevTerrainModDrag)
				{
					num = this.prevTerrainModDrag;
				}
				else
				{
					num = Mathf.MoveTowards(this.prevTerrainModDrag, num2, 0.33f * this.timeSinceTerrainModCheck);
				}
				this.prevTerrainModDrag = num;
			}
			else
			{
				this.prevTerrainModDrag = 0f;
			}
		}
		this.timeSinceTerrainModCheck = 0f;
		this.InSlowMode = (num >= this.badTerrainDrag);
		return num;
	}

	// Token: 0x060011D5 RID: 4565 RVA: 0x0006FAA5 File Offset: 0x0006DCA5
	public override float MaxVelocity()
	{
		return Mathf.Max(this.GetMaxForwardSpeed() * 1.3f, 30f);
	}

	// Token: 0x060011D6 RID: 4566 RVA: 0x00090068 File Offset: 0x0008E268
	public CarWheel[] GetWheels()
	{
		if (this.wheels == null)
		{
			this.wheels = new CarWheel[]
			{
				this.wheelSkiFL,
				this.wheelSkiFR,
				this.wheelTreadFL,
				this.wheelTreadFR,
				this.wheelTreadRL,
				this.wheelTreadRR
			};
		}
		return this.wheels;
	}

	// Token: 0x060011D7 RID: 4567 RVA: 0x000900C5 File Offset: 0x0008E2C5
	public float GetWheelsMidPos()
	{
		return (this.wheelSkiFL.wheelCollider.transform.localPosition.z - this.wheelTreadRL.wheelCollider.transform.localPosition.z) * 0.5f;
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x00090104 File Offset: 0x0008E304
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.snowmobile = Facepunch.Pool.Get<ProtoBuf.Snowmobile>();
		info.msg.snowmobile.steerInput = this.SteerInput;
		info.msg.snowmobile.driveWheelVel = this.DriveWheelVelocity;
		info.msg.snowmobile.throttleInput = this.ThrottleInput;
		info.msg.snowmobile.brakeInput = this.BrakeInput;
		info.msg.snowmobile.storageID = this.itemStorageInstance.uid;
		info.msg.snowmobile.fuelStorageID = this.GetFuelSystem().fuelStorageInstance.uid;
	}

	// Token: 0x060011D9 RID: 4569 RVA: 0x00007074 File Offset: 0x00005274
	public override int StartingFuelUnits()
	{
		return 0;
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x000901BC File Offset: 0x0008E3BC
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && this.isSpawned && child.prefabID == this.itemStoragePrefab.GetEntity().prefabID)
		{
			this.itemStorageInstance.Set((StorageContainer)child);
		}
	}

	// Token: 0x060011DB RID: 4571 RVA: 0x0009020C File Offset: 0x0008E40C
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			StorageContainer storageContainer = this.itemStorageInstance.Get(base.isServer);
			if (storageContainer != null && storageContainer.IsValid())
			{
				storageContainer.DropItems(null);
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x060011DC RID: 4572 RVA: 0x0006FAC5 File Offset: 0x0006DCC5
	public override bool MeetsEngineRequirements()
	{
		return base.HasDriver();
	}

	// Token: 0x060011DD RID: 4573 RVA: 0x00090250 File Offset: 0x0008E450
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (global::Snowmobile.allowPassengerOnly)
		{
			base.AttemptMount(player, doMountChecks);
			return;
		}
		if (!this.MountEligable(player))
		{
			return;
		}
		BaseMountable baseMountable;
		if (!base.HasDriver())
		{
			baseMountable = this.mountPoints[0].mountable;
		}
		else
		{
			baseMountable = base.GetIdealMountPointFor(player);
		}
		if (baseMountable != null)
		{
			baseMountable.AttemptMount(player, doMountChecks);
		}
		if (this.PlayerIsMounted(player))
		{
			this.PlayerMounted(player, baseMountable);
		}
	}

	// Token: 0x060011DE RID: 4574 RVA: 0x000902C0 File Offset: 0x0008E4C0
	public void SnowmobileDecay()
	{
		if (this.IsDead())
		{
			return;
		}
		if (this.timeSinceLastUsed < 2700f)
		{
			return;
		}
		float num = this.IsOutside() ? global::Snowmobile.outsideDecayMinutes : float.PositiveInfinity;
		if (!float.IsPositiveInfinity(num))
		{
			float num2 = 1f / num;
			base.Hurt(this.MaxHealth() * num2, DamageType.Decay, this, false);
		}
	}

	// Token: 0x060011DF RID: 4575 RVA: 0x00090320 File Offset: 0x0008E520
	public StorageContainer GetItemContainer()
	{
		global::BaseEntity baseEntity = this.itemStorageInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x060011E0 RID: 4576 RVA: 0x00090358 File Offset: 0x0008E558
	private void UpdateClients()
	{
		if (base.HasDriver())
		{
			int num = (int)((byte)((this.ThrottleInput + 1f) * 7f));
			byte b = (byte)(this.BrakeInput * 15f);
			byte arg = (byte)(num + ((int)b << 4));
			base.ClientRPC<float, byte, float, float>(null, "SnowmobileUpdate", this.SteerInput, arg, this.DriveWheelVelocity, this.GetFuelFraction());
		}
	}

	// Token: 0x060011E1 RID: 4577 RVA: 0x0004ACB8 File Offset: 0x00048EB8
	public override void OnEngineStartFailed()
	{
		base.ClientRPC(null, "EngineStartFailed");
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x000903B3 File Offset: 0x0008E5B3
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		this.riderProtection.Scale(info.damageTypes, 1f);
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x000903D4 File Offset: 0x0008E5D4
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeLooted(player))
		{
			return;
		}
		if (!base.IsDriver(player))
		{
			return;
		}
		this.GetFuelSystem().LootFuel(player);
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x00090408 File Offset: 0x0008E608
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_OpenItemStorage(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeLooted(player))
		{
			return;
		}
		StorageContainer itemContainer = this.GetItemContainer();
		if (itemContainer != null)
		{
			itemContainer.PlayerOpenLoot(player, "", true);
		}
	}

	// Token: 0x1700017A RID: 378
	// (get) Token: 0x060011E5 RID: 4581 RVA: 0x00090444 File Offset: 0x0008E644
	// (set) Token: 0x060011E6 RID: 4582 RVA: 0x0009045F File Offset: 0x0008E65F
	public float ThrottleInput
	{
		get
		{
			if (!this.engineController.IsOn)
			{
				return 0f;
			}
			return this._throttle;
		}
		protected set
		{
			this._throttle = Mathf.Clamp(value, -1f, 1f);
		}
	}

	// Token: 0x1700017B RID: 379
	// (get) Token: 0x060011E7 RID: 4583 RVA: 0x00090477 File Offset: 0x0008E677
	// (set) Token: 0x060011E8 RID: 4584 RVA: 0x0009047F File Offset: 0x0008E67F
	public float BrakeInput
	{
		get
		{
			return this._brake;
		}
		protected set
		{
			this._brake = Mathf.Clamp(value, 0f, 1f);
		}
	}

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x060011E9 RID: 4585 RVA: 0x00090497 File Offset: 0x0008E697
	public bool IsBraking
	{
		get
		{
			return this.BrakeInput > 0f;
		}
	}

	// Token: 0x1700017D RID: 381
	// (get) Token: 0x060011EA RID: 4586 RVA: 0x000904A6 File Offset: 0x0008E6A6
	// (set) Token: 0x060011EB RID: 4587 RVA: 0x000904AE File Offset: 0x0008E6AE
	public float SteerInput
	{
		get
		{
			return this._steer;
		}
		protected set
		{
			this._steer = Mathf.Clamp(value, -1f, 1f);
		}
	}

	// Token: 0x1700017E RID: 382
	// (get) Token: 0x060011EC RID: 4588 RVA: 0x000904C6 File Offset: 0x0008E6C6
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

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x060011ED RID: 4589 RVA: 0x000904E1 File Offset: 0x0008E6E1
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

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x060011EE RID: 4590 RVA: 0x000904FC File Offset: 0x0008E6FC
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

	// Token: 0x17000181 RID: 385
	// (get) Token: 0x060011EF RID: 4591 RVA: 0x00090517 File Offset: 0x0008E717
	public float MaxSteerAngle
	{
		get
		{
			return this.carSettings.maxSteerAngle;
		}
	}

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x060011F0 RID: 4592 RVA: 0x000028C8 File Offset: 0x00000AC8
	// (set) Token: 0x060011F1 RID: 4593 RVA: 0x00090524 File Offset: 0x0008E724
	public bool InSlowMode
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved8);
		}
		private set
		{
			if (this.InSlowMode != value)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved8, value, false, true);
			}
		}
	}

	// Token: 0x17000183 RID: 387
	// (get) Token: 0x060011F2 RID: 4594 RVA: 0x0009053D File Offset: 0x0008E73D
	private float Mass
	{
		get
		{
			if (base.isServer)
			{
				return this.rigidBody.mass;
			}
			return this._mass;
		}
	}

	// Token: 0x060011F3 RID: 4595 RVA: 0x0009055C File Offset: 0x0008E75C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.snowmobile == null)
		{
			return;
		}
		this.itemStorageInstance.uid = info.msg.snowmobile.storageID;
		this.engineController.FuelSystem.fuelStorageInstance.uid = info.msg.snowmobile.fuelStorageID;
		this.cachedFuelFraction = info.msg.snowmobile.fuelFraction;
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x000905D4 File Offset: 0x0008E7D4
	public float GetMaxDriveForce()
	{
		return (float)this.engineKW * 10f * this.GetPerformanceFraction();
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x000905EA File Offset: 0x0008E7EA
	public override float GetMaxForwardSpeed()
	{
		return this.GetMaxDriveForce() / this.Mass * 15f;
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x000905FF File Offset: 0x0008E7FF
	public override float GetThrottleInput()
	{
		return this.ThrottleInput;
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x00090607 File Offset: 0x0008E807
	public override float GetBrakeInput()
	{
		return this.BrakeInput;
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x0009060F File Offset: 0x0008E80F
	public float GetSteerInput()
	{
		return this.SteerInput;
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x00007074 File Offset: 0x00005274
	public bool GetSteerModInput()
	{
		return false;
	}

	// Token: 0x060011FA RID: 4602 RVA: 0x00090618 File Offset: 0x0008E818
	public float GetPerformanceFraction()
	{
		float t = Mathf.InverseLerp(0.25f, 0.5f, base.healthFraction);
		return Mathf.Lerp(0.5f, 1f, t);
	}

	// Token: 0x060011FB RID: 4603 RVA: 0x0009064B File Offset: 0x0008E84B
	public float GetFuelFraction()
	{
		if (base.isServer)
		{
			return this.engineController.FuelSystem.GetFuelFraction();
		}
		return this.cachedFuelFraction;
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x000704A6 File Offset: 0x0006E6A6
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.CanBeLooted(player) && (this.PlayerIsMounted(player) || !base.IsOn());
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x0009066C File Offset: 0x0008E86C
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && GameInfo.HasAchievements && !old.HasFlag(global::BaseEntity.Flags.On) && next.HasFlag(global::BaseEntity.Flags.On))
		{
			global::BasePlayer driver = base.GetDriver();
			if (driver != null && driver.FindTrigger<TriggerSnowmobileAchievement>() != null)
			{
				driver.GiveAchievement("DRIVE_SNOWMOBILE");
			}
		}
	}

	// Token: 0x04000B19 RID: 2841
	private CarPhysics<global::Snowmobile> carPhysics;

	// Token: 0x04000B1A RID: 2842
	private VehicleTerrainHandler serverTerrainHandler;

	// Token: 0x04000B1B RID: 2843
	private CarWheel[] wheels;

	// Token: 0x04000B1C RID: 2844
	private TimeSince timeSinceLastUsed;

	// Token: 0x04000B1D RID: 2845
	private const float DECAY_TICK_TIME = 60f;

	// Token: 0x04000B1E RID: 2846
	private float prevTerrainModDrag;

	// Token: 0x04000B1F RID: 2847
	private TimeSince timeSinceTerrainModCheck;

	// Token: 0x04000B20 RID: 2848
	[Header("Snowmobile")]
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x04000B21 RID: 2849
	[SerializeField]
	private GameObjectRef itemStoragePrefab;

	// Token: 0x04000B22 RID: 2850
	[SerializeField]
	private VisualCarWheel wheelSkiFL;

	// Token: 0x04000B23 RID: 2851
	[SerializeField]
	private VisualCarWheel wheelSkiFR;

	// Token: 0x04000B24 RID: 2852
	[SerializeField]
	private VisualCarWheel wheelTreadFL;

	// Token: 0x04000B25 RID: 2853
	[SerializeField]
	private VisualCarWheel wheelTreadFR;

	// Token: 0x04000B26 RID: 2854
	[SerializeField]
	private VisualCarWheel wheelTreadRL;

	// Token: 0x04000B27 RID: 2855
	[SerializeField]
	private VisualCarWheel wheelTreadRR;

	// Token: 0x04000B28 RID: 2856
	[SerializeField]
	private CarSettings carSettings;

	// Token: 0x04000B29 RID: 2857
	[SerializeField]
	private int engineKW = 59;

	// Token: 0x04000B2A RID: 2858
	[SerializeField]
	private float idleFuelPerSec = 0.03f;

	// Token: 0x04000B2B RID: 2859
	[SerializeField]
	private float maxFuelPerSec = 0.15f;

	// Token: 0x04000B2C RID: 2860
	[SerializeField]
	private float airControlStability = 10f;

	// Token: 0x04000B2D RID: 2861
	[SerializeField]
	private float airControlPower = 40f;

	// Token: 0x04000B2E RID: 2862
	[SerializeField]
	private float badTerrainDrag = 1f;

	// Token: 0x04000B2F RID: 2863
	[SerializeField]
	private ProtectionProperties riderProtection;

	// Token: 0x04000B30 RID: 2864
	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	// Token: 0x04000B31 RID: 2865
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	// Token: 0x04000B32 RID: 2866
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	// Token: 0x04000B33 RID: 2867
	[Header("Snowmobile Visuals")]
	public float minGroundFXSpeed;

	// Token: 0x04000B34 RID: 2868
	[SerializeField]
	private SnowmobileChassisVisuals chassisVisuals;

	// Token: 0x04000B35 RID: 2869
	[SerializeField]
	private VehicleLight[] lights;

	// Token: 0x04000B36 RID: 2870
	[SerializeField]
	private Transform steeringLeftIK;

	// Token: 0x04000B37 RID: 2871
	[SerializeField]
	private Transform steeringRightIK;

	// Token: 0x04000B38 RID: 2872
	[SerializeField]
	private Transform leftFootIK;

	// Token: 0x04000B39 RID: 2873
	[SerializeField]
	private Transform rightFootIK;

	// Token: 0x04000B3A RID: 2874
	[SerializeField]
	private Transform starterKey;

	// Token: 0x04000B3B RID: 2875
	[SerializeField]
	private Vector3 engineOffKeyRot;

	// Token: 0x04000B3C RID: 2876
	[SerializeField]
	private Vector3 engineOnKeyRot;

	// Token: 0x04000B3D RID: 2877
	[ServerVar(Help = "How long before a snowmobile loses all its health while outside")]
	public static float outsideDecayMinutes = 1440f;

	// Token: 0x04000B3E RID: 2878
	[ServerVar(Help = "Allow mounting as a passenger when there's no driver")]
	public static bool allowPassengerOnly = false;

	// Token: 0x04000B3F RID: 2879
	[ServerVar(Help = "If true, snowmobile goes fast on all terrain types")]
	public static bool allTerrain = false;

	// Token: 0x04000B40 RID: 2880
	private float _throttle;

	// Token: 0x04000B41 RID: 2881
	private float _brake;

	// Token: 0x04000B42 RID: 2882
	private float _steer;

	// Token: 0x04000B43 RID: 2883
	private float _mass = -1f;

	// Token: 0x04000B44 RID: 2884
	public const global::BaseEntity.Flags Flag_Slowmode = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000B45 RID: 2885
	private EntityRef<StorageContainer> itemStorageInstance;

	// Token: 0x04000B46 RID: 2886
	private float cachedFuelFraction;

	// Token: 0x04000B47 RID: 2887
	private const float FORCE_MULTIPLIER = 10f;
}
