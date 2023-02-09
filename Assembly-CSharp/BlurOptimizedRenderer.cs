using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020005FC RID: 1532
public class BlurOptimizedRenderer : PostProcessEffectRenderer<BlurOptimized>
{
	// Token: 0x06002CB5 RID: 11445 RVA: 0x0010B969 File Offset: 0x00109B69
	public override void Init()
	{
		base.Init();
		this.blurShader = Shader.Find("Hidden/PostProcessing/BlurOptimized");
	}

	// Token: 0x06002CB6 RID: 11446 RVA: 0x0010B984 File Offset: 0x00109B84
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("BlurOptimized");
		int value = base.settings.downsample.value;
		float value2 = base.settings.fadeToBlurDistance.value;
		float value3 = base.settings.blurSize.value;
		int value4 = base.settings.blurIterations.value;
		bool value5 = base.settings.blurType.value != BlurType.StandardGauss;
		float num = 1f / (1f * (float)(1 << value));
		float z = 1f / Mathf.Clamp(value2, 0.001f, 10000f);
		PropertySheet propertySheet = context.propertySheets.Get(this.blurShader);
		propertySheet.properties.SetVector("_Parameter", new Vector4(value3 * num, -value3 * num, z, 0f));
		int width = context.width >> value;
		int height = context.height >> value;
		int nameID = Shader.PropertyToID("_BlurRT1");
		int nameID2 = Shader.PropertyToID("_BlurRT2");
		command.GetTemporaryRT(nameID, width, height, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Default);
		command.BlitFullscreenTriangle(context.source, nameID, propertySheet, 0, false, null);
		int num2 = (!value5) ? 0 : 2;
		for (int i = 0; i < value4; i++)
		{
			float num3 = (float)i * 1f;
			propertySheet.properties.SetVector("_Parameter", new Vector4(value3 * num + num3, -value3 * num - num3, z, 0f));
			command.GetTemporaryRT(nameID2, width, height, 0, FilterMode.Bilinear, context.sourceFormat);
			command.BlitFullscreenTriangle(nameID, nameID2, propertySheet, 1 + num2, false, null);
			command.ReleaseTemporaryRT(nameID);
			command.GetTemporaryRT(nameID, width, height, 0, FilterMode.Bilinear, context.sourceFormat);
			command.BlitFullscreenTriangle(nameID2, nameID, propertySheet, 2 + num2, false, null);
			command.ReleaseTemporaryRT(nameID2);
		}
		if (value2 <= 0f)
		{
			command.BlitFullscreenTriangle(nameID, context.destination, false, null);
		}
		else
		{
			command.SetGlobalTexture("_Source", context.source);
			command.BlitFullscreenTriangle(nameID, context.destination, propertySheet, 5, false, null);
		}
		command.ReleaseTemporaryRT(nameID);
		command.EndSample("BlurOptimized");
	}

	// Token: 0x04002484 RID: 9348
	private int dataProperty = Shader.PropertyToID("_data");

	// Token: 0x04002485 RID: 9349
	private Shader blurShader;
}
