using System;
using UnityEngine;

// Token: 0x020006AD RID: 1709
public class PlaceDecorWhiteNoise : ProceduralComponent
{
	// Token: 0x06003044 RID: 12356 RVA: 0x001270F8 File Offset: 0x001252F8
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			World.Spawn("Decor", "assets/bundled/prefabs/autospawn/" + this.ResourceFolder + "/");
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, null, null, true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		int num = Mathf.RoundToInt(this.ObjectDensity * size.x * size.z * 1E-06f);
		float x = position.x;
		float z = position.z;
		float max = position.x + size.x;
		float max2 = position.z + size.z;
		for (int i = 0; i < num; i++)
		{
			float x2 = SeedRandom.Range(ref seed, x, max);
			float z2 = SeedRandom.Range(ref seed, z, max2);
			float normX = TerrainMeta.NormalizeX(x2);
			float normZ = TerrainMeta.NormalizeZ(z2);
			float num2 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(normX, normZ, true);
			Prefab random = array.GetRandom(ref seed);
			if (factor * factor >= num2)
			{
				float height = heightMap.GetHeight(normX, normZ);
				Vector3 vector = new Vector3(x2, height, z2);
				Quaternion localRotation = random.Object.transform.localRotation;
				Vector3 localScale = random.Object.transform.localScale;
				random.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
				if (random.ApplyTerrainAnchors(ref vector, localRotation, localScale, this.Filter) && random.ApplyTerrainChecks(vector, localRotation, localScale, this.Filter) && random.ApplyTerrainFilters(vector, localRotation, localScale, null) && random.ApplyWaterChecks(vector, localRotation, localScale))
				{
					World.AddPrefab("Decor", random, vector, localRotation, localScale);
				}
			}
		}
	}

	// Token: 0x04002726 RID: 10022
	public SpawnFilter Filter;

	// Token: 0x04002727 RID: 10023
	public string ResourceFolder = string.Empty;

	// Token: 0x04002728 RID: 10024
	public float ObjectDensity = 100f;
}
