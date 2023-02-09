using System;
using UnityEngine;

// Token: 0x0200054B RID: 1355
public class SpaceCheckingSpawnPoint : GenericSpawnPoint
{
	// Token: 0x06002934 RID: 10548 RVA: 0x000FA50C File Offset: 0x000F870C
	public override bool IsAvailableTo(GameObjectRef prefabRef)
	{
		if (!base.IsAvailableTo(prefabRef))
		{
			return false;
		}
		if (this.useCustomBoundsCheckMask)
		{
			return SpawnHandler.CheckBounds(prefabRef.Get(), base.transform.position, base.transform.rotation, Vector3.one * this.customBoundsCheckScale, this.customBoundsCheckMask);
		}
		return SingletonComponent<SpawnHandler>.Instance.CheckBounds(prefabRef.Get(), base.transform.position, base.transform.rotation, Vector3.one * this.customBoundsCheckScale);
	}

	// Token: 0x04002171 RID: 8561
	public bool useCustomBoundsCheckMask;

	// Token: 0x04002172 RID: 8562
	public LayerMask customBoundsCheckMask;

	// Token: 0x04002173 RID: 8563
	public float customBoundsCheckScale = 1f;
}
