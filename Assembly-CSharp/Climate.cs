using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x020004C3 RID: 1219
public class Climate : SingletonComponent<Climate>
{
	// Token: 0x1700031A RID: 794
	// (get) Token: 0x06002734 RID: 10036 RVA: 0x000F1DED File Offset: 0x000EFFED
	// (set) Token: 0x06002735 RID: 10037 RVA: 0x000F1DF5 File Offset: 0x000EFFF5
	public float WeatherStateBlend { get; private set; }

	// Token: 0x1700031B RID: 795
	// (get) Token: 0x06002736 RID: 10038 RVA: 0x000F1DFE File Offset: 0x000EFFFE
	// (set) Token: 0x06002737 RID: 10039 RVA: 0x000F1E06 File Offset: 0x000F0006
	public uint WeatherSeedPrevious { get; private set; }

	// Token: 0x1700031C RID: 796
	// (get) Token: 0x06002738 RID: 10040 RVA: 0x000F1E0F File Offset: 0x000F000F
	// (set) Token: 0x06002739 RID: 10041 RVA: 0x000F1E17 File Offset: 0x000F0017
	public uint WeatherSeedTarget { get; private set; }

	// Token: 0x1700031D RID: 797
	// (get) Token: 0x0600273A RID: 10042 RVA: 0x000F1E20 File Offset: 0x000F0020
	// (set) Token: 0x0600273B RID: 10043 RVA: 0x000F1E28 File Offset: 0x000F0028
	public uint WeatherSeedNext { get; private set; }

	// Token: 0x1700031E RID: 798
	// (get) Token: 0x0600273C RID: 10044 RVA: 0x000F1E31 File Offset: 0x000F0031
	// (set) Token: 0x0600273D RID: 10045 RVA: 0x000F1E39 File Offset: 0x000F0039
	public WeatherPreset WeatherStatePrevious { get; private set; }

	// Token: 0x1700031F RID: 799
	// (get) Token: 0x0600273E RID: 10046 RVA: 0x000F1E42 File Offset: 0x000F0042
	// (set) Token: 0x0600273F RID: 10047 RVA: 0x000F1E4A File Offset: 0x000F004A
	public WeatherPreset WeatherStateTarget { get; private set; }

	// Token: 0x17000320 RID: 800
	// (get) Token: 0x06002740 RID: 10048 RVA: 0x000F1E53 File Offset: 0x000F0053
	// (set) Token: 0x06002741 RID: 10049 RVA: 0x000F1E5B File Offset: 0x000F005B
	public WeatherPreset WeatherStateNext { get; private set; }

	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06002742 RID: 10050 RVA: 0x000F1E64 File Offset: 0x000F0064
	// (set) Token: 0x06002743 RID: 10051 RVA: 0x000F1E6C File Offset: 0x000F006C
	public WeatherPreset WeatherState { get; private set; }

	// Token: 0x17000322 RID: 802
	// (get) Token: 0x06002744 RID: 10052 RVA: 0x000F1E75 File Offset: 0x000F0075
	// (set) Token: 0x06002745 RID: 10053 RVA: 0x000F1E7D File Offset: 0x000F007D
	public WeatherPreset WeatherClampsMin { get; private set; }

	// Token: 0x17000323 RID: 803
	// (get) Token: 0x06002746 RID: 10054 RVA: 0x000F1E86 File Offset: 0x000F0086
	// (set) Token: 0x06002747 RID: 10055 RVA: 0x000F1E8E File Offset: 0x000F008E
	public WeatherPreset WeatherClampsMax { get; private set; }

	// Token: 0x17000324 RID: 804
	// (get) Token: 0x06002748 RID: 10056 RVA: 0x000F1E97 File Offset: 0x000F0097
	// (set) Token: 0x06002749 RID: 10057 RVA: 0x000F1E9F File Offset: 0x000F009F
	public WeatherPreset WeatherOverrides { get; private set; }

	// Token: 0x17000325 RID: 805
	// (get) Token: 0x0600274A RID: 10058 RVA: 0x000F1EA8 File Offset: 0x000F00A8
	// (set) Token: 0x0600274B RID: 10059 RVA: 0x000F1EB0 File Offset: 0x000F00B0
	public LegacyWeatherState Overrides { get; private set; }

	// Token: 0x0600274C RID: 10060 RVA: 0x000F1EBC File Offset: 0x000F00BC
	protected override void Awake()
	{
		base.Awake();
		this.WeatherState = (ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset);
		this.WeatherClampsMin = (ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset);
		this.WeatherClampsMax = (ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset);
		this.WeatherOverrides = (ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset);
		this.WeatherState.Reset();
		this.WeatherClampsMin.Reset();
		this.WeatherClampsMax.Reset();
		this.WeatherOverrides.Reset();
		this.Overrides = new LegacyWeatherState(this.WeatherOverrides);
	}

	// Token: 0x0600274D RID: 10061 RVA: 0x000F1F74 File Offset: 0x000F0174
	protected override void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.OnDestroy();
		if (this.WeatherState != null)
		{
			UnityEngine.Object.Destroy(this.WeatherState);
		}
		if (this.WeatherClampsMin != null)
		{
			UnityEngine.Object.Destroy(this.WeatherClampsMin);
		}
		if (this.WeatherClampsMax != null)
		{
			UnityEngine.Object.Destroy(this.WeatherClampsMax);
		}
		if (this.WeatherOverrides != null)
		{
			UnityEngine.Object.Destroy(this.WeatherOverrides);
		}
	}

	// Token: 0x0600274E RID: 10062 RVA: 0x000F1FF4 File Offset: 0x000F01F4
	protected void Update()
	{
		if (Rust.Application.isReceiving)
		{
			return;
		}
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!TOD_Sky.Instance)
		{
			return;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		long num = (long)((ulong)World.Seed + (ulong)instance.Cycle.Ticks);
		long num2 = 648000000000L;
		long num3 = 216000000000L;
		long num4 = num / num2;
		this.WeatherStateBlend = Mathf.InverseLerp(0f, (float)num3, (float)(num % num2));
		this.WeatherStatePrevious = this.GetWeatherPreset(this.WeatherSeedPrevious = this.GetSeedFromLong(num4));
		this.WeatherStateTarget = this.GetWeatherPreset(this.WeatherSeedTarget = this.GetSeedFromLong(num4 + 1L));
		this.WeatherStateNext = this.GetWeatherPreset(this.WeatherSeedNext = this.GetSeedFromLong(num4 + 2L));
		this.WeatherState.Fade(this.WeatherStatePrevious, this.WeatherStateTarget, this.WeatherStateBlend);
		this.WeatherState.Override(this.WeatherOverrides);
	}

	// Token: 0x0600274F RID: 10063 RVA: 0x000F20F8 File Offset: 0x000F02F8
	private static bool Initialized()
	{
		return SingletonComponent<Climate>.Instance && SingletonComponent<Climate>.Instance.WeatherStatePrevious && SingletonComponent<Climate>.Instance.WeatherStateTarget && SingletonComponent<Climate>.Instance.WeatherStateNext && SingletonComponent<Climate>.Instance.WeatherState && SingletonComponent<Climate>.Instance.WeatherClampsMin && SingletonComponent<Climate>.Instance.WeatherOverrides;
	}

	// Token: 0x06002750 RID: 10064 RVA: 0x000F2186 File Offset: 0x000F0386
	public static float GetClouds(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		return SingletonComponent<Climate>.Instance.WeatherState.Clouds.Coverage;
	}

	// Token: 0x06002751 RID: 10065 RVA: 0x000F21A9 File Offset: 0x000F03A9
	public static float GetFog(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		return SingletonComponent<Climate>.Instance.WeatherState.Atmosphere.Fogginess;
	}

	// Token: 0x06002752 RID: 10066 RVA: 0x000F21CC File Offset: 0x000F03CC
	public static float GetWind(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		return SingletonComponent<Climate>.Instance.WeatherState.Wind;
	}

	// Token: 0x06002753 RID: 10067 RVA: 0x000F21EC File Offset: 0x000F03EC
	public static float GetThunder(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		float thunder = SingletonComponent<Climate>.Instance.WeatherOverrides.Thunder;
		if (thunder >= 0f)
		{
			return thunder;
		}
		float thunder2 = SingletonComponent<Climate>.Instance.WeatherState.Thunder;
		float thunder3 = SingletonComponent<Climate>.Instance.WeatherStatePrevious.Thunder;
		float thunder4 = SingletonComponent<Climate>.Instance.WeatherStateTarget.Thunder;
		if (thunder3 > 0f && thunder2 > 0.5f * thunder3)
		{
			return thunder2;
		}
		if (thunder4 > 0f && thunder2 > 0.5f * thunder4)
		{
			return thunder2;
		}
		return 0f;
	}

	// Token: 0x06002754 RID: 10068 RVA: 0x000F2280 File Offset: 0x000F0480
	public static float GetRainbow(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		if (!instance || !instance.IsDay || instance.LerpValue < 1f)
		{
			return 0f;
		}
		if (Climate.GetFog(position) > 0.25f)
		{
			return 0f;
		}
		float num = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 3) : 0f;
		if (num <= 0f)
		{
			return 0f;
		}
		float rainbow = SingletonComponent<Climate>.Instance.WeatherOverrides.Rainbow;
		if (rainbow >= 0f)
		{
			return rainbow * num;
		}
		if (SingletonComponent<Climate>.Instance.WeatherState.Rainbow <= 0f)
		{
			return 0f;
		}
		if (SingletonComponent<Climate>.Instance.WeatherStateTarget.Rainbow > 0f)
		{
			return 0f;
		}
		float rainbow2 = SingletonComponent<Climate>.Instance.WeatherStatePrevious.Rainbow;
		float num2 = SeedRandom.Value(SingletonComponent<Climate>.Instance.WeatherSeedPrevious);
		if (rainbow2 < num2)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x06002755 RID: 10069 RVA: 0x000F2384 File Offset: 0x000F0584
	public static float GetAurora(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		if (!instance || !instance.IsNight || instance.LerpValue > 0f)
		{
			return 0f;
		}
		if (Climate.GetClouds(position) > 0.1f)
		{
			return 0f;
		}
		if (Climate.GetFog(position) > 0.1f)
		{
			return 0f;
		}
		if (!TerrainMeta.BiomeMap)
		{
			return 0f;
		}
		return TerrainMeta.BiomeMap.GetBiome(position, 8);
	}

	// Token: 0x06002756 RID: 10070 RVA: 0x000F240C File Offset: 0x000F060C
	public static float GetRain(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		float t = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 1) : 0f;
		float num = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0f;
		return SingletonComponent<Climate>.Instance.WeatherState.Rain * Mathf.Lerp(1f, 0.5f, t) * (1f - num);
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x000F2490 File Offset: 0x000F0690
	public static float GetSnow(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		float num = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0f;
		return SingletonComponent<Climate>.Instance.WeatherState.Rain * num;
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x000F24DC File Offset: 0x000F06DC
	public static float GetTemperature(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 15f;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		if (!instance)
		{
			return 15f;
		}
		Climate.ClimateParameters climateParameters;
		Climate.ClimateParameters climateParameters2;
		float t = SingletonComponent<Climate>.Instance.FindBlendParameters(position, out climateParameters, out climateParameters2);
		if (climateParameters == null || climateParameters2 == null)
		{
			return 15f;
		}
		float hour = instance.Cycle.Hour;
		float a = climateParameters.Temperature.Evaluate(hour);
		float b = climateParameters2.Temperature.Evaluate(hour);
		return Mathf.Lerp(a, b, t);
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x000F2558 File Offset: 0x000F0758
	private uint GetSeedFromLong(long val)
	{
		uint result = (uint)((val % (long)((ulong)-1) + (long)((ulong)-1)) % (long)((ulong)-1));
		SeedRandom.Wanghash(ref result);
		SeedRandom.Wanghash(ref result);
		SeedRandom.Wanghash(ref result);
		return result;
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x000F258C File Offset: 0x000F078C
	private WeatherPreset GetWeatherPreset(uint seed)
	{
		float max = this.Weather.ClearChance + this.Weather.DustChance + this.Weather.FogChance + this.Weather.OvercastChance + this.Weather.StormChance + this.Weather.RainChance;
		float num = SeedRandom.Range(ref seed, 0f, max);
		if (num < this.Weather.RainChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Rain);
		}
		if (num < this.Weather.RainChance + this.Weather.StormChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Storm);
		}
		if (num < this.Weather.RainChance + this.Weather.StormChance + this.Weather.OvercastChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Overcast);
		}
		if (num < this.Weather.RainChance + this.Weather.StormChance + this.Weather.OvercastChance + this.Weather.FogChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Fog);
		}
		if (num < this.Weather.RainChance + this.Weather.StormChance + this.Weather.OvercastChance + this.Weather.FogChance + this.Weather.DustChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Dust);
		}
		return this.GetWeatherPreset(seed, WeatherPresetType.Clear);
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x000F26E4 File Offset: 0x000F08E4
	private WeatherPreset GetWeatherPreset(uint seed, WeatherPresetType type)
	{
		if (this.presetLookup == null)
		{
			this.presetLookup = new Dictionary<WeatherPresetType, WeatherPreset[]>();
		}
		WeatherPreset[] array;
		if (!this.presetLookup.TryGetValue(type, out array))
		{
			this.presetLookup.Add(type, array = this.CacheWeatherPresets(type));
		}
		return array.GetRandom(ref seed);
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x000F2734 File Offset: 0x000F0934
	private WeatherPreset[] CacheWeatherPresets(WeatherPresetType type)
	{
		return (from x in this.WeatherPresets
		where x.Type == type
		select x).ToArray<WeatherPreset>();
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000F276C File Offset: 0x000F096C
	private float FindBlendParameters(Vector3 pos, out Climate.ClimateParameters src, out Climate.ClimateParameters dst)
	{
		if (this.climateLookup == null)
		{
			this.climateLookup = new Climate.ClimateParameters[]
			{
				this.Arid,
				this.Temperate,
				this.Tundra,
				this.Arctic
			};
		}
		if (TerrainMeta.BiomeMap == null)
		{
			src = this.Temperate;
			dst = this.Temperate;
			return 0.5f;
		}
		int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
		int biomeMaxType2 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType);
		src = this.climateLookup[TerrainBiome.TypeToIndex(biomeMaxType)];
		dst = this.climateLookup[TerrainBiome.TypeToIndex(biomeMaxType2)];
		return TerrainMeta.BiomeMap.GetBiome(pos, biomeMaxType2);
	}

	// Token: 0x04001F8A RID: 8074
	private const float fadeAngle = 20f;

	// Token: 0x04001F8B RID: 8075
	private const float defaultTemp = 15f;

	// Token: 0x04001F8C RID: 8076
	private const int weatherDurationHours = 18;

	// Token: 0x04001F8D RID: 8077
	private const int weatherFadeHours = 6;

	// Token: 0x04001F8E RID: 8078
	[Range(0f, 1f)]
	public float BlendingSpeed = 1f;

	// Token: 0x04001F8F RID: 8079
	[Range(1f, 9f)]
	public float FogMultiplier = 5f;

	// Token: 0x04001F90 RID: 8080
	public float FogDarknessDistance = 200f;

	// Token: 0x04001F91 RID: 8081
	public bool DebugLUTBlending;

	// Token: 0x04001F92 RID: 8082
	public Climate.WeatherParameters Weather;

	// Token: 0x04001F93 RID: 8083
	public WeatherPreset[] WeatherPresets;

	// Token: 0x04001F94 RID: 8084
	public Climate.ClimateParameters Arid;

	// Token: 0x04001F95 RID: 8085
	public Climate.ClimateParameters Temperate;

	// Token: 0x04001F96 RID: 8086
	public Climate.ClimateParameters Tundra;

	// Token: 0x04001F97 RID: 8087
	public Climate.ClimateParameters Arctic;

	// Token: 0x04001FA4 RID: 8100
	private Dictionary<WeatherPresetType, WeatherPreset[]> presetLookup;

	// Token: 0x04001FA5 RID: 8101
	private Climate.ClimateParameters[] climateLookup;

	// Token: 0x02000CD0 RID: 3280
	[Serializable]
	public class ClimateParameters
	{
		// Token: 0x040043E8 RID: 17384
		public AnimationCurve Temperature;

		// Token: 0x040043E9 RID: 17385
		[Horizontal(4, -1)]
		public Climate.Float4 AerialDensity;

		// Token: 0x040043EA RID: 17386
		[Horizontal(4, -1)]
		public Climate.Float4 FogDensity;

		// Token: 0x040043EB RID: 17387
		[Horizontal(4, -1)]
		public Climate.Texture2D4 LUT;
	}

	// Token: 0x02000CD1 RID: 3281
	[Serializable]
	public class WeatherParameters
	{
		// Token: 0x040043EC RID: 17388
		[Range(0f, 1f)]
		public float ClearChance = 1f;

		// Token: 0x040043ED RID: 17389
		[Range(0f, 1f)]
		public float DustChance;

		// Token: 0x040043EE RID: 17390
		[Range(0f, 1f)]
		public float FogChance;

		// Token: 0x040043EF RID: 17391
		[Range(0f, 1f)]
		public float OvercastChance;

		// Token: 0x040043F0 RID: 17392
		[Range(0f, 1f)]
		public float StormChance;

		// Token: 0x040043F1 RID: 17393
		[Range(0f, 1f)]
		public float RainChance;
	}

	// Token: 0x02000CD2 RID: 3282
	public class Value4<T>
	{
		// Token: 0x06004D9D RID: 19869 RVA: 0x00198EB4 File Offset: 0x001970B4
		public float FindBlendParameters(TOD_Sky sky, out T src, out T dst)
		{
			float num = Mathf.Abs(sky.SunriseTime - sky.Cycle.Hour);
			float num2 = Mathf.Abs(sky.SunsetTime - sky.Cycle.Hour);
			float num3 = (180f - sky.SunZenith) / 180f;
			float num4 = 0.11111111f;
			if (num < num2)
			{
				if (num3 < 0.5f)
				{
					src = this.Night;
					dst = this.Dawn;
					return Mathf.InverseLerp(0.5f - num4, 0.5f, num3);
				}
				src = this.Dawn;
				dst = this.Noon;
				return Mathf.InverseLerp(0.5f, 0.5f + num4, num3);
			}
			else
			{
				if (num3 > 0.5f)
				{
					src = this.Noon;
					dst = this.Dusk;
					return Mathf.InverseLerp(0.5f + num4, 0.5f, num3);
				}
				src = this.Dusk;
				dst = this.Night;
				return Mathf.InverseLerp(0.5f, 0.5f - num4, num3);
			}
		}

		// Token: 0x040043F2 RID: 17394
		public T Dawn;

		// Token: 0x040043F3 RID: 17395
		public T Noon;

		// Token: 0x040043F4 RID: 17396
		public T Dusk;

		// Token: 0x040043F5 RID: 17397
		public T Night;
	}

	// Token: 0x02000CD3 RID: 3283
	[Serializable]
	public class Float4 : Climate.Value4<float>
	{
	}

	// Token: 0x02000CD4 RID: 3284
	[Serializable]
	public class Color4 : Climate.Value4<Color>
	{
	}

	// Token: 0x02000CD5 RID: 3285
	[Serializable]
	public class Texture2D4 : Climate.Value4<Texture2D>
	{
	}
}
