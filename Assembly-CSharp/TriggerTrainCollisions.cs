using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000493 RID: 1171
public class TriggerTrainCollisions : TriggerBase
{
	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06002611 RID: 9745 RVA: 0x000EDADD File Offset: 0x000EBCDD
	public bool HasAnyStaticContents
	{
		get
		{
			return this.staticContents.Count > 0;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06002612 RID: 9746 RVA: 0x000EDAED File Offset: 0x000EBCED
	public bool HasAnyTrainContents
	{
		get
		{
			return this.trainContents.Count > 0;
		}
	}

	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06002613 RID: 9747 RVA: 0x000EDAFD File Offset: 0x000EBCFD
	public bool HasAnyOtherRigidbodyContents
	{
		get
		{
			return this.otherRigidbodyContents.Count > 0;
		}
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06002614 RID: 9748 RVA: 0x000EDB0D File Offset: 0x000EBD0D
	public bool HasAnyNonStaticContents
	{
		get
		{
			return this.HasAnyTrainContents || this.HasAnyOtherRigidbodyContents;
		}
	}

	// Token: 0x06002615 RID: 9749 RVA: 0x000EDB20 File Offset: 0x000EBD20
	internal override void OnObjectAdded(GameObject obj, Collider col)
	{
		if (!this.owner.isServer)
		{
			return;
		}
		base.OnObjectAdded(obj, col);
		if (obj != null)
		{
			BaseEntity baseEntity = obj.ToBaseEntity();
			if (baseEntity != null)
			{
				Vector3 a = baseEntity.transform.position + baseEntity.transform.rotation * Vector3.Scale(obj.transform.lossyScale, baseEntity.bounds.center);
				Vector3 center = this.triggerCollider.bounds.center;
				Vector3 rhs = a - center;
				bool flag = Vector3.Dot(this.owner.transform.forward, rhs) > 0f;
				if (this.location == TriggerTrainCollisions.Location.Front && !flag)
				{
					return;
				}
				if (this.location == TriggerTrainCollisions.Location.Rear && flag)
				{
					return;
				}
			}
		}
		if (obj != null)
		{
			Rigidbody componentInParent = obj.GetComponentInParent<Rigidbody>();
			if (componentInParent != null)
			{
				TrainCar componentInParent2 = obj.GetComponentInParent<TrainCar>();
				if (componentInParent2 != null)
				{
					this.trainContents.Add(componentInParent2);
					if (this.owner.coupling != null)
					{
						this.owner.coupling.TryCouple(componentInParent2, this.location);
					}
					base.InvokeRepeating(new Action(this.TrainContentsTick), 0.2f, 0.2f);
				}
				else
				{
					this.otherRigidbodyContents.Add(componentInParent);
				}
			}
			else
			{
				ITrainCollidable componentInParent3 = obj.GetComponentInParent<ITrainCollidable>();
				if (componentInParent3 == null)
				{
					if (!obj.CompareTag("Railway"))
					{
						this.staticContents.Add(obj);
					}
				}
				else if (!componentInParent3.EqualNetID(this.owner) && !componentInParent3.CustomCollision(this.owner, this))
				{
					this.staticContents.Add(obj);
				}
			}
		}
		if (col != null)
		{
			this.colliderContents.Add(col);
		}
	}

	// Token: 0x06002616 RID: 9750 RVA: 0x000EDCF0 File Offset: 0x000EBEF0
	internal override void OnObjectRemoved(GameObject obj)
	{
		if (!this.owner.isServer)
		{
			return;
		}
		if (obj == null)
		{
			return;
		}
		foreach (Collider item in obj.GetComponents<Collider>())
		{
			this.colliderContents.Remove(item);
		}
		if (!this.staticContents.Remove(obj))
		{
			TrainCar componentInParent = obj.GetComponentInParent<TrainCar>();
			if (componentInParent != null)
			{
				if (!this.<OnObjectRemoved>g__HasAnotherColliderFor|18_0<TrainCar>(componentInParent))
				{
					this.trainContents.Remove(componentInParent);
					if (this.trainContents == null || this.trainContents.Count == 0)
					{
						base.CancelInvoke(new Action(this.TrainContentsTick));
					}
				}
			}
			else
			{
				Rigidbody componentInParent2 = obj.GetComponentInParent<Rigidbody>();
				if (!this.<OnObjectRemoved>g__HasAnotherColliderFor|18_0<Rigidbody>(componentInParent2))
				{
					this.otherRigidbodyContents.Remove(componentInParent2);
				}
			}
		}
		base.OnObjectRemoved(obj);
	}

	// Token: 0x06002617 RID: 9751 RVA: 0x000EDDC0 File Offset: 0x000EBFC0
	private void TrainContentsTick()
	{
		if (this.trainContents == null)
		{
			return;
		}
		foreach (TrainCar trainCar in this.trainContents)
		{
			if (trainCar.IsValid() && !trainCar.IsDestroyed && this.owner.coupling != null)
			{
				this.owner.coupling.TryCouple(trainCar, this.location);
			}
		}
	}

	// Token: 0x06002619 RID: 9753 RVA: 0x000EDE80 File Offset: 0x000EC080
	[CompilerGenerated]
	private bool <OnObjectRemoved>g__HasAnotherColliderFor|18_0<T>(T component) where T : Component
	{
		foreach (Collider collider in this.colliderContents)
		{
			if (collider != null && collider.GetComponentInParent<T>() == component)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04001EEC RID: 7916
	public Collider triggerCollider;

	// Token: 0x04001EED RID: 7917
	public TriggerTrainCollisions.Location location;

	// Token: 0x04001EEE RID: 7918
	public TrainCar owner;

	// Token: 0x04001EEF RID: 7919
	[NonSerialized]
	public HashSet<GameObject> staticContents = new HashSet<GameObject>();

	// Token: 0x04001EF0 RID: 7920
	[NonSerialized]
	public HashSet<TrainCar> trainContents = new HashSet<TrainCar>();

	// Token: 0x04001EF1 RID: 7921
	[NonSerialized]
	public HashSet<Rigidbody> otherRigidbodyContents = new HashSet<Rigidbody>();

	// Token: 0x04001EF2 RID: 7922
	[NonSerialized]
	public HashSet<Collider> colliderContents = new HashSet<Collider>();

	// Token: 0x04001EF3 RID: 7923
	private const float TICK_RATE = 0.2f;

	// Token: 0x02000CC2 RID: 3266
	public enum Location
	{
		// Token: 0x040043B6 RID: 17334
		Front,
		// Token: 0x040043B7 RID: 17335
		Rear
	}
}
