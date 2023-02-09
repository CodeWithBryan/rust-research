using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200056C RID: 1388
public class TriggerSafeZone : TriggerBase
{
	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06002A07 RID: 10759 RVA: 0x000FE62B File Offset: 0x000FC82B
	// (set) Token: 0x06002A08 RID: 10760 RVA: 0x000FE633 File Offset: 0x000FC833
	public Collider triggerCollider { get; private set; }

	// Token: 0x06002A09 RID: 10761 RVA: 0x000FE63C File Offset: 0x000FC83C
	protected void Awake()
	{
		this.triggerCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06002A0A RID: 10762 RVA: 0x000FE64A File Offset: 0x000FC84A
	protected void OnEnable()
	{
		TriggerSafeZone.allSafeZones.Add(this);
	}

	// Token: 0x06002A0B RID: 10763 RVA: 0x000FE657 File Offset: 0x000FC857
	protected override void OnDisable()
	{
		base.OnDisable();
		TriggerSafeZone.allSafeZones.Remove(this);
	}

	// Token: 0x06002A0C RID: 10764 RVA: 0x000FE66C File Offset: 0x000FC86C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002A0D RID: 10765 RVA: 0x000FE6B0 File Offset: 0x000FC8B0
	public bool PassesHeightChecks(Vector3 entPos)
	{
		Vector3 position = base.transform.position;
		float num = Mathf.Abs(position.y - entPos.y);
		return (this.maxDepth == -1f || entPos.y >= position.y || num <= this.maxDepth) && (this.maxAltitude == -1f || entPos.y <= position.y || num <= this.maxAltitude);
	}

	// Token: 0x06002A0E RID: 10766 RVA: 0x000FE729 File Offset: 0x000FC929
	public float GetSafeLevel(Vector3 pos)
	{
		if (!this.PassesHeightChecks(pos))
		{
			return 0f;
		}
		return 1f;
	}

	// Token: 0x040021F6 RID: 8694
	public static List<TriggerSafeZone> allSafeZones = new List<TriggerSafeZone>();

	// Token: 0x040021F7 RID: 8695
	public float maxDepth = 20f;

	// Token: 0x040021F8 RID: 8696
	public float maxAltitude = -1f;
}
