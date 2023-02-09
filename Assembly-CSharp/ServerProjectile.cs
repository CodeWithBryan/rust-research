using System;
using UnityEngine;

// Token: 0x0200043B RID: 1083
public class ServerProjectile : EntityComponent<BaseEntity>, IServerComponent
{
	// Token: 0x170002BA RID: 698
	// (get) Token: 0x0600238D RID: 9101 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool HasRangeLimit
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600238E RID: 9102 RVA: 0x000E1224 File Offset: 0x000DF424
	public float GetMaxRange(float maxFuseTime)
	{
		if (this.gravityModifier == 0f)
		{
			return float.PositiveInfinity;
		}
		float a = Mathf.Sin(1.5707964f) * this.speed * this.speed / -(Physics.gravity.y * this.gravityModifier);
		float b = this.speed * maxFuseTime;
		return Mathf.Min(a, b);
	}

	// Token: 0x170002BB RID: 699
	// (get) Token: 0x0600238F RID: 9103 RVA: 0x000E127E File Offset: 0x000DF47E
	protected virtual int mask
	{
		get
		{
			return 1236478737;
		}
	}

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x06002390 RID: 9104 RVA: 0x000E1285 File Offset: 0x000DF485
	// (set) Token: 0x06002391 RID: 9105 RVA: 0x000E128D File Offset: 0x000DF48D
	public Vector3 CurrentVelocity { get; protected set; }

	// Token: 0x06002392 RID: 9106 RVA: 0x000E1296 File Offset: 0x000DF496
	protected void FixedUpdate()
	{
		if (base.baseEntity != null && base.baseEntity.isServer)
		{
			this.DoMovement();
		}
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x000E12BC File Offset: 0x000DF4BC
	public virtual bool DoMovement()
	{
		if (this.impacted)
		{
			return false;
		}
		this.CurrentVelocity += Physics.gravity * this.gravityModifier * Time.fixedDeltaTime * Time.timeScale;
		Vector3 a = this.CurrentVelocity;
		if (this.swimScale != Vector3.zero)
		{
			if (this.swimRandom == 0f)
			{
				this.swimRandom = UnityEngine.Random.Range(0f, 20f);
			}
			float num = Time.time + this.swimRandom;
			Vector3 vector = new Vector3(Mathf.Sin(num * this.swimSpeed.x) * this.swimScale.x, Mathf.Cos(num * this.swimSpeed.y) * this.swimScale.y, Mathf.Sin(num * this.swimSpeed.z) * this.swimScale.z);
			vector = base.transform.InverseTransformDirection(vector);
			a += vector;
		}
		float num2 = a.magnitude * Time.fixedDeltaTime;
		Vector3 position = base.transform.position;
		RaycastHit raycastHit;
		if (GamePhysics.Trace(new Ray(position, a.normalized), this.radius, out raycastHit, num2 + this.scanRange, this.mask, QueryTriggerInteraction.Ignore, null))
		{
			BaseEntity entity = raycastHit.GetEntity();
			if (this.IsAValidHit(entity))
			{
				base.transform.position += base.transform.forward * Mathf.Max(0f, raycastHit.distance - 0.1f);
				ServerProjectile.IProjectileImpact component = base.GetComponent<ServerProjectile.IProjectileImpact>();
				if (component != null)
				{
					component.ProjectileImpact(raycastHit, position);
				}
				this.impacted = true;
				return false;
			}
		}
		base.transform.position += base.transform.forward * num2;
		base.transform.rotation = Quaternion.LookRotation(a.normalized);
		return true;
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x000E14C4 File Offset: 0x000DF6C4
	protected virtual bool IsAValidHit(BaseEntity hitEnt)
	{
		return !hitEnt.IsValid() || !base.baseEntity.creatorEntity.IsValid() || hitEnt.net.ID != base.baseEntity.creatorEntity.net.ID;
	}

	// Token: 0x06002395 RID: 9109 RVA: 0x000E1512 File Offset: 0x000DF712
	public virtual void InitializeVelocity(Vector3 overrideVel)
	{
		base.transform.rotation = Quaternion.LookRotation(overrideVel.normalized);
		this.initialVelocity = overrideVel;
		this.CurrentVelocity = overrideVel;
	}

	// Token: 0x04001C3E RID: 7230
	public Vector3 initialVelocity;

	// Token: 0x04001C3F RID: 7231
	public float drag;

	// Token: 0x04001C40 RID: 7232
	public float gravityModifier = 1f;

	// Token: 0x04001C41 RID: 7233
	public float speed = 15f;

	// Token: 0x04001C42 RID: 7234
	public float scanRange;

	// Token: 0x04001C43 RID: 7235
	public Vector3 swimScale;

	// Token: 0x04001C44 RID: 7236
	public Vector3 swimSpeed;

	// Token: 0x04001C45 RID: 7237
	public float radius;

	// Token: 0x04001C46 RID: 7238
	private bool impacted;

	// Token: 0x04001C47 RID: 7239
	private float swimRandom;

	// Token: 0x02000C99 RID: 3225
	public interface IProjectileImpact
	{
		// Token: 0x06004D12 RID: 19730
		void ProjectileImpact(RaycastHit hitInfo, Vector3 rayOrigin);
	}
}
