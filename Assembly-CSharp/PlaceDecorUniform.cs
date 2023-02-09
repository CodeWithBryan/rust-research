using System;
using UnityEngine;

// Token: 0x020006AB RID: 1707
public class PlaceDecorUniform : ProceduralComponent
{
	// Token: 0x06003040 RID: 12352 RVA: 0x00126C54 File Offset: 0x00124E54
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
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		for (float num3 = z; num3 < num2; num3 += this.ObjectDistance)
		{
			for (float num4 = x; num4 < num; num4 += this.ObjectDistance)
			{
				float x2 = num4 + SeedRandom.Range(ref seed, -this.ObjectDithering, this.ObjectDithering);
				float z2 = num3 + SeedRandom.Range(ref seed, -this.ObjectDithering, this.ObjectDithering);
				float normX = TerrainMeta.NormalizeX(x2);
				float normZ = TerrainMeta.NormalizeZ(z2);
				float num5 = SeedRandom.Value(ref seed);
				float factor = this.Filter.GetFactor(normX, normZ, true);
				Prefab random = array.GetRandom(ref seed);
				if (factor * factor >= num5)
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
	}

	// Token: 0x0400271E RID: 10014
	public SpawnFilter Filter;

	// Token: 0x0400271F RID: 10015
	public string ResourceFolder = string.Empty;

	// Token: 0x04002720 RID: 10016
	public float ObjectDistance = 10f;

	// Token: 0x04002721 RID: 10017
	public float ObjectDithering = 5f;
}
