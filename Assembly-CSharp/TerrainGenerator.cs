using System;
using UnityEngine;

// Token: 0x020006BD RID: 1725
public class TerrainGenerator : SingletonComponent<TerrainGenerator>
{
	// Token: 0x0600306D RID: 12397 RVA: 0x0012A5CF File Offset: 0x001287CF
	public static int GetHeightMapRes()
	{
		return Mathf.Min(4096, Mathf.ClosestPowerOfTwo((int)(World.Size * 1f))) + 1;
	}

	// Token: 0x0600306E RID: 12398 RVA: 0x0012A5F0 File Offset: 0x001287F0
	public static int GetSplatMapRes()
	{
		return Mathf.Min(2048, Mathf.NextPowerOfTwo((int)(World.Size * 0.5f)));
	}

	// Token: 0x0600306F RID: 12399 RVA: 0x0012A60F File Offset: 0x0012880F
	public static int GetBaseMapRes()
	{
		return Mathf.Min(2048, Mathf.NextPowerOfTwo((int)(World.Size * 0.01f)));
	}

	// Token: 0x06003070 RID: 12400 RVA: 0x0012A62E File Offset: 0x0012882E
	public GameObject CreateTerrain()
	{
		return this.CreateTerrain(TerrainGenerator.GetHeightMapRes(), TerrainGenerator.GetSplatMapRes());
	}

	// Token: 0x06003071 RID: 12401 RVA: 0x0012A640 File Offset: 0x00128840
	public GameObject CreateTerrain(int heightmapResolution, int alphamapResolution)
	{
		Terrain component = Terrain.CreateTerrainGameObject(new TerrainData
		{
			baseMapResolution = TerrainGenerator.GetBaseMapRes(),
			heightmapResolution = heightmapResolution,
			alphamapResolution = alphamapResolution,
			size = new Vector3(World.Size, 1000f, World.Size)
		}).GetComponent<Terrain>();
		component.transform.position = base.transform.position + new Vector3((float)(-(float)((ulong)World.Size)) * 0.5f, 0f, (float)(-(float)((ulong)World.Size)) * 0.5f);
		component.drawInstanced = false;
		component.castShadows = this.config.CastShadows;
		component.materialType = Terrain.MaterialType.Custom;
		component.materialTemplate = this.config.Material;
		component.gameObject.tag = base.gameObject.tag;
		component.gameObject.layer = base.gameObject.layer;
		component.gameObject.GetComponent<TerrainCollider>().sharedMaterial = this.config.GenericMaterial;
		TerrainMeta terrainMeta = component.gameObject.AddComponent<TerrainMeta>();
		component.gameObject.AddComponent<TerrainPhysics>();
		component.gameObject.AddComponent<TerrainColors>();
		component.gameObject.AddComponent<TerrainCollision>();
		component.gameObject.AddComponent<TerrainBiomeMap>();
		component.gameObject.AddComponent<TerrainAlphaMap>();
		component.gameObject.AddComponent<TerrainHeightMap>();
		component.gameObject.AddComponent<TerrainSplatMap>();
		component.gameObject.AddComponent<TerrainTopologyMap>();
		component.gameObject.AddComponent<TerrainWaterMap>();
		component.gameObject.AddComponent<TerrainPlacementMap>();
		component.gameObject.AddComponent<TerrainPath>();
		component.gameObject.AddComponent<TerrainTexturing>();
		terrainMeta.terrain = component;
		terrainMeta.config = this.config;
		UnityEngine.Object.DestroyImmediate(base.gameObject);
		return component.gameObject;
	}

	// Token: 0x04002767 RID: 10087
	public TerrainConfig config;

	// Token: 0x04002768 RID: 10088
	private const float HeightMapRes = 1f;

	// Token: 0x04002769 RID: 10089
	private const float SplatMapRes = 0.5f;

	// Token: 0x0400276A RID: 10090
	private const float BaseMapRes = 0.01f;
}
