using System;
using System.Collections.Generic;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E2 RID: 226
public class VehicleModuleSeating : BaseVehicleModule, IPrefabPreProcess
{
	// Token: 0x060013AB RID: 5035 RVA: 0x0009B318 File Offset: 0x00099518
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VehicleModuleSeating.OnRpcMessage", 0))
		{
			if (rpc == 2791546333U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DestroyLock ");
				}
				using (TimeWarning.New("RPC_DestroyLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2791546333U, "RPC_DestroyLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_DestroyLock(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_DestroyLock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x060013AC RID: 5036 RVA: 0x0009B480 File Offset: 0x00099680
	public override bool HasSeating
	{
		get
		{
			return this.mountPoints.Count > 0;
		}
	}

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x060013AD RID: 5037 RVA: 0x0009B490 File Offset: 0x00099690
	// (set) Token: 0x060013AE RID: 5038 RVA: 0x0009B498 File Offset: 0x00099698
	public ModularCar Car { get; private set; }

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x060013AF RID: 5039 RVA: 0x0009B4A1 File Offset: 0x000996A1
	protected bool IsOnACar
	{
		get
		{
			return this.Car != null;
		}
	}

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x060013B0 RID: 5040 RVA: 0x0009B4AF File Offset: 0x000996AF
	protected bool IsOnAVehicleLockUser
	{
		get
		{
			return this.VehicleLockUser != null;
		}
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x060013B1 RID: 5041 RVA: 0x0009B4BA File Offset: 0x000996BA
	public bool DoorsAreLockable
	{
		get
		{
			return this.seating.doorsAreLockable;
		}
	}

	// Token: 0x060013B2 RID: 5042 RVA: 0x0009B4C8 File Offset: 0x000996C8
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (this.seating.steeringWheel != null)
		{
			this.steerAngle = this.seating.steeringWheel.localEulerAngles;
		}
		if (this.seating.accelPedal != null)
		{
			this.accelAngle = this.seating.accelPedal.localEulerAngles;
		}
		if (this.seating.brakePedal != null)
		{
			this.brakeAngle = this.seating.brakePedal.localEulerAngles;
		}
		if (this.seating.speedometer != null)
		{
			this.speedometerAngle = new Vector3(-160f, 0f, -40f);
		}
		if (this.seating.fuelGauge != null)
		{
			this.fuelAngle = this.seating.fuelGauge.localEulerAngles;
		}
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x0009B5B8 File Offset: 0x000997B8
	public virtual bool IsOnThisModule(BasePlayer player)
	{
		BaseMountable mounted = player.GetMounted();
		return mounted != null && mounted.GetParentEntity() as VehicleModuleSeating == this;
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x0009B5E8 File Offset: 0x000997E8
	public bool HasADriverSeat()
	{
		using (List<BaseVehicle.MountPointInfo>.Enumerator enumerator = this.mountPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isDriver)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060013B5 RID: 5045 RVA: 0x0009B644 File Offset: 0x00099844
	public override void ModuleAdded(BaseModularVehicle vehicle, int firstSocketIndex)
	{
		base.ModuleAdded(vehicle, firstSocketIndex);
		this.Car = (vehicle as ModularCar);
		this.VehicleLockUser = (vehicle as IVehicleLockUser);
		if (this.HasSeating && base.isServer)
		{
			using (List<BaseVehicle.MountPointInfo>.Enumerator enumerator = this.mountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ModularCarSeat modularCarSeat;
					if ((modularCarSeat = (enumerator.Current.mountable as ModularCarSeat)) != null)
					{
						modularCarSeat.associatedSeatingModule = this;
					}
				}
			}
		}
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x0009B6D4 File Offset: 0x000998D4
	public override void ModuleRemoved()
	{
		base.ModuleRemoved();
		this.Car = null;
		this.VehicleLockUser = null;
	}

	// Token: 0x060013B7 RID: 5047 RVA: 0x0009B6EC File Offset: 0x000998EC
	public bool PlayerCanDestroyLock(BasePlayer player)
	{
		return this.IsOnAVehicleLockUser && !(player == null) && !base.Vehicle.IsDead() && this.HasADriverSeat() && this.VehicleLockUser.PlayerCanDestroyLock(player, this) && (!player.isMounted || !this.VehicleLockUser.PlayerHasUnlockPermission(player));
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x0009B74F File Offset: 0x0009994F
	protected BaseVehicleSeat GetSeatAtIndex(int index)
	{
		return this.mountPoints[index].mountable as BaseVehicleSeat;
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x0009B767 File Offset: 0x00099967
	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		if (this.passengerProtection != null)
		{
			this.passengerProtection.Scale(info.damageTypes, 1f);
		}
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x0009B798 File Offset: 0x00099998
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (this.hornLoop != null && this.IsOnThisModule(player))
		{
			bool flag = inputState.IsDown(BUTTON.FIRE_PRIMARY);
			if (flag != base.HasFlag(BaseEntity.Flags.Reserved8))
			{
				base.SetFlag(BaseEntity.Flags.Reserved8, flag, false, true);
			}
			if (flag)
			{
				this.hornPlayer = player;
			}
		}
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x0009B7F6 File Offset: 0x000999F6
	public override void OnPlayerDismountedVehicle(BasePlayer player)
	{
		base.OnPlayerDismountedVehicle(player);
		if (base.HasFlag(BaseEntity.Flags.Reserved8) && player == this.hornPlayer)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		}
	}

	// Token: 0x060013BC RID: 5052 RVA: 0x0009B828 File Offset: 0x00099A28
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_DestroyLock(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (this.IsOnAVehicleLockUser)
		{
			if (!this.PlayerCanDestroyLock(player))
			{
				return;
			}
			this.VehicleLockUser.RemoveLock();
		}
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x0009B859 File Offset: 0x00099A59
	protected virtual Vector3 ModifySeatPositionLocalSpace(int index, Vector3 desiredPos)
	{
		return desiredPos;
	}

	// Token: 0x060013BE RID: 5054 RVA: 0x0009B85C File Offset: 0x00099A5C
	public override void OnEngineStateChanged(VehicleEngineController<GroundVehicle>.EngineState oldState, VehicleEngineController<GroundVehicle>.EngineState newState)
	{
		base.OnEngineStateChanged(oldState, newState);
		if (!GameInfo.HasAchievements || base.isClient || newState != VehicleEngineController<GroundVehicle>.EngineState.On || this.mountPoints == null)
		{
			return;
		}
		bool flag = true;
		using (List<BaseVehicleModule>.Enumerator enumerator = this.Car.AttachedModuleEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				VehicleModuleEngine vehicleModuleEngine;
				if ((vehicleModuleEngine = (enumerator.Current as VehicleModuleEngine)) != null && !vehicleModuleEngine.AtPeakPerformance)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
			{
				if (mountPointInfo.mountable.GetMounted() != null)
				{
					mountPointInfo.mountable.GetMounted().GiveAchievement("BUCKLE_UP");
				}
			}
		}
	}

	// Token: 0x04000C63 RID: 3171
	[SerializeField]
	private ProtectionProperties passengerProtection;

	// Token: 0x04000C64 RID: 3172
	[SerializeField]
	private ModularCarCodeLockVisuals codeLockVisuals;

	// Token: 0x04000C65 RID: 3173
	[SerializeField]
	private VehicleModuleSeating.Seating seating;

	// Token: 0x04000C66 RID: 3174
	[SerializeField]
	[HideInInspector]
	private Vector3 steerAngle;

	// Token: 0x04000C67 RID: 3175
	[SerializeField]
	[HideInInspector]
	private Vector3 accelAngle;

	// Token: 0x04000C68 RID: 3176
	[SerializeField]
	[HideInInspector]
	private Vector3 brakeAngle;

	// Token: 0x04000C69 RID: 3177
	[SerializeField]
	[HideInInspector]
	private Vector3 speedometerAngle;

	// Token: 0x04000C6A RID: 3178
	[SerializeField]
	[HideInInspector]
	private Vector3 fuelAngle;

	// Token: 0x04000C6B RID: 3179
	[Header("Horn")]
	[SerializeField]
	private SoundDefinition hornLoop;

	// Token: 0x04000C6C RID: 3180
	[SerializeField]
	private SoundDefinition hornStart;

	// Token: 0x04000C6D RID: 3181
	private const BaseEntity.Flags FLAG_HORN = BaseEntity.Flags.Reserved8;

	// Token: 0x04000C6E RID: 3182
	private float steerPercent;

	// Token: 0x04000C6F RID: 3183
	private float throttlePercent;

	// Token: 0x04000C70 RID: 3184
	private float brakePercent;

	// Token: 0x04000C71 RID: 3185
	private bool? checkEngineLightOn;

	// Token: 0x04000C72 RID: 3186
	private bool? fuelLightOn;

	// Token: 0x04000C74 RID: 3188
	protected IVehicleLockUser VehicleLockUser;

	// Token: 0x04000C75 RID: 3189
	private MaterialPropertyBlock dashboardLightPB;

	// Token: 0x04000C76 RID: 3190
	private static int emissionColorID = Shader.PropertyToID("_EmissionColor");

	// Token: 0x04000C77 RID: 3191
	private BasePlayer hornPlayer;

	// Token: 0x02000BC5 RID: 3013
	[Serializable]
	public class MountHotSpot
	{
		// Token: 0x04003F82 RID: 16258
		public Transform transform;

		// Token: 0x04003F83 RID: 16259
		public Vector2 size;
	}

	// Token: 0x02000BC6 RID: 3014
	[Serializable]
	public class Seating
	{
		// Token: 0x04003F84 RID: 16260
		[Header("Seating & Controls")]
		public bool doorsAreLockable = true;

		// Token: 0x04003F85 RID: 16261
		[Obsolete("Use BaseVehicle.mountPoints instead")]
		[HideInInspector]
		public BaseVehicle.MountPointInfo[] mountPoints;

		// Token: 0x04003F86 RID: 16262
		public Transform steeringWheel;

		// Token: 0x04003F87 RID: 16263
		public Transform accelPedal;

		// Token: 0x04003F88 RID: 16264
		public Transform brakePedal;

		// Token: 0x04003F89 RID: 16265
		public Transform steeringWheelLeftGrip;

		// Token: 0x04003F8A RID: 16266
		public Transform steeringWheelRightGrip;

		// Token: 0x04003F8B RID: 16267
		public Transform accelPedalGrip;

		// Token: 0x04003F8C RID: 16268
		public Transform brakePedalGrip;

		// Token: 0x04003F8D RID: 16269
		public VehicleModuleSeating.MountHotSpot[] mountHotSpots;

		// Token: 0x04003F8E RID: 16270
		[Header("Dashboard")]
		public Transform speedometer;

		// Token: 0x04003F8F RID: 16271
		public Transform fuelGauge;

		// Token: 0x04003F90 RID: 16272
		public Renderer dashboardRenderer;

		// Token: 0x04003F91 RID: 16273
		[Range(0f, 3f)]
		public int checkEngineLightMatIndex = 2;

		// Token: 0x04003F92 RID: 16274
		[ColorUsage(true, true)]
		public Color checkEngineLightEmission;

		// Token: 0x04003F93 RID: 16275
		[Range(0f, 3f)]
		public int fuelLightMatIndex = 3;

		// Token: 0x04003F94 RID: 16276
		[ColorUsage(true, true)]
		public Color fuelLightEmission;
	}
}
