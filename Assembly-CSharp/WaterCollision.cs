using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006D0 RID: 1744
public class WaterCollision : MonoBehaviour
{
	// Token: 0x060030BC RID: 12476 RVA: 0x0012BF4C File Offset: 0x0012A14C
	private void Awake()
	{
		this.ignoredColliders = new ListDictionary<Collider, List<Collider>>();
		this.waterColliders = new HashSet<Collider>();
	}

	// Token: 0x060030BD RID: 12477 RVA: 0x0012BF64 File Offset: 0x0012A164
	public void Clear()
	{
		if (this.waterColliders.Count == 0)
		{
			return;
		}
		HashSet<Collider>.Enumerator enumerator = this.waterColliders.GetEnumerator();
		while (enumerator.MoveNext())
		{
			foreach (Collider collider in this.ignoredColliders.Keys)
			{
				Physics.IgnoreCollision(collider, enumerator.Current, false);
			}
		}
		this.ignoredColliders.Clear();
	}

	// Token: 0x060030BE RID: 12478 RVA: 0x0012BFF4 File Offset: 0x0012A1F4
	public void Reset(Collider collider)
	{
		if (this.waterColliders.Count == 0 || !collider)
		{
			return;
		}
		foreach (Collider collider2 in this.waterColliders)
		{
			Physics.IgnoreCollision(collider, collider2, false);
		}
		this.ignoredColliders.Remove(collider);
	}

	// Token: 0x060030BF RID: 12479 RVA: 0x0012C049 File Offset: 0x0012A249
	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		return GamePhysics.CheckSphere<WaterVisibilityTrigger>(pos, radius, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x060030C0 RID: 12480 RVA: 0x0012C058 File Offset: 0x0012A258
	public bool GetIgnore(Bounds bounds)
	{
		return GamePhysics.CheckBounds<WaterVisibilityTrigger>(bounds, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x060030C1 RID: 12481 RVA: 0x0012C066 File Offset: 0x0012A266
	public bool GetIgnore(Vector3 start, Vector3 end, float radius)
	{
		return GamePhysics.CheckCapsule<WaterVisibilityTrigger>(start, end, radius, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x060030C2 RID: 12482 RVA: 0x0012C076 File Offset: 0x0012A276
	public bool GetIgnore(RaycastHit hit)
	{
		return this.waterColliders.Contains(hit.collider) && this.GetIgnore(hit.point, 0.01f);
	}

	// Token: 0x060030C3 RID: 12483 RVA: 0x0012C0A0 File Offset: 0x0012A2A0
	public bool GetIgnore(Collider collider)
	{
		return this.waterColliders.Count != 0 && collider && this.ignoredColliders.Contains(collider);
	}

	// Token: 0x060030C4 RID: 12484 RVA: 0x0012C0C8 File Offset: 0x0012A2C8
	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (this.waterColliders.Count == 0 || !collider)
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
				foreach (Collider collider2 in this.waterColliders)
				{
					Physics.IgnoreCollision(collider, collider2, true);
				}
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

	// Token: 0x060030C5 RID: 12485 RVA: 0x0012C16C File Offset: 0x0012A36C
	protected void LateUpdate()
	{
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
				foreach (Collider collider in this.waterColliders)
				{
					Physics.IgnoreCollision(key, collider, false);
				}
				this.ignoredColliders.RemoveAt(i--);
			}
		}
	}

	// Token: 0x0400279A RID: 10138
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	// Token: 0x0400279B RID: 10139
	private HashSet<Collider> waterColliders;
}
