using System;
using ConVar;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000276 RID: 630
public class AudioSettings : MonoBehaviour
{
	// Token: 0x06001BD9 RID: 7129 RVA: 0x000C133C File Offset: 0x000BF53C
	private void Update()
	{
		if (this.mixer == null)
		{
			return;
		}
		this.mixer.SetFloat("MasterVol", this.LinearToDecibel(Audio.master));
		float a;
		this.mixer.GetFloat("MusicVol", out a);
		if (!LevelManager.isLoaded || !MainCamera.isValid)
		{
			this.mixer.SetFloat("MusicVol", Mathf.Lerp(a, this.LinearToDecibel(Audio.musicvolumemenu), UnityEngine.Time.deltaTime));
		}
		else
		{
			this.mixer.SetFloat("MusicVol", Mathf.Lerp(a, this.LinearToDecibel(Audio.musicvolume), UnityEngine.Time.deltaTime));
		}
		float num = 1f - ((SingletonComponent<MixerSnapshotManager>.Instance == null) ? 0f : SingletonComponent<MixerSnapshotManager>.Instance.deafness);
		this.mixer.SetFloat("WorldVol", this.LinearToDecibel(Audio.game * num));
		this.mixer.SetFloat("WorldVolFlashbang", this.LinearToDecibel(Audio.game));
		this.mixer.SetFloat("VoiceVol", this.LinearToDecibel(Audio.voices * num));
		this.mixer.SetFloat("InstrumentVol", this.LinearToDecibel(Audio.instruments * num));
		float num2 = this.LinearToDecibel(Audio.voiceProps * num) - 28.7f;
		this.mixer.SetFloat("VoicePropsVol", num2 * num);
		this.mixer.SetFloat("SeasonalEventsVol", this.LinearToDecibel(Audio.eventAudio * num));
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x000C14C8 File Offset: 0x000BF6C8
	private float LinearToDecibel(float linear)
	{
		float result;
		if (linear > 0f)
		{
			result = 20f * Mathf.Log10(linear);
		}
		else
		{
			result = -144f;
		}
		return result;
	}

	// Token: 0x040014FE RID: 5374
	public AudioMixer mixer;
}
