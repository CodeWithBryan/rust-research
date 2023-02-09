using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x020000F8 RID: 248
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/NVG Effect")]
public class NVGEffect : PostEffectsBase, IImageEffect
{
	// Token: 0x060014B2 RID: 5298 RVA: 0x000A3875 File Offset: 0x000A1A75
	private void Awake()
	{
		this.updateTexturesOnStartup = true;
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x000A3880 File Offset: 0x000A1A80
	private void OnDestroy()
	{
		if (this.rgbChannelTex1 != null)
		{
			UnityEngine.Object.DestroyImmediate(this.rgbChannelTex1);
			this.rgbChannelTex1 = null;
		}
		if (this.rgbChannelTex2 != null)
		{
			UnityEngine.Object.DestroyImmediate(this.rgbChannelTex2);
			this.rgbChannelTex2 = null;
		}
		if (this.material != null)
		{
			UnityEngine.Object.DestroyImmediate(this.material);
			this.material = null;
		}
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x000A38F0 File Offset: 0x000A1AF0
	private void UpdateColorCorrectionTexture(NVGEffect.ColorCorrectionParams param, ref Texture2D tex)
	{
		if (param.redChannel != null && param.greenChannel != null && param.blueChannel != null)
		{
			for (float num = 0f; num <= 1f; num += 0.003921569f)
			{
				float num2 = Mathf.Clamp(param.redChannel.Evaluate(num), 0f, 1f);
				float num3 = Mathf.Clamp(param.greenChannel.Evaluate(num), 0f, 1f);
				float num4 = Mathf.Clamp(param.blueChannel.Evaluate(num), 0f, 1f);
				tex.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num2, num2, num2));
				tex.SetPixel((int)Mathf.Floor(num * 255f), 1, new Color(num3, num3, num3));
				tex.SetPixel((int)Mathf.Floor(num * 255f), 2, new Color(num4, num4, num4));
			}
			tex.Apply();
		}
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x000A39EE File Offset: 0x000A1BEE
	public void UpdateTextures()
	{
		this.CheckResources();
		this.UpdateColorCorrectionTexture(this.ColorCorrection1, ref this.rgbChannelTex1);
		this.UpdateColorCorrectionTexture(this.ColorCorrection2, ref this.rgbChannelTex2);
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x000A3A1C File Offset: 0x000A1C1C
	public override bool CheckResources()
	{
		base.CheckSupport(false);
		this.material = base.CheckShaderAndCreateMaterial(this.Shader, this.material);
		if (this.rgbChannelTex1 == null || this.rgbChannelTex2 == null)
		{
			this.rgbChannelTex1 = new Texture2D(256, 4, TextureFormat.ARGB32, false, true)
			{
				hideFlags = HideFlags.DontSave,
				wrapMode = TextureWrapMode.Clamp
			};
			this.rgbChannelTex2 = new Texture2D(256, 4, TextureFormat.ARGB32, false, true)
			{
				hideFlags = HideFlags.DontSave,
				wrapMode = TextureWrapMode.Clamp
			};
		}
		if (!this.isSupported)
		{
			base.ReportAutoDisable();
		}
		return this.isSupported;
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x000A3ABF File Offset: 0x000A1CBF
	public bool IsActive()
	{
		return base.enabled && this.CheckResources() && this.NoiseTexture != null;
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x000A3AE0 File Offset: 0x000A1CE0
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
			if (this.NoiseTexture == null)
			{
				Debug.LogWarning("[NVGEffect] Noise & Grain effect failing as noise texture is not assigned. please assign.", base.transform);
			}
			return;
		}
		if (this.updateTexturesOnStartup)
		{
			this.UpdateTextures();
			this.updateTexturesOnStartup = false;
		}
		this.material.SetTexture("_MainTex", source);
		this.material.SetTexture("_RgbTex1", this.rgbChannelTex1);
		this.material.SetFloat("_Saturation1", this.ColorCorrection1.saturation);
		this.material.SetTexture("_RgbTex2", this.rgbChannelTex2);
		this.material.SetFloat("_Saturation2", this.ColorCorrection2.saturation);
		this.material.SetTexture("_NoiseTex", this.NoiseTexture);
		this.material.SetVector("_NoisePerChannel", this.NoiseAndGrain.monochrome ? Vector3.one : this.NoiseAndGrain.intensities);
		this.material.SetVector("_NoiseTilingPerChannel", this.NoiseAndGrain.monochrome ? (Vector3.one * this.NoiseAndGrain.monochromeTiling) : this.NoiseAndGrain.tiling);
		this.material.SetVector("_MidGrey", new Vector3(this.NoiseAndGrain.midGrey, 1f / (1f - this.NoiseAndGrain.midGrey), -1f / this.NoiseAndGrain.midGrey));
		this.material.SetVector("_NoiseAmount", new Vector3(this.NoiseAndGrain.generalIntensity, this.NoiseAndGrain.blackIntensity, this.NoiseAndGrain.whiteIntensity) * this.NoiseAndGrain.intensityMultiplier);
		if (this.NoiseTexture)
		{
			this.NoiseTexture.wrapMode = TextureWrapMode.Repeat;
			this.NoiseTexture.filterMode = this.NoiseAndGrain.filterMode;
		}
		RenderTexture.active = destination;
		float num = (float)this.NoiseTexture.width * 1f;
		float num2 = 1f * (float)source.width / NVGEffect.NOISE_TILE_AMOUNT;
		GL.PushMatrix();
		GL.LoadOrtho();
		float num3 = 1f * (float)source.width / (1f * (float)source.height);
		float num4 = 1f / num2;
		float num5 = num4 * num3;
		float num6 = num / ((float)this.NoiseTexture.width * 1f);
		this.material.SetPass(0);
		GL.Begin(7);
		for (float num7 = 0f; num7 < 1f; num7 += num4)
		{
			for (float num8 = 0f; num8 < 1f; num8 += num5)
			{
				float num9 = UnityEngine.Random.Range(0f, 1f);
				float num10 = UnityEngine.Random.Range(0f, 1f);
				num9 = Mathf.Floor(num9 * num) / num;
				num10 = Mathf.Floor(num10 * num) / num;
				float num11 = 1f / num;
				GL.MultiTexCoord2(0, num9, num10);
				GL.MultiTexCoord2(1, 0f, 0f);
				GL.Vertex3(num7, num8, 0.1f);
				GL.MultiTexCoord2(0, num9 + num6 * num11, num10);
				GL.MultiTexCoord2(1, 1f, 0f);
				GL.Vertex3(num7 + num4, num8, 0.1f);
				GL.MultiTexCoord2(0, num9 + num6 * num11, num10 + num6 * num11);
				GL.MultiTexCoord2(1, 1f, 1f);
				GL.Vertex3(num7 + num4, num8 + num5, 0.1f);
				GL.MultiTexCoord2(0, num9, num10 + num6 * num11);
				GL.MultiTexCoord2(1, 0f, 1f);
				GL.Vertex3(num7, num8 + num5, 0.1f);
			}
		}
		GL.End();
		GL.PopMatrix();
	}

	// Token: 0x04000D4D RID: 3405
	public NVGEffect.ColorCorrectionParams ColorCorrection1 = new NVGEffect.ColorCorrectionParams
	{
		saturation = 1f,
		redChannel = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		}),
		greenChannel = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		}),
		blueChannel = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		})
	};

	// Token: 0x04000D4E RID: 3406
	public NVGEffect.ColorCorrectionParams ColorCorrection2 = new NVGEffect.ColorCorrectionParams
	{
		saturation = 1f,
		redChannel = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		}),
		greenChannel = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		}),
		blueChannel = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		})
	};

	// Token: 0x04000D4F RID: 3407
	public NVGEffect.NoiseAndGrainParams NoiseAndGrain = new NVGEffect.NoiseAndGrainParams
	{
		intensityMultiplier = 1.5f,
		generalIntensity = 1f,
		blackIntensity = 1f,
		whiteIntensity = 1f,
		midGrey = 0.182f,
		monochrome = true,
		intensities = new Vector3(1f, 1f, 1f),
		tiling = new Vector3(60f, 70f, 80f),
		monochromeTiling = 55f,
		filterMode = FilterMode.Point
	};

	// Token: 0x04000D50 RID: 3408
	private Texture2D rgbChannelTex1;

	// Token: 0x04000D51 RID: 3409
	private Texture2D rgbChannelTex2;

	// Token: 0x04000D52 RID: 3410
	private bool updateTexturesOnStartup = true;

	// Token: 0x04000D53 RID: 3411
	public Texture2D NoiseTexture;

	// Token: 0x04000D54 RID: 3412
	private static float NOISE_TILE_AMOUNT = 64f;

	// Token: 0x04000D55 RID: 3413
	public Shader Shader;

	// Token: 0x04000D56 RID: 3414
	private Material material;

	// Token: 0x02000BCF RID: 3023
	[Serializable]
	public struct ColorCorrectionParams
	{
		// Token: 0x04003FB9 RID: 16313
		public float saturation;

		// Token: 0x04003FBA RID: 16314
		public AnimationCurve redChannel;

		// Token: 0x04003FBB RID: 16315
		public AnimationCurve greenChannel;

		// Token: 0x04003FBC RID: 16316
		public AnimationCurve blueChannel;
	}

	// Token: 0x02000BD0 RID: 3024
	[Serializable]
	public struct NoiseAndGrainParams
	{
		// Token: 0x04003FBD RID: 16317
		public float intensityMultiplier;

		// Token: 0x04003FBE RID: 16318
		public float generalIntensity;

		// Token: 0x04003FBF RID: 16319
		public float blackIntensity;

		// Token: 0x04003FC0 RID: 16320
		public float whiteIntensity;

		// Token: 0x04003FC1 RID: 16321
		public float midGrey;

		// Token: 0x04003FC2 RID: 16322
		public bool monochrome;

		// Token: 0x04003FC3 RID: 16323
		public Vector3 intensities;

		// Token: 0x04003FC4 RID: 16324
		public Vector3 tiling;

		// Token: 0x04003FC5 RID: 16325
		public float monochromeTiling;

		// Token: 0x04003FC6 RID: 16326
		public FilterMode filterMode;
	}
}
