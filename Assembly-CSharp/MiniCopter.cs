using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000024 RID: 36
public class MiniCopter : BaseHelicopterVehicle, IEngineControllerUser, IEntity, SamSite.ISamSiteTarget
{
	// Token: 0x060000D0 RID: 208 RVA: 0x000066AE File Offset: 0x000048AE
	public float GetFuelFraction()
	{
		if (base.isServer)
		{
			this.cachedFuelFraction = Mathf.Clamp01((float)this.GetFuelSystem().GetFuelAmount() / this.fuelGaugeMax);
		}
		return this.cachedFuelFraction;
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x000066DC File Offset: 0x000048DC
	public override EntityFuelSystem GetFuelSystem()
	{
		return this.engineController.FuelSystem;
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x000066E9 File Offset: 0x000048E9
	public override int StartingFuelUnits()
	{
		return 100;
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x000066ED File Offset: 0x000048ED
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && this.isSpawned)
		{
			this.GetFuelSystem().CheckNewChild(child);
		}
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00006714 File Offset: 0x00004914
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(6f)]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		global::BasePlayer driver = base.GetDriver();
		if (driver != null && driver != player)
		{
			return;
		}
		if (base.IsSafe() && player != this.creatorEntity)
		{
			return;
		}
		this.engineController.FuelSystem.LootFuel(player);
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x060000D5 RID: 213 RVA: 0x00006774 File Offset: 0x00004974
	public bool IsStartingUp
	{
		get
		{
			return this.engineController != null && this.engineController.IsStarting;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x060000D6 RID: 214 RVA: 0x0000678B File Offset: 0x0000498B
	public VehicleEngineController<MiniCopter>.EngineState CurEngineState
	{
		get
		{
			if (this.engineController == null)
			{
				return VehicleEngineController<MiniCopter>.EngineState.Off;
			}
			return this.engineController.CurEngineState;
		}
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x000067A2 File Offset: 0x000049A2
	public override void InitShared()
	{
		this.engineController = new VehicleEngineController<MiniCopter>(this, base.isServer, 5f, this.fuelStoragePrefab, this.waterSample, global::BaseEntity.Flags.Reserved4);
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x060000D8 RID: 216 RVA: 0x000067CC File Offset: 0x000049CC
	public SamSite.SamTargetType SAMTargetType
	{
		get
		{
			return SamSite.targetTypeVehicle;
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x000067D3 File Offset: 0x000049D3
	public override float GetServiceCeiling()
	{
		return global::HotAirBalloon.serviceCeiling;
	}

	// Token: 0x060000DA RID: 218 RVA: 0x000067DA File Offset: 0x000049DA
	public bool IsValidSAMTarget(bool staticRespawn)
	{
		return staticRespawn || !base.InSafeZone();
	}

	// Token: 0x060000DB RID: 219 RVA: 0x000067EC File Offset: 0x000049EC
	public override void PilotInput(InputState inputState, global::BasePlayer player)
	{
		base.PilotInput(inputState, player);
		if (!base.IsOn() && !this.IsStartingUp && inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD))
		{
			this.engineController.TryStartEngine(player);
		}
		this.currentInputState.groundControl = inputState.IsDown(BUTTON.DUCK);
		if (this.currentInputState.groundControl)
		{
			this.currentInputState.roll = 0f;
			this.currentInputState.throttle = (inputState.IsDown(BUTTON.FORWARD) ? 1f : 0f);
			this.currentInputState.throttle -= (inputState.IsDown(BUTTON.BACKWARD) ? 1f : 0f);
		}
		this.cachedRoll = this.currentInputState.roll;
		this.cachedYaw = this.currentInputState.yaw;
		this.cachedPitch = this.currentInputState.pitch;
	}

	// Token: 0x060000DC RID: 220 RVA: 0x000068D9 File Offset: 0x00004AD9
	public bool Grounded()
	{
		return this.leftWheel.isGrounded && this.rightWheel.isGrounded;
	}

	// Token: 0x060000DD RID: 221 RVA: 0x000068F8 File Offset: 0x00004AF8
	public override void SetDefaultInputState()
	{
		this.currentInputState.Reset();
		this.cachedRoll = 0f;
		this.cachedYaw = 0f;
		this.cachedPitch = 0f;
		if (this.Grounded())
		{
			return;
		}
		if (base.HasDriver())
		{
			float num = Vector3.Dot(Vector3.up, base.transform.right);
			float num2 = Vector3.Dot(Vector3.up, base.transform.forward);
			this.currentInputState.roll = ((num < 0f) ? 1f : 0f);
			this.currentInputState.roll -= ((num > 0f) ? 1f : 0f);
			if (num2 < --0f)
			{
				this.currentInputState.pitch = -1f;
				return;
			}
			if (num2 > 0f)
			{
				this.currentInputState.pitch = 1f;
				return;
			}
		}
		else
		{
			this.currentInputState.throttle = -1f;
		}
	}

	// Token: 0x060000DE RID: 222 RVA: 0x000069F8 File Offset: 0x00004BF8
	private void ApplyForceAtWheels()
	{
		if (this.rigidBody == null)
		{
			return;
		}
		float brakeScale;
		float num;
		float turning;
		if (this.currentInputState.groundControl)
		{
			brakeScale = ((this.currentInputState.throttle == 0f) ? 50f : 0f);
			num = this.currentInputState.throttle;
			turning = this.currentInputState.yaw;
		}
		else
		{
			brakeScale = 20f;
			turning = 0f;
			num = 0f;
		}
		num *= (base.IsOn() ? 1f : 0f);
		if (this.isPushing)
		{
			brakeScale = 0f;
			num = 0.1f;
			turning = 0f;
		}
		this.ApplyWheelForce(this.frontWheel, num, brakeScale, turning);
		this.ApplyWheelForce(this.leftWheel, num, brakeScale, 0f);
		this.ApplyWheelForce(this.rightWheel, num, brakeScale, 0f);
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00006AE4 File Offset: 0x00004CE4
	public void ApplyWheelForce(WheelCollider wheel, float gasScale, float brakeScale, float turning)
	{
		if (wheel.isGrounded)
		{
			float num = gasScale * this.motorForceConstant;
			float num2 = brakeScale * this.brakeForceConstant;
			float num3 = 45f * turning;
			if (!Mathf.Approximately(wheel.motorTorque, num))
			{
				wheel.motorTorque = num;
			}
			if (!Mathf.Approximately(wheel.brakeTorque, num2))
			{
				wheel.brakeTorque = num2;
			}
			if (!Mathf.Approximately(wheel.steerAngle, num3))
			{
				wheel.steerAngle = num3;
			}
		}
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00006B53 File Offset: 0x00004D53
	public override void MovementUpdate()
	{
		if (this.Grounded())
		{
			this.ApplyForceAtWheels();
		}
		if (base.IsOn() && (!this.currentInputState.groundControl || !this.Grounded()))
		{
			base.MovementUpdate();
		}
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00006B88 File Offset: 0x00004D88
	public override void ServerInit()
	{
		base.ServerInit();
		this.lastEngineOnTime = UnityEngine.Time.realtimeSinceStartup;
		this.rigidBody.inertiaTensor = this.rigidBody.inertiaTensor;
		this.preventBuildingObject.SetActive(true);
		base.InvokeRandomized(new Action(this.UpdateNetwork), 0f, 0.2f, 0.05f);
		base.InvokeRandomized(new Action(this.DecayTick), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00006C14 File Offset: 0x00004E14
	public void DecayTick()
	{
		if (base.healthFraction == 0f)
		{
			return;
		}
		if (base.IsOn())
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastEngineOnTime + 600f)
		{
			return;
		}
		float num = 1f / (this.IsOutside() ? MiniCopter.outsidedecayminutes : MiniCopter.insidedecayminutes);
		base.Hurt(this.MaxHealth() * num, DamageType.Decay, this, false);
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00006C79 File Offset: 0x00004E79
	public override bool ShouldApplyHoverForce()
	{
		return base.IsOn();
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00006C79 File Offset: 0x00004E79
	public override bool IsEngineOn()
	{
		return base.IsOn();
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00006C81 File Offset: 0x00004E81
	public bool MeetsEngineRequirements()
	{
		if (this.engineController.IsOff)
		{
			return base.HasDriver();
		}
		return base.HasDriver() || UnityEngine.Time.time <= this.lastPlayerInputTime + 1f;
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnEngineStartFailed()
	{
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00006CB7 File Offset: 0x00004EB7
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && this.CurEngineState == VehicleEngineController<MiniCopter>.EngineState.Off)
		{
			this.lastEngineOnTime = UnityEngine.Time.time;
		}
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00006CDC File Offset: 0x00004EDC
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		this.engineController.CheckEngineState();
		this.engineController.TickFuel(this.fuelPerSec);
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00006D04 File Offset: 0x00004F04
	public void UpdateNetwork()
	{
		global::BaseEntity.Flags flags = this.flags;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this.leftWheel.isGrounded, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, this.rightWheel.isGrounded, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, this.frontWheel.isGrounded, false, false);
		if (base.HasDriver())
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return;
		}
		if (flags != this.flags)
		{
			base.SendNetworkUpdate_Flags();
		}
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00006D7F File Offset: 0x00004F7F
	public void UpdateCOM()
	{
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00006D98 File Offset: 0x00004F98
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.miniCopter = Facepunch.Pool.Get<Minicopter>();
		info.msg.miniCopter.fuelStorageID = this.engineController.FuelSystem.fuelStorageInstance.uid;
		info.msg.miniCopter.fuelFraction = this.GetFuelFraction();
		info.msg.miniCopter.pitch = this.currentInputState.pitch;
		info.msg.miniCopter.roll = this.currentInputState.roll;
		info.msg.miniCopter.yaw = this.currentInputState.yaw;
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00006E48 File Offset: 0x00005048
	public override void DismountAllPlayers()
	{
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					mounted.Hurt(10000f, DamageType.Explosion, this, false);
				}
			}
		}
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00006EC8 File Offset: 0x000050C8
	protected override void DoPushAction(global::BasePlayer player)
	{
		Vector3 a = Vector3Ex.Direction2D(player.transform.position, base.transform.position);
		Vector3 a2 = player.eyes.BodyForward();
		a2.y = 0.25f;
		Vector3 position = base.transform.position + a * 2f;
		float d = this.rigidBody.mass * 2f;
		this.rigidBody.AddForceAtPosition(a2 * d, position, ForceMode.Impulse);
		this.rigidBody.AddForce(Vector3.up * 3f, ForceMode.Impulse);
		this.isPushing = true;
		base.Invoke(new Action(this.DisablePushing), 0.5f);
	}

	// Token: 0x060000EE RID: 238 RVA: 0x00006F84 File Offset: 0x00005184
	private void DisablePushing()
	{
		this.isPushing = false;
	}

	// Token: 0x060000EF RID: 239 RVA: 0x00006F8D File Offset: 0x0000518D
	public float RemapValue(float toUse, float maxRemap)
	{
		return toUse * maxRemap;
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x00006F94 File Offset: 0x00005194
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.miniCopter != null)
		{
			this.engineController.FuelSystem.fuelStorageInstance.uid = info.msg.miniCopter.fuelStorageID;
			this.cachedFuelFraction = info.msg.miniCopter.fuelFraction;
			this.cachedPitch = this.RemapValue(info.msg.miniCopter.pitch, 0.5f);
			this.cachedRoll = this.RemapValue(info.msg.miniCopter.roll, 0.2f);
			this.cachedYaw = this.RemapValue(info.msg.miniCopter.yaw, 0.35f);
		}
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x00007056 File Offset: 0x00005256
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && pusher.IsOnGround() && !pusher.isMounted;
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x000062DD File Offset: 0x000044DD
	public override float InheritedVelocityScale()
	{
		return 1f;
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00007074 File Offset: 0x00005274
	public override bool InheritedVelocityDirection()
	{
		return false;
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x00007078 File Offset: 0x00005278
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MiniCopter.OnRpcMessage", 0))
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
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1851540757U, "RPC_OpenFuel", this, player, 6f))
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
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00007260 File Offset: 0x00005460
	void IEngineControllerUser.Invoke(Action action, float time)
	{
		base.Invoke(action, time);
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x0000726A File Offset: 0x0000546A
	void IEngineControllerUser.CancelInvoke(Action action)
	{
		base.CancelInvoke(action);
	}

	// Token: 0x04000101 RID: 257
	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	// Token: 0x04000102 RID: 258
	public float fuelPerSec = 0.25f;

	// Token: 0x04000103 RID: 259
	public float fuelGaugeMax = 100f;

	// Token: 0x04000104 RID: 260
	private float cachedFuelFraction;

	// Token: 0x04000105 RID: 261
	public Transform waterSample;

	// Token: 0x04000106 RID: 262
	public WheelCollider leftWheel;

	// Token: 0x04000107 RID: 263
	public WheelCollider rightWheel;

	// Token: 0x04000108 RID: 264
	public WheelCollider frontWheel;

	// Token: 0x04000109 RID: 265
	public Transform leftWheelTrans;

	// Token: 0x0400010A RID: 266
	public Transform rightWheelTrans;

	// Token: 0x0400010B RID: 267
	public Transform frontWheelTrans;

	// Token: 0x0400010C RID: 268
	public float cachedrotation_left;

	// Token: 0x0400010D RID: 269
	public float cachedrotation_right;

	// Token: 0x0400010E RID: 270
	public float cachedrotation_front;

	// Token: 0x0400010F RID: 271
	[Header("IK")]
	public Transform joystickPositionLeft;

	// Token: 0x04000110 RID: 272
	public Transform joystickPositionRight;

	// Token: 0x04000111 RID: 273
	public Transform leftFootPosition;

	// Token: 0x04000112 RID: 274
	public Transform rightFootPosition;

	// Token: 0x04000113 RID: 275
	public AnimationCurve bladeEngineCurve;

	// Token: 0x04000114 RID: 276
	public Animator animator;

	// Token: 0x04000115 RID: 277
	public float maxRotorSpeed = 10f;

	// Token: 0x04000116 RID: 278
	public float timeUntilMaxRotorSpeed = 7f;

	// Token: 0x04000117 RID: 279
	public float rotorBlurThreshold = 8f;

	// Token: 0x04000118 RID: 280
	public Transform mainRotorBlur;

	// Token: 0x04000119 RID: 281
	public Transform mainRotorBlades;

	// Token: 0x0400011A RID: 282
	public Transform rearRotorBlades;

	// Token: 0x0400011B RID: 283
	public Transform rearRotorBlur;

	// Token: 0x0400011C RID: 284
	public float motorForceConstant = 150f;

	// Token: 0x0400011D RID: 285
	public float brakeForceConstant = 500f;

	// Token: 0x0400011E RID: 286
	public GameObject preventBuildingObject;

	// Token: 0x0400011F RID: 287
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 0f;

	// Token: 0x04000120 RID: 288
	[ServerVar(Help = "How long before a minicopter loses all its health while outside")]
	public static float outsidedecayminutes = 480f;

	// Token: 0x04000121 RID: 289
	[ServerVar(Help = "How long before a minicopter loses all its health while indoors")]
	public static float insidedecayminutes = 2880f;

	// Token: 0x04000122 RID: 290
	private VehicleEngineController<MiniCopter> engineController;

	// Token: 0x04000123 RID: 291
	private bool isPushing;

	// Token: 0x04000124 RID: 292
	private float lastEngineOnTime;

	// Token: 0x04000125 RID: 293
	private float cachedPitch;

	// Token: 0x04000126 RID: 294
	private float cachedYaw;

	// Token: 0x04000127 RID: 295
	private float cachedRoll;
}
