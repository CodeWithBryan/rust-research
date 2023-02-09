using System;
using System.Linq;
using UnityEngine;

// Token: 0x020006AA RID: 1706
public class PlaceCliffs : ProceduralComponent
{
	// Token: 0x0600303A RID: 12346 RVA: 0x00126458 File Offset: 0x00124658
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
		Prefab[] array2 = (from prefab in array
		where prefab.Attribute.Find<DecorSocketMale>(prefab.ID) && prefab.Attribute.Find<DecorSocketFemale>(prefab.ID)
		select prefab).ToArray<Prefab>();
		if (array2 == null || array2.Length == 0)
		{
			return;
		}
		Prefab[] array3 = (from prefab in array
		where prefab.Attribute.Find<DecorSocketMale>(prefab.ID)
		select prefab).ToArray<Prefab>();
		if (array3 == null || array3.Length == 0)
		{
			return;
		}
		Prefab[] array4 = (from prefab in array
		where prefab.Attribute.Find<DecorSocketFemale>(prefab.ID)
		select prefab).ToArray<Prefab>();
		if (array4 == null || array4.Length == 0)
		{
			return;
		}
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float max = position.x + size.x;
		float max2 = position.z + size.z;
		int num = Mathf.RoundToInt(size.x * size.z * 0.001f * (float)this.RetryMultiplier);
		for (int i = 0; i < num; i++)
		{
			float x2 = SeedRandom.Range(ref seed, x, max);
			float z2 = SeedRandom.Range(ref seed, z, max2);
			float normX = TerrainMeta.NormalizeX(x2);
			float normZ = TerrainMeta.NormalizeZ(z2);
			float num2 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(normX, normZ, true);
			Prefab random = array2.GetRandom(ref seed);
			if (factor * factor >= num2)
			{
				Vector3 normal = TerrainMeta.HeightMap.GetNormal(normX, normZ);
				if (Vector3.Angle(Vector3.up, normal) >= (float)this.CutoffSlope)
				{
					float height = heightMap.GetHeight(normX, normZ);
					Vector3 vector = new Vector3(x2, height, z2);
					Quaternion lhs = QuaternionEx.LookRotationForcedUp(normal, Vector3.up);
					float num3 = Mathf.Max((this.MaxScale - this.MinScale) / (float)PlaceCliffs.max_scale_attempts, PlaceCliffs.min_scale_delta);
					for (float num4 = this.MaxScale; num4 >= this.MinScale; num4 -= num3)
					{
						Vector3 vector2 = vector;
						Quaternion quaternion = lhs * random.Object.transform.localRotation;
						Vector3 vector3 = num4 * random.Object.transform.localScale;
						if (random.ApplyTerrainAnchors(ref vector2, quaternion, vector3, null) && random.ApplyTerrainChecks(vector2, quaternion, vector3, null) && random.ApplyTerrainFilters(vector2, quaternion, vector3, null) && random.ApplyWaterChecks(vector2, quaternion, vector3))
						{
							PlaceCliffs.CliffPlacement cliffPlacement = this.PlaceMale(array3, ref seed, random, vector2, quaternion, vector3);
							PlaceCliffs.CliffPlacement cliffPlacement2 = this.PlaceFemale(array4, ref seed, random, vector2, quaternion, vector3);
							World.AddPrefab("Decor", random, vector2, quaternion, vector3);
							while (cliffPlacement != null)
							{
								if (cliffPlacement.prefab == null)
								{
									break;
								}
								World.AddPrefab("Decor", cliffPlacement.prefab, cliffPlacement.pos, cliffPlacement.rot, cliffPlacement.scale);
								cliffPlacement = cliffPlacement.next;
								i++;
							}
							while (cliffPlacement2 != null)
							{
								if (cliffPlacement2.prefab == null)
								{
									break;
								}
								World.AddPrefab("Decor", cliffPlacement2.prefab, cliffPlacement2.pos, cliffPlacement2.rot, cliffPlacement2.scale);
								cliffPlacement2 = cliffPlacement2.next;
								i++;
							}
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600303B RID: 12347 RVA: 0x001267F4 File Offset: 0x001249F4
	private PlaceCliffs.CliffPlacement PlaceMale(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale)
	{
		return this.Place<DecorSocketFemale, DecorSocketMale>(prefabs, ref seed, parentPrefab, parentPos, parentRot, parentScale, 0, 0, 0);
	}

	// Token: 0x0600303C RID: 12348 RVA: 0x00126814 File Offset: 0x00124A14
	private PlaceCliffs.CliffPlacement PlaceFemale(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale)
	{
		return this.Place<DecorSocketMale, DecorSocketFemale>(prefabs, ref seed, parentPrefab, parentPos, parentRot, parentScale, 0, 0, 0);
	}

	// Token: 0x0600303D RID: 12349 RVA: 0x00126834 File Offset: 0x00124A34
	private PlaceCliffs.CliffPlacement Place<ParentSocketType, ChildSocketType>(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale, int parentAngle = 0, int parentCount = 0, int parentScore = 0) where ParentSocketType : PrefabAttribute where ChildSocketType : PrefabAttribute
	{
		PlaceCliffs.CliffPlacement cliffPlacement = null;
		if (parentAngle > 160 || parentAngle < -160)
		{
			return cliffPlacement;
		}
		int num = SeedRandom.Range(ref seed, 0, prefabs.Length);
		ParentSocketType parentSocketType = parentPrefab.Attribute.Find<ParentSocketType>(parentPrefab.ID);
		Vector3 a = parentPos + parentRot * Vector3.Scale(parentSocketType.worldPosition, parentScale);
		float num2 = Mathf.Max((this.MaxScale - this.MinScale) / (float)PlaceCliffs.max_scale_attempts, PlaceCliffs.min_scale_delta);
		for (int i = 0; i < prefabs.Length; i++)
		{
			Prefab prefab = prefabs[(num + i) % prefabs.Length];
			if (prefab != parentPrefab)
			{
				ParentSocketType parentSocketType2 = prefab.Attribute.Find<ParentSocketType>(prefab.ID);
				ChildSocketType childSocketType = prefab.Attribute.Find<ChildSocketType>(prefab.ID);
				bool flag = parentSocketType2 != null;
				if (cliffPlacement == null || cliffPlacement.count <= PlaceCliffs.target_count || cliffPlacement.score <= PlaceCliffs.target_length || !flag)
				{
					for (float num3 = this.MaxScale; num3 >= this.MinScale; num3 -= num2)
					{
						for (int j = PlaceCliffs.min_rotation; j <= PlaceCliffs.max_rotation; j += PlaceCliffs.rotation_delta)
						{
							for (int k = -1; k <= 1; k += 2)
							{
								Vector3[] array = PlaceCliffs.offsets;
								int l = 0;
								while (l < array.Length)
								{
									Vector3 b = array[l];
									Vector3 vector = parentScale * num3;
									Quaternion quaternion = Quaternion.Euler(0f, (float)(k * j), 0f) * parentRot;
									Vector3 vector2 = a - quaternion * (Vector3.Scale(childSocketType.worldPosition, vector) + b);
									if (this.Filter.GetFactor(vector2, true) >= 0.5f && prefab.ApplyTerrainAnchors(ref vector2, quaternion, vector, null) && prefab.ApplyTerrainChecks(vector2, quaternion, vector, null) && prefab.ApplyTerrainFilters(vector2, quaternion, vector, null) && prefab.ApplyWaterChecks(vector2, quaternion, vector))
									{
										int parentAngle2 = parentAngle + j;
										int num4 = parentCount + 1;
										int num5 = parentScore + Mathf.CeilToInt(Vector3Ex.Distance2D(parentPos, vector2));
										PlaceCliffs.CliffPlacement cliffPlacement2 = null;
										if (flag)
										{
											cliffPlacement2 = this.Place<ParentSocketType, ChildSocketType>(prefabs, ref seed, prefab, vector2, quaternion, vector, parentAngle2, num4, num5);
											if (cliffPlacement2 != null)
											{
												num4 = cliffPlacement2.count;
												num5 = cliffPlacement2.score;
											}
										}
										else
										{
											num5 *= 2;
										}
										if (cliffPlacement == null)
										{
											cliffPlacement = new PlaceCliffs.CliffPlacement();
										}
										if (cliffPlacement.score < num5)
										{
											cliffPlacement.next = cliffPlacement2;
											cliffPlacement.count = num4;
											cliffPlacement.score = num5;
											cliffPlacement.prefab = prefab;
											cliffPlacement.pos = vector2;
											cliffPlacement.rot = quaternion;
											cliffPlacement.scale = vector;
											goto IL_2D3;
										}
										goto IL_2D3;
									}
									else
									{
										l++;
									}
								}
							}
						}
					}
				}
			}
			IL_2D3:;
		}
		return cliffPlacement;
	}

	// Token: 0x0400270D RID: 9997
	public SpawnFilter Filter;

	// Token: 0x0400270E RID: 9998
	public string ResourceFolder = string.Empty;

	// Token: 0x0400270F RID: 9999
	public int RetryMultiplier = 1;

	// Token: 0x04002710 RID: 10000
	public int CutoffSlope = 10;

	// Token: 0x04002711 RID: 10001
	public float MinScale = 1f;

	// Token: 0x04002712 RID: 10002
	public float MaxScale = 2f;

	// Token: 0x04002713 RID: 10003
	private static int target_count = 4;

	// Token: 0x04002714 RID: 10004
	private static int target_length = 0;

	// Token: 0x04002715 RID: 10005
	private static float min_scale_delta = 0.1f;

	// Token: 0x04002716 RID: 10006
	private static int max_scale_attempts = 10;

	// Token: 0x04002717 RID: 10007
	private static int min_rotation = PlaceCliffs.rotation_delta;

	// Token: 0x04002718 RID: 10008
	private static int max_rotation = 60;

	// Token: 0x04002719 RID: 10009
	private static int rotation_delta = 10;

	// Token: 0x0400271A RID: 10010
	private static float offset_c = 0f;

	// Token: 0x0400271B RID: 10011
	private static float offset_l = -0.75f;

	// Token: 0x0400271C RID: 10012
	private static float offset_r = 0.75f;

	// Token: 0x0400271D RID: 10013
	private static Vector3[] offsets = new Vector3[]
	{
		new Vector3(PlaceCliffs.offset_c, PlaceCliffs.offset_c, PlaceCliffs.offset_c),
		new Vector3(PlaceCliffs.offset_l, PlaceCliffs.offset_c, PlaceCliffs.offset_c),
		new Vector3(PlaceCliffs.offset_r, PlaceCliffs.offset_c, PlaceCliffs.offset_c),
		new Vector3(PlaceCliffs.offset_c, PlaceCliffs.offset_c, PlaceCliffs.offset_l),
		new Vector3(PlaceCliffs.offset_c, PlaceCliffs.offset_c, PlaceCliffs.offset_r)
	};

	// Token: 0x02000DB1 RID: 3505
	private class CliffPlacement
	{
		// Token: 0x0400473A RID: 18234
		public int count;

		// Token: 0x0400473B RID: 18235
		public int score;

		// Token: 0x0400473C RID: 18236
		public Prefab prefab;

		// Token: 0x0400473D RID: 18237
		public Vector3 pos = Vector3.zero;

		// Token: 0x0400473E RID: 18238
		public Quaternion rot = Quaternion.identity;

		// Token: 0x0400473F RID: 18239
		public Vector3 scale = Vector3.one;

		// Token: 0x04004740 RID: 18240
		public PlaceCliffs.CliffPlacement next;
	}
}
