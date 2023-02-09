using System;
using UnityEngine;

// Token: 0x02000549 RID: 1353
public class RadialSpawnPoint : BaseSpawnPoint
{
	// Token: 0x0600292C RID: 10540 RVA: 0x000FA44C File Offset: 0x000F864C
	public override void GetLocation(out Vector3 pos, out Quaternion rot)
	{
		Vector2 vector = UnityEngine.Random.insideUnitCircle * this.radius;
		pos = base.transform.position + new Vector3(vector.x, 0f, vector.y);
		rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		base.DropToGround(ref pos, ref rot);
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x000FA4C2 File Offset: 0x000F86C2
	public override bool HasPlayersIntersecting()
	{
		return BaseNetworkable.HasCloseConnections(base.transform.position, this.radius + 1f);
	}

	// Token: 0x0600292E RID: 10542 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void ObjectSpawned(SpawnPointInstance instance)
	{
	}

	// Token: 0x0600292F RID: 10543 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void ObjectRetired(SpawnPointInstance instance)
	{
	}

	// Token: 0x04002170 RID: 8560
	public float radius = 10f;
}
