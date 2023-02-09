using System;
using UnityEngine;

// Token: 0x020006AE RID: 1710
public class PlaceMonument : ProceduralComponent
{
	// Token: 0x06003046 RID: 12358 RVA: 0x001272E0 File Offset: 0x001254E0
	public override void Process(uint seed)
	{
		if (!this.Monument.isValid)
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float max = position.x + size.x;
		float max2 = position.z + size.z;
		PlaceMonument.SpawnInfo spawnInfo = default(PlaceMonument.SpawnInfo);
		int num = int.MinValue;
		Prefab<MonumentInfo> prefab = Prefab.Load<MonumentInfo>(this.Monument.resourceID, null, null);
		for (int i = 0; i < 10000; i++)
		{
			float x2 = SeedRandom.Range(ref seed, x, max);
			float z2 = SeedRandom.Range(ref seed, z, max2);
			float normX = TerrainMeta.NormalizeX(x2);
			float normZ = TerrainMeta.NormalizeZ(z2);
			float num2 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(normX, normZ, true);
			if (factor * factor >= num2)
			{
				float height = heightMap.GetHeight(normX, normZ);
				Vector3 vector = new Vector3(x2, height, z2);
				Quaternion localRotation = prefab.Object.transform.localRotation;
				Vector3 localScale = prefab.Object.transform.localScale;
				prefab.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
				if ((!prefab.Component || prefab.Component.CheckPlacement(vector, localRotation, localScale)) && prefab.ApplyTerrainAnchors(ref vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainChecks(vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainFilters(vector, localRotation, localScale, null) && prefab.ApplyWaterChecks(vector, localRotation, localScale) && !prefab.CheckEnvironmentVolumes(vector, localRotation, localScale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
				{
					PlaceMonument.SpawnInfo spawnInfo2 = default(PlaceMonument.SpawnInfo);
					spawnInfo2.prefab = prefab;
					spawnInfo2.position = vector;
					spawnInfo2.rotation = localRotation;
					spawnInfo2.scale = localScale;
					int num3 = -Mathf.RoundToInt(vector.Magnitude2D());
					if (num3 > num)
					{
						num = num3;
						spawnInfo = spawnInfo2;
					}
				}
			}
		}
		if (num != -2147483648)
		{
			World.AddPrefab("Monument", spawnInfo.prefab, spawnInfo.position, spawnInfo.rotation, spawnInfo.scale);
		}
	}

	// Token: 0x04002729 RID: 10025
	public SpawnFilter Filter;

	// Token: 0x0400272A RID: 10026
	public GameObjectRef Monument;

	// Token: 0x0400272B RID: 10027
	private const int Attempts = 10000;

	// Token: 0x02000DB3 RID: 3507
	private struct SpawnInfo
	{
		// Token: 0x04004745 RID: 18245
		public Prefab prefab;

		// Token: 0x04004746 RID: 18246
		public Vector3 position;

		// Token: 0x04004747 RID: 18247
		public Quaternion rotation;

		// Token: 0x04004748 RID: 18248
		public Vector3 scale;
	}
}
