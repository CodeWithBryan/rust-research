using System;
using ConVar;
using UnityEngine;

// Token: 0x02000968 RID: 2408
[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class NGSS_Directional : MonoBehaviour
{
	// Token: 0x060038C6 RID: 14534 RVA: 0x0014ECE4 File Offset: 0x0014CEE4
	private void Update()
	{
		bool globalSettings = ConVar.Graphics.shadowquality >= 2;
		this.SetGlobalSettings(globalSettings);
	}

	// Token: 0x060038C7 RID: 14535 RVA: 0x0014ED04 File Offset: 0x0014CF04
	private void SetGlobalSettings(bool enabled)
	{
		if (enabled)
		{
			Shader.SetGlobalFloat("NGSS_PCSS_GLOBAL_SOFTNESS", this.PCSS_GLOBAL_SOFTNESS);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MIN", (this.PCSS_FILTER_DIR_MIN > this.PCSS_FILTER_DIR_MAX) ? this.PCSS_FILTER_DIR_MAX : this.PCSS_FILTER_DIR_MIN);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MAX", (this.PCSS_FILTER_DIR_MAX < this.PCSS_FILTER_DIR_MIN) ? this.PCSS_FILTER_DIR_MIN : this.PCSS_FILTER_DIR_MAX);
			Shader.SetGlobalFloat("NGSS_POISSON_SAMPLING_NOISE_DIR", this.BANDING_NOISE_AMOUNT);
		}
	}

	// Token: 0x0400332B RID: 13099
	[Tooltip("Overall softness for both PCF and PCSS shadows.\nRecommended value: 0.01.")]
	[Range(0f, 0.02f)]
	public float PCSS_GLOBAL_SOFTNESS = 0.01f;

	// Token: 0x0400332C RID: 13100
	[Tooltip("PCSS softness when shadows is close to caster.\nRecommended value: 0.05.")]
	[Range(0f, 1f)]
	public float PCSS_FILTER_DIR_MIN = 0.05f;

	// Token: 0x0400332D RID: 13101
	[Tooltip("PCSS softness when shadows is far from caster.\nRecommended value: 0.25.\nIf too high can lead to visible artifacts when early bailout is enabled.")]
	[Range(0f, 0.5f)]
	public float PCSS_FILTER_DIR_MAX = 0.25f;

	// Token: 0x0400332E RID: 13102
	[Tooltip("Amount of banding or noise. Example: 0.0 gives 100 % Banding and 10.0 gives 100 % Noise.")]
	[Range(0f, 10f)]
	public float BANDING_NOISE_AMOUNT = 1f;

	// Token: 0x0400332F RID: 13103
	[Tooltip("Recommended values: Mobile = 16, Consoles = 25, Desktop Low = 32, Desktop High = 64")]
	public NGSS_Directional.SAMPLER_COUNT SAMPLERS_COUNT;

	// Token: 0x02000E75 RID: 3701
	public enum SAMPLER_COUNT
	{
		// Token: 0x04004A8C RID: 19084
		SAMPLERS_16,
		// Token: 0x04004A8D RID: 19085
		SAMPLERS_25,
		// Token: 0x04004A8E RID: 19086
		SAMPLERS_32,
		// Token: 0x04004A8F RID: 19087
		SAMPLERS_64
	}
}
