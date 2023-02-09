using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C0 RID: 192
public class SamSite : ContainerIOEntity
{
	// Token: 0x060010F7 RID: 4343 RVA: 0x00089D6C File Offset: 0x00087F6C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SamSite.OnRpcMessage", 0))
		{
			if (rpc == 3160662357U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ToggleDefenderMode ");
				}
				using (TimeWarning.New("ToggleDefenderMode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3160662357U, "ToggleDefenderMode", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3160662357U, "ToggleDefenderMode", this, player, 3f))
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
							this.ToggleDefenderMode(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ToggleDefenderMode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x00089F2C File Offset: 0x0008812C
	public override bool IsPowered()
	{
		return this.staticRespawn || base.HasFlag(global::BaseEntity.Flags.Reserved8);
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x00089F43 File Offset: 0x00088143
	public override int ConsumptionAmount()
	{
		return 25;
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x00089F47 File Offset: 0x00088147
	public bool IsInDefenderMode()
	{
		return base.HasFlag(this.Flag_DefenderMode);
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x00089F55 File Offset: 0x00088155
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x00089F5E File Offset: 0x0008815E
	private void SetTarget(SamSite.ISamSiteTarget target)
	{
		bool flag = this.currentTarget != target;
		this.currentTarget = target;
		if (!target.IsUnityNull<SamSite.ISamSiteTarget>())
		{
			this.mostRecentTargetType = target.SAMTargetType;
		}
		if (flag)
		{
			this.MarkIODirty();
		}
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x00089F8F File Offset: 0x0008818F
	private void MarkIODirty()
	{
		if (this.staticRespawn)
		{
			return;
		}
		this.lastPassthroughEnergy = -1;
		base.MarkDirtyForceUpdateOutputs();
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x00089FA7 File Offset: 0x000881A7
	private void ClearTarget()
	{
		this.SetTarget(null);
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x00089FB0 File Offset: 0x000881B0
	public override void ServerInit()
	{
		base.ServerInit();
		SamSite.targetTypeUnknown = new SamSite.SamTargetType(this.vehicleScanRadius, 1f, 5f);
		SamSite.targetTypeVehicle = new SamSite.SamTargetType(this.vehicleScanRadius, 1f, 5f);
		SamSite.targetTypeMissile = new SamSite.SamTargetType(this.missileScanRadius, 2.25f, 3.5f);
		this.mostRecentTargetType = SamSite.targetTypeUnknown;
		this.ClearTarget();
		base.InvokeRandomized(new Action(this.TargetScan), 1f, 3f, 1f);
		this.currentAimDir = base.transform.forward;
		if (base.inventory != null && !this.staticRespawn)
		{
			base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedRemoved);
		}
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x0008A07B File Offset: 0x0008827B
	private void OnItemAddedRemoved(global::Item arg1, bool arg2)
	{
		this.EnsureReloaded();
		if (this.IsPowered())
		{
			this.MarkIODirty();
		}
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x0008A091 File Offset: 0x00088291
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.samSite = Facepunch.Pool.Get<SAMSite>();
		info.msg.samSite.aimDir = this.GetAimDir();
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x0008A0C0 File Offset: 0x000882C0
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.staticRespawn && base.HasFlag(global::BaseEntity.Flags.Reserved1))
		{
			base.Invoke(new Action(this.SelfHeal), SamSite.staticrepairseconds);
		}
	}

	// Token: 0x06001103 RID: 4355 RVA: 0x0008A0F4 File Offset: 0x000882F4
	public void SelfHeal()
	{
		this.lifestate = BaseCombatEntity.LifeState.Alive;
		base.health = this.startHealth;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x0008A118 File Offset: 0x00088318
	public override void Die(HitInfo info = null)
	{
		if (this.staticRespawn)
		{
			this.ClearTarget();
			Quaternion rotation = Quaternion.Euler(0f, Quaternion.LookRotation(this.currentAimDir, Vector3.up).eulerAngles.y, 0f);
			this.currentAimDir = rotation * Vector3.forward;
			base.Invoke(new Action(this.SelfHeal), SamSite.staticrepairseconds);
			this.lifestate = BaseCombatEntity.LifeState.Dead;
			base.health = 0f;
			base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
			return;
		}
		base.Die(info);
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x0008A1B0 File Offset: 0x000883B0
	public void FixedUpdate()
	{
		Vector3 rhs = this.currentAimDir;
		if (!this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>() && this.IsPowered())
		{
			float num = this.projectileTest.Get().GetComponent<ServerProjectile>().speed * this.currentTarget.SAMTargetType.speedMultiplier;
			Vector3 a = this.currentTarget.CenterPoint();
			float num2 = Vector3.Distance(a, this.eyePoint.transform.position);
			float num3 = num2 / num;
			Vector3 a2 = a + this.currentTarget.GetWorldVelocity() * num3;
			num3 = Vector3.Distance(a2, this.eyePoint.transform.position) / num;
			a2 = a + this.currentTarget.GetWorldVelocity() * num3;
			if (this.currentTarget.GetWorldVelocity().magnitude > 0.1f)
			{
				float d = Mathf.Sin(UnityEngine.Time.time * 3f) * (1f + num3 * 0.5f);
				a2 += this.currentTarget.GetWorldVelocity().normalized * d;
			}
			this.currentAimDir = (a2 - this.eyePoint.transform.position).normalized;
			if (num2 > this.currentTarget.SAMTargetType.scanRadius)
			{
				this.ClearTarget();
			}
		}
		Vector3 vector = Quaternion.LookRotation(this.currentAimDir, base.transform.up).eulerAngles;
		vector = BaseMountable.ConvertVector(vector);
		float t = Mathf.InverseLerp(0f, 90f, -vector.x);
		float z = Mathf.Lerp(15f, -75f, t);
		Quaternion localRotation = Quaternion.Euler(0f, vector.y, 0f);
		this.yaw.transform.localRotation = localRotation;
		Quaternion localRotation2 = Quaternion.Euler(this.pitch.transform.localRotation.eulerAngles.x, this.pitch.transform.localRotation.eulerAngles.y, z);
		this.pitch.transform.localRotation = localRotation2;
		if (this.currentAimDir != rhs)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x0008A404 File Offset: 0x00088604
	public Vector3 GetAimDir()
	{
		return this.currentAimDir;
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x0008A40C File Offset: 0x0008860C
	public bool HasValidTarget()
	{
		return !this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>();
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x0008A41C File Offset: 0x0008861C
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && (!base.isServer || !this.pickup.requireEmptyInv || base.inventory == null || base.inventory.itemList.Count <= 0) && !this.HasAmmo();
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x0008A470 File Offset: 0x00088670
	public void TargetScan()
	{
		if (!this.IsPowered())
		{
			this.lastTargetVisibleTime = 0f;
			return;
		}
		if (UnityEngine.Time.time > this.lastTargetVisibleTime + 3f)
		{
			this.ClearTarget();
		}
		if (!this.staticRespawn)
		{
			int num = (this.ammoItem != null && this.ammoItem.parent == base.inventory) ? this.ammoItem.amount : 0;
			bool flag = this.lastAmmoCount < this.lowAmmoThreshold;
			bool flag2 = num < this.lowAmmoThreshold;
			if (num != this.lastAmmoCount && flag != flag2)
			{
				this.MarkIODirty();
			}
			this.lastAmmoCount = num;
		}
		if (this.HasValidTarget())
		{
			return;
		}
		if (this.IsDead())
		{
			return;
		}
		List<SamSite.ISamSiteTarget> list = Facepunch.Pool.GetList<SamSite.ISamSiteTarget>();
		if (!this.IsInDefenderMode())
		{
			this.<TargetScan>g__AddTargetSet|55_0(list, 32768, SamSite.targetTypeVehicle.scanRadius);
		}
		this.<TargetScan>g__AddTargetSet|55_0(list, 1048576, SamSite.targetTypeMissile.scanRadius);
		SamSite.ISamSiteTarget samSiteTarget = null;
		foreach (SamSite.ISamSiteTarget samSiteTarget2 in list)
		{
			if (!samSiteTarget2.isClient && samSiteTarget2.CenterPoint().y >= this.eyePoint.transform.position.y && samSiteTarget2.IsVisible(this.eyePoint.transform.position, samSiteTarget2.SAMTargetType.scanRadius * 2f) && samSiteTarget2.IsValidSAMTarget(this.staticRespawn))
			{
				samSiteTarget = samSiteTarget2;
				break;
			}
		}
		if (!samSiteTarget.IsUnityNull<SamSite.ISamSiteTarget>() && this.currentTarget != samSiteTarget)
		{
			this.lockOnTime = UnityEngine.Time.time + 0.5f;
		}
		this.SetTarget(samSiteTarget);
		if (!this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>())
		{
			this.lastTargetVisibleTime = UnityEngine.Time.time;
		}
		Facepunch.Pool.FreeList<SamSite.ISamSiteTarget>(ref list);
		if (this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>())
		{
			base.CancelInvoke(new Action(this.WeaponTick));
			return;
		}
		base.InvokeRandomized(new Action(this.WeaponTick), 0f, 0.5f, 0.2f);
	}

	// Token: 0x0600110A RID: 4362 RVA: 0x0008A690 File Offset: 0x00088890
	public virtual bool HasAmmo()
	{
		return this.staticRespawn || (this.ammoItem != null && this.ammoItem.amount > 0 && this.ammoItem.parent == base.inventory);
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x0008A6C8 File Offset: 0x000888C8
	public void Reload()
	{
		if (this.staticRespawn)
		{
			return;
		}
		for (int i = 0; i < base.inventory.itemList.Count; i++)
		{
			global::Item item = base.inventory.itemList[i];
			if (item != null && item.info.itemid == this.ammoType.itemid && item.amount > 0)
			{
				this.ammoItem = item;
				return;
			}
		}
		this.ammoItem = null;
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x0008A73E File Offset: 0x0008893E
	public void EnsureReloaded()
	{
		if (!this.HasAmmo())
		{
			this.Reload();
		}
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x0008A74E File Offset: 0x0008894E
	public bool IsReloading()
	{
		return base.IsInvoking(new Action(this.Reload));
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x0008A764 File Offset: 0x00088964
	public void WeaponTick()
	{
		if (this.IsDead())
		{
			return;
		}
		if (UnityEngine.Time.time < this.lockOnTime)
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextBurstTime)
		{
			return;
		}
		if (!this.IsPowered())
		{
			this.firedCount = 0;
			return;
		}
		if (this.firedCount >= 6)
		{
			float timeBetweenBursts = this.mostRecentTargetType.timeBetweenBursts;
			this.nextBurstTime = UnityEngine.Time.time + timeBetweenBursts;
			this.firedCount = 0;
			return;
		}
		this.EnsureReloaded();
		if (!this.HasAmmo())
		{
			return;
		}
		bool flag = this.ammoItem != null && this.ammoItem.amount == this.lowAmmoThreshold;
		if (!this.staticRespawn && this.ammoItem != null)
		{
			this.ammoItem.UseItem(1);
		}
		this.firedCount++;
		float speedMultiplier = 1f;
		if (!this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>())
		{
			speedMultiplier = this.currentTarget.SAMTargetType.speedMultiplier;
		}
		this.FireProjectile(this.tubes[this.currentTubeIndex].position, this.currentAimDir, speedMultiplier);
		Effect.server.Run(this.muzzleFlashTest.resourcePath, this, StringPool.Get("Tube " + (this.currentTubeIndex + 1).ToString()), Vector3.zero, Vector3.up, null, false);
		this.currentTubeIndex++;
		if (this.currentTubeIndex >= this.tubes.Length)
		{
			this.currentTubeIndex = 0;
		}
		if (flag)
		{
			this.MarkIODirty();
		}
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x0008A8D4 File Offset: 0x00088AD4
	public void FireProjectile(Vector3 origin, Vector3 direction, float speedMultiplier)
	{
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.projectileTest.resourcePath, origin, Quaternion.LookRotation(direction, Vector3.up), true);
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.creatorEntity = this;
		ServerProjectile component = baseEntity.GetComponent<ServerProjectile>();
		if (component)
		{
			component.InitializeVelocity(this.GetInheritedProjectileVelocity(direction) + direction * component.speed * speedMultiplier);
		}
		baseEntity.Spawn();
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x0008A950 File Offset: 0x00088B50
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int result = Mathf.Min(1, this.GetCurrentEnergy());
		if (outputSlot == 0)
		{
			if (this.currentTarget.IsUnityNull<SamSite.ISamSiteTarget>())
			{
				return 0;
			}
			return result;
		}
		else if (outputSlot == 1)
		{
			if (this.ammoItem == null || this.ammoItem.amount >= this.lowAmmoThreshold || this.ammoItem.parent != base.inventory)
			{
				return 0;
			}
			return result;
		}
		else
		{
			if (outputSlot != 2)
			{
				return this.GetCurrentEnergy();
			}
			if (this.HasAmmo())
			{
				return 0;
			}
			return result;
		}
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x0008A9CC File Offset: 0x00088BCC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	private void ToggleDefenderMode(global::BaseEntity.RPCMessage msg)
	{
		if (this.staticRespawn)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null || !player.CanBuild())
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (flag == this.IsInDefenderMode())
		{
			return;
		}
		base.SetFlag(this.Flag_DefenderMode, flag, false, true);
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x0008AACC File Offset: 0x00088CCC
	[CompilerGenerated]
	private void <TargetScan>g__AddTargetSet|55_0(List<SamSite.ISamSiteTarget> allTargets, int layerMask, float scanRadius)
	{
		List<SamSite.ISamSiteTarget> list = Facepunch.Pool.GetList<SamSite.ISamSiteTarget>();
		global::Vis.Entities<SamSite.ISamSiteTarget>(this.eyePoint.transform.position, scanRadius, list, layerMask, QueryTriggerInteraction.Ignore);
		allTargets.AddRange(list);
		Facepunch.Pool.FreeList<SamSite.ISamSiteTarget>(ref list);
	}

	// Token: 0x04000A8A RID: 2698
	public Animator pitchAnimator;

	// Token: 0x04000A8B RID: 2699
	public GameObject yaw;

	// Token: 0x04000A8C RID: 2700
	public GameObject pitch;

	// Token: 0x04000A8D RID: 2701
	public GameObject gear;

	// Token: 0x04000A8E RID: 2702
	public Transform eyePoint;

	// Token: 0x04000A8F RID: 2703
	public float gearEpislonDegrees = 20f;

	// Token: 0x04000A90 RID: 2704
	public float turnSpeed = 1f;

	// Token: 0x04000A91 RID: 2705
	public float clientLerpSpeed = 1f;

	// Token: 0x04000A92 RID: 2706
	public Vector3 currentAimDir = Vector3.forward;

	// Token: 0x04000A93 RID: 2707
	public Vector3 targetAimDir = Vector3.forward;

	// Token: 0x04000A94 RID: 2708
	public float vehicleScanRadius = 350f;

	// Token: 0x04000A95 RID: 2709
	public float missileScanRadius = 500f;

	// Token: 0x04000A96 RID: 2710
	public GameObjectRef projectileTest;

	// Token: 0x04000A97 RID: 2711
	public GameObjectRef muzzleFlashTest;

	// Token: 0x04000A98 RID: 2712
	public bool staticRespawn;

	// Token: 0x04000A99 RID: 2713
	public ItemDefinition ammoType;

	// Token: 0x04000A9A RID: 2714
	public Transform[] tubes;

	// Token: 0x04000A9B RID: 2715
	[ServerVar(Help = "how long until static sam sites auto repair")]
	public static float staticrepairseconds = 1200f;

	// Token: 0x04000A9C RID: 2716
	public SoundDefinition yawMovementLoopDef;

	// Token: 0x04000A9D RID: 2717
	public float yawGainLerp = 8f;

	// Token: 0x04000A9E RID: 2718
	public float yawGainMovementSpeedMult = 0.1f;

	// Token: 0x04000A9F RID: 2719
	public SoundDefinition pitchMovementLoopDef;

	// Token: 0x04000AA0 RID: 2720
	public float pitchGainLerp = 10f;

	// Token: 0x04000AA1 RID: 2721
	public float pitchGainMovementSpeedMult = 0.5f;

	// Token: 0x04000AA2 RID: 2722
	public int lowAmmoThreshold = 5;

	// Token: 0x04000AA3 RID: 2723
	public global::BaseEntity.Flags Flag_DefenderMode = global::BaseEntity.Flags.Reserved9;

	// Token: 0x04000AA4 RID: 2724
	public static SamSite.SamTargetType targetTypeUnknown;

	// Token: 0x04000AA5 RID: 2725
	public static SamSite.SamTargetType targetTypeVehicle;

	// Token: 0x04000AA6 RID: 2726
	public static SamSite.SamTargetType targetTypeMissile;

	// Token: 0x04000AA7 RID: 2727
	private SamSite.ISamSiteTarget currentTarget;

	// Token: 0x04000AA8 RID: 2728
	private SamSite.SamTargetType mostRecentTargetType;

	// Token: 0x04000AA9 RID: 2729
	private global::Item ammoItem;

	// Token: 0x04000AAA RID: 2730
	private float lockOnTime;

	// Token: 0x04000AAB RID: 2731
	private float lastTargetVisibleTime;

	// Token: 0x04000AAC RID: 2732
	private int lastAmmoCount;

	// Token: 0x04000AAD RID: 2733
	private int currentTubeIndex;

	// Token: 0x04000AAE RID: 2734
	private int firedCount;

	// Token: 0x04000AAF RID: 2735
	private float nextBurstTime;

	// Token: 0x02000BAD RID: 2989
	public interface ISamSiteTarget
	{
		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x06004B26 RID: 19238
		SamSite.SamTargetType SAMTargetType { get; }

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x06004B27 RID: 19239
		bool isClient { get; }

		// Token: 0x06004B28 RID: 19240
		bool IsValidSAMTarget(bool staticRespawn);

		// Token: 0x06004B29 RID: 19241
		Vector3 CenterPoint();

		// Token: 0x06004B2A RID: 19242
		Vector3 GetWorldVelocity();

		// Token: 0x06004B2B RID: 19243
		bool IsVisible(Vector3 position, float maxDistance = float.PositiveInfinity);
	}

	// Token: 0x02000BAE RID: 2990
	public class SamTargetType
	{
		// Token: 0x06004B2C RID: 19244 RVA: 0x0019188E File Offset: 0x0018FA8E
		public SamTargetType(float scanRadius, float speedMultiplier, float timeBetweenBursts)
		{
			this.scanRadius = scanRadius;
			this.speedMultiplier = speedMultiplier;
			this.timeBetweenBursts = timeBetweenBursts;
		}

		// Token: 0x04003F23 RID: 16163
		public readonly float scanRadius;

		// Token: 0x04003F24 RID: 16164
		public readonly float speedMultiplier;

		// Token: 0x04003F25 RID: 16165
		public readonly float timeBetweenBursts;
	}
}
