using System;
using UnityEngine;

// Token: 0x0200095A RID: 2394
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("KriptoFX/Explosion_Bloom")]
[ImageEffectAllowedInSceneView]
public class Explosion_Bloom : MonoBehaviour
{
	// Token: 0x17000440 RID: 1088
	// (get) Token: 0x06003882 RID: 14466 RVA: 0x0014D6DA File Offset: 0x0014B8DA
	public Shader shader
	{
		get
		{
			if (this.m_Shader == null)
			{
				this.m_Shader = Shader.Find("Hidden/KriptoFX/PostEffects/Explosion_Bloom");
			}
			return this.m_Shader;
		}
	}

	// Token: 0x17000441 RID: 1089
	// (get) Token: 0x06003883 RID: 14467 RVA: 0x0014D700 File Offset: 0x0014B900
	public Material material
	{
		get
		{
			if (this.m_Material == null)
			{
				this.m_Material = Explosion_Bloom.CheckShaderAndCreateMaterial(this.shader);
			}
			return this.m_Material;
		}
	}

	// Token: 0x06003884 RID: 14468 RVA: 0x0014D728 File Offset: 0x0014B928
	public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
	{
		if (s == null || !s.isSupported)
		{
			Debug.LogWarningFormat("Missing shader for image effect {0}", new object[]
			{
				effect
			});
			return false;
		}
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", new object[]
			{
				effect
			});
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", new object[]
			{
				effect
			});
			return false;
		}
		if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", new object[]
			{
				effect
			});
			return false;
		}
		return true;
	}

	// Token: 0x06003885 RID: 14469 RVA: 0x0014D7BC File Offset: 0x0014B9BC
	public static Material CheckShaderAndCreateMaterial(Shader s)
	{
		if (s == null || !s.isSupported)
		{
			return null;
		}
		return new Material(s)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x17000442 RID: 1090
	// (get) Token: 0x06003886 RID: 14470 RVA: 0x0014D7DF File Offset: 0x0014B9DF
	public static bool supportsDX11
	{
		get
		{
			return SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
		}
	}

	// Token: 0x06003887 RID: 14471 RVA: 0x0014D7F4 File Offset: 0x0014B9F4
	private void Awake()
	{
		this.m_Threshold = Shader.PropertyToID("_Threshold");
		this.m_Curve = Shader.PropertyToID("_Curve");
		this.m_PrefilterOffs = Shader.PropertyToID("_PrefilterOffs");
		this.m_SampleScale = Shader.PropertyToID("_SampleScale");
		this.m_Intensity = Shader.PropertyToID("_Intensity");
		this.m_BaseTex = Shader.PropertyToID("_BaseTex");
	}

	// Token: 0x06003888 RID: 14472 RVA: 0x0014D861 File Offset: 0x0014BA61
	private void OnEnable()
	{
		if (!Explosion_Bloom.IsSupported(this.shader, true, false, this))
		{
			base.enabled = false;
		}
	}

	// Token: 0x06003889 RID: 14473 RVA: 0x0014D87A File Offset: 0x0014BA7A
	private void OnDisable()
	{
		if (this.m_Material != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_Material);
		}
		this.m_Material = null;
	}

	// Token: 0x0600388A RID: 14474 RVA: 0x0014D89C File Offset: 0x0014BA9C
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		bool isMobilePlatform = Application.isMobilePlatform;
		int num = source.width;
		int num2 = source.height;
		if (!this.settings.highQuality)
		{
			num /= 2;
			num2 /= 2;
		}
		RenderTextureFormat format = isMobilePlatform ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
		float num3 = Mathf.Log((float)num2, 2f) + this.settings.radius - 8f;
		int num4 = (int)num3;
		int num5 = Mathf.Clamp(num4, 1, 16);
		float thresholdLinear = this.settings.thresholdLinear;
		this.material.SetFloat(this.m_Threshold, thresholdLinear);
		float num6 = thresholdLinear * this.settings.softKnee + 1E-05f;
		Vector3 v = new Vector3(thresholdLinear - num6, num6 * 2f, 0.25f / num6);
		this.material.SetVector(this.m_Curve, v);
		bool flag = !this.settings.highQuality && this.settings.antiFlicker;
		this.material.SetFloat(this.m_PrefilterOffs, flag ? -0.5f : 0f);
		this.material.SetFloat(this.m_SampleScale, 0.5f + num3 - (float)num4);
		this.material.SetFloat(this.m_Intensity, Mathf.Max(0f, this.settings.intensity));
		RenderTexture temporary = RenderTexture.GetTemporary(num, num2, 0, format);
		Graphics.Blit(source, temporary, this.material, this.settings.antiFlicker ? 1 : 0);
		RenderTexture renderTexture = temporary;
		for (int i = 0; i < num5; i++)
		{
			this.m_blurBuffer1[i] = RenderTexture.GetTemporary(renderTexture.width / 2, renderTexture.height / 2, 0, format);
			Graphics.Blit(renderTexture, this.m_blurBuffer1[i], this.material, (i == 0) ? (this.settings.antiFlicker ? 3 : 2) : 4);
			renderTexture = this.m_blurBuffer1[i];
		}
		for (int j = num5 - 2; j >= 0; j--)
		{
			RenderTexture renderTexture2 = this.m_blurBuffer1[j];
			this.material.SetTexture(this.m_BaseTex, renderTexture2);
			this.m_blurBuffer2[j] = RenderTexture.GetTemporary(renderTexture2.width, renderTexture2.height, 0, format);
			Graphics.Blit(renderTexture, this.m_blurBuffer2[j], this.material, this.settings.highQuality ? 6 : 5);
			renderTexture = this.m_blurBuffer2[j];
		}
		int num7 = 7;
		num7 += (this.settings.highQuality ? 1 : 0);
		this.material.SetTexture(this.m_BaseTex, source);
		Graphics.Blit(renderTexture, destination, this.material, num7);
		for (int k = 0; k < 16; k++)
		{
			if (this.m_blurBuffer1[k] != null)
			{
				RenderTexture.ReleaseTemporary(this.m_blurBuffer1[k]);
			}
			if (this.m_blurBuffer2[k] != null)
			{
				RenderTexture.ReleaseTemporary(this.m_blurBuffer2[k]);
			}
			this.m_blurBuffer1[k] = null;
			this.m_blurBuffer2[k] = null;
		}
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x040032C9 RID: 13001
	[SerializeField]
	public Explosion_Bloom.Settings settings = Explosion_Bloom.Settings.defaultSettings;

	// Token: 0x040032CA RID: 13002
	[SerializeField]
	[HideInInspector]
	private Shader m_Shader;

	// Token: 0x040032CB RID: 13003
	private Material m_Material;

	// Token: 0x040032CC RID: 13004
	private const int kMaxIterations = 16;

	// Token: 0x040032CD RID: 13005
	private RenderTexture[] m_blurBuffer1 = new RenderTexture[16];

	// Token: 0x040032CE RID: 13006
	private RenderTexture[] m_blurBuffer2 = new RenderTexture[16];

	// Token: 0x040032CF RID: 13007
	private int m_Threshold;

	// Token: 0x040032D0 RID: 13008
	private int m_Curve;

	// Token: 0x040032D1 RID: 13009
	private int m_PrefilterOffs;

	// Token: 0x040032D2 RID: 13010
	private int m_SampleScale;

	// Token: 0x040032D3 RID: 13011
	private int m_Intensity;

	// Token: 0x040032D4 RID: 13012
	private int m_BaseTex;

	// Token: 0x02000E73 RID: 3699
	[Serializable]
	public struct Settings
	{
		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x06005099 RID: 20633 RVA: 0x001A20C0 File Offset: 0x001A02C0
		// (set) Token: 0x06005098 RID: 20632 RVA: 0x001A20B7 File Offset: 0x001A02B7
		public float thresholdGamma
		{
			get
			{
				return Mathf.Max(0f, this.threshold);
			}
			set
			{
				this.threshold = value;
			}
		}

		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x0600509B RID: 20635 RVA: 0x001A20E0 File Offset: 0x001A02E0
		// (set) Token: 0x0600509A RID: 20634 RVA: 0x001A20D2 File Offset: 0x001A02D2
		public float thresholdLinear
		{
			get
			{
				return Mathf.GammaToLinearSpace(this.thresholdGamma);
			}
			set
			{
				this.threshold = Mathf.LinearToGammaSpace(value);
			}
		}

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x0600509C RID: 20636 RVA: 0x001A20F0 File Offset: 0x001A02F0
		public static Explosion_Bloom.Settings defaultSettings
		{
			get
			{
				return new Explosion_Bloom.Settings
				{
					threshold = 2f,
					softKnee = 0f,
					radius = 7f,
					intensity = 0.7f,
					highQuality = true,
					antiFlicker = true
				};
			}
		}

		// Token: 0x04004A82 RID: 19074
		[SerializeField]
		[Tooltip("Filters out pixels under this level of brightness.")]
		public float threshold;

		// Token: 0x04004A83 RID: 19075
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Makes transition between under/over-threshold gradual.")]
		public float softKnee;

		// Token: 0x04004A84 RID: 19076
		[SerializeField]
		[Range(1f, 7f)]
		[Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
		public float radius;

		// Token: 0x04004A85 RID: 19077
		[SerializeField]
		[Tooltip("Blend factor of the result image.")]
		public float intensity;

		// Token: 0x04004A86 RID: 19078
		[SerializeField]
		[Tooltip("Controls filter quality and buffer resolution.")]
		public bool highQuality;

		// Token: 0x04004A87 RID: 19079
		[SerializeField]
		[Tooltip("Reduces flashing noise with an additional filter.")]
		public bool antiFlicker;
	}
}
