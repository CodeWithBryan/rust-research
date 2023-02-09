using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020006DD RID: 1757
[ExecuteInEditMode]
public class WaterSystem : MonoBehaviour
{
	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x0600311F RID: 12575 RVA: 0x0012ED19 File Offset: 0x0012CF19
	public WaterGerstner.PrecomputedWave[] PrecomputedWaves
	{
		get
		{
			return this.precomputedWaves;
		}
	}

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x06003120 RID: 12576 RVA: 0x0012ED21 File Offset: 0x0012CF21
	public WaterGerstner.PrecomputedShoreWaves PrecomputedShoreWaves
	{
		get
		{
			return this.precomputedShoreWaves;
		}
	}

	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x06003121 RID: 12577 RVA: 0x0012ED29 File Offset: 0x0012CF29
	public Vector4 Global0
	{
		get
		{
			return this.global0;
		}
	}

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x06003122 RID: 12578 RVA: 0x0012ED31 File Offset: 0x0012CF31
	public Vector4 Global1
	{
		get
		{
			return this.global1;
		}
	}

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x06003123 RID: 12579 RVA: 0x0012ED39 File Offset: 0x0012CF39
	// (set) Token: 0x06003124 RID: 12580 RVA: 0x0012ED41 File Offset: 0x0012CF41
	public float ShoreWavesRcpFadeDistance { get; private set; } = 0.04f;

	// Token: 0x170003CB RID: 971
	// (get) Token: 0x06003125 RID: 12581 RVA: 0x0012ED4A File Offset: 0x0012CF4A
	// (set) Token: 0x06003126 RID: 12582 RVA: 0x0012ED52 File Offset: 0x0012CF52
	public float TerrainRcpFadeDistance { get; private set; } = 0.1f;

	// Token: 0x170003CC RID: 972
	// (get) Token: 0x06003128 RID: 12584 RVA: 0x0012ED64 File Offset: 0x0012CF64
	// (set) Token: 0x06003127 RID: 12583 RVA: 0x0012ED5B File Offset: 0x0012CF5B
	public bool IsInitialized { get; private set; }

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x06003129 RID: 12585 RVA: 0x0012ED6C File Offset: 0x0012CF6C
	// (set) Token: 0x0600312A RID: 12586 RVA: 0x0012ED73 File Offset: 0x0012CF73
	public static WaterCollision Collision { get; private set; }

	// Token: 0x170003CE RID: 974
	// (get) Token: 0x0600312C RID: 12588 RVA: 0x0012ED83 File Offset: 0x0012CF83
	// (set) Token: 0x0600312B RID: 12587 RVA: 0x0012ED7B File Offset: 0x0012CF7B
	public static WaterDynamics Dynamics { get; private set; }

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x0600312E RID: 12590 RVA: 0x0012ED92 File Offset: 0x0012CF92
	// (set) Token: 0x0600312D RID: 12589 RVA: 0x0012ED8A File Offset: 0x0012CF8A
	public static WaterBody Ocean { get; private set; } = null;

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x06003130 RID: 12592 RVA: 0x0012EDA1 File Offset: 0x0012CFA1
	// (set) Token: 0x0600312F RID: 12591 RVA: 0x0012ED99 File Offset: 0x0012CF99
	public static HashSet<WaterBody> WaterBodies { get; private set; } = new HashSet<WaterBody>();

	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x06003131 RID: 12593 RVA: 0x0012EDA8 File Offset: 0x0012CFA8
	// (set) Token: 0x06003132 RID: 12594 RVA: 0x0012EDAF File Offset: 0x0012CFAF
	public static float OceanLevel
	{
		get
		{
			return WaterSystem.oceanLevel;
		}
		set
		{
			value = Mathf.Max(value, 0f);
			if (!Mathf.Approximately(WaterSystem.oceanLevel, value))
			{
				WaterSystem.oceanLevel = value;
				WaterSystem.UpdateOceanLevel();
			}
		}
	}

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x06003133 RID: 12595 RVA: 0x0012EDD9 File Offset: 0x0012CFD9
	// (set) Token: 0x06003134 RID: 12596 RVA: 0x0012EDE0 File Offset: 0x0012CFE0
	public static float WaveTime { get; private set; } = 0f;

	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x06003135 RID: 12597 RVA: 0x0012EDE8 File Offset: 0x0012CFE8
	public static WaterSystem Instance
	{
		get
		{
			return WaterSystem.instance;
		}
	}

	// Token: 0x06003136 RID: 12598 RVA: 0x0012EDF0 File Offset: 0x0012CFF0
	private void CheckInstance()
	{
		WaterSystem.instance = ((WaterSystem.instance != null) ? WaterSystem.instance : this);
		WaterSystem.Collision = ((WaterSystem.Collision != null) ? WaterSystem.Collision : base.GetComponent<WaterCollision>());
		WaterSystem.Dynamics = ((WaterSystem.Dynamics != null) ? WaterSystem.Dynamics : base.GetComponent<WaterDynamics>());
	}

	// Token: 0x06003137 RID: 12599 RVA: 0x0012EE55 File Offset: 0x0012D055
	public void Awake()
	{
		this.CheckInstance();
	}

	// Token: 0x06003138 RID: 12600
	[DllImport("RustNative", EntryPoint = "Water_SetBaseConstants")]
	private static extern void SetBaseConstants_Native(int shoreMapSize, ref Vector3 shoreMap, int waterHeightMapSize, ref short waterHeightMap, Vector4 packedParams);

	// Token: 0x06003139 RID: 12601
	[DllImport("RustNative", EntryPoint = "Water_SetTerrainConstants")]
	private static extern void SetTerrainConstants_Native(int terrainHeightMapSize, ref short terrainHeightMap, Vector3 terrainPosition, Vector3 terrainSize);

	// Token: 0x0600313A RID: 12602
	[DllImport("RustNative", EntryPoint = "Water_SetGerstnerConstants")]
	private static extern void SetGerstnerConstants_Native(Vector4 globalParams0, Vector4 globalParams1, ref Vector4 openWaves, ref Vector4 shoreWaves);

	// Token: 0x0600313B RID: 12603
	[DllImport("RustNative", EntryPoint = "Water_UpdateOceanLevel")]
	private static extern void UpdateOceanLevel_Native(float oceanWaterLevel);

	// Token: 0x0600313C RID: 12604
	[DllImport("RustNative", EntryPoint = "Water_GetHeightArray")]
	private static extern float GetHeightArray_Native(int sampleCount, ref Vector2 pos, ref Vector2 posUV, ref Vector3 shore, ref float terrainHeight, ref float waterHeight);

	// Token: 0x0600313D RID: 12605
	[DllImport("RustNative", EntryPoint = "Water_GetHeight")]
	private static extern float GetHeight_Native(Vector3 pos);

	// Token: 0x0600313E RID: 12606
	[DllImport("RustNative")]
	private static extern bool CPU_SupportsSSE41();

	// Token: 0x0600313F RID: 12607 RVA: 0x0012EE60 File Offset: 0x0012D060
	private static void SetNativeConstants(TerrainTexturing terrainTexturing, TerrainWaterMap terrainWaterMap, TerrainHeightMap terrainHeightMap, Vector4 globalParams0, Vector4 globalParams1, Vector4[] openWaves, Vector4[] shoreWaves)
	{
		if (WaterSystem.nativePathState == WaterSystem.NativePathState.Initializing)
		{
			try
			{
				WaterSystem.nativePathState = ((!WaterSystem.CPU_SupportsSSE41()) ? WaterSystem.NativePathState.Failed : WaterSystem.nativePathState);
			}
			catch (EntryPointNotFoundException)
			{
				WaterSystem.nativePathState = WaterSystem.NativePathState.Failed;
			}
		}
		if (WaterSystem.nativePathState != WaterSystem.NativePathState.Failed)
		{
			try
			{
				int shoreMapSize = 1;
				Vector3[] shoreMap = WaterSystem.emptyShoreMap;
				if (terrainTexturing != null && terrainTexturing.ShoreMap != null)
				{
					shoreMapSize = terrainTexturing.ShoreMapSize;
					shoreMap = terrainTexturing.ShoreMap;
				}
				int waterHeightMapSize = 1;
				short[] src = WaterSystem.emptyWaterMap;
				if (terrainWaterMap != null && terrainWaterMap.src != null && terrainWaterMap.src.Length != 0)
				{
					waterHeightMapSize = terrainWaterMap.res;
					src = terrainWaterMap.src;
				}
				int terrainHeightMapSize = 1;
				short[] src2 = WaterSystem.emptyHeightMap;
				if (terrainHeightMap != null && terrainHeightMap.src != null && terrainHeightMap.src.Length != 0)
				{
					terrainHeightMapSize = terrainHeightMap.res;
					src2 = terrainHeightMap.src;
				}
				Vector4 packedParams;
				packedParams.x = WaterSystem.OceanLevel;
				packedParams.y = ((WaterSystem.instance != null) ? 1f : 0f);
				packedParams.z = ((TerrainTexturing.Instance != null) ? 1f : 0f);
				packedParams.w = 0f;
				WaterSystem.PinObject<Vector3[]>(shoreMap, ref WaterSystem.currentShoreMap, ref WaterSystem.currentShoreMapHandle);
				WaterSystem.PinObject<short[]>(src, ref WaterSystem.currentWaterMap, ref WaterSystem.currentWaterMapHandle);
				WaterSystem.PinObject<short[]>(src2, ref WaterSystem.currentHeightMap, ref WaterSystem.currentHeightMapHandle);
				WaterSystem.PinObject<Vector4[]>(openWaves, ref WaterSystem.currentOpenWaves, ref WaterSystem.currentOpenWavesHandle);
				WaterSystem.PinObject<Vector4[]>(shoreWaves, ref WaterSystem.currentShoreWaves, ref WaterSystem.currentShoreWavesHandle);
				WaterSystem.SetBaseConstants_Native(shoreMapSize, ref shoreMap[0], waterHeightMapSize, ref src[0], packedParams);
				WaterSystem.SetTerrainConstants_Native(terrainHeightMapSize, ref src2[0], TerrainMeta.Position, TerrainMeta.Size);
				WaterSystem.SetGerstnerConstants_Native(globalParams0, globalParams1, ref openWaves[0], ref shoreWaves[0]);
				WaterSystem.nativePathState = WaterSystem.NativePathState.Ready;
			}
			catch (EntryPointNotFoundException)
			{
				WaterSystem.nativePathState = WaterSystem.NativePathState.Failed;
			}
		}
	}

	// Token: 0x06003140 RID: 12608 RVA: 0x0012F064 File Offset: 0x0012D264
	private static void PinObject<T>(T value, ref T currentValue, ref GCHandle currentValueHandle) where T : class
	{
		if (value != null && value != currentValue)
		{
			if (currentValueHandle.IsAllocated)
			{
				currentValueHandle.Free();
			}
			currentValue = value;
			currentValueHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
		}
	}

	// Token: 0x06003141 RID: 12609 RVA: 0x0012F0B4 File Offset: 0x0012D2B4
	private static float GetHeight_Managed(Vector3 pos)
	{
		Vector2 uv;
		uv.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		uv.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		float num = WaterSystem.OceanLevel;
		float num2 = (TerrainMeta.WaterMap != null) ? TerrainMeta.WaterMap.GetHeightFast(uv) : 0f;
		float num3 = (TerrainMeta.HeightMap != null) ? TerrainMeta.HeightMap.GetHeightFast(uv) : 0f;
		if (WaterSystem.instance != null && (double)num2 <= (double)num + 0.01)
		{
			Vector3 shore = (TerrainTexturing.Instance != null) ? TerrainTexturing.Instance.GetCoarseVectorToShore(uv) : Vector3.zero;
			float num4 = Mathf.Clamp01(Mathf.Abs(num - num3) * 0.1f);
			num2 = WaterGerstner.SampleHeight(WaterSystem.instance, pos, shore) * num4;
		}
		return num2;
	}

	// Token: 0x06003142 RID: 12610 RVA: 0x0012F1B4 File Offset: 0x0012D3B4
	public static void GetHeightArray_Managed(Vector2[] pos, Vector2[] posUV, Vector3[] shore, float[] terrainHeight, float[] waterHeight)
	{
		if (TerrainTexturing.Instance != null)
		{
			for (int i = 0; i < posUV.Length; i++)
			{
				shore[i] = TerrainTexturing.Instance.GetCoarseVectorToShore(posUV[i]);
			}
		}
		if (WaterSystem.instance != null)
		{
			WaterGerstner.SampleHeightArray(WaterSystem.instance, pos, shore, waterHeight);
		}
		float num = WaterSystem.OceanLevel;
		for (int j = 0; j < posUV.Length; j++)
		{
			Vector2 uv = posUV[j];
			terrainHeight[j] = ((TerrainMeta.HeightMap != null) ? TerrainMeta.HeightMap.GetHeightFast(uv) : 0f);
			float num2 = (TerrainMeta.WaterMap != null) ? TerrainMeta.WaterMap.GetHeightFast(uv) : 0f;
			if (WaterSystem.instance != null && (double)num2 <= (double)num + 0.01)
			{
				float num3 = Mathf.Clamp01(Mathf.Abs(num - terrainHeight[j]) * 0.1f);
				waterHeight[j] = num + waterHeight[j] * num3;
			}
			else
			{
				waterHeight[j] = num2;
			}
		}
	}

	// Token: 0x06003143 RID: 12611 RVA: 0x0012F2C0 File Offset: 0x0012D4C0
	public static float GetHeight(Vector3 pos)
	{
		float val;
		if (WaterSystem.nativePathState == WaterSystem.NativePathState.Ready)
		{
			val = WaterSystem.GetHeight_Native(pos);
		}
		else
		{
			val = WaterSystem.GetHeight_Managed(pos);
		}
		return Math.Max(val, WaterSystem.OceanLevel);
	}

	// Token: 0x06003144 RID: 12612 RVA: 0x0012F2F0 File Offset: 0x0012D4F0
	public static void GetHeightArray(Vector2[] pos, Vector2[] posUV, Vector3[] shore, float[] terrainHeight, float[] waterHeight)
	{
		Debug.Assert(pos.Length == posUV.Length);
		Debug.Assert(pos.Length == shore.Length);
		Debug.Assert(pos.Length == terrainHeight.Length);
		Debug.Assert(pos.Length == waterHeight.Length);
		if (WaterSystem.nativePathState == WaterSystem.NativePathState.Ready)
		{
			WaterSystem.GetHeightArray_Native(pos.Length, ref pos[0], ref posUV[0], ref shore[0], ref terrainHeight[0], ref waterHeight[0]);
			return;
		}
		WaterSystem.GetHeightArray_Managed(pos, posUV, shore, terrainHeight, waterHeight);
	}

	// Token: 0x06003145 RID: 12613 RVA: 0x0012F374 File Offset: 0x0012D574
	public static Vector3 GetNormal(Vector3 pos)
	{
		return ((TerrainMeta.WaterMap != null) ? TerrainMeta.WaterMap.GetNormal(pos) : Vector3.up).normalized;
	}

	// Token: 0x06003146 RID: 12614 RVA: 0x0012F3A8 File Offset: 0x0012D5A8
	public static void RegisterBody(WaterBody body)
	{
		if (body.Type == WaterBodyType.Ocean)
		{
			if (WaterSystem.Ocean == null)
			{
				WaterSystem.Ocean = body;
				body.Transform.position = body.Transform.position.WithY(WaterSystem.OceanLevel);
			}
			else if (WaterSystem.Ocean != body)
			{
				Debug.LogWarning("[Water] Ocean body is already registered. Ignoring call because only one is allowed.");
				return;
			}
		}
		WaterSystem.WaterBodies.Add(body);
	}

	// Token: 0x06003147 RID: 12615 RVA: 0x0012F417 File Offset: 0x0012D617
	public static void UnregisterBody(WaterBody body)
	{
		WaterSystem.WaterBodies.Remove(body);
	}

	// Token: 0x06003148 RID: 12616 RVA: 0x0012F428 File Offset: 0x0012D628
	private void UpdateWaves()
	{
		WaterSystem.WaveTime = (this.ProgressTime ? Time.realtimeSinceStartup : WaterSystem.WaveTime);
		using (TimeWarning.New("WaterGerstner.UpdatePrecomputedWaves", 0))
		{
			WaterGerstner.UpdatePrecomputedWaves(this.Simulation.OpenSeaWaves, ref this.precomputedWaves);
		}
		using (TimeWarning.New("WaterGerstner.UpdatePrecomputedShoreWaves", 0))
		{
			WaterGerstner.UpdatePrecomputedShoreWaves(this.Simulation.ShoreWaves, ref this.precomputedShoreWaves);
		}
	}

	// Token: 0x06003149 RID: 12617 RVA: 0x0012F4C8 File Offset: 0x0012D6C8
	private static void UpdateOceanLevel()
	{
		if (WaterSystem.Ocean != null)
		{
			WaterSystem.Ocean.Transform.position = WaterSystem.Ocean.Transform.position.WithY(WaterSystem.OceanLevel);
		}
		if (WaterSystem.nativePathState == WaterSystem.NativePathState.Ready)
		{
			WaterSystem.UpdateOceanLevel_Native(WaterSystem.OceanLevel);
		}
		foreach (WaterBody waterBody in WaterSystem.WaterBodies)
		{
			waterBody.OnOceanLevelChanged(WaterSystem.OceanLevel);
		}
	}

	// Token: 0x0600314A RID: 12618 RVA: 0x0012F564 File Offset: 0x0012D764
	public void UpdateWaveData()
	{
		this.ShoreWavesRcpFadeDistance = 1f / this.Simulation.ShoreWavesFadeDistance;
		this.TerrainRcpFadeDistance = 1f / this.Simulation.TerrainFadeDistance;
		this.global0.x = this.ShoreWavesRcpFadeDistance;
		this.global0.y = this.TerrainRcpFadeDistance;
		this.global0.z = this.precomputedShoreWaves.DirectionVarFreq;
		this.global0.w = this.precomputedShoreWaves.DirectionVarAmp;
		this.global1.x = this.precomputedShoreWaves.Steepness;
		this.global1.y = this.precomputedShoreWaves.Amplitude;
		this.global1.z = this.precomputedShoreWaves.K;
		this.global1.w = this.precomputedShoreWaves.C;
		using (TimeWarning.New("WaterGerstner.UpdateWaveArray", 0))
		{
			WaterGerstner.UpdateWaveArray(this.precomputedWaves, ref this.waveArray);
		}
		using (TimeWarning.New("WaterGerstner.UpdateShoreWaveArray", 0))
		{
			WaterGerstner.UpdateShoreWaveArray(this.precomputedShoreWaves, ref this.shoreWaveArray);
		}
		using (TimeWarning.New("WaterSystem.SetNativeConstants", 0))
		{
			WaterSystem.SetNativeConstants(TerrainTexturing.Instance, TerrainMeta.WaterMap, TerrainMeta.HeightMap, this.global0, this.global1, this.waveArray, this.shoreWaveArray);
		}
	}

	// Token: 0x0600314B RID: 12619 RVA: 0x0012F704 File Offset: 0x0012D904
	private void Update()
	{
		using (TimeWarning.New("UpdateWaves", 0))
		{
			this.UpdateWaves();
		}
		using (TimeWarning.New("UpdateWaveData", 0))
		{
			this.UpdateWaveData();
		}
	}

	// Token: 0x040027CE RID: 10190
	public WaterQuality Quality = WaterQuality.High;

	// Token: 0x040027CF RID: 10191
	public bool ShowDebug;

	// Token: 0x040027D0 RID: 10192
	public bool ShowGizmos;

	// Token: 0x040027D1 RID: 10193
	public bool ProgressTime = true;

	// Token: 0x040027D2 RID: 10194
	public GameObject FallbackPlane;

	// Token: 0x040027D3 RID: 10195
	public WaterSystem.SimulationSettings Simulation = new WaterSystem.SimulationSettings();

	// Token: 0x040027D4 RID: 10196
	public WaterSystem.RenderingSettings Rendering = new WaterSystem.RenderingSettings();

	// Token: 0x040027D5 RID: 10197
	private WaterGerstner.PrecomputedWave[] precomputedWaves = new WaterGerstner.PrecomputedWave[]
	{
		WaterGerstner.PrecomputedWave.Default,
		WaterGerstner.PrecomputedWave.Default,
		WaterGerstner.PrecomputedWave.Default,
		WaterGerstner.PrecomputedWave.Default,
		WaterGerstner.PrecomputedWave.Default,
		WaterGerstner.PrecomputedWave.Default
	};

	// Token: 0x040027D6 RID: 10198
	private WaterGerstner.PrecomputedShoreWaves precomputedShoreWaves = WaterGerstner.PrecomputedShoreWaves.Default;

	// Token: 0x040027D7 RID: 10199
	private Vector4[] waveArray = new Vector4[0];

	// Token: 0x040027D8 RID: 10200
	private Vector4[] shoreWaveArray = new Vector4[0];

	// Token: 0x040027D9 RID: 10201
	private Vector4 global0;

	// Token: 0x040027DA RID: 10202
	private Vector4 global1;

	// Token: 0x040027E2 RID: 10210
	private static float oceanLevel = 0f;

	// Token: 0x040027E4 RID: 10212
	private static WaterSystem instance;

	// Token: 0x040027E5 RID: 10213
	private static Vector3[] emptyShoreMap = new Vector3[]
	{
		Vector3.one
	};

	// Token: 0x040027E6 RID: 10214
	private static short[] emptyWaterMap = new short[1];

	// Token: 0x040027E7 RID: 10215
	private static short[] emptyHeightMap = new short[1];

	// Token: 0x040027E8 RID: 10216
	private static WaterSystem.NativePathState nativePathState = WaterSystem.NativePathState.Initializing;

	// Token: 0x040027E9 RID: 10217
	private static Vector3[] currentShoreMap;

	// Token: 0x040027EA RID: 10218
	private static GCHandle currentShoreMapHandle;

	// Token: 0x040027EB RID: 10219
	private static short[] currentWaterMap;

	// Token: 0x040027EC RID: 10220
	private static GCHandle currentWaterMapHandle;

	// Token: 0x040027ED RID: 10221
	private static short[] currentHeightMap;

	// Token: 0x040027EE RID: 10222
	private static GCHandle currentHeightMapHandle;

	// Token: 0x040027EF RID: 10223
	private static Vector4[] currentOpenWaves;

	// Token: 0x040027F0 RID: 10224
	private static GCHandle currentOpenWavesHandle;

	// Token: 0x040027F1 RID: 10225
	private static Vector4[] currentShoreWaves;

	// Token: 0x040027F2 RID: 10226
	private static GCHandle currentShoreWavesHandle;

	// Token: 0x02000DDB RID: 3547
	[Serializable]
	public class SimulationSettings
	{
		// Token: 0x0400481B RID: 18459
		public Vector3 Wind = new Vector3(3f, 0f, 3f);

		// Token: 0x0400481C RID: 18460
		public int SolverResolution = 64;

		// Token: 0x0400481D RID: 18461
		public float SolverSizeInWorld = 18f;

		// Token: 0x0400481E RID: 18462
		public float Gravity = 9.81f;

		// Token: 0x0400481F RID: 18463
		public float Amplitude = 0.0001f;

		// Token: 0x04004820 RID: 18464
		public Texture2D PerlinNoise;

		// Token: 0x04004821 RID: 18465
		public WaterGerstner.WaveParams[] OpenSeaWaves = new WaterGerstner.WaveParams[6];

		// Token: 0x04004822 RID: 18466
		public WaterGerstner.ShoreWaveParams ShoreWaves = new WaterGerstner.ShoreWaveParams();

		// Token: 0x04004823 RID: 18467
		[Range(0.1f, 250f)]
		public float ShoreWavesFadeDistance = 25f;

		// Token: 0x04004824 RID: 18468
		[Range(0.1f, 250f)]
		public float TerrainFadeDistance = 10f;

		// Token: 0x04004825 RID: 18469
		[Range(0.001f, 1f)]
		public float OpenSeaCrestFoamThreshold = 0.08f;

		// Token: 0x04004826 RID: 18470
		[Range(0.001f, 1f)]
		public float ShoreCrestFoamThreshold = 0.08f;

		// Token: 0x04004827 RID: 18471
		[Range(0.001f, 1f)]
		public float ShoreCrestFoamFarThreshold = 0.08f;

		// Token: 0x04004828 RID: 18472
		[Range(0.1f, 250f)]
		public float ShoreCrestFoamFadeDistance = 10f;
	}

	// Token: 0x02000DDC RID: 3548
	[Serializable]
	public class RenderingSettings
	{
		// Token: 0x04004829 RID: 18473
		public float MaxDisplacementDistance = 50f;

		// Token: 0x0400482A RID: 18474
		public WaterSystem.RenderingSettings.SkyProbe SkyReflections;

		// Token: 0x0400482B RID: 18475
		public WaterSystem.RenderingSettings.SSR ScreenSpaceReflections;

		// Token: 0x0400482C RID: 18476
		public WaterSystem.RenderingSettings.Caustics CausticsAnimation;

		// Token: 0x02000F69 RID: 3945
		[Serializable]
		public class SkyProbe
		{
			// Token: 0x04004E32 RID: 20018
			public float ProbeUpdateInterval = 1f;

			// Token: 0x04004E33 RID: 20019
			public bool TimeSlicing = true;
		}

		// Token: 0x02000F6A RID: 3946
		[Serializable]
		public class SSR
		{
			// Token: 0x04004E34 RID: 20020
			public float FresnelCutoff = 0.02f;

			// Token: 0x04004E35 RID: 20021
			public float ThicknessMin = 1f;

			// Token: 0x04004E36 RID: 20022
			public float ThicknessMax = 20f;

			// Token: 0x04004E37 RID: 20023
			public float ThicknessStartDist = 40f;

			// Token: 0x04004E38 RID: 20024
			public float ThicknessEndDist = 100f;
		}

		// Token: 0x02000F6B RID: 3947
		[Serializable]
		public class Caustics
		{
			// Token: 0x04004E39 RID: 20025
			public float FrameRate = 15f;

			// Token: 0x04004E3A RID: 20026
			public Texture2D[] FramesShallow = new Texture2D[0];

			// Token: 0x04004E3B RID: 20027
			public Texture2D[] FramesDeep = new Texture2D[0];
		}
	}

	// Token: 0x02000DDD RID: 3549
	private enum NativePathState
	{
		// Token: 0x0400482E RID: 18478
		Initializing,
		// Token: 0x0400482F RID: 18479
		Failed,
		// Token: 0x04004830 RID: 18480
		Ready
	}
}
