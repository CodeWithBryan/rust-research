using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004A0 RID: 1184
public class DoorManipulator : IOEntity
{
	// Token: 0x0600265E RID: 9822 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool PairWithLockedDoors()
	{
		return true;
	}

	// Token: 0x0600265F RID: 9823 RVA: 0x000EEE04 File Offset: 0x000ED004
	public virtual void SetTargetDoor(Door newTargetDoor)
	{
		UnityEngine.Object x = this.targetDoor;
		this.targetDoor = newTargetDoor;
		base.SetFlag(BaseEntity.Flags.On, this.targetDoor != null, false, true);
		this.entityRef.Set(newTargetDoor);
		if (x != this.targetDoor && this.targetDoor != null)
		{
			this.DoAction();
		}
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x000EEE60 File Offset: 0x000ED060
	public virtual void SetupInitialDoorConnection()
	{
		if (this.targetDoor == null && !this.entityRef.IsValid(true))
		{
			this.SetTargetDoor(this.FindDoor(this.PairWithLockedDoors()));
		}
		if (this.targetDoor != null && !this.entityRef.IsValid(true))
		{
			this.entityRef.Set(this.targetDoor);
		}
		if (this.entityRef.IsValid(true) && this.targetDoor == null)
		{
			this.SetTargetDoor(this.entityRef.Get(true).GetComponent<Door>());
		}
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x000EEEFB File Offset: 0x000ED0FB
	public override void Init()
	{
		base.Init();
		this.SetupInitialDoorConnection();
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x000EEF0C File Offset: 0x000ED10C
	public Door FindDoor(bool allowLocked = true)
	{
		List<Door> list = Pool.GetList<Door>();
		Vis.Entities<Door>(base.transform.position, 1f, list, 2097152, QueryTriggerInteraction.Ignore);
		Door result = null;
		float num = float.PositiveInfinity;
		foreach (Door door in list)
		{
			if (door.isServer)
			{
				if (!allowLocked)
				{
					BaseLock baseLock = door.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
					if (baseLock != null && baseLock.IsLocked())
					{
						continue;
					}
				}
				float num2 = Vector3.Distance(door.transform.position, base.transform.position);
				if (num2 < num)
				{
					result = door;
					num = num2;
				}
			}
		}
		Pool.FreeList<Door>(ref list);
		return result;
	}

	// Token: 0x06002663 RID: 9827 RVA: 0x000EEFE0 File Offset: 0x000ED1E0
	public virtual void DoActionDoorMissing()
	{
		this.SetTargetDoor(this.FindDoor(this.PairWithLockedDoors()));
	}

	// Token: 0x06002664 RID: 9828 RVA: 0x000EEFF4 File Offset: 0x000ED1F4
	public void DoAction()
	{
		bool flag = this.IsPowered();
		if (this.targetDoor == null)
		{
			this.DoActionDoorMissing();
		}
		if (this.targetDoor != null)
		{
			if (this.targetDoor.IsBusy())
			{
				base.Invoke(new Action(this.DoAction), 1f);
				return;
			}
			if (this.powerAction == DoorManipulator.DoorEffect.Open)
			{
				if (flag)
				{
					if (!this.targetDoor.IsOpen())
					{
						this.targetDoor.SetOpen(true, false);
						return;
					}
				}
				else if (this.targetDoor.IsOpen())
				{
					this.targetDoor.SetOpen(false, false);
					return;
				}
			}
			else if (this.powerAction == DoorManipulator.DoorEffect.Close)
			{
				if (flag)
				{
					if (this.targetDoor.IsOpen())
					{
						this.targetDoor.SetOpen(false, false);
						return;
					}
				}
				else if (!this.targetDoor.IsOpen())
				{
					this.targetDoor.SetOpen(true, false);
					return;
				}
			}
			else if (this.powerAction == DoorManipulator.DoorEffect.Toggle)
			{
				if (flag && this.toggle)
				{
					this.targetDoor.SetOpen(!this.targetDoor.IsOpen(), false);
					this.toggle = false;
					return;
				}
				if (!this.toggle)
				{
					this.toggle = true;
				}
			}
		}
	}

	// Token: 0x06002665 RID: 9829 RVA: 0x000EF120 File Offset: 0x000ED320
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		this.DoAction();
	}

	// Token: 0x06002666 RID: 9830 RVA: 0x000EF130 File Offset: 0x000ED330
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericEntRef1 = this.entityRef.uid;
		info.msg.ioEntity.genericInt1 = (int)this.powerAction;
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x000EF16C File Offset: 0x000ED36C
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.entityRef.uid = info.msg.ioEntity.genericEntRef1;
			this.powerAction = (DoorManipulator.DoorEffect)info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x04001F19 RID: 7961
	public EntityRef entityRef;

	// Token: 0x04001F1A RID: 7962
	public Door targetDoor;

	// Token: 0x04001F1B RID: 7963
	public DoorManipulator.DoorEffect powerAction;

	// Token: 0x04001F1C RID: 7964
	private bool toggle = true;

	// Token: 0x02000CC6 RID: 3270
	public enum DoorEffect
	{
		// Token: 0x040043C4 RID: 17348
		Close,
		// Token: 0x040043C5 RID: 17349
		Open,
		// Token: 0x040043C6 RID: 17350
		Toggle
	}
}
