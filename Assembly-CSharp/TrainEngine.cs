using System;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x020000DE RID: 222
public class TrainEngine : TrainCar, IEngineControllerUser, IEntity
{
	// Token: 0x06001349 RID: 4937 RVA: 0x00098EE8 File Offset: 0x000970E8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TrainEngine.OnRpcMessage", 0))
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

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x0600134A RID: 4938 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool networkUpdateOnCompleteTrainChange
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600134B RID: 4939 RVA: 0x0009900C File Offset: 0x0009720C
	public override void ServerInit()
	{
		base.ServerInit();
		this.engineDamage = new EngineDamageOverTime(this.engineDamageToSlow, this.engineDamageTimeframe, new Action(this.OnEngineTookHeavyDamage));
		this.engineLocalOffset = base.transform.InverseTransformPoint(this.engineWorldCol.transform.position + this.engineWorldCol.transform.rotation * this.engineWorldCol.center);
	}

	// Token: 0x0600134C RID: 4940 RVA: 0x000066ED File Offset: 0x000048ED
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && this.isSpawned)
		{
			this.GetFuelSystem().CheckNewChild(child);
		}
	}

	// Token: 0x0600134D RID: 4941 RVA: 0x00099088 File Offset: 0x00097288
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		this.engineController.CheckEngineState();
		if (this.engineController.IsOn)
		{
			float fuelPerSecond = Mathf.Lerp(this.idleFuelPerSec, this.maxFuelPerSec, Mathf.Abs(this.GetThrottleFraction()));
			if (this.engineController.TickFuel(fuelPerSecond) > 0)
			{
				base.ClientRPC<int>(null, "SetFuelAmount", this.GetFuelAmount());
			}
			if (this.completeTrain != null && this.completeTrain.LinedUpToUnload != this.lastSentLinedUpToUnload)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
		}
		else if (this.LightsAreOn && !base.HasDriver())
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, false, false, true);
		}
	}

	// Token: 0x0600134E RID: 4942 RVA: 0x00099134 File Offset: 0x00097334
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.trainEngine = Facepunch.Pool.Get<ProtoBuf.TrainEngine>();
		info.msg.trainEngine.throttleSetting = (int)this.CurThrottleSetting;
		info.msg.trainEngine.fuelStorageID = this.GetFuelSystem().fuelStorageInstance.uid;
		info.msg.trainEngine.fuelAmount = this.GetFuelAmount();
		info.msg.trainEngine.numConnectedCars = this.completeTrain.NumTrainCars;
		info.msg.trainEngine.linedUpToUnload = this.completeTrain.LinedUpToUnload;
		this.lastSentLinedUpToUnload = this.completeTrain.LinedUpToUnload;
	}

	// Token: 0x0600134F RID: 4943 RVA: 0x000991EB File Offset: 0x000973EB
	public override EntityFuelSystem GetFuelSystem()
	{
		return this.engineController.FuelSystem;
	}

	// Token: 0x06001350 RID: 4944 RVA: 0x000991F8 File Offset: 0x000973F8
	public override void LightToggle(global::BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved5, !this.LightsAreOn, false, true);
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x0009921C File Offset: 0x0009741C
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		global::TrainEngine.<>c__DisplayClass19_0 CS$<>8__locals1;
		CS$<>8__locals1.inputState = inputState;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.player = player;
		if (!base.IsDriver(CS$<>8__locals1.player))
		{
			return;
		}
		if (this.engineController.IsOff)
		{
			if ((CS$<>8__locals1.inputState.IsDown(BUTTON.FORWARD) && !CS$<>8__locals1.inputState.WasDown(BUTTON.FORWARD)) || (CS$<>8__locals1.inputState.IsDown(BUTTON.BACKWARD) && !CS$<>8__locals1.inputState.WasDown(BUTTON.BACKWARD)))
			{
				this.engineController.TryStartEngine(CS$<>8__locals1.player);
			}
			base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
		}
		else
		{
			if (!this.<PlayerServerInput>g__ProcessThrottleInput|19_0(BUTTON.FORWARD, new Action(this.IncreaseThrottle), ref CS$<>8__locals1))
			{
				this.<PlayerServerInput>g__ProcessThrottleInput|19_0(BUTTON.BACKWARD, new Action(this.DecreaseThrottle), ref CS$<>8__locals1);
			}
			base.SetFlag(global::BaseEntity.Flags.Reserved8, CS$<>8__locals1.inputState.IsDown(BUTTON.FIRE_PRIMARY), false, true);
		}
		if (CS$<>8__locals1.inputState.IsDown(BUTTON.LEFT))
		{
			this.SetTrackSelection(TrainTrackSpline.TrackSelection.Left);
			return;
		}
		if (CS$<>8__locals1.inputState.IsDown(BUTTON.RIGHT))
		{
			this.SetTrackSelection(TrainTrackSpline.TrackSelection.Right);
			return;
		}
		this.SetTrackSelection(TrainTrackSpline.TrackSelection.Default);
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x00099334 File Offset: 0x00097534
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x0009934C File Offset: 0x0009754C
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		this.driverProtection.Scale(info.damageTypes, 1f);
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x0009936C File Offset: 0x0009756C
	public bool MeetsEngineRequirements()
	{
		return (base.HasDriver() || this.CurThrottleSetting != global::TrainEngine.EngineSpeeds.Zero) && (this.completeTrain.AnyPlayersOnTrain() || vehicle.trainskeeprunning);
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnEngineStartFailed()
	{
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x00099395 File Offset: 0x00097595
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (!this.CanMount(player))
		{
			return;
		}
		base.AttemptMount(player, doMountChecks);
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x000993AC File Offset: 0x000975AC
	protected override float GetThrottleForce()
	{
		if (this.IsDead() || base.IsDestroyed)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = this.engineController.IsOn ? this.GetThrottleFraction() : 0f;
		float num3 = this.maxSpeed * num2;
		float curTopSpeed = this.GetCurTopSpeed();
		num3 = Mathf.Clamp(num3, -curTopSpeed, curTopSpeed);
		float trackSpeed = base.GetTrackSpeed();
		if (num2 > 0f && trackSpeed < num3)
		{
			num += this.GetCurEngineForce();
		}
		else if (num2 < 0f && trackSpeed > num3)
		{
			num -= this.GetCurEngineForce();
		}
		return num;
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x00099441 File Offset: 0x00097641
	public override bool HasThrottleInput()
	{
		return this.engineController.IsOn && this.CurThrottleSetting != global::TrainEngine.EngineSpeeds.Zero;
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x00099460 File Offset: 0x00097660
	public override void Hurt(HitInfo info)
	{
		if (this.engineDamage != null && Vector3.SqrMagnitude(this.engineLocalOffset - info.HitPositionLocal) < 2f)
		{
			this.engineDamage.TakeDamage(info.damageTypes.Total());
		}
		base.Hurt(info);
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x000994AF File Offset: 0x000976AF
	public void StopEngine()
	{
		this.engineController.StopEngine();
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x000994BC File Offset: 0x000976BC
	protected override Vector3 GetExplosionPos()
	{
		return this.engineWorldCol.transform.position + this.engineWorldCol.center;
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x000994DE File Offset: 0x000976DE
	private void IncreaseThrottle()
	{
		if (this.CurThrottleSetting == global::TrainEngine.MaxThrottle)
		{
			return;
		}
		this.SetThrottle(this.CurThrottleSetting + 1);
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x000994FC File Offset: 0x000976FC
	private void DecreaseThrottle()
	{
		if (this.CurThrottleSetting == global::TrainEngine.MinThrottle)
		{
			return;
		}
		this.SetThrottle(this.CurThrottleSetting - 1);
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x0009951A File Offset: 0x0009771A
	private void SetZeroThrottle()
	{
		this.SetThrottle(global::TrainEngine.EngineSpeeds.Zero);
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x00099524 File Offset: 0x00097724
	protected override void ServerFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.ServerFlagsChanged(old, next);
		if (next.HasFlag(global::BaseEntity.Flags.On) && !old.HasFlag(global::BaseEntity.Flags.On))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, true, false, true);
			base.InvokeRandomized(new Action(this.CheckForHazards), 0f, 1f, 0.1f);
			return;
		}
		if (!next.HasFlag(global::BaseEntity.Flags.On) && old.HasFlag(global::BaseEntity.Flags.On))
		{
			base.CancelInvoke(new Action(this.CheckForHazards));
			base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
		}
	}

	// Token: 0x06001360 RID: 4960 RVA: 0x000995D8 File Offset: 0x000977D8
	private void CheckForHazards()
	{
		float trackSpeed = base.GetTrackSpeed();
		if (trackSpeed > 4.5f || trackSpeed < -4.5f)
		{
			float maxHazardDist = Mathf.Lerp(40f, 325f, Mathf.Abs(trackSpeed) * 0.05f);
			base.SetFlag(global::BaseEntity.Flags.Reserved6, base.FrontTrackSection.HasValidHazardWithin(this, base.FrontWheelSplineDist, 20f, maxHazardDist, this.localTrackSelection, trackSpeed, base.RearTrackSection, null), false, true);
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x0009965A File Offset: 0x0009785A
	private void OnEngineTookHeavyDamage()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved10, true, false, true);
		base.Invoke(new Action(this.ResetEngineToNormal), this.engineSlowedTime);
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x0000385E File Offset: 0x00001A5E
	private void ResetEngineToNormal()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved10, false, false, true);
	}

	// Token: 0x06001363 RID: 4963 RVA: 0x00099684 File Offset: 0x00097884
	private float GetCurTopSpeed()
	{
		float num = this.maxSpeed * this.GetEnginePowerMultiplier(0.5f);
		if (this.EngineIsSlowed)
		{
			num = Mathf.Clamp(num, -this.engineSlowedMaxVel, this.engineSlowedMaxVel);
		}
		return num;
	}

	// Token: 0x06001364 RID: 4964 RVA: 0x000996C1 File Offset: 0x000978C1
	private float GetCurEngineForce()
	{
		return this.engineForce * this.GetEnginePowerMultiplier(0.75f);
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x000996D8 File Offset: 0x000978D8
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

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x06001366 RID: 4966 RVA: 0x000035F8 File Offset: 0x000017F8
	public bool LightsAreOn
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved5);
		}
	}

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x06001367 RID: 4967 RVA: 0x000035EB File Offset: 0x000017EB
	public bool CloseToHazard
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x06001368 RID: 4968 RVA: 0x0009970C File Offset: 0x0009790C
	public bool EngineIsSlowed
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved10);
		}
	}

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x06001369 RID: 4969 RVA: 0x00099719 File Offset: 0x00097919
	// (set) Token: 0x0600136A RID: 4970 RVA: 0x00099721 File Offset: 0x00097921
	public global::TrainEngine.EngineSpeeds CurThrottleSetting { get; protected set; } = global::TrainEngine.EngineSpeeds.Zero;

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x0600136B RID: 4971 RVA: 0x00003A54 File Offset: 0x00001C54
	public override TrainCar.TrainCarType CarType
	{
		get
		{
			return TrainCar.TrainCarType.Engine;
		}
	}

	// Token: 0x0600136C RID: 4972 RVA: 0x0009972C File Offset: 0x0009792C
	public override void InitShared()
	{
		base.InitShared();
		this.engineController = new VehicleEngineController<global::TrainEngine>(this, base.isServer, this.engineStartupTime, this.fuelStoragePrefab, null, global::BaseEntity.Flags.Reserved1);
		if (base.isServer)
		{
			bool b = SeedRandom.Range(this.net.ID, 0, 2) == 0;
			base.SetFlag(global::BaseEntity.Flags.Reserved9, b, false, true);
		}
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x00099790 File Offset: 0x00097990
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.trainEngine != null)
		{
			this.engineController.FuelSystem.fuelStorageInstance.uid = info.msg.trainEngine.fuelStorageID;
			this.SetThrottle((global::TrainEngine.EngineSpeeds)info.msg.trainEngine.throttleSetting);
		}
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x000997EC File Offset: 0x000979EC
	public override bool CanBeLooted(global::BasePlayer player)
	{
		if (!base.CanBeLooted(player))
		{
			return false;
		}
		if (player.isMounted)
		{
			return false;
		}
		if (this.lootablesAreOnPlatform)
		{
			return base.PlayerIsOnPlatform(player);
		}
		return base.GetLocalVelocity().magnitude < 2f || base.PlayerIsOnPlatform(player);
	}

	// Token: 0x0600136F RID: 4975 RVA: 0x0009983C File Offset: 0x00097A3C
	private float GetEnginePowerMultiplier(float minPercent)
	{
		if (base.healthFraction > 0.4f)
		{
			return 1f;
		}
		return Mathf.Lerp(minPercent, 1f, base.healthFraction / 0.4f);
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x00099868 File Offset: 0x00097A68
	public float GetThrottleFraction()
	{
		switch (this.CurThrottleSetting)
		{
		case global::TrainEngine.EngineSpeeds.Rev_Hi:
			return -1f;
		case global::TrainEngine.EngineSpeeds.Rev_Med:
			return -0.5f;
		case global::TrainEngine.EngineSpeeds.Rev_Lo:
			return -0.2f;
		case global::TrainEngine.EngineSpeeds.Zero:
			return 0f;
		case global::TrainEngine.EngineSpeeds.Fwd_Lo:
			return 0.2f;
		case global::TrainEngine.EngineSpeeds.Fwd_Med:
			return 0.5f;
		case global::TrainEngine.EngineSpeeds.Fwd_Hi:
			return 1f;
		default:
			return 0f;
		}
	}

	// Token: 0x06001371 RID: 4977 RVA: 0x000998D0 File Offset: 0x00097AD0
	public bool IsNearDesiredSpeed(float leeway)
	{
		float num = Vector3.Dot(base.transform.forward, base.GetLocalVelocity());
		float num2 = this.maxSpeed * this.GetThrottleFraction();
		if (num2 < 0f)
		{
			return num - leeway <= num2;
		}
		return num + leeway >= num2;
	}

	// Token: 0x06001372 RID: 4978 RVA: 0x0009991D File Offset: 0x00097B1D
	protected override void SetTrackSelection(TrainTrackSpline.TrackSelection trackSelection)
	{
		base.SetTrackSelection(trackSelection);
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x00099926 File Offset: 0x00097B26
	private void SetThrottle(global::TrainEngine.EngineSpeeds throttle)
	{
		if (this.CurThrottleSetting == throttle)
		{
			return;
		}
		this.CurThrottleSetting = throttle;
		if (base.isServer)
		{
			base.ClientRPC<sbyte>(null, "SetThrottle", (sbyte)throttle);
		}
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x0009994F File Offset: 0x00097B4F
	private int GetFuelAmount()
	{
		if (base.isServer)
		{
			return this.engineController.FuelSystem.GetFuelAmount();
		}
		return 0;
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x0009996B File Offset: 0x00097B6B
	private bool CanMount(global::BasePlayer player)
	{
		return !this.mustMountFromPlatform || base.PlayerIsOnPlatform(player);
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x00007260 File Offset: 0x00005460
	void IEngineControllerUser.Invoke(Action action, float time)
	{
		base.Invoke(action, time);
	}

	// Token: 0x06001379 RID: 4985 RVA: 0x0000726A File Offset: 0x0000546A
	void IEngineControllerUser.CancelInvoke(Action action)
	{
		base.CancelInvoke(action);
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x00099A1C File Offset: 0x00097C1C
	[CompilerGenerated]
	private bool <PlayerServerInput>g__ProcessThrottleInput|19_0(BUTTON button, Action action, ref global::TrainEngine.<>c__DisplayClass19_0 A_3)
	{
		if (A_3.inputState.IsDown(button))
		{
			if (!A_3.inputState.WasDown(button))
			{
				action();
				this.buttonHoldTime = 0f;
			}
			else
			{
				this.buttonHoldTime += A_3.player.clientTickInterval;
				if (this.buttonHoldTime > 0.55f)
				{
					action();
					this.buttonHoldTime = 0.4f;
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x04000C08 RID: 3080
	public const float HAZARD_CHECK_EVERY = 1f;

	// Token: 0x04000C09 RID: 3081
	public const float HAZARD_DIST_MAX = 325f;

	// Token: 0x04000C0A RID: 3082
	public const float HAZARD_DIST_MIN = 20f;

	// Token: 0x04000C0B RID: 3083
	public const float HAZARD_SPEED_MIN = 4.5f;

	// Token: 0x04000C0C RID: 3084
	private float buttonHoldTime;

	// Token: 0x04000C0D RID: 3085
	private static readonly global::TrainEngine.EngineSpeeds MaxThrottle = global::TrainEngine.EngineSpeeds.Fwd_Hi;

	// Token: 0x04000C0E RID: 3086
	private static readonly global::TrainEngine.EngineSpeeds MinThrottle = global::TrainEngine.EngineSpeeds.Rev_Hi;

	// Token: 0x04000C0F RID: 3087
	private EngineDamageOverTime engineDamage;

	// Token: 0x04000C10 RID: 3088
	private Vector3 engineLocalOffset;

	// Token: 0x04000C11 RID: 3089
	private int lastSentLinedUpToUnload = -1;

	// Token: 0x04000C12 RID: 3090
	[Header("Train Engine")]
	[SerializeField]
	private Transform leftHandLever;

	// Token: 0x04000C13 RID: 3091
	[SerializeField]
	private Transform rightHandLever;

	// Token: 0x04000C14 RID: 3092
	[SerializeField]
	private Transform leftHandGrip;

	// Token: 0x04000C15 RID: 3093
	[SerializeField]
	private Transform rightHandGrip;

	// Token: 0x04000C16 RID: 3094
	[SerializeField]
	private global::TrainEngine.LeverStyle leverStyle;

	// Token: 0x04000C17 RID: 3095
	[SerializeField]
	private Canvas monitorCanvas;

	// Token: 0x04000C18 RID: 3096
	[SerializeField]
	private RustText monitorText;

	// Token: 0x04000C19 RID: 3097
	[SerializeField]
	private LocomotiveExtraVisuals gauges;

	// Token: 0x04000C1A RID: 3098
	[SerializeField]
	private float engineForce = 50000f;

	// Token: 0x04000C1B RID: 3099
	[SerializeField]
	private float maxSpeed = 12f;

	// Token: 0x04000C1C RID: 3100
	[SerializeField]
	private float engineStartupTime = 1f;

	// Token: 0x04000C1D RID: 3101
	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	// Token: 0x04000C1E RID: 3102
	[SerializeField]
	private float idleFuelPerSec = 0.05f;

	// Token: 0x04000C1F RID: 3103
	[SerializeField]
	private float maxFuelPerSec = 0.15f;

	// Token: 0x04000C20 RID: 3104
	[SerializeField]
	private ProtectionProperties driverProtection;

	// Token: 0x04000C21 RID: 3105
	[SerializeField]
	private bool lootablesAreOnPlatform;

	// Token: 0x04000C22 RID: 3106
	[SerializeField]
	private bool mustMountFromPlatform = true;

	// Token: 0x04000C23 RID: 3107
	[SerializeField]
	private VehicleLight[] onLights;

	// Token: 0x04000C24 RID: 3108
	[SerializeField]
	private VehicleLight[] headlights;

	// Token: 0x04000C25 RID: 3109
	[SerializeField]
	private VehicleLight[] notMovingLights;

	// Token: 0x04000C26 RID: 3110
	[SerializeField]
	private VehicleLight[] movingForwardLights;

	// Token: 0x04000C27 RID: 3111
	[FormerlySerializedAs("movingBackwardsLights")]
	[SerializeField]
	private VehicleLight[] movingBackwardLights;

	// Token: 0x04000C28 RID: 3112
	[SerializeField]
	private ParticleSystemContainer fxEngineOn;

	// Token: 0x04000C29 RID: 3113
	[SerializeField]
	private ParticleSystemContainer fxLightDamage;

	// Token: 0x04000C2A RID: 3114
	[SerializeField]
	private ParticleSystemContainer fxMediumDamage;

	// Token: 0x04000C2B RID: 3115
	[SerializeField]
	private ParticleSystemContainer fxHeavyDamage;

	// Token: 0x04000C2C RID: 3116
	[SerializeField]
	private ParticleSystemContainer fxEngineTrouble;

	// Token: 0x04000C2D RID: 3117
	[SerializeField]
	private BoxCollider engineWorldCol;

	// Token: 0x04000C2E RID: 3118
	[SerializeField]
	private float engineDamageToSlow = 150f;

	// Token: 0x04000C2F RID: 3119
	[SerializeField]
	private float engineDamageTimeframe = 10f;

	// Token: 0x04000C30 RID: 3120
	[SerializeField]
	private float engineSlowedTime = 10f;

	// Token: 0x04000C31 RID: 3121
	[SerializeField]
	private float engineSlowedMaxVel = 4f;

	// Token: 0x04000C32 RID: 3122
	[SerializeField]
	private ParticleSystemContainer[] sparks;

	// Token: 0x04000C33 RID: 3123
	[FormerlySerializedAs("brakeSparkLights")]
	[SerializeField]
	private Light[] sparkLights;

	// Token: 0x04000C34 RID: 3124
	[SerializeField]
	private TrainEngineAudio trainAudio;

	// Token: 0x04000C35 RID: 3125
	public const global::BaseEntity.Flags Flag_HazardAhead = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000C36 RID: 3126
	private const global::BaseEntity.Flags Flag_Horn = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000C37 RID: 3127
	public const global::BaseEntity.Flags Flag_AltColor = global::BaseEntity.Flags.Reserved9;

	// Token: 0x04000C38 RID: 3128
	public const global::BaseEntity.Flags Flag_EngineSlowed = global::BaseEntity.Flags.Reserved10;

	// Token: 0x04000C39 RID: 3129
	private VehicleEngineController<global::TrainEngine> engineController;

	// Token: 0x02000BC2 RID: 3010
	private enum LeverStyle
	{
		// Token: 0x04003F75 RID: 16245
		WorkCart,
		// Token: 0x04003F76 RID: 16246
		Locomotive
	}

	// Token: 0x02000BC3 RID: 3011
	public enum EngineSpeeds
	{
		// Token: 0x04003F78 RID: 16248
		Rev_Hi,
		// Token: 0x04003F79 RID: 16249
		Rev_Med,
		// Token: 0x04003F7A RID: 16250
		Rev_Lo,
		// Token: 0x04003F7B RID: 16251
		Zero,
		// Token: 0x04003F7C RID: 16252
		Fwd_Lo,
		// Token: 0x04003F7D RID: 16253
		Fwd_Med,
		// Token: 0x04003F7E RID: 16254
		Fwd_Hi
	}
}
