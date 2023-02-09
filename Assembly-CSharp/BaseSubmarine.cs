using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Sonar;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using VLB;

// Token: 0x02000044 RID: 68
public class BaseSubmarine : global::BaseVehicle, IPoolVehicle, IEngineControllerUser, IEntity, IAirSupply
{
	// Token: 0x06000742 RID: 1858 RVA: 0x00049CF8 File Offset: 0x00047EF8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseSubmarine.OnRpcMessage", 0))
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
			if (rpc == 2181221870U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenTorpedoStorage ");
				}
				using (TimeWarning.New("RPC_OpenTorpedoStorage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2181221870U, "RPC_OpenTorpedoStorage", this, player, 3f))
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
							this.RPC_OpenTorpedoStorage(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_OpenTorpedoStorage");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x0004A108 File Offset: 0x00048308
	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.centerOfMass = this.centreOfMassTransform.localPosition;
		this.timeSinceLastUsed = this.timeUntilAutoSurface;
		this.buoyancy.buoyancyScale = 1f;
		this.normalDrag = this.rigidBody.drag;
		this.highDrag = this.normalDrag * 2.5f;
		this.Oxygen = 1f;
		base.InvokeRandomized(new Action(this.UpdateClients), 0f, 0.15f, 0.02f);
		base.InvokeRandomized(new Action(this.SubmarineDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x0004A1CC File Offset: 0x000483CC
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (this.isSpawned)
			{
				this.GetFuelSystem().CheckNewChild(child);
			}
			if (child.prefabID == this.itemStoragePrefab.GetEntity().prefabID)
			{
				this.itemStorageInstance.Set((StorageContainer)child);
			}
			if (child.prefabID == this.torpedoStoragePrefab.GetEntity().prefabID)
			{
				this.torpedoStorageInstance.Set((StorageContainer)child);
			}
		}
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x0004A24E File Offset: 0x0004844E
	private void ServerFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		if (next.HasFlag(global::BaseEntity.Flags.On) && !old.HasFlag(global::BaseEntity.Flags.On))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, true, false, true);
		}
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x0004A284 File Offset: 0x00048484
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

	// Token: 0x06000747 RID: 1863 RVA: 0x0004A2C8 File Offset: 0x000484C8
	protected void OnCollisionEnter(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		this.ProcessCollision(collision);
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x0004A2DA File Offset: 0x000484DA
	public override float MaxVelocity()
	{
		return 10f;
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x0004A2E1 File Offset: 0x000484E1
	public override EntityFuelSystem GetFuelSystem()
	{
		return this.engineController.FuelSystem;
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x0004A2EE File Offset: 0x000484EE
	public override int StartingFuelUnits()
	{
		return 50;
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x0004A2F4 File Offset: 0x000484F4
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (!this.CanMount(player))
		{
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

	// Token: 0x0600074C RID: 1868 RVA: 0x00026D90 File Offset: 0x00024F90
	public void OnPoolDestroyed()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x0004A35B File Offset: 0x0004855B
	public void WakeUp()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
			this.rigidBody.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
		}
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x0004A391 File Offset: 0x00048591
	protected override void OnServerWake()
	{
		if (this.buoyancy != null)
		{
			this.buoyancy.Wake();
		}
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x0004A3AC File Offset: 0x000485AC
	public override void OnKilled(HitInfo info)
	{
		DamageType majorityDamageType = info.damageTypes.GetMajorityDamageType();
		if (majorityDamageType == DamageType.Explosion || majorityDamageType == DamageType.AntiVehicle)
		{
			foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
			{
				if (mountPointInfo.mountable != null)
				{
					global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
					if (mounted != null)
					{
						mounted.Hurt(10000f, DamageType.Explosion, this, false);
					}
				}
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x0004A448 File Offset: 0x00048648
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (!base.IsMovingOrOn)
		{
			this.Velocity = Vector3.zero;
			this.targetClimbSpeed = 0f;
			this.buoyancy.ArtificialHeight = null;
			return;
		}
		this.Velocity = base.GetLocalVelocity();
		this.UpdateWaterInfo();
		if (this.IsSurfaced && !this.wasOnSurface && base.transform.position.y > Env.oceanlevel - 1f)
		{
			this.wasOnSurface = true;
		}
		this.buoyancy.ArtificialHeight = new float?(this.waterSurfaceY);
		this.rigidBody.drag = (base.HasDriver() ? this.normalDrag : this.highDrag);
		float num = 2f;
		if (this.IsSurfaced)
		{
			float num2 = 20f * num;
			if (this.Oxygen < 0.5f)
			{
				this.Oxygen = 0.5f;
			}
			else
			{
				this.Oxygen += UnityEngine.Time.deltaTime / num2;
			}
		}
		else if (this.AnyMounted())
		{
			float num3 = BaseSubmarine.oxygenminutes * 60f * num;
			this.Oxygen -= UnityEngine.Time.deltaTime / num3;
		}
		this.engineController.CheckEngineState();
		if (this.engineController.IsOn)
		{
			float fuelPerSecond = Mathf.Lerp(this.idleFuelPerSec, this.maxFuelPerSec, Mathf.Abs(this.ThrottleInput));
			this.engineController.TickFuel(fuelPerSecond);
		}
		if (this.IsInWater)
		{
			float num4 = this.depthChangeTargetSpeed * this.UpDownInput;
			float num5;
			if ((this.UpDownInput > 0f && num4 > this.targetClimbSpeed && this.targetClimbSpeed > 0f) || (this.UpDownInput < 0f && num4 < this.targetClimbSpeed && this.targetClimbSpeed < 0f))
			{
				num5 = 0.7f;
			}
			else
			{
				num5 = 4f;
			}
			this.targetClimbSpeed = Mathf.MoveTowards(this.targetClimbSpeed, num4, num5 * UnityEngine.Time.fixedDeltaTime);
			float num6 = this.rigidBody.velocity.y - this.targetClimbSpeed;
			float value = this.buoyancy.buoyancyScale - num6 * 50f * UnityEngine.Time.fixedDeltaTime;
			this.buoyancy.buoyancyScale = Mathf.Clamp(value, 0.01f, 1f);
			Vector3 torque = Vector3.Cross(Quaternion.AngleAxis(this.rigidBody.angularVelocity.magnitude * 57.29578f * 10f / 200f, this.rigidBody.angularVelocity) * base.transform.up, Vector3.up) * 200f * 200f;
			this.rigidBody.AddTorque(torque);
			float d = 0.1f;
			this.rigidBody.AddForce(Vector3.up * -num6 * d, ForceMode.VelocityChange);
		}
		else
		{
			float b = 0f;
			this.buoyancy.buoyancyScale = Mathf.Lerp(this.buoyancy.buoyancyScale, b, UnityEngine.Time.fixedDeltaTime);
		}
		if (base.IsOn() && this.IsInWater)
		{
			this.rigidBody.AddForce(base.transform.forward * this.engineKW * 40f * this.ThrottleInput, ForceMode.Force);
			float num7 = this.turnPower * this.rigidBody.mass * this.rigidBody.angularDrag;
			float speed = this.GetSpeed();
			float num8 = Mathf.Min(Mathf.Abs(speed) * 0.6f, 6f) + 4f;
			float num9 = num7 * this.RudderInput * num8;
			if (speed < -1f)
			{
				num9 *= -1f;
			}
			this.rigidBody.AddTorque(base.transform.up * num9, ForceMode.Force);
		}
		this.UpdatePhysicalRudder(this.RudderInput, UnityEngine.Time.fixedDeltaTime);
		if (UnityEngine.Time.time >= this.nextCollisionDamageTime && this.maxDamageThisTick > 0f)
		{
			this.nextCollisionDamageTime = UnityEngine.Time.time + 0.33f;
			base.Hurt(this.maxDamageThisTick, DamageType.Collision, this, false);
			this.maxDamageThisTick = 0f;
		}
		StorageContainer torpedoContainer = this.GetTorpedoContainer();
		if (torpedoContainer != null)
		{
			bool b2 = torpedoContainer.inventory.HasAmmo(AmmoTypes.TORPEDO);
			base.SetFlag(global::BaseEntity.Flags.Reserved6, b2, false, true);
		}
		global::BasePlayer driver = base.GetDriver();
		if (driver != null && this.primaryFireInput)
		{
			bool flag = true;
			if (this.IsInWater && this.timeSinceTorpedoFired >= this.maxFireRate)
			{
				float minSpeed = this.GetSpeed() + 2f;
				ServerProjectile serverProjectile;
				if (BaseMountable.TryFireProjectile(torpedoContainer, AmmoTypes.TORPEDO, this.torpedoFiringPoint.position, this.torpedoFiringPoint.forward, driver, 1f, minSpeed, out serverProjectile))
				{
					this.timeSinceTorpedoFired = 0f;
					flag = false;
					driver.MarkHostileFor(60f);
					base.ClientRPC(null, "TorpedoFired");
				}
			}
			if (!this.prevPrimaryFireInput && flag && this.timeSinceFailRPCSent > 0.5f)
			{
				this.timeSinceFailRPCSent = 0f;
				base.ClientRPCPlayer(null, driver, "TorpedoFireFailed");
			}
		}
		else if (driver == null)
		{
			this.primaryFireInput = false;
		}
		this.prevPrimaryFireInput = this.primaryFireInput;
		if (this.timeSinceLastUsed > 300f && this.LightsAreOn)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, false, false, true);
		}
		for (int i = 0; i < this.parentTriggers.Length; i++)
		{
			float num10 = this.parentTriggers[i].triggerWaterLevel.position.y - base.transform.position.y;
			bool flag2 = this.curSubDepthY - num10 <= 0f;
			if (flag2 != this.parentTriggers[i].trigger.enabled)
			{
				this.parentTriggers[i].trigger.enabled = flag2;
			}
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0004AA5E File Offset: 0x00048C5E
	public override void LightToggle(global::BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved5, !this.LightsAreOn, false, true);
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x0004AA80 File Offset: 0x00048C80
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		this.timeSinceLastUsed = 0f;
		if (!base.IsDriver(player))
		{
			return;
		}
		if (inputState.IsDown(BUTTON.SPRINT))
		{
			this.UpDownInput = 1f;
		}
		else if (inputState.IsDown(BUTTON.DUCK))
		{
			this.UpDownInput = -1f;
		}
		else
		{
			this.UpDownInput = 0f;
		}
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.ThrottleInput = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.ThrottleInput = -1f;
		}
		else
		{
			this.ThrottleInput = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.RudderInput = -1f;
		}
		else if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.RudderInput = 1f;
		}
		else
		{
			this.RudderInput = 0f;
		}
		this.primaryFireInput = inputState.IsDown(BUTTON.FIRE_PRIMARY);
		if (this.engineController.IsOff && ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD)) || (inputState.IsDown(BUTTON.SPRINT) && !inputState.WasDown(BUTTON.SPRINT)) || (inputState.IsDown(BUTTON.DUCK) && !inputState.WasDown(BUTTON.DUCK))))
		{
			this.engineController.TryStartEngine(player);
		}
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0004ABD0 File Offset: 0x00048DD0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.submarine = Facepunch.Pool.Get<Submarine>();
		info.msg.submarine.throttle = this.ThrottleInput;
		info.msg.submarine.upDown = this.UpDownInput;
		info.msg.submarine.rudder = this.RudderInput;
		info.msg.submarine.fuelStorageID = this.GetFuelSystem().fuelStorageInstance.uid;
		info.msg.submarine.fuelAmount = this.GetFuelAmount();
		info.msg.submarine.torpedoStorageID = this.torpedoStorageInstance.uid;
		info.msg.submarine.oxygen = this.Oxygen;
		info.msg.submarine.itemStorageID = this.itemStorageInstance.uid;
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00031AC6 File Offset: 0x0002FCC6
	public bool MeetsEngineRequirements()
	{
		return this.AnyMounted();
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x0004ACB8 File Offset: 0x00048EB8
	public void OnEngineStartFailed()
	{
		base.ClientRPC(null, "EngineStartFailed");
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x0004ACC8 File Offset: 0x00048EC8
	public StorageContainer GetTorpedoContainer()
	{
		global::BaseEntity baseEntity = this.torpedoStorageInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x0004AD00 File Offset: 0x00048F00
	public StorageContainer GetItemContainer()
	{
		global::BaseEntity baseEntity = this.itemStorageInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x0004AD38 File Offset: 0x00048F38
	private void ProcessCollision(Collision collision)
	{
		if (base.isClient || collision == null || collision.gameObject == null || collision.gameObject == null)
		{
			return;
		}
		float value = collision.impulse.magnitude / UnityEngine.Time.fixedDeltaTime;
		float num = Mathf.InverseLerp(100000f, 2500000f, value);
		if (num > 0f)
		{
			float b = Mathf.Lerp(1f, 200f, num);
			this.maxDamageThisTick = Mathf.Max(this.maxDamageThisTick, b);
		}
		if (num > 0f)
		{
			GameObjectRef effectGO = (this.curSubDepthY > 2f) ? this.underWatercollisionEffect : this.aboveWatercollisionEffect;
			base.TryShowCollisionFX(collision, effectGO);
		}
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x0004ADEC File Offset: 0x00048FEC
	private void UpdateClients()
	{
		if (base.HasDriver())
		{
			int num = (int)((byte)((this.ThrottleInput + 1f) * 7f));
			byte b = (byte)((this.UpDownInput + 1f) * 7f);
			byte arg = (byte)(num + ((int)b << 4));
			int arg2 = Mathf.CeilToInt(this.GetFuelAmount());
			base.ClientRPC<float, byte, int, float>(null, "SubmarineUpdate", this.RudderInput, arg, arg2, this.Oxygen);
		}
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x0004AE54 File Offset: 0x00049054
	private void SubmarineDecay()
	{
		BaseBoat.WaterVehicleDecay(this, 60f, this.timeSinceLastUsed, BaseSubmarine.outsidedecayminutes, BaseSubmarine.deepwaterdecayminutes);
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x0004AE78 File Offset: 0x00049078
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

	// Token: 0x0600075C RID: 1884 RVA: 0x0004AEA4 File Offset: 0x000490A4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_OpenTorpedoStorage(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeLooted(player))
		{
			return;
		}
		if (!this.PlayerIsMounted(player))
		{
			return;
		}
		StorageContainer torpedoContainer = this.GetTorpedoContainer();
		if (torpedoContainer != null)
		{
			torpedoContainer.PlayerOpenLoot(player, "", true);
		}
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x0004AEEC File Offset: 0x000490EC
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

	// Token: 0x0600075E RID: 1886 RVA: 0x0004AF28 File Offset: 0x00049128
	public void OnSurfacedInMoonpool()
	{
		if (!this.wasOnSurface || !GameInfo.HasAchievements)
		{
			return;
		}
		this.wasOnSurface = false;
		global::BasePlayer driver = base.GetDriver();
		if (driver != null)
		{
			driver.GiveAchievement("SUBMARINE_MOONPOOL");
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x0600075F RID: 1887 RVA: 0x0004AF67 File Offset: 0x00049167
	public ItemModGiveOxygen.AirSupplyType AirType
	{
		get
		{
			return ItemModGiveOxygen.AirSupplyType.Submarine;
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000760 RID: 1888 RVA: 0x0004AF6A File Offset: 0x0004916A
	public VehicleEngineController<BaseSubmarine>.EngineState EngineState
	{
		get
		{
			return this.engineController.CurEngineState;
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000761 RID: 1889 RVA: 0x0004AF77 File Offset: 0x00049177
	// (set) Token: 0x06000762 RID: 1890 RVA: 0x0004AF7F File Offset: 0x0004917F
	public Vector3 Velocity { get; private set; }

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000763 RID: 1891 RVA: 0x000035F8 File Offset: 0x000017F8
	public bool LightsAreOn
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved5);
		}
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000764 RID: 1892 RVA: 0x000035EB File Offset: 0x000017EB
	public bool HasAmmo
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000765 RID: 1893 RVA: 0x0004AF88 File Offset: 0x00049188
	// (set) Token: 0x06000766 RID: 1894 RVA: 0x0004AFA3 File Offset: 0x000491A3
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

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000767 RID: 1895 RVA: 0x0004AFBB File Offset: 0x000491BB
	// (set) Token: 0x06000768 RID: 1896 RVA: 0x0004AFC3 File Offset: 0x000491C3
	public float RudderInput
	{
		get
		{
			return this._rudder;
		}
		protected set
		{
			this._rudder = Mathf.Clamp(value, -1f, 1f);
		}
	}

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000769 RID: 1897 RVA: 0x0004AFDC File Offset: 0x000491DC
	// (set) Token: 0x0600076A RID: 1898 RVA: 0x0004B035 File Offset: 0x00049235
	public float UpDownInput
	{
		get
		{
			if (!base.isServer)
			{
				return this._upDown;
			}
			if (this.timeSinceLastUsed >= this.timeUntilAutoSurface)
			{
				return 0.15f;
			}
			if (!this.engineController.IsOn)
			{
				return Mathf.Max(0f, this._upDown);
			}
			return this._upDown;
		}
		protected set
		{
			this._upDown = Mathf.Clamp(value, -1f, 1f);
		}
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x0600076B RID: 1899 RVA: 0x0004B04D File Offset: 0x0004924D
	// (set) Token: 0x0600076C RID: 1900 RVA: 0x0004B055 File Offset: 0x00049255
	public float Oxygen
	{
		get
		{
			return this._oxygen;
		}
		protected set
		{
			this._oxygen = Mathf.Clamp(value, 0f, 1f);
		}
	}

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x0600076D RID: 1901 RVA: 0x0004B070 File Offset: 0x00049270
	protected float PhysicalRudderAngle
	{
		get
		{
			float num = this.rudderDetailedColliderTransform.localEulerAngles.y;
			if (num > 180f)
			{
				num -= 360f;
			}
			return num;
		}
	}

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x0600076E RID: 1902 RVA: 0x0004B09F File Offset: 0x0004929F
	protected bool IsInWater
	{
		get
		{
			return this.curSubDepthY > 0.2f;
		}
	}

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x0600076F RID: 1903 RVA: 0x0004B0AE File Offset: 0x000492AE
	protected bool IsSurfaced
	{
		get
		{
			return this.curSubDepthY < 1.1f;
		}
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x0004B0C0 File Offset: 0x000492C0
	public override void InitShared()
	{
		base.InitShared();
		this.waterLayerMask = LayerMask.GetMask(new string[]
		{
			"Water"
		});
		this.engineController = new VehicleEngineController<BaseSubmarine>(this, base.isServer, this.engineStartupTime, this.fuelStoragePrefab, null, global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x0004B110 File Offset: 0x00049310
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.submarine != null)
		{
			this.ThrottleInput = info.msg.submarine.throttle;
			this.UpDownInput = info.msg.submarine.upDown;
			this.RudderInput = info.msg.submarine.rudder;
			this.engineController.FuelSystem.fuelStorageInstance.uid = info.msg.submarine.fuelStorageID;
			this.cachedFuelAmount = info.msg.submarine.fuelAmount;
			this.torpedoStorageInstance.uid = info.msg.submarine.torpedoStorageID;
			this.Oxygen = info.msg.submarine.oxygen;
			this.itemStorageInstance.uid = info.msg.submarine.itemStorageID;
			this.UpdatePhysicalRudder(this.RudderInput, 0f);
		}
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x0004B20E File Offset: 0x0004940E
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old == next)
		{
			return;
		}
		if (base.isServer)
		{
			this.ServerFlagsChanged(old, next);
		}
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x00026FFC File Offset: 0x000251FC
	public override float WaterFactorForPlayer(global::BasePlayer player)
	{
		return 0f;
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x0004B22D File Offset: 0x0004942D
	public override float AirFactor()
	{
		return this.Oxygen;
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool BlocksWaterFor(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x0004B235 File Offset: 0x00049435
	public float GetFuelAmount()
	{
		if (base.isServer)
		{
			return (float)this.engineController.FuelSystem.GetFuelAmount();
		}
		return this.cachedFuelAmount;
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x0004B257 File Offset: 0x00049457
	public float GetSpeed()
	{
		if (base.IsStationary())
		{
			return 0f;
		}
		return Vector3.Dot(this.Velocity, base.transform.forward);
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x0004B27D File Offset: 0x0004947D
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.CanBeLooted(player) && (this.PlayerIsMounted(player) || (!this.internalAccessStorage && !base.IsOn()));
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x0004B2A8 File Offset: 0x000494A8
	public float GetAirTimeRemaining()
	{
		if (this.Oxygen <= 0.5f)
		{
			return 0f;
		}
		return Mathf.InverseLerp(0.5f, 1f, this.Oxygen) * BaseSubmarine.oxygenminutes * 60f;
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x0004B2DE File Offset: 0x000494DE
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && !pusher.isMounted && !pusher.IsSwimming() && pusher.IsOnGround() && !pusher.IsStandingOnEntity(this, 8192);
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x0004B314 File Offset: 0x00049514
	private void UpdatePhysicalRudder(float turnInput, float deltaTime)
	{
		float num = -turnInput * this.maxRudderAngle;
		float y;
		if (base.IsMovingOrOn)
		{
			y = Mathf.MoveTowards(this.PhysicalRudderAngle, num, 200f * deltaTime);
		}
		else
		{
			y = num;
		}
		Quaternion localRotation = Quaternion.Euler(0f, y, 0f);
		if (base.isClient)
		{
			this.rudderVisualTransform.localRotation = localRotation;
		}
		this.rudderDetailedColliderTransform.localRotation = localRotation;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x0004B37C File Offset: 0x0004957C
	private bool CanMount(global::BasePlayer player)
	{
		return !player.IsDead();
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x0004B387 File Offset: 0x00049587
	private void UpdateWaterInfo()
	{
		this.waterSurfaceY = this.GetWaterSurfaceY();
		this.curSubDepthY = this.waterSurfaceY - base.transform.position.y;
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x0004B3B4 File Offset: 0x000495B4
	private float GetWaterSurfaceY()
	{
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(base.transform.position - Vector3.up * 1.5f, Vector3.up, out raycastHit, 5f, this.waterLayerMask, QueryTriggerInteraction.Collide))
		{
			return raycastHit.point.y;
		}
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(base.transform.position, true, this, false);
		if (!waterInfo.isValid)
		{
			return base.transform.position.y - 1f;
		}
		return waterInfo.surfaceLevel;
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x00007260 File Offset: 0x00005460
	void IEngineControllerUser.Invoke(Action action, float time)
	{
		base.Invoke(action, time);
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x0000726A File Offset: 0x0000546A
	void IEngineControllerUser.CancelInvoke(Action action)
	{
		base.CancelInvoke(action);
	}

	// Token: 0x040004DA RID: 1242
	private float targetClimbSpeed;

	// Token: 0x040004DB RID: 1243
	private float maxDamageThisTick;

	// Token: 0x040004DC RID: 1244
	private float nextCollisionDamageTime;

	// Token: 0x040004DD RID: 1245
	private bool prevPrimaryFireInput;

	// Token: 0x040004DE RID: 1246
	private bool primaryFireInput;

	// Token: 0x040004DF RID: 1247
	private const float DECAY_TICK_TIME = 60f;

	// Token: 0x040004E0 RID: 1248
	private TimeSince timeSinceLastUsed;

	// Token: 0x040004E1 RID: 1249
	private TimeSince timeSinceTorpedoFired;

	// Token: 0x040004E2 RID: 1250
	private TimeSince timeSinceFailRPCSent;

	// Token: 0x040004E3 RID: 1251
	private float normalDrag;

	// Token: 0x040004E4 RID: 1252
	private float highDrag;

	// Token: 0x040004E5 RID: 1253
	private bool wasOnSurface;

	// Token: 0x040004E6 RID: 1254
	[Header("Submarine Main")]
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x040004E7 RID: 1255
	[SerializeField]
	private Buoyancy buoyancy;

	// Token: 0x040004E8 RID: 1256
	[SerializeField]
	protected float maxRudderAngle = 35f;

	// Token: 0x040004E9 RID: 1257
	[SerializeField]
	private Transform rudderVisualTransform;

	// Token: 0x040004EA RID: 1258
	[SerializeField]
	private Transform rudderDetailedColliderTransform;

	// Token: 0x040004EB RID: 1259
	[SerializeField]
	private Transform propellerTransform;

	// Token: 0x040004EC RID: 1260
	[SerializeField]
	private float timeUntilAutoSurface = 300f;

	// Token: 0x040004ED RID: 1261
	[SerializeField]
	private Renderer[] interiorRenderers;

	// Token: 0x040004EE RID: 1262
	[SerializeField]
	private SonarObject sonarObject;

	// Token: 0x040004EF RID: 1263
	[SerializeField]
	private BaseSubmarine.ParentTriggerInfo[] parentTriggers;

	// Token: 0x040004F0 RID: 1264
	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	// Token: 0x040004F1 RID: 1265
	[Header("Submarine Engine & Fuel")]
	[SerializeField]
	private float engineKW = 200f;

	// Token: 0x040004F2 RID: 1266
	[SerializeField]
	private float turnPower = 0.25f;

	// Token: 0x040004F3 RID: 1267
	[SerializeField]
	private float engineStartupTime = 0.5f;

	// Token: 0x040004F4 RID: 1268
	[SerializeField]
	private GameObjectRef itemStoragePrefab;

	// Token: 0x040004F5 RID: 1269
	[SerializeField]
	private float depthChangeTargetSpeed = 1f;

	// Token: 0x040004F6 RID: 1270
	[SerializeField]
	private float idleFuelPerSec = 0.03f;

	// Token: 0x040004F7 RID: 1271
	[SerializeField]
	private float maxFuelPerSec = 0.15f;

	// Token: 0x040004F8 RID: 1272
	[FormerlySerializedAs("internalAccessFuelTank")]
	[SerializeField]
	private bool internalAccessStorage;

	// Token: 0x040004F9 RID: 1273
	[Header("Submarine Weaponry")]
	[SerializeField]
	private GameObjectRef torpedoStoragePrefab;

	// Token: 0x040004FA RID: 1274
	[SerializeField]
	private Transform torpedoFiringPoint;

	// Token: 0x040004FB RID: 1275
	[SerializeField]
	private float maxFireRate = 1.5f;

	// Token: 0x040004FC RID: 1276
	[Header("Submarine Audio & FX")]
	[SerializeField]
	protected SubmarineAudio submarineAudio;

	// Token: 0x040004FD RID: 1277
	[SerializeField]
	private ParticleSystem fxTorpedoFire;

	// Token: 0x040004FE RID: 1278
	[SerializeField]
	private GameObject internalFXContainer;

	// Token: 0x040004FF RID: 1279
	[SerializeField]
	private GameObject internalOnFXContainer;

	// Token: 0x04000500 RID: 1280
	[SerializeField]
	private ParticleSystem fxIntAmbientBubbleLoop;

	// Token: 0x04000501 RID: 1281
	[SerializeField]
	private ParticleSystem fxIntInitialDiveBubbles;

	// Token: 0x04000502 RID: 1282
	[SerializeField]
	private ParticleSystem fxIntWaterDropSpray;

	// Token: 0x04000503 RID: 1283
	[SerializeField]
	private ParticleSystem fxIntWindowFilm;

	// Token: 0x04000504 RID: 1284
	[SerializeField]
	private ParticleSystemContainer fxIntMediumDamage;

	// Token: 0x04000505 RID: 1285
	[SerializeField]
	private ParticleSystemContainer fxIntHeavyDamage;

	// Token: 0x04000506 RID: 1286
	[SerializeField]
	private GameObject externalFXContainer;

	// Token: 0x04000507 RID: 1287
	[SerializeField]
	private GameObject externalOnFXContainer;

	// Token: 0x04000508 RID: 1288
	[SerializeField]
	private ParticleSystem fxExtAmbientBubbleLoop;

	// Token: 0x04000509 RID: 1289
	[SerializeField]
	private ParticleSystem fxExtInitialDiveBubbles;

	// Token: 0x0400050A RID: 1290
	[SerializeField]
	private ParticleSystem fxExtAboveWaterEngineThrustForward;

	// Token: 0x0400050B RID: 1291
	[SerializeField]
	private ParticleSystem fxExtAboveWaterEngineThrustReverse;

	// Token: 0x0400050C RID: 1292
	[SerializeField]
	private ParticleSystem fxExtUnderWaterEngineThrustForward;

	// Token: 0x0400050D RID: 1293
	[SerializeField]
	private ParticleSystem[] fxExtUnderWaterEngineThrustForwardSubs;

	// Token: 0x0400050E RID: 1294
	[SerializeField]
	private ParticleSystem fxExtUnderWaterEngineThrustReverse;

	// Token: 0x0400050F RID: 1295
	[SerializeField]
	private ParticleSystem[] fxExtUnderWaterEngineThrustReverseSubs;

	// Token: 0x04000510 RID: 1296
	[SerializeField]
	private ParticleSystem fxExtBowWave;

	// Token: 0x04000511 RID: 1297
	[SerializeField]
	private ParticleSystem fxExtWakeEffect;

	// Token: 0x04000512 RID: 1298
	[SerializeField]
	private GameObjectRef aboveWatercollisionEffect;

	// Token: 0x04000513 RID: 1299
	[SerializeField]
	private GameObjectRef underWatercollisionEffect;

	// Token: 0x04000514 RID: 1300
	[SerializeField]
	private VolumetricLightBeam spotlightVolumetrics;

	// Token: 0x04000515 RID: 1301
	[SerializeField]
	private float mountedAlphaInside = 0.04f;

	// Token: 0x04000516 RID: 1302
	[SerializeField]
	private float mountedAlphaOutside = 0.015f;

	// Token: 0x04000517 RID: 1303
	[ServerVar(Help = "How long before a submarine loses all its health while outside. If it's in deep water, deepwaterdecayminutes is used")]
	public static float outsidedecayminutes = 180f;

	// Token: 0x04000518 RID: 1304
	[ServerVar(Help = "How long before a submarine loses all its health while in deep water")]
	public static float deepwaterdecayminutes = 120f;

	// Token: 0x04000519 RID: 1305
	[ServerVar(Help = "How long a submarine can stay underwater until players start taking damage from low oxygen")]
	public static float oxygenminutes = 10f;

	// Token: 0x0400051B RID: 1307
	public const global::BaseEntity.Flags Flag_Ammo = global::BaseEntity.Flags.Reserved6;

	// Token: 0x0400051C RID: 1308
	private float _throttle;

	// Token: 0x0400051D RID: 1309
	private float _rudder;

	// Token: 0x0400051E RID: 1310
	private float _upDown;

	// Token: 0x0400051F RID: 1311
	private float _oxygen = 1f;

	// Token: 0x04000520 RID: 1312
	protected VehicleEngineController<BaseSubmarine> engineController;

	// Token: 0x04000521 RID: 1313
	protected float cachedFuelAmount;

	// Token: 0x04000522 RID: 1314
	protected Vector3 steerAngle;

	// Token: 0x04000523 RID: 1315
	protected float waterSurfaceY;

	// Token: 0x04000524 RID: 1316
	protected float curSubDepthY;

	// Token: 0x04000525 RID: 1317
	private EntityRef<StorageContainer> torpedoStorageInstance;

	// Token: 0x04000526 RID: 1318
	private EntityRef<StorageContainer> itemStorageInstance;

	// Token: 0x04000527 RID: 1319
	private int waterLayerMask;

	// Token: 0x02000B70 RID: 2928
	[Serializable]
	public class ParentTriggerInfo
	{
		// Token: 0x04003E67 RID: 15975
		public TriggerParent trigger;

		// Token: 0x04003E68 RID: 15976
		public Transform triggerWaterLevel;
	}
}
