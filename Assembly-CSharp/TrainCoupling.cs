using System;
using UnityEngine;

// Token: 0x0200048E RID: 1166
public class TrainCoupling
{
	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x060025D5 RID: 9685 RVA: 0x000EC6A3 File Offset: 0x000EA8A3
	public bool IsCoupled
	{
		get
		{
			return this.owner.HasFlag(this.flag);
		}
	}

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x060025D6 RID: 9686 RVA: 0x000EC6B6 File Offset: 0x000EA8B6
	public bool IsUncoupled
	{
		get
		{
			return !this.owner.HasFlag(this.flag);
		}
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x000EC6CC File Offset: 0x000EA8CC
	public TrainCoupling(TrainCar owner, bool isFrontCoupling, TrainCouplingController controller) : this(owner, isFrontCoupling, controller, null, null, BaseEntity.Flags.Placeholder)
	{
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x000EC6DC File Offset: 0x000EA8DC
	public TrainCoupling(TrainCar owner, bool isFrontCoupling, TrainCouplingController controller, Transform couplingPoint, Transform couplingPivot, BaseEntity.Flags flag)
	{
		this.owner = owner;
		this.isFrontCoupling = isFrontCoupling;
		this.controller = controller;
		this.couplingPoint = couplingPoint;
		this.couplingPivot = couplingPivot;
		this.flag = flag;
		this.isValid = (couplingPoint != null);
	}

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x060025D9 RID: 9689 RVA: 0x000EC72A File Offset: 0x000EA92A
	// (set) Token: 0x060025DA RID: 9690 RVA: 0x000EC732 File Offset: 0x000EA932
	public TrainCoupling CoupledTo { get; private set; }

	// Token: 0x060025DB RID: 9691 RVA: 0x000EC73B File Offset: 0x000EA93B
	public bool IsCoupledTo(TrainCar them)
	{
		return this.CoupledTo != null && this.CoupledTo.owner == them;
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x000EC758 File Offset: 0x000EA958
	public bool IsCoupledTo(TrainCoupling them)
	{
		return this.CoupledTo != null && this.CoupledTo == them;
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x000EC770 File Offset: 0x000EA970
	public bool TryCouple(TrainCoupling theirCoupling, bool reflect)
	{
		if (!this.isValid)
		{
			return false;
		}
		if (this.CoupledTo == theirCoupling)
		{
			return true;
		}
		if (this.IsCoupled)
		{
			return false;
		}
		if (reflect && !theirCoupling.TryCouple(this, false))
		{
			return false;
		}
		this.controller.OnPreCouplingChange();
		this.CoupledTo = theirCoupling;
		this.owner.SetFlag(this.flag, true, false, false);
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x000EC7E0 File Offset: 0x000EA9E0
	public void Uncouple(bool reflect)
	{
		if (this.IsUncoupled)
		{
			return;
		}
		if (reflect && this.CoupledTo != null)
		{
			this.CoupledTo.Uncouple(false);
		}
		this.controller.OnPreCouplingChange();
		this.CoupledTo = null;
		this.owner.SetFlag(this.flag, false, false, false);
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		this.timeSinceCouplingBlock = 0f;
	}

	// Token: 0x060025DF RID: 9695 RVA: 0x000EC84F File Offset: 0x000EAA4F
	public TrainCoupling GetOppositeCoupling()
	{
		if (!this.isFrontCoupling)
		{
			return this.controller.frontCoupling;
		}
		return this.controller.rearCoupling;
	}

	// Token: 0x060025E0 RID: 9696 RVA: 0x000EC870 File Offset: 0x000EAA70
	public bool TryGetCoupledToID(out uint id)
	{
		if (this.CoupledTo != null && this.CoupledTo.owner != null && this.CoupledTo.owner.IsValid())
		{
			id = this.CoupledTo.owner.net.ID;
			return true;
		}
		id = 0U;
		return false;
	}

	// Token: 0x04001EAD RID: 7853
	public readonly TrainCar owner;

	// Token: 0x04001EAE RID: 7854
	public readonly bool isFrontCoupling;

	// Token: 0x04001EAF RID: 7855
	public readonly TrainCouplingController controller;

	// Token: 0x04001EB0 RID: 7856
	public readonly Transform couplingPoint;

	// Token: 0x04001EB1 RID: 7857
	public readonly Transform couplingPivot;

	// Token: 0x04001EB2 RID: 7858
	public readonly BaseEntity.Flags flag;

	// Token: 0x04001EB3 RID: 7859
	public readonly bool isValid;

	// Token: 0x04001EB5 RID: 7861
	public TimeSince timeSinceCouplingBlock;
}
