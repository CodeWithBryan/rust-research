using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000400 RID: 1024
public class SphereEntity : global::BaseEntity
{
	// Token: 0x0600227F RID: 8831 RVA: 0x000DCF5D File Offset: 0x000DB15D
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.currentRadius;
	}

	// Token: 0x06002280 RID: 8832 RVA: 0x000DCF8C File Offset: 0x000DB18C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer)
		{
			if (info.msg.sphereEntity != null)
			{
				this.currentRadius = (this.lerpRadius = info.msg.sphereEntity.radius);
			}
			this.UpdateScale();
		}
	}

	// Token: 0x06002281 RID: 8833 RVA: 0x000DCFDA File Offset: 0x000DB1DA
	public void LerpRadiusTo(float radius, float speed)
	{
		this.lerpRadius = radius;
		this.lerpSpeed = speed;
	}

	// Token: 0x06002282 RID: 8834 RVA: 0x000DCFEA File Offset: 0x000DB1EA
	protected void UpdateScale()
	{
		base.transform.localScale = new Vector3(this.currentRadius, this.currentRadius, this.currentRadius);
	}

	// Token: 0x06002283 RID: 8835 RVA: 0x000DD010 File Offset: 0x000DB210
	protected void Update()
	{
		if (this.currentRadius == this.lerpRadius)
		{
			return;
		}
		if (base.isServer)
		{
			this.currentRadius = Mathf.MoveTowards(this.currentRadius, this.lerpRadius, Time.deltaTime * this.lerpSpeed);
			this.UpdateScale();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x04001B00 RID: 6912
	public float currentRadius = 1f;

	// Token: 0x04001B01 RID: 6913
	public float lerpRadius = 1f;

	// Token: 0x04001B02 RID: 6914
	public float lerpSpeed = 1f;
}
