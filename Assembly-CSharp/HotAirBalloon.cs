using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007D RID: 125
public class HotAirBalloon : BaseCombatEntity, SamSite.ISamSiteTarget
{
	// Token: 0x06000BD8 RID: 3032 RVA: 0x00065FF8 File Offset: 0x000641F8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HotAirBalloon.OnRpcMessage", 0))
		{
			if (rpc == 578721460U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - EngineSwitch ");
				}
				using (TimeWarning.New("EngineSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(578721460U, "EngineSwitch", this, player, 3f))
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
							this.EngineSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in EngineSwitch");
					}
				}
				return true;
			}
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
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenFuel(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_OpenFuel");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x000662A8 File Offset: 0x000644A8
	public bool IsFullyInflated
	{
		get
		{
			return this.inflationLevel >= 1f;
		}
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x000662BA File Offset: 0x000644BA
	public override void InitShared()
	{
		this.fuelSystem = new EntityFuelSystem(base.isServer, this.fuelStoragePrefab, this.children, true);
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x000662DC File Offset: 0x000644DC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.hotAirBalloon != null)
		{
			this.inflationLevel = info.msg.hotAirBalloon.inflationAmount;
			if (info.fromDisk && this.myRigidbody)
			{
				this.myRigidbody.velocity = info.msg.hotAirBalloon.velocity;
			}
		}
		if (info.msg.motorBoat != null)
		{
			this.fuelSystem.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
			this.storageUnitInstance.uid = info.msg.motorBoat.storageid;
		}
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x0006638B File Offset: 0x0006458B
	public bool WaterLogged()
	{
		return WaterLevel.Test(this.engineHeight.position, true, this);
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000BDD RID: 3037 RVA: 0x000067CC File Offset: 0x000049CC
	public SamSite.SamTargetType SAMTargetType
	{
		get
		{
			return SamSite.targetTypeVehicle;
		}
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x000663A0 File Offset: 0x000645A0
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (this.isSpawned)
			{
				this.fuelSystem.CheckNewChild(child);
			}
			if (child.prefabID == this.storageUnitPrefab.GetEntity().prefabID)
			{
				this.storageUnitInstance.Set((StorageContainer)child);
			}
		}
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x000663F9 File Offset: 0x000645F9
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot && this.storageUnitInstance.IsValid(base.isServer))
		{
			this.storageUnitInstance.Get(base.isServer).DropItems(null);
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x00066432 File Offset: 0x00064632
	public bool IsValidSAMTarget(bool staticRespawn)
	{
		if (staticRespawn)
		{
			return this.IsFullyInflated;
		}
		return this.IsFullyInflated && !global::BaseVehicle.InSafeZone(this.triggers, base.transform.position);
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x000058B6 File Offset: 0x00003AB6
	public override float GetNetworkTime()
	{
		return UnityEngine.Time.fixedTime;
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x00066461 File Offset: 0x00064661
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x00066474 File Offset: 0x00064674
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		this.fuelSystem.LootFuel(player);
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x000664A0 File Offset: 0x000646A0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.hotAirBalloon = Facepunch.Pool.Get<ProtoBuf.HotAirBalloon>();
		info.msg.hotAirBalloon.inflationAmount = this.inflationLevel;
		if (info.forDisk && this.myRigidbody)
		{
			info.msg.hotAirBalloon.velocity = this.myRigidbody.velocity;
		}
		info.msg.motorBoat = Facepunch.Pool.Get<Motorboat>();
		info.msg.motorBoat.storageid = this.storageUnitInstance.uid;
		info.msg.motorBoat.fuelStorageID = this.fuelSystem.fuelStorageInstance.uid;
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x00066558 File Offset: 0x00064758
	public override void ServerInit()
	{
		this.myRigidbody.centerOfMass = this.centerOfMass.localPosition;
		this.myRigidbody.isKinematic = false;
		this.avgTerrainHeight = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		base.ServerInit();
		this.bounds = this.collapsedBounds;
		base.InvokeRandomized(new Action(this.DecayTick), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
		base.InvokeRandomized(new Action(this.UpdateIsGrounded), 0f, 3f, 0.2f);
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x00066600 File Offset: 0x00064800
	public void DecayTick()
	{
		if (base.healthFraction == 0f || this.IsFullyInflated)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastBlastTime + 600f)
		{
			return;
		}
		float num = 1f / global::HotAirBalloon.outsidedecayminutes;
		if (this.IsOutside())
		{
			base.Hurt(this.MaxHealth() * num, DamageType.Decay, this, false);
		}
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x00066660 File Offset: 0x00064860
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void EngineSwitch(global::BaseEntity.RPCMessage msg)
	{
		bool b = msg.read.Bit();
		base.SetFlag(global::BaseEntity.Flags.On, b, false, true);
		if (base.IsOn())
		{
			base.Invoke(new Action(this.ScheduleOff), 60f);
			return;
		}
		base.CancelInvoke(new Action(this.ScheduleOff));
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x0005E44D File Offset: 0x0005C64D
	public void ScheduleOff()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x000666B8 File Offset: 0x000648B8
	public void UpdateIsGrounded()
	{
		if (this.lastBlastTime + 5f > UnityEngine.Time.time)
		{
			return;
		}
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(this.groundSample.transform.position, 1.25f, list, 1218511105, QueryTriggerInteraction.Ignore);
		this.grounded = (list.Count > 0);
		this.CheckGlobal(this.flags);
		Facepunch.Pool.FreeList<Collider>(ref list);
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x00066722 File Offset: 0x00064922
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			this.CheckGlobal(next);
		}
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x0006673C File Offset: 0x0006493C
	private void CheckGlobal(global::BaseEntity.Flags flags)
	{
		bool wants = flags.HasFlag(global::BaseEntity.Flags.On) || flags.HasFlag(global::BaseEntity.Flags.Reserved2) || flags.HasFlag(global::BaseEntity.Flags.Reserved1) || !this.grounded;
		base.EnableGlobalBroadcast(wants);
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x000667A0 File Offset: 0x000649A0
	protected void FixedUpdate()
	{
		if (!this.isSpawned)
		{
			return;
		}
		if (base.isClient)
		{
			return;
		}
		if (!this.fuelSystem.HasFuel(false) || this.WaterLogged())
		{
			base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		}
		if (base.IsOn())
		{
			this.fuelSystem.TryUseFuel(UnityEngine.Time.fixedDeltaTime, this.fuelPerSec);
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, this.fuelSystem.HasFuel(false), false, true);
		bool flag = (this.IsFullyInflated && this.myRigidbody.velocity.y < 0f) || this.myRigidbody.velocity.y < 0.75f;
		foreach (GameObject gameObject in this.killTriggers)
		{
			if (gameObject.activeSelf != flag)
			{
				gameObject.SetActive(flag);
			}
		}
		float num = this.inflationLevel;
		if (base.IsOn() && !this.IsFullyInflated)
		{
			this.inflationLevel = Mathf.Clamp01(this.inflationLevel + UnityEngine.Time.fixedDeltaTime / 10f);
		}
		else if (this.grounded && this.inflationLevel > 0f && !base.IsOn() && (UnityEngine.Time.time > this.lastBlastTime + 30f || this.WaterLogged()))
		{
			this.inflationLevel = Mathf.Clamp01(this.inflationLevel - UnityEngine.Time.fixedDeltaTime / 10f);
		}
		if (num != this.inflationLevel)
		{
			if (this.IsFullyInflated)
			{
				this.bounds = this.raisedBounds;
			}
			else if (this.inflationLevel == 0f)
			{
				this.bounds = this.collapsedBounds;
			}
			base.SetFlag(global::BaseEntity.Flags.Reserved1, this.inflationLevel > 0.3f, false, true);
			base.SetFlag(global::BaseEntity.Flags.Reserved2, this.inflationLevel >= 1f, false, true);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			float num2 = this.inflationLevel;
		}
		bool flag2 = !this.myRigidbody.IsSleeping() || this.inflationLevel > 0f;
		foreach (GameObject gameObject2 in this.balloonColliders)
		{
			if (gameObject2.activeSelf != flag2)
			{
				gameObject2.SetActive(flag2);
			}
		}
		if (base.IsOn())
		{
			if (this.IsFullyInflated)
			{
				this.currentBuoyancy += UnityEngine.Time.fixedDeltaTime * 0.2f;
				this.lastBlastTime = UnityEngine.Time.time;
			}
		}
		else
		{
			this.currentBuoyancy -= UnityEngine.Time.fixedDeltaTime * 0.1f;
		}
		this.currentBuoyancy = Mathf.Clamp(this.currentBuoyancy, 0f, 0.8f + 0.2f * base.healthFraction);
		if (this.inflationLevel > 0f)
		{
			this.avgTerrainHeight = Mathf.Lerp(this.avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(base.transform.position), UnityEngine.Time.deltaTime);
			float num3 = 1f - Mathf.InverseLerp(this.avgTerrainHeight + global::HotAirBalloon.serviceCeiling - 20f, this.avgTerrainHeight + global::HotAirBalloon.serviceCeiling, this.buoyancyPoint.position.y);
			this.myRigidbody.AddForceAtPosition(Vector3.up * -UnityEngine.Physics.gravity.y * this.myRigidbody.mass * 0.5f * this.inflationLevel, this.buoyancyPoint.position, ForceMode.Force);
			this.myRigidbody.AddForceAtPosition(Vector3.up * this.liftAmount * this.currentBuoyancy * num3, this.buoyancyPoint.position, ForceMode.Force);
			Vector3 windAtPos = this.GetWindAtPos(this.buoyancyPoint.position);
			float magnitude = windAtPos.magnitude;
			float num4 = 1f;
			float num5 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(this.buoyancyPoint.position), TerrainMeta.WaterMap.GetHeight(this.buoyancyPoint.position));
			float num6 = Mathf.InverseLerp(num5 + 20f, num5 + 60f, this.buoyancyPoint.position.y);
			float num7 = 1f;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.SphereCast(new Ray(base.transform.position + Vector3.up * 2f, Vector3.down), 1.5f, out raycastHit, 5f, 1218511105))
			{
				num7 = Mathf.Clamp01(raycastHit.distance / 5f);
			}
			num4 *= num6 * num3 * num7;
			num4 *= 0.2f + 0.8f * base.healthFraction;
			Vector3 vector = windAtPos.normalized * num4 * this.windForce;
			this.currentWindVec = Vector3.Lerp(this.currentWindVec, vector, UnityEngine.Time.fixedDeltaTime * 0.25f);
			this.myRigidbody.AddForceAtPosition(vector * 0.1f, this.buoyancyPoint.position, ForceMode.Force);
			this.myRigidbody.AddForce(vector * 0.9f, ForceMode.Force);
		}
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x00066CB0 File Offset: 0x00064EB0
	public override Vector3 GetLocalVelocityServer()
	{
		if (this.myRigidbody == null)
		{
			return Vector3.zero;
		}
		return this.myRigidbody.velocity;
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x00066CD4 File Offset: 0x00064ED4
	public override Quaternion GetAngularVelocityServer()
	{
		if (this.myRigidbody == null)
		{
			return Quaternion.identity;
		}
		if (this.myRigidbody.angularVelocity.sqrMagnitude < 0.1f)
		{
			return Quaternion.identity;
		}
		return Quaternion.LookRotation(this.myRigidbody.angularVelocity, base.transform.up);
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x00066D30 File Offset: 0x00064F30
	public Vector3 GetWindAtPos(Vector3 pos)
	{
		float num = pos.y * 6f;
		Vector3 vector = new Vector3(Mathf.Sin(num * 0.017453292f), 0f, Mathf.Cos(num * 0.017453292f));
		return vector.normalized * 1f;
	}

	// Token: 0x04000785 RID: 1925
	protected const global::BaseEntity.Flags Flag_HasFuel = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000786 RID: 1926
	protected const global::BaseEntity.Flags Flag_HalfInflated = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000787 RID: 1927
	protected const global::BaseEntity.Flags Flag_FullInflated = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000788 RID: 1928
	public Transform centerOfMass;

	// Token: 0x04000789 RID: 1929
	public Rigidbody myRigidbody;

	// Token: 0x0400078A RID: 1930
	public Transform buoyancyPoint;

	// Token: 0x0400078B RID: 1931
	public float liftAmount = 10f;

	// Token: 0x0400078C RID: 1932
	public Transform windSock;

	// Token: 0x0400078D RID: 1933
	public Transform[] windFlags;

	// Token: 0x0400078E RID: 1934
	public GameObject staticBalloonDeflated;

	// Token: 0x0400078F RID: 1935
	public GameObject staticBalloon;

	// Token: 0x04000790 RID: 1936
	public GameObject animatedBalloon;

	// Token: 0x04000791 RID: 1937
	public Animator balloonAnimator;

	// Token: 0x04000792 RID: 1938
	public Transform groundSample;

	// Token: 0x04000793 RID: 1939
	public float inflationLevel;

	// Token: 0x04000794 RID: 1940
	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	// Token: 0x04000795 RID: 1941
	public float fuelPerSec = 0.25f;

	// Token: 0x04000796 RID: 1942
	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	// Token: 0x04000797 RID: 1943
	public EntityRef<StorageContainer> storageUnitInstance;

	// Token: 0x04000798 RID: 1944
	[Header("Damage")]
	public DamageRenderer damageRenderer;

	// Token: 0x04000799 RID: 1945
	public Transform engineHeight;

	// Token: 0x0400079A RID: 1946
	public GameObject[] killTriggers;

	// Token: 0x0400079B RID: 1947
	private EntityFuelSystem fuelSystem;

	// Token: 0x0400079C RID: 1948
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 1f;

	// Token: 0x0400079D RID: 1949
	[ServerVar(Help = "How long before a HAB loses all its health while outside")]
	public static float outsidedecayminutes = 180f;

	// Token: 0x0400079E RID: 1950
	public float windForce = 30000f;

	// Token: 0x0400079F RID: 1951
	public Vector3 currentWindVec = Vector3.zero;

	// Token: 0x040007A0 RID: 1952
	public Bounds collapsedBounds;

	// Token: 0x040007A1 RID: 1953
	public Bounds raisedBounds;

	// Token: 0x040007A2 RID: 1954
	public GameObject[] balloonColliders;

	// Token: 0x040007A3 RID: 1955
	[ServerVar]
	public static float serviceCeiling = 200f;

	// Token: 0x040007A4 RID: 1956
	private float currentBuoyancy;

	// Token: 0x040007A5 RID: 1957
	private float lastBlastTime;

	// Token: 0x040007A6 RID: 1958
	private float avgTerrainHeight;

	// Token: 0x040007A7 RID: 1959
	protected bool grounded;
}
