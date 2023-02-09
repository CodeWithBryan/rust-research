using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200021B RID: 539
public class MusicManager : SingletonComponent<MusicManager>, IClientComponent
{
	// Token: 0x17000207 RID: 519
	// (get) Token: 0x06001AB2 RID: 6834 RVA: 0x000BC132 File Offset: 0x000BA332
	public double currentThemeTime
	{
		get
		{
			return UnityEngine.AudioSettings.dspTime - this.themeStartTime;
		}
	}

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x06001AB3 RID: 6835 RVA: 0x000BC140 File Offset: 0x000BA340
	public int themeBar
	{
		get
		{
			return this.currentBar + this.barOffset;
		}
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void RaiseIntensityTo(float amount, int holdLengthBars = 0)
	{
	}

	// Token: 0x06001AB5 RID: 6837 RVA: 0x000059DD File Offset: 0x00003BDD
	public void StopMusic()
	{
	}

	// Token: 0x04001359 RID: 4953
	public AudioMixerGroup mixerGroup;

	// Token: 0x0400135A RID: 4954
	public List<MusicTheme> themes;

	// Token: 0x0400135B RID: 4955
	public MusicTheme currentTheme;

	// Token: 0x0400135C RID: 4956
	public List<AudioSource> sources = new List<AudioSource>();

	// Token: 0x0400135D RID: 4957
	public double nextMusic;

	// Token: 0x0400135E RID: 4958
	public double nextMusicFromIntensityRaise;

	// Token: 0x0400135F RID: 4959
	[Range(0f, 1f)]
	public float intensity;

	// Token: 0x04001360 RID: 4960
	public Dictionary<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData> clipPlaybackData = new Dictionary<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData>();

	// Token: 0x04001361 RID: 4961
	public int holdIntensityUntilBar;

	// Token: 0x04001362 RID: 4962
	public bool musicPlaying;

	// Token: 0x04001363 RID: 4963
	public bool loadingFirstClips;

	// Token: 0x04001364 RID: 4964
	public MusicTheme nextTheme;

	// Token: 0x04001365 RID: 4965
	public double lastClipUpdate;

	// Token: 0x04001366 RID: 4966
	public float clipUpdateInterval = 0.1f;

	// Token: 0x04001367 RID: 4967
	public double themeStartTime;

	// Token: 0x04001368 RID: 4968
	public int lastActiveClipRefresh = -10;

	// Token: 0x04001369 RID: 4969
	public int activeClipRefreshInterval = 1;

	// Token: 0x0400136A RID: 4970
	public bool forceThemeChange;

	// Token: 0x0400136B RID: 4971
	public float randomIntensityJumpChance;

	// Token: 0x0400136C RID: 4972
	public int clipScheduleBarsEarly = 1;

	// Token: 0x0400136D RID: 4973
	public List<MusicTheme.PositionedClip> activeClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x0400136E RID: 4974
	public List<MusicTheme.PositionedClip> activeMusicClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x0400136F RID: 4975
	public List<MusicTheme.PositionedClip> activeControlClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x04001370 RID: 4976
	public List<MusicZone> currentMusicZones = new List<MusicZone>();

	// Token: 0x04001371 RID: 4977
	public int currentBar;

	// Token: 0x04001372 RID: 4978
	public int barOffset;

	// Token: 0x02000C26 RID: 3110
	[Serializable]
	public class ClipPlaybackData
	{
		// Token: 0x040040F4 RID: 16628
		public AudioSource source;

		// Token: 0x040040F5 RID: 16629
		public MusicTheme.PositionedClip positionedClip;

		// Token: 0x040040F6 RID: 16630
		public bool isActive;

		// Token: 0x040040F7 RID: 16631
		public bool fadingIn;

		// Token: 0x040040F8 RID: 16632
		public bool fadingOut;

		// Token: 0x040040F9 RID: 16633
		public double fadeStarted;

		// Token: 0x040040FA RID: 16634
		public bool needsSync;
	}
}
