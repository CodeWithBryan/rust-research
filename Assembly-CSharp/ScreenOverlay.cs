using System;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000616 RID: 1558
[PostProcess(typeof(ScreenOverlayRenderer), PostProcessEvent.AfterStack, "Custom/ScreenOverlay", true)]
[Serializable]
public class ScreenOverlay : PostProcessEffectSettings
{
	// Token: 0x040024D8 RID: 9432
	public OverlayBlendModeParameter blendMode = new OverlayBlendModeParameter
	{
		value = OverlayBlendMode.Multiply
	};

	// Token: 0x040024D9 RID: 9433
	public FloatParameter intensity = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040024DA RID: 9434
	public TextureParameter texture = new TextureParameter
	{
		value = null
	};

	// Token: 0x040024DB RID: 9435
	public TextureParameter normals = new TextureParameter
	{
		value = null
	};
}
