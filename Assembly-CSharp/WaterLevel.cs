using System;
using UnityEngine;

// Token: 0x02000575 RID: 1397
public static class WaterLevel
{
	// Token: 0x06002A3B RID: 10811 RVA: 0x000FF2B8 File Offset: 0x000FD4B8
	public static float Factor(Vector3 start, Vector3 end, float radius, BaseEntity forEntity = null)
	{
		float result;
		using (TimeWarning.New("WaterLevel.Factor", 0))
		{
			WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(start, end, radius, forEntity, true);
			result = (waterInfo.isValid ? Mathf.InverseLerp(Mathf.Min(start.y, end.y) - radius, Mathf.Max(start.y, end.y) + radius, waterInfo.surfaceLevel) : 0f);
		}
		return result;
	}

	// Token: 0x06002A3C RID: 10812 RVA: 0x000FF33C File Offset: 0x000FD53C
	public static float Factor(Bounds bounds, BaseEntity forEntity = null)
	{
		float result;
		using (TimeWarning.New("WaterLevel.Factor", 0))
		{
			if (bounds.size == Vector3.zero)
			{
				bounds.size = new Vector3(0.1f, 0.1f, 0.1f);
			}
			WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(bounds, forEntity, true);
			result = (waterInfo.isValid ? Mathf.InverseLerp(bounds.min.y, bounds.max.y, waterInfo.surfaceLevel) : 0f);
		}
		return result;
	}

	// Token: 0x06002A3D RID: 10813 RVA: 0x000FF3DC File Offset: 0x000FD5DC
	public static bool Test(Vector3 pos, bool waves = true, BaseEntity forEntity = null)
	{
		bool isValid;
		using (TimeWarning.New("WaterLevel.Test", 0))
		{
			isValid = WaterLevel.GetWaterInfo(pos, waves, forEntity, false).isValid;
		}
		return isValid;
	}

	// Token: 0x06002A3E RID: 10814 RVA: 0x000FF424 File Offset: 0x000FD624
	public static float GetWaterDepth(Vector3 pos, bool waves = true, BaseEntity forEntity = null)
	{
		float currentDepth;
		using (TimeWarning.New("WaterLevel.GetWaterDepth", 0))
		{
			currentDepth = WaterLevel.GetWaterInfo(pos, waves, forEntity, false).currentDepth;
		}
		return currentDepth;
	}

	// Token: 0x06002A3F RID: 10815 RVA: 0x000FF46C File Offset: 0x000FD66C
	public static float GetOverallWaterDepth(Vector3 pos, bool waves = true, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		float overallDepth;
		using (TimeWarning.New("WaterLevel.GetOverallWaterDepth", 0))
		{
			overallDepth = WaterLevel.GetWaterInfo(pos, waves, forEntity, noEarlyExit).overallDepth;
		}
		return overallDepth;
	}

	// Token: 0x06002A40 RID: 10816 RVA: 0x000FF4B4 File Offset: 0x000FD6B4
	public static WaterLevel.WaterInfo GetBuoyancyWaterInfo(Vector3 pos, Vector2 posUV, float terrainHeight, float waterHeight, bool doDeepwaterChecks, BaseEntity forEntity = null)
	{
		WaterLevel.WaterInfo result;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			if (pos.y > waterHeight)
			{
				result = WaterLevel.GetWaterInfoFromVolumes(pos, forEntity);
			}
			else
			{
				bool flag = pos.y < terrainHeight - 1f;
				if (flag)
				{
					waterHeight = 0f;
					if (pos.y > waterHeight)
					{
						return waterInfo;
					}
				}
				bool flag2 = doDeepwaterChecks && pos.y < waterHeight - 10f;
				int num = TerrainMeta.TopologyMap ? TerrainMeta.TopologyMap.GetTopologyFast(posUV) : 0;
				if ((flag || flag2 || (num & 246144) == 0) && WaterSystem.Collision && WaterSystem.Collision.GetIgnore(pos, 0.01f))
				{
					result = waterInfo;
				}
				else
				{
					RaycastHit raycastHit;
					if (flag2 && Physics.Raycast(pos, Vector3.up, out raycastHit, 5f, 16, QueryTriggerInteraction.Collide))
					{
						waterHeight = Mathf.Min(waterHeight, raycastHit.point.y);
					}
					waterInfo.isValid = true;
					waterInfo.currentDepth = Mathf.Max(0f, waterHeight - pos.y);
					waterInfo.overallDepth = Mathf.Max(0f, waterHeight - terrainHeight);
					waterInfo.surfaceLevel = waterHeight;
					result = waterInfo;
				}
			}
		}
		return result;
	}

	// Token: 0x06002A41 RID: 10817 RVA: 0x000FF618 File Offset: 0x000FD818
	public static WaterLevel.WaterInfo GetWaterInfo(Vector3 pos, bool waves = true, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		WaterLevel.WaterInfo result;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			float num = 0f;
			if (waves)
			{
				num = WaterSystem.GetHeight(pos);
			}
			else if (TerrainMeta.WaterMap)
			{
				num = TerrainMeta.WaterMap.GetHeight(pos);
			}
			if (pos.y > num)
			{
				if (!noEarlyExit)
				{
					return WaterLevel.GetWaterInfoFromVolumes(pos, forEntity);
				}
				waterInfo = WaterLevel.GetWaterInfoFromVolumes(pos, forEntity);
			}
			float num2 = TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(pos) : 0f;
			if (pos.y < num2 - 1f)
			{
				num = 0f;
				if (pos.y > num && !noEarlyExit)
				{
					return waterInfo;
				}
			}
			if (WaterSystem.Collision && WaterSystem.Collision.GetIgnore(pos, 0.01f))
			{
				result = waterInfo;
			}
			else
			{
				waterInfo.isValid = true;
				waterInfo.currentDepth = Mathf.Max(0f, num - pos.y);
				waterInfo.overallDepth = Mathf.Max(0f, num - num2);
				waterInfo.surfaceLevel = num;
				result = waterInfo;
			}
		}
		return result;
	}

	// Token: 0x06002A42 RID: 10818 RVA: 0x000FF75C File Offset: 0x000FD95C
	public static WaterLevel.WaterInfo GetWaterInfo(Bounds bounds, BaseEntity forEntity = null, bool waves = true)
	{
		WaterLevel.WaterInfo result;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			float num = 0f;
			if (waves)
			{
				num = WaterSystem.GetHeight(bounds.center);
			}
			else if (TerrainMeta.WaterMap)
			{
				num = TerrainMeta.WaterMap.GetHeight(bounds.center);
			}
			if (bounds.min.y > num)
			{
				result = WaterLevel.GetWaterInfoFromVolumes(bounds, forEntity);
			}
			else
			{
				float num2 = TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(bounds.center) : 0f;
				if (bounds.max.y < num2 - 1f)
				{
					num = 0f;
					if (bounds.min.y > num)
					{
						return waterInfo;
					}
				}
				if (WaterSystem.Collision && WaterSystem.Collision.GetIgnore(bounds))
				{
					result = waterInfo;
				}
				else
				{
					waterInfo.isValid = true;
					waterInfo.currentDepth = Mathf.Max(0f, num - bounds.min.y);
					waterInfo.overallDepth = Mathf.Max(0f, num - num2);
					waterInfo.surfaceLevel = num;
					result = waterInfo;
				}
			}
		}
		return result;
	}

	// Token: 0x06002A43 RID: 10819 RVA: 0x000FF8B4 File Offset: 0x000FDAB4
	public static WaterLevel.WaterInfo GetWaterInfo(Vector3 start, Vector3 end, float radius, BaseEntity forEntity = null, bool waves = true)
	{
		WaterLevel.WaterInfo result;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			float num = 0f;
			Vector3 vector = (start + end) * 0.5f;
			float num2 = Mathf.Min(start.y, end.y) - radius;
			float num3 = Mathf.Max(start.y, end.y) + radius;
			if (waves)
			{
				num = WaterSystem.GetHeight(vector);
			}
			else if (TerrainMeta.WaterMap)
			{
				num = TerrainMeta.WaterMap.GetHeight(vector);
			}
			if (num2 > num)
			{
				result = WaterLevel.GetWaterInfoFromVolumes(start, end, radius, forEntity);
			}
			else
			{
				float num4 = TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(vector) : 0f;
				if (num3 < num4 - 1f)
				{
					num = 0f;
					if (num2 > num)
					{
						return waterInfo;
					}
				}
				if (WaterSystem.Collision && WaterSystem.Collision.GetIgnore(start, end, radius))
				{
					Vector3 vector2 = vector.WithY(Mathf.Lerp(num2, num3, 0.75f));
					if (WaterSystem.Collision.GetIgnore(vector2, 0.01f))
					{
						return waterInfo;
					}
					num = Mathf.Min(num, vector2.y);
				}
				waterInfo.isValid = true;
				waterInfo.currentDepth = Mathf.Max(0f, num - num2);
				waterInfo.overallDepth = Mathf.Max(0f, num - num4);
				waterInfo.surfaceLevel = num;
				result = waterInfo;
			}
		}
		return result;
	}

	// Token: 0x06002A44 RID: 10820 RVA: 0x000FFA50 File Offset: 0x000FDC50
	private static WaterLevel.WaterInfo GetWaterInfoFromVolumes(Bounds bounds, BaseEntity forEntity)
	{
		WaterLevel.WaterInfo result = default(WaterLevel.WaterInfo);
		if (forEntity == null)
		{
			return result;
		}
		forEntity.WaterTestFromVolumes(bounds, out result);
		return result;
	}

	// Token: 0x06002A45 RID: 10821 RVA: 0x000FFA7C File Offset: 0x000FDC7C
	private static WaterLevel.WaterInfo GetWaterInfoFromVolumes(Vector3 pos, BaseEntity forEntity)
	{
		WaterLevel.WaterInfo result = default(WaterLevel.WaterInfo);
		if (forEntity == null)
		{
			return result;
		}
		forEntity.WaterTestFromVolumes(pos, out result);
		return result;
	}

	// Token: 0x06002A46 RID: 10822 RVA: 0x000FFAA8 File Offset: 0x000FDCA8
	private static WaterLevel.WaterInfo GetWaterInfoFromVolumes(Vector3 start, Vector3 end, float radius, BaseEntity forEntity)
	{
		WaterLevel.WaterInfo result = default(WaterLevel.WaterInfo);
		if (forEntity == null)
		{
			return result;
		}
		forEntity.WaterTestFromVolumes(start, end, radius, out result);
		return result;
	}

	// Token: 0x02000D07 RID: 3335
	public struct WaterInfo
	{
		// Token: 0x040044AD RID: 17581
		public bool isValid;

		// Token: 0x040044AE RID: 17582
		public float currentDepth;

		// Token: 0x040044AF RID: 17583
		public float overallDepth;

		// Token: 0x040044B0 RID: 17584
		public float surfaceLevel;
	}
}
