using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A22 RID: 2594
	[Preserve]
	internal sealed class LensDistortionRenderer : PostProcessEffectRenderer<LensDistortion>
	{
		// Token: 0x06003D8D RID: 15757 RVA: 0x0016941C File Offset: 0x0016761C
		public override void Render(PostProcessRenderContext context)
		{
			PropertySheet uberSheet = context.uberSheet;
			float val = 1.6f * Math.Max(Mathf.Abs(base.settings.intensity.value), 1f);
			float num = 0.017453292f * Math.Min(160f, val);
			float y = 2f * Mathf.Tan(num * 0.5f);
			Vector4 value = new Vector4(base.settings.centerX.value, base.settings.centerY.value, Mathf.Max(base.settings.intensityX.value, 0.0001f), Mathf.Max(base.settings.intensityY.value, 0.0001f));
			Vector4 value2 = new Vector4((base.settings.intensity.value >= 0f) ? num : (1f / num), y, 1f / base.settings.scale.value, base.settings.intensity.value);
			uberSheet.EnableKeyword("DISTORT");
			uberSheet.properties.SetVector(ShaderIDs.Distortion_CenterScale, value);
			uberSheet.properties.SetVector(ShaderIDs.Distortion_Amount, value2);
		}
	}
}
