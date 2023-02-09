using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A20 RID: 2592
	[Preserve]
	internal sealed class GrainRenderer : PostProcessEffectRenderer<Grain>
	{
		// Token: 0x06003D87 RID: 15751 RVA: 0x0016909C File Offset: 0x0016729C
		public override void Render(PostProcessRenderContext context)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float z = HaltonSeq.Get(this.m_SampleIndex & 1023, 2);
			float w = HaltonSeq.Get(this.m_SampleIndex & 1023, 3);
			int num = this.m_SampleIndex + 1;
			this.m_SampleIndex = num;
			if (num >= 1024)
			{
				this.m_SampleIndex = 0;
			}
			if (this.m_GrainLookupRT == null || !this.m_GrainLookupRT.IsCreated())
			{
				RuntimeUtilities.Destroy(this.m_GrainLookupRT);
				this.m_GrainLookupRT = new RenderTexture(128, 128, 0, this.GetLookupFormat())
				{
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Repeat,
					anisoLevel = 0,
					name = "Grain Lookup Texture"
				};
				this.m_GrainLookupRT.Create();
			}
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.grainBaker);
			propertySheet.properties.Clear();
			propertySheet.properties.SetFloat(ShaderIDs.Phase, realtimeSinceStartup % 10f);
			propertySheet.properties.SetVector(ShaderIDs.GrainNoiseParameters, new Vector3(12.9898f, 78.233f, 43758.547f));
			context.command.BeginSample("GrainLookup");
			context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, this.m_GrainLookupRT, propertySheet, base.settings.colored.value ? 1 : 0, false, null);
			context.command.EndSample("GrainLookup");
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword("GRAIN");
			uberSheet.properties.SetTexture(ShaderIDs.GrainTex, this.m_GrainLookupRT);
			uberSheet.properties.SetVector(ShaderIDs.Grain_Params1, new Vector2(base.settings.lumContrib.value, base.settings.intensity.value * 20f));
			uberSheet.properties.SetVector(ShaderIDs.Grain_Params2, new Vector4((float)context.width / (float)this.m_GrainLookupRT.width / base.settings.size.value, (float)context.height / (float)this.m_GrainLookupRT.height / base.settings.size.value, z, w));
		}

		// Token: 0x06003D88 RID: 15752 RVA: 0x001692F1 File Offset: 0x001674F1
		private RenderTextureFormat GetLookupFormat()
		{
			if (RenderTextureFormat.ARGBHalf.IsSupported())
			{
				return RenderTextureFormat.ARGBHalf;
			}
			return RenderTextureFormat.ARGB32;
		}

		// Token: 0x06003D89 RID: 15753 RVA: 0x001692FE File Offset: 0x001674FE
		public override void Release()
		{
			RuntimeUtilities.Destroy(this.m_GrainLookupRT);
			this.m_GrainLookupRT = null;
			this.m_SampleIndex = 0;
		}

		// Token: 0x040036CE RID: 14030
		private RenderTexture m_GrainLookupRT;

		// Token: 0x040036CF RID: 14031
		private const int k_SampleCount = 1024;

		// Token: 0x040036D0 RID: 14032
		private int m_SampleIndex;
	}
}
