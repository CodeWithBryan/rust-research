using System;
using UnityEngine;

// Token: 0x0200055F RID: 1375
public class TriggerNoSpray : TriggerBase
{
	// Token: 0x060029CA RID: 10698 RVA: 0x000FD814 File Offset: 0x000FBA14
	private void OnEnable()
	{
		this.cachedTransform = base.transform;
		this.cachedBounds = new OBB(this.cachedTransform, new Bounds(this.TriggerCollider.center, this.TriggerCollider.size));
	}

	// Token: 0x060029CB RID: 10699 RVA: 0x000FD850 File Offset: 0x000FBA50
	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.ToPlayer() == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x060029CC RID: 10700 RVA: 0x000FD885 File Offset: 0x000FBA85
	public bool IsPositionValid(Vector3 worldPosition)
	{
		return !this.cachedBounds.Contains(worldPosition);
	}

	// Token: 0x040021D2 RID: 8658
	public BoxCollider TriggerCollider;

	// Token: 0x040021D3 RID: 8659
	private OBB cachedBounds;

	// Token: 0x040021D4 RID: 8660
	private Transform cachedTransform;
}
