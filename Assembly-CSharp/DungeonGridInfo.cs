using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000646 RID: 1606
public class DungeonGridInfo : LandmarkInfo
{
	// Token: 0x06002DFC RID: 11772 RVA: 0x001141DC File Offset: 0x001123DC
	public float Distance(Vector3 position)
	{
		return (base.transform.position - position).magnitude;
	}

	// Token: 0x06002DFD RID: 11773 RVA: 0x00114204 File Offset: 0x00112404
	public float SqrDistance(Vector3 position)
	{
		return (base.transform.position - position).sqrMagnitude;
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x0011422C File Offset: 0x0011242C
	public bool IsValidSpawnPosition(Vector3 position)
	{
		OBB bounds = base.GetComponentInChildren<DungeonVolume>().GetBounds(position, Quaternion.identity);
		Vector3 b = WorldSpaceGrid.ClosestGridCell(bounds.position, TerrainMeta.Size.x * 2f, (float)this.CellSize);
		Vector3 vector = bounds.position - b;
		return Mathf.Abs(vector.x) > 3f || Mathf.Abs(vector.z) > 3f;
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x001142A0 File Offset: 0x001124A0
	public Vector3 SnapPosition(Vector3 pos)
	{
		pos.x = (float)Mathf.RoundToInt(pos.x / this.LinkRadius) * this.LinkRadius;
		pos.y = (float)Mathf.CeilToInt(pos.y / this.LinkHeight) * this.LinkHeight;
		pos.z = (float)Mathf.RoundToInt(pos.z / this.LinkRadius) * this.LinkRadius;
		return pos;
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x00114311 File Offset: 0x00112511
	protected override void Awake()
	{
		base.Awake();
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.DungeonGridEntrances.Add(this);
		}
	}

	// Token: 0x06002E01 RID: 11777 RVA: 0x00114335 File Offset: 0x00112535
	protected void Start()
	{
		base.transform.SetHierarchyGroup("Dungeon", true, false);
	}

	// Token: 0x040025A0 RID: 9632
	[Header("DungeonGridInfo")]
	public int CellSize = 216;

	// Token: 0x040025A1 RID: 9633
	public float LinkHeight = 1.5f;

	// Token: 0x040025A2 RID: 9634
	public float LinkRadius = 3f;

	// Token: 0x040025A3 RID: 9635
	internal List<GameObject> Links = new List<GameObject>();
}
