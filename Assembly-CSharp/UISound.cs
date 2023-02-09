using System;
using ConVar;
using UnityEngine;

// Token: 0x02000873 RID: 2163
public static class UISound
{
	// Token: 0x06003558 RID: 13656 RVA: 0x001413BC File Offset: 0x0013F5BC
	private static AudioSource GetAudioSource()
	{
		if (UISound.source != null)
		{
			return UISound.source;
		}
		UISound.source = new GameObject("UISound").AddComponent<AudioSource>();
		UISound.source.spatialBlend = 0f;
		UISound.source.volume = 1f;
		return UISound.source;
	}

	// Token: 0x06003559 RID: 13657 RVA: 0x00141413 File Offset: 0x0013F613
	public static void Play(AudioClip clip, float volume = 1f)
	{
		if (clip == null)
		{
			return;
		}
		UISound.GetAudioSource().volume = volume * Audio.master * 0.4f;
		UISound.GetAudioSource().PlayOneShot(clip);
	}

	// Token: 0x0400300B RID: 12299
	private static AudioSource source;
}
