using System;
using System.Collections.Generic;
using JSON;
using UnityEngine;

// Token: 0x02000211 RID: 529
public class EngineAudioClip : MonoBehaviour, IClientComponent
{
	// Token: 0x06001A9B RID: 6811 RVA: 0x000BB9FF File Offset: 0x000B9BFF
	private int GetBucketRPM(int RPM)
	{
		return Mathf.RoundToInt((float)(RPM / 25)) * 25;
	}

	// Token: 0x0400130C RID: 4876
	public AudioClip granularClip;

	// Token: 0x0400130D RID: 4877
	public AudioClip accelerationClip;

	// Token: 0x0400130E RID: 4878
	public TextAsset accelerationCyclesJson;

	// Token: 0x0400130F RID: 4879
	public List<EngineAudioClip.EngineCycle> accelerationCycles = new List<EngineAudioClip.EngineCycle>();

	// Token: 0x04001310 RID: 4880
	public List<EngineAudioClip.EngineCycleBucket> cycleBuckets = new List<EngineAudioClip.EngineCycleBucket>();

	// Token: 0x04001311 RID: 4881
	public Dictionary<int, EngineAudioClip.EngineCycleBucket> accelerationCyclesByRPM = new Dictionary<int, EngineAudioClip.EngineCycleBucket>();

	// Token: 0x04001312 RID: 4882
	public Dictionary<int, int> rpmBucketLookup = new Dictionary<int, int>();

	// Token: 0x04001313 RID: 4883
	public int sampleRate = 44100;

	// Token: 0x04001314 RID: 4884
	public int samplesUntilNextGrain;

	// Token: 0x04001315 RID: 4885
	public int lastCycleId;

	// Token: 0x04001316 RID: 4886
	public List<EngineAudioClip.Grain> grains = new List<EngineAudioClip.Grain>();

	// Token: 0x04001317 RID: 4887
	public int currentRPM;

	// Token: 0x04001318 RID: 4888
	public int targetRPM = 1500;

	// Token: 0x04001319 RID: 4889
	public int minRPM;

	// Token: 0x0400131A RID: 4890
	public int maxRPM;

	// Token: 0x0400131B RID: 4891
	public int cyclePadding;

	// Token: 0x0400131C RID: 4892
	[Range(0f, 1f)]
	public float RPMControl;

	// Token: 0x0400131D RID: 4893
	public AudioSource source;

	// Token: 0x0400131E RID: 4894
	public float rpmLerpSpeed = 0.025f;

	// Token: 0x0400131F RID: 4895
	public float rpmLerpSpeedDown = 0.01f;

	// Token: 0x02000C1F RID: 3103
	[Serializable]
	public class EngineCycle
	{
		// Token: 0x06004C2F RID: 19503 RVA: 0x00194E00 File Offset: 0x00193000
		public EngineCycle(int RPM, int startSample, int endSample, float period, int id)
		{
			this.RPM = RPM;
			this.startSample = startSample;
			this.endSample = endSample;
			this.period = period;
			this.id = id;
		}

		// Token: 0x040040CA RID: 16586
		public int RPM;

		// Token: 0x040040CB RID: 16587
		public int startSample;

		// Token: 0x040040CC RID: 16588
		public int endSample;

		// Token: 0x040040CD RID: 16589
		public float period;

		// Token: 0x040040CE RID: 16590
		public int id;
	}

	// Token: 0x02000C20 RID: 3104
	public class EngineCycleBucket
	{
		// Token: 0x06004C30 RID: 19504 RVA: 0x00194E2D File Offset: 0x0019302D
		public EngineCycleBucket(int RPM)
		{
			this.RPM = RPM;
		}

		// Token: 0x06004C31 RID: 19505 RVA: 0x00194E54 File Offset: 0x00193054
		public EngineAudioClip.EngineCycle GetCycle(System.Random random, int lastCycleId)
		{
			if (this.remainingCycles.Count == 0)
			{
				this.ResetRemainingCycles(random);
			}
			int index = this.remainingCycles.Pop<int>();
			if (this.cycles[index].id == lastCycleId)
			{
				if (this.remainingCycles.Count == 0)
				{
					this.ResetRemainingCycles(random);
				}
				index = this.remainingCycles.Pop<int>();
			}
			return this.cycles[index];
		}

		// Token: 0x06004C32 RID: 19506 RVA: 0x00194EC4 File Offset: 0x001930C4
		private void ResetRemainingCycles(System.Random random)
		{
			for (int i = 0; i < this.cycles.Count; i++)
			{
				this.remainingCycles.Add(i);
			}
			this.remainingCycles.Shuffle((uint)random.Next());
		}

		// Token: 0x06004C33 RID: 19507 RVA: 0x00194F04 File Offset: 0x00193104
		public void Add(EngineAudioClip.EngineCycle cycle)
		{
			if (!this.cycles.Contains(cycle))
			{
				this.cycles.Add(cycle);
			}
		}

		// Token: 0x040040CF RID: 16591
		public int RPM;

		// Token: 0x040040D0 RID: 16592
		public List<EngineAudioClip.EngineCycle> cycles = new List<EngineAudioClip.EngineCycle>();

		// Token: 0x040040D1 RID: 16593
		public List<int> remainingCycles = new List<int>();
	}

	// Token: 0x02000C21 RID: 3105
	public class Grain
	{
		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x06004C34 RID: 19508 RVA: 0x00194F20 File Offset: 0x00193120
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06004C35 RID: 19509 RVA: 0x00194F34 File Offset: 0x00193134
		public void Init(float[] source, EngineAudioClip.EngineCycle cycle, int cyclePadding)
		{
			this.sourceData = source;
			this.startSample = cycle.startSample - cyclePadding;
			this.currentSample = this.startSample;
			this.attackTimeSamples = cyclePadding;
			this.sustainTimeSamples = cycle.endSample - cycle.startSample;
			this.releaseTimeSamples = cyclePadding;
			this.gainPerSampleAttack = 1f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -1f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06004C36 RID: 19510 RVA: 0x00194FF0 File Offset: 0x001931F0
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
				if (this.gain > 0.8f)
				{
					this.gain = 0.8f;
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

		// Token: 0x040040D2 RID: 16594
		private float[] sourceData;

		// Token: 0x040040D3 RID: 16595
		private int startSample;

		// Token: 0x040040D4 RID: 16596
		private int currentSample;

		// Token: 0x040040D5 RID: 16597
		private int attackTimeSamples;

		// Token: 0x040040D6 RID: 16598
		private int sustainTimeSamples;

		// Token: 0x040040D7 RID: 16599
		private int releaseTimeSamples;

		// Token: 0x040040D8 RID: 16600
		private float gain;

		// Token: 0x040040D9 RID: 16601
		private float gainPerSampleAttack;

		// Token: 0x040040DA RID: 16602
		private float gainPerSampleRelease;

		// Token: 0x040040DB RID: 16603
		private int attackEndSample;

		// Token: 0x040040DC RID: 16604
		private int releaseStartSample;

		// Token: 0x040040DD RID: 16605
		private int endSample;
	}
}
