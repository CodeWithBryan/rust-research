using System;
using UnityEngine;

// Token: 0x02000655 RID: 1621
public class TerrainPathConnect : MonoBehaviour
{
	// Token: 0x06002E1E RID: 11806 RVA: 0x00114B54 File Offset: 0x00112D54
	public PathFinder.Point GetPathFinderPoint(int res, Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return new PathFinder.Point
		{
			x = Mathf.Clamp((int)(num * (float)res), 0, res - 1),
			y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1)
		};
	}

	// Token: 0x06002E1F RID: 11807 RVA: 0x00114BAC File Offset: 0x00112DAC
	public PathFinder.Point GetPathFinderPoint(int res)
	{
		return this.GetPathFinderPoint(res, base.transform.position);
	}

	// Token: 0x040025D3 RID: 9683
	public InfrastructureType Type;
}
