using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006B0 RID: 1712
public class PlaceMonumentsOffshore : ProceduralComponent
{
	// Token: 0x0600304B RID: 12363 RVA: 0x00127F6C File Offset: 0x0012616C
	public override void Process(uint seed)
	{
		string[] array = (from folder in this.ResourceFolder.Split(new char[]
		{
			','
		})
		select "assets/bundled/prefabs/autospawn/" + folder + "/").ToArray<string>();
		if (World.Networked)
		{
			World.Spawn("Monument", array);
			return;
		}
		if ((ulong)World.Size < (ulong)((long)this.MinWorldSize))
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		List<Prefab<MonumentInfo>> list = new List<Prefab<MonumentInfo>>();
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Prefab<MonumentInfo>[] array3 = Prefab.Load<MonumentInfo>(array2[i], null, null, true);
			array3.Shuffle(ref seed);
			list.AddRange(array3);
		}
		Prefab<MonumentInfo>[] array4 = list.ToArray();
		if (array4 == null || array4.Length == 0)
		{
			return;
		}
		array4.BubbleSort<Prefab<MonumentInfo>>();
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float min = position.x - (float)this.MaxDistanceFromTerrain;
		float max = position.x - (float)this.MinDistanceFromTerrain;
		float min2 = position.x + size.x + (float)this.MinDistanceFromTerrain;
		float max2 = position.x + size.x + (float)this.MaxDistanceFromTerrain;
		float num = position.z - (float)this.MaxDistanceFromTerrain;
		int minDistanceFromTerrain = this.MinDistanceFromTerrain;
		float min3 = position.z + size.z + (float)this.MinDistanceFromTerrain;
		float max3 = position.z + size.z + (float)this.MaxDistanceFromTerrain;
		List<PlaceMonumentsOffshore.SpawnInfo> list2 = new List<PlaceMonumentsOffshore.SpawnInfo>();
		int num2 = 0;
		List<PlaceMonumentsOffshore.SpawnInfo> list3 = new List<PlaceMonumentsOffshore.SpawnInfo>();
		for (int j = 0; j < 10; j++)
		{
			int num3 = 0;
			list2.Clear();
			foreach (Prefab<MonumentInfo> prefab in array4)
			{
				int num4 = (int)(prefab.Parameters ? (prefab.Parameters.Priority + 1) : PrefabPriority.Low);
				int num5 = num4 * num4 * num4 * num4;
				for (int k = 0; k < 10000; k++)
				{
					float x = 0f;
					float z = 0f;
					switch (seed % 4U)
					{
					case 0U:
						x = SeedRandom.Range(ref seed, min, max);
						z = SeedRandom.Range(ref seed, num, max3);
						break;
					case 1U:
						x = SeedRandom.Range(ref seed, min2, max2);
						z = SeedRandom.Range(ref seed, num, max3);
						break;
					case 2U:
						x = SeedRandom.Range(ref seed, min, max2);
						z = SeedRandom.Range(ref seed, num, num);
						break;
					case 3U:
						x = SeedRandom.Range(ref seed, min, max2);
						z = SeedRandom.Range(ref seed, min3, max3);
						break;
					}
					float normX = TerrainMeta.NormalizeX(x);
					float normZ = TerrainMeta.NormalizeZ(z);
					float height = heightMap.GetHeight(normX, normZ);
					Vector3 vector = new Vector3(x, height, z);
					Quaternion localRotation = prefab.Object.transform.localRotation;
					Vector3 localScale = prefab.Object.transform.localScale;
					if (!this.CheckRadius(list2, vector, (float)this.DistanceBetweenMonuments))
					{
						prefab.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
						if ((!prefab.Component || prefab.Component.CheckPlacement(vector, localRotation, localScale)) && !prefab.CheckEnvironmentVolumes(vector, localRotation, localScale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
						{
							PlaceMonumentsOffshore.SpawnInfo item = new PlaceMonumentsOffshore.SpawnInfo
							{
								prefab = prefab,
								position = vector,
								rotation = localRotation,
								scale = localScale
							};
							list2.Add(item);
							num3 += num5;
							break;
						}
					}
				}
				if (this.TargetCount > 0 && list2.Count >= this.TargetCount)
				{
					break;
				}
			}
			if (num3 > num2)
			{
				num2 = num3;
				GenericsUtil.Swap<List<PlaceMonumentsOffshore.SpawnInfo>>(ref list2, ref list3);
			}
		}
		foreach (PlaceMonumentsOffshore.SpawnInfo spawnInfo in list3)
		{
			World.AddPrefab("Monument", spawnInfo.prefab, spawnInfo.position, spawnInfo.rotation, spawnInfo.scale);
		}
	}

	// Token: 0x0600304C RID: 12364 RVA: 0x00128380 File Offset: 0x00126580
	private bool CheckRadius(List<PlaceMonumentsOffshore.SpawnInfo> spawns, Vector3 pos, float radius)
	{
		float num = radius * radius;
		using (List<PlaceMonumentsOffshore.SpawnInfo>.Enumerator enumerator = spawns.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if ((enumerator.Current.position - pos).sqrMagnitude < num)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04002738 RID: 10040
	public string ResourceFolder = string.Empty;

	// Token: 0x04002739 RID: 10041
	public int TargetCount;

	// Token: 0x0400273A RID: 10042
	public int MinDistanceFromTerrain = 100;

	// Token: 0x0400273B RID: 10043
	public int MaxDistanceFromTerrain = 500;

	// Token: 0x0400273C RID: 10044
	public int DistanceBetweenMonuments = 500;

	// Token: 0x0400273D RID: 10045
	[FormerlySerializedAs("MinSize")]
	public int MinWorldSize;

	// Token: 0x0400273E RID: 10046
	private const int Candidates = 10;

	// Token: 0x0400273F RID: 10047
	private const int Attempts = 10000;

	// Token: 0x02000DB8 RID: 3512
	private struct SpawnInfo
	{
		// Token: 0x0400475B RID: 18267
		public Prefab prefab;

		// Token: 0x0400475C RID: 18268
		public Vector3 position;

		// Token: 0x0400475D RID: 18269
		public Quaternion rotation;

		// Token: 0x0400475E RID: 18270
		public Vector3 scale;
	}
}
