using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x02000485 RID: 1157
public class CompleteTrain : IDisposable
{
	// Token: 0x170002ED RID: 749
	// (get) Token: 0x06002585 RID: 9605 RVA: 0x000EA230 File Offset: 0x000E8430
	// (set) Token: 0x06002586 RID: 9606 RVA: 0x000EA238 File Offset: 0x000E8438
	public TrainCar PrimaryTrainCar { get; private set; }

	// Token: 0x170002EE RID: 750
	// (get) Token: 0x06002587 RID: 9607 RVA: 0x000EA241 File Offset: 0x000E8441
	public bool TrainIsReversing
	{
		get
		{
			return this.PrimaryTrainCar != this.trainCars[0];
		}
	}

	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06002588 RID: 9608 RVA: 0x000EA25A File Offset: 0x000E845A
	// (set) Token: 0x06002589 RID: 9609 RVA: 0x000EA262 File Offset: 0x000E8462
	public float TotalForces { get; private set; }

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x0600258A RID: 9610 RVA: 0x000EA26B File Offset: 0x000E846B
	// (set) Token: 0x0600258B RID: 9611 RVA: 0x000EA273 File Offset: 0x000E8473
	public float TotalMass { get; private set; }

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x0600258C RID: 9612 RVA: 0x000EA27C File Offset: 0x000E847C
	public int NumTrainCars
	{
		get
		{
			return this.trainCars.Count;
		}
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x0600258D RID: 9613 RVA: 0x000EA289 File Offset: 0x000E8489
	// (set) Token: 0x0600258E RID: 9614 RVA: 0x000EA291 File Offset: 0x000E8491
	public int LinedUpToUnload { get; private set; } = -1;

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x0600258F RID: 9615 RVA: 0x000EA29A File Offset: 0x000E849A
	public bool IsLinedUpToUnload
	{
		get
		{
			return this.LinedUpToUnload >= 0;
		}
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x000EA2A8 File Offset: 0x000E84A8
	public CompleteTrain(TrainCar trainCar)
	{
		List<TrainCar> list = Facepunch.Pool.GetList<TrainCar>();
		list.Add(trainCar);
		this.Init(list);
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x000EA318 File Offset: 0x000E8518
	public CompleteTrain(List<TrainCar> allTrainCars)
	{
		this.Init(allTrainCars);
	}

	// Token: 0x06002592 RID: 9618 RVA: 0x000EA37C File Offset: 0x000E857C
	private void Init(List<TrainCar> allTrainCars)
	{
		this.trainCars = allTrainCars;
		this.timeSinceLastChange = 0f;
		this.lastMovingTime = UnityEngine.Time.time;
		float num = 0f;
		this.PrimaryTrainCar = this.trainCars[0];
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			TrainCar trainCar = this.trainCars[i];
			if (trainCar.completeTrain != this)
			{
				if (trainCar.completeTrain != null)
				{
					bool flag = this.IsCoupledBackwards(i);
					bool preChangeCoupledBackwards = trainCar.coupling.PreChangeCoupledBackwards;
					float preChangeTrackSpeed = trainCar.coupling.PreChangeTrackSpeed;
					if (flag != preChangeCoupledBackwards)
					{
						num -= preChangeTrackSpeed;
					}
					else
					{
						num += preChangeTrackSpeed;
					}
				}
				trainCar.SetNewCompleteTrain(this);
			}
		}
		this.trackSpeed = num / (float)this.trainCars.Count;
		this.prevTrackSpeed = this.trackSpeed;
		this.ParamsTick();
	}

	// Token: 0x06002593 RID: 9619 RVA: 0x000EA458 File Offset: 0x000E8658
	~CompleteTrain()
	{
		this.Cleanup();
	}

	// Token: 0x06002594 RID: 9620 RVA: 0x000EA484 File Offset: 0x000E8684
	public void Dispose()
	{
		this.Cleanup();
		System.GC.SuppressFinalize(this);
	}

	// Token: 0x06002595 RID: 9621 RVA: 0x000EA492 File Offset: 0x000E8692
	private void Cleanup()
	{
		if (this.disposed)
		{
			return;
		}
		this.EndShunting(CoalingTower.ActionAttemptStatus.GenericError);
		this.disposed = true;
		Facepunch.Pool.FreeList<TrainCar>(ref this.trainCars);
	}

	// Token: 0x06002596 RID: 9622 RVA: 0x000EA4B8 File Offset: 0x000E86B8
	public void RemoveTrainCar(TrainCar trainCar)
	{
		if (this.disposed)
		{
			return;
		}
		if (this.trainCars.Count <= 1)
		{
			Debug.LogWarning(base.GetType().Name + ": Can't remove car from CompleteTrain of length one.");
			return;
		}
		int num = this.IndexOf(trainCar);
		bool flag;
		if (num == 0)
		{
			flag = this.IsCoupledBackwards(1);
		}
		else
		{
			flag = this.IsCoupledBackwards(0);
		}
		this.trainCars.RemoveAt(num);
		this.timeSinceLastChange = 0f;
		this.LinedUpToUnload = -1;
		if (this.IsCoupledBackwards(0) != flag)
		{
			this.trackSpeed *= -1f;
		}
	}

	// Token: 0x06002597 RID: 9623 RVA: 0x000EA554 File Offset: 0x000E8754
	public float GetTrackSpeedFor(TrainCar trainCar)
	{
		if (this.disposed)
		{
			return 0f;
		}
		if (this.trainCars.IndexOf(trainCar) < 0)
		{
			Debug.LogError(base.GetType().Name + ": Train car not found in the trainCars list.");
			return 0f;
		}
		if (this.IsCoupledBackwards(trainCar))
		{
			return -this.trackSpeed;
		}
		return this.trackSpeed;
	}

	// Token: 0x06002598 RID: 9624 RVA: 0x000EA5B8 File Offset: 0x000E87B8
	public float GetPrevTrackSpeedFor(TrainCar trainCar)
	{
		if (this.trainCars.IndexOf(trainCar) < 0)
		{
			Debug.LogError(base.GetType().Name + ": Train car not found in the trainCars list.");
			return 0f;
		}
		if (this.IsCoupledBackwards(trainCar))
		{
			return -this.prevTrackSpeed;
		}
		return this.prevTrackSpeed;
	}

	// Token: 0x06002599 RID: 9625 RVA: 0x000EA60C File Offset: 0x000E880C
	public void UpdateTick(float dt)
	{
		if (this.ranUpdateTick || this.disposed)
		{
			return;
		}
		this.ranUpdateTick = true;
		if (this.IsAllAsleep() && !this.HasAnyEnginesOn() && !this.HasAnyCollisions() && !this.isShunting)
		{
			this.trackSpeed = 0f;
			return;
		}
		this.ParamsTick();
		this.MovementTick(dt);
		this.LinedUpToUnload = this.CheckLinedUpToUnload(out this.unloaderPos);
		if (this.disposed)
		{
			return;
		}
		if (Mathf.Abs(this.trackSpeed) > 0.1f)
		{
			this.lastMovingTime = UnityEngine.Time.time;
		}
		if (!this.HasAnyEnginesOn() && !this.HasAnyCollisions() && UnityEngine.Time.time > this.lastMovingTime + 10f)
		{
			this.trackSpeed = 0f;
			this.SleepAll();
		}
	}

	// Token: 0x0600259A RID: 9626 RVA: 0x000EA6DC File Offset: 0x000E88DC
	public bool IncludesAnEngine()
	{
		if (this.disposed)
		{
			return false;
		}
		using (List<TrainCar>.Enumerator enumerator = this.trainCars.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.CarType == TrainCar.TrainCarType.Engine)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x000EA740 File Offset: 0x000E8940
	protected bool HasAnyCollisions()
	{
		return this.frontCollisionTrigger.HasAnyContents || this.rearCollisionTrigger.HasAnyContents;
	}

	// Token: 0x0600259C RID: 9628 RVA: 0x000EA75C File Offset: 0x000E895C
	private bool HasAnyEnginesOn()
	{
		if (this.disposed)
		{
			return false;
		}
		foreach (TrainCar trainCar in this.trainCars)
		{
			if (trainCar.CarType == TrainCar.TrainCarType.Engine && trainCar.IsOn())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600259D RID: 9629 RVA: 0x000EA7CC File Offset: 0x000E89CC
	private bool IsAllAsleep()
	{
		if (this.disposed)
		{
			return true;
		}
		using (List<TrainCar>.Enumerator enumerator = this.trainCars.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.rigidBody.IsSleeping())
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600259E RID: 9630 RVA: 0x000EA834 File Offset: 0x000E8A34
	private void SleepAll()
	{
		if (this.disposed)
		{
			return;
		}
		foreach (TrainCar trainCar in this.trainCars)
		{
			trainCar.rigidBody.Sleep();
		}
	}

	// Token: 0x0600259F RID: 9631 RVA: 0x000EA894 File Offset: 0x000E8A94
	public bool TryShuntCarTo(Vector3 shuntDirection, float shuntDistance, TrainCar shuntTarget, Action<CoalingTower.ActionAttemptStatus> shuntEndCallback, out CoalingTower.ActionAttemptStatus status)
	{
		if (this.disposed)
		{
			status = CoalingTower.ActionAttemptStatus.NoTrainCar;
			return false;
		}
		if (this.isShunting)
		{
			status = CoalingTower.ActionAttemptStatus.AlreadyShunting;
			return false;
		}
		if (Mathf.Abs(this.trackSpeed) > 0.1f)
		{
			status = CoalingTower.ActionAttemptStatus.TrainIsMoving;
			return false;
		}
		if (this.HasThrottleInput())
		{
			status = CoalingTower.ActionAttemptStatus.TrainHasThrottle;
			return false;
		}
		this.shuntDirection = shuntDirection;
		this.shuntDistance = shuntDistance;
		this.shuntTarget = shuntTarget;
		this.timeSinceShuntStart = 0f;
		this.shuntStartPos2D.x = shuntTarget.transform.position.x;
		this.shuntStartPos2D.y = shuntTarget.transform.position.z;
		this.isShunting = true;
		this.shuntEndCallback = shuntEndCallback;
		status = CoalingTower.ActionAttemptStatus.NoError;
		return true;
	}

	// Token: 0x060025A0 RID: 9632 RVA: 0x000EA952 File Offset: 0x000E8B52
	private void EndShunting(CoalingTower.ActionAttemptStatus status)
	{
		this.isShunting = false;
		if (this.shuntEndCallback != null)
		{
			this.shuntEndCallback(status);
			this.shuntEndCallback = null;
		}
		this.shuntTarget = null;
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x000EA97D File Offset: 0x000E8B7D
	public bool ContainsOnly(TrainCar trainCar)
	{
		return !this.disposed && this.trainCars.Count == 1 && this.trainCars[0] == trainCar;
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x000EA9AB File Offset: 0x000E8BAB
	public int IndexOf(TrainCar trainCar)
	{
		if (this.disposed)
		{
			return -1;
		}
		return this.trainCars.IndexOf(trainCar);
	}

	// Token: 0x060025A3 RID: 9635 RVA: 0x000EA9C4 File Offset: 0x000E8BC4
	public bool TryGetAdjacentTrainCar(TrainCar trainCar, bool next, Vector3 forwardDir, out TrainCar result)
	{
		int num = this.trainCars.IndexOf(trainCar);
		Vector3 lhs;
		if (this.IsCoupledBackwards(num))
		{
			lhs = -trainCar.transform.forward;
		}
		else
		{
			lhs = trainCar.transform.forward;
		}
		if (Vector3.Dot(lhs, forwardDir) < 0f)
		{
			next = !next;
		}
		if (num >= 0)
		{
			if (next)
			{
				num++;
			}
			else
			{
				num--;
			}
			if (num >= 0 && num < this.trainCars.Count)
			{
				result = this.trainCars[num];
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x060025A4 RID: 9636 RVA: 0x000EAA54 File Offset: 0x000E8C54
	private void ParamsTick()
	{
		this.TotalForces = 0f;
		this.TotalMass = 0f;
		int num = 0;
		float num2 = 0f;
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			TrainCar trainCar = this.trainCars[i];
			if (trainCar.rigidBody.mass > num2)
			{
				num2 = trainCar.rigidBody.mass;
				num = i;
			}
		}
		bool flag = false;
		for (int j = 0; j < this.trainCars.Count; j++)
		{
			TrainCar trainCar2 = this.trainCars[j];
			float forces = trainCar2.GetForces();
			this.TotalForces += (this.IsCoupledBackwards(trainCar2) ? (-forces) : forces);
			flag |= trainCar2.HasThrottleInput();
			if (j == num)
			{
				this.TotalMass += trainCar2.rigidBody.mass;
			}
			else
			{
				this.TotalMass += trainCar2.rigidBody.mass * 0.4f;
			}
		}
		if (this.isShunting && flag)
		{
			this.EndShunting(CoalingTower.ActionAttemptStatus.TrainHasThrottle);
		}
		if (this.trainCars.Count == 1)
		{
			this.frontCollisionTrigger = this.trainCars[0].FrontCollisionTrigger;
			this.rearCollisionTrigger = this.trainCars[0].RearCollisionTrigger;
			return;
		}
		this.frontCollisionTrigger = (this.trainCars[0].coupling.IsRearCoupled ? this.trainCars[0].FrontCollisionTrigger : this.trainCars[0].RearCollisionTrigger);
		this.rearCollisionTrigger = (this.trainCars[this.trainCars.Count - 1].coupling.IsRearCoupled ? this.trainCars[this.trainCars.Count - 1].FrontCollisionTrigger : this.trainCars[this.trainCars.Count - 1].RearCollisionTrigger);
	}

	// Token: 0x060025A5 RID: 9637 RVA: 0x000EAC58 File Offset: 0x000E8E58
	private void MovementTick(float dt)
	{
		this.prevTrackSpeed = this.trackSpeed;
		if (!this.isShunting)
		{
			this.trackSpeed += this.TotalForces * dt / this.TotalMass;
		}
		else
		{
			bool flag = Vector3.Dot(this.shuntDirection, this.PrimaryTrainCar.transform.forward) >= 0f;
			if (this.IsCoupledBackwards(this.PrimaryTrainCar))
			{
				flag = !flag;
			}
			if (this.shuntTarget == null || this.shuntTarget.IsDead() || this.shuntTarget.IsDestroyed)
			{
				this.EndShunting(CoalingTower.ActionAttemptStatus.NoTrainCar);
			}
			else
			{
				float num = 4f;
				this.shuntTargetPos2D.x = this.shuntTarget.transform.position.x;
				this.shuntTargetPos2D.y = this.shuntTarget.transform.position.z;
				float num2 = this.shuntDistance - Vector3.Distance(this.shuntStartPos2D, this.shuntTargetPos2D);
				if (num2 < 2f)
				{
					float t = Mathf.InverseLerp(0f, 2f, num2);
					num *= Mathf.Lerp(0.1f, 1f, t);
				}
				this.trackSpeed = Mathf.MoveTowards(this.trackSpeed, flag ? num : (-num), dt * 10f);
				if (this.timeSinceShuntStart > 20f || num2 <= 0f)
				{
					this.EndShunting(CoalingTower.ActionAttemptStatus.NoError);
					this.trackSpeed = 0f;
				}
			}
		}
		float num3 = this.trainCars[0].rigidBody.drag;
		if (this.IsLinedUpToUnload)
		{
			float num4 = Mathf.Abs(this.trackSpeed);
			if (num4 > 1f)
			{
				TrainCarUnloadable trainCarUnloadable = this.trainCars[this.LinedUpToUnload] as TrainCarUnloadable;
				if (trainCarUnloadable != null)
				{
					float value = trainCarUnloadable.MinDistToUnloadingArea(this.unloaderPos);
					float num5 = Mathf.InverseLerp(2f, 0f, value);
					if (num4 < 2f)
					{
						float num6 = (num4 - 1f) / 1f;
						num5 *= num6;
					}
					num3 = Mathf.Lerp(num3, 3.5f, num5);
				}
			}
		}
		if (this.trackSpeed > 0f)
		{
			this.trackSpeed -= num3 * 4f * dt;
			if (this.trackSpeed < 0f)
			{
				this.trackSpeed = 0f;
			}
		}
		else if (this.trackSpeed < 0f)
		{
			this.trackSpeed += num3 * 4f * dt;
			if (this.trackSpeed > 0f)
			{
				this.trackSpeed = 0f;
			}
		}
		float num7 = this.trackSpeed;
		this.trackSpeed = this.ApplyCollisionsToTrackSpeed(this.trackSpeed, this.TotalMass, dt);
		if (this.isShunting && this.trackSpeed != num7)
		{
			this.EndShunting(CoalingTower.ActionAttemptStatus.GenericError);
		}
		if (this.disposed)
		{
			return;
		}
		this.trackSpeed = Mathf.Clamp(this.trackSpeed, -(TrainCar.TRAINCAR_MAX_SPEED - 1f), TrainCar.TRAINCAR_MAX_SPEED - 1f);
		if (this.trackSpeed > 0f)
		{
			this.PrimaryTrainCar = this.trainCars[0];
		}
		else if (this.trackSpeed < 0f)
		{
			this.PrimaryTrainCar = this.trainCars[this.trainCars.Count - 1];
		}
		else if (this.TotalForces > 0f)
		{
			this.PrimaryTrainCar = this.trainCars[0];
		}
		else if (this.TotalForces < 0f)
		{
			this.PrimaryTrainCar = this.trainCars[this.trainCars.Count - 1];
		}
		else
		{
			this.PrimaryTrainCar = this.trainCars[0];
		}
		if (this.trackSpeed != 0f || this.TotalForces != 0f)
		{
			this.PrimaryTrainCar.FrontTrainCarTick(this.GetTrackSelection(), dt);
			if (this.trainCars.Count > 1)
			{
				if (this.PrimaryTrainCar == this.trainCars[0])
				{
					for (int i = 1; i < this.trainCars.Count; i++)
					{
						this.MoveOtherTrainCar(this.trainCars[i], this.trainCars[i - 1]);
					}
					return;
				}
				for (int j = this.trainCars.Count - 2; j >= 0; j--)
				{
					this.MoveOtherTrainCar(this.trainCars[j], this.trainCars[j + 1]);
				}
			}
		}
	}

	// Token: 0x060025A6 RID: 9638 RVA: 0x000EB100 File Offset: 0x000E9300
	private void MoveOtherTrainCar(TrainCar trainCar, TrainCar prevTrainCar)
	{
		TrainTrackSpline frontTrackSection = prevTrainCar.FrontTrackSection;
		float frontWheelSplineDist = prevTrainCar.FrontWheelSplineDist;
		float num = 0f;
		TrainCoupling coupledTo = trainCar.coupling.frontCoupling.CoupledTo;
		TrainCoupling coupledTo2 = trainCar.coupling.rearCoupling.CoupledTo;
		if (coupledTo == prevTrainCar.coupling.frontCoupling)
		{
			num += trainCar.DistFrontWheelToFrontCoupling;
			num += prevTrainCar.DistFrontWheelToFrontCoupling;
		}
		else if (coupledTo2 == prevTrainCar.coupling.rearCoupling)
		{
			num -= trainCar.DistFrontWheelToBackCoupling;
			num -= prevTrainCar.DistFrontWheelToBackCoupling;
		}
		else if (coupledTo == prevTrainCar.coupling.rearCoupling)
		{
			num += trainCar.DistFrontWheelToFrontCoupling;
			num += prevTrainCar.DistFrontWheelToBackCoupling;
		}
		else if (coupledTo2 == prevTrainCar.coupling.frontCoupling)
		{
			num -= trainCar.DistFrontWheelToBackCoupling;
			num -= prevTrainCar.DistFrontWheelToFrontCoupling;
		}
		else
		{
			Debug.LogError(base.GetType().Name + ": Uncoupled!");
		}
		trainCar.OtherTrainCarTick(frontTrackSection, frontWheelSplineDist, -num);
	}

	// Token: 0x060025A7 RID: 9639 RVA: 0x000EB1F5 File Offset: 0x000E93F5
	public void ResetUpdateTick()
	{
		this.ranUpdateTick = false;
	}

	// Token: 0x060025A8 RID: 9640 RVA: 0x000EB200 File Offset: 0x000E9400
	public bool Matches(List<TrainCar> listToCompare)
	{
		if (this.disposed)
		{
			return false;
		}
		if (listToCompare.Count != this.trainCars.Count)
		{
			return false;
		}
		for (int i = 0; i < listToCompare.Count; i++)
		{
			if (this.trainCars[i] != listToCompare[i])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060025A9 RID: 9641 RVA: 0x000EB25C File Offset: 0x000E945C
	public void ReduceSpeedBy(float velChange)
	{
		this.prevTrackSpeed = this.trackSpeed;
		if (this.trackSpeed > 0f)
		{
			this.trackSpeed = Mathf.Max(0f, this.trackSpeed - velChange);
			return;
		}
		if (this.trackSpeed < 0f)
		{
			this.trackSpeed = Mathf.Min(0f, this.trackSpeed + velChange);
		}
	}

	// Token: 0x060025AA RID: 9642 RVA: 0x000EB2C0 File Offset: 0x000E94C0
	public bool AnyPlayersOnTrain()
	{
		if (this.disposed)
		{
			return false;
		}
		using (List<TrainCar>.Enumerator enumerator = this.trainCars.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.AnyPlayersOnTrainCar())
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060025AB RID: 9643 RVA: 0x000EB324 File Offset: 0x000E9524
	private int CheckLinedUpToUnload(out Vector3 unloaderPos)
	{
		if (this.disposed)
		{
			unloaderPos = Vector3.zero;
			return -1;
		}
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			TrainCar trainCar = this.trainCars[i];
			bool flag;
			if (CoalingTower.IsUnderAnUnloader(trainCar, out flag, out unloaderPos))
			{
				trainCar.SetFlag(BaseEntity.Flags.Reserved4, flag, false, true);
				if (flag)
				{
					return i;
				}
			}
		}
		unloaderPos = Vector3.zero;
		return -1;
	}

	// Token: 0x060025AC RID: 9644 RVA: 0x000EB393 File Offset: 0x000E9593
	public bool IsCoupledBackwards(TrainCar trainCar)
	{
		return !this.disposed && this.IsCoupledBackwards(this.trainCars.IndexOf(trainCar));
	}

	// Token: 0x060025AD RID: 9645 RVA: 0x000EB3B4 File Offset: 0x000E95B4
	private bool IsCoupledBackwards(int trainCarIndex)
	{
		if (this.disposed || this.trainCars.Count == 1 || trainCarIndex < 0 || trainCarIndex > this.trainCars.Count - 1)
		{
			return false;
		}
		TrainCar trainCar = this.trainCars[trainCarIndex];
		if (trainCarIndex == 0)
		{
			return trainCar.coupling.IsFrontCoupled;
		}
		TrainCoupling coupledTo = trainCar.coupling.frontCoupling.CoupledTo;
		return coupledTo == null || coupledTo.owner != this.trainCars[trainCarIndex - 1];
	}

	// Token: 0x060025AE RID: 9646 RVA: 0x000EB438 File Offset: 0x000E9638
	private bool HasThrottleInput()
	{
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			if (this.trainCars[i].HasThrottleInput())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060025AF RID: 9647 RVA: 0x000EB474 File Offset: 0x000E9674
	private TrainTrackSpline.TrackSelection GetTrackSelection()
	{
		TrainTrackSpline.TrackSelection result = TrainTrackSpline.TrackSelection.Default;
		foreach (TrainCar trainCar in this.trainCars)
		{
			if (trainCar.localTrackSelection != TrainTrackSpline.TrackSelection.Default)
			{
				if (this.IsCoupledBackwards(trainCar) != this.IsCoupledBackwards(this.PrimaryTrainCar))
				{
					if (trainCar.localTrackSelection == TrainTrackSpline.TrackSelection.Left)
					{
						return TrainTrackSpline.TrackSelection.Right;
					}
					if (trainCar.localTrackSelection == TrainTrackSpline.TrackSelection.Right)
					{
						return TrainTrackSpline.TrackSelection.Left;
					}
				}
				return trainCar.localTrackSelection;
			}
		}
		return result;
	}

	// Token: 0x060025B0 RID: 9648 RVA: 0x000EB508 File Offset: 0x000E9708
	public void FreeStaticCollision()
	{
		this.staticCollidingAtFront = CompleteTrain.StaticCollisionState.Free;
		this.staticCollidingAtRear = CompleteTrain.StaticCollisionState.Free;
	}

	// Token: 0x060025B1 RID: 9649 RVA: 0x000EB518 File Offset: 0x000E9718
	private float ApplyCollisionsToTrackSpeed(float trackSpeed, float totalMass, float deltaTime)
	{
		TrainCar owner = this.frontCollisionTrigger.owner;
		Vector3 forwardVector = this.IsCoupledBackwards(owner) ? (-owner.transform.forward) : owner.transform.forward;
		trackSpeed = this.ApplyCollisions(trackSpeed, owner, forwardVector, true, this.frontCollisionTrigger, totalMass, ref this.staticCollidingAtFront, this.staticCollidingAtRear, deltaTime);
		if (this.disposed)
		{
			return trackSpeed;
		}
		owner = this.rearCollisionTrigger.owner;
		forwardVector = (this.IsCoupledBackwards(owner) ? (-owner.transform.forward) : owner.transform.forward);
		trackSpeed = this.ApplyCollisions(trackSpeed, owner, forwardVector, false, this.rearCollisionTrigger, totalMass, ref this.staticCollidingAtRear, this.staticCollidingAtFront, deltaTime);
		if (this.disposed)
		{
			return trackSpeed;
		}
		Rigidbody rigidbody = null;
		foreach (KeyValuePair<Rigidbody, float> keyValuePair in this.prevTrackSpeeds)
		{
			if (keyValuePair.Key == null || (!this.frontCollisionTrigger.otherRigidbodyContents.Contains(keyValuePair.Key) && !this.rearCollisionTrigger.otherRigidbodyContents.Contains(keyValuePair.Key)))
			{
				rigidbody = keyValuePair.Key;
				break;
			}
		}
		if (rigidbody != null)
		{
			this.prevTrackSpeeds.Remove(rigidbody);
		}
		return trackSpeed;
	}

	// Token: 0x060025B2 RID: 9650 RVA: 0x000EB684 File Offset: 0x000E9884
	private float ApplyCollisions(float trackSpeed, TrainCar ourTrainCar, Vector3 forwardVector, bool atOurFront, TriggerTrainCollisions trigger, float ourTotalMass, ref CompleteTrain.StaticCollisionState wasStaticColliding, CompleteTrain.StaticCollisionState otherEndStaticColliding, float deltaTime)
	{
		Vector3 b = forwardVector * trackSpeed;
		bool flag = trigger.HasAnyStaticContents;
		if (atOurFront && ourTrainCar.FrontAtEndOfLine)
		{
			flag = true;
		}
		else if (!atOurFront && ourTrainCar.RearAtEndOfLine)
		{
			flag = true;
		}
		float num = flag ? (b.magnitude * Mathf.Clamp(ourTotalMass, 1f, 13000f)) : 0f;
		trackSpeed = this.HandleStaticCollisions(flag, atOurFront, trackSpeed, ref wasStaticColliding, trigger);
		if (!flag && otherEndStaticColliding == CompleteTrain.StaticCollisionState.Free)
		{
			foreach (TrainCar trainCar in trigger.trainContents)
			{
				Vector3 a = trainCar.transform.forward * trainCar.GetPrevTrackSpeed();
				trackSpeed = this.HandleTrainCollision(atOurFront, forwardVector, trackSpeed, ourTrainCar.transform, trainCar, deltaTime, ref wasStaticColliding);
				num += Vector3.Magnitude(a - b) * Mathf.Clamp(trainCar.rigidBody.mass, 1f, 13000f);
			}
			foreach (Rigidbody rigidbody in trigger.otherRigidbodyContents)
			{
				trackSpeed = this.HandleRigidbodyCollision(atOurFront, trackSpeed, forwardVector, ourTotalMass, rigidbody, rigidbody.mass, deltaTime, true);
				num += Vector3.Magnitude(rigidbody.velocity - b) * Mathf.Clamp(rigidbody.mass, 1f, 13000f);
			}
		}
		if (num >= 70000f && this.timeSinceLastChange > 1f && trigger.owner.ApplyCollisionDamage(num) > 5f)
		{
			foreach (Collider collider in trigger.colliderContents)
			{
				Vector3 contactPoint = collider.ClosestPointOnBounds(trigger.owner.transform.position);
				trigger.owner.TryShowCollisionFX(contactPoint, trigger.owner.collisionEffect);
			}
		}
		return trackSpeed;
	}

	// Token: 0x060025B3 RID: 9651 RVA: 0x000EB8C4 File Offset: 0x000E9AC4
	private float HandleStaticCollisions(bool staticColliding, bool front, float trackSpeed, ref CompleteTrain.StaticCollisionState wasStaticColliding, TriggerTrainCollisions trigger = null)
	{
		float num = front ? -5f : 5f;
		if (staticColliding && (front ? (trackSpeed > num) : (trackSpeed < num)))
		{
			trackSpeed = num;
			wasStaticColliding = CompleteTrain.StaticCollisionState.StaticColliding;
			HashSet<GameObject> hashSet = front ? this.monitoredStaticContentF : this.monitoredStaticContentR;
			hashSet.Clear();
			if (!(trigger != null))
			{
				return trackSpeed;
			}
			using (HashSet<GameObject>.Enumerator enumerator = trigger.staticContents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameObject item = enumerator.Current;
					hashSet.Add(item);
				}
				return trackSpeed;
			}
		}
		if (wasStaticColliding == CompleteTrain.StaticCollisionState.StaticColliding)
		{
			trackSpeed = 0f;
			wasStaticColliding = CompleteTrain.StaticCollisionState.StayingStill;
		}
		else if (wasStaticColliding == CompleteTrain.StaticCollisionState.StayingStill)
		{
			bool flag = front ? (trackSpeed > 0.01f) : (trackSpeed < -0.01f);
			bool flag2 = false;
			if (!flag)
			{
				flag2 = (front ? (trackSpeed < -0.01f) : (trackSpeed > 0.01f));
			}
			if (flag)
			{
				HashSet<GameObject> hashSet2 = front ? this.monitoredStaticContentF : this.monitoredStaticContentR;
				if (hashSet2.Count > 0)
				{
					bool flag3 = true;
					using (HashSet<GameObject>.Enumerator enumerator = hashSet2.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current != null)
							{
								flag3 = false;
								break;
							}
						}
					}
					if (flag3)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				trackSpeed = 0f;
			}
			else if (flag2)
			{
				wasStaticColliding = CompleteTrain.StaticCollisionState.Free;
			}
		}
		else if (front)
		{
			this.monitoredStaticContentF.Clear();
		}
		else
		{
			this.monitoredStaticContentR.Clear();
		}
		return trackSpeed;
	}

	// Token: 0x060025B4 RID: 9652 RVA: 0x000EBA64 File Offset: 0x000E9C64
	private float HandleTrainCollision(bool front, Vector3 forwardVector, float trackSpeed, Transform ourTransform, TrainCar theirTrain, float deltaTime, ref CompleteTrain.StaticCollisionState wasStaticColliding)
	{
		Vector3 vector = front ? forwardVector : (-forwardVector);
		float num = Vector3.Angle(vector, theirTrain.transform.forward);
		float f = Vector3.Dot(vector, (theirTrain.transform.position - ourTransform.position).normalized);
		if ((num > 30f && num < 150f) || Mathf.Abs(f) < 0.975f)
		{
			trackSpeed = (front ? -0.5f : 0.5f);
		}
		else
		{
			List<CompleteTrain> list = Facepunch.Pool.GetList<CompleteTrain>();
			float totalPushingMass = this.GetTotalPushingMass(vector, forwardVector, ref list);
			if (totalPushingMass < 0f)
			{
				trackSpeed = this.HandleStaticCollisions(true, front, trackSpeed, ref wasStaticColliding, null);
			}
			else
			{
				trackSpeed = this.HandleRigidbodyCollision(front, trackSpeed, forwardVector, this.TotalMass, theirTrain.rigidBody, totalPushingMass, deltaTime, false);
			}
			list.Clear();
			float num2 = this.GetTotalPushingForces(vector, forwardVector, ref list);
			if (!front)
			{
				num2 *= -1f;
			}
			if ((front && num2 <= 0f) || (!front && num2 >= 0f))
			{
				trackSpeed += num2 / this.TotalMass * deltaTime;
			}
			Facepunch.Pool.FreeList<CompleteTrain>(ref list);
		}
		return trackSpeed;
	}

	// Token: 0x060025B5 RID: 9653 RVA: 0x000EBB8C File Offset: 0x000E9D8C
	private float HandleRigidbodyCollision(bool atOurFront, float trackSpeed, Vector3 forwardVector, float ourTotalMass, Rigidbody theirRB, float theirTotalMass, float deltaTime, bool calcSecondaryForces)
	{
		float num = Vector3.Dot(forwardVector, theirRB.velocity);
		float num2 = trackSpeed - num;
		if ((atOurFront && num2 <= 0f) || (!atOurFront && num2 >= 0f))
		{
			return trackSpeed;
		}
		float num3 = num2 / deltaTime * theirTotalMass * 0.75f;
		if (calcSecondaryForces)
		{
			if (this.prevTrackSpeeds.ContainsKey(theirRB))
			{
				float num4 = num2 / deltaTime * ourTotalMass * 0.75f / theirTotalMass * deltaTime;
				float num5 = this.prevTrackSpeeds[theirRB] - num;
				num3 -= Mathf.Clamp((num5 - num4) * ourTotalMass, 0f, 1000000f);
				this.prevTrackSpeeds[theirRB] = num;
			}
			else if (num != 0f)
			{
				this.prevTrackSpeeds.Add(theirRB, num);
			}
		}
		float num6 = num3 / ourTotalMass * deltaTime;
		num6 = Mathf.Clamp(num6, -Mathf.Abs(num - trackSpeed) - 0.5f, Mathf.Abs(num - trackSpeed) + 0.5f);
		trackSpeed -= num6;
		return trackSpeed;
	}

	// Token: 0x060025B6 RID: 9654 RVA: 0x000EBC7C File Offset: 0x000E9E7C
	private float GetTotalPushingMass(Vector3 pushDirection, Vector3 ourForward, ref List<CompleteTrain> prevTrains)
	{
		float num = 0f;
		if (prevTrains.Count > 0)
		{
			if (prevTrains.Contains(this))
			{
				if (Global.developer > 1 || UnityEngine.Application.isEditor)
				{
					Debug.LogWarning("GetTotalPushingMass: Recursive loop detected. Bailing out.");
				}
				return 0f;
			}
			num += this.TotalMass;
		}
		prevTrains.Add(this);
		bool flag = Vector3.Dot(ourForward, pushDirection) >= 0f;
		if ((flag ? this.staticCollidingAtFront : this.staticCollidingAtRear) != CompleteTrain.StaticCollisionState.Free)
		{
			return -1f;
		}
		TriggerTrainCollisions triggerTrainCollisions = flag ? this.frontCollisionTrigger : this.rearCollisionTrigger;
		foreach (TrainCar trainCar in triggerTrainCollisions.trainContents)
		{
			if (trainCar.completeTrain != this)
			{
				Vector3 ourForward2 = trainCar.completeTrain.IsCoupledBackwards(trainCar) ? (-trainCar.transform.forward) : trainCar.transform.forward;
				float totalPushingMass = trainCar.completeTrain.GetTotalPushingMass(pushDirection, ourForward2, ref prevTrains);
				if (totalPushingMass < 0f)
				{
					return -1f;
				}
				num += totalPushingMass;
			}
		}
		foreach (Rigidbody rigidbody in triggerTrainCollisions.otherRigidbodyContents)
		{
			num += rigidbody.mass;
		}
		return num;
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x000EBE00 File Offset: 0x000EA000
	private float GetTotalPushingForces(Vector3 pushDirection, Vector3 ourForward, ref List<CompleteTrain> prevTrains)
	{
		float num = 0f;
		if (prevTrains.Count > 0)
		{
			if (prevTrains.Contains(this))
			{
				if (Global.developer > 1 || UnityEngine.Application.isEditor)
				{
					Debug.LogWarning("GetTotalPushingForces: Recursive loop detected. Bailing out.");
				}
				return 0f;
			}
			num += this.TotalForces;
		}
		prevTrains.Add(this);
		bool flag = Vector3.Dot(ourForward, pushDirection) >= 0f;
		TriggerTrainCollisions triggerTrainCollisions = flag ? this.frontCollisionTrigger : this.rearCollisionTrigger;
		if (!flag)
		{
			num *= -1f;
		}
		foreach (TrainCar trainCar in triggerTrainCollisions.trainContents)
		{
			if (trainCar.completeTrain != this)
			{
				Vector3 ourForward2 = trainCar.completeTrain.IsCoupledBackwards(trainCar) ? (-trainCar.transform.forward) : trainCar.transform.forward;
				num += trainCar.completeTrain.GetTotalPushingForces(pushDirection, ourForward2, ref prevTrains);
			}
		}
		return num;
	}

	// Token: 0x04001E54 RID: 7764
	private Vector3 unloaderPos;

	// Token: 0x04001E55 RID: 7765
	private float trackSpeed;

	// Token: 0x04001E56 RID: 7766
	private float prevTrackSpeed;

	// Token: 0x04001E57 RID: 7767
	private List<TrainCar> trainCars;

	// Token: 0x04001E58 RID: 7768
	private TriggerTrainCollisions frontCollisionTrigger;

	// Token: 0x04001E59 RID: 7769
	private TriggerTrainCollisions rearCollisionTrigger;

	// Token: 0x04001E5A RID: 7770
	private bool ranUpdateTick;

	// Token: 0x04001E5B RID: 7771
	private bool disposed;

	// Token: 0x04001E5C RID: 7772
	private const float IMPACT_ENERGY_FRACTION = 0.75f;

	// Token: 0x04001E5D RID: 7773
	private const float MIN_COLLISION_FORCE = 70000f;

	// Token: 0x04001E5E RID: 7774
	private float lastMovingTime = float.MinValue;

	// Token: 0x04001E5F RID: 7775
	private const float SLEEP_SPEED = 0.1f;

	// Token: 0x04001E60 RID: 7776
	private const float SLEEP_DELAY = 10f;

	// Token: 0x04001E61 RID: 7777
	private TimeSince timeSinceLastChange;

	// Token: 0x04001E62 RID: 7778
	private bool isShunting;

	// Token: 0x04001E63 RID: 7779
	private TimeSince timeSinceShuntStart;

	// Token: 0x04001E64 RID: 7780
	private const float MAX_SHUNT_TIME = 20f;

	// Token: 0x04001E65 RID: 7781
	private const float SHUNT_SPEED = 4f;

	// Token: 0x04001E66 RID: 7782
	private const float SHUNT_SPEED_CHANGE_RATE = 10f;

	// Token: 0x04001E67 RID: 7783
	private Action<CoalingTower.ActionAttemptStatus> shuntEndCallback;

	// Token: 0x04001E68 RID: 7784
	private float shuntDistance;

	// Token: 0x04001E69 RID: 7785
	private Vector3 shuntDirection;

	// Token: 0x04001E6A RID: 7786
	private Vector2 shuntStartPos2D = Vector2.zero;

	// Token: 0x04001E6B RID: 7787
	private Vector2 shuntTargetPos2D = Vector2.zero;

	// Token: 0x04001E6C RID: 7788
	private TrainCar shuntTarget;

	// Token: 0x04001E6D RID: 7789
	private CompleteTrain.StaticCollisionState staticCollidingAtFront;

	// Token: 0x04001E6E RID: 7790
	private HashSet<GameObject> monitoredStaticContentF = new HashSet<GameObject>();

	// Token: 0x04001E6F RID: 7791
	private CompleteTrain.StaticCollisionState staticCollidingAtRear;

	// Token: 0x04001E70 RID: 7792
	private HashSet<GameObject> monitoredStaticContentR = new HashSet<GameObject>();

	// Token: 0x04001E71 RID: 7793
	private Dictionary<Rigidbody, float> prevTrackSpeeds = new Dictionary<Rigidbody, float>();

	// Token: 0x02000CB6 RID: 3254
	private enum ShuntState
	{
		// Token: 0x0400438C RID: 17292
		None,
		// Token: 0x0400438D RID: 17293
		Forwards,
		// Token: 0x0400438E RID: 17294
		Backwards
	}

	// Token: 0x02000CB7 RID: 3255
	private enum StaticCollisionState
	{
		// Token: 0x04004390 RID: 17296
		Free,
		// Token: 0x04004391 RID: 17297
		StaticColliding,
		// Token: 0x04004392 RID: 17298
		StayingStill
	}
}
