using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021C RID: 540
[CreateAssetMenu(menuName = "Rust/MusicTheme")]
public class MusicTheme : ScriptableObject
{
	// Token: 0x17000209 RID: 521
	// (get) Token: 0x06001AB7 RID: 6839 RVA: 0x000BC1C6 File Offset: 0x000BA3C6
	public int layerCount
	{
		get
		{
			return this.layers.Count;
		}
	}

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x06001AB8 RID: 6840 RVA: 0x000BC1D3 File Offset: 0x000BA3D3
	public int samplesPerBar
	{
		get
		{
			return MusicUtil.BarsToSamples(this.tempo, 1f, 44100);
		}
	}

	// Token: 0x06001AB9 RID: 6841 RVA: 0x000BC1EC File Offset: 0x000BA3EC
	private void OnValidate()
	{
		this.audioClipDict.Clear();
		this.activeClips.Clear();
		this.UpdateLengthInBars();
		for (int i = 0; i < this.clips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.clips[i];
			int num = this.ActiveClipCollectionID(positionedClip.startingBar - 8);
			int num2 = this.ActiveClipCollectionID(positionedClip.endingBar);
			for (int j = num; j <= num2; j++)
			{
				if (!this.activeClips.ContainsKey(j))
				{
					this.activeClips.Add(j, new List<MusicTheme.PositionedClip>());
				}
				if (!this.activeClips[j].Contains(positionedClip))
				{
					this.activeClips[j].Add(positionedClip);
				}
			}
			if (positionedClip.musicClip != null)
			{
				AudioClip audioClip = positionedClip.musicClip.audioClip;
				if (!this.audioClipDict.ContainsKey(audioClip))
				{
					this.audioClipDict.Add(audioClip, true);
				}
				if (positionedClip.startingBar < 8 && !this.firstAudioClips.Contains(audioClip))
				{
					this.firstAudioClips.Add(audioClip);
				}
				positionedClip.musicClip.lengthInBarsWithTail = Mathf.CeilToInt(MusicUtil.SecondsToBars(this.tempo, (double)positionedClip.musicClip.audioClip.length));
			}
		}
	}

	// Token: 0x06001ABA RID: 6842 RVA: 0x000BC334 File Offset: 0x000BA534
	public List<MusicTheme.PositionedClip> GetActiveClipsForBar(int bar)
	{
		int key = this.ActiveClipCollectionID(bar);
		if (!this.activeClips.ContainsKey(key))
		{
			return null;
		}
		return this.activeClips[key];
	}

	// Token: 0x06001ABB RID: 6843 RVA: 0x000BC365 File Offset: 0x000BA565
	private int ActiveClipCollectionID(int bar)
	{
		return Mathf.FloorToInt(Mathf.Max((float)(bar / 4), 0f));
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x000BC37A File Offset: 0x000BA57A
	public MusicTheme.Layer LayerById(int id)
	{
		if (this.layers.Count <= id)
		{
			return null;
		}
		return this.layers[id];
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x000BC398 File Offset: 0x000BA598
	public void AddLayer()
	{
		MusicTheme.Layer layer = new MusicTheme.Layer();
		layer.name = "layer " + this.layers.Count;
		this.layers.Add(layer);
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x000BC3D8 File Offset: 0x000BA5D8
	private void UpdateLengthInBars()
	{
		int num = 0;
		for (int i = 0; i < this.clips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.clips[i];
			if (!(positionedClip.musicClip == null))
			{
				int num2 = positionedClip.startingBar + positionedClip.musicClip.lengthInBars;
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		this.lengthInBars = num;
	}

	// Token: 0x06001ABF RID: 6847 RVA: 0x000BC438 File Offset: 0x000BA638
	public bool CanPlayInEnvironment(int currentBiome, int currentTopology, float currentRain, float currentSnow, float currentWind)
	{
		return (!TOD_Sky.Instance || this.time.Evaluate(TOD_Sky.Instance.Cycle.Hour) >= 0f) && (this.biomes == (TerrainBiome.Enum)(-1) || (this.biomes & (TerrainBiome.Enum)currentBiome) != (TerrainBiome.Enum)0) && (this.topologies == (TerrainTopology.Enum)(-1) || (this.topologies & (TerrainTopology.Enum)currentTopology) == (TerrainTopology.Enum)0) && ((this.rain.min <= 0f && this.rain.max >= 1f) || currentRain >= this.rain.min) && currentRain <= this.rain.max && ((this.snow.min <= 0f && this.snow.max >= 1f) || currentSnow >= this.snow.min) && currentSnow <= this.snow.max && ((this.wind.min <= 0f && this.wind.max >= 1f) || currentWind >= this.wind.min) && currentWind <= this.wind.max;
	}

	// Token: 0x06001AC0 RID: 6848 RVA: 0x000BC56C File Offset: 0x000BA76C
	public bool FirstClipsLoaded()
	{
		for (int i = 0; i < this.firstAudioClips.Count; i++)
		{
			if (this.firstAudioClips[i].loadState != AudioDataLoadState.Loaded)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001AC1 RID: 6849 RVA: 0x000BC5A6 File Offset: 0x000BA7A6
	public bool ContainsAudioClip(AudioClip clip)
	{
		return this.audioClipDict.ContainsKey(clip);
	}

	// Token: 0x04001373 RID: 4979
	[Header("Basic info")]
	public float tempo = 80f;

	// Token: 0x04001374 RID: 4980
	public int intensityHoldBars = 4;

	// Token: 0x04001375 RID: 4981
	public int lengthInBars;

	// Token: 0x04001376 RID: 4982
	[Header("Playback restrictions")]
	public bool canPlayInMenus = true;

	// Token: 0x04001377 RID: 4983
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange rain = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x04001378 RID: 4984
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange wind = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x04001379 RID: 4985
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange snow = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x0400137A RID: 4986
	[InspectorFlags]
	public TerrainBiome.Enum biomes = (TerrainBiome.Enum)(-1);

	// Token: 0x0400137B RID: 4987
	[InspectorFlags]
	public TerrainTopology.Enum topologies = (TerrainTopology.Enum)(-1);

	// Token: 0x0400137C RID: 4988
	public AnimationCurve time = AnimationCurve.Linear(0f, 0f, 24f, 0f);

	// Token: 0x0400137D RID: 4989
	[Header("Clip data")]
	public List<MusicTheme.PositionedClip> clips = new List<MusicTheme.PositionedClip>();

	// Token: 0x0400137E RID: 4990
	public List<MusicTheme.Layer> layers = new List<MusicTheme.Layer>();

	// Token: 0x0400137F RID: 4991
	private Dictionary<int, List<MusicTheme.PositionedClip>> activeClips = new Dictionary<int, List<MusicTheme.PositionedClip>>();

	// Token: 0x04001380 RID: 4992
	private List<AudioClip> firstAudioClips = new List<AudioClip>();

	// Token: 0x04001381 RID: 4993
	private Dictionary<AudioClip, bool> audioClipDict = new Dictionary<AudioClip, bool>();

	// Token: 0x02000C27 RID: 3111
	[Serializable]
	public class Layer
	{
		// Token: 0x040040FB RID: 16635
		public string name = "layer";
	}

	// Token: 0x02000C28 RID: 3112
	[Serializable]
	public class PositionedClip
	{
		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x06004C40 RID: 19520 RVA: 0x0019521F File Offset: 0x0019341F
		public int endingBar
		{
			get
			{
				if (!(this.musicClip == null))
				{
					return this.startingBar + this.musicClip.lengthInBarsWithTail;
				}
				return this.startingBar;
			}
		}

		// Token: 0x06004C41 RID: 19521 RVA: 0x00195248 File Offset: 0x00193448
		public bool CanPlay(float intensity)
		{
			return (intensity > this.minIntensity || (this.minIntensity == 0f && intensity == 0f)) && intensity <= this.maxIntensity;
		}

		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x06004C42 RID: 19522 RVA: 0x00195276 File Offset: 0x00193476
		public bool isControlClip
		{
			get
			{
				return this.musicClip == null;
			}
		}

		// Token: 0x06004C43 RID: 19523 RVA: 0x00195284 File Offset: 0x00193484
		public void CopySettingsFrom(MusicTheme.PositionedClip otherClip)
		{
			if (this.isControlClip != otherClip.isControlClip)
			{
				return;
			}
			if (otherClip == this)
			{
				return;
			}
			this.allowFadeIn = otherClip.allowFadeIn;
			this.fadeInTime = otherClip.fadeInTime;
			this.allowFadeOut = otherClip.allowFadeOut;
			this.fadeOutTime = otherClip.fadeOutTime;
			this.maxIntensity = otherClip.maxIntensity;
			this.minIntensity = otherClip.minIntensity;
			this.intensityReduction = otherClip.intensityReduction;
		}

		// Token: 0x040040FC RID: 16636
		public MusicTheme theme;

		// Token: 0x040040FD RID: 16637
		public MusicClip musicClip;

		// Token: 0x040040FE RID: 16638
		public int startingBar;

		// Token: 0x040040FF RID: 16639
		public int layerId;

		// Token: 0x04004100 RID: 16640
		public float minIntensity;

		// Token: 0x04004101 RID: 16641
		public float maxIntensity = 1f;

		// Token: 0x04004102 RID: 16642
		public bool allowFadeIn = true;

		// Token: 0x04004103 RID: 16643
		public bool allowFadeOut = true;

		// Token: 0x04004104 RID: 16644
		public float fadeInTime = 1f;

		// Token: 0x04004105 RID: 16645
		public float fadeOutTime = 0.5f;

		// Token: 0x04004106 RID: 16646
		public float intensityReduction;

		// Token: 0x04004107 RID: 16647
		public int jumpBarCount;

		// Token: 0x04004108 RID: 16648
		public float jumpMinimumIntensity = 0.5f;

		// Token: 0x04004109 RID: 16649
		public float jumpMaximumIntensity = 0.5f;
	}

	// Token: 0x02000C29 RID: 3113
	[Serializable]
	public class ValueRange
	{
		// Token: 0x06004C45 RID: 19525 RVA: 0x00195354 File Offset: 0x00193554
		public ValueRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		// Token: 0x0400410A RID: 16650
		public float min;

		// Token: 0x0400410B RID: 16651
		public float max;
	}
}
