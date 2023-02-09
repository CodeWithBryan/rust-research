using System;
using UnityEngine;

// Token: 0x02000524 RID: 1316
public class MonumentInfo : LandmarkInfo, IPrefabPreProcess
{
	// Token: 0x06002863 RID: 10339 RVA: 0x000F62FA File Offset: 0x000F44FA
	protected override void Awake()
	{
		base.Awake();
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.Monuments.Add(this);
		}
	}

	// Token: 0x06002864 RID: 10340 RVA: 0x000F6320 File Offset: 0x000F4520
	public bool CheckPlacement(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		OBB obb = new OBB(pos, scale, rot, this.Bounds);
		Vector3 point = obb.GetPoint(-1f, 0f, -1f);
		Vector3 point2 = obb.GetPoint(-1f, 0f, 1f);
		Vector3 point3 = obb.GetPoint(1f, 0f, -1f);
		Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
		int topology = TerrainMeta.TopologyMap.GetTopology(point);
		int topology2 = TerrainMeta.TopologyMap.GetTopology(point2);
		int topology3 = TerrainMeta.TopologyMap.GetTopology(point3);
		int topology4 = TerrainMeta.TopologyMap.GetTopology(point4);
		int num = MonumentInfo.TierToMask(this.Tier);
		int num2 = 0;
		if ((num & topology) != 0)
		{
			num2++;
		}
		if ((num & topology2) != 0)
		{
			num2++;
		}
		if ((num & topology3) != 0)
		{
			num2++;
		}
		if ((num & topology4) != 0)
		{
			num2++;
		}
		return num2 >= 3;
	}

	// Token: 0x06002865 RID: 10341 RVA: 0x000F6414 File Offset: 0x000F4614
	public float Distance(Vector3 position)
	{
		return new OBB(base.transform.position, base.transform.rotation, this.Bounds).Distance(position);
	}

	// Token: 0x06002866 RID: 10342 RVA: 0x000F644C File Offset: 0x000F464C
	public float SqrDistance(Vector3 position)
	{
		return new OBB(base.transform.position, base.transform.rotation, this.Bounds).SqrDistance(position);
	}

	// Token: 0x06002867 RID: 10343 RVA: 0x000F6484 File Offset: 0x000F4684
	public float Distance(OBB obb)
	{
		return new OBB(base.transform.position, base.transform.rotation, this.Bounds).Distance(obb);
	}

	// Token: 0x06002868 RID: 10344 RVA: 0x000F64BC File Offset: 0x000F46BC
	public float SqrDistance(OBB obb)
	{
		return new OBB(base.transform.position, base.transform.rotation, this.Bounds).SqrDistance(obb);
	}

	// Token: 0x06002869 RID: 10345 RVA: 0x000F64F4 File Offset: 0x000F46F4
	public bool IsInBounds(Vector3 position)
	{
		return new OBB(base.transform.position, base.transform.rotation, this.Bounds).Contains(position);
	}

	// Token: 0x0600286A RID: 10346 RVA: 0x000F652C File Offset: 0x000F472C
	public Vector3 ClosestPointOnBounds(Vector3 position)
	{
		return new OBB(base.transform.position, base.transform.rotation, this.Bounds).ClosestPoint(position);
	}

	// Token: 0x0600286B RID: 10347 RVA: 0x000F6564 File Offset: 0x000F4764
	public PathFinder.Point GetPathFinderPoint(int res)
	{
		Vector3 position = base.transform.position;
		float num = TerrainMeta.NormalizeX(position.x);
		float num2 = TerrainMeta.NormalizeZ(position.z);
		return new PathFinder.Point
		{
			x = Mathf.Clamp((int)(num * (float)res), 0, res - 1),
			y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1)
		};
	}

	// Token: 0x0600286C RID: 10348 RVA: 0x000F65C8 File Offset: 0x000F47C8
	public int GetPathFinderRadius(int res)
	{
		float a = this.Bounds.extents.x * TerrainMeta.OneOverSize.x;
		float b = this.Bounds.extents.z * TerrainMeta.OneOverSize.z;
		return Mathf.CeilToInt(Mathf.Max(a, b) * (float)res);
	}

	// Token: 0x0600286D RID: 10349 RVA: 0x000F661C File Offset: 0x000F481C
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(0f, 0.7f, 1f, 0.1f);
		Gizmos.DrawCube(this.Bounds.center, this.Bounds.size);
		Gizmos.color = new Color(0f, 0.7f, 1f, 1f);
		Gizmos.DrawWireCube(this.Bounds.center, this.Bounds.size);
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x000F66AB File Offset: 0x000F48AB
	public MonumentNavMesh GetMonumentNavMesh()
	{
		return base.GetComponent<MonumentNavMesh>();
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x000F66B4 File Offset: 0x000F48B4
	public static int TierToMask(MonumentTier tier)
	{
		int num = 0;
		if ((tier & MonumentTier.Tier0) != (MonumentTier)0)
		{
			num |= 67108864;
		}
		if ((tier & MonumentTier.Tier1) != (MonumentTier)0)
		{
			num |= 134217728;
		}
		if ((tier & MonumentTier.Tier2) != (MonumentTier)0)
		{
			num |= 268435456;
		}
		return num;
	}

	// Token: 0x06002870 RID: 10352 RVA: 0x000F66EB File Offset: 0x000F48EB
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.HasDungeonLink = this.DetermineHasDungeonLink();
		this.WantsDungeonLink = this.DetermineWantsDungeonLink();
		this.DungeonEntrance = this.FindDungeonEntrance();
	}

	// Token: 0x06002871 RID: 10353 RVA: 0x000F6711 File Offset: 0x000F4911
	private DungeonGridInfo FindDungeonEntrance()
	{
		return base.GetComponentInChildren<DungeonGridInfo>();
	}

	// Token: 0x06002872 RID: 10354 RVA: 0x000F6719 File Offset: 0x000F4919
	private bool DetermineHasDungeonLink()
	{
		return base.GetComponentInChildren<DungeonGridLink>() != null;
	}

	// Token: 0x06002873 RID: 10355 RVA: 0x000F6728 File Offset: 0x000F4928
	private bool DetermineWantsDungeonLink()
	{
		return this.Type != MonumentType.WaterWell && (this.Type != MonumentType.Building || !this.displayPhrase.token.StartsWith("mining_quarry")) && (this.Type != MonumentType.Radtown || !this.displayPhrase.token.StartsWith("swamp"));
	}

	// Token: 0x040020D6 RID: 8406
	[Header("MonumentInfo")]
	public MonumentType Type = MonumentType.Building;

	// Token: 0x040020D7 RID: 8407
	[InspectorFlags]
	public MonumentTier Tier = (MonumentTier)(-1);

	// Token: 0x040020D8 RID: 8408
	public int MinWorldSize;

	// Token: 0x040020D9 RID: 8409
	public Bounds Bounds = new Bounds(Vector3.zero, Vector3.zero);

	// Token: 0x040020DA RID: 8410
	public bool HasNavmesh;

	// Token: 0x040020DB RID: 8411
	public bool IsSafeZone;

	// Token: 0x040020DC RID: 8412
	[HideInInspector]
	public bool WantsDungeonLink;

	// Token: 0x040020DD RID: 8413
	[HideInInspector]
	public bool HasDungeonLink;

	// Token: 0x040020DE RID: 8414
	[HideInInspector]
	public DungeonGridInfo DungeonEntrance;
}
