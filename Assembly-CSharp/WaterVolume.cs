using System;
using UnityEngine;

// Token: 0x02000577 RID: 1399
public class WaterVolume : TriggerBase
{
	// Token: 0x06002A4B RID: 10827 RVA: 0x000FFB68 File Offset: 0x000FDD68
	private void OnEnable()
	{
		this.cachedTransform = base.transform;
		this.cachedBounds = new OBB(this.cachedTransform, this.WaterBounds);
	}

	// Token: 0x06002A4C RID: 10828 RVA: 0x000FFB90 File Offset: 0x000FDD90
	public bool Test(Vector3 pos, out WaterLevel.WaterInfo info)
	{
		if (!this.waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		this.UpdateCachedTransform();
		if (!this.cachedBounds.Contains(pos))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		if (!this.CheckCutOffPlanes(pos))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		Plane plane = new Plane(this.cachedBounds.up, this.cachedBounds.position);
		Vector3 a = plane.ClosestPointOnPlane(pos);
		float y = (a + this.cachedBounds.up * this.cachedBounds.extents.y).y;
		float y2 = (a + -this.cachedBounds.up * this.cachedBounds.extents.y).y;
		info.isValid = true;
		info.currentDepth = Mathf.Max(0f, y - pos.y);
		info.overallDepth = Mathf.Max(0f, y - y2);
		info.surfaceLevel = y;
		return true;
	}

	// Token: 0x06002A4D RID: 10829 RVA: 0x000FFC9C File Offset: 0x000FDE9C
	public bool Test(Bounds bounds, out WaterLevel.WaterInfo info)
	{
		if (!this.waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		this.UpdateCachedTransform();
		if (!this.cachedBounds.Contains(bounds.ClosestPoint(this.cachedBounds.position)))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		if (!this.CheckCutOffPlanes(bounds.center))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		Plane plane = new Plane(this.cachedBounds.up, this.cachedBounds.position);
		Vector3 a = plane.ClosestPointOnPlane(bounds.center);
		float y = (a + this.cachedBounds.up * this.cachedBounds.extents.y).y;
		float y2 = (a + -this.cachedBounds.up * this.cachedBounds.extents.y).y;
		info.isValid = true;
		info.currentDepth = Mathf.Max(0f, y - bounds.min.y);
		info.overallDepth = Mathf.Max(0f, y - y2);
		info.surfaceLevel = y;
		return true;
	}

	// Token: 0x06002A4E RID: 10830 RVA: 0x000FFDCC File Offset: 0x000FDFCC
	public bool Test(Vector3 start, Vector3 end, float radius, out WaterLevel.WaterInfo info)
	{
		if (!this.waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		this.UpdateCachedTransform();
		Vector3 vector = (start + end) * 0.5f;
		float num = Mathf.Min(start.y, end.y) - radius;
		if (this.cachedBounds.Distance(start) >= radius && this.cachedBounds.Distance(end) >= radius)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		if (!this.CheckCutOffPlanes(vector))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		Plane plane = new Plane(this.cachedBounds.up, this.cachedBounds.position);
		Vector3 a = plane.ClosestPointOnPlane(vector);
		float y = (a + this.cachedBounds.up * this.cachedBounds.extents.y).y;
		float y2 = (a + -this.cachedBounds.up * this.cachedBounds.extents.y).y;
		info.isValid = true;
		info.currentDepth = Mathf.Max(0f, y - num);
		info.overallDepth = Mathf.Max(0f, y - y2);
		info.surfaceLevel = y;
		return true;
	}

	// Token: 0x06002A4F RID: 10831 RVA: 0x000FFF18 File Offset: 0x000FE118
	private bool CheckCutOffPlanes(Vector3 pos)
	{
		int num = this.cutOffPlanes.Length;
		bool result = true;
		for (int i = 0; i < num; i++)
		{
			if (this.cutOffPlanes[i] != null && this.cutOffPlanes[i].InverseTransformPoint(pos).y > 0f)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	// Token: 0x06002A50 RID: 10832 RVA: 0x000FFF70 File Offset: 0x000FE170
	private void UpdateCachedTransform()
	{
		if (this.cachedTransform != null && this.cachedTransform.hasChanged)
		{
			this.cachedBounds = new OBB(this.cachedTransform, this.WaterBounds);
			this.cachedTransform.hasChanged = false;
		}
	}

	// Token: 0x06002A51 RID: 10833 RVA: 0x000FFFB0 File Offset: 0x000FE1B0
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
		return baseEntity.gameObject;
	}

	// Token: 0x0400220D RID: 8717
	public Bounds WaterBounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x0400220E RID: 8718
	private OBB cachedBounds;

	// Token: 0x0400220F RID: 8719
	private Transform cachedTransform;

	// Token: 0x04002210 RID: 8720
	public Transform[] cutOffPlanes = new Transform[0];

	// Token: 0x04002211 RID: 8721
	public bool waterEnabled = true;
}
