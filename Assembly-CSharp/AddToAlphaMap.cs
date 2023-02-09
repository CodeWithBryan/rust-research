using System;
using UnityEngine;

// Token: 0x020006BA RID: 1722
public class AddToAlphaMap : ProceduralObject
{
	// Token: 0x06003066 RID: 12390 RVA: 0x0012A180 File Offset: 0x00128380
	public override void Process()
	{
		OBB obb = new OBB(base.transform, this.bounds);
		Vector3 point = obb.GetPoint(-1f, 0f, -1f);
		Vector3 point2 = obb.GetPoint(1f, 0f, -1f);
		Vector3 point3 = obb.GetPoint(-1f, 0f, 1f);
		Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
		TerrainMeta.AlphaMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
		{
			TerrainMeta.AlphaMap.SetAlpha(x, z, 0f);
		});
		GameManager.Destroy(this, 0f);
	}

	// Token: 0x04002765 RID: 10085
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
}
