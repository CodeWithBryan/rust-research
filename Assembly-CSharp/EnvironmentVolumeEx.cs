using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004DD RID: 1245
public static class EnvironmentVolumeEx
{
	// Token: 0x060027B0 RID: 10160 RVA: 0x000F34AC File Offset: 0x000F16AC
	public static bool CheckEnvironmentVolumes(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type)
	{
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		transform.GetComponentsInChildren<EnvironmentVolume>(true, list);
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			OBB obb = new OBB(environmentVolume.transform, new Bounds(environmentVolume.Center, environmentVolume.Size));
			obb.Transform(pos, scale, rot);
			if (EnvironmentManager.Check(obb, type))
			{
				Pool.FreeList<EnvironmentVolume>(ref list);
				return true;
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return false;
	}

	// Token: 0x060027B1 RID: 10161 RVA: 0x000F3523 File Offset: 0x000F1723
	public static bool CheckEnvironmentVolumes(this Transform transform, EnvironmentType type)
	{
		return transform.CheckEnvironmentVolumes(transform.position, transform.rotation, transform.lossyScale, type);
	}

	// Token: 0x060027B2 RID: 10162 RVA: 0x000F3540 File Offset: 0x000F1740
	public static bool CheckEnvironmentVolumesInsideTerrain(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		if (TerrainMeta.HeightMap == null)
		{
			return true;
		}
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		transform.GetComponentsInChildren<EnvironmentVolume>(true, list);
		if (list.Count == 0)
		{
			Pool.FreeList<EnvironmentVolume>(ref list);
			return true;
		}
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			if ((environmentVolume.Type & type) != (EnvironmentType)0)
			{
				OBB obb = new OBB(environmentVolume.transform, new Bounds(environmentVolume.Center, environmentVolume.Size));
				obb.Transform(pos, scale, rot);
				Vector3 point = obb.GetPoint(-1f, 0f, -1f);
				Vector3 point2 = obb.GetPoint(1f, 0f, -1f);
				Vector3 point3 = obb.GetPoint(-1f, 0f, 1f);
				Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
				float max = obb.ToBounds().max.y + padding;
				bool fail = false;
				TerrainMeta.HeightMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
				{
					if (TerrainMeta.HeightMap.GetHeight(x, z) <= max)
					{
						fail = true;
					}
				});
				if (fail)
				{
					Pool.FreeList<EnvironmentVolume>(ref list);
					return false;
				}
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return true;
	}

	// Token: 0x060027B3 RID: 10163 RVA: 0x000F3692 File Offset: 0x000F1892
	public static bool CheckEnvironmentVolumesInsideTerrain(this Transform transform, EnvironmentType type)
	{
		return transform.CheckEnvironmentVolumesInsideTerrain(transform.position, transform.rotation, transform.lossyScale, type, 0f);
	}

	// Token: 0x060027B4 RID: 10164 RVA: 0x000F36B4 File Offset: 0x000F18B4
	public static bool CheckEnvironmentVolumesOutsideTerrain(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		if (TerrainMeta.HeightMap == null)
		{
			return true;
		}
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		transform.GetComponentsInChildren<EnvironmentVolume>(true, list);
		if (list.Count == 0)
		{
			Pool.FreeList<EnvironmentVolume>(ref list);
			return true;
		}
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			if ((environmentVolume.Type & type) != (EnvironmentType)0)
			{
				OBB obb = new OBB(environmentVolume.transform, new Bounds(environmentVolume.Center, environmentVolume.Size));
				obb.Transform(pos, scale, rot);
				Vector3 point = obb.GetPoint(-1f, 0f, -1f);
				Vector3 point2 = obb.GetPoint(1f, 0f, -1f);
				Vector3 point3 = obb.GetPoint(-1f, 0f, 1f);
				Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
				float min = obb.ToBounds().min.y - padding;
				bool fail = false;
				TerrainMeta.HeightMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
				{
					if (TerrainMeta.HeightMap.GetHeight(x, z) >= min)
					{
						fail = true;
					}
				});
				if (fail)
				{
					Pool.FreeList<EnvironmentVolume>(ref list);
					return false;
				}
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return true;
	}

	// Token: 0x060027B5 RID: 10165 RVA: 0x000F3806 File Offset: 0x000F1A06
	public static bool CheckEnvironmentVolumesOutsideTerrain(this Transform transform, EnvironmentType type)
	{
		return transform.CheckEnvironmentVolumesOutsideTerrain(transform.position, transform.rotation, transform.lossyScale, type, 0f);
	}
}
