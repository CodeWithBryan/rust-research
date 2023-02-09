using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000607 RID: 1543
public class FrostRenderer : PostProcessEffectRenderer<Frost>
{
	// Token: 0x06002CCC RID: 11468 RVA: 0x0010C443 File Offset: 0x0010A643
	public override void Init()
	{
		base.Init();
		this.frostShader = Shader.Find("Hidden/PostProcessing/Frost");
	}

	// Token: 0x06002CCD RID: 11469 RVA: 0x0010C45C File Offset: 0x0010A65C
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("Frost");
		PropertySheet propertySheet = context.propertySheets.Get(this.frostShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetFloat(this.scaleProperty, base.settings.scale.value);
		propertySheet.properties.SetFloat(this.sharpnessProperty, base.settings.sharpness.value * 0.01f);
		propertySheet.properties.SetFloat(this.darknessProperty, base.settings.darkness.value * 0.02f);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, base.settings.enableVignette.value ? 1 : 0, false, null);
		command.EndSample("Frost");
	}

	// Token: 0x040024A4 RID: 9380
	private int scaleProperty = Shader.PropertyToID("_scale");

	// Token: 0x040024A5 RID: 9381
	private int sharpnessProperty = Shader.PropertyToID("_sharpness");

	// Token: 0x040024A6 RID: 9382
	private int darknessProperty = Shader.PropertyToID("_darkness");

	// Token: 0x040024A7 RID: 9383
	private Shader frostShader;
}
