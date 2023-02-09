using System;
using UnityEngine;

// Token: 0x020006D4 RID: 1748
public class WaterGerstner
{
	// Token: 0x060030EA RID: 12522 RVA: 0x0012CCE8 File Offset: 0x0012AEE8
	public static void UpdatePrecomputedWaves(WaterGerstner.WaveParams[] waves, ref WaterGerstner.PrecomputedWave[] precomputed)
	{
		if (precomputed == null || precomputed.Length != 6)
		{
			precomputed = new WaterGerstner.PrecomputedWave[6];
		}
		Debug.Assert(precomputed.Length == waves.Length);
		for (int i = 0; i < 6; i++)
		{
			float num = waves[i].Angle * 0.017453292f;
			precomputed[i].Angle = num;
			precomputed[i].Direction = new Vector2(Mathf.Cos(num), Mathf.Sin(num));
			precomputed[i].Steepness = waves[i].Steepness;
			precomputed[i].K = 6.2831855f / waves[i].Length;
			precomputed[i].C = Mathf.Sqrt(9.8f / precomputed[i].K) * waves[i].Speed * WaterSystem.WaveTime;
			precomputed[i].A = waves[i].Steepness / precomputed[i].K;
		}
	}

	// Token: 0x060030EB RID: 12523 RVA: 0x0012CDEC File Offset: 0x0012AFEC
	public static void UpdatePrecomputedShoreWaves(WaterGerstner.ShoreWaveParams shoreWaves, ref WaterGerstner.PrecomputedShoreWaves precomputed)
	{
		if (precomputed.Directions == null || precomputed.Directions.Length != 6)
		{
			precomputed.Directions = new Vector2[6];
		}
		Debug.Assert(precomputed.Directions.Length == shoreWaves.DirectionAngles.Length);
		for (int i = 0; i < 6; i++)
		{
			float f = shoreWaves.DirectionAngles[i] * 0.017453292f;
			precomputed.Directions[i] = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
		}
		precomputed.Steepness = shoreWaves.Steepness;
		precomputed.Amplitude = shoreWaves.Amplitude;
		precomputed.K = 6.2831855f / shoreWaves.Length;
		precomputed.C = Mathf.Sqrt(9.8f / precomputed.K) * shoreWaves.Speed * WaterSystem.WaveTime;
		precomputed.A = shoreWaves.Steepness / precomputed.K;
		precomputed.DirectionVarFreq = shoreWaves.DirectionVarFreq;
		precomputed.DirectionVarAmp = shoreWaves.DirectionVarAmp;
	}

	// Token: 0x060030EC RID: 12524 RVA: 0x0012CEE0 File Offset: 0x0012B0E0
	public static void UpdateWaveArray(WaterGerstner.PrecomputedWave[] precomputed, ref Vector4[] array)
	{
		if (array == null || array.Length != 6)
		{
			array = new Vector4[6];
		}
		Debug.Assert(array.Length == precomputed.Length);
		for (int i = 0; i < 6; i++)
		{
			array[i] = new Vector4(precomputed[i].Angle, precomputed[i].Steepness, precomputed[i].K, precomputed[i].C);
		}
	}

	// Token: 0x060030ED RID: 12525 RVA: 0x0012CF58 File Offset: 0x0012B158
	public static void UpdateShoreWaveArray(WaterGerstner.PrecomputedShoreWaves precomputed, ref Vector4[] array)
	{
		Debug.Assert(precomputed.Directions.Length == 6);
		if (array == null || array.Length != 3)
		{
			array = new Vector4[3];
		}
		Debug.Assert(array.Length == 3);
		Vector2[] directions = precomputed.Directions;
		array[0] = new Vector4(directions[0].x, directions[0].y, directions[1].x, directions[1].y);
		array[1] = new Vector4(directions[2].x, directions[2].y, directions[3].x, directions[3].y);
		array[2] = new Vector4(directions[4].x, directions[4].y, directions[5].x, directions[5].y);
	}

	// Token: 0x060030EE RID: 12526 RVA: 0x0012D054 File Offset: 0x0012B254
	private static void GerstnerWave(WaterGerstner.PrecomputedWave wave, Vector2 pos, Vector2 shoreVec, ref float outH)
	{
		Vector2 direction = wave.Direction;
		float num = Mathf.Sin(wave.K * (direction.x * pos.x + direction.y * pos.y - wave.C));
		outH += wave.A * num;
	}

	// Token: 0x060030EF RID: 12527 RVA: 0x0012D0A4 File Offset: 0x0012B2A4
	private static void GerstnerWave(WaterGerstner.PrecomputedWave wave, Vector2 pos, Vector2 shoreVec, ref Vector3 outP)
	{
		Vector2 direction = wave.Direction;
		float f = wave.K * (direction.x * pos.x + direction.y * pos.y - wave.C);
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		outP.x += direction.x * wave.A * num;
		outP.y += wave.A * num2;
		outP.z += direction.y * wave.A * num;
	}

	// Token: 0x060030F0 RID: 12528 RVA: 0x0012D134 File Offset: 0x0012B334
	private static void GerstnerShoreWave(WaterGerstner.PrecomputedShoreWaves wave, Vector2 waveDir, Vector2 pos, Vector2 shoreVec, float variation_t, ref float outH)
	{
		float num = Mathf.Clamp01(waveDir.x * shoreVec.x + waveDir.y * shoreVec.y);
		num *= num;
		float f = wave.K * (waveDir.x * pos.x + waveDir.y * pos.y - wave.C + variation_t);
		Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		outH += wave.A * wave.Amplitude * num2 * num;
	}

	// Token: 0x060030F1 RID: 12529 RVA: 0x0012D1B8 File Offset: 0x0012B3B8
	private static void GerstnerShoreWave(WaterGerstner.PrecomputedShoreWaves wave, Vector2 waveDir, Vector2 pos, Vector2 shoreVec, float variation_t, ref Vector3 outP)
	{
		float num = Mathf.Clamp01(waveDir.x * shoreVec.x + waveDir.y * shoreVec.y);
		num *= num;
		float f = wave.K * (waveDir.x * pos.x + waveDir.y * pos.y - wave.C + variation_t);
		float num2 = Mathf.Cos(f);
		float num3 = Mathf.Sin(f);
		outP.x += waveDir.x * wave.A * num2 * num;
		outP.y += wave.A * wave.Amplitude * num3 * num;
		outP.z += waveDir.y * wave.A * num2 * num;
	}

	// Token: 0x060030F2 RID: 12530 RVA: 0x0012D278 File Offset: 0x0012B478
	public static Vector3 SampleDisplacement(WaterSystem instance, Vector3 location, Vector3 shore)
	{
		WaterGerstner.PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		WaterGerstner.PrecomputedShoreWaves precomputedShoreWaves = instance.PrecomputedShoreWaves;
		Vector2 vector = new Vector2(location.x, location.z);
		Vector2 shoreVec = new Vector2(shore.x, shore.y);
		float t = 1f - Mathf.Clamp01(shore.z * instance.ShoreWavesRcpFadeDistance);
		float d = Mathf.Clamp01(shore.z * instance.TerrainRcpFadeDistance);
		float num = Mathf.Cos(vector.x * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float num2 = Mathf.Cos(vector.y * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float variation_t = num + num2;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		for (int i = 0; i < 6; i++)
		{
			WaterGerstner.GerstnerWave(precomputedWaves[i], vector, shoreVec, ref zero);
			WaterGerstner.GerstnerShoreWave(precomputedShoreWaves, precomputedShoreWaves.Directions[i], vector, shoreVec, variation_t, ref zero2);
		}
		return Vector3.Lerp(zero, zero2, t) * d;
	}

	// Token: 0x060030F3 RID: 12531 RVA: 0x0012D37C File Offset: 0x0012B57C
	private static float SampleHeightREF(WaterSystem instance, Vector3 location, Vector3 shore)
	{
		WaterGerstner.PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		WaterGerstner.PrecomputedShoreWaves precomputedShoreWaves = instance.PrecomputedShoreWaves;
		Vector2 vector = new Vector2(location.x, location.z);
		Vector2 shoreVec = new Vector2(shore.x, shore.y);
		float t = 1f - Mathf.Clamp01(shore.z * instance.ShoreWavesRcpFadeDistance);
		float num = Mathf.Clamp01(shore.z * instance.TerrainRcpFadeDistance);
		float num2 = Mathf.Cos(vector.x * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float num3 = Mathf.Cos(vector.y * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float variation_t = num2 + num3;
		float a = 0f;
		float b = 0f;
		for (int i = 0; i < 6; i++)
		{
			WaterGerstner.GerstnerWave(precomputedWaves[i], vector, shoreVec, ref a);
			WaterGerstner.GerstnerShoreWave(precomputedShoreWaves, precomputedShoreWaves.Directions[i], vector, shoreVec, variation_t, ref b);
		}
		return Mathf.Lerp(a, b, t) * num;
	}

	// Token: 0x060030F4 RID: 12532 RVA: 0x0012D47C File Offset: 0x0012B67C
	private static void SampleHeightArrayREF(WaterSystem instance, Vector2[] location, Vector3[] shore, float[] height)
	{
		Debug.Assert(location.Length == height.Length);
		for (int i = 0; i < location.Length; i++)
		{
			Vector3 location2 = new Vector3(location[i].x, 0f, location[i].y);
			height[i] = WaterGerstner.SampleHeight(instance, location2, shore[i]);
		}
	}

	// Token: 0x060030F5 RID: 12533 RVA: 0x0012D4DC File Offset: 0x0012B6DC
	public static float SampleHeight(WaterSystem instance, Vector3 location, Vector3 shore)
	{
		WaterGerstner.PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		Vector2[] directions = instance.PrecomputedShoreWaves.Directions;
		Vector4 global = instance.Global0;
		Vector4 global2 = instance.Global1;
		float x = global2.x;
		float y = global2.y;
		float z = global2.z;
		float w = global2.w;
		float num = x / z;
		Vector2 vector = new Vector2(location.x, location.z);
		Vector2 vector2 = new Vector2(shore.x, shore.y);
		float t = 1f - Mathf.Clamp01(shore.z * global.x);
		float num2 = Mathf.Clamp01(shore.z * global.y);
		float num3 = Mathf.Cos(vector.x * global.z) * global.w;
		float num4 = Mathf.Cos(vector.y * global.z) * global.w;
		float num5 = num3 + num4;
		float num6 = 0f;
		float num7 = 0f;
		for (int i = 0; i < 6; i++)
		{
			Vector2 direction = precomputedWaves[i].Direction;
			float c = precomputedWaves[i].C;
			float k = precomputedWaves[i].K;
			float a = precomputedWaves[i].A;
			float num8 = Mathf.Sin(k * (direction.x * vector.x + direction.y * vector.y - c));
			num6 += a * num8;
			Vector2 vector3 = directions[i];
			float num9 = Mathf.Clamp01(vector3.x * vector2.x + vector3.y * vector2.y);
			num9 *= num9;
			float num10 = Mathf.Sin(z * (vector3.x * vector.x + vector3.y * vector.y - w + num5));
			num7 += num * y * num10 * num9;
		}
		return Mathf.Lerp(num6, num7, t) * num2;
	}

	// Token: 0x060030F6 RID: 12534 RVA: 0x0012D6D8 File Offset: 0x0012B8D8
	public static void SampleHeightArray(WaterSystem instance, Vector2[] location, Vector3[] shore, float[] height)
	{
		Debug.Assert(location.Length == height.Length);
		WaterGerstner.PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		Vector2[] directions = instance.PrecomputedShoreWaves.Directions;
		Vector4 global = instance.Global0;
		Vector4 global2 = instance.Global1;
		float x = global2.x;
		float y = global2.y;
		float z = global2.z;
		float w = global2.w;
		float num = x / z;
		for (int i = 0; i < location.Length; i++)
		{
			Vector2 vector = new Vector2(location[i].x, location[i].y);
			Vector2 vector2 = new Vector2(shore[i].x, shore[i].y);
			float t = 1f - Mathf.Clamp01(shore[i].z * global.x);
			float num2 = Mathf.Clamp01(shore[i].z * global.y);
			float num3 = Mathf.Cos(vector.x * global.z) * global.w;
			float num4 = Mathf.Cos(vector.y * global.z) * global.w;
			float num5 = num3 + num4;
			float num6 = 0f;
			float num7 = 0f;
			for (int j = 0; j < 6; j++)
			{
				Vector2 direction = precomputedWaves[j].Direction;
				float c = precomputedWaves[j].C;
				float k = precomputedWaves[j].K;
				float a = precomputedWaves[j].A;
				float num8 = Mathf.Sin(k * (direction.x * vector.x + direction.y * vector.y - c));
				num6 += a * num8;
				Vector2 vector3 = directions[j];
				float num9 = Mathf.Clamp01(vector3.x * vector2.x + vector3.y * vector2.y);
				num9 *= num9;
				float num10 = Mathf.Sin(z * (vector3.x * vector.x + vector3.y * vector.y - w + num5));
				num7 += num * y * num10 * num9;
			}
			height[i] = Mathf.Lerp(num6, num7, t) * num2;
		}
	}

	// Token: 0x040027B1 RID: 10161
	public const int WaveCount = 6;

	// Token: 0x02000DD7 RID: 3543
	[Serializable]
	public class WaveParams
	{
		// Token: 0x04004800 RID: 18432
		[Range(0f, 360f)]
		public float Angle;

		// Token: 0x04004801 RID: 18433
		[Range(0f, 0.99f)]
		public float Steepness = 0.4f;

		// Token: 0x04004802 RID: 18434
		[Range(0.01f, 1000f)]
		public float Length = 15f;

		// Token: 0x04004803 RID: 18435
		[Range(-10f, 10f)]
		public float Speed = 0.4f;
	}

	// Token: 0x02000DD8 RID: 3544
	[Serializable]
	public class ShoreWaveParams
	{
		// Token: 0x04004804 RID: 18436
		[Range(0f, 2f)]
		public float Steepness = 0.99f;

		// Token: 0x04004805 RID: 18437
		[Range(0f, 1f)]
		public float Amplitude = 0.2f;

		// Token: 0x04004806 RID: 18438
		[Range(0.01f, 1000f)]
		public float Length = 20f;

		// Token: 0x04004807 RID: 18439
		[Range(-10f, 10f)]
		public float Speed = 0.6f;

		// Token: 0x04004808 RID: 18440
		public float[] DirectionAngles = new float[]
		{
			0f,
			57.3f,
			114.5f,
			171.9f,
			229.2f,
			286.5f
		};

		// Token: 0x04004809 RID: 18441
		public float DirectionVarFreq = 0.1f;

		// Token: 0x0400480A RID: 18442
		public float DirectionVarAmp = 2.5f;
	}

	// Token: 0x02000DD9 RID: 3545
	public struct PrecomputedWave
	{
		// Token: 0x0400480B RID: 18443
		public float Angle;

		// Token: 0x0400480C RID: 18444
		public Vector2 Direction;

		// Token: 0x0400480D RID: 18445
		public float Steepness;

		// Token: 0x0400480E RID: 18446
		public float K;

		// Token: 0x0400480F RID: 18447
		public float C;

		// Token: 0x04004810 RID: 18448
		public float A;

		// Token: 0x04004811 RID: 18449
		public static WaterGerstner.PrecomputedWave Default = new WaterGerstner.PrecomputedWave
		{
			Angle = 0f,
			Direction = Vector2.right,
			Steepness = 0.4f,
			K = 1f,
			C = 1f,
			A = 1f
		};
	}

	// Token: 0x02000DDA RID: 3546
	public struct PrecomputedShoreWaves
	{
		// Token: 0x04004812 RID: 18450
		public Vector2[] Directions;

		// Token: 0x04004813 RID: 18451
		public float Steepness;

		// Token: 0x04004814 RID: 18452
		public float Amplitude;

		// Token: 0x04004815 RID: 18453
		public float K;

		// Token: 0x04004816 RID: 18454
		public float C;

		// Token: 0x04004817 RID: 18455
		public float A;

		// Token: 0x04004818 RID: 18456
		public float DirectionVarFreq;

		// Token: 0x04004819 RID: 18457
		public float DirectionVarAmp;

		// Token: 0x0400481A RID: 18458
		public static WaterGerstner.PrecomputedShoreWaves Default = new WaterGerstner.PrecomputedShoreWaves
		{
			Directions = new Vector2[]
			{
				Vector2.right,
				Vector2.right,
				Vector2.right,
				Vector2.right,
				Vector2.right,
				Vector2.right
			},
			Steepness = 0.75f,
			Amplitude = 0.2f,
			K = 1f,
			C = 1f,
			A = 1f,
			DirectionVarFreq = 0.1f,
			DirectionVarAmp = 3f
		};
	}
}
