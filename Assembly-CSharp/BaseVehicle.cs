using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x02000045 RID: 69
public class BaseVehicle : BaseMountable
{
	// Token: 0x06000783 RID: 1923 RVA: 0x0004B4F8 File Offset: 0x000496F8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseVehicle.OnRpcMessage", 0))
		{
			if (rpc == 2115395408U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsPush ");
				}
				using (TimeWarning.New("RPC_WantsPush", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2115395408U, "RPC_WantsPush", this, player, 5f))
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
							this.RPC_WantsPush(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_WantsPush");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06000784 RID: 1924 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool AlwaysAllowBradleyTargeting
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06000785 RID: 1925 RVA: 0x0004B660 File Offset: 0x00049860
	protected bool RecentlyPushed
	{
		get
		{
			return this.timeSinceLastPush < 1f;
		}
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x0004B674 File Offset: 0x00049874
	public override void OnAttacked(HitInfo info)
	{
		if (this.IsSafe() && !info.damageTypes.Has(DamageType.Decay))
		{
			info.damageTypes.ScaleAll(0f);
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x0004B6A4 File Offset: 0x000498A4
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.ClearOwnerEntry();
		this.CheckAndSpawnMountPoints();
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x0004B6B8 File Offset: 0x000498B8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (base.isServer && info.forDisk)
		{
			info.msg.baseVehicle = Facepunch.Pool.Get<ProtoBuf.BaseVehicle>();
			info.msg.baseVehicle.mountPoints = Facepunch.Pool.GetList<ProtoBuf.BaseVehicle.MountPoint>();
			for (int i = 0; i < this.mountPoints.Count; i++)
			{
				global::BaseVehicle.MountPointInfo mountPointInfo = this.mountPoints[i];
				if (!(mountPointInfo.mountable == null))
				{
					ProtoBuf.BaseVehicle.MountPoint mountPoint = Facepunch.Pool.Get<ProtoBuf.BaseVehicle.MountPoint>();
					mountPoint.index = i;
					mountPoint.mountableId = mountPointInfo.mountable.net.ID;
					info.msg.baseVehicle.mountPoints.Add(mountPoint);
				}
			}
		}
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x0004B774 File Offset: 0x00049974
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer && info.fromDisk && info.msg.baseVehicle != null)
		{
			ProtoBuf.BaseVehicle baseVehicle = this.pendingLoad;
			if (baseVehicle != null)
			{
				baseVehicle.Dispose();
			}
			this.pendingLoad = info.msg.baseVehicle;
			info.msg.baseVehicle = null;
		}
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x000058B6 File Offset: 0x00003AB6
	public override float GetNetworkTime()
	{
		return UnityEngine.Time.fixedTime;
	}

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x0600078B RID: 1931 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool PositionTickFixedTime
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x0004B7D4 File Offset: 0x000499D4
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.clippingChecks != global::BaseVehicle.ClippingCheckMode.OnMountOnly && this.AnyMounted() && UnityEngine.Physics.OverlapBox(base.transform.TransformPoint(this.bounds.center), this.bounds.extents, base.transform.rotation, this.GetClipCheckMask()).Length != 0)
		{
			this.CheckSeatsForClipping();
		}
		if (this.rigidBody != null)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved7, this.DetermineIfStationary(), false, true);
			bool flag = this.rigidBody.IsSleeping();
			if (this.prevSleeping && !flag)
			{
				this.OnServerWake();
			}
			else if (!this.prevSleeping && flag)
			{
				this.OnServerSleep();
			}
			this.prevSleeping = flag;
		}
		if (this.OnlyOwnerAccessible() && this.safeAreaRadius != -1f && Vector3.Distance(base.transform.position, this.safeAreaOrigin) > this.safeAreaRadius)
		{
			this.ClearOwnerEntry();
		}
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x0004B8CC File Offset: 0x00049ACC
	private int GetClipCheckMask()
	{
		int num = this.IsFlipped() ? 1218511105 : 1210122497;
		if (this.checkVehicleClipping)
		{
			num |= 8192;
		}
		return num;
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x0004B8FF File Offset: 0x00049AFF
	protected virtual bool DetermineIfStationary()
	{
		return this.rigidBody.IsSleeping() && !this.AnyMounted();
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x0004B919 File Offset: 0x00049B19
	public override Vector3 GetLocalVelocityServer()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.velocity;
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x0004B93C File Offset: 0x00049B3C
	public override Quaternion GetAngularVelocityServer()
	{
		if (this.rigidBody == null)
		{
			return Quaternion.identity;
		}
		if (this.rigidBody.angularVelocity.sqrMagnitude < 0.025f)
		{
			return Quaternion.identity;
		}
		return Quaternion.LookRotation(this.rigidBody.angularVelocity, base.transform.up);
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x00040DA9 File Offset: 0x0003EFA9
	public virtual int StartingFuelUnits()
	{
		return -1;
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x0004B998 File Offset: 0x00049B98
	public bool InSafeZone()
	{
		return global::BaseVehicle.InSafeZone(this.triggers, base.transform.position);
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x0004B9B0 File Offset: 0x00049BB0
	public static bool InSafeZone(List<TriggerBase> triggers, Vector3 position)
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null && !activeGameMode.safeZone)
		{
			return false;
		}
		float num = 0f;
		if (triggers != null)
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerSafeZone triggerSafeZone = triggers[i] as TriggerSafeZone;
				if (!(triggerSafeZone == null))
				{
					float safeLevel = triggerSafeZone.GetSafeLevel(position);
					if (safeLevel > num)
					{
						num = safeLevel;
					}
				}
			}
		}
		return num > 0f;
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x0004BA20 File Offset: 0x00049C20
	public virtual bool IsSeatVisible(BaseMountable mountable, Vector3 eyePos, int mask = 1218511105)
	{
		if (!this.doClippingAndVisChecks)
		{
			return true;
		}
		if (mountable == null)
		{
			return false;
		}
		Vector3 p = mountable.transform.position + base.transform.up * 0.15f;
		return GamePhysics.LineOfSight(eyePos, p, mask, null);
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x0004BA74 File Offset: 0x00049C74
	public virtual bool IsSeatClipping(BaseMountable mountable)
	{
		if (!this.doClippingAndVisChecks)
		{
			return false;
		}
		if (mountable == null)
		{
			return false;
		}
		int clipCheckMask = this.GetClipCheckMask();
		Vector3 position = mountable.eyePositionOverride.transform.position;
		Vector3 position2 = mountable.transform.position;
		Vector3 a = position - position2;
		float num = 0.4f;
		if (mountable.modifiesPlayerCollider)
		{
			num = Mathf.Min(num, mountable.customPlayerCollider.radius);
		}
		Vector3 vector = position - a * (num - 0.2f);
		bool result = false;
		if (this.checkVehicleClipping)
		{
			List<Collider> list = Facepunch.Pool.GetList<Collider>();
			if (this.clippingChecks == global::BaseVehicle.ClippingCheckMode.AlwaysHeadOnly)
			{
				GamePhysics.OverlapSphere(vector, num, list, clipCheckMask, QueryTriggerInteraction.Ignore);
			}
			else
			{
				Vector3 point = position2 + a * (num + 0.05f);
				GamePhysics.OverlapCapsule(vector, point, num, list, clipCheckMask, QueryTriggerInteraction.Ignore);
			}
			foreach (Collider collider in list)
			{
				global::BaseEntity baseEntity = collider.ToBaseEntity();
				if (baseEntity != this && !base.EqualNetID(baseEntity))
				{
					result = true;
					break;
				}
			}
			Facepunch.Pool.FreeList<Collider>(ref list);
		}
		else if (this.clippingChecks == global::BaseVehicle.ClippingCheckMode.AlwaysHeadOnly)
		{
			result = GamePhysics.CheckSphere(vector, num, clipCheckMask, QueryTriggerInteraction.Ignore);
		}
		else
		{
			Vector3 end = position2 + a * (num + 0.05f);
			result = GamePhysics.CheckCapsule(vector, end, num, clipCheckMask, QueryTriggerInteraction.Ignore);
		}
		return result;
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x0004BBE4 File Offset: 0x00049DE4
	public virtual void CheckSeatsForClipping()
	{
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			BaseMountable mountable = mountPointInfo.mountable;
			if (!(mountable == null) && mountable.AnyMounted() && this.IsSeatClipping(mountable))
			{
				this.SeatClippedWorld(mountable);
			}
		}
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0004BC58 File Offset: 0x00049E58
	public virtual void SeatClippedWorld(BaseMountable mountable)
	{
		mountable.DismountPlayer(mountable.GetMounted(), false);
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void MounteeTookDamage(global::BasePlayer mountee, HitInfo info)
	{
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x0004BC68 File Offset: 0x00049E68
	public override void DismountAllPlayers()
	{
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				mountPointInfo.mountable.DismountAllPlayers();
			}
		}
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x0004BCD0 File Offset: 0x00049ED0
	public override void ServerInit()
	{
		base.ServerInit();
		this.clearRecentDriverAction = new Action(this.ClearRecentDriver);
		this.prevSleeping = false;
		if (this.rigidBody != null)
		{
			this.savedCollisionDetectionMode = this.rigidBody.collisionDetectionMode;
		}
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x0004BD10 File Offset: 0x00049F10
	public virtual void SpawnSubEntities()
	{
		this.CheckAndSpawnMountPoints();
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x0004BD18 File Offset: 0x00049F18
	public virtual bool AdminFixUp(int tier)
	{
		if (this.IsDead())
		{
			return false;
		}
		EntityFuelSystem fuelSystem = this.GetFuelSystem();
		if (fuelSystem != null)
		{
			fuelSystem.AdminAddFuel();
		}
		base.SetHealth(this.MaxHealth());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x0004BD53 File Offset: 0x00049F53
	private void OnPhysicsNeighbourChanged()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
		}
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x0004BD70 File Offset: 0x00049F70
	private void CheckAndSpawnMountPoints()
	{
		ProtoBuf.BaseVehicle baseVehicle = this.pendingLoad;
		if (((baseVehicle != null) ? baseVehicle.mountPoints : null) != null)
		{
			foreach (ProtoBuf.BaseVehicle.MountPoint mountPoint in this.pendingLoad.mountPoints)
			{
				EntityRef<BaseMountable> entityRef = new EntityRef<BaseMountable>(mountPoint.mountableId);
				if (!entityRef.IsValid(true))
				{
					Debug.LogError(string.Format("Loaded a mountpoint which doesn't exist: {0}", mountPoint.index), this);
				}
				else if (mountPoint.index < 0 || mountPoint.index >= this.mountPoints.Count)
				{
					Debug.LogError(string.Format("Loaded a mountpoint which has no info: {0}", mountPoint.index), this);
					entityRef.Get(true).Kill(global::BaseNetworkable.DestroyMode.None);
				}
				else
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = this.mountPoints[mountPoint.index];
					if (mountPointInfo.mountable != null)
					{
						Debug.LogError(string.Format("Loading a mountpoint after one was already set: {0}", mountPoint.index), this);
						mountPointInfo.mountable.Kill(global::BaseNetworkable.DestroyMode.None);
					}
					mountPointInfo.mountable = entityRef.Get(true);
					if (!mountPointInfo.mountable.enableSaving)
					{
						mountPointInfo.mountable.EnableSaving(true);
					}
				}
			}
		}
		ProtoBuf.BaseVehicle baseVehicle2 = this.pendingLoad;
		if (baseVehicle2 != null)
		{
			baseVehicle2.Dispose();
		}
		this.pendingLoad = null;
		for (int i = 0; i < this.mountPoints.Count; i++)
		{
			this.SpawnMountPoint(this.mountPoints[i], this.model);
		}
		this.UpdateMountFlags();
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x0004BF28 File Offset: 0x0004A128
	public override void Spawn()
	{
		base.Spawn();
		if (base.isServer && !Rust.Application.isLoadingSave)
		{
			this.SpawnSubEntities();
		}
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x0004BF48 File Offset: 0x0004A148
	public override void Hurt(HitInfo info)
	{
		if (!this.IsDead() && this.rigidBody != null && !this.rigidBody.isKinematic)
		{
			float num = info.damageTypes.Get(DamageType.Explosion) + info.damageTypes.Get(DamageType.AntiVehicle);
			if (num > 3f)
			{
				float explosionForce = Mathf.Min(num * this.explosionForceMultiplier, this.explosionForceMax);
				this.rigidBody.AddExplosionForce(explosionForce, info.HitPositionWorld, 1f, 2.5f);
			}
		}
		base.Hurt(info);
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x0004BFD4 File Offset: 0x0004A1D4
	public int NumMounted()
	{
		if (this.HasMountPoints())
		{
			int num = 0;
			foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
			{
				if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() != null)
				{
					num++;
				}
			}
			return num;
		}
		if (!this.AnyMounted())
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x0004C060 File Offset: 0x0004A260
	public int MaxMounted()
	{
		if (!this.HasMountPoints())
		{
			return 1;
		}
		int num = 0;
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.mountable != null)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x0004C0CC File Offset: 0x0004A2CC
	public bool HasDriver()
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver && mountPointInfo.mountable.AnyMounted())
					{
						return true;
					}
				}
				return false;
			}
		}
		return base.AnyMounted();
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0004C158 File Offset: 0x0004A358
	public bool IsDriver(global::BasePlayer player)
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver)
					{
						global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
						if (mounted != null && mounted == player)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		if (this._mounted != null)
		{
			return this._mounted == player;
		}
		return false;
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0004C20C File Offset: 0x0004A40C
	public global::BasePlayer GetDriver()
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver)
					{
						global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
						if (mounted != null)
						{
							return mounted;
						}
					}
				}
				goto IL_82;
			}
		}
		if (this._mounted != null)
		{
			return this._mounted;
		}
		IL_82:
		return null;
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0004C2B0 File Offset: 0x0004A4B0
	public void GetDrivers(List<global::BasePlayer> drivers)
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver)
					{
						global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
						if (mounted != null)
						{
							drivers.Add(mounted);
						}
					}
				}
				return;
			}
		}
		if (this._mounted != null)
		{
			drivers.Add(this._mounted);
		}
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0004C358 File Offset: 0x0004A558
	public global::BasePlayer GetPlayerDamageInitiator()
	{
		if (this.HasDriver())
		{
			return this.GetDriver();
		}
		if (this.recentDrivers.Count <= 0)
		{
			return null;
		}
		return this.recentDrivers.Peek();
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x0004C384 File Offset: 0x0004A584
	public int GetPlayerSeat(global::BasePlayer player)
	{
		if (!this.HasMountPoints() && base.GetMounted() == player)
		{
			return 0;
		}
		int num = 0;
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() == player)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x0004C41C File Offset: 0x0004A61C
	public global::BaseVehicle.MountPointInfo GetPlayerSeatInfo(global::BasePlayer player)
	{
		if (!this.HasMountPoints())
		{
			return null;
		}
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() == player)
			{
				return mountPointInfo;
			}
		}
		return null;
	}

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x060007AA RID: 1962 RVA: 0x00003A54 File Offset: 0x00001C54
	protected virtual bool CanSwapSeats
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x0004C4A0 File Offset: 0x0004A6A0
	public void SwapSeats(global::BasePlayer player, int targetSeat = 0)
	{
		if (!this.HasMountPoints() || !this.CanSwapSeats)
		{
			return;
		}
		int playerSeat = this.GetPlayerSeat(player);
		if (playerSeat == -1)
		{
			return;
		}
		BaseMountable mountable = this.GetMountPoint(playerSeat).mountable;
		int num = playerSeat;
		BaseMountable baseMountable = null;
		if (baseMountable == null)
		{
			int num2 = this.MaxMounted();
			for (int i = 0; i < num2; i++)
			{
				num++;
				if (num >= num2)
				{
					num = 0;
				}
				global::BaseVehicle.MountPointInfo mountPoint = this.GetMountPoint(num);
				if (((mountPoint != null) ? mountPoint.mountable : null) != null && !mountPoint.mountable.AnyMounted() && mountPoint.mountable.CanSwapToThis(player) && !this.IsSeatClipping(mountPoint.mountable) && this.IsSeatVisible(mountPoint.mountable, player.eyes.position, 1218511105))
				{
					baseMountable = mountPoint.mountable;
					break;
				}
			}
		}
		if (baseMountable != null && baseMountable != mountable)
		{
			mountable.DismountPlayer(player, true);
			baseMountable.MountPlayer(player);
			player.MarkSwapSeat();
		}
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x0004C5B0 File Offset: 0x0004A7B0
	public bool HasDriverMountPoints()
	{
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
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

	// Token: 0x060007AD RID: 1965 RVA: 0x0002783E File Offset: 0x00025A3E
	public bool OnlyOwnerAccessible()
	{
		return base.HasFlag(global::BaseEntity.Flags.Locked);
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x0004C60C File Offset: 0x0004A80C
	public bool IsDespawnEligable()
	{
		return this.spawnTime == -1f || this.spawnTime + 300f < UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x0004C630 File Offset: 0x0004A830
	public void SetupOwner(global::BasePlayer owner, Vector3 newSafeAreaOrigin, float newSafeAreaRadius)
	{
		if (owner != null)
		{
			this.creatorEntity = owner;
			base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
			this.safeAreaRadius = newSafeAreaRadius;
			this.safeAreaOrigin = newSafeAreaOrigin;
			this.spawnTime = UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x0004C666 File Offset: 0x0004A866
	public void ClearOwnerEntry()
	{
		this.creatorEntity = null;
		base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
		this.safeAreaRadius = -1f;
		this.safeAreaOrigin = Vector3.zero;
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x0002A0CF File Offset: 0x000282CF
	public virtual EntityFuelSystem GetFuelSystem()
	{
		return null;
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x0004C690 File Offset: 0x0004A890
	public bool IsSafe()
	{
		return this.OnlyOwnerAccessible() && Vector3.Distance(this.safeAreaOrigin, base.transform.position) <= this.safeAreaRadius;
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x0004C6BD File Offset: 0x0004A8BD
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		if (this.IsSafe())
		{
			info.damageTypes.ScaleAll(0f);
		}
		base.ScaleDamageForPlayer(player, info);
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x0004C6E0 File Offset: 0x0004A8E0
	public BaseMountable GetIdealMountPoint(Vector3 eyePos, Vector3 pos, global::BasePlayer playerFor = null)
	{
		if (playerFor == null)
		{
			return null;
		}
		if (!this.HasMountPoints())
		{
			return this;
		}
		global::BasePlayer basePlayer = this.creatorEntity as global::BasePlayer;
		bool flag = basePlayer != null;
		bool flag2 = flag && basePlayer.Team != null;
		bool flag3 = flag && playerFor == basePlayer;
		if (!flag3 && flag && this.OnlyOwnerAccessible() && playerFor != null && (playerFor.Team == null || !playerFor.Team.members.Contains(basePlayer.userID)))
		{
			return null;
		}
		BaseMountable result = null;
		float num = float.PositiveInfinity;
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (!mountPointInfo.mountable.AnyMounted())
			{
				float num2 = Vector3.Distance(mountPointInfo.mountable.mountAnchor.position, pos);
				if (num2 <= num)
				{
					if (this.IsSeatClipping(mountPointInfo.mountable))
					{
						if (UnityEngine.Application.isEditor)
						{
							Debug.Log(string.Format("Skipping seat {0} - it's clipping", mountPointInfo.mountable));
						}
					}
					else if (!this.IsSeatVisible(mountPointInfo.mountable, eyePos, 1218511105))
					{
						if (UnityEngine.Application.isEditor)
						{
							Debug.Log(string.Format("Skipping seat {0} - it's not visible", mountPointInfo.mountable));
						}
					}
					else if (!this.OnlyOwnerAccessible() || !flag3 || flag2 || mountPointInfo.isDriver)
					{
						result = mountPointInfo.mountable;
						num = num2;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x0004C884 File Offset: 0x0004AA84
	public virtual bool MountEligable(global::BasePlayer player)
	{
		if (this.creatorEntity != null && this.OnlyOwnerAccessible() && player != this.creatorEntity)
		{
			global::BasePlayer basePlayer = this.creatorEntity as global::BasePlayer;
			if (basePlayer != null && basePlayer.Team != null && !basePlayer.Team.members.Contains(player.userID))
			{
				return false;
			}
		}
		global::BaseVehicle baseVehicle = this.VehicleParent();
		return !(baseVehicle != null) || baseVehicle.MountEligable(player);
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x0004C908 File Offset: 0x0004AB08
	public int GetIndexFromSeat(BaseMountable seat)
	{
		int num = 0;
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.mountable == seat)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void PlayerMounted(global::BasePlayer player, BaseMountable seat)
	{
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void PrePlayerDismount(global::BasePlayer player, BaseMountable seat)
	{
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x0004C970 File Offset: 0x0004AB70
	public virtual void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		this.recentDrivers.Enqueue(player);
		if (!base.IsInvoking(this.clearRecentDriverAction))
		{
			base.Invoke(this.clearRecentDriverAction, 3f);
		}
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x0004C9A0 File Offset: 0x0004ABA0
	public void TryShowCollisionFX(Collision collision, GameObjectRef effectGO)
	{
		this.TryShowCollisionFX(collision.GetContact(0).point, effectGO);
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x0004C9C4 File Offset: 0x0004ABC4
	public void TryShowCollisionFX(Vector3 contactPoint, GameObjectRef effectGO)
	{
		if (UnityEngine.Time.time < this.nextCollisionFXTime)
		{
			return;
		}
		this.nextCollisionFXTime = UnityEngine.Time.time + 0.25f;
		if (effectGO.isValid)
		{
			contactPoint += (base.transform.position - contactPoint) * 0.25f;
			Effect.server.Run(effectGO.resourcePath, contactPoint, base.transform.up, null, false);
		}
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0004CA34 File Offset: 0x0004AC34
	public void SetToKinematic()
	{
		if (this.rigidBody == null || this.rigidBody.isKinematic)
		{
			return;
		}
		this.savedCollisionDetectionMode = this.rigidBody.collisionDetectionMode;
		this.rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		this.rigidBody.isKinematic = true;
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x0004CA86 File Offset: 0x0004AC86
	public void SetToNonKinematic()
	{
		if (this.rigidBody == null || !this.rigidBody.isKinematic)
		{
			return;
		}
		this.rigidBody.isKinematic = false;
		this.rigidBody.collisionDetectionMode = this.savedCollisionDetectionMode;
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0004CAC4 File Offset: 0x0004ACC4
	public override void UpdateMountFlags()
	{
		int num = this.NumMounted();
		base.SetFlag(global::BaseEntity.Flags.InUse, num > 0, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved11, num == this.MaxMounted(), false, true);
		global::BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			baseVehicle.UpdateMountFlags();
		}
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x0004CB15 File Offset: 0x0004AD15
	private void ClearRecentDriver()
	{
		if (this.recentDrivers.Count > 0)
		{
			this.recentDrivers.Dequeue();
		}
		if (this.recentDrivers.Count > 0)
		{
			base.Invoke(this.clearRecentDriverAction, 3f);
		}
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x0004CB50 File Offset: 0x0004AD50
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (this._mounted != null)
		{
			return;
		}
		if (!this.MountEligable(player))
		{
			return;
		}
		BaseMountable idealMountPointFor = this.GetIdealMountPointFor(player);
		if (idealMountPointFor == null)
		{
			return;
		}
		if (idealMountPointFor == this)
		{
			base.AttemptMount(player, doMountChecks);
		}
		else
		{
			idealMountPointFor.AttemptMount(player, doMountChecks);
		}
		if (player.GetMountedVehicle() == this)
		{
			this.PlayerMounted(player, idealMountPointFor);
		}
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x0004CBB9 File Offset: 0x0004ADB9
	protected BaseMountable GetIdealMountPointFor(global::BasePlayer player)
	{
		return this.GetIdealMountPoint(player.eyes.position, player.eyes.position + player.eyes.HeadForward() * 1f, player);
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x0004CBF4 File Offset: 0x0004ADF4
	public override bool GetDismountPosition(global::BasePlayer player, out Vector3 res)
	{
		global::BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			return baseVehicle.GetDismountPosition(player, out res);
		}
		List<Vector3> list = Facepunch.Pool.GetList<Vector3>();
		foreach (Transform transform in this.dismountPositions)
		{
			if (this.ValidDismountPosition(player, transform.transform.position))
			{
				list.Add(transform.transform.position);
				if (this.dismountStyle == global::BaseVehicle.DismountStyle.Ordered)
				{
					break;
				}
			}
		}
		if (list.Count == 0)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Failed to find dismount position for player :",
				player.displayName,
				" / ",
				player.userID,
				" on obj : ",
				base.gameObject.name
			}));
			Facepunch.Pool.FreeList<Vector3>(ref list);
			res = player.transform.position;
			return false;
		}
		Vector3 pos = player.transform.position;
		list.Sort((Vector3 a, Vector3 b) => Vector3.Distance(a, pos).CompareTo(Vector3.Distance(b, pos)));
		res = list[0];
		Facepunch.Pool.FreeList<Vector3>(ref list);
		return true;
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x0004CD1C File Offset: 0x0004AF1C
	private BaseMountable SpawnMountPoint(global::BaseVehicle.MountPointInfo mountToSpawn, Model model)
	{
		if (mountToSpawn.mountable != null)
		{
			return mountToSpawn.mountable;
		}
		Vector3 vector = Quaternion.Euler(mountToSpawn.rot) * Vector3.forward;
		Vector3 pos = mountToSpawn.pos;
		Vector3 up = Vector3.up;
		if (mountToSpawn.bone != "")
		{
			pos = model.FindBone(mountToSpawn.bone).transform.position + base.transform.TransformDirection(mountToSpawn.pos);
			vector = base.transform.TransformDirection(vector);
			up = base.transform.up;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(mountToSpawn.prefab.resourcePath, pos, Quaternion.LookRotation(vector, up), true);
		BaseMountable baseMountable = baseEntity as BaseMountable;
		if (baseMountable != null)
		{
			if (!baseMountable.enableSaving)
			{
				baseMountable.EnableSaving(true);
			}
			if (mountToSpawn.bone != "")
			{
				baseMountable.SetParent(this, mountToSpawn.bone, true, true);
			}
			else
			{
				baseMountable.SetParent(this, false, false);
			}
			baseMountable.Spawn();
			mountToSpawn.mountable = baseMountable;
		}
		else
		{
			Debug.LogError("MountPointInfo prefab is not a BaseMountable. Cannot spawn mount point.");
			if (baseEntity != null)
			{
				baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
		return baseMountable;
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x0004CE58 File Offset: 0x0004B058
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(5f)]
	public void RPC_WantsPush(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player.isMounted)
		{
			return;
		}
		if (this.RecentlyPushed)
		{
			return;
		}
		if (!this.CanPushNow(player))
		{
			return;
		}
		if (this.rigidBody == null)
		{
			return;
		}
		if (this.OnlyOwnerAccessible() && player != this.creatorEntity)
		{
			return;
		}
		player.metabolism.calories.Subtract(3f);
		player.metabolism.SendChangesToClient();
		if (this.rigidBody.IsSleeping())
		{
			this.rigidBody.WakeUp();
		}
		this.DoPushAction(player);
		this.timeSinceLastPush = 0f;
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x0004CF00 File Offset: 0x0004B100
	protected virtual void DoPushAction(global::BasePlayer player)
	{
		if (this.rigidBody == null)
		{
			return;
		}
		if (this.IsFlipped())
		{
			float d = this.rigidBody.mass * 8f;
			Vector3 vector = Vector3.forward * d;
			if (Vector3.Dot(base.transform.InverseTransformVector(base.transform.position - player.transform.position), Vector3.right) > 0f)
			{
				vector *= -1f;
			}
			if (base.transform.up.y < 0f)
			{
				vector *= -1f;
			}
			this.rigidBody.AddRelativeTorque(vector, ForceMode.Impulse);
			return;
		}
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.position - player.eyes.position, base.transform.up).normalized;
		float d2 = this.rigidBody.mass * 4f;
		this.rigidBody.AddForce(normalized * d2, ForceMode.Impulse);
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnServerWake()
	{
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnServerSleep()
	{
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x00047C77 File Offset: 0x00045E77
	public bool IsStationary()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved7);
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x0004D013 File Offset: 0x0004B213
	public bool IsMoving()
	{
		return !base.HasFlag(global::BaseEntity.Flags.Reserved7);
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x060007CA RID: 1994 RVA: 0x0004D023 File Offset: 0x0004B223
	public bool IsMovingOrOn
	{
		get
		{
			return this.IsMoving() || base.IsOn();
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x060007CB RID: 1995 RVA: 0x0004D035 File Offset: 0x0004B235
	public override float RealisticMass
	{
		get
		{
			if (this.rigidBody != null)
			{
				return this.rigidBody.mass;
			}
			return base.RealisticMass;
		}
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0004D057 File Offset: 0x0004B257
	public override bool AnyMounted()
	{
		return base.HasFlag(global::BaseEntity.Flags.InUse);
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0004D064 File Offset: 0x0004B264
	public override bool PlayerIsMounted(global::BasePlayer player)
	{
		return player.IsValid() && player.GetMountedVehicle() == this;
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0004D07C File Offset: 0x0004B27C
	protected virtual bool CanPushNow(global::BasePlayer pusher)
	{
		return !base.IsOn();
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x0004D088 File Offset: 0x0004B288
	public bool HasMountPoints()
	{
		if (this.mountPoints.Count > 0)
		{
			return true;
		}
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0004D0F0 File Offset: 0x0004B2F0
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return this.IsAlive() && !base.IsDestroyed && player != null;
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0004D10B File Offset: 0x0004B30B
	public bool IsFlipped()
	{
		return Vector3.Dot(Vector3.up, base.transform.up) <= 0f;
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool IsVehicleRoot()
	{
		return true;
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0004D12C File Offset: 0x0004B32C
	public override bool DirectlyMountable()
	{
		return this.IsVehicleRoot();
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0002A0CF File Offset: 0x000282CF
	public override global::BaseVehicle VehicleParent()
	{
		return null;
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x0004D134 File Offset: 0x0004B334
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (this.IsDead() || base.IsDestroyed)
		{
			return;
		}
		global::BaseVehicle baseVehicle;
		if ((baseVehicle = (child as global::BaseVehicle)) != null && !baseVehicle.IsVehicleRoot() && !this.childVehicles.Contains(baseVehicle))
		{
			this.childVehicles.Add(baseVehicle);
		}
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x0004D188 File Offset: 0x0004B388
	protected override void OnChildRemoved(global::BaseEntity child)
	{
		base.OnChildRemoved(child);
		global::BaseVehicle baseVehicle;
		if ((baseVehicle = (child as global::BaseVehicle)) != null && !baseVehicle.IsVehicleRoot())
		{
			this.childVehicles.Remove(baseVehicle);
		}
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x060007D7 RID: 2007 RVA: 0x0004D1BB File Offset: 0x0004B3BB
	public global::BaseVehicle.Enumerable allMountPoints
	{
		get
		{
			return new global::BaseVehicle.Enumerable(this);
		}
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0004D1C4 File Offset: 0x0004B3C4
	public global::BaseVehicle.MountPointInfo GetMountPoint(int index)
	{
		if (index < 0)
		{
			return null;
		}
		if (index < this.mountPoints.Count)
		{
			return this.mountPoints[index];
		}
		index -= this.mountPoints.Count;
		int num = 0;
		foreach (global::BaseVehicle baseVehicle in this.childVehicles)
		{
			if (!(baseVehicle == null))
			{
				foreach (global::BaseVehicle.MountPointInfo result in baseVehicle.allMountPoints)
				{
					if (num == index)
					{
						return result;
					}
					num++;
				}
			}
		}
		return null;
	}

	// Token: 0x04000528 RID: 1320
	private const float MIN_TIME_BETWEEN_PUSHES = 1f;

	// Token: 0x04000529 RID: 1321
	public TimeSince timeSinceLastPush;

	// Token: 0x0400052A RID: 1322
	private bool prevSleeping;

	// Token: 0x0400052B RID: 1323
	private float nextCollisionFXTime;

	// Token: 0x0400052C RID: 1324
	private CollisionDetectionMode savedCollisionDetectionMode;

	// Token: 0x0400052D RID: 1325
	private ProtoBuf.BaseVehicle pendingLoad;

	// Token: 0x0400052E RID: 1326
	private Queue<global::BasePlayer> recentDrivers = new Queue<global::BasePlayer>();

	// Token: 0x0400052F RID: 1327
	private Action clearRecentDriverAction;

	// Token: 0x04000530 RID: 1328
	private float safeAreaRadius;

	// Token: 0x04000531 RID: 1329
	private Vector3 safeAreaOrigin;

	// Token: 0x04000532 RID: 1330
	private float spawnTime = -1f;

	// Token: 0x04000533 RID: 1331
	[Tooltip("Allow players to mount other mountables/ladders from this vehicle")]
	public bool mountChaining = true;

	// Token: 0x04000534 RID: 1332
	public global::BaseVehicle.ClippingCheckMode clippingChecks;

	// Token: 0x04000535 RID: 1333
	public bool checkVehicleClipping;

	// Token: 0x04000536 RID: 1334
	public global::BaseVehicle.DismountStyle dismountStyle;

	// Token: 0x04000537 RID: 1335
	public bool shouldShowHudHealth;

	// Token: 0x04000538 RID: 1336
	public bool ignoreDamageFromOutside;

	// Token: 0x04000539 RID: 1337
	[Header("Rigidbody (Optional)")]
	public Rigidbody rigidBody;

	// Token: 0x0400053A RID: 1338
	[Header("Mount Points")]
	public List<global::BaseVehicle.MountPointInfo> mountPoints;

	// Token: 0x0400053B RID: 1339
	public bool doClippingAndVisChecks = true;

	// Token: 0x0400053C RID: 1340
	[Header("Damage")]
	public DamageRenderer damageRenderer;

	// Token: 0x0400053D RID: 1341
	[FormerlySerializedAs("explosionDamageMultiplier")]
	public float explosionForceMultiplier = 400f;

	// Token: 0x0400053E RID: 1342
	public float explosionForceMax = 75000f;

	// Token: 0x0400053F RID: 1343
	public const global::BaseEntity.Flags Flag_OnlyOwnerEntry = global::BaseEntity.Flags.Locked;

	// Token: 0x04000540 RID: 1344
	public const global::BaseEntity.Flags Flag_Headlights = global::BaseEntity.Flags.Reserved5;

	// Token: 0x04000541 RID: 1345
	public const global::BaseEntity.Flags Flag_Stationary = global::BaseEntity.Flags.Reserved7;

	// Token: 0x04000542 RID: 1346
	public const global::BaseEntity.Flags Flag_SeatsFull = global::BaseEntity.Flags.Reserved11;

	// Token: 0x04000543 RID: 1347
	protected const global::BaseEntity.Flags Flag_AnyMounted = global::BaseEntity.Flags.InUse;

	// Token: 0x04000544 RID: 1348
	private readonly List<global::BaseVehicle> childVehicles = new List<global::BaseVehicle>(0);

	// Token: 0x02000B71 RID: 2929
	public enum ClippingCheckMode
	{
		// Token: 0x04003E6A RID: 15978
		OnMountOnly,
		// Token: 0x04003E6B RID: 15979
		Always,
		// Token: 0x04003E6C RID: 15980
		AlwaysHeadOnly
	}

	// Token: 0x02000B72 RID: 2930
	public enum DismountStyle
	{
		// Token: 0x04003E6E RID: 15982
		Closest,
		// Token: 0x04003E6F RID: 15983
		Ordered
	}

	// Token: 0x02000B73 RID: 2931
	[Serializable]
	public class MountPointInfo
	{
		// Token: 0x04003E70 RID: 15984
		public bool isDriver;

		// Token: 0x04003E71 RID: 15985
		public Vector3 pos;

		// Token: 0x04003E72 RID: 15986
		public Vector3 rot;

		// Token: 0x04003E73 RID: 15987
		public string bone = "";

		// Token: 0x04003E74 RID: 15988
		public GameObjectRef prefab;

		// Token: 0x04003E75 RID: 15989
		[HideInInspector]
		public BaseMountable mountable;
	}

	// Token: 0x02000B74 RID: 2932
	public readonly struct Enumerable : IEnumerable<global::BaseVehicle.MountPointInfo>, IEnumerable
	{
		// Token: 0x06004AB1 RID: 19121 RVA: 0x0019098A File Offset: 0x0018EB8A
		public Enumerable(global::BaseVehicle vehicle)
		{
			if (vehicle == null)
			{
				throw new ArgumentNullException("vehicle");
			}
			this._vehicle = vehicle;
		}

		// Token: 0x06004AB2 RID: 19122 RVA: 0x001909A7 File Offset: 0x0018EBA7
		public global::BaseVehicle.Enumerator GetEnumerator()
		{
			return new global::BaseVehicle.Enumerator(this._vehicle);
		}

		// Token: 0x06004AB3 RID: 19123 RVA: 0x001909B4 File Offset: 0x0018EBB4
		IEnumerator<global::BaseVehicle.MountPointInfo> IEnumerable<global::BaseVehicle.MountPointInfo>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06004AB4 RID: 19124 RVA: 0x001909B4 File Offset: 0x0018EBB4
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x04003E76 RID: 15990
		private readonly global::BaseVehicle _vehicle;
	}

	// Token: 0x02000B75 RID: 2933
	public struct Enumerator : IEnumerator<global::BaseVehicle.MountPointInfo>, IEnumerator, IDisposable
	{
		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x06004AB5 RID: 19125 RVA: 0x001909C1 File Offset: 0x0018EBC1
		// (set) Token: 0x06004AB6 RID: 19126 RVA: 0x001909C9 File Offset: 0x0018EBC9
		public global::BaseVehicle.MountPointInfo Current { get; private set; }

		// Token: 0x06004AB7 RID: 19127 RVA: 0x001909D2 File Offset: 0x0018EBD2
		public Enumerator(global::BaseVehicle vehicle)
		{
			if (vehicle == null)
			{
				throw new ArgumentNullException("vehicle");
			}
			this._vehicle = vehicle;
			this._state = global::BaseVehicle.Enumerator.State.Direct;
			this._index = -1;
			this._childIndex = -1;
			this._enumerator = null;
			this.Current = null;
		}

		// Token: 0x06004AB8 RID: 19128 RVA: 0x00190A14 File Offset: 0x0018EC14
		public bool MoveNext()
		{
			this.Current = null;
			switch (this._state)
			{
			case global::BaseVehicle.Enumerator.State.Direct:
				this._index++;
				if (this._index < this._vehicle.mountPoints.Count)
				{
					this.Current = this._vehicle.mountPoints[this._index];
					return true;
				}
				this._state = global::BaseVehicle.Enumerator.State.EnterChild;
				break;
			case global::BaseVehicle.Enumerator.State.EnterChild:
				break;
			case global::BaseVehicle.Enumerator.State.EnumerateChild:
				goto IL_11B;
			case global::BaseVehicle.Enumerator.State.Finished:
				return false;
			default:
				throw new NotSupportedException();
			}
			do
			{
				IL_76:
				this._childIndex++;
			}
			while (this._childIndex < this._vehicle.childVehicles.Count && this._vehicle.childVehicles[this._childIndex] == null);
			if (this._childIndex >= this._vehicle.childVehicles.Count)
			{
				this._state = global::BaseVehicle.Enumerator.State.Finished;
				return false;
			}
			this._enumerator = Facepunch.Pool.Get<global::BaseVehicle.Enumerator.Box>();
			this._enumerator.Value = this._vehicle.childVehicles[this._childIndex].allMountPoints.GetEnumerator();
			this._state = global::BaseVehicle.Enumerator.State.EnumerateChild;
			IL_11B:
			if (this._enumerator.Value.MoveNext())
			{
				this.Current = this._enumerator.Value.Current;
				return true;
			}
			this._enumerator.Value.Dispose();
			Facepunch.Pool.Free<global::BaseVehicle.Enumerator.Box>(ref this._enumerator);
			this._state = global::BaseVehicle.Enumerator.State.EnterChild;
			goto IL_76;
		}

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x06004AB9 RID: 19129 RVA: 0x00190B94 File Offset: 0x0018ED94
		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		// Token: 0x06004ABA RID: 19130 RVA: 0x00190B9C File Offset: 0x0018ED9C
		public void Dispose()
		{
			if (this._enumerator != null)
			{
				this._enumerator.Value.Dispose();
				Facepunch.Pool.Free<global::BaseVehicle.Enumerator.Box>(ref this._enumerator);
			}
		}

		// Token: 0x06004ABB RID: 19131 RVA: 0x0015820B File Offset: 0x0015640B
		public void Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x04003E77 RID: 15991
		private readonly global::BaseVehicle _vehicle;

		// Token: 0x04003E78 RID: 15992
		private global::BaseVehicle.Enumerator.State _state;

		// Token: 0x04003E79 RID: 15993
		private int _index;

		// Token: 0x04003E7A RID: 15994
		private int _childIndex;

		// Token: 0x04003E7B RID: 15995
		private global::BaseVehicle.Enumerator.Box _enumerator;

		// Token: 0x02000F5E RID: 3934
		private enum State
		{
			// Token: 0x04004E02 RID: 19970
			Direct,
			// Token: 0x04004E03 RID: 19971
			EnterChild,
			// Token: 0x04004E04 RID: 19972
			EnumerateChild,
			// Token: 0x04004E05 RID: 19973
			Finished
		}

		// Token: 0x02000F5F RID: 3935
		private class Box : Facepunch.Pool.IPooled
		{
			// Token: 0x06005264 RID: 21092 RVA: 0x001A713E File Offset: 0x001A533E
			public void EnterPool()
			{
				this.Value = default(global::BaseVehicle.Enumerator);
			}

			// Token: 0x06005265 RID: 21093 RVA: 0x001A713E File Offset: 0x001A533E
			public void LeavePool()
			{
				this.Value = default(global::BaseVehicle.Enumerator);
			}

			// Token: 0x04004E06 RID: 19974
			public global::BaseVehicle.Enumerator Value;
		}
	}
}
