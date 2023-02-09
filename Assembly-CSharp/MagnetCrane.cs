using System;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000090 RID: 144
public class MagnetCrane : GroundVehicle, CarPhysics<MagnetCrane>.ICar
{
	// Token: 0x06000D3C RID: 3388 RVA: 0x0006F608 File Offset: 0x0006D808
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MagnetCrane.OnRpcMessage", 0))
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
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000D3D RID: 3389 RVA: 0x0006F72C File Offset: 0x0006D92C
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

	// Token: 0x06000D3E RID: 3390 RVA: 0x0006F744 File Offset: 0x0006D944
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.UpdateParams), 0f, 0.1f);
		this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		this.animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
		this.myRigidbody.centerOfMass = this.COM.localPosition;
		this.carPhysics = new CarPhysics<MagnetCrane>(this, base.transform, this.rigidBody, this.carSettings);
		this.serverTerrainHandler = new VehicleTerrainHandler(this);
		this.Magnet.SetMagnetEnabled(false, null);
		this.spawnOrigin = base.transform.position;
		this.lastDrivenTime = UnityEngine.Time.realtimeSinceStartup;
		GameObject[] onTriggers = this.OnTriggers;
		for (int i = 0; i < onTriggers.Length; i++)
		{
			onTriggers[i].SetActive(false);
		}
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x0006F814 File Offset: 0x0006DA14
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (!base.IsDriver(player))
		{
			return;
		}
		this.throttleInput = 0f;
		this.steerInput = 0f;
		this.extensionInput = 0f;
		this.yawInput = 0f;
		this.raiseArmInput = 0f;
		if (this.engineController.IsOff)
		{
			if (inputState.IsAnyDown())
			{
				this.engineController.TryStartEngine(player);
			}
		}
		else if (this.engineController.IsOn)
		{
			bool flag = inputState.IsDown(BUTTON.SPRINT);
			if (inputState.IsDown(BUTTON.RELOAD) && UnityEngine.Time.realtimeSinceStartup > this.nextToggleTime)
			{
				this.Magnet.SetMagnetEnabled(!this.Magnet.IsMagnetOn(), player);
				this.nextToggleTime = UnityEngine.Time.realtimeSinceStartup + 0.5f;
			}
			if (flag)
			{
				float speed = base.GetSpeed();
				float num = 0f;
				if (inputState.IsDown(BUTTON.FORWARD))
				{
					num = 1f;
				}
				else if (inputState.IsDown(BUTTON.BACKWARD))
				{
					num = -1f;
				}
				if (speed > 1f && num < 0f)
				{
					this.throttleInput = 0f;
					this.brakeInput = -num;
				}
				else if (speed < -1f && num > 0f)
				{
					this.throttleInput = 0f;
					this.brakeInput = num;
				}
				else
				{
					this.throttleInput = num;
					this.brakeInput = 0f;
				}
				if (inputState.IsDown(BUTTON.RIGHT))
				{
					this.steerInput = -1f;
				}
				if (inputState.IsDown(BUTTON.LEFT))
				{
					this.steerInput = 1f;
				}
			}
			else
			{
				if (inputState.IsDown(BUTTON.LEFT))
				{
					this.yawInput = 1f;
				}
				else if (inputState.IsDown(BUTTON.RIGHT))
				{
					this.yawInput = -1f;
				}
				else if (inputState.IsDown(BUTTON.DUCK))
				{
					float @float = this.animator.GetFloat(MagnetCrane.yawParam);
					if (@float > 0.01f && @float < 0.99f)
					{
						this.yawInput = ((@float <= 0.5f) ? -1f : 1f);
					}
				}
				if (inputState.IsDown(BUTTON.FORWARD))
				{
					this.raiseArmInput = 1f;
				}
				else if (inputState.IsDown(BUTTON.BACKWARD))
				{
					this.raiseArmInput = -1f;
				}
			}
			if (inputState.IsDown(BUTTON.FIRE_PRIMARY))
			{
				this.extensionInput = 1f;
			}
			if (inputState.IsDown(BUTTON.FIRE_SECONDARY))
			{
				this.extensionInput = -1f;
			}
		}
		this.handbrakeOn = (this.throttleInput == 0f && this.steerInput == 0f);
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x0006FAA5 File Offset: 0x0006DCA5
	public override float MaxVelocity()
	{
		return Mathf.Max(this.GetMaxForwardSpeed() * 1.3f, 30f);
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x0006FABD File Offset: 0x0006DCBD
	public float GetSteerInput()
	{
		return this.steerInput;
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x00007074 File Offset: 0x00005274
	public bool GetSteerModInput()
	{
		return false;
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void OnEngineStartFailed()
	{
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x0006FAC5 File Offset: 0x0006DCC5
	public override bool MeetsEngineRequirements()
	{
		return base.HasDriver();
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x0006FAD0 File Offset: 0x0006DCD0
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		this.rigidBody.ResetInertiaTensor();
		this.rigidBody.inertiaTensor = Vector3.Lerp(this.rigidBody.inertiaTensor, this.customInertiaTensor, 0.5f);
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		float num = Mathf.Clamp(realtimeSinceStartup - this.lastFixedUpdateTime, 0f, 0.5f);
		this.lastFixedUpdateTime = realtimeSinceStartup;
		float speed = base.GetSpeed();
		this.carPhysics.FixedUpdate(UnityEngine.Time.fixedDeltaTime, speed);
		this.serverTerrainHandler.FixedUpdate();
		bool flag = base.IsOn();
		if (base.IsOn())
		{
			float t = Mathf.Max(Mathf.Abs(this.throttleInput), Mathf.Abs(this.steerInput));
			float num2 = Mathf.Lerp(this.idleFuelPerSec, this.maxFuelPerSec, t);
			if (!this.Magnet.HasConnectedObject())
			{
				num2 = Mathf.Min(num2, this.maxFuelPerSec * 0.75f);
			}
			this.engineController.TickFuel(num2);
		}
		this.engineController.CheckEngineState();
		if (base.IsOn() != flag)
		{
			GameObject[] onTriggers = this.OnTriggers;
			for (int i = 0; i < onTriggers.Length; i++)
			{
				onTriggers[i].SetActive(base.IsOn());
			}
		}
		if (Vector3.Dot(base.transform.up, Vector3.down) >= 0.4f)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.Gib);
			return;
		}
		if (realtimeSinceStartup > this.lastDrivenTime + 14400f)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.Gib);
			return;
		}
		if (this.spawnOrigin != Vector3.zero && this.maxDistanceFromOrigin != 0f)
		{
			if (Vector3Ex.Distance2D(base.transform.position, this.spawnOrigin) > this.maxDistanceFromOrigin)
			{
				if (Vector3Ex.Distance2D(base.transform.position, this.lastDamagePos) > 6f)
				{
					if (base.GetDriver() != null)
					{
						base.GetDriver().ShowToast(GameTip.Styles.Red_Normal, MagnetCrane.ReturnMessage, Array.Empty<string>());
					}
					base.Hurt(this.MaxHealth() * 0.15f, DamageType.Generic, this, false);
					this.lastDamagePos = base.transform.position;
					this.nextSelfHealTime = realtimeSinceStartup + 3600f;
					Effect.server.Run(this.selfDamageEffect.resourcePath, base.transform.position + Vector3.up * 2f, Vector3.up, null, false);
					return;
				}
			}
			else if (base.healthFraction < 1f && realtimeSinceStartup > this.nextSelfHealTime && base.SecondsSinceAttacked > 600f)
			{
				this.Heal(1000f);
			}
		}
		if (!base.HasDriver() || !base.IsOn())
		{
			this.handbrakeOn = true;
			this.throttleInput = 0f;
			this.steerInput = 0f;
			base.SetFlag(global::BaseEntity.Flags.Reserved10, false, false, true);
			this.Magnet.SetMagnetEnabled(false, null);
		}
		else
		{
			this.lastDrivenTime = realtimeSinceStartup;
			if (this.Magnet.IsMagnetOn() && this.Magnet.HasConnectedObject() && GamePhysics.CheckOBB(this.Magnet.GetConnectedOBB(0.75f), 1084293121, QueryTriggerInteraction.Ignore))
			{
				this.Magnet.SetMagnetEnabled(false, null);
				this.nextToggleTime = realtimeSinceStartup + 2f;
				Effect.server.Run(this.selfDamageEffect.resourcePath, this.Magnet.transform.position, Vector3.up, null, false);
			}
		}
		this.extensionMove = MagnetCrane.<VehicleFixedUpdate>g__UpdateMoveInput|35_0(this.extensionInput, this.extensionMove, 3f, UnityEngine.Time.fixedDeltaTime);
		this.yawMove = MagnetCrane.<VehicleFixedUpdate>g__UpdateMoveInput|35_0(this.yawInput, this.yawMove, 3f, UnityEngine.Time.fixedDeltaTime);
		this.raiseArmMove = MagnetCrane.<VehicleFixedUpdate>g__UpdateMoveInput|35_0(this.raiseArmInput, this.raiseArmMove, 3f, UnityEngine.Time.fixedDeltaTime);
		bool flag2 = this.extensionInput != 0f || this.raiseArmInput != 0f || this.yawInput != 0f;
		base.SetFlag(global::BaseEntity.Flags.Reserved7, flag2, false, true);
		this.magnetDamage.damageEnabled = (base.IsOn() && flag2);
		this.extensionArmState += this.extensionInput * this.arm1Speed * num;
		this.raiseArmState += this.raiseArmInput * this.arm2Speed * num;
		this.yawState += this.yawInput * this.turnYawSpeed * num;
		this.yawState %= 1f;
		if (this.yawState < 0f)
		{
			this.yawState += 1f;
		}
		this.extensionArmState = Mathf.Clamp(this.extensionArmState, -1f, 1f);
		this.raiseArmState = Mathf.Clamp(this.raiseArmState, -1f, 1f);
		this.UpdateAnimator(UnityEngine.Time.fixedDeltaTime);
		this.Magnet.MagnetThink(UnityEngine.Time.fixedDeltaTime);
		base.SetFlag(global::BaseEntity.Flags.Reserved10, this.throttleInput != 0f || this.steerInput != 0f, false, true);
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x0006FFEC File Offset: 0x0006E1EC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.crane = Facepunch.Pool.Get<Crane>();
		info.msg.crane.arm1 = this.extensionArmState;
		info.msg.crane.arm2 = this.raiseArmState;
		info.msg.crane.yaw = this.yawState;
		info.msg.crane.time = this.GetNetworkTime();
		int num = (int)((byte)((this.carPhysics.TankThrottleLeft + 1f) * 7f));
		byte b = (byte)((this.carPhysics.TankThrottleRight + 1f) * 7f);
		byte treadInput = (byte)(num + ((int)b << 4));
		info.msg.crane.treadInput = (int)treadInput;
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x00007338 File Offset: 0x00005538
	public void UpdateParams()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x000700B0 File Offset: 0x0006E2B0
	public void LateUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.HasDriver() && this.IsColliding())
		{
			this.extensionArmState = this.lastExtensionArmState;
			this.raiseArmState = this.lastRaiseArmState;
			this.yawState = this.lastYawState;
			this.extensionInput = -this.extensionInput;
			this.yawInput = -this.yawInput;
			this.raiseArmInput = -this.raiseArmInput;
			this.UpdateAnimator(UnityEngine.Time.deltaTime);
			return;
		}
		this.lastExtensionArmState = this.extensionArmState;
		this.lastRaiseArmState = this.raiseArmState;
		this.lastYawState = this.yawState;
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x00070154 File Offset: 0x0006E354
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			global::BasePlayer driver = base.GetDriver();
			if (driver != null && info.damageTypes.Has(DamageType.Bullet))
			{
				Capsule capsule = new Capsule(this.driverCollision.transform.position, this.driverCollision.radius, this.driverCollision.height);
				float num = Vector3.Distance(info.PointStart, info.PointEnd);
				Ray ray = new Ray(info.PointStart, Vector3Ex.Direction(info.PointEnd, info.PointStart));
				RaycastHit raycastHit;
				if (capsule.Trace(ray, out raycastHit, 0.05f, num * 1.2f))
				{
					driver.Hurt(info.damageTypes.Total() * 0.15f, DamageType.Bullet, info.Initiator, true);
				}
			}
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00070230 File Offset: 0x0006E430
	public override void OnKilled(HitInfo info)
	{
		if (base.HasDriver())
		{
			base.GetDriver().Hurt(10000f, DamageType.Blunt, info.Initiator, false);
		}
		if (this.explosionEffect.isValid)
		{
			Effect.server.Run(this.explosionEffect.resourcePath, this.explosionPoint.position, Vector3.up, null, false);
		}
		base.OnKilled(info);
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x00070294 File Offset: 0x0006E494
	public bool IsColliding()
	{
		foreach (Transform transform in this.collisionTestingPoints)
		{
			if (transform.gameObject.activeSelf)
			{
				Vector3 position = transform.position;
				Quaternion rotation = transform.rotation;
				if (GamePhysics.CheckOBB(new OBB(position, new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z), rotation), 1084293121, QueryTriggerInteraction.Ignore))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x00070314 File Offset: 0x0006E514
	public float GetMaxDriveForce()
	{
		return (float)this.engineKW * 10f;
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x00070324 File Offset: 0x0006E524
	public float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float num = MathEx.BiasedLerp(1f - absSpeed / topSpeed, 0.5f);
		num = Mathf.Lerp(num, 1f, Mathf.Abs(this.steerInput));
		return this.GetMaxDriveForce() * num;
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00070364 File Offset: 0x0006E564
	public CarWheel[] GetWheels()
	{
		return this.wheels;
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x00026FFC File Offset: 0x000251FC
	public float GetWheelsMidPos()
	{
		return 0f;
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x0007036C File Offset: 0x0006E56C
	public void UpdateAnimator(float dt)
	{
		this.animator.SetFloat("Arm_01", this.extensionArmState);
		this.animator.SetFloat("Arm_02", this.raiseArmState);
		this.animator.SetFloat("Yaw", this.yawState);
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x000703BC File Offset: 0x0006E5BC
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeLooted(player))
		{
			return;
		}
		this.GetFuelSystem().LootFuel(player);
	}

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06000D52 RID: 3410 RVA: 0x000703E6 File Offset: 0x0006E5E6
	public override float DriveWheelVelocity
	{
		get
		{
			return base.GetSpeed();
		}
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x000703EE File Offset: 0x0006E5EE
	public override float GetThrottleInput()
	{
		if (base.isServer)
		{
			return this.throttleInput;
		}
		throw new NotImplementedException("We don't know magnet crane throttle input on the client.");
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00070409 File Offset: 0x0006E609
	public override float GetBrakeInput()
	{
		if (!base.isServer)
		{
			throw new NotImplementedException("We don't know magnet crane brake input on the client.");
		}
		if (this.handbrakeOn)
		{
			return 1f;
		}
		return this.brakeInput;
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x00070434 File Offset: 0x0006E634
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.crane != null && base.isServer)
		{
			this.yawState = info.msg.crane.yaw;
			this.extensionArmState = info.msg.crane.arm1;
			this.raiseArmState = info.msg.crane.arm2;
		}
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x0007049F File Offset: 0x0006E69F
	public override float GetMaxForwardSpeed()
	{
		return 13f;
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x000704A6 File Offset: 0x0006E6A6
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.CanBeLooted(player) && (this.PlayerIsMounted(player) || !base.IsOn());
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x000705B8 File Offset: 0x0006E7B8
	[CompilerGenerated]
	internal static float <VehicleFixedUpdate>g__UpdateMoveInput|35_0(float input, float move, float slowRate, float dt)
	{
		if (input != 0f)
		{
			return input;
		}
		return Mathf.MoveTowards(move, 0f, dt * slowRate);
	}

	// Token: 0x04000863 RID: 2147
	private float steerInput;

	// Token: 0x04000864 RID: 2148
	private float throttleInput;

	// Token: 0x04000865 RID: 2149
	private float brakeInput;

	// Token: 0x04000866 RID: 2150
	private float yawInput;

	// Token: 0x04000867 RID: 2151
	private float extensionInput;

	// Token: 0x04000868 RID: 2152
	private float raiseArmInput;

	// Token: 0x04000869 RID: 2153
	private float extensionMove;

	// Token: 0x0400086A RID: 2154
	private float yawMove;

	// Token: 0x0400086B RID: 2155
	private float raiseArmMove;

	// Token: 0x0400086C RID: 2156
	private float nextToggleTime;

	// Token: 0x0400086D RID: 2157
	private Vector3 spawnOrigin = Vector3.zero;

	// Token: 0x0400086E RID: 2158
	private float lastExtensionArmState;

	// Token: 0x0400086F RID: 2159
	private float lastRaiseArmState;

	// Token: 0x04000870 RID: 2160
	private float lastYawState;

	// Token: 0x04000871 RID: 2161
	private bool handbrakeOn = true;

	// Token: 0x04000872 RID: 2162
	private float nextSelfHealTime;

	// Token: 0x04000873 RID: 2163
	private Vector3 lastDamagePos = Vector3.zero;

	// Token: 0x04000874 RID: 2164
	private float lastDrivenTime;

	// Token: 0x04000875 RID: 2165
	private float lastFixedUpdateTime;

	// Token: 0x04000876 RID: 2166
	private CarPhysics<MagnetCrane> carPhysics;

	// Token: 0x04000877 RID: 2167
	private VehicleTerrainHandler serverTerrainHandler;

	// Token: 0x04000878 RID: 2168
	private Vector3 customInertiaTensor = new Vector3(25000f, 11000f, 19000f);

	// Token: 0x04000879 RID: 2169
	private float extensionArmState;

	// Token: 0x0400087A RID: 2170
	private float raiseArmState;

	// Token: 0x0400087B RID: 2171
	private float yawState = 1f;

	// Token: 0x0400087C RID: 2172
	[Header("Magnet Crane")]
	public Animator animator;

	// Token: 0x0400087D RID: 2173
	[SerializeField]
	private Transform COM;

	// Token: 0x0400087E RID: 2174
	[SerializeField]
	private float arm1Speed = 0.01f;

	// Token: 0x0400087F RID: 2175
	[SerializeField]
	private float arm2Speed = 0.01f;

	// Token: 0x04000880 RID: 2176
	[SerializeField]
	private float turnYawSpeed = 0.01f;

	// Token: 0x04000881 RID: 2177
	[SerializeField]
	private BaseMagnet Magnet;

	// Token: 0x04000882 RID: 2178
	[SerializeField]
	private MagnetCraneAudio mcAudio;

	// Token: 0x04000883 RID: 2179
	[SerializeField]
	private Rigidbody myRigidbody;

	// Token: 0x04000884 RID: 2180
	[SerializeField]
	private Transform[] collisionTestingPoints;

	// Token: 0x04000885 RID: 2181
	[SerializeField]
	private float maxDistanceFromOrigin;

	// Token: 0x04000886 RID: 2182
	[SerializeField]
	private GameObjectRef selfDamageEffect;

	// Token: 0x04000887 RID: 2183
	[SerializeField]
	private GameObjectRef explosionEffect;

	// Token: 0x04000888 RID: 2184
	[SerializeField]
	private Transform explosionPoint;

	// Token: 0x04000889 RID: 2185
	[SerializeField]
	private CapsuleCollider driverCollision;

	// Token: 0x0400088A RID: 2186
	[SerializeField]
	private Transform leftHandTarget;

	// Token: 0x0400088B RID: 2187
	[SerializeField]
	private Transform rightHandTarget;

	// Token: 0x0400088C RID: 2188
	[SerializeField]
	private Transform leftFootTarget;

	// Token: 0x0400088D RID: 2189
	[SerializeField]
	private Transform rightFootTarget;

	// Token: 0x0400088E RID: 2190
	[SerializeField]
	private float idleFuelPerSec;

	// Token: 0x0400088F RID: 2191
	[SerializeField]
	private float maxFuelPerSec;

	// Token: 0x04000890 RID: 2192
	[SerializeField]
	private GameObject[] OnTriggers;

	// Token: 0x04000891 RID: 2193
	[SerializeField]
	private TriggerHurtEx magnetDamage;

	// Token: 0x04000892 RID: 2194
	[SerializeField]
	private int engineKW = 250;

	// Token: 0x04000893 RID: 2195
	[SerializeField]
	private CarWheel[] wheels;

	// Token: 0x04000894 RID: 2196
	[SerializeField]
	private CarSettings carSettings;

	// Token: 0x04000895 RID: 2197
	[SerializeField]
	private ParticleSystem exhaustInner;

	// Token: 0x04000896 RID: 2198
	[SerializeField]
	private ParticleSystem exhaustOuter;

	// Token: 0x04000897 RID: 2199
	[SerializeField]
	private EmissionToggle lightToggle;

	// Token: 0x04000898 RID: 2200
	public static readonly Translate.Phrase ReturnMessage = new Translate.Phrase("junkyardcrane.return", "Return to the Junkyard. Excessive damage will occur.");

	// Token: 0x04000899 RID: 2201
	private const global::BaseEntity.Flags Flag_ArmMovement = global::BaseEntity.Flags.Reserved7;

	// Token: 0x0400089A RID: 2202
	private const global::BaseEntity.Flags Flag_BaseMovementInput = global::BaseEntity.Flags.Reserved10;

	// Token: 0x0400089B RID: 2203
	private static int leftTreadParam = Animator.StringToHash("left tread movement");

	// Token: 0x0400089C RID: 2204
	private static int rightTreadParam = Animator.StringToHash("right tread movement");

	// Token: 0x0400089D RID: 2205
	private static int yawParam = Animator.StringToHash("Yaw");

	// Token: 0x0400089E RID: 2206
	private static int arm1Param = Animator.StringToHash("Arm_01");

	// Token: 0x0400089F RID: 2207
	private static int arm2Param = Animator.StringToHash("Arm_02");
}
