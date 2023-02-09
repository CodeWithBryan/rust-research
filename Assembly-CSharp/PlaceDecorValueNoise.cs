using System;
using UnityEngine;

// Token: 0x020006AC RID: 1708
public class PlaceDecorValueNoise : ProceduralComponent
{
	// Token: 0x06003042 RID: 12354 RVA: 0x00126E60 File Offset: 0x00125060
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
		float num2 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		float num3 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		int octaves = this.Cluster.Octaves;
		float offset = this.Cluster.Offset;
		float frequency = this.Cluster.Frequency * 0.01f;
		float amplitude = this.Cluster.Amplitude;
		for (int i = 0; i < num; i++)
		{
			float num4 = SeedRandom.Range(ref seed, x, max);
			float num5 = SeedRandom.Range(ref seed, z, max2);
			float normX = TerrainMeta.NormalizeX(num4);
			float normZ = TerrainMeta.NormalizeZ(num5);
			float num6 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(normX, normZ, true);
			Prefab random = array.GetRandom(ref seed);
			if (factor > 0f && (offset + Noise.Turbulence(num2 + num4, num3 + num5, octaves, frequency, amplitude, 2f, 0.5f)) * factor * factor >= num6)
			{
				float height = heightMap.GetHeight(normX, normZ);
				Vector3 vector = new Vector3(num4, height, num5);
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

	// Token: 0x04002722 RID: 10018
	public SpawnFilter Filter;

	// Token: 0x04002723 RID: 10019
	public string ResourceFolder = string.Empty;

	// Token: 0x04002724 RID: 10020
	public NoiseParameters Cluster = new NoiseParameters(2, 0.5f, 1f, 0f);

	// Token: 0x04002725 RID: 10021
	public float ObjectDensity = 100f;
}
