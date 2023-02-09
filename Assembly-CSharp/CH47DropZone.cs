using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000458 RID: 1112
public class CH47DropZone : MonoBehaviour
{
	// Token: 0x0600247F RID: 9343 RVA: 0x000E6AC0 File Offset: 0x000E4CC0
	public void Awake()
	{
		if (!CH47DropZone.dropZones.Contains(this))
		{
			CH47DropZone.dropZones.Add(this);
		}
	}

	// Token: 0x06002480 RID: 9344 RVA: 0x000E6ADC File Offset: 0x000E4CDC
	public static CH47DropZone GetClosest(Vector3 pos)
	{
		float num = float.PositiveInfinity;
		CH47DropZone result = null;
		foreach (CH47DropZone ch47DropZone in CH47DropZone.dropZones)
		{
			float num2 = Vector3Ex.Distance2D(pos, ch47DropZone.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = ch47DropZone;
			}
		}
		return result;
	}

	// Token: 0x06002481 RID: 9345 RVA: 0x000E6B50 File Offset: 0x000E4D50
	public void OnDestroy()
	{
		if (CH47DropZone.dropZones.Contains(this))
		{
			CH47DropZone.dropZones.Remove(this);
		}
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x000E6B6B File Offset: 0x000E4D6B
	public float TimeSinceLastDrop()
	{
		return Time.time - this.lastDropTime;
	}

	// Token: 0x06002483 RID: 9347 RVA: 0x000E6B79 File Offset: 0x000E4D79
	public void Used()
	{
		this.lastDropTime = Time.time;
	}

	// Token: 0x06002484 RID: 9348 RVA: 0x000E6B86 File Offset: 0x000E4D86
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.position, 5f);
	}

	// Token: 0x04001D29 RID: 7465
	public float lastDropTime;

	// Token: 0x04001D2A RID: 7466
	private static List<CH47DropZone> dropZones = new List<CH47DropZone>();
}
