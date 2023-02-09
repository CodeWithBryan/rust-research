using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000603 RID: 1539
public class DoubleVisionRenderer : PostProcessEffectRenderer<DoubleVision>
{
	// Token: 0x06002CC1 RID: 11457 RVA: 0x0010C0EE File Offset: 0x0010A2EE
	public override void Init()
	{
		base.Init();
		this.doubleVisionShader = Shader.Find("Hidden/PostProcessing/DoubleVision");
	}

	// Token: 0x06002CC2 RID: 11458 RVA: 0x0010C108 File Offset: 0x0010A308
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("DoubleVision");
		PropertySheet propertySheet = context.propertySheets.Get(this.doubleVisionShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetVector(this.displaceProperty, base.settings.displace.value);
		propertySheet.properties.SetFloat(this.amountProperty, base.settings.amount.value);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("DoubleVision");
	}

	// Token: 0x04002498 RID: 9368
	private int displaceProperty = Shader.PropertyToID("_displace");

	// Token: 0x04002499 RID: 9369
	private int amountProperty = Shader.PropertyToID("_amount");

	// Token: 0x0400249A RID: 9370
	private Shader doubleVisionShader;
}
