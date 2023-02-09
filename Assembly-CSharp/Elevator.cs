using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class Elevator : global::IOEntity, IFlagNotify
{
	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x060014EC RID: 5356 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual bool IsStatic
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x060014ED RID: 5357 RVA: 0x000A4B0E File Offset: 0x000A2D0E
	// (set) Token: 0x060014EE RID: 5358 RVA: 0x000A4B16 File Offset: 0x000A2D16
	public int Floor { get; set; }

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x060014EF RID: 5359 RVA: 0x00020A80 File Offset: 0x0001EC80
	protected bool IsTop
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved1);
		}
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x000A4B20 File Offset: 0x000A2D20
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.elevator != null)
		{
			this.Floor = info.msg.elevator.floor;
		}
		if (this.FloorBlockerVolume != null)
		{
			this.FloorBlockerVolume.SetActive(this.Floor > 0);
		}
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x000A4B7C File Offset: 0x000A2D7C
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		global::Elevator elevatorInDirection = this.GetElevatorInDirection(global::Elevator.Direction.Down);
		if (elevatorInDirection != null)
		{
			elevatorInDirection.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
			this.Floor = elevatorInDirection.Floor + 1;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x000A4BCD File Offset: 0x000A2DCD
	protected virtual void CallElevator()
	{
		base.EntityLinkBroadcast<global::Elevator, ConstructionSocket>(delegate(global::Elevator elevatorEnt)
		{
			if (elevatorEnt.IsTop)
			{
				float num;
				elevatorEnt.RequestMoveLiftTo(this.Floor, out num);
			}
		}, (ConstructionSocket socket) => socket.socketType == ConstructionSocket.Type.Elevator);
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x000A4C00 File Offset: 0x000A2E00
	public void Server_RaiseLowerElevator(global::Elevator.Direction dir, bool goTopBottom)
	{
		if (base.IsBusy())
		{
			return;
		}
		int num = this.LiftPositionToFloor();
		if (dir != global::Elevator.Direction.Up)
		{
			if (dir == global::Elevator.Direction.Down)
			{
				num--;
				if (goTopBottom)
				{
					num = 0;
				}
			}
		}
		else
		{
			num++;
			if (goTopBottom)
			{
				num = this.Floor;
			}
		}
		float num2;
		this.RequestMoveLiftTo(num, out num2);
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x000A4C4C File Offset: 0x000A2E4C
	protected bool RequestMoveLiftTo(int targetFloor, out float timeToTravel)
	{
		timeToTravel = 0f;
		if (base.IsBusy())
		{
			return false;
		}
		if (!this.IsStatic && this.ioEntity != null && !this.ioEntity.IsPowered())
		{
			return false;
		}
		if (!this.IsValidFloor(targetFloor))
		{
			return false;
		}
		if (!this.liftEntity.CanMove())
		{
			return false;
		}
		if (this.LiftPositionToFloor() == targetFloor)
		{
			this.OnLiftCalledWhenAtTargetFloor();
			return false;
		}
		Vector3 worldSpaceFloorPosition = this.GetWorldSpaceFloorPosition(targetFloor);
		if (!GamePhysics.LineOfSight(this.liftEntity.transform.position, worldSpaceFloorPosition, 2097152, null))
		{
			return false;
		}
		this.OnMoveBegin();
		Vector3 vector = base.transform.InverseTransformPoint(worldSpaceFloorPosition);
		timeToTravel = this.TimeToTravelDistance(Mathf.Abs(this.liftEntity.transform.localPosition.y - vector.y));
		LeanTween.moveLocalY(this.liftEntity.gameObject, vector.y, timeToTravel);
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		if (targetFloor < this.Floor)
		{
			this.liftEntity.ToggleHurtTrigger(true);
		}
		base.Invoke(new Action(this.ClearBusy), timeToTravel);
		if (this.ioEntity != null)
		{
			this.ioEntity.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
			this.ioEntity.SendChangedToRoot(true);
		}
		return true;
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnLiftCalledWhenAtTargetFloor()
	{
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnMoveBegin()
	{
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x000A4D9D File Offset: 0x000A2F9D
	private float TimeToTravelDistance(float distance)
	{
		return distance / this.LiftSpeedPerMetre;
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x000A4DA8 File Offset: 0x000A2FA8
	protected virtual Vector3 GetWorldSpaceFloorPosition(int targetFloor)
	{
		int num = this.Floor - targetFloor;
		Vector3 b = Vector3.up * ((float)num * this.FloorHeight);
		b.y -= 1f;
		return base.transform.position - b;
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x060014F9 RID: 5369 RVA: 0x000A4DF3 File Offset: 0x000A2FF3
	protected virtual float FloorHeight
	{
		get
		{
			return 3f;
		}
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x000A4DFC File Offset: 0x000A2FFC
	protected virtual void ClearBusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		if (this.liftEntity != null)
		{
			this.liftEntity.ToggleHurtTrigger(false);
		}
		if (this.ioEntity != null)
		{
			this.ioEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			this.ioEntity.SendChangedToRoot(true);
		}
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x000A4E5E File Offset: 0x000A305E
	protected virtual bool IsValidFloor(int targetFloor)
	{
		return targetFloor <= this.Floor && targetFloor >= 0;
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x000A4E74 File Offset: 0x000A3074
	private global::Elevator GetElevatorInDirection(global::Elevator.Direction dir)
	{
		EntityLink entityLink = base.FindLink((dir == global::Elevator.Direction.Down) ? "elevator/sockets/elevator-male" : "elevator/sockets/elevator-female");
		if (entityLink != null && !entityLink.IsEmpty())
		{
			global::BaseEntity owner = entityLink.connections[0].owner;
			global::Elevator elevator;
			if (owner != null && owner.isServer && (elevator = (owner as global::Elevator)) != null && elevator != this)
			{
				return elevator;
			}
		}
		return null;
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x000A4EDC File Offset: 0x000A30DC
	public void UpdateChildEntities(bool isTop)
	{
		if (isTop)
		{
			if (this.liftEntity == null)
			{
				this.FindExistingLiftChild();
			}
			if (this.liftEntity == null)
			{
				this.liftEntity = (GameManager.server.CreateEntity(this.LiftEntityPrefab.resourcePath, this.GetWorldSpaceFloorPosition(this.Floor), this.LiftRoot.rotation, true) as ElevatorLift);
				this.liftEntity.SetParent(this, true, false);
				this.liftEntity.Spawn();
			}
			if (this.ioEntity == null)
			{
				this.FindExistingIOChild();
			}
			if (this.ioEntity == null && this.IoEntityPrefab.isValid)
			{
				this.ioEntity = (GameManager.server.CreateEntity(this.IoEntityPrefab.resourcePath, this.IoEntitySpawnPoint.position, this.IoEntitySpawnPoint.rotation, true) as global::IOEntity);
				this.ioEntity.SetParent(this, true, false);
				this.ioEntity.Spawn();
				return;
			}
		}
		else
		{
			if (this.liftEntity != null)
			{
				this.liftEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			if (this.ioEntity != null)
			{
				this.ioEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x000A501C File Offset: 0x000A321C
	private void FindExistingIOChild()
	{
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::IOEntity ioentity;
				if ((ioentity = (enumerator.Current as global::IOEntity)) != null)
				{
					this.ioEntity = ioentity;
					break;
				}
			}
		}
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x000A507C File Offset: 0x000A327C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.elevator == null)
		{
			info.msg.elevator = Pool.Get<ProtoBuf.Elevator>();
		}
		info.msg.elevator.floor = this.Floor;
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x000A50B8 File Offset: 0x000A32B8
	protected int LiftPositionToFloor()
	{
		Vector3 position = this.liftEntity.transform.position;
		int result = -1;
		float num = float.MaxValue;
		for (int i = 0; i <= this.Floor; i++)
		{
			float num2 = Vector3.Distance(this.GetWorldSpaceFloorPosition(i), position);
			if (num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x000A5109 File Offset: 0x000A3309
	public override void DestroyShared()
	{
		this.Cleanup();
		base.DestroyShared();
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x000A5118 File Offset: 0x000A3318
	private void Cleanup()
	{
		global::Elevator elevatorInDirection = this.GetElevatorInDirection(global::Elevator.Direction.Down);
		if (elevatorInDirection != null)
		{
			elevatorInDirection.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
		}
		global::Elevator elevatorInDirection2 = this.GetElevatorInDirection(global::Elevator.Direction.Up);
		if (elevatorInDirection2 != null)
		{
			elevatorInDirection2.Kill(global::BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x000A515C File Offset: 0x000A335C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		this.UpdateChildEntities(this.IsTop);
		if (this.ioEntity != null)
		{
			this.ioEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		}
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x000A51AA File Offset: 0x000A33AA
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputAmount > 0 && this.previousPowerAmount[inputSlot] == 0)
		{
			this.CallElevator();
		}
		this.previousPowerAmount[inputSlot] = inputAmount;
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x000A51D1 File Offset: 0x000A33D1
	private void OnPhysicsNeighbourChanged()
	{
		if (this.IsStatic)
		{
			return;
		}
		if (this.GetElevatorInDirection(global::Elevator.Direction.Down) == null && !this.HasFloorSocketConnection())
		{
			base.Kill(global::BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x000A51FC File Offset: 0x000A33FC
	private bool HasFloorSocketConnection()
	{
		EntityLink entityLink = base.FindLink("elevator/sockets/block-male");
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x000A5224 File Offset: 0x000A3424
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (!Rust.Application.isLoading && base.isServer && old.HasFlag(global::BaseEntity.Flags.Reserved1) != next.HasFlag(global::BaseEntity.Flags.Reserved1))
		{
			this.UpdateChildEntities(next.HasFlag(global::BaseEntity.Flags.Reserved1));
		}
		if (old.HasFlag(global::BaseEntity.Flags.Busy) != next.HasFlag(global::BaseEntity.Flags.Busy))
		{
			if (this.liftEntity == null)
			{
				this.FindExistingLiftChild();
			}
			if (this.liftEntity != null)
			{
				this.liftEntity.ToggleMovementCollider(!next.HasFlag(global::BaseEntity.Flags.Busy));
			}
		}
		if (old.HasFlag(global::BaseEntity.Flags.Reserved1) != next.HasFlag(global::BaseEntity.Flags.Reserved1) && this.FloorBlockerVolume != null)
		{
			this.FloorBlockerVolume.SetActive(next.HasFlag(global::BaseEntity.Flags.Reserved1));
		}
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x000A535C File Offset: 0x000A355C
	private void FindExistingLiftChild()
	{
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ElevatorLift elevatorLift;
				if ((elevatorLift = (enumerator.Current as ElevatorLift)) != null)
				{
					this.liftEntity = elevatorLift;
					break;
				}
			}
		}
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x000A53BC File Offset: 0x000A35BC
	public void OnFlagToggled(bool state)
	{
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved2, state, false, true);
		}
	}

	// Token: 0x04000D98 RID: 3480
	public Transform LiftRoot;

	// Token: 0x04000D99 RID: 3481
	public GameObjectRef LiftEntityPrefab;

	// Token: 0x04000D9A RID: 3482
	public GameObjectRef IoEntityPrefab;

	// Token: 0x04000D9B RID: 3483
	public Transform IoEntitySpawnPoint;

	// Token: 0x04000D9C RID: 3484
	public GameObject FloorBlockerVolume;

	// Token: 0x04000D9D RID: 3485
	public float LiftSpeedPerMetre = 1f;

	// Token: 0x04000D9E RID: 3486
	public GameObject[] PoweredObjects;

	// Token: 0x04000D9F RID: 3487
	public MeshRenderer PoweredMesh;

	// Token: 0x04000DA0 RID: 3488
	[ColorUsage(true, true)]
	public Color PoweredLightColour;

	// Token: 0x04000DA1 RID: 3489
	[ColorUsage(true, true)]
	public Color UnpoweredLightColour;

	// Token: 0x04000DA2 RID: 3490
	public SkinnedMeshRenderer[] CableRenderers;

	// Token: 0x04000DA3 RID: 3491
	public LODGroup CableLod;

	// Token: 0x04000DA4 RID: 3492
	public Transform CableRoot;

	// Token: 0x04000DA6 RID: 3494
	protected const global::BaseEntity.Flags TopFloorFlag = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000DA7 RID: 3495
	public const global::BaseEntity.Flags ElevatorPowered = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000DA8 RID: 3496
	private ElevatorLift liftEntity;

	// Token: 0x04000DA9 RID: 3497
	private global::IOEntity ioEntity;

	// Token: 0x04000DAA RID: 3498
	private int[] previousPowerAmount = new int[2];

	// Token: 0x02000BD2 RID: 3026
	public enum Direction
	{
		// Token: 0x04003FCC RID: 16332
		Up,
		// Token: 0x04003FCD RID: 16333
		Down
	}
}
