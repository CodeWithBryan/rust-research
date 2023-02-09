using System;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class Sound : MonoBehaviour, IClientComponent
{
	// Token: 0x1700020B RID: 523
	// (get) Token: 0x06001AD9 RID: 6873 RVA: 0x000BC7FB File Offset: 0x000BA9FB
	public SoundFade fade
	{
		get
		{
			return this._fade;
		}
	}

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x06001ADA RID: 6874 RVA: 0x000BC803 File Offset: 0x000BAA03
	public SoundModulation modulation
	{
		get
		{
			return this._modulation;
		}
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x06001ADB RID: 6875 RVA: 0x000BC80B File Offset: 0x000BAA0B
	public SoundOcclusion occlusion
	{
		get
		{
			return this._occlusion;
		}
	}

	// Token: 0x040013AA RID: 5034
	public static float volumeExponent = Mathf.Log(Mathf.Sqrt(10f), 2f);

	// Token: 0x040013AB RID: 5035
	public SoundDefinition definition;

	// Token: 0x040013AC RID: 5036
	public SoundModifier[] modifiers;

	// Token: 0x040013AD RID: 5037
	public SoundSource soundSource;

	// Token: 0x040013AE RID: 5038
	public AudioSource[] audioSources = new AudioSource[2];

	// Token: 0x040013AF RID: 5039
	[SerializeField]
	private SoundFade _fade;

	// Token: 0x040013B0 RID: 5040
	[SerializeField]
	private SoundModulation _modulation;

	// Token: 0x040013B1 RID: 5041
	[SerializeField]
	private SoundOcclusion _occlusion;
}
