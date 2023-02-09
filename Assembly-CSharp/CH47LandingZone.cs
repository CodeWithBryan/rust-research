using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200045B RID: 1115
public class CH47LandingZone : MonoBehaviour
{
	// Token: 0x060024B2 RID: 9394 RVA: 0x000E7A9C File Offset: 0x000E5C9C
	public void Awake()
	{
		if (!CH47LandingZone.landingZones.Contains(this))
		{
			CH47LandingZone.landingZones.Add(this);
		}
	}

	// Token: 0x060024B3 RID: 9395 RVA: 0x000E7AB8 File Offset: 0x000E5CB8
	public static CH47LandingZone GetClosest(Vector3 pos)
	{
		float num = float.PositiveInfinity;
		CH47LandingZone result = null;
		foreach (CH47LandingZone ch47LandingZone in CH47LandingZone.landingZones)
		{
			float num2 = Vector3Ex.Distance2D(pos, ch47LandingZone.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = ch47LandingZone;
			}
		}
		return result;
	}

	// Token: 0x060024B4 RID: 9396 RVA: 0x000E7B2C File Offset: 0x000E5D2C
	public void OnDestroy()
	{
		if (CH47LandingZone.landingZones.Contains(this))
		{
			CH47LandingZone.landingZones.Remove(this);
		}
	}

	// Token: 0x060024B5 RID: 9397 RVA: 0x000E7B47 File Offset: 0x000E5D47
	public float TimeSinceLastDrop()
	{
		return Time.time - this.lastDropTime;
	}

	// Token: 0x060024B6 RID: 9398 RVA: 0x000E7B55 File Offset: 0x000E5D55
	public void Used()
	{
		this.lastDropTime = Time.time;
	}

	// Token: 0x060024B7 RID: 9399 RVA: 0x000E7B64 File Offset: 0x000E5D64
	public void OnDrawGizmos()
	{
		Color magenta = Color.magenta;
		magenta.a = 0.25f;
		Gizmos.color = magenta;
		GizmosUtil.DrawCircleY(base.transform.position, 6f);
		magenta.a = 1f;
		Gizmos.color = magenta;
		GizmosUtil.DrawWireCircleY(base.transform.position, 6f);
	}

	// Token: 0x04001D42 RID: 7490
	public float lastDropTime;

	// Token: 0x04001D43 RID: 7491
	private static List<CH47LandingZone> landingZones = new List<CH47LandingZone>();

	// Token: 0x04001D44 RID: 7492
	public float dropoffScale = 1f;
}
