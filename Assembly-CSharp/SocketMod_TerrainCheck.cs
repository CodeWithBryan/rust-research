using System;
using UnityEngine;

// Token: 0x02000262 RID: 610
public class SocketMod_TerrainCheck : SocketMod
{
	// Token: 0x06001B9B RID: 7067 RVA: 0x000C035C File Offset: 0x000BE55C
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		bool flag = SocketMod_TerrainCheck.IsInTerrain(base.transform.position);
		if (!this.wantsInTerrain)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green : Color.red);
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	// Token: 0x06001B9C RID: 7068 RVA: 0x000C03BC File Offset: 0x000BE5BC
	public static bool IsInTerrain(Vector3 vPoint)
	{
		if (TerrainMeta.OutOfBounds(vPoint))
		{
			return false;
		}
		if (!TerrainMeta.Collision || !TerrainMeta.Collision.GetIgnore(vPoint, 0.01f))
		{
			foreach (Terrain terrain in Terrain.activeTerrains)
			{
				if (terrain.SampleHeight(vPoint) + terrain.transform.position.y > vPoint.y)
				{
					return true;
				}
			}
		}
		return Physics.Raycast(new Ray(vPoint + Vector3.up * 3f, Vector3.down), 3f, 65536);
	}

	// Token: 0x06001B9D RID: 7069 RVA: 0x000C0460 File Offset: 0x000BE660
	public override bool DoCheck(Construction.Placement place)
	{
		if (SocketMod_TerrainCheck.IsInTerrain(place.position + place.rotation * this.worldPosition) == this.wantsInTerrain)
		{
			return true;
		}
		Construction.lastPlacementError = this.fullName + ": not in terrain";
		return false;
	}

	// Token: 0x040014A7 RID: 5287
	public bool wantsInTerrain = true;
}
