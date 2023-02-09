using System;
using UnityEngine;

// Token: 0x02000683 RID: 1667
public class TerrainFilter : PrefabAttribute
{
	// Token: 0x06002FBA RID: 12218 RVA: 0x0011CD38 File Offset: 0x0011AF38
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawCube(base.transform.position + Vector3.up * 50f * 0.5f, new Vector3(0.5f, 50f, 0.5f));
		Gizmos.DrawSphere(base.transform.position + Vector3.up * 50f, 2f);
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x0011CDCE File Offset: 0x0011AFCE
	public bool Check(Vector3 pos)
	{
		return this.Filter.GetFactor(pos, this.CheckPlacementMap) > 0f;
	}

	// Token: 0x06002FBC RID: 12220 RVA: 0x0011CDE9 File Offset: 0x0011AFE9
	protected override Type GetIndexedType()
	{
		return typeof(TerrainFilter);
	}

	// Token: 0x04002682 RID: 9858
	public SpawnFilter Filter;

	// Token: 0x04002683 RID: 9859
	public bool CheckPlacementMap = true;
}
