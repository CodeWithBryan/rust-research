using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200021A RID: 538
public class MusicClipLoader
{
	// Token: 0x06001AAE RID: 6830 RVA: 0x000BBEEC File Offset: 0x000BA0EC
	public void Update()
	{
		for (int i = this.clipsToLoad.Count - 1; i >= 0; i--)
		{
			AudioClip audioClip = this.clipsToLoad[i];
			if (audioClip.loadState != AudioDataLoadState.Loaded && audioClip.loadState != AudioDataLoadState.Loading)
			{
				audioClip.LoadAudioData();
				this.clipsToLoad.RemoveAt(i);
				return;
			}
		}
		for (int j = this.clipsToUnload.Count - 1; j >= 0; j--)
		{
			AudioClip audioClip2 = this.clipsToUnload[j];
			if (audioClip2.loadState == AudioDataLoadState.Loaded)
			{
				audioClip2.UnloadAudioData();
				this.clipsToUnload.RemoveAt(j);
				return;
			}
		}
	}

	// Token: 0x06001AAF RID: 6831 RVA: 0x000BBF88 File Offset: 0x000BA188
	public void Refresh()
	{
		for (int i = 0; i < SingletonComponent<MusicManager>.Instance.activeMusicClips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = SingletonComponent<MusicManager>.Instance.activeMusicClips[i];
			MusicClipLoader.LoadedAudioClip loadedAudioClip = this.FindLoadedClip(positionedClip.musicClip.audioClip);
			if (loadedAudioClip == null)
			{
				loadedAudioClip = Pool.Get<MusicClipLoader.LoadedAudioClip>();
				loadedAudioClip.clip = positionedClip.musicClip.audioClip;
				loadedAudioClip.unloadTime = (float)UnityEngine.AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				this.loadedClips.Add(loadedAudioClip);
				this.loadedClipDict.Add(loadedAudioClip.clip, loadedAudioClip);
				this.clipsToLoad.Add(loadedAudioClip.clip);
			}
			else
			{
				loadedAudioClip.unloadTime = (float)UnityEngine.AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				this.clipsToUnload.Remove(loadedAudioClip.clip);
			}
		}
		for (int j = this.loadedClips.Count - 1; j >= 0; j--)
		{
			MusicClipLoader.LoadedAudioClip loadedAudioClip2 = this.loadedClips[j];
			if (UnityEngine.AudioSettings.dspTime > (double)loadedAudioClip2.unloadTime)
			{
				this.clipsToUnload.Add(loadedAudioClip2.clip);
				this.loadedClips.Remove(loadedAudioClip2);
				this.loadedClipDict.Remove(loadedAudioClip2.clip);
				Pool.Free<MusicClipLoader.LoadedAudioClip>(ref loadedAudioClip2);
			}
		}
	}

	// Token: 0x06001AB0 RID: 6832 RVA: 0x000BC0E0 File Offset: 0x000BA2E0
	private MusicClipLoader.LoadedAudioClip FindLoadedClip(AudioClip clip)
	{
		if (this.loadedClipDict.ContainsKey(clip))
		{
			return this.loadedClipDict[clip];
		}
		return null;
	}

	// Token: 0x04001355 RID: 4949
	public List<MusicClipLoader.LoadedAudioClip> loadedClips = new List<MusicClipLoader.LoadedAudioClip>();

	// Token: 0x04001356 RID: 4950
	public Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip> loadedClipDict = new Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip>();

	// Token: 0x04001357 RID: 4951
	public List<AudioClip> clipsToLoad = new List<AudioClip>();

	// Token: 0x04001358 RID: 4952
	public List<AudioClip> clipsToUnload = new List<AudioClip>();

	// Token: 0x02000C25 RID: 3109
	public class LoadedAudioClip
	{
		// Token: 0x040040F2 RID: 16626
		public AudioClip clip;

		// Token: 0x040040F3 RID: 16627
		public float unloadTime;
	}
}
