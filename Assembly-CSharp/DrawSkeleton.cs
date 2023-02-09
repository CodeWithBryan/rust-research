using System;
using UnityEngine;

// Token: 0x020002EA RID: 746
public class DrawSkeleton : MonoBehaviour
{
	// Token: 0x06001D53 RID: 7507 RVA: 0x000C8D34 File Offset: 0x000C6F34
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		DrawSkeleton.DrawTransform(base.transform);
	}

	// Token: 0x06001D54 RID: 7508 RVA: 0x000C8D4C File Offset: 0x000C6F4C
	private static void DrawTransform(Transform t)
	{
		for (int i = 0; i < t.childCount; i++)
		{
			Gizmos.DrawLine(t.position, t.GetChild(i).position);
			DrawSkeleton.DrawTransform(t.GetChild(i));
		}
	}
}
