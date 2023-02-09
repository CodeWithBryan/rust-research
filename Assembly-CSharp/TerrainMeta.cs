using System;
using UnityEngine;

// Token: 0x02000678 RID: 1656
[ExecuteInEditMode]
public class TerrainMeta : MonoBehaviour
{
	// Token: 0x17000390 RID: 912
	// (get) Token: 0x06002F44 RID: 12100 RVA: 0x0011AE6D File Offset: 0x0011906D
	// (set) Token: 0x06002F45 RID: 12101 RVA: 0x0011AE74 File Offset: 0x00119074
	public static TerrainConfig Config { get; private set; }

	// Token: 0x17000391 RID: 913
	// (get) Token: 0x06002F46 RID: 12102 RVA: 0x0011AE7C File Offset: 0x0011907C
	// (set) Token: 0x06002F47 RID: 12103 RVA: 0x0011AE83 File Offset: 0x00119083
	public static Terrain Terrain { get; private set; }

	// Token: 0x17000392 RID: 914
	// (get) Token: 0x06002F48 RID: 12104 RVA: 0x0011AE8B File Offset: 0x0011908B
	// (set) Token: 0x06002F49 RID: 12105 RVA: 0x0011AE92 File Offset: 0x00119092
	public static Transform Transform { get; private set; }

	// Token: 0x17000393 RID: 915
	// (get) Token: 0x06002F4A RID: 12106 RVA: 0x0011AE9A File Offset: 0x0011909A
	// (set) Token: 0x06002F4B RID: 12107 RVA: 0x0011AEA1 File Offset: 0x001190A1
	public static Vector3 Position { get; private set; }

	// Token: 0x17000394 RID: 916
	// (get) Token: 0x06002F4C RID: 12108 RVA: 0x0011AEA9 File Offset: 0x001190A9
	// (set) Token: 0x06002F4D RID: 12109 RVA: 0x0011AEB0 File Offset: 0x001190B0
	public static Vector3 Size { get; private set; }

	// Token: 0x17000395 RID: 917
	// (get) Token: 0x06002F4E RID: 12110 RVA: 0x0011AEB8 File Offset: 0x001190B8
	public static Vector3 Center
	{
		get
		{
			return TerrainMeta.Position + TerrainMeta.Size * 0.5f;
		}
	}

	// Token: 0x17000396 RID: 918
	// (get) Token: 0x06002F4F RID: 12111 RVA: 0x0011AED3 File Offset: 0x001190D3
	// (set) Token: 0x06002F50 RID: 12112 RVA: 0x0011AEDA File Offset: 0x001190DA
	public static Vector3 OneOverSize { get; private set; }

	// Token: 0x17000397 RID: 919
	// (get) Token: 0x06002F51 RID: 12113 RVA: 0x0011AEE2 File Offset: 0x001190E2
	// (set) Token: 0x06002F52 RID: 12114 RVA: 0x0011AEE9 File Offset: 0x001190E9
	public static Vector3 HighestPoint { get; set; }

	// Token: 0x17000398 RID: 920
	// (get) Token: 0x06002F53 RID: 12115 RVA: 0x0011AEF1 File Offset: 0x001190F1
	// (set) Token: 0x06002F54 RID: 12116 RVA: 0x0011AEF8 File Offset: 0x001190F8
	public static Vector3 LowestPoint { get; set; }

	// Token: 0x17000399 RID: 921
	// (get) Token: 0x06002F55 RID: 12117 RVA: 0x0011AF00 File Offset: 0x00119100
	// (set) Token: 0x06002F56 RID: 12118 RVA: 0x0011AF07 File Offset: 0x00119107
	public static float LootAxisAngle { get; private set; }

	// Token: 0x1700039A RID: 922
	// (get) Token: 0x06002F57 RID: 12119 RVA: 0x0011AF0F File Offset: 0x0011910F
	// (set) Token: 0x06002F58 RID: 12120 RVA: 0x0011AF16 File Offset: 0x00119116
	public static float BiomeAxisAngle { get; private set; }

	// Token: 0x1700039B RID: 923
	// (get) Token: 0x06002F59 RID: 12121 RVA: 0x0011AF1E File Offset: 0x0011911E
	// (set) Token: 0x06002F5A RID: 12122 RVA: 0x0011AF25 File Offset: 0x00119125
	public static TerrainData Data { get; private set; }

	// Token: 0x1700039C RID: 924
	// (get) Token: 0x06002F5B RID: 12123 RVA: 0x0011AF2D File Offset: 0x0011912D
	// (set) Token: 0x06002F5C RID: 12124 RVA: 0x0011AF34 File Offset: 0x00119134
	public static TerrainCollider Collider { get; private set; }

	// Token: 0x1700039D RID: 925
	// (get) Token: 0x06002F5D RID: 12125 RVA: 0x0011AF3C File Offset: 0x0011913C
	// (set) Token: 0x06002F5E RID: 12126 RVA: 0x0011AF43 File Offset: 0x00119143
	public static TerrainCollision Collision { get; private set; }

	// Token: 0x1700039E RID: 926
	// (get) Token: 0x06002F5F RID: 12127 RVA: 0x0011AF4B File Offset: 0x0011914B
	// (set) Token: 0x06002F60 RID: 12128 RVA: 0x0011AF52 File Offset: 0x00119152
	public static TerrainPhysics Physics { get; private set; }

	// Token: 0x1700039F RID: 927
	// (get) Token: 0x06002F61 RID: 12129 RVA: 0x0011AF5A File Offset: 0x0011915A
	// (set) Token: 0x06002F62 RID: 12130 RVA: 0x0011AF61 File Offset: 0x00119161
	public static TerrainColors Colors { get; private set; }

	// Token: 0x170003A0 RID: 928
	// (get) Token: 0x06002F63 RID: 12131 RVA: 0x0011AF69 File Offset: 0x00119169
	// (set) Token: 0x06002F64 RID: 12132 RVA: 0x0011AF70 File Offset: 0x00119170
	public static TerrainQuality Quality { get; private set; }

	// Token: 0x170003A1 RID: 929
	// (get) Token: 0x06002F65 RID: 12133 RVA: 0x0011AF78 File Offset: 0x00119178
	// (set) Token: 0x06002F66 RID: 12134 RVA: 0x0011AF7F File Offset: 0x0011917F
	public static TerrainPath Path { get; private set; }

	// Token: 0x170003A2 RID: 930
	// (get) Token: 0x06002F67 RID: 12135 RVA: 0x0011AF87 File Offset: 0x00119187
	// (set) Token: 0x06002F68 RID: 12136 RVA: 0x0011AF8E File Offset: 0x0011918E
	public static TerrainBiomeMap BiomeMap { get; private set; }

	// Token: 0x170003A3 RID: 931
	// (get) Token: 0x06002F69 RID: 12137 RVA: 0x0011AF96 File Offset: 0x00119196
	// (set) Token: 0x06002F6A RID: 12138 RVA: 0x0011AF9D File Offset: 0x0011919D
	public static TerrainAlphaMap AlphaMap { get; private set; }

	// Token: 0x170003A4 RID: 932
	// (get) Token: 0x06002F6B RID: 12139 RVA: 0x0011AFA5 File Offset: 0x001191A5
	// (set) Token: 0x06002F6C RID: 12140 RVA: 0x0011AFAC File Offset: 0x001191AC
	public static TerrainBlendMap BlendMap { get; private set; }

	// Token: 0x170003A5 RID: 933
	// (get) Token: 0x06002F6D RID: 12141 RVA: 0x0011AFB4 File Offset: 0x001191B4
	// (set) Token: 0x06002F6E RID: 12142 RVA: 0x0011AFBB File Offset: 0x001191BB
	public static TerrainHeightMap HeightMap { get; private set; }

	// Token: 0x170003A6 RID: 934
	// (get) Token: 0x06002F6F RID: 12143 RVA: 0x0011AFC3 File Offset: 0x001191C3
	// (set) Token: 0x06002F70 RID: 12144 RVA: 0x0011AFCA File Offset: 0x001191CA
	public static TerrainSplatMap SplatMap { get; private set; }

	// Token: 0x170003A7 RID: 935
	// (get) Token: 0x06002F71 RID: 12145 RVA: 0x0011AFD2 File Offset: 0x001191D2
	// (set) Token: 0x06002F72 RID: 12146 RVA: 0x0011AFD9 File Offset: 0x001191D9
	public static TerrainTopologyMap TopologyMap { get; private set; }

	// Token: 0x170003A8 RID: 936
	// (get) Token: 0x06002F73 RID: 12147 RVA: 0x0011AFE1 File Offset: 0x001191E1
	// (set) Token: 0x06002F74 RID: 12148 RVA: 0x0011AFE8 File Offset: 0x001191E8
	public static TerrainWaterMap WaterMap { get; private set; }

	// Token: 0x170003A9 RID: 937
	// (get) Token: 0x06002F75 RID: 12149 RVA: 0x0011AFF0 File Offset: 0x001191F0
	// (set) Token: 0x06002F76 RID: 12150 RVA: 0x0011AFF7 File Offset: 0x001191F7
	public static TerrainDistanceMap DistanceMap { get; private set; }

	// Token: 0x170003AA RID: 938
	// (get) Token: 0x06002F77 RID: 12151 RVA: 0x0011AFFF File Offset: 0x001191FF
	// (set) Token: 0x06002F78 RID: 12152 RVA: 0x0011B006 File Offset: 0x00119206
	public static TerrainPlacementMap PlacementMap { get; private set; }

	// Token: 0x170003AB RID: 939
	// (get) Token: 0x06002F79 RID: 12153 RVA: 0x0011B00E File Offset: 0x0011920E
	// (set) Token: 0x06002F7A RID: 12154 RVA: 0x0011B015 File Offset: 0x00119215
	public static TerrainTexturing Texturing { get; private set; }

	// Token: 0x06002F7B RID: 12155 RVA: 0x0011B020 File Offset: 0x00119220
	public static bool OutOfBounds(Vector3 worldPos)
	{
		return worldPos.x < TerrainMeta.Position.x || worldPos.z < TerrainMeta.Position.z || worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x || worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z;
	}

	// Token: 0x06002F7C RID: 12156 RVA: 0x0011B094 File Offset: 0x00119294
	public static bool OutOfMargin(Vector3 worldPos)
	{
		return worldPos.x < TerrainMeta.Position.x - TerrainMeta.Size.x || worldPos.z < TerrainMeta.Position.z - TerrainMeta.Size.z || worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x + TerrainMeta.Size.x || worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z + TerrainMeta.Size.z;
	}

	// Token: 0x06002F7D RID: 12157 RVA: 0x0011B134 File Offset: 0x00119334
	public static Vector3 RandomPointOffshore()
	{
		float num = UnityEngine.Random.Range(-1f, 1f);
		float num2 = UnityEngine.Random.Range(0f, 100f);
		Vector3 vector = new Vector3(Mathf.Min(TerrainMeta.Size.x, 4000f) - 100f, 0f, Mathf.Min(TerrainMeta.Size.z, 4000f) - 100f);
		if (num2 < 25f)
		{
			return TerrainMeta.Center + new Vector3(-vector.x, 0f, num * vector.z);
		}
		if (num2 < 50f)
		{
			return TerrainMeta.Center + new Vector3(vector.x, 0f, num * vector.z);
		}
		if (num2 < 75f)
		{
			return TerrainMeta.Center + new Vector3(num * vector.x, 0f, -vector.z);
		}
		return TerrainMeta.Center + new Vector3(num * vector.x, 0f, vector.z);
	}

	// Token: 0x06002F7E RID: 12158 RVA: 0x0011B248 File Offset: 0x00119448
	public static Vector3 Normalize(Vector3 worldPos)
	{
		float x = (worldPos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		float y = (worldPos.y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y;
		float z = (worldPos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return new Vector3(x, y, z);
	}

	// Token: 0x06002F7F RID: 12159 RVA: 0x0011B2B2 File Offset: 0x001194B2
	public static float NormalizeX(float x)
	{
		return (x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
	}

	// Token: 0x06002F80 RID: 12160 RVA: 0x0011B2CB File Offset: 0x001194CB
	public static float NormalizeY(float y)
	{
		return (y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y;
	}

	// Token: 0x06002F81 RID: 12161 RVA: 0x0011B2E4 File Offset: 0x001194E4
	public static float NormalizeZ(float z)
	{
		return (z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
	}

	// Token: 0x06002F82 RID: 12162 RVA: 0x0011B300 File Offset: 0x00119500
	public static Vector3 Denormalize(Vector3 normPos)
	{
		float x = TerrainMeta.Position.x + normPos.x * TerrainMeta.Size.x;
		float y = TerrainMeta.Position.y + normPos.y * TerrainMeta.Size.y;
		float z = TerrainMeta.Position.z + normPos.z * TerrainMeta.Size.z;
		return new Vector3(x, y, z);
	}

	// Token: 0x06002F83 RID: 12163 RVA: 0x0011B36A File Offset: 0x0011956A
	public static float DenormalizeX(float normX)
	{
		return TerrainMeta.Position.x + normX * TerrainMeta.Size.x;
	}

	// Token: 0x06002F84 RID: 12164 RVA: 0x0011B383 File Offset: 0x00119583
	public static float DenormalizeY(float normY)
	{
		return TerrainMeta.Position.y + normY * TerrainMeta.Size.y;
	}

	// Token: 0x06002F85 RID: 12165 RVA: 0x0011B39C File Offset: 0x0011959C
	public static float DenormalizeZ(float normZ)
	{
		return TerrainMeta.Position.z + normZ * TerrainMeta.Size.z;
	}

	// Token: 0x06002F86 RID: 12166 RVA: 0x0011B3B5 File Offset: 0x001195B5
	protected void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Shader.DisableKeyword("TERRAIN_PAINTING");
	}

	// Token: 0x06002F87 RID: 12167 RVA: 0x0011B3CC File Offset: 0x001195CC
	public void Init(Terrain terrainOverride = null, TerrainConfig configOverride = null)
	{
		if (terrainOverride != null)
		{
			this.terrain = terrainOverride;
		}
		if (configOverride != null)
		{
			this.config = configOverride;
		}
		TerrainMeta.Terrain = this.terrain;
		TerrainMeta.Config = this.config;
		TerrainMeta.Transform = this.terrain.transform;
		TerrainMeta.Data = this.terrain.terrainData;
		TerrainMeta.Size = this.terrain.terrainData.size;
		TerrainMeta.OneOverSize = TerrainMeta.Size.Inverse();
		TerrainMeta.Position = this.terrain.GetPosition();
		TerrainMeta.Collider = this.terrain.GetComponent<TerrainCollider>();
		TerrainMeta.Collision = this.terrain.GetComponent<TerrainCollision>();
		TerrainMeta.Physics = this.terrain.GetComponent<TerrainPhysics>();
		TerrainMeta.Colors = this.terrain.GetComponent<TerrainColors>();
		TerrainMeta.Quality = this.terrain.GetComponent<TerrainQuality>();
		TerrainMeta.Path = this.terrain.GetComponent<TerrainPath>();
		TerrainMeta.BiomeMap = this.terrain.GetComponent<TerrainBiomeMap>();
		TerrainMeta.AlphaMap = this.terrain.GetComponent<TerrainAlphaMap>();
		TerrainMeta.BlendMap = this.terrain.GetComponent<TerrainBlendMap>();
		TerrainMeta.HeightMap = this.terrain.GetComponent<TerrainHeightMap>();
		TerrainMeta.SplatMap = this.terrain.GetComponent<TerrainSplatMap>();
		TerrainMeta.TopologyMap = this.terrain.GetComponent<TerrainTopologyMap>();
		TerrainMeta.WaterMap = this.terrain.GetComponent<TerrainWaterMap>();
		TerrainMeta.DistanceMap = this.terrain.GetComponent<TerrainDistanceMap>();
		TerrainMeta.PlacementMap = this.terrain.GetComponent<TerrainPlacementMap>();
		TerrainMeta.Texturing = this.terrain.GetComponent<TerrainTexturing>();
		this.terrain.drawInstanced = false;
		TerrainMeta.HighestPoint = new Vector3(TerrainMeta.Position.x, TerrainMeta.Position.y + TerrainMeta.Size.y, TerrainMeta.Position.z);
		TerrainMeta.LowestPoint = new Vector3(TerrainMeta.Position.x, TerrainMeta.Position.y, TerrainMeta.Position.z);
		TerrainExtension[] components = base.GetComponents<TerrainExtension>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Init(this.terrain, this.config);
		}
		uint seed = World.Seed;
		int num = SeedRandom.Range(ref seed, 0, 4) * 90;
		int num2 = SeedRandom.Range(ref seed, -45, 46);
		int num3 = SeedRandom.Sign(ref seed);
		TerrainMeta.LootAxisAngle = (float)num;
		TerrainMeta.BiomeAxisAngle = (float)(num + num2 + num3 * 90);
	}

	// Token: 0x06002F88 RID: 12168 RVA: 0x0011B632 File Offset: 0x00119832
	public static void InitNoTerrain(bool createPath = false)
	{
		TerrainMeta.Size = new Vector3(4096f, 4096f, 4096f);
		TerrainMeta.OneOverSize = TerrainMeta.Size.Inverse();
		TerrainMeta.Position = -0.5f * TerrainMeta.Size;
	}

	// Token: 0x06002F89 RID: 12169 RVA: 0x0011B670 File Offset: 0x00119870
	public void SetupComponents()
	{
		foreach (TerrainExtension terrainExtension in base.GetComponents<TerrainExtension>())
		{
			terrainExtension.Setup();
			terrainExtension.isInitialized = true;
		}
	}

	// Token: 0x06002F8A RID: 12170 RVA: 0x0011B6A4 File Offset: 0x001198A4
	public void PostSetupComponents()
	{
		TerrainExtension[] components = base.GetComponents<TerrainExtension>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].PostSetup();
		}
	}

	// Token: 0x06002F8B RID: 12171 RVA: 0x0011B6D0 File Offset: 0x001198D0
	public void BindShaderProperties()
	{
		if (this.config)
		{
			Shader.SetGlobalTexture("Terrain_AlbedoArray", this.config.AlbedoArray);
			Shader.SetGlobalTexture("Terrain_NormalArray", this.config.NormalArray);
			Shader.SetGlobalVector("Terrain_TexelSize", new Vector2(1f / this.config.GetMinSplatTiling(), 1f / this.config.GetMinSplatTiling()));
			Shader.SetGlobalVector("Terrain_TexelSize0", new Vector4(1f / this.config.Splats[0].SplatTiling, 1f / this.config.Splats[1].SplatTiling, 1f / this.config.Splats[2].SplatTiling, 1f / this.config.Splats[3].SplatTiling));
			Shader.SetGlobalVector("Terrain_TexelSize1", new Vector4(1f / this.config.Splats[4].SplatTiling, 1f / this.config.Splats[5].SplatTiling, 1f / this.config.Splats[6].SplatTiling, 1f / this.config.Splats[7].SplatTiling));
			Shader.SetGlobalVector("Splat0_UVMIX", new Vector3(this.config.Splats[0].UVMIXMult, this.config.Splats[0].UVMIXStart, 1f / this.config.Splats[0].UVMIXDist));
			Shader.SetGlobalVector("Splat1_UVMIX", new Vector3(this.config.Splats[1].UVMIXMult, this.config.Splats[1].UVMIXStart, 1f / this.config.Splats[1].UVMIXDist));
			Shader.SetGlobalVector("Splat2_UVMIX", new Vector3(this.config.Splats[2].UVMIXMult, this.config.Splats[2].UVMIXStart, 1f / this.config.Splats[2].UVMIXDist));
			Shader.SetGlobalVector("Splat3_UVMIX", new Vector3(this.config.Splats[3].UVMIXMult, this.config.Splats[3].UVMIXStart, 1f / this.config.Splats[3].UVMIXDist));
			Shader.SetGlobalVector("Splat4_UVMIX", new Vector3(this.config.Splats[4].UVMIXMult, this.config.Splats[4].UVMIXStart, 1f / this.config.Splats[4].UVMIXDist));
			Shader.SetGlobalVector("Splat5_UVMIX", new Vector3(this.config.Splats[5].UVMIXMult, this.config.Splats[5].UVMIXStart, 1f / this.config.Splats[5].UVMIXDist));
			Shader.SetGlobalVector("Splat6_UVMIX", new Vector3(this.config.Splats[6].UVMIXMult, this.config.Splats[6].UVMIXStart, 1f / this.config.Splats[6].UVMIXDist));
			Shader.SetGlobalVector("Splat7_UVMIX", new Vector3(this.config.Splats[7].UVMIXMult, this.config.Splats[7].UVMIXStart, 1f / this.config.Splats[7].UVMIXDist));
		}
		if (TerrainMeta.HeightMap)
		{
			Shader.SetGlobalTexture("Terrain_Normal", TerrainMeta.HeightMap.NormalTexture);
		}
		if (TerrainMeta.AlphaMap)
		{
			Shader.SetGlobalTexture("Terrain_Alpha", TerrainMeta.AlphaMap.AlphaTexture);
		}
		if (TerrainMeta.BiomeMap)
		{
			Shader.SetGlobalTexture("Terrain_Biome", TerrainMeta.BiomeMap.BiomeTexture);
		}
		if (TerrainMeta.SplatMap)
		{
			Shader.SetGlobalTexture("Terrain_Control0", TerrainMeta.SplatMap.SplatTexture0);
			Shader.SetGlobalTexture("Terrain_Control1", TerrainMeta.SplatMap.SplatTexture1);
		}
		TerrainMeta.WaterMap;
		if (TerrainMeta.DistanceMap)
		{
			Shader.SetGlobalTexture("Terrain_Distance", TerrainMeta.DistanceMap.DistanceTexture);
		}
		if (this.terrain)
		{
			Shader.SetGlobalVector("Terrain_Position", TerrainMeta.Position);
			Shader.SetGlobalVector("Terrain_Size", TerrainMeta.Size);
			Shader.SetGlobalVector("Terrain_RcpSize", TerrainMeta.OneOverSize);
			if (this.terrain.materialTemplate)
			{
				if (this.terrain.materialTemplate.IsKeywordEnabled("_TERRAIN_BLEND_LINEAR"))
				{
					this.terrain.materialTemplate.DisableKeyword("_TERRAIN_BLEND_LINEAR");
				}
				if (this.terrain.materialTemplate.IsKeywordEnabled("_TERRAIN_VERTEX_NORMALS"))
				{
					this.terrain.materialTemplate.DisableKeyword("_TERRAIN_VERTEX_NORMALS");
				}
			}
		}
	}

	// Token: 0x04002636 RID: 9782
	public Terrain terrain;

	// Token: 0x04002637 RID: 9783
	public TerrainConfig config;

	// Token: 0x04002638 RID: 9784
	public TerrainMeta.PaintMode paint;

	// Token: 0x04002639 RID: 9785
	[HideInInspector]
	public TerrainMeta.PaintMode currentPaintMode;

	// Token: 0x02000D77 RID: 3447
	public enum PaintMode
	{
		// Token: 0x04004690 RID: 18064
		None,
		// Token: 0x04004691 RID: 18065
		Splats,
		// Token: 0x04004692 RID: 18066
		Biomes,
		// Token: 0x04004693 RID: 18067
		Alpha,
		// Token: 0x04004694 RID: 18068
		Blend,
		// Token: 0x04004695 RID: 18069
		Field,
		// Token: 0x04004696 RID: 18070
		Cliff,
		// Token: 0x04004697 RID: 18071
		Summit,
		// Token: 0x04004698 RID: 18072
		Beachside,
		// Token: 0x04004699 RID: 18073
		Beach,
		// Token: 0x0400469A RID: 18074
		Forest,
		// Token: 0x0400469B RID: 18075
		Forestside,
		// Token: 0x0400469C RID: 18076
		Ocean,
		// Token: 0x0400469D RID: 18077
		Oceanside,
		// Token: 0x0400469E RID: 18078
		Decor,
		// Token: 0x0400469F RID: 18079
		Monument,
		// Token: 0x040046A0 RID: 18080
		Road,
		// Token: 0x040046A1 RID: 18081
		Roadside,
		// Token: 0x040046A2 RID: 18082
		Bridge,
		// Token: 0x040046A3 RID: 18083
		River,
		// Token: 0x040046A4 RID: 18084
		Riverside,
		// Token: 0x040046A5 RID: 18085
		Lake,
		// Token: 0x040046A6 RID: 18086
		Lakeside,
		// Token: 0x040046A7 RID: 18087
		Offshore,
		// Token: 0x040046A8 RID: 18088
		Rail,
		// Token: 0x040046A9 RID: 18089
		Railside,
		// Token: 0x040046AA RID: 18090
		Building,
		// Token: 0x040046AB RID: 18091
		Cliffside,
		// Token: 0x040046AC RID: 18092
		Mountain,
		// Token: 0x040046AD RID: 18093
		Clutter,
		// Token: 0x040046AE RID: 18094
		Alt,
		// Token: 0x040046AF RID: 18095
		Tier0,
		// Token: 0x040046B0 RID: 18096
		Tier1,
		// Token: 0x040046B1 RID: 18097
		Tier2,
		// Token: 0x040046B2 RID: 18098
		Mainland,
		// Token: 0x040046B3 RID: 18099
		Hilltop
	}
}
