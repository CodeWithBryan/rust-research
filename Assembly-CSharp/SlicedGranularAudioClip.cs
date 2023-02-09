using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class SlicedGranularAudioClip : MonoBehaviour, IClientComponent
{
	// Token: 0x0400139B RID: 5019
	public AudioClip sourceClip;

	// Token: 0x0400139C RID: 5020
	public AudioClip granularClip;

	// Token: 0x0400139D RID: 5021
	public int sampleRate = 44100;

	// Token: 0x0400139E RID: 5022
	public float grainAttack = 0.1f;

	// Token: 0x0400139F RID: 5023
	public float grainSustain = 0.1f;

	// Token: 0x040013A0 RID: 5024
	public float grainRelease = 0.1f;

	// Token: 0x040013A1 RID: 5025
	public float grainFrequency = 0.1f;

	// Token: 0x040013A2 RID: 5026
	public int grainAttackSamples;

	// Token: 0x040013A3 RID: 5027
	public int grainSustainSamples;

	// Token: 0x040013A4 RID: 5028
	public int grainReleaseSamples;

	// Token: 0x040013A5 RID: 5029
	public int grainFrequencySamples;

	// Token: 0x040013A6 RID: 5030
	public int samplesUntilNextGrain;

	// Token: 0x040013A7 RID: 5031
	public List<SlicedGranularAudioClip.Grain> grains = new List<SlicedGranularAudioClip.Grain>();

	// Token: 0x040013A8 RID: 5032
	public List<int> startPositions = new List<int>();

	// Token: 0x040013A9 RID: 5033
	public int lastStartPositionIdx = int.MaxValue;

	// Token: 0x02000C2A RID: 3114
	public class Grain
	{
		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x06004C46 RID: 19526 RVA: 0x0019536A File Offset: 0x0019356A
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06004C47 RID: 19527 RVA: 0x00195380 File Offset: 0x00193580
		public void Init(float[] source, int start, int attack, int sustain, int release)
		{
			this.sourceData = source;
			this.startSample = start;
			this.currentSample = start;
			this.attackTimeSamples = attack;
			this.sustainTimeSamples = sustain;
			this.releaseTimeSamples = release;
			this.gainPerSampleAttack = 0.5f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -0.5f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06004C48 RID: 19528 RVA: 0x00195424 File Offset: 0x00193624
		public float GetSample()
		{
			if (this.currentSample >= this.sourceData.Length)
			{
				return 0f;
			}
			float num = this.sourceData[this.currentSample];
			if (this.currentSample <= this.attackEndSample)
			{
				this.gain += this.gainPerSampleAttack;
				if (this.gain > 0.5f)
				{
					this.gain = 0.5f;
				}
			}
			else if (this.currentSample >= this.releaseStartSample)
			{
				this.gain += this.gainPerSampleRelease;
				if (this.gain < 0f)
				{
					this.gain = 0f;
				}
			}
			this.currentSample++;
			return num * this.gain;
		}

		// Token: 0x06004C49 RID: 19529 RVA: 0x001954DD File Offset: 0x001936DD
		public void FadeOut()
		{
			this.releaseStartSample = this.currentSample;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
		}

		// Token: 0x0400410C RID: 16652
		private float[] sourceData;

		// Token: 0x0400410D RID: 16653
		private int startSample;

		// Token: 0x0400410E RID: 16654
		private int currentSample;

		// Token: 0x0400410F RID: 16655
		private int attackTimeSamples;

		// Token: 0x04004110 RID: 16656
		private int sustainTimeSamples;

		// Token: 0x04004111 RID: 16657
		private int releaseTimeSamples;

		// Token: 0x04004112 RID: 16658
		private float gain;

		// Token: 0x04004113 RID: 16659
		private float gainPerSampleAttack;

		// Token: 0x04004114 RID: 16660
		private float gainPerSampleRelease;

		// Token: 0x04004115 RID: 16661
		private int attackEndSample;

		// Token: 0x04004116 RID: 16662
		private int releaseStartSample;

		// Token: 0x04004117 RID: 16663
		private int endSample;
	}
}
