using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C8 RID: 456
public class AIMovePoint : AIPoint
{
	// Token: 0x06001844 RID: 6212 RVA: 0x000B3898 File Offset: 0x000B1A98
	public void OnDrawGizmos()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.green;
		GizmosUtil.DrawWireCircleY(base.transform.position, this.radius);
		Gizmos.color = color;
	}

	// Token: 0x06001845 RID: 6213 RVA: 0x000B38C4 File Offset: 0x000B1AC4
	public void DrawLookAtPoints()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.gray;
		if (this.LookAtPoints != null)
		{
			foreach (Transform transform in this.LookAtPoints)
			{
				if (!(transform == null))
				{
					Gizmos.DrawSphere(transform.position, 0.2f);
					Gizmos.DrawLine(base.transform.position, transform.position);
				}
			}
		}
		Gizmos.color = color;
	}

	// Token: 0x06001846 RID: 6214 RVA: 0x000B3960 File Offset: 0x000B1B60
	public void Clear()
	{
		this.LookAtPoints = null;
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x000B3969 File Offset: 0x000B1B69
	public void AddLookAtPoint(Transform transform)
	{
		if (this.LookAtPoints == null)
		{
			this.LookAtPoints = new List<Transform>();
		}
		this.LookAtPoints.Add(transform);
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x000B398A File Offset: 0x000B1B8A
	public bool HasLookAtPoints()
	{
		return this.LookAtPoints != null && this.LookAtPoints.Count > 0;
	}

	// Token: 0x06001849 RID: 6217 RVA: 0x000B39A4 File Offset: 0x000B1BA4
	public Transform GetRandomLookAtPoint()
	{
		if (this.LookAtPoints == null || this.LookAtPoints.Count == 0)
		{
			return null;
		}
		return this.LookAtPoints[UnityEngine.Random.Range(0, this.LookAtPoints.Count)];
	}

	// Token: 0x04001176 RID: 4470
	public ListDictionary<AIMovePoint, float> distances = new ListDictionary<AIMovePoint, float>();

	// Token: 0x04001177 RID: 4471
	public ListDictionary<AICoverPoint, float> distancesToCover = new ListDictionary<AICoverPoint, float>();

	// Token: 0x04001178 RID: 4472
	public float radius = 1f;

	// Token: 0x04001179 RID: 4473
	public float WaitTime;

	// Token: 0x0400117A RID: 4474
	public List<Transform> LookAtPoints;

	// Token: 0x02000BED RID: 3053
	public class DistTo
	{
		// Token: 0x04004038 RID: 16440
		public float distance;

		// Token: 0x04004039 RID: 16441
		public AIMovePoint target;
	}
}
