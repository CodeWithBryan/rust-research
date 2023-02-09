using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000619 RID: 1561
public class SharpenAndVignetteRenderer : PostProcessEffectRenderer<SharpenAndVignette>
{
	// Token: 0x06002CE9 RID: 11497 RVA: 0x0010D693 File Offset: 0x0010B893
	public override void Init()
	{
		base.Init();
		this.sharpenAndVigenetteShader = Shader.Find("Hidden/PostProcessing/SharpenAndVignette");
	}

	// Token: 0x06002CEA RID: 11498 RVA: 0x0010D6AC File Offset: 0x0010B8AC
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("SharpenAndVignette");
		PropertySheet propertySheet = context.propertySheets.Get(this.sharpenAndVigenetteShader);
		propertySheet.properties.Clear();
		bool value = base.settings.applySharpen.value;
		bool value2 = base.settings.applyVignette.value;
		if (value)
		{
			propertySheet.properties.SetFloat("_px", 1f / (float)Screen.width);
			propertySheet.properties.SetFloat("_py", 1f / (float)Screen.height);
			propertySheet.properties.SetFloat("_strength", base.settings.strength.value);
			propertySheet.properties.SetFloat("_clamp", base.settings.clamp.value);
		}
		if (value2)
		{
			propertySheet.properties.SetFloat("_sharpness", base.settings.sharpness.value * 0.01f);
			propertySheet.properties.SetFloat("_darkness", base.settings.darkness.value * 0.02f);
		}
		if (value && !value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		}
		else if (value && value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 1, false, null);
		}
		else if (!value && value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 2, false, null);
		}
		else
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		}
		command.EndSample("SharpenAndVignette");
	}

	// Token: 0x040024E3 RID: 9443
	private Shader sharpenAndVigenetteShader;
}
