using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class ElevatorStatic : Elevator
{
	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x0600150D RID: 5389 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool IsStatic
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x000A5420 File Offset: 0x000A3620
	public override void Spawn()
	{
		base.Spawn();
		base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved1, this.StaticTop, false, true);
		if (!base.IsTop)
		{
			return;
		}
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(base.transform.position, -Vector3.up), 0f, list, 200f, 262144, QueryTriggerInteraction.Collide, null);
		foreach (RaycastHit raycastHit in list)
		{
			if (raycastHit.transform.parent != null)
			{
				ElevatorStatic component = raycastHit.transform.parent.GetComponent<ElevatorStatic>();
				if (component != null && component != this && component.isServer)
				{
					this.floorPositions.Add(component);
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		this.floorPositions.Reverse();
		base.Floor = this.floorPositions.Count;
		for (int i = 0; i < this.floorPositions.Count; i++)
		{
			this.floorPositions[i].SetFloorDetails(i, this);
		}
	}

	// Token: 0x0600150F RID: 5391 RVA: 0x000A556C File Offset: 0x000A376C
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		base.UpdateChildEntities(base.IsTop);
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x000A5580 File Offset: 0x000A3780
	protected override bool IsValidFloor(int targetFloor)
	{
		return targetFloor >= 0 && targetFloor <= base.Floor;
	}

	// Token: 0x06001511 RID: 5393 RVA: 0x000A5594 File Offset: 0x000A3794
	protected override Vector3 GetWorldSpaceFloorPosition(int targetFloor)
	{
		if (targetFloor == base.Floor)
		{
			return base.transform.position + Vector3.up * 1f;
		}
		Vector3 position = base.transform.position;
		position.y = this.floorPositions[targetFloor].transform.position.y + 1f;
		return position;
	}

	// Token: 0x06001512 RID: 5394 RVA: 0x000A55FF File Offset: 0x000A37FF
	public void SetFloorDetails(int floor, ElevatorStatic owner)
	{
		this.ownerElevator = owner;
		base.Floor = floor;
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x000A5610 File Offset: 0x000A3810
	protected override void CallElevator()
	{
		if (this.ownerElevator != null)
		{
			float num;
			this.ownerElevator.RequestMoveLiftTo(base.Floor, out num);
			return;
		}
		if (base.IsTop)
		{
			float num2;
			base.RequestMoveLiftTo(base.Floor, out num2);
		}
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x000A5657 File Offset: 0x000A3857
	private ElevatorStatic ElevatorAtFloor(int floor)
	{
		if (floor == base.Floor)
		{
			return this;
		}
		if (floor >= 0 && floor < this.floorPositions.Count)
		{
			return this.floorPositions[floor];
		}
		return null;
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x000A5684 File Offset: 0x000A3884
	protected override void OnMoveBegin()
	{
		base.OnMoveBegin();
		ElevatorStatic elevatorStatic = this.ElevatorAtFloor(base.LiftPositionToFloor());
		if (elevatorStatic != null)
		{
			elevatorStatic.OnLiftLeavingFloor();
		}
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x000A56B3 File Offset: 0x000A38B3
	private void OnLiftLeavingFloor()
	{
		this.ClearPowerOutput();
		if (base.IsInvoking(new Action(this.ClearPowerOutput)))
		{
			base.CancelInvoke(new Action(this.ClearPowerOutput));
		}
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x000A56E4 File Offset: 0x000A38E4
	protected override void ClearBusy()
	{
		base.ClearBusy();
		ElevatorStatic elevatorStatic = this.ElevatorAtFloor(base.LiftPositionToFloor());
		if (elevatorStatic != null)
		{
			elevatorStatic.OnLiftArrivedAtFloor();
		}
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x000A5713 File Offset: 0x000A3913
	protected override void OnLiftCalledWhenAtTargetFloor()
	{
		base.OnLiftCalledWhenAtTargetFloor();
		this.OnLiftArrivedAtFloor();
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x000A5721 File Offset: 0x000A3921
	private void OnLiftArrivedAtFloor()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
		this.MarkDirty();
		base.Invoke(new Action(this.ClearPowerOutput), 10f);
	}

	// Token: 0x0600151A RID: 5402 RVA: 0x000833AA File Offset: 0x000815AA
	private void ClearPowerOutput()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x0600151B RID: 5403 RVA: 0x000A574E File Offset: 0x000A394E
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.HasFlag(BaseEntity.Flags.Reserved3))
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x000A5760 File Offset: 0x000A3960
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		}
	}

	// Token: 0x04000DAB RID: 3499
	public bool StaticTop;

	// Token: 0x04000DAC RID: 3500
	private const BaseEntity.Flags LiftRecentlyArrived = BaseEntity.Flags.Reserved3;

	// Token: 0x04000DAD RID: 3501
	private List<ElevatorStatic> floorPositions = new List<ElevatorStatic>();

	// Token: 0x04000DAE RID: 3502
	private ElevatorStatic ownerElevator;
}
