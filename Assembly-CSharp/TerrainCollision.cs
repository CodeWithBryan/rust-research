using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000667 RID: 1639
public class TerrainCollision : TerrainExtension
{
	// Token: 0x06002E45 RID: 11845 RVA: 0x001159DE File Offset: 0x00113BDE
	public override void Setup()
	{
		this.ignoredColliders = new ListDictionary<Collider, List<Collider>>();
		this.terrainCollider = this.terrain.GetComponent<TerrainCollider>();
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x001159FC File Offset: 0x00113BFC
	public void Clear()
	{
		if (!this.terrainCollider)
		{
			return;
		}
		foreach (Collider collider in this.ignoredColliders.Keys)
		{
			Physics.IgnoreCollision(collider, this.terrainCollider, false);
		}
		this.ignoredColliders.Clear();
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x00115A74 File Offset: 0x00113C74
	public void Reset(Collider collider)
	{
		if (!this.terrainCollider || !collider)
		{
			return;
		}
		Physics.IgnoreCollision(collider, this.terrainCollider, false);
		this.ignoredColliders.Remove(collider);
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x00115AA6 File Offset: 0x00113CA6
	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		return GamePhysics.CheckSphere<TerrainCollisionTrigger>(pos, radius, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x00115AB5 File Offset: 0x00113CB5
	public bool GetIgnore(RaycastHit hit)
	{
		return hit.collider is TerrainCollider && this.GetIgnore(hit.point, 0.01f);
	}

	// Token: 0x06002E4A RID: 11850 RVA: 0x00115AD9 File Offset: 0x00113CD9
	public bool GetIgnore(Collider collider)
	{
		return this.terrainCollider && collider && this.ignoredColliders.Contains(collider);
	}

	// Token: 0x06002E4B RID: 11851 RVA: 0x00115B00 File Offset: 0x00113D00
	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (!this.terrainCollider || !collider)
		{
			return;
		}
		if (!this.GetIgnore(collider))
		{
			if (ignore)
			{
				List<Collider> val = new List<Collider>
				{
					trigger
				};
				Physics.IgnoreCollision(collider, this.terrainCollider, true);
				this.ignoredColliders.Add(collider, val);
				return;
			}
		}
		else
		{
			List<Collider> list = this.ignoredColliders[collider];
			if (ignore)
			{
				if (!list.Contains(trigger))
				{
					list.Add(trigger);
					return;
				}
			}
			else if (list.Contains(trigger))
			{
				list.Remove(trigger);
			}
		}
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x00115B8C File Offset: 0x00113D8C
	protected void LateUpdate()
	{
		if (this.ignoredColliders == null)
		{
			return;
		}
		for (int i = 0; i < this.ignoredColliders.Count; i++)
		{
			KeyValuePair<Collider, List<Collider>> byIndex = this.ignoredColliders.GetByIndex(i);
			Collider key = byIndex.Key;
			List<Collider> value = byIndex.Value;
			if (key == null)
			{
				this.ignoredColliders.RemoveAt(i--);
			}
			else if (value.Count == 0)
			{
				Physics.IgnoreCollision(key, this.terrainCollider, false);
				this.ignoredColliders.RemoveAt(i--);
			}
		}
	}

	// Token: 0x0400261B RID: 9755
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	// Token: 0x0400261C RID: 9756
	private TerrainCollider terrainCollider;
}
