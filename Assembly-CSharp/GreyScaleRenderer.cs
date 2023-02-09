using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200060F RID: 1551
public class GreyScaleRenderer : PostProcessEffectRenderer<GreyScale>
{
	// Token: 0x06002CD8 RID: 11480 RVA: 0x0010CC81 File Offset: 0x0010AE81
	public override void Init()
	{
		base.Init();
		this.greyScaleShader = Shader.Find("Hidden/PostProcessing/GreyScale");
	}

	// Token: 0x06002CD9 RID: 11481 RVA: 0x0010CC9C File Offset: 0x0010AE9C
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("GreyScale");
		PropertySheet propertySheet = context.propertySheets.Get(this.greyScaleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetVector(this.dataProperty, new Vector4(base.settings.redLuminance.value, base.settings.greenLuminance.value, base.settings.blueLuminance.value, base.settings.amount.value));
		propertySheet.properties.SetColor(this.colorProperty, base.settings.color.value);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("GreyScale");
	}

	// Token: 0x040024C0 RID: 9408
	private int dataProperty = Shader.PropertyToID("_data");

	// Token: 0x040024C1 RID: 9409
	private int colorProperty = Shader.PropertyToID("_color");

	// Token: 0x040024C2 RID: 9410
	private Shader greyScaleShader;
}
