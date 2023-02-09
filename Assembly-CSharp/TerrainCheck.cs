using System;
using UnityEngine;

// Token: 0x02000662 RID: 1634
public class TerrainCheck : PrefabAttribute
{
	// Token: 0x06002E3B RID: 11835 RVA: 0x001155D8 File Offset: 0x001137D8
	public bool Check(Vector3 pos)
	{
		float extents = this.Extents;
		float height = TerrainMeta.HeightMap.GetHeight(pos);
		float num = pos.y - extents;
		float num2 = pos.y + extents;
		return num <= height && num2 >= height;
	}

	// Token: 0x06002E3C RID: 11836 RVA: 0x00115615 File Offset: 0x00113815
	protected override Type GetIndexedType()
	{
		return typeof(TerrainCheck);
	}

	// Token: 0x040025F7 RID: 9719
	public bool Rotate = true;

	// Token: 0x040025F8 RID: 9720
	public float Extents = 1f;
}
