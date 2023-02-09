using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200060D RID: 1549
public class GodRaysRenderer : PostProcessEffectRenderer<GodRays>
{
	// Token: 0x06002CD2 RID: 11474 RVA: 0x0010C62C File Offset: 0x0010A82C
	public override void Init()
	{
		if (!this.GodRayShader)
		{
			this.GodRayShader = Shader.Find("Hidden/PostProcessing/GodRays");
		}
		if (!this.ScreenClearShader)
		{
			this.ScreenClearShader = Shader.Find("Hidden/PostProcessing/ScreenClear");
		}
		if (!this.SkyMaskShader)
		{
			this.SkyMaskShader = Shader.Find("Hidden/PostProcessing/SkyMask");
		}
	}

	// Token: 0x06002CD3 RID: 11475 RVA: 0x0010C690 File Offset: 0x0010A890
	private void DrawBorder(PostProcessRenderContext context, RenderTargetIdentifier buffer1)
	{
		PropertySheet propertySheet = context.propertySheets.Get(this.ScreenClearShader);
		Rect value = new Rect(0f, (float)(context.height - 1), (float)context.width, 1f);
		Rect value2 = new Rect(0f, 0f, (float)context.width, 1f);
		Rect value3 = new Rect(0f, 0f, 1f, (float)context.height);
		Rect value4 = new Rect((float)(context.width - 1), 0f, 1f, (float)context.height);
		context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, buffer1, propertySheet, 0, false, new Rect?(value));
		context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, buffer1, propertySheet, 0, false, new Rect?(value2));
		context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, buffer1, propertySheet, 0, false, new Rect?(value3));
		context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, buffer1, propertySheet, 0, false, new Rect?(value4));
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x0010C798 File Offset: 0x0010A998
	private int GetSkyMask(PostProcessRenderContext context, ResolutionType resolution, Vector3 lightPos, int blurIterations, float blurRadius, float maxRadius)
	{
		CommandBuffer command = context.command;
		Camera camera = context.camera;
		PropertySheet propertySheet = context.propertySheets.Get(this.SkyMaskShader);
		command.BeginSample("GodRays");
		int width;
		int height;
		int depthBuffer;
		if (resolution == ResolutionType.High)
		{
			width = context.screenWidth;
			height = context.screenHeight;
			depthBuffer = 0;
		}
		else if (resolution == ResolutionType.Normal)
		{
			width = context.screenWidth / 2;
			height = context.screenHeight / 2;
			depthBuffer = 0;
		}
		else
		{
			width = context.screenWidth / 4;
			height = context.screenHeight / 4;
			depthBuffer = 0;
		}
		int num = Shader.PropertyToID("buffer1");
		int nameID = Shader.PropertyToID("buffer2");
		command.GetTemporaryRT(num, width, height, depthBuffer);
		propertySheet.properties.SetVector("_BlurRadius4", new Vector4(1f, 1f, 0f, 0f) * blurRadius);
		propertySheet.properties.SetVector("_LightPosition", new Vector4(lightPos.x, lightPos.y, lightPos.z, maxRadius));
		if ((camera.depthTextureMode & DepthTextureMode.Depth) != DepthTextureMode.None)
		{
			command.BlitFullscreenTriangle(context.source, num, propertySheet, 1, false, null);
		}
		else
		{
			command.BlitFullscreenTriangle(context.source, num, propertySheet, 2, false, null);
		}
		if (camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Mono)
		{
			this.DrawBorder(context, num);
		}
		float num2 = blurRadius * 0.0013020834f;
		propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num2, num2, 0f, 0f));
		propertySheet.properties.SetVector("_LightPosition", new Vector4(lightPos.x, lightPos.y, lightPos.z, maxRadius));
		for (int i = 0; i < blurIterations; i++)
		{
			command.GetTemporaryRT(nameID, width, height, depthBuffer);
			command.BlitFullscreenTriangle(num, nameID, propertySheet, 0, false, null);
			command.ReleaseTemporaryRT(num);
			num2 = blurRadius * (((float)i * 2f + 1f) * 6f) / 768f;
			propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num2, num2, 0f, 0f));
			command.GetTemporaryRT(num, width, height, depthBuffer);
			command.BlitFullscreenTriangle(nameID, num, propertySheet, 0, false, null);
			command.ReleaseTemporaryRT(nameID);
			num2 = blurRadius * (((float)i * 2f + 2f) * 6f) / 768f;
			propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num2, num2, 0f, 0f));
		}
		command.EndSample("GodRays");
		return num;
	}

	// Token: 0x06002CD5 RID: 11477 RVA: 0x0010CA68 File Offset: 0x0010AC68
	public override void Render(PostProcessRenderContext context)
	{
		Camera camera = context.camera;
		TOD_Sky instance = TOD_Sky.Instance;
		if (instance == null)
		{
			return;
		}
		Vector3 vector = camera.WorldToViewportPoint(instance.Components.LightTransform.position);
		CommandBuffer command = context.command;
		PropertySheet propertySheet = context.propertySheets.Get(this.GodRayShader);
		int skyMask = this.GetSkyMask(context, base.settings.Resolution.value, vector, base.settings.BlurIterations.value, base.settings.BlurRadius.value, base.settings.MaxRadius.value);
		Color value = Color.black;
		if ((double)vector.z >= 0.0)
		{
			if (instance.IsDay)
			{
				value = base.settings.Intensity.value * instance.SunVisibility * instance.SunRayColor;
			}
			else
			{
				value = base.settings.Intensity.value * instance.MoonVisibility * instance.MoonRayColor;
			}
		}
		propertySheet.properties.SetColor("_LightColor", value);
		command.SetGlobalTexture("_SkyMask", skyMask);
		if (base.settings.BlendMode.value == BlendModeType.Screen)
		{
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		}
		else
		{
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 1, false, null);
		}
		command.ReleaseTemporaryRT(skyMask);
	}

	// Token: 0x040024B6 RID: 9398
	private const int PASS_SCREEN = 0;

	// Token: 0x040024B7 RID: 9399
	private const int PASS_ADD = 1;

	// Token: 0x040024B8 RID: 9400
	public Shader GodRayShader;

	// Token: 0x040024B9 RID: 9401
	public Shader ScreenClearShader;

	// Token: 0x040024BA RID: 9402
	public Shader SkyMaskShader;
}
