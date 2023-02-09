using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000613 RID: 1555
public class PhotoFilterRenderer : PostProcessEffectRenderer<PhotoFilter>
{
	// Token: 0x06002CE0 RID: 11488 RVA: 0x0010D2FA File Offset: 0x0010B4FA
	public override void Init()
	{
		base.Init();
		this.greyScaleShader = Shader.Find("Hidden/PostProcessing/PhotoFilter");
	}

	// Token: 0x06002CE1 RID: 11489 RVA: 0x0010D314 File Offset: 0x0010B514
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("PhotoFilter");
		PropertySheet propertySheet = context.propertySheets.Get(this.greyScaleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetColor(this.rgbProperty, base.settings.color.value);
		propertySheet.properties.SetFloat(this.densityProperty, base.settings.density.value);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("PhotoFilter");
	}

	// Token: 0x040024CE RID: 9422
	private int rgbProperty = Shader.PropertyToID("_rgb");

	// Token: 0x040024CF RID: 9423
	private int densityProperty = Shader.PropertyToID("_density");

	// Token: 0x040024D0 RID: 9424
	private Shader greyScaleShader;
}
