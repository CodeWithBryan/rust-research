using System;
using Rust;
using UnityEngine;

// Token: 0x02000139 RID: 313
public class SkyLantern : StorageContainer, IIgniteable
{
	// Token: 0x06001603 RID: 5635 RVA: 0x000058B6 File Offset: 0x00003AB6
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x06001604 RID: 5636 RVA: 0x000A8714 File Offset: 0x000A6914
	public override void ServerInit()
	{
		base.ServerInit();
		this.randOffset = ((UnityEngine.Random.Range(0.5f, 1f) * (float)UnityEngine.Random.Range(0, 2) == 1f) ? -1f : 1f);
		this.travelVec = (Vector3.forward + Vector3.right * this.randOffset).normalized;
		base.Invoke(new Action(this.StartSinking), this.lifeTime - 15f);
		base.Invoke(new Action(this.SelfDestroy), this.lifeTime);
		this.travelSpeed = UnityEngine.Random.Range(1.75f, 2.25f);
		this.gravityScale *= UnityEngine.Random.Range(1f, 1.25f);
		base.InvokeRepeating(new Action(this.UpdateIdealAltitude), 0f, 1f);
	}

	// Token: 0x06001605 RID: 5637 RVA: 0x000A8804 File Offset: 0x000A6A04
	public void Ignite(Vector3 fromPos)
	{
		base.gameObject.transform.RemoveComponent<GroundWatch>();
		base.gameObject.transform.RemoveComponent<DestroyOnGroundMissing>();
		base.gameObject.layer = 14;
		this.travelVec = Vector3Ex.Direction2D(base.transform.position, fromPos);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.UpdateIdealAltitude();
	}

	// Token: 0x06001606 RID: 5638 RVA: 0x000A8868 File Offset: 0x000A6A68
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (!base.isServer)
		{
			return;
		}
		if (info.damageTypes.Has(DamageType.Heat) && this.CanIgnite())
		{
			this.Ignite(info.PointStart);
			return;
		}
		if (base.IsOn() && !base.IsBroken())
		{
			this.StartSinking();
		}
	}

	// Token: 0x06001607 RID: 5639 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void SelfDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001608 RID: 5640 RVA: 0x000A88BE File Offset: 0x000A6ABE
	public bool CanIgnite()
	{
		return !base.IsOn() && !base.IsBroken();
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x000A88D4 File Offset: 0x000A6AD4
	public void UpdateIdealAltitude()
	{
		if (!base.IsOn())
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		float a = (heightMap != null) ? heightMap.GetHeight(base.transform.position) : 0f;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		float b = (waterMap != null) ? waterMap.GetHeight(base.transform.position) : 0f;
		this.idealAltitude = Mathf.Max(a, b) + this.hoverHeight;
		if (this.hoverHeight != 0f)
		{
			this.idealAltitude -= 2f * Mathf.Abs(this.randOffset);
		}
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x000A896B File Offset: 0x000A6B6B
	public void StartSinking()
	{
		if (base.IsBroken())
		{
			return;
		}
		this.hoverHeight = 0f;
		this.travelVec = Vector3.zero;
		this.UpdateIdealAltitude();
		base.SetFlag(BaseEntity.Flags.Broken, true, false, true);
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x000A89A0 File Offset: 0x000A6BA0
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (!base.IsOn())
		{
			return;
		}
		float value = Mathf.Abs(base.transform.position.y - this.idealAltitude);
		float num = (base.transform.position.y < this.idealAltitude) ? -1f : 1f;
		float d = Mathf.InverseLerp(0f, 10f, value) * num;
		if (base.IsBroken())
		{
			this.travelVec = Vector3.Lerp(this.travelVec, Vector3.zero, Time.fixedDeltaTime * 0.5f);
			d = 0.7f;
		}
		Vector3 a = Vector3.zero;
		a = Vector3.up * this.gravityScale * Physics.gravity.y * d;
		a += this.travelVec * this.travelSpeed;
		Vector3 vector = base.transform.position + a * Time.fixedDeltaTime;
		Vector3 direction = Vector3Ex.Direction(vector, base.transform.position);
		float maxDistance = Vector3.Distance(vector, base.transform.position);
		RaycastHit raycastHit;
		if (!Physics.SphereCast(this.collisionCheckPoint.position, this.collisionRadius, direction, out raycastHit, maxDistance, 1218519297))
		{
			base.transform.position = vector;
			base.transform.Rotate(Vector3.up, this.rotationSpeed * this.randOffset * Time.deltaTime, Space.Self);
			return;
		}
		this.StartSinking();
	}

	// Token: 0x04000E9B RID: 3739
	public float gravityScale = -0.1f;

	// Token: 0x04000E9C RID: 3740
	public float travelSpeed = 2f;

	// Token: 0x04000E9D RID: 3741
	public float collisionRadius = 0.5f;

	// Token: 0x04000E9E RID: 3742
	public float rotationSpeed = 5f;

	// Token: 0x04000E9F RID: 3743
	public float randOffset = 1f;

	// Token: 0x04000EA0 RID: 3744
	public float lifeTime = 120f;

	// Token: 0x04000EA1 RID: 3745
	public float hoverHeight = 14f;

	// Token: 0x04000EA2 RID: 3746
	public Transform collisionCheckPoint;

	// Token: 0x04000EA3 RID: 3747
	private float idealAltitude;

	// Token: 0x04000EA4 RID: 3748
	private Vector3 travelVec = Vector3.forward;
}
