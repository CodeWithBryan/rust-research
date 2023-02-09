using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200061B RID: 1563
public class WiggleRenderer : PostProcessEffectRenderer<Wiggle>
{
	// Token: 0x06002CED RID: 11501 RVA: 0x0010D8B0 File Offset: 0x0010BAB0
	public override void Init()
	{
		base.Init();
		this.wiggleShader = Shader.Find("Hidden/PostProcessing/Wiggle");
	}

	// Token: 0x06002CEE RID: 11502 RVA: 0x0010D8C8 File Offset: 0x0010BAC8
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("Wiggle");
		this.timer += base.settings.speed.value * Time.deltaTime;
		PropertySheet propertySheet = context.propertySheets.Get(this.wiggleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetFloat(this.timerProperty, this.timer);
		propertySheet.properties.SetFloat(this.scaleProperty, base.settings.scale.value);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("Wiggle");
	}

	// Token: 0x040024E6 RID: 9446
	private int timerProperty = Shader.PropertyToID("_timer");

	// Token: 0x040024E7 RID: 9447
	private int scaleProperty = Shader.PropertyToID("_scale");

	// Token: 0x040024E8 RID: 9448
	private Shader wiggleShader;

	// Token: 0x040024E9 RID: 9449
	private float timer;
}
