using System;
using System.Runtime.CompilerServices;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class DeliveryDrone : Drone
{
	// Token: 0x0600166B RID: 5739 RVA: 0x000AA5F8 File Offset: 0x000A87F8
	public void Setup(Marketplace marketplace, global::MarketTerminal terminal, global::VendingMachine vendingMachine)
	{
		this.sourceMarketplace.Set(marketplace);
		this.sourceTerminal.Set(terminal);
		this.targetVendingMachine.Set(vendingMachine);
		this._state = global::DeliveryDrone.State.Takeoff;
		this._sinceLastStateChange = 0f;
		this._pickUpTicks = 0;
	}

	// Token: 0x0600166C RID: 5740 RVA: 0x000AA647 File Offset: 0x000A8847
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.Think), 0f, 0.5f, 0.25f);
		this.CreateMapMarker();
	}

	// Token: 0x0600166D RID: 5741 RVA: 0x000AA678 File Offset: 0x000A8878
	public void CreateMapMarker()
	{
		if (this._mapMarkerInstance != null)
		{
			this._mapMarkerInstance.Kill(global::BaseNetworkable.DestroyMode.None);
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.mapMarkerPrefab;
		global::BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, Vector3.zero, Quaternion.identity, true);
		baseEntity.OwnerID = base.OwnerID;
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this._mapMarkerInstance = baseEntity;
	}

	// Token: 0x0600166E RID: 5742 RVA: 0x000AA6EC File Offset: 0x000A88EC
	private void Think()
	{
		global::DeliveryDrone.<>c__DisplayClass24_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this._sinceLastStateChange > this.stateTimeout)
		{
			Debug.LogError("Delivery drone hasn't change state in too long, killing", this);
			this.ForceRemove();
			return;
		}
		global::MarketTerminal marketTerminal;
		if (!this.sourceMarketplace.TryGet(true, out CS$<>8__locals1.marketplace) || !this.sourceTerminal.TryGet(true, out marketTerminal))
		{
			Debug.LogError("Delivery drone's marketplace or terminal was destroyed, killing", this);
			this.ForceRemove();
			return;
		}
		global::VendingMachine vendingMachine;
		if (!this.targetVendingMachine.TryGet(true, out vendingMachine) && this._state <= global::DeliveryDrone.State.AscendBeforeReturn)
		{
			this.<Think>g__SetState|24_7(global::DeliveryDrone.State.ReturnToTerminal, ref CS$<>8__locals1);
		}
		CS$<>8__locals1.currentPosition = base.transform.position;
		float num = this.<Think>g__GetMinimumHeight|24_1(Vector3.zero, ref CS$<>8__locals1);
		if (this._goToY != null)
		{
			if (!this.<Think>g__IsAtGoToY|24_6(ref CS$<>8__locals1))
			{
				this.targetPosition = new Vector3?(CS$<>8__locals1.currentPosition.WithY(this._goToY.Value));
				return;
			}
			this._goToY = null;
			this._sinceLastObstacleBlock = 0f;
			this._minimumYLock = new float?(CS$<>8__locals1.currentPosition.y);
		}
		switch (this._state)
		{
		case global::DeliveryDrone.State.Takeoff:
			this.<Think>g__SetGoalPosition|24_3(CS$<>8__locals1.marketplace.droneLaunchPoint.position + Vector3.up * 15f, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.FlyToVendingMachine, ref CS$<>8__locals1);
			}
			break;
		case global::DeliveryDrone.State.FlyToVendingMachine:
		{
			bool flag;
			float num2 = this.<Think>g__CalculatePreferredY|24_0(out flag, ref CS$<>8__locals1);
			if (flag && CS$<>8__locals1.currentPosition.y < num2)
			{
				this.<Think>g__SetGoToY|24_5(num2 + this.marginAbovePreferredHeight, ref CS$<>8__locals1);
				return;
			}
			Vector3 vector;
			Vector3 position;
			this.config.FindDescentPoints(vendingMachine, num2 + this.marginAbovePreferredHeight, out vector, out position);
			this.<Think>g__SetGoalPosition|24_3(position, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.DescendToVendingMachine, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.DescendToVendingMachine:
		{
			Vector3 vector;
			Vector3 position2;
			this.config.FindDescentPoints(vendingMachine, CS$<>8__locals1.currentPosition.y, out position2, out vector);
			this.<Think>g__SetGoalPosition|24_3(position2, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.PickUpItems, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.PickUpItems:
			this._pickUpTicks++;
			if (this._pickUpTicks >= this.pickUpDelayInTicks)
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.AscendBeforeReturn, ref CS$<>8__locals1);
			}
			break;
		case global::DeliveryDrone.State.AscendBeforeReturn:
		{
			Vector3 vector;
			Vector3 position3;
			this.config.FindDescentPoints(vendingMachine, num + this.preferredCruiseHeight, out vector, out position3);
			this.<Think>g__SetGoalPosition|24_3(position3, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.ReturnToTerminal, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.ReturnToTerminal:
		{
			bool flag2;
			float num3 = this.<Think>g__CalculatePreferredY|24_0(out flag2, ref CS$<>8__locals1);
			if (flag2 && CS$<>8__locals1.currentPosition.y < num3)
			{
				this.<Think>g__SetGoToY|24_5(num3 + this.marginAbovePreferredHeight, ref CS$<>8__locals1);
				return;
			}
			Vector3 vector2 = this.<Think>g__LandingPosition|24_2(ref CS$<>8__locals1);
			if (Vector3Ex.Distance2D(CS$<>8__locals1.currentPosition, vector2) < 30f)
			{
				vector2.y = Mathf.Max(vector2.y, num3 + this.marginAbovePreferredHeight);
			}
			else
			{
				vector2.y = num3 + this.marginAbovePreferredHeight;
			}
			this.<Think>g__SetGoalPosition|24_3(vector2, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.Landing, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.Landing:
			this.<Think>g__SetGoalPosition|24_3(this.<Think>g__LandingPosition|24_2(ref CS$<>8__locals1), ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				CS$<>8__locals1.marketplace.ReturnDrone(this);
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.Invalid, ref CS$<>8__locals1);
			}
			break;
		default:
			this.ForceRemove();
			break;
		}
		if (this._minimumYLock != null)
		{
			if (this._sinceLastObstacleBlock > this.obstacleHeightLockDuration)
			{
				this._minimumYLock = null;
				return;
			}
			if (this.targetPosition != null && this.targetPosition.Value.y < this._minimumYLock.Value)
			{
				this.targetPosition = new Vector3?(this.targetPosition.Value.WithY(this._minimumYLock.Value));
			}
		}
	}

	// Token: 0x0600166F RID: 5743 RVA: 0x000AAAF0 File Offset: 0x000A8CF0
	private void ForceRemove()
	{
		Marketplace marketplace;
		if (this.sourceMarketplace.TryGet(true, out marketplace))
		{
			marketplace.ReturnDrone(this);
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001670 RID: 5744 RVA: 0x000AAB1C File Offset: 0x000A8D1C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.deliveryDrone = Pool.Get<ProtoBuf.DeliveryDrone>();
			info.msg.deliveryDrone.marketplaceId = this.sourceMarketplace.uid;
			info.msg.deliveryDrone.terminalId = this.sourceTerminal.uid;
			info.msg.deliveryDrone.vendingMachineId = this.targetVendingMachine.uid;
			info.msg.deliveryDrone.state = (int)this._state;
		}
	}

	// Token: 0x06001671 RID: 5745 RVA: 0x000AABB0 File Offset: 0x000A8DB0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.deliveryDrone != null)
		{
			this.sourceMarketplace = new EntityRef<Marketplace>(info.msg.deliveryDrone.marketplaceId);
			this.sourceTerminal = new EntityRef<global::MarketTerminal>(info.msg.deliveryDrone.terminalId);
			this.targetVendingMachine = new EntityRef<global::VendingMachine>(info.msg.deliveryDrone.vendingMachineId);
			this._state = (global::DeliveryDrone.State)info.msg.deliveryDrone.state;
		}
	}

	// Token: 0x06001672 RID: 5746 RVA: 0x00007074 File Offset: 0x00005274
	public override bool CanControl()
	{
		return false;
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x000AAC94 File Offset: 0x000A8E94
	[CompilerGenerated]
	private float <Think>g__CalculatePreferredY|24_0(out bool isBlocked, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		Vector3 toDirection;
		float num;
		this.body.velocity.WithY(0f).ToDirectionAndMagnitude(out toDirection, out num);
		if (num >= 0.5f)
		{
			float num2 = num * 2f;
			float a = this.<Think>g__GetMinimumHeight|24_1(Vector3.zero, ref A_2);
			float b = this.<Think>g__GetMinimumHeight|24_1(new Vector3(0f, 0f, num2 / 2f), ref A_2);
			float b2 = this.<Think>g__GetMinimumHeight|24_1(new Vector3(0f, 0f, num2), ref A_2);
			float num3 = Mathf.Max(Mathf.Max(a, b), b2) + this.preferredCruiseHeight;
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, toDirection);
			Vector3 vector = this.config.halfExtents.WithZ(num2 / 2f);
			Vector3 vector2 = (A_2.currentPosition.WithY(num3) + quaternion * new Vector3(0f, 0f, vector.z / 2f)).WithY(num3 + 1000f);
			RaycastHit raycastHit;
			isBlocked = Physics.BoxCast(vector2, vector, Vector3.down, out raycastHit, quaternion, 1000f, this.config.layerMask);
			float result;
			if (isBlocked)
			{
				Ray ray = new Ray(vector2, Vector3.down);
				Vector3 b3 = ray.ClosestPoint(raycastHit.point);
				float num4 = Vector3.Distance(ray.origin, b3);
				result = num3 + (1000f - num4) + this.preferredHeightAboveObstacle;
			}
			else
			{
				result = num3;
			}
			return result;
		}
		float num5 = this.<Think>g__GetMinimumHeight|24_1(Vector3.zero, ref A_2) + this.preferredCruiseHeight;
		Vector3 origin = A_2.currentPosition.WithY(num5 + 1000f);
		A_2.currentPosition.WithY(num5);
		RaycastHit raycastHit2;
		isBlocked = Physics.Raycast(origin, Vector3.down, out raycastHit2, 1000f, this.config.layerMask);
		if (!isBlocked)
		{
			return num5;
		}
		return num5 + (1000f - raycastHit2.distance) + this.preferredHeightAboveObstacle;
	}

	// Token: 0x06001675 RID: 5749 RVA: 0x000AAE84 File Offset: 0x000A9084
	[CompilerGenerated]
	private float <Think>g__GetMinimumHeight|24_1(Vector3 offset, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		Vector3 vector = base.transform.TransformPoint(offset);
		float height = TerrainMeta.HeightMap.GetHeight(vector);
		float height2 = WaterSystem.GetHeight(vector);
		return Mathf.Max(height, height2);
	}

	// Token: 0x06001676 RID: 5750 RVA: 0x000AAEB6 File Offset: 0x000A90B6
	[CompilerGenerated]
	private Vector3 <Think>g__LandingPosition|24_2(ref global::DeliveryDrone.<>c__DisplayClass24_0 A_1)
	{
		return A_1.marketplace.droneLaunchPoint.position;
	}

	// Token: 0x06001677 RID: 5751 RVA: 0x000AAEC8 File Offset: 0x000A90C8
	[CompilerGenerated]
	private void <Think>g__SetGoalPosition|24_3(Vector3 position, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		this._goToY = null;
		this._stateGoalPosition = new Vector3?(position);
		this.targetPosition = new Vector3?(position);
	}

	// Token: 0x06001678 RID: 5752 RVA: 0x000AAEEE File Offset: 0x000A90EE
	[CompilerGenerated]
	private bool <Think>g__IsAtGoalPosition|24_4(ref global::DeliveryDrone.<>c__DisplayClass24_0 A_1)
	{
		return this._stateGoalPosition != null && Vector3.Distance(this._stateGoalPosition.Value, A_1.currentPosition) < this.targetPositionTolerance;
	}

	// Token: 0x06001679 RID: 5753 RVA: 0x000AAF1D File Offset: 0x000A911D
	[CompilerGenerated]
	private void <Think>g__SetGoToY|24_5(float y, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		this._goToY = new float?(y);
		this.targetPosition = new Vector3?(A_2.currentPosition.WithY(y));
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x000AAF42 File Offset: 0x000A9142
	[CompilerGenerated]
	private bool <Think>g__IsAtGoToY|24_6(ref global::DeliveryDrone.<>c__DisplayClass24_0 A_1)
	{
		return this._goToY != null && Mathf.Abs(this._goToY.Value - A_1.currentPosition.y) < this.targetPositionTolerance;
	}

	// Token: 0x0600167B RID: 5755 RVA: 0x000AAF78 File Offset: 0x000A9178
	[CompilerGenerated]
	private void <Think>g__SetState|24_7(global::DeliveryDrone.State newState, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		this._state = newState;
		this._sinceLastStateChange = 0f;
		this._pickUpTicks = 0;
		this._stateGoalPosition = null;
		this._goToY = null;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this._state >= global::DeliveryDrone.State.AscendBeforeReturn, false, true);
	}

	// Token: 0x04000F56 RID: 3926
	[Header("Delivery Drone")]
	public float stateTimeout = 300f;

	// Token: 0x04000F57 RID: 3927
	public float targetPositionTolerance = 1f;

	// Token: 0x04000F58 RID: 3928
	public float preferredCruiseHeight = 20f;

	// Token: 0x04000F59 RID: 3929
	public float preferredHeightAboveObstacle = 5f;

	// Token: 0x04000F5A RID: 3930
	public float marginAbovePreferredHeight = 3f;

	// Token: 0x04000F5B RID: 3931
	public float obstacleHeightLockDuration = 3f;

	// Token: 0x04000F5C RID: 3932
	public int pickUpDelayInTicks = 3;

	// Token: 0x04000F5D RID: 3933
	public DeliveryDroneConfig config;

	// Token: 0x04000F5E RID: 3934
	public GameObjectRef mapMarkerPrefab;

	// Token: 0x04000F5F RID: 3935
	public EntityRef<Marketplace> sourceMarketplace;

	// Token: 0x04000F60 RID: 3936
	public EntityRef<global::MarketTerminal> sourceTerminal;

	// Token: 0x04000F61 RID: 3937
	public EntityRef<global::VendingMachine> targetVendingMachine;

	// Token: 0x04000F62 RID: 3938
	private global::DeliveryDrone.State _state;

	// Token: 0x04000F63 RID: 3939
	private RealTimeSince _sinceLastStateChange;

	// Token: 0x04000F64 RID: 3940
	private Vector3? _stateGoalPosition;

	// Token: 0x04000F65 RID: 3941
	private float? _goToY;

	// Token: 0x04000F66 RID: 3942
	private TimeSince _sinceLastObstacleBlock;

	// Token: 0x04000F67 RID: 3943
	private float? _minimumYLock;

	// Token: 0x04000F68 RID: 3944
	private int _pickUpTicks;

	// Token: 0x04000F69 RID: 3945
	private global::BaseEntity _mapMarkerInstance;

	// Token: 0x02000BDF RID: 3039
	private enum State
	{
		// Token: 0x04003FFF RID: 16383
		Invalid,
		// Token: 0x04004000 RID: 16384
		Takeoff,
		// Token: 0x04004001 RID: 16385
		FlyToVendingMachine,
		// Token: 0x04004002 RID: 16386
		DescendToVendingMachine,
		// Token: 0x04004003 RID: 16387
		PickUpItems,
		// Token: 0x04004004 RID: 16388
		AscendBeforeReturn,
		// Token: 0x04004005 RID: 16389
		ReturnToTerminal,
		// Token: 0x04004006 RID: 16390
		Landing
	}
}
