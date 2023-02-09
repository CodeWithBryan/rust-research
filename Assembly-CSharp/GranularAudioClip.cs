using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class GranularAudioClip : MonoBehaviour
{
	// Token: 0x06001A9F RID: 6815 RVA: 0x000BBAA4 File Offset: 0x000B9CA4
	private void Update()
	{
		if (!this.inited && this.sourceClip.loadState == AudioDataLoadState.Loaded)
		{
			this.sampleRate = this.sourceClip.frequency;
			this.sourceAudioData = new float[this.sourceClip.samples * this.sourceClip.channels];
			this.sourceClip.GetData(this.sourceAudioData, 0);
			this.InitAudioClip();
			AudioSource component = base.GetComponent<AudioSource>();
			component.clip = this.granularClip;
			component.loop = true;
			component.Play();
			this.inited = true;
		}
		this.RefreshCachedData();
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x000BBB40 File Offset: 0x000B9D40
	private void RefreshCachedData()
	{
		this.grainAttackSamples = Mathf.FloorToInt(this.grainAttack * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainSustainSamples = Mathf.FloorToInt(this.grainSustain * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainReleaseSamples = Mathf.FloorToInt(this.grainRelease * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainFrequencySamples = Mathf.FloorToInt(this.grainFrequency * (float)this.sampleRate * (float)this.sourceChannels);
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x000BBBD4 File Offset: 0x000B9DD4
	private void InitAudioClip()
	{
		int lengthSamples = 1;
		int num = 1;
		UnityEngine.AudioSettings.GetDSPBufferSize(out lengthSamples, out num);
		this.granularClip = AudioClip.Create(this.sourceClip.name + " (granular)", lengthSamples, this.sourceClip.channels, this.sampleRate, true, new AudioClip.PCMReaderCallback(this.OnAudioRead));
		this.sourceChannels = this.sourceClip.channels;
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x000BBC40 File Offset: 0x000B9E40
	private void OnAudioRead(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (this.samplesUntilNextGrain <= 0)
			{
				this.SpawnGrain();
			}
			float num = 0f;
			for (int j = 0; j < this.grains.Count; j++)
			{
				num += this.grains[j].GetSample();
			}
			data[i] = num;
			this.samplesUntilNextGrain--;
		}
		this.CleanupFinishedGrains();
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x000BBCB4 File Offset: 0x000B9EB4
	private void SpawnGrain()
	{
		if (this.grainFrequencySamples == 0)
		{
			return;
		}
		float num = (float)(this.random.NextDouble() * (double)this.sourceTimeVariation * 2.0) - this.sourceTimeVariation;
		int start = Mathf.FloorToInt((this.sourceTime + num) * (float)this.sampleRate / (float)this.sourceChannels);
		GranularAudioClip.Grain grain = Pool.Get<GranularAudioClip.Grain>();
		grain.Init(this.sourceAudioData, start, this.grainAttackSamples, this.grainSustainSamples, this.grainReleaseSamples);
		this.grains.Add(grain);
		this.samplesUntilNextGrain = this.grainFrequencySamples;
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x000BBD4C File Offset: 0x000B9F4C
	private void CleanupFinishedGrains()
	{
		for (int i = this.grains.Count - 1; i >= 0; i--)
		{
			GranularAudioClip.Grain grain = this.grains[i];
			if (grain.finished)
			{
				Pool.Free<GranularAudioClip.Grain>(ref grain);
				this.grains.RemoveAt(i);
			}
		}
	}

	// Token: 0x04001328 RID: 4904
	public AudioClip sourceClip;

	// Token: 0x04001329 RID: 4905
	private float[] sourceAudioData;

	// Token: 0x0400132A RID: 4906
	private int sourceChannels = 1;

	// Token: 0x0400132B RID: 4907
	public AudioClip granularClip;

	// Token: 0x0400132C RID: 4908
	public int sampleRate = 44100;

	// Token: 0x0400132D RID: 4909
	public float sourceTime = 0.5f;

	// Token: 0x0400132E RID: 4910
	public float sourceTimeVariation = 0.1f;

	// Token: 0x0400132F RID: 4911
	public float grainAttack = 0.1f;

	// Token: 0x04001330 RID: 4912
	public float grainSustain = 0.1f;

	// Token: 0x04001331 RID: 4913
	public float grainRelease = 0.1f;

	// Token: 0x04001332 RID: 4914
	public float grainFrequency = 0.1f;

	// Token: 0x04001333 RID: 4915
	public int grainAttackSamples;

	// Token: 0x04001334 RID: 4916
	public int grainSustainSamples;

	// Token: 0x04001335 RID: 4917
	public int grainReleaseSamples;

	// Token: 0x04001336 RID: 4918
	public int grainFrequencySamples;

	// Token: 0x04001337 RID: 4919
	public int samplesUntilNextGrain;

	// Token: 0x04001338 RID: 4920
	public List<GranularAudioClip.Grain> grains = new List<GranularAudioClip.Grain>();

	// Token: 0x04001339 RID: 4921
	private System.Random random = new System.Random();

	// Token: 0x0400133A RID: 4922
	private bool inited;

	// Token: 0x02000C23 RID: 3107
	public class Grain
	{
		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x06004C38 RID: 19512 RVA: 0x001950A9 File Offset: 0x001932A9
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06004C39 RID: 19513 RVA: 0x001950BC File Offset: 0x001932BC
		public void Init(float[] source, int start, int attack, int sustain, int release)
		{
			this.sourceData = source;
			this.sourceDataLength = this.sourceData.Length;
			this.startSample = start;
			this.currentSample = start;
			this.attackTimeSamples = attack;
			this.sustainTimeSamples = sustain;
			this.releaseTimeSamples = release;
			this.gainPerSampleAttack = 1f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -1f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06004C3A RID: 19514 RVA: 0x00195170 File Offset: 0x00193370
		public float GetSample()
		{
			int num = this.currentSample % this.sourceDataLength;
			if (num < 0)
			{
				num += this.sourceDataLength;
			}
			float num2 = this.sourceData[num];
			if (this.currentSample <= this.attackEndSample)
			{
				this.gain += this.gainPerSampleAttack;
			}
			else if (this.currentSample >= this.releaseStartSample)
			{
				this.gain += this.gainPerSampleRelease;
			}
			this.currentSample++;
			return num2 * this.gain;
		}

		// Token: 0x040040E2 RID: 16610
		private float[] sourceData;

		// Token: 0x040040E3 RID: 16611
		private int sourceDataLength;

		// Token: 0x040040E4 RID: 16612
		private int startSample;

		// Token: 0x040040E5 RID: 16613
		private int currentSample;

		// Token: 0x040040E6 RID: 16614
		private int attackTimeSamples;

		// Token: 0x040040E7 RID: 16615
		private int sustainTimeSamples;

		// Token: 0x040040E8 RID: 16616
		private int releaseTimeSamples;

		// Token: 0x040040E9 RID: 16617
		private float gain;

		// Token: 0x040040EA RID: 16618
		private float gainPerSampleAttack;

		// Token: 0x040040EB RID: 16619
		private float gainPerSampleRelease;

		// Token: 0x040040EC RID: 16620
		private int attackEndSample;

		// Token: 0x040040ED RID: 16621
		private int releaseStartSample;

		// Token: 0x040040EE RID: 16622
		private int endSample;
	}
}
