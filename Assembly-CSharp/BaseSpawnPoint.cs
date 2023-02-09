using System;
using UnityEngine;

// Token: 0x02000541 RID: 1345
public abstract class BaseSpawnPoint : MonoBehaviour, IServerComponent
{
	// Token: 0x060028FF RID: 10495
	public abstract void GetLocation(out Vector3 pos, out Quaternion rot);

	// Token: 0x06002900 RID: 10496
	public abstract void ObjectSpawned(SpawnPointInstance instance);

	// Token: 0x06002901 RID: 10497
	public abstract void ObjectRetired(SpawnPointInstance instance);

	// Token: 0x06002902 RID: 10498 RVA: 0x000F9C48 File Offset: 0x000F7E48
	public virtual bool IsAvailableTo(GameObjectRef prefabRef)
	{
		return base.gameObject.activeSelf;
	}

	// Token: 0x06002903 RID: 10499 RVA: 0x000F9C55 File Offset: 0x000F7E55
	public virtual bool HasPlayersIntersecting()
	{
		return BaseNetworkable.HasCloseConnections(base.transform.position, 2f);
	}

	// Token: 0x06002904 RID: 10500 RVA: 0x000F9C6C File Offset: 0x000F7E6C
	protected void DropToGround(ref Vector3 pos, ref Quaternion rot)
	{
		if (TerrainMeta.HeightMap && TerrainMeta.Collision && !TerrainMeta.Collision.GetIgnore(pos, 0.01f))
		{
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			pos.y = Mathf.Max(pos.y, height);
		}
		RaycastHit raycastHit;
		if (TransformUtil.GetGroundInfo(pos, out raycastHit, 20f, 1235288065, null))
		{
			pos = raycastHit.point;
			rot = Quaternion.LookRotation(rot * Vector3.forward, raycastHit.normal);
		}
	}
}
