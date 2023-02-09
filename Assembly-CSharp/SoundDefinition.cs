using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class SoundDefinition : ScriptableObject
{
	// Token: 0x1700020E RID: 526
	// (get) Token: 0x06001AE0 RID: 6880 RVA: 0x000BC890 File Offset: 0x000BAA90
	public float maxDistance
	{
		get
		{
			if (this.template == null)
			{
				return 0f;
			}
			AudioSource component = this.template.Get().GetComponent<AudioSource>();
			if (component == null)
			{
				return 0f;
			}
			return component.maxDistance;
		}
	}

	// Token: 0x06001AE1 RID: 6881 RVA: 0x000BC8D4 File Offset: 0x000BAAD4
	public float GetLength()
	{
		float num = 0f;
		for (int i = 0; i < this.weightedAudioClips.Count; i++)
		{
			AudioClip audioClip = this.weightedAudioClips[i].audioClip;
			if (audioClip)
			{
				num = Mathf.Max(audioClip.length, num);
			}
		}
		for (int j = 0; j < this.distanceAudioClips.Count; j++)
		{
			List<WeightedAudioClip> audioClips = this.distanceAudioClips[j].audioClips;
			for (int k = 0; k < audioClips.Count; k++)
			{
				AudioClip audioClip2 = audioClips[k].audioClip;
				if (audioClip2)
				{
					num = Mathf.Max(audioClip2.length, num);
				}
			}
		}
		float num2 = 1f / (this.pitch - this.pitchVariation);
		return num * num2;
	}

	// Token: 0x06001AE2 RID: 6882 RVA: 0x0002A0CF File Offset: 0x000282CF
	public Sound Play()
	{
		return null;
	}

	// Token: 0x06001AE3 RID: 6883 RVA: 0x0002A0CF File Offset: 0x000282CF
	public Sound Play(GameObject forGameObject)
	{
		return null;
	}

	// Token: 0x040013BD RID: 5053
	public GameObjectRef template;

	// Token: 0x040013BE RID: 5054
	[Horizontal(2, -1)]
	public List<WeightedAudioClip> weightedAudioClips = new List<WeightedAudioClip>
	{
		new WeightedAudioClip()
	};

	// Token: 0x040013BF RID: 5055
	public List<SoundDefinition.DistanceAudioClipList> distanceAudioClips;

	// Token: 0x040013C0 RID: 5056
	public SoundClass soundClass;

	// Token: 0x040013C1 RID: 5057
	public bool defaultToFirstPerson;

	// Token: 0x040013C2 RID: 5058
	public bool loop;

	// Token: 0x040013C3 RID: 5059
	public bool randomizeStartPosition;

	// Token: 0x040013C4 RID: 5060
	public bool useHighQualityFades;

	// Token: 0x040013C5 RID: 5061
	[Range(0f, 1f)]
	public float volume = 1f;

	// Token: 0x040013C6 RID: 5062
	[Range(0f, 1f)]
	public float volumeVariation;

	// Token: 0x040013C7 RID: 5063
	[Range(-3f, 3f)]
	public float pitch = 1f;

	// Token: 0x040013C8 RID: 5064
	[Range(0f, 1f)]
	public float pitchVariation;

	// Token: 0x040013C9 RID: 5065
	[Header("Voice limiting")]
	public bool dontVoiceLimit;

	// Token: 0x040013CA RID: 5066
	public int globalVoiceMaxCount = 100;

	// Token: 0x040013CB RID: 5067
	public int localVoiceMaxCount = 100;

	// Token: 0x040013CC RID: 5068
	public float localVoiceRange = 10f;

	// Token: 0x040013CD RID: 5069
	public float voiceLimitFadeOutTime = 0.05f;

	// Token: 0x040013CE RID: 5070
	public float localVoiceDebounceTime = 0.1f;

	// Token: 0x040013CF RID: 5071
	[Header("Occlusion Settings")]
	public bool forceOccludedPlayback;

	// Token: 0x040013D0 RID: 5072
	[Header("Doppler")]
	public bool enableDoppler;

	// Token: 0x040013D1 RID: 5073
	public float dopplerAmount = 0.18f;

	// Token: 0x040013D2 RID: 5074
	public float dopplerScale = 1f;

	// Token: 0x040013D3 RID: 5075
	public float dopplerAdjustmentRate = 1f;

	// Token: 0x040013D4 RID: 5076
	[Header("Custom curves")]
	public AnimationCurve falloffCurve;

	// Token: 0x040013D5 RID: 5077
	public bool useCustomFalloffCurve;

	// Token: 0x040013D6 RID: 5078
	public AnimationCurve spatialBlendCurve;

	// Token: 0x040013D7 RID: 5079
	public bool useCustomSpatialBlendCurve;

	// Token: 0x040013D8 RID: 5080
	public AnimationCurve spreadCurve;

	// Token: 0x040013D9 RID: 5081
	public bool useCustomSpreadCurve;

	// Token: 0x02000C2B RID: 3115
	[Serializable]
	public class DistanceAudioClipList
	{
		// Token: 0x04004118 RID: 16664
		public int distance;

		// Token: 0x04004119 RID: 16665
		[Horizontal(2, -1)]
		public List<WeightedAudioClip> audioClips;
	}
}
