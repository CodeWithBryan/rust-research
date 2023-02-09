using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000617 RID: 1559
public class ScreenOverlayRenderer : PostProcessEffectRenderer<ScreenOverlay>
{
	// Token: 0x06002CE5 RID: 11493 RVA: 0x0010D44B File Offset: 0x0010B64B
	public override void Init()
	{
		base.Init();
		this.overlayShader = Shader.Find("Hidden/PostProcessing/ScreenOverlay");
	}

	// Token: 0x06002CE6 RID: 11494 RVA: 0x0010D464 File Offset: 0x0010B664
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("ScreenOverlay");
		PropertySheet propertySheet = context.propertySheets.Get(this.overlayShader);
		propertySheet.properties.Clear();
		Vector4 value = new Vector4(1f, 0f, 0f, 1f);
		propertySheet.properties.SetVector("_UV_Transform", value);
		propertySheet.properties.SetFloat("_Intensity", base.settings.intensity);
		if (TOD_Sky.Instance)
		{
			propertySheet.properties.SetVector("_LightDir", context.camera.transform.InverseTransformDirection(TOD_Sky.Instance.LightDirection));
			propertySheet.properties.SetColor("_LightCol", TOD_Sky.Instance.LightColor * TOD_Sky.Instance.LightIntensity);
		}
		if (base.settings.texture.value)
		{
			propertySheet.properties.SetTexture("_Overlay", base.settings.texture.value);
		}
		if (base.settings.normals.value)
		{
			propertySheet.properties.SetTexture("_Normals", base.settings.normals.value);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, (int)base.settings.blendMode.value, false, null);
		command.EndSample("ScreenOverlay");
	}

	// Token: 0x040024DC RID: 9436
	private Shader overlayShader;
}
