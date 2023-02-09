using System;
using ConVar;
using UnityEngine;

// Token: 0x02000566 RID: 1382
public class TriggerParentEnclosed : TriggerParent
{
	// Token: 0x060029E8 RID: 10728 RVA: 0x000FDE68 File Offset: 0x000FC068
	protected void OnEnable()
	{
		this.boxCollider = base.GetComponent<BoxCollider>();
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x000FDE76 File Offset: 0x000FC076
	public override bool ShouldParent(BaseEntity ent, bool bypassOtherTriggerCheck = false)
	{
		return base.ShouldParent(ent, bypassOtherTriggerCheck) && this.IsInside(ent, this.Padding);
	}

	// Token: 0x060029EA RID: 10730 RVA: 0x000FDE94 File Offset: 0x000FC094
	internal override bool SkipOnTriggerExit(Collider collider)
	{
		if (!this.CheckBoundsOnUnparent)
		{
			return false;
		}
		if (!Debugging.checkparentingtriggers)
		{
			return false;
		}
		BaseEntity baseEntity = collider.ToBaseEntity();
		return !(baseEntity == null) && this.IsInside(baseEntity, 0f);
	}

	// Token: 0x060029EB RID: 10731 RVA: 0x000FDED4 File Offset: 0x000FC0D4
	private bool IsInside(BaseEntity ent, float padding)
	{
		Bounds bounds = new Bounds(this.boxCollider.center, this.boxCollider.size);
		if (padding > 0f)
		{
			bounds.Expand(padding);
		}
		OBB obb = new OBB(this.boxCollider.transform, bounds);
		Vector3 target = (this.intersectionMode == TriggerParentEnclosed.TriggerMode.TriggerPoint) ? ent.TriggerPoint() : ent.PivotPoint();
		return obb.Contains(target);
	}

	// Token: 0x040021E6 RID: 8678
	public float Padding;

	// Token: 0x040021E7 RID: 8679
	[Tooltip("AnyIntersect: Look for any intersection with the trigger. OriginIntersect: Only consider objects in the trigger if their origin is inside")]
	public TriggerParentEnclosed.TriggerMode intersectionMode;

	// Token: 0x040021E8 RID: 8680
	public bool CheckBoundsOnUnparent;

	// Token: 0x040021E9 RID: 8681
	private BoxCollider boxCollider;

	// Token: 0x02000D05 RID: 3333
	public enum TriggerMode
	{
		// Token: 0x040044A6 RID: 17574
		TriggerPoint,
		// Token: 0x040044A7 RID: 17575
		PivotPoint
	}
}
