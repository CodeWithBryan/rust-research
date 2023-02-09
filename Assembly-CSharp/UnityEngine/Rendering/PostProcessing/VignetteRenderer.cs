using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A32 RID: 2610
	[Preserve]
	internal sealed class VignetteRenderer : PostProcessEffectRenderer<Vignette>
	{
		// Token: 0x06003DD6 RID: 15830 RVA: 0x0016C214 File Offset: 0x0016A414
		public override void Render(PostProcessRenderContext context)
		{
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword("VIGNETTE");
			uberSheet.properties.SetColor(ShaderIDs.Vignette_Color, base.settings.color.value);
			if (base.settings.mode == VignetteMode.Classic)
			{
				uberSheet.properties.SetFloat(ShaderIDs.Vignette_Mode, 0f);
				uberSheet.properties.SetVector(ShaderIDs.Vignette_Center, base.settings.center.value);
				float z = (1f - base.settings.roundness.value) * 6f + base.settings.roundness.value;
				uberSheet.properties.SetVector(ShaderIDs.Vignette_Settings, new Vector4(base.settings.intensity.value * 3f, base.settings.smoothness.value * 5f, z, base.settings.rounded.value ? 1f : 0f));
				return;
			}
			uberSheet.properties.SetFloat(ShaderIDs.Vignette_Mode, 1f);
			uberSheet.properties.SetTexture(ShaderIDs.Vignette_Mask, base.settings.mask.value);
			uberSheet.properties.SetFloat(ShaderIDs.Vignette_Opacity, Mathf.Clamp01(base.settings.opacity.value));
		}
	}
}
