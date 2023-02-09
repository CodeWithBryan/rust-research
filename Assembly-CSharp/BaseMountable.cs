using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x0200003D RID: 61
public class BaseMountable : BaseCombatEntity
{
	// Token: 0x060003EE RID: 1006 RVA: 0x000317C0 File Offset: 0x0002F9C0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseMountable.OnRpcMessage", 0))
		{
			if (rpc == 1735799362U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsDismount ");
				}
				using (TimeWarning.New("RPC_WantsDismount", 0))
				{
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
							this.RPC_WantsDismount(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_WantsDismount");
					}
				}
				return true;
			}
			if (rpc == 4014300952U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsMount ");
				}
				using (TimeWarning.New("RPC_WantsMount", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4014300952U, "RPC_WantsMount", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_WantsMount(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_WantsMount");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x00031A74 File Offset: 0x0002FC74
	public virtual bool CanHoldItems()
	{
		return this.canWieldItems;
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x00031A7C File Offset: 0x0002FC7C
	public virtual BasePlayer.CameraMode GetMountedCameraMode()
	{
		return this.MountedCameraMode;
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool DirectlyMountable()
	{
		return true;
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x00031A84 File Offset: 0x0002FC84
	public virtual Transform GetEyeOverride()
	{
		if (this.eyePositionOverride != null)
		{
			return this.eyePositionOverride;
		}
		return base.transform;
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00031AA1 File Offset: 0x0002FCA1
	public virtual Quaternion GetMountedBodyAngles()
	{
		return this.GetEyeOverride().rotation;
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool ModifiesThirdPersonCamera()
	{
		return false;
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x00031AAE File Offset: 0x0002FCAE
	public virtual Vector2 GetPitchClamp()
	{
		return this.pitchClamp;
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x00031AB6 File Offset: 0x0002FCB6
	public virtual Vector2 GetYawClamp()
	{
		return this.yawClamp;
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x00031ABE File Offset: 0x0002FCBE
	public virtual bool AnyMounted()
	{
		return base.IsBusy();
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x00031AC6 File Offset: 0x0002FCC6
	public bool IsMounted()
	{
		return this.AnyMounted();
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00031ACE File Offset: 0x0002FCCE
	public virtual Vector3 EyePositionForPlayer(BasePlayer player, Quaternion lookRot)
	{
		if (player.GetMounted() != this)
		{
			return Vector3.zero;
		}
		return this.eyePositionOverride.transform.position;
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x00031AF4 File Offset: 0x0002FCF4
	public virtual Vector3 EyeCenterForPlayer(BasePlayer player, Quaternion lookRot)
	{
		if (player.GetMounted() != this)
		{
			return Vector3.zero;
		}
		return this.eyeCenterOverride.transform.position;
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00031B1C File Offset: 0x0002FD1C
	public virtual float WaterFactorForPlayer(BasePlayer player)
	{
		return WaterLevel.Factor(player.WorldSpaceBounds().ToBounds(), this);
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x00031B40 File Offset: 0x0002FD40
	public override float MaxVelocity()
	{
		BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity)
		{
			return parentEntity.MaxVelocity();
		}
		return base.MaxVelocity();
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x00031B69 File Offset: 0x0002FD69
	public virtual bool PlayerIsMounted(BasePlayer player)
	{
		return player.IsValid() && player.GetMounted() == this;
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x00031B81 File Offset: 0x0002FD81
	public virtual BaseVehicle VehicleParent()
	{
		if (this.ignoreVehicleParent)
		{
			return null;
		}
		return base.GetParentEntity() as BaseVehicle;
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x00031B98 File Offset: 0x0002FD98
	public virtual bool HasValidDismountPosition(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			return baseVehicle.HasValidDismountPosition(player);
		}
		foreach (Transform transform in this.dismountPositions)
		{
			if (this.ValidDismountPosition(player, transform.transform.position))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x00031BF0 File Offset: 0x0002FDF0
	public virtual bool ValidDismountPosition(BasePlayer player, Vector3 disPos)
	{
		bool debugDismounts = Debugging.DebugDismounts;
		Vector3 dismountCheckStart = this.GetDismountCheckStart(player);
		if (debugDismounts)
		{
			Debug.Log(string.Format("ValidDismountPosition debug: Checking dismount point {0} from {1}.", disPos, dismountCheckStart));
		}
		Vector3 start = disPos + new Vector3(0f, 0.5f, 0f);
		Vector3 end = disPos + new Vector3(0f, 1.3f, 0f);
		if (!UnityEngine.Physics.CheckCapsule(start, end, 0.5f, 1537286401))
		{
			Vector3 position = disPos + base.transform.up * 0.5f;
			if (debugDismounts)
			{
				Debug.Log(string.Format("ValidDismountPosition debug: Dismount point {0} capsule check is OK.", disPos));
			}
			if (base.IsVisible(position, float.PositiveInfinity))
			{
				Vector3 vector = disPos + player.NoClipOffset();
				if (debugDismounts)
				{
					Debug.Log(string.Format("ValidDismountPosition debug: Dismount point {0} is visible.", disPos));
				}
				if (!global::AntiHack.TestNoClipping(player, dismountCheckStart, vector, player.NoClipRadius(ConVar.AntiHack.noclip_margin_dismount), ConVar.AntiHack.noclip_backtracking, true, false, this.legacyDismount ? null : this))
				{
					if (debugDismounts)
					{
						Debug.Log(string.Format("<color=green>ValidDismountPosition debug: Dismount point {0} is valid</color>.", disPos));
						Debug.DrawLine(dismountCheckStart, vector, Color.green, 10f);
					}
					return true;
				}
			}
		}
		if (debugDismounts)
		{
			Debug.DrawLine(dismountCheckStart, disPos, Color.red, 10f);
			if (debugDismounts)
			{
				Debug.Log(string.Format("<color=red>ValidDismountPosition debug: Dismount point {0} is invalid</color>.", disPos));
			}
		}
		return false;
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00031D5D File Offset: 0x0002FF5D
	public BasePlayer GetMounted()
	{
		return this._mounted;
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void LightToggle(BasePlayer player)
	{
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool CanSwapToThis(BasePlayer player)
	{
		return true;
	}

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000405 RID: 1029 RVA: 0x00031D65 File Offset: 0x0002FF65
	protected override float PositionTickRate
	{
		get
		{
			return 0.05f;
		}
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00031D6C File Offset: 0x0002FF6C
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !this.AnyMounted();
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x00031D82 File Offset: 0x0002FF82
	public override void OnKilled(HitInfo info)
	{
		this.DismountAllPlayers();
		base.OnKilled(info);
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00031D91 File Offset: 0x0002FF91
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_WantsMount(BaseEntity.RPCMessage msg)
	{
		this.WantsMount(msg.player);
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00031DA0 File Offset: 0x0002FFA0
	public void WantsMount(BasePlayer player)
	{
		if (!player.IsValid() || !player.CanInteract())
		{
			return;
		}
		if (!this.DirectlyMountable())
		{
			BaseVehicle baseVehicle = this.VehicleParent();
			if (baseVehicle != null)
			{
				baseVehicle.WantsMount(player);
				return;
			}
		}
		this.AttemptMount(player, true);
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00031DE8 File Offset: 0x0002FFE8
	public virtual void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (this._mounted != null)
		{
			return;
		}
		if (this.IsDead())
		{
			return;
		}
		if (!player.CanMountMountablesNow())
		{
			return;
		}
		if (doMountChecks)
		{
			if (this.checkPlayerLosOnMount && UnityEngine.Physics.Linecast(player.eyes.position, this.mountAnchor.position + base.transform.up * 0.5f, 1218652417))
			{
				Debug.Log("No line of sight to mount pos");
				return;
			}
			if (!this.HasValidDismountPosition(player))
			{
				Debug.Log("no valid dismount");
				return;
			}
		}
		this.MountPlayer(player);
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00031E83 File Offset: 0x00030083
	public virtual bool AttemptDismount(BasePlayer player)
	{
		if (player != this._mounted)
		{
			return false;
		}
		this.DismountPlayer(player, false);
		return true;
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x00031EA0 File Offset: 0x000300A0
	[BaseEntity.RPC_Server]
	public void RPC_WantsDismount(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!this.HasValidDismountPosition(player))
		{
			return;
		}
		this.AttemptDismount(player);
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00031EC8 File Offset: 0x000300C8
	public void MountPlayer(BasePlayer player)
	{
		if (this._mounted != null)
		{
			return;
		}
		if (this.mountAnchor == null)
		{
			return;
		}
		player.EnsureDismounted();
		this._mounted = player;
		Transform transform = this.mountAnchor.transform;
		player.MountObject(this, 0);
		player.MovePosition(transform.position);
		player.transform.rotation = transform.rotation;
		player.ServerRotation = transform.rotation;
		player.OverrideViewAngles(transform.rotation.eulerAngles);
		this._mounted.eyes.NetworkUpdate(transform.rotation);
		player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", player.transform.position);
		this.OnPlayerMounted();
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00031F85 File Offset: 0x00030185
	public virtual void OnPlayerMounted()
	{
		this.UpdateMountFlags();
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00031F85 File Offset: 0x00030185
	public virtual void OnPlayerDismounted(BasePlayer player)
	{
		this.UpdateMountFlags();
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00031F90 File Offset: 0x00030190
	public virtual void UpdateMountFlags()
	{
		base.SetFlag(BaseEntity.Flags.Busy, this._mounted != null, false, true);
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			baseVehicle.UpdateMountFlags();
		}
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00031FCC File Offset: 0x000301CC
	public virtual void DismountAllPlayers()
	{
		if (this._mounted)
		{
			this.DismountPlayer(this._mounted, false);
		}
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00031FE8 File Offset: 0x000301E8
	public void DismountPlayer(BasePlayer player, bool lite = false)
	{
		if (this._mounted == null)
		{
			return;
		}
		if (this._mounted != player)
		{
			return;
		}
		BaseVehicle baseVehicle = this.VehicleParent();
		if (lite)
		{
			if (baseVehicle != null)
			{
				baseVehicle.PrePlayerDismount(player, this);
			}
			this._mounted.DismountObject();
			this._mounted = null;
			if (baseVehicle != null)
			{
				baseVehicle.PlayerDismounted(player, this);
			}
			this.OnPlayerDismounted(player);
			return;
		}
		Vector3 position;
		if (!this.GetDismountPosition(player, out position) || base.Distance(position) > 10f)
		{
			if (baseVehicle != null)
			{
				baseVehicle.PrePlayerDismount(player, this);
			}
			position = player.transform.position;
			this._mounted.DismountObject();
			this._mounted.MovePosition(position);
			this._mounted.ClientRPCPlayer<Vector3>(null, this._mounted, "ForcePositionTo", position);
			BasePlayer mounted = this._mounted;
			this._mounted = null;
			Debug.LogWarning(string.Concat(new object[]
			{
				"Killing player due to invalid dismount point :",
				player.displayName,
				" / ",
				player.userID,
				" on obj : ",
				base.gameObject.name
			}));
			mounted.Hurt(1000f, DamageType.Suicide, mounted, false);
			if (baseVehicle != null)
			{
				baseVehicle.PlayerDismounted(player, this);
			}
			this.OnPlayerDismounted(player);
			return;
		}
		if (baseVehicle != null)
		{
			baseVehicle.PrePlayerDismount(player, this);
		}
		this._mounted.DismountObject();
		this._mounted.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
		this._mounted.MovePosition(position);
		this._mounted.SendNetworkUpdateImmediate(false);
		this._mounted.SendModelState(true);
		this._mounted = null;
		if (baseVehicle != null)
		{
			baseVehicle.PlayerDismounted(player, this);
		}
		player.ForceUpdateTriggers(true, true, true);
		if (player.GetParentEntity())
		{
			BaseEntity parentEntity = player.GetParentEntity();
			player.ClientRPCPlayer<Vector3, uint>(null, player, "ForcePositionToParentOffset", parentEntity.transform.InverseTransformPoint(position), parentEntity.net.ID);
		}
		else
		{
			player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", position);
		}
		this.OnPlayerDismounted(player);
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00032214 File Offset: 0x00030414
	public virtual bool GetDismountPosition(BasePlayer player, out Vector3 res)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			return baseVehicle.GetDismountPosition(player, out res);
		}
		int num = 0;
		foreach (Transform transform in this.dismountPositions)
		{
			if (this.ValidDismountPosition(player, transform.transform.position))
			{
				res = transform.transform.position;
				return true;
			}
			num++;
		}
		Debug.LogWarning(string.Concat(new object[]
		{
			"Failed to find dismount position for player :",
			player.displayName,
			" / ",
			player.userID,
			" on obj : ",
			base.gameObject.name
		}));
		res = player.transform.position;
		return false;
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x000322E2 File Offset: 0x000304E2
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.isMobile)
		{
			BaseMountable.FixedUpdateMountables.Add(this);
		}
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x000322FD File Offset: 0x000304FD
	internal override void DoServerDestroy()
	{
		BaseMountable.FixedUpdateMountables.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00032314 File Offset: 0x00030514
	public static void FixedUpdateCycle()
	{
		for (int i = BaseMountable.FixedUpdateMountables.Count - 1; i >= 0; i--)
		{
			BaseMountable baseMountable = BaseMountable.FixedUpdateMountables[i];
			if (baseMountable == null)
			{
				BaseMountable.FixedUpdateMountables.RemoveAt(i);
			}
			else if (baseMountable.isSpawned)
			{
				baseMountable.VehicleFixedUpdate();
			}
		}
		for (int j = BaseMountable.FixedUpdateMountables.Count - 1; j >= 0; j--)
		{
			BaseMountable baseMountable2 = BaseMountable.FixedUpdateMountables[j];
			if (baseMountable2 == null)
			{
				BaseMountable.FixedUpdateMountables.RemoveAt(j);
			}
			else if (baseMountable2.isSpawned)
			{
				baseMountable2.PostVehicleFixedUpdate();
			}
		}
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x000323B0 File Offset: 0x000305B0
	public virtual void VehicleFixedUpdate()
	{
		if (this._mounted)
		{
			this._mounted.transform.rotation = this.mountAnchor.transform.rotation;
			this._mounted.ServerRotation = this.mountAnchor.transform.rotation;
			this._mounted.MovePosition(this.mountAnchor.transform.position);
		}
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void PostVehicleFixedUpdate()
	{
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void PlayerServerInput(InputState inputState, BasePlayer player)
	{
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00026FFC File Offset: 0x000251FC
	public virtual float GetComfort()
	{
		return 0f;
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x00032420 File Offset: 0x00030620
	public static bool TryFireProjectile(StorageContainer ammoStorage, AmmoTypes ammoType, Vector3 firingPos, Vector3 firingDir, BasePlayer driver, float launchOffset, float minSpeed, out ServerProjectile projectile)
	{
		projectile = null;
		if (ammoStorage == null)
		{
			return false;
		}
		bool result = false;
		List<Item> list = Facepunch.Pool.GetList<Item>();
		ammoStorage.inventory.FindAmmo(list, ammoType);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (list[i].amount <= 0)
			{
				list.RemoveAt(i);
			}
		}
		if (list.Count > 0)
		{
			RaycastHit raycastHit;
			if (UnityEngine.Physics.Raycast(firingPos, firingDir, out raycastHit, launchOffset, 1236478737))
			{
				launchOffset = raycastHit.distance - 0.1f;
			}
			Item item = list[list.Count - 1];
			ItemModProjectile component = item.info.GetComponent<ItemModProjectile>();
			BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, firingPos + firingDir * launchOffset, default(Quaternion), true);
			projectile = baseEntity.GetComponent<ServerProjectile>();
			Vector3 vector = projectile.initialVelocity + firingDir * projectile.speed;
			if (minSpeed > 0f)
			{
				float num = Vector3.Dot(vector, firingDir) - minSpeed;
				if (num < 0f)
				{
					vector += firingDir * -num;
				}
			}
			projectile.InitializeVelocity(vector);
			if (driver.IsValid())
			{
				baseEntity.creatorEntity = driver;
				baseEntity.OwnerID = driver.userID;
			}
			baseEntity.Spawn();
			item.UseItem(1);
			result = true;
		}
		Facepunch.Pool.FreeList<Item>(ref list);
		return result;
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x0600041D RID: 1053 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsSummerDlcVehicle
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsInstrument()
	{
		return false;
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x0003258C File Offset: 0x0003078C
	public Vector3 GetDismountCheckStart(BasePlayer player)
	{
		Vector3 vector = this.GetMountedPosition() + player.NoClipOffset();
		Vector3 a = (this.mountAnchor == null) ? base.transform.forward : this.mountAnchor.transform.forward;
		Vector3 a2 = (this.mountAnchor == null) ? base.transform.up : this.mountAnchor.transform.up;
		if (this.mountPose == PlayerModel.MountPoses.Chair)
		{
			vector += -a * 0.32f;
			vector += a2 * 0.25f;
		}
		else if (this.mountPose == PlayerModel.MountPoses.SitGeneric)
		{
			vector += -a * 0.26f;
			vector += a2 * 0.25f;
		}
		else if (this.mountPose == PlayerModel.MountPoses.SitGeneric)
		{
			vector += -a * 0.26f;
		}
		return vector;
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x0003268B File Offset: 0x0003088B
	public Vector3 GetMountedPosition()
	{
		if (this.mountAnchor == null)
		{
			return base.transform.position;
		}
		return this.mountAnchor.transform.position;
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x000326B8 File Offset: 0x000308B8
	public bool NearMountPoint(BasePlayer player)
	{
		if (player == null)
		{
			return false;
		}
		if (this.mountAnchor == null)
		{
			return false;
		}
		if (Vector3.Distance(player.transform.position, this.mountAnchor.position) <= this.maxMountDistance)
		{
			RaycastHit hit;
			if (!UnityEngine.Physics.SphereCast(player.eyes.HeadRay(), 0.25f, out hit, 2f, 1218652417))
			{
				return false;
			}
			BaseEntity entity = hit.GetEntity();
			if (entity != null)
			{
				if (entity == this || base.EqualNetID(entity))
				{
					return true;
				}
				BasePlayer basePlayer;
				if ((basePlayer = (entity as BasePlayer)) != null)
				{
					BaseMountable mounted = basePlayer.GetMounted();
					if (mounted == this)
					{
						return true;
					}
					if (mounted != null && mounted.VehicleParent() == this)
					{
						return true;
					}
				}
				BaseEntity parentEntity = entity.GetParentEntity();
				if (hit.IsOnLayer(Rust.Layer.Vehicle_Detailed) && (parentEntity == this || base.EqualNetID(parentEntity)))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x000327B4 File Offset: 0x000309B4
	public static Vector3 ConvertVector(Vector3 vec)
	{
		for (int i = 0; i < 3; i++)
		{
			if (vec[i] > 180f)
			{
				ref Vector3 ptr = ref vec;
				int index = i;
				ptr[index] -= 360f;
			}
			else if (vec[i] < -180f)
			{
				ref Vector3 ptr = ref vec;
				int index = i;
				ptr[index] += 360f;
			}
		}
		return vec;
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000423 RID: 1059 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool BlocksDoors
	{
		get
		{
			return true;
		}
	}

	// Token: 0x04000314 RID: 788
	public static Translate.Phrase dismountPhrase = new Translate.Phrase("dismount", "Dismount");

	// Token: 0x04000315 RID: 789
	[Header("View")]
	[FormerlySerializedAs("eyeOverride")]
	public Transform eyePositionOverride;

	// Token: 0x04000316 RID: 790
	[FormerlySerializedAs("eyeOverride")]
	public Transform eyeCenterOverride;

	// Token: 0x04000317 RID: 791
	public Vector2 pitchClamp = new Vector2(-80f, 50f);

	// Token: 0x04000318 RID: 792
	public Vector2 yawClamp = new Vector2(-80f, 80f);

	// Token: 0x04000319 RID: 793
	public bool canWieldItems = true;

	// Token: 0x0400031A RID: 794
	public bool relativeViewAngles = true;

	// Token: 0x0400031B RID: 795
	[Header("Mounting")]
	public Transform mountAnchor;

	// Token: 0x0400031C RID: 796
	public PlayerModel.MountPoses mountPose;

	// Token: 0x0400031D RID: 797
	public float maxMountDistance = 1.5f;

	// Token: 0x0400031E RID: 798
	public Transform[] dismountPositions;

	// Token: 0x0400031F RID: 799
	public bool checkPlayerLosOnMount;

	// Token: 0x04000320 RID: 800
	public bool disableMeshCullingForPlayers;

	// Token: 0x04000321 RID: 801
	public bool allowHeadLook;

	// Token: 0x04000322 RID: 802
	public bool ignoreVehicleParent;

	// Token: 0x04000323 RID: 803
	public bool legacyDismount;

	// Token: 0x04000324 RID: 804
	[FormerlySerializedAs("modifyPlayerCollider")]
	public bool modifiesPlayerCollider;

	// Token: 0x04000325 RID: 805
	public BasePlayer.CapsuleColliderInfo customPlayerCollider;

	// Token: 0x04000326 RID: 806
	public SoundDefinition mountSoundDef;

	// Token: 0x04000327 RID: 807
	public SoundDefinition swapSoundDef;

	// Token: 0x04000328 RID: 808
	public SoundDefinition dismountSoundDef;

	// Token: 0x04000329 RID: 809
	public BaseMountable.MountStatType mountTimeStatType;

	// Token: 0x0400032A RID: 810
	public BaseMountable.MountGestureType allowedGestures;

	// Token: 0x0400032B RID: 811
	public bool canDrinkWhileMounted = true;

	// Token: 0x0400032C RID: 812
	public bool allowSleeperMounting;

	// Token: 0x0400032D RID: 813
	[Help("Set this to true if the mountable is enclosed so it doesn't move inside cars and such")]
	public bool animateClothInLocalSpace = true;

	// Token: 0x0400032E RID: 814
	[Header("Camera")]
	public BasePlayer.CameraMode MountedCameraMode;

	// Token: 0x0400032F RID: 815
	[FormerlySerializedAs("needsVehicleTick")]
	public bool isMobile;

	// Token: 0x04000330 RID: 816
	public float SideLeanAmount = 0.2f;

	// Token: 0x04000331 RID: 817
	public const float playerHeight = 1.8f;

	// Token: 0x04000332 RID: 818
	public const float playerRadius = 0.5f;

	// Token: 0x04000333 RID: 819
	protected BasePlayer _mounted;

	// Token: 0x04000334 RID: 820
	public static ListHashSet<BaseMountable> FixedUpdateMountables = new ListHashSet<BaseMountable>(8);

	// Token: 0x02000B4B RID: 2891
	public enum MountStatType
	{
		// Token: 0x04003D64 RID: 15716
		None,
		// Token: 0x04003D65 RID: 15717
		Boating,
		// Token: 0x04003D66 RID: 15718
		Flying,
		// Token: 0x04003D67 RID: 15719
		Driving
	}

	// Token: 0x02000B4C RID: 2892
	public enum MountGestureType
	{
		// Token: 0x04003D69 RID: 15721
		None,
		// Token: 0x04003D6A RID: 15722
		UpperBody
	}
}
