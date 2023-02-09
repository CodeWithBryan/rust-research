using System;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200048A RID: 1162
public class TrainBarricade : BaseCombatEntity, ITrainCollidable, TrainTrackSpline.ITrainTrackUser
{
	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x060025C0 RID: 9664 RVA: 0x000299AB File Offset: 0x00027BAB
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x060025C1 RID: 9665 RVA: 0x000EC053 File Offset: 0x000EA253
	// (set) Token: 0x060025C2 RID: 9666 RVA: 0x000EC05B File Offset: 0x000EA25B
	public float FrontWheelSplineDist { get; private set; }

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x060025C3 RID: 9667 RVA: 0x0004AF67 File Offset: 0x00049167
	public TrainCar.TrainCarType CarType
	{
		get
		{
			return TrainCar.TrainCarType.Other;
		}
	}

	// Token: 0x060025C4 RID: 9668 RVA: 0x000EC064 File Offset: 0x000EA264
	public bool CustomCollision(TrainCar train, TriggerTrainCollisions trainTrigger)
	{
		bool result = false;
		if (base.isServer)
		{
			float num = Mathf.Abs(train.GetTrackSpeed());
			this.SetHitTrain(train, trainTrigger);
			if (num < this.minVelToDestroy && !vehicle.cinematictrains)
			{
				base.InvokeRandomized(new Action(this.PushForceTick), 0f, 0.25f, 0.025f);
			}
			else
			{
				result = true;
				base.Invoke(new Action(this.DestroyThisBarrier), 0f);
			}
		}
		return result;
	}

	// Token: 0x060025C5 RID: 9669 RVA: 0x000EC0DC File Offset: 0x000EA2DC
	public override void ServerInit()
	{
		base.ServerInit();
		TrainTrackSpline trainTrackSpline;
		float frontWheelSplineDist;
		if (TrainTrackSpline.TryFindTrackNear(base.transform.position, 3f, out trainTrackSpline, out frontWheelSplineDist))
		{
			this.track = trainTrackSpline;
			this.FrontWheelSplineDist = frontWheelSplineDist;
			this.track.RegisterTrackUser(this);
		}
	}

	// Token: 0x060025C6 RID: 9670 RVA: 0x000EC124 File Offset: 0x000EA324
	internal override void DoServerDestroy()
	{
		if (this.track != null)
		{
			this.track.DeregisterTrackUser(this);
		}
		base.DoServerDestroy();
	}

	// Token: 0x060025C7 RID: 9671 RVA: 0x000EC146 File Offset: 0x000EA346
	private void SetHitTrain(TrainCar train, TriggerTrainCollisions trainTrigger)
	{
		this.hitTrain = train;
		this.hitTrainTrigger = trainTrigger;
	}

	// Token: 0x060025C8 RID: 9672 RVA: 0x000EC156 File Offset: 0x000EA356
	private void ClearHitTrain()
	{
		this.SetHitTrain(null, null);
	}

	// Token: 0x060025C9 RID: 9673 RVA: 0x000EC160 File Offset: 0x000EA360
	private void DestroyThisBarrier()
	{
		if (this.IsDead() || base.IsDestroyed)
		{
			return;
		}
		if (this.hitTrain != null)
		{
			this.hitTrain.completeTrain.ReduceSpeedBy(this.velReduction);
			if (vehicle.cinematictrains)
			{
				this.hitTrain.Hurt(9999f, DamageType.Collision, this, false);
			}
			else
			{
				float amount = Mathf.Abs(this.hitTrain.GetTrackSpeed()) * this.trainDamagePerMPS;
				this.hitTrain.Hurt(amount, DamageType.Collision, this, false);
			}
		}
		this.ClearHitTrain();
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060025CA RID: 9674 RVA: 0x000EC1F4 File Offset: 0x000EA3F4
	private void PushForceTick()
	{
		if (this.hitTrain == null || this.hitTrainTrigger == null || this.hitTrain.IsDead() || this.hitTrain.IsDestroyed || this.IsDead())
		{
			this.ClearHitTrain();
			base.CancelInvoke(new Action(this.PushForceTick));
			return;
		}
		bool flag = true;
		if (!this.hitTrainTrigger.triggerCollider.bounds.Intersects(this.bounds))
		{
			Vector3 vector;
			if (this.hitTrainTrigger.location == TriggerTrainCollisions.Location.Front)
			{
				vector = this.hitTrainTrigger.owner.GetFrontOfTrainPos();
			}
			else
			{
				vector = this.hitTrainTrigger.owner.GetRearOfTrainPos();
			}
			Vector3 vector2 = base.transform.position + this.bounds.ClosestPoint(vector - base.transform.position);
			Debug.DrawRay(vector2, Vector3.up, Color.red, 10f);
			flag = (Vector3.SqrMagnitude(vector2 - vector) < 1f);
		}
		if (flag)
		{
			float num = this.hitTrainTrigger.owner.completeTrain.TotalForces;
			if (this.hitTrainTrigger.location == TriggerTrainCollisions.Location.Rear)
			{
				num *= -1f;
			}
			num = Mathf.Max(0f, num);
			base.Hurt(0.002f * num);
			if (this.IsDead())
			{
				this.hitTrain.completeTrain.FreeStaticCollision();
				return;
			}
		}
		else
		{
			this.ClearHitTrain();
			base.CancelInvoke(new Action(this.PushForceTick));
		}
	}

	// Token: 0x04001E82 RID: 7810
	[FormerlySerializedAs("damagePerMPS")]
	[SerializeField]
	private float trainDamagePerMPS = 10f;

	// Token: 0x04001E83 RID: 7811
	[SerializeField]
	private float minVelToDestroy = 6f;

	// Token: 0x04001E84 RID: 7812
	[SerializeField]
	private float velReduction = 2f;

	// Token: 0x04001E85 RID: 7813
	[SerializeField]
	private GameObjectRef barricadeDamageEffect;

	// Token: 0x04001E87 RID: 7815
	private TrainCar hitTrain;

	// Token: 0x04001E88 RID: 7816
	private TriggerTrainCollisions hitTrainTrigger;

	// Token: 0x04001E89 RID: 7817
	private TrainTrackSpline track;
}
