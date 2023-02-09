using System;
using UnityEngine;

// Token: 0x02000573 RID: 1395
public class ValidBounds : SingletonComponent<ValidBounds>
{
	// Token: 0x06002A29 RID: 10793 RVA: 0x000FED15 File Offset: 0x000FCF15
	public static bool Test(Vector3 vPos)
	{
		return !SingletonComponent<ValidBounds>.Instance || SingletonComponent<ValidBounds>.Instance.IsInside(vPos);
	}

	// Token: 0x06002A2A RID: 10794 RVA: 0x000FED30 File Offset: 0x000FCF30
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(this.worldBounds.center, this.worldBounds.size);
	}

	// Token: 0x06002A2B RID: 10795 RVA: 0x000FED58 File Offset: 0x000FCF58
	internal bool IsInside(Vector3 vPos)
	{
		if (vPos.IsNaNOrInfinity())
		{
			return false;
		}
		if (!this.worldBounds.Contains(vPos))
		{
			return false;
		}
		if (TerrainMeta.Terrain != null)
		{
			if (World.Procedural && vPos.y < TerrainMeta.Position.y)
			{
				return false;
			}
			if (TerrainMeta.OutOfMargin(vPos))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400220A RID: 8714
	public Bounds worldBounds;
}
