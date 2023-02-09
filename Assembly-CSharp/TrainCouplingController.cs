using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200048F RID: 1167
public class TrainCouplingController
{
	// Token: 0x170002FB RID: 763
	// (get) Token: 0x060025E1 RID: 9697 RVA: 0x000EC8C7 File Offset: 0x000EAAC7
	public bool IsCoupled
	{
		get
		{
			return this.IsFrontCoupled || this.IsRearCoupled;
		}
	}

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x060025E2 RID: 9698 RVA: 0x000EC8D9 File Offset: 0x000EAAD9
	public bool IsFrontCoupled
	{
		get
		{
			return this.owner.HasFlag(BaseEntity.Flags.Reserved2);
		}
	}

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x060025E3 RID: 9699 RVA: 0x000EC8EB File Offset: 0x000EAAEB
	public bool IsRearCoupled
	{
		get
		{
			return this.owner.HasFlag(BaseEntity.Flags.Reserved3);
		}
	}

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x060025E4 RID: 9700 RVA: 0x000EC8FD File Offset: 0x000EAAFD
	// (set) Token: 0x060025E5 RID: 9701 RVA: 0x000EC905 File Offset: 0x000EAB05
	public float PreChangeTrackSpeed { get; private set; }

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x060025E6 RID: 9702 RVA: 0x000EC90E File Offset: 0x000EAB0E
	// (set) Token: 0x060025E7 RID: 9703 RVA: 0x000EC916 File Offset: 0x000EAB16
	public bool PreChangeCoupledBackwards { get; private set; }

	// Token: 0x060025E8 RID: 9704 RVA: 0x000EC920 File Offset: 0x000EAB20
	public TrainCouplingController(TrainCar owner)
	{
		this.owner = owner;
		this.frontCoupling = new TrainCoupling(owner, true, this, owner.frontCoupling, owner.frontCouplingPivot, BaseEntity.Flags.Reserved2);
		this.rearCoupling = new TrainCoupling(owner, false, this, owner.rearCoupling, owner.rearCouplingPivot, BaseEntity.Flags.Reserved3);
	}

	// Token: 0x060025E9 RID: 9705 RVA: 0x000EC978 File Offset: 0x000EAB78
	public bool IsCoupledTo(TrainCar them)
	{
		return this.frontCoupling.IsCoupledTo(them) || this.rearCoupling.IsCoupledTo(them);
	}

	// Token: 0x060025EA RID: 9706 RVA: 0x000EC998 File Offset: 0x000EAB98
	public bool TryCouple(TrainCar them, TriggerTrainCollisions.Location ourLocation)
	{
		TrainCoupling trainCoupling = (ourLocation == TriggerTrainCollisions.Location.Front) ? this.frontCoupling : this.rearCoupling;
		if (!trainCoupling.isValid)
		{
			return false;
		}
		if (trainCoupling.IsCoupled)
		{
			return false;
		}
		if (trainCoupling.timeSinceCouplingBlock < 1.5f)
		{
			return false;
		}
		float num = Vector3.Angle(this.owner.transform.forward, them.transform.forward);
		if (num > 25f && num < 155f)
		{
			return false;
		}
		bool flag = num < 90f;
		TrainCoupling trainCoupling2;
		if (flag)
		{
			trainCoupling2 = ((ourLocation == TriggerTrainCollisions.Location.Front) ? them.coupling.rearCoupling : them.coupling.frontCoupling);
		}
		else
		{
			trainCoupling2 = ((ourLocation == TriggerTrainCollisions.Location.Front) ? them.coupling.frontCoupling : them.coupling.rearCoupling);
		}
		float num2 = them.GetTrackSpeed();
		if (!flag)
		{
			num2 = -num2;
		}
		if (Mathf.Abs(num2 - this.owner.GetTrackSpeed()) > TrainCouplingController.max_couple_speed)
		{
			trainCoupling.timeSinceCouplingBlock = 0f;
			trainCoupling2.timeSinceCouplingBlock = 0f;
			return false;
		}
		if (!trainCoupling2.isValid)
		{
			return false;
		}
		if (Vector3.SqrMagnitude(trainCoupling.couplingPoint.position - trainCoupling2.couplingPoint.position) > 0.5f)
		{
			return false;
		}
		TrainTrackSpline frontTrackSection = this.owner.FrontTrackSection;
		TrainTrackSpline frontTrackSection2 = them.FrontTrackSection;
		return (!(frontTrackSection2 != frontTrackSection) || frontTrackSection.HasConnectedTrack(frontTrackSection2)) && trainCoupling.TryCouple(trainCoupling2, true);
	}

	// Token: 0x060025EB RID: 9707 RVA: 0x000ECB06 File Offset: 0x000EAD06
	public void Uncouple(bool front)
	{
		if (front)
		{
			this.frontCoupling.Uncouple(true);
			return;
		}
		this.rearCoupling.Uncouple(true);
	}

	// Token: 0x060025EC RID: 9708 RVA: 0x000ECB24 File Offset: 0x000EAD24
	public void GetAll(ref List<TrainCar> result)
	{
		result.Add(this.owner);
		TrainCoupling trainCoupling = this.rearCoupling.CoupledTo;
		while (trainCoupling != null && trainCoupling.IsCoupled && !result.Contains(trainCoupling.owner))
		{
			result.Insert(0, trainCoupling.owner);
			trainCoupling = trainCoupling.GetOppositeCoupling();
			trainCoupling = trainCoupling.CoupledTo;
		}
		TrainCoupling trainCoupling2 = this.frontCoupling.CoupledTo;
		while (trainCoupling2 != null && trainCoupling2.IsCoupled && !result.Contains(trainCoupling2.owner))
		{
			result.Add(trainCoupling2.owner);
			trainCoupling2 = trainCoupling2.GetOppositeCoupling();
			trainCoupling2 = trainCoupling2.CoupledTo;
		}
	}

	// Token: 0x060025ED RID: 9709 RVA: 0x000ECBC5 File Offset: 0x000EADC5
	public void OnPreCouplingChange()
	{
		this.PreChangeCoupledBackwards = this.owner.IsCoupledBackwards();
		this.PreChangeTrackSpeed = this.owner.GetTrackSpeed();
		if (this.PreChangeCoupledBackwards)
		{
			this.PreChangeTrackSpeed = -this.PreChangeTrackSpeed;
		}
	}

	// Token: 0x04001EB8 RID: 7864
	public const BaseEntity.Flags Flag_CouplingFront = BaseEntity.Flags.Reserved2;

	// Token: 0x04001EB9 RID: 7865
	public const BaseEntity.Flags Flag_CouplingRear = BaseEntity.Flags.Reserved3;

	// Token: 0x04001EBA RID: 7866
	public readonly TrainCoupling frontCoupling;

	// Token: 0x04001EBB RID: 7867
	public readonly TrainCoupling rearCoupling;

	// Token: 0x04001EBC RID: 7868
	private readonly TrainCar owner;

	// Token: 0x04001EBD RID: 7869
	[ServerVar(Help = "Maximum difference in velocity for train cars to couple")]
	public static float max_couple_speed = 9f;
}
