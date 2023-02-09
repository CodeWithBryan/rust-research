using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000099 RID: 153
public class MLRS : BaseMountable
{
	// Token: 0x06000DBD RID: 3517 RVA: 0x000734D4 File Offset: 0x000716D4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MLRS.OnRpcMessage", 0))
		{
			if (rpc == 455279877U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Fire_Rockets ");
				}
				using (TimeWarning.New("RPC_Fire_Rockets", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(455279877U, "RPC_Fire_Rockets", this, player, 3f))
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
							this.RPC_Fire_Rockets(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Fire_Rockets");
					}
				}
				return true;
			}
			if (rpc == 751446792U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Open_Dashboard ");
				}
				using (TimeWarning.New("RPC_Open_Dashboard", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(751446792U, "RPC_Open_Dashboard", this, player, 3f))
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
							this.RPC_Open_Dashboard(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Open_Dashboard");
					}
				}
				return true;
			}
			if (rpc == 1311007340U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Open_Rockets ");
				}
				using (TimeWarning.New("RPC_Open_Rockets", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1311007340U, "RPC_Open_Rockets", this, player, 3f))
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
							this.RPC_Open_Rockets(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_Open_Rockets");
					}
				}
				return true;
			}
			if (rpc == 858951307U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_SetTargetHitPos ");
				}
				using (TimeWarning.New("RPC_SetTargetHitPos", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(858951307U, "RPC_SetTargetHitPos", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_SetTargetHitPos(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_SetTargetHitPos");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x00073A8C File Offset: 0x00071C8C
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (child.prefabID == this.rocketStoragePrefab.GetEntity().prefabID)
			{
				this.rocketStorageInstance.Set(child);
			}
			if (child.prefabID == this.dashboardStoragePrefab.GetEntity().prefabID)
			{
				this.dashboardStorageInstance.Set(child);
			}
		}
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x00073AF0 File Offset: 0x00071CF0
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (base.IsBroken())
		{
			if (this.timeSinceBroken < global::MLRS.brokenDownMinutes * 60f)
			{
				global::Item item;
				base.SetFlag(global::BaseEntity.Flags.Reserved8, this.TryGetAimingModule(out item), false, true);
				return;
			}
			this.SetRepaired();
		}
		int rocketAmmoCount = this.RocketAmmoCount;
		this.UpdateStorageState();
		if (this.CanBeUsed && this.AnyMounted())
		{
			Vector3 vector = this.UserTargetHitPos;
			vector += Vector3.forward * this.upDownInput * 75f * UnityEngine.Time.fixedDeltaTime;
			vector += Vector3.right * this.leftRightInput * 75f * UnityEngine.Time.fixedDeltaTime;
			this.SetUserTargetHitPos(vector);
		}
		if (!this.IsFiringRockets)
		{
			float num;
			float num2;
			float num3;
			this.HitPosToRotation(this.trueTargetHitPos, out num, out num2, out num3);
			float num4 = num3 / -UnityEngine.Physics.gravity.y;
			this.IsRealigning = (Mathf.Abs(Mathf.DeltaAngle(this.VRotation, num2)) > 0.001f || Mathf.Abs(Mathf.DeltaAngle(this.HRotation, num)) > 0.001f || !Mathf.Approximately(this.CurGravityMultiplier, num4));
			if (this.IsRealigning)
			{
				if (this.isInitialLoad)
				{
					this.VRotation = num2;
					this.HRotation = num;
					this.isInitialLoad = false;
				}
				else
				{
					this.VRotation = Mathf.MoveTowardsAngle(this.VRotation, num2, UnityEngine.Time.deltaTime * this.vRotSpeed);
					this.HRotation = Mathf.MoveTowardsAngle(this.HRotation, num, UnityEngine.Time.deltaTime * this.hRotSpeed);
				}
				this.CurGravityMultiplier = num4;
				this.TrueHitPos = this.GetTrueHitPos();
			}
		}
		if (this.UserTargetHitPos != this.lastSentTargetHitPos || this.TrueHitPos != this.lastSentTrueHitPos || this.RocketAmmoCount != rocketAmmoCount)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x00073CE8 File Offset: 0x00071EE8
	private Vector3 GetTrueHitPos()
	{
		global::MLRS.TheoreticalProjectile theoreticalProjectile = new global::MLRS.TheoreticalProjectile(this.firingPoint.position, this.firingPoint.forward.normalized * this.rocketSpeed, this.CurGravityMultiplier);
		int num = 0;
		float dt = (theoreticalProjectile.forward.y > 0f) ? 2f : 0.66f;
		while (!this.NextRayHitSomething(ref theoreticalProjectile, dt) && (float)num < 128f)
		{
			num++;
		}
		return theoreticalProjectile.pos;
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x00073D6C File Offset: 0x00071F6C
	private bool NextRayHitSomething(ref global::MLRS.TheoreticalProjectile projectile, float dt)
	{
		float num = UnityEngine.Physics.gravity.y * projectile.gravityMult;
		Vector3 pos = projectile.pos;
		float d = projectile.forward.MagnitudeXZ() * dt;
		float y = projectile.forward.y * dt + num * dt * dt * 0.5f;
		Vector2 vector = projectile.forward.XZ2D().normalized * d;
		Vector3 b = new Vector3(vector.x, y, vector.y);
		projectile.pos += b;
		float y2 = projectile.forward.y + num * dt;
		projectile.forward.y = y2;
		RaycastHit hit;
		if (UnityEngine.Physics.Linecast(pos, projectile.pos, out hit, 1084293393, QueryTriggerInteraction.Ignore))
		{
			projectile.pos = hit.point;
			global::BaseEntity entity = hit.GetEntity();
			bool flag = entity != null && entity.EqualNetID(this);
			if (flag)
			{
				projectile.pos += projectile.forward * 1f;
			}
			return !flag;
		}
		return false;
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x00073E8C File Offset: 0x0007208C
	private float GetSurfaceHeight(Vector3 pos)
	{
		float height = TerrainMeta.HeightMap.GetHeight(pos);
		float height2 = TerrainMeta.WaterMap.GetHeight(pos);
		return Mathf.Max(height, height2);
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x00073EB6 File Offset: 0x000720B6
	private void SetRepaired()
	{
		base.SetFlag(global::BaseEntity.Flags.Broken, false, false, true);
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x00073EC8 File Offset: 0x000720C8
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.upDownInput = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.upDownInput = -1f;
		}
		else
		{
			this.upDownInput = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.leftRightInput = -1f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.leftRightInput = 1f;
			return;
		}
		this.leftRightInput = 0f;
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x00073F44 File Offset: 0x00072144
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.mlrs = Facepunch.Pool.Get<ProtoBuf.MLRS>();
		info.msg.mlrs.targetPos = this.UserTargetHitPos;
		info.msg.mlrs.curHitPos = this.TrueHitPos;
		info.msg.mlrs.rocketStorageID = this.rocketStorageInstance.uid;
		info.msg.mlrs.dashboardStorageID = this.dashboardStorageInstance.uid;
		info.msg.mlrs.ammoCount = (uint)this.RocketAmmoCount;
		this.lastSentTargetHitPos = this.UserTargetHitPos;
		this.lastSentTrueHitPos = this.TrueHitPos;
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x00073FF8 File Offset: 0x000721F8
	public bool AdminFixUp()
	{
		if (this.IsDead() || this.IsFiringRockets)
		{
			return false;
		}
		StorageContainer dashboardContainer = this.GetDashboardContainer();
		if (!this.HasAimingModule)
		{
			dashboardContainer.inventory.AddItem(ItemManager.FindItemDefinition("aiming.module.mlrs"), 1, 0UL, global::ItemContainer.LimitStack.Existing);
		}
		StorageContainer rocketContainer = this.GetRocketContainer();
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("ammo.rocket.mlrs");
		if (this.RocketAmmoCount < rocketContainer.inventory.capacity * itemDefinition.stackable)
		{
			int num;
			for (int i = itemDefinition.stackable * rocketContainer.inventory.capacity - this.RocketAmmoCount; i > 0; i -= num)
			{
				num = Mathf.Min(i, itemDefinition.stackable);
				rocketContainer.inventory.AddItem(itemDefinition, itemDefinition.stackable, 0UL, global::ItemContainer.LimitStack.Existing);
			}
		}
		this.SetRepaired();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x000740C4 File Offset: 0x000722C4
	private void Fire(global::BasePlayer owner)
	{
		this.UpdateStorageState();
		if (!this.CanFire)
		{
			return;
		}
		if (this._mounted == null)
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, true, false, true);
		this.radiusModIndex = 0;
		this.nextRocketIndex = Mathf.Min(this.RocketAmmoCount - 1, this.rocketTubes.Length - 1);
		this.rocketOwnerRef.Set(owner);
		base.InvokeRepeating(new Action(this.FireNextRocket), 0f, 0.5f);
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x0007414C File Offset: 0x0007234C
	private void EndFiring()
	{
		base.CancelInvoke(new Action(this.FireNextRocket));
		this.rocketOwnerRef.Set(null);
		global::Item item;
		if (this.TryGetAimingModule(out item))
		{
			item.LoseCondition(1f);
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Broken, true, false, false);
		base.SendNetworkUpdate_Flags();
		this.timeSinceBroken = 0f;
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x000741C0 File Offset: 0x000723C0
	private void FireNextRocket()
	{
		this.RocketAmmoCount = this.GetRocketContainer().inventory.GetAmmoAmount(AmmoTypes.MLRS_ROCKET);
		if (this.nextRocketIndex < 0 || this.nextRocketIndex >= this.RocketAmmoCount || base.IsBroken())
		{
			this.EndFiring();
			return;
		}
		StorageContainer rocketContainer = this.GetRocketContainer();
		Vector3 firingPos = this.firingPoint.position + this.firingPoint.rotation * this.rocketTubes[this.nextRocketIndex].firingOffset;
		float d = 1f;
		if (this.radiusModIndex < this.radiusMods.Length)
		{
			d = this.radiusMods[this.radiusModIndex];
		}
		this.radiusModIndex++;
		Vector2 vector = UnityEngine.Random.insideUnitCircle * (this.targetAreaRadius - this.RocketDamageRadius) * d;
		Vector3 targetPos = this.TrueHitPos + new Vector3(vector.x, 0f, vector.y);
		float num;
		Vector3 aimToTarget = this.GetAimToTarget(targetPos, out num);
		ServerProjectile serverProjectile;
		if (BaseMountable.TryFireProjectile(rocketContainer, AmmoTypes.MLRS_ROCKET, firingPos, aimToTarget, this._mounted, 0f, 0f, out serverProjectile))
		{
			serverProjectile.gravityModifier = num / -UnityEngine.Physics.gravity.y;
			this.nextRocketIndex--;
			return;
		}
		this.EndFiring();
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x00074310 File Offset: 0x00072510
	private void UpdateStorageState()
	{
		global::Item item;
		bool b = this.TryGetAimingModule(out item);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, b, false, true);
		this.RocketAmmoCount = this.GetRocketContainer().inventory.GetAmmoAmount(AmmoTypes.MLRS_ROCKET);
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x00074350 File Offset: 0x00072550
	private bool TryGetAimingModule(out global::Item item)
	{
		global::ItemContainer inventory = this.GetDashboardContainer().inventory;
		if (!inventory.IsEmpty())
		{
			item = inventory.itemList[0];
			return true;
		}
		item = null;
		return false;
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x00074388 File Offset: 0x00072588
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_SetTargetHitPos(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.PlayerIsMounted(player))
		{
			return;
		}
		this.SetUserTargetHitPos(msg.read.Vector3());
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x000743B8 File Offset: 0x000725B8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Fire_Rockets(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.PlayerIsMounted(player))
		{
			return;
		}
		this.Fire(player);
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x000743E0 File Offset: 0x000725E0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Open_Rockets(global::BaseEntity.RPCMessage msg)
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
		IItemContainerEntity rocketContainer = this.GetRocketContainer();
		if (!rocketContainer.IsUnityNull<IItemContainerEntity>())
		{
			rocketContainer.PlayerOpenLoot(player, "", false);
			return;
		}
		Debug.LogError(base.GetType().Name + ": No container component found.");
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x00074440 File Offset: 0x00072640
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Open_Dashboard(global::BaseEntity.RPCMessage msg)
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
		IItemContainerEntity dashboardContainer = this.GetDashboardContainer();
		if (!dashboardContainer.IsUnityNull<IItemContainerEntity>())
		{
			dashboardContainer.PlayerOpenLoot(player, "", true);
			return;
		}
		Debug.LogError(base.GetType().Name + ": No container component found.");
	}

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06000DD0 RID: 3536 RVA: 0x000744A0 File Offset: 0x000726A0
	// (set) Token: 0x06000DD1 RID: 3537 RVA: 0x000744A8 File Offset: 0x000726A8
	public Vector3 UserTargetHitPos { get; private set; }

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x06000DD2 RID: 3538 RVA: 0x000744B1 File Offset: 0x000726B1
	// (set) Token: 0x06000DD3 RID: 3539 RVA: 0x000744B9 File Offset: 0x000726B9
	public Vector3 TrueHitPos { get; private set; }

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000DD4 RID: 3540 RVA: 0x000028C8 File Offset: 0x00000AC8
	public bool HasAimingModule
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved8);
		}
	}

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000DD5 RID: 3541 RVA: 0x000744C2 File Offset: 0x000726C2
	private bool CanBeUsed
	{
		get
		{
			return this.HasAimingModule && !base.IsBroken();
		}
	}

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000DD6 RID: 3542 RVA: 0x000744D7 File Offset: 0x000726D7
	private bool CanFire
	{
		get
		{
			return this.CanBeUsed && this.RocketAmmoCount > 0 && !this.IsFiringRockets && !this.IsRealigning;
		}
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000DD7 RID: 3543 RVA: 0x000744FD File Offset: 0x000726FD
	// (set) Token: 0x06000DD8 RID: 3544 RVA: 0x00074510 File Offset: 0x00072710
	private float HRotation
	{
		get
		{
			return this.hRotator.eulerAngles.y;
		}
		set
		{
			Vector3 eulerAngles = this.hRotator.eulerAngles;
			eulerAngles.y = value;
			this.hRotator.eulerAngles = eulerAngles;
		}
	}

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x06000DD9 RID: 3545 RVA: 0x0007453D File Offset: 0x0007273D
	// (set) Token: 0x06000DDA RID: 3546 RVA: 0x00074550 File Offset: 0x00072750
	private float VRotation
	{
		get
		{
			return this.vRotator.localEulerAngles.x;
		}
		set
		{
			Vector3 localEulerAngles = this.vRotator.localEulerAngles;
			if (value < 0f)
			{
				localEulerAngles.x = Mathf.Clamp(value, -this.vRotMax, 0f);
			}
			else if (value > 0f)
			{
				localEulerAngles.x = Mathf.Clamp(value, 360f - this.vRotMax, 360f);
			}
			this.vRotator.localEulerAngles = localEulerAngles;
		}
	}

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x06000DDB RID: 3547 RVA: 0x000745BE File Offset: 0x000727BE
	// (set) Token: 0x06000DDC RID: 3548 RVA: 0x000745C6 File Offset: 0x000727C6
	public float CurGravityMultiplier { get; private set; }

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000DDD RID: 3549 RVA: 0x000745CF File Offset: 0x000727CF
	// (set) Token: 0x06000DDE RID: 3550 RVA: 0x000745D7 File Offset: 0x000727D7
	public int RocketAmmoCount { get; private set; }

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000DDF RID: 3551 RVA: 0x000745E0 File Offset: 0x000727E0
	// (set) Token: 0x06000DE0 RID: 3552 RVA: 0x000745E8 File Offset: 0x000727E8
	public bool IsRealigning { get; private set; }

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000DE1 RID: 3553 RVA: 0x000035EB File Offset: 0x000017EB
	public bool IsFiringRockets
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000DE2 RID: 3554 RVA: 0x000745F1 File Offset: 0x000727F1
	// (set) Token: 0x06000DE3 RID: 3555 RVA: 0x000745F9 File Offset: 0x000727F9
	public float RocketDamageRadius { get; private set; }

	// Token: 0x06000DE4 RID: 3556 RVA: 0x00074604 File Offset: 0x00072804
	public override void InitShared()
	{
		base.InitShared();
		GameObject gameObject = this.mlrsRocket.Get();
		ServerProjectile component = gameObject.GetComponent<ServerProjectile>();
		this.rocketBaseGravity = -UnityEngine.Physics.gravity.y * component.gravityModifier;
		this.rocketSpeed = component.speed;
		TimedExplosive component2 = gameObject.GetComponent<TimedExplosive>();
		this.RocketDamageRadius = component2.explosionRadius;
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x00074660 File Offset: 0x00072860
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.mlrs != null)
		{
			this.SetUserTargetHitPos(info.msg.mlrs.targetPos);
			this.TrueHitPos = info.msg.mlrs.curHitPos;
			float hrotation;
			float vrotation;
			float num;
			this.HitPosToRotation(this.TrueHitPos, out hrotation, out vrotation, out num);
			this.CurGravityMultiplier = num / -UnityEngine.Physics.gravity.y;
			if (base.isServer)
			{
				this.HRotation = hrotation;
				this.VRotation = vrotation;
			}
			this.rocketStorageInstance.uid = info.msg.mlrs.rocketStorageID;
			this.dashboardStorageInstance.uid = info.msg.mlrs.dashboardStorageID;
			this.RocketAmmoCount = (int)info.msg.mlrs.ammoCount;
		}
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x00074737 File Offset: 0x00072937
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return !this.IsFiringRockets;
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00074744 File Offset: 0x00072944
	private void SetUserTargetHitPos(Vector3 worldPos)
	{
		if (this.UserTargetHitPos == worldPos)
		{
			return;
		}
		if (base.isServer)
		{
			Vector3 position = TerrainMeta.Position;
			Vector3 vector = position + TerrainMeta.Size;
			worldPos.x = Mathf.Clamp(worldPos.x, position.x, vector.x);
			worldPos.z = Mathf.Clamp(worldPos.z, position.z, vector.z);
			worldPos.y = this.GetSurfaceHeight(worldPos);
		}
		this.UserTargetHitPos = worldPos;
		if (base.isServer)
		{
			this.trueTargetHitPos = this.UserTargetHitPos;
			foreach (TriggerSafeZone triggerSafeZone in TriggerSafeZone.allSafeZones)
			{
				Vector3 center = triggerSafeZone.triggerCollider.bounds.center;
				center.y = 0f;
				float num = triggerSafeZone.triggerCollider.GetRadius(triggerSafeZone.transform.localScale) + this.targetAreaRadius;
				this.trueTargetHitPos.y = 0f;
				if (Vector3.Distance(center, this.trueTargetHitPos) < num)
				{
					Vector3 vector2 = this.trueTargetHitPos - center;
					this.trueTargetHitPos = center + vector2.normalized * num;
					this.trueTargetHitPos.y = this.GetSurfaceHeight(this.trueTargetHitPos);
					break;
				}
			}
		}
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x000748CC File Offset: 0x00072ACC
	private StorageContainer GetRocketContainer()
	{
		global::BaseEntity baseEntity = this.rocketStorageInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x00074904 File Offset: 0x00072B04
	private StorageContainer GetDashboardContainer()
	{
		global::BaseEntity baseEntity = this.dashboardStorageInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x0007493C File Offset: 0x00072B3C
	private void HitPosToRotation(Vector3 hitPos, out float hRot, out float vRot, out float g)
	{
		Vector3 aimToTarget = this.GetAimToTarget(hitPos, out g);
		Vector3 eulerAngles = Quaternion.LookRotation(aimToTarget, Vector3.up).eulerAngles;
		vRot = eulerAngles.x - 360f;
		aimToTarget.y = 0f;
		hRot = eulerAngles.y;
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x0007498C File Offset: 0x00072B8C
	private Vector3 GetAimToTarget(Vector3 targetPos, out float g)
	{
		g = this.rocketBaseGravity;
		float num = this.rocketSpeed;
		Vector3 vector = targetPos - this.firingPoint.position;
		float num2 = vector.Magnitude2D();
		float y = vector.y;
		float num3 = Mathf.Sqrt(num * num * num * num - g * (g * (num2 * num2) + 2f * y * num * num));
		float num4 = Mathf.Atan((num * num + num3) / (g * num2)) * 57.29578f;
		float num5 = Mathf.Clamp(num4, 0f, 90f);
		if (float.IsNaN(num4))
		{
			num5 = 45f;
			g = global::MLRS.ProjectileDistToGravity(num2, y, num5, num);
		}
		else if (num4 > this.vRotMax)
		{
			num5 = this.vRotMax;
			g = global::MLRS.ProjectileDistToGravity(Mathf.Max(num2, this.minRange), y, num5, num);
		}
		vector.Normalize();
		vector.y = 0f;
		Vector3 axis = Vector3.Cross(vector, Vector3.up);
		return Quaternion.AngleAxis(num5, axis) * vector;
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x00074A90 File Offset: 0x00072C90
	private static float ProjectileDistToSpeed(float x, float y, float angle, float g, float fallbackV)
	{
		float num = angle * 0.017453292f;
		float num2 = Mathf.Sqrt(x * x * g / (x * Mathf.Sin(2f * num) - 2f * y * Mathf.Cos(num) * Mathf.Cos(num)));
		if (float.IsNaN(num2) || num2 < 1f)
		{
			num2 = fallbackV;
		}
		return num2;
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x00074AEC File Offset: 0x00072CEC
	private static float ProjectileDistToGravity(float x, float y, float θ, float v)
	{
		float num = θ * 0.017453292f;
		float num2 = (v * v * x * Mathf.Sin(2f * num) - 2f * v * v * y * Mathf.Cos(num) * Mathf.Cos(num)) / (x * x);
		if (float.IsNaN(num2) || num2 < 0.01f)
		{
			num2 = -UnityEngine.Physics.gravity.y;
		}
		return num2;
	}

	// Token: 0x040008E8 RID: 2280
	public const string MLRS_PLAYER_KILL_STAT = "mlrs_kills";

	// Token: 0x040008E9 RID: 2281
	private float leftRightInput;

	// Token: 0x040008EA RID: 2282
	private float upDownInput;

	// Token: 0x040008EB RID: 2283
	private Vector3 lastSentTargetHitPos;

	// Token: 0x040008EC RID: 2284
	private Vector3 lastSentTrueHitPos;

	// Token: 0x040008ED RID: 2285
	private int nextRocketIndex;

	// Token: 0x040008EE RID: 2286
	private EntityRef rocketOwnerRef;

	// Token: 0x040008EF RID: 2287
	private TimeSince timeSinceBroken;

	// Token: 0x040008F0 RID: 2288
	private int radiusModIndex;

	// Token: 0x040008F1 RID: 2289
	private float[] radiusMods = new float[]
	{
		0.1f,
		0.2f,
		0.33333334f,
		0.6666667f
	};

	// Token: 0x040008F2 RID: 2290
	private Vector3 trueTargetHitPos;

	// Token: 0x040008F3 RID: 2291
	[Header("MLRS Components")]
	[SerializeField]
	private GameObjectRef rocketStoragePrefab;

	// Token: 0x040008F4 RID: 2292
	[SerializeField]
	private GameObjectRef dashboardStoragePrefab;

	// Token: 0x040008F5 RID: 2293
	[Header("MLRS Rotation")]
	[SerializeField]
	private Transform hRotator;

	// Token: 0x040008F6 RID: 2294
	[SerializeField]
	private float hRotSpeed = 25f;

	// Token: 0x040008F7 RID: 2295
	[SerializeField]
	private Transform vRotator;

	// Token: 0x040008F8 RID: 2296
	[SerializeField]
	private float vRotSpeed = 10f;

	// Token: 0x040008F9 RID: 2297
	[SerializeField]
	[Range(50f, 90f)]
	private float vRotMax = 85f;

	// Token: 0x040008FA RID: 2298
	[SerializeField]
	private Transform hydraulics;

	// Token: 0x040008FB RID: 2299
	[Header("MLRS Weaponry")]
	[Tooltip("Minimum distance from the MLRS to a targeted hit point. In metres.")]
	[SerializeField]
	public float minRange = 200f;

	// Token: 0x040008FC RID: 2300
	[Tooltip("The size of the area that the rockets may hit, minus rocket damage radius.")]
	[SerializeField]
	public float targetAreaRadius = 30f;

	// Token: 0x040008FD RID: 2301
	[SerializeField]
	private GameObjectRef mlrsRocket;

	// Token: 0x040008FE RID: 2302
	[SerializeField]
	public Transform firingPoint;

	// Token: 0x040008FF RID: 2303
	[SerializeField]
	private global::MLRS.RocketTube[] rocketTubes;

	// Token: 0x04000900 RID: 2304
	[Header("MLRS Dashboard/FX")]
	[SerializeField]
	private GameObject screensChild;

	// Token: 0x04000901 RID: 2305
	[SerializeField]
	private Transform leftHandGrip;

	// Token: 0x04000902 RID: 2306
	[SerializeField]
	private Transform leftJoystick;

	// Token: 0x04000903 RID: 2307
	[SerializeField]
	private Transform rightHandGrip;

	// Token: 0x04000904 RID: 2308
	[SerializeField]
	private Transform rightJoystick;

	// Token: 0x04000905 RID: 2309
	[SerializeField]
	private Transform controlKnobHeight;

	// Token: 0x04000906 RID: 2310
	[SerializeField]
	private Transform controlKnobAngle;

	// Token: 0x04000907 RID: 2311
	[SerializeField]
	private GameObjectRef uiDialogPrefab;

	// Token: 0x04000908 RID: 2312
	[SerializeField]
	private Light fireButtonLight;

	// Token: 0x04000909 RID: 2313
	[SerializeField]
	private GameObject brokenDownEffect;

	// Token: 0x0400090A RID: 2314
	[SerializeField]
	private ParticleSystem topScreenShutdown;

	// Token: 0x0400090B RID: 2315
	[SerializeField]
	private ParticleSystem bottomScreenShutdown;

	// Token: 0x0400090C RID: 2316
	[ServerVar(Help = "How many minutes before the MLRS recovers from use and can be used again")]
	public static float brokenDownMinutes = 10f;

	// Token: 0x04000912 RID: 2322
	public const global::BaseEntity.Flags FLAG_FIRING_ROCKETS = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000913 RID: 2323
	public const global::BaseEntity.Flags FLAG_HAS_AIMING_MODULE = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000914 RID: 2324
	private EntityRef rocketStorageInstance;

	// Token: 0x04000915 RID: 2325
	private EntityRef dashboardStorageInstance;

	// Token: 0x04000916 RID: 2326
	private float rocketBaseGravity;

	// Token: 0x04000917 RID: 2327
	private float rocketSpeed;

	// Token: 0x04000919 RID: 2329
	private bool isInitialLoad = true;

	// Token: 0x02000B99 RID: 2969
	[Serializable]
	public class RocketTube
	{
		// Token: 0x04003EE2 RID: 16098
		public Vector3 firingOffset;

		// Token: 0x04003EE3 RID: 16099
		public Transform hinge;

		// Token: 0x04003EE4 RID: 16100
		public Renderer rocket;
	}

	// Token: 0x02000B9A RID: 2970
	private struct TheoreticalProjectile
	{
		// Token: 0x06004AFE RID: 19198 RVA: 0x0019134D File Offset: 0x0018F54D
		public TheoreticalProjectile(Vector3 pos, Vector3 forward, float gravityMult)
		{
			this.pos = pos;
			this.forward = forward;
			this.gravityMult = gravityMult;
		}

		// Token: 0x04003EE5 RID: 16101
		public Vector3 pos;

		// Token: 0x04003EE6 RID: 16102
		public Vector3 forward;

		// Token: 0x04003EE7 RID: 16103
		public float gravityMult;
	}
}
