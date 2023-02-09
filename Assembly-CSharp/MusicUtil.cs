using System;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class MusicUtil
{
	// Token: 0x06001AC3 RID: 6851 RVA: 0x000BC683 File Offset: 0x000BA883
	public static double BeatsToSeconds(float tempo, float beats)
	{
		return 60.0 / (double)tempo * (double)beats;
	}

	// Token: 0x06001AC4 RID: 6852 RVA: 0x000BC694 File Offset: 0x000BA894
	public static double BarsToSeconds(float tempo, float bars)
	{
		return MusicUtil.BeatsToSeconds(tempo, bars * 4f);
	}

	// Token: 0x06001AC5 RID: 6853 RVA: 0x000BC6A3 File Offset: 0x000BA8A3
	public static int SecondsToSamples(double seconds)
	{
		return MusicUtil.SecondsToSamples(seconds, UnityEngine.AudioSettings.outputSampleRate);
	}

	// Token: 0x06001AC6 RID: 6854 RVA: 0x000BC6B0 File Offset: 0x000BA8B0
	public static int SecondsToSamples(double seconds, int sampleRate)
	{
		return (int)((double)sampleRate * seconds);
	}

	// Token: 0x06001AC7 RID: 6855 RVA: 0x000BC6B7 File Offset: 0x000BA8B7
	public static int SecondsToSamples(float seconds)
	{
		return MusicUtil.SecondsToSamples(seconds, UnityEngine.AudioSettings.outputSampleRate);
	}

	// Token: 0x06001AC8 RID: 6856 RVA: 0x000BC6C4 File Offset: 0x000BA8C4
	public static int SecondsToSamples(float seconds, int sampleRate)
	{
		return (int)((float)sampleRate * seconds);
	}

	// Token: 0x06001AC9 RID: 6857 RVA: 0x000BC6CB File Offset: 0x000BA8CB
	public static int BarsToSamples(float tempo, float bars, int sampleRate)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BarsToSeconds(tempo, bars), sampleRate);
	}

	// Token: 0x06001ACA RID: 6858 RVA: 0x000BC6DA File Offset: 0x000BA8DA
	public static int BarsToSamples(float tempo, float bars)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BarsToSeconds(tempo, bars));
	}

	// Token: 0x06001ACB RID: 6859 RVA: 0x000BC6E8 File Offset: 0x000BA8E8
	public static int BeatsToSamples(float tempo, float beats)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BeatsToSeconds(tempo, beats));
	}

	// Token: 0x06001ACC RID: 6860 RVA: 0x000BC6F6 File Offset: 0x000BA8F6
	public static float SecondsToBeats(float tempo, double seconds)
	{
		return tempo / 60f * (float)seconds;
	}

	// Token: 0x06001ACD RID: 6861 RVA: 0x000BC702 File Offset: 0x000BA902
	public static float SecondsToBars(float tempo, double seconds)
	{
		return MusicUtil.SecondsToBeats(tempo, seconds) / 4f;
	}

	// Token: 0x06001ACE RID: 6862 RVA: 0x000BC711 File Offset: 0x000BA911
	public static float Quantize(float position, float gridSize)
	{
		return Mathf.Round(position / gridSize) * gridSize;
	}

	// Token: 0x06001ACF RID: 6863 RVA: 0x000BC71D File Offset: 0x000BA91D
	public static float FlooredQuantize(float position, float gridSize)
	{
		return Mathf.Floor(position / gridSize) * gridSize;
	}

	// Token: 0x04001382 RID: 4994
	public const float OneSixteenth = 0.0625f;
}
