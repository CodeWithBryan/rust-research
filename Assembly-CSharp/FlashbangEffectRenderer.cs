using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000605 RID: 1541
public class FlashbangEffectRenderer : PostProcessEffectRenderer<FlashbangEffect>
{
	// Token: 0x06002CC5 RID: 11461 RVA: 0x0010C20E File Offset: 0x0010A40E
	public override void Init()
	{
		base.Init();
		this.flashbangEffectShader = Shader.Find("Hidden/PostProcessing/FlashbangEffect");
	}

	// Token: 0x06002CC6 RID: 11462 RVA: 0x0010C228 File Offset: 0x0010A428
	public override void Render(PostProcessRenderContext context)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		CommandBuffer command = context.command;
		FlashbangEffectRenderer.CheckCreateRenderTexture(ref this.screenRT, "Flashbang", context.width, context.height, context.sourceFormat);
		command.BeginSample("FlashbangEffect");
		if (FlashbangEffectRenderer.needsCapture)
		{
			command.CopyTexture(context.source, this.screenRT);
			FlashbangEffectRenderer.needsCapture = false;
		}
		PropertySheet propertySheet = context.propertySheets.Get(this.flashbangEffectShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetFloat("_BurnIntensity", base.settings.burnIntensity.value);
		propertySheet.properties.SetFloat("_WhiteoutIntensity", base.settings.whiteoutIntensity.value);
		if (this.screenRT)
		{
			propertySheet.properties.SetTexture("_BurnOverlay", this.screenRT);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("FlashbangEffect");
	}

	// Token: 0x06002CC7 RID: 11463 RVA: 0x0010C343 File Offset: 0x0010A543
	public override void Release()
	{
		base.Release();
		FlashbangEffectRenderer.SafeDestroyRenderTexture(ref this.screenRT);
	}

	// Token: 0x06002CC8 RID: 11464 RVA: 0x0010C358 File Offset: 0x0010A558
	private static void CheckCreateRenderTexture(ref RenderTexture rt, string name, int width, int height, RenderTextureFormat format)
	{
		if (rt == null || rt.width != width || rt.height != height)
		{
			FlashbangEffectRenderer.SafeDestroyRenderTexture(ref rt);
			rt = new RenderTexture(width, height, 0, format)
			{
				hideFlags = HideFlags.DontSave
			};
			rt.name = name;
			rt.wrapMode = TextureWrapMode.Clamp;
			rt.Create();
		}
	}

	// Token: 0x06002CC9 RID: 11465 RVA: 0x0010C3B5 File Offset: 0x0010A5B5
	private static void SafeDestroyRenderTexture(ref RenderTexture rt)
	{
		if (rt != null)
		{
			rt.Release();
			UnityEngine.Object.DestroyImmediate(rt);
			rt = null;
		}
	}

	// Token: 0x0400249D RID: 9373
	public static bool needsCapture;

	// Token: 0x0400249E RID: 9374
	private Shader flashbangEffectShader;

	// Token: 0x0400249F RID: 9375
	private RenderTexture screenRT;
}
