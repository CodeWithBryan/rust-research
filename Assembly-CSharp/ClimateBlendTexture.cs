﻿using System;
using UnityEngine;

// Token: 0x020008FA RID: 2298
public class ClimateBlendTexture : ProcessedTexture
{
	// Token: 0x060036B9 RID: 14009 RVA: 0x0014587D File Offset: 0x00143A7D
	public ClimateBlendTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/ClimateBlendLUTs");
		this.result = base.CreateRenderTexture("Climate Blend Texture", width, height, linear);
		this.result.wrapMode = TextureWrapMode.Clamp;
	}

	// Token: 0x060036BA RID: 14010 RVA: 0x001458B6 File Offset: 0x00143AB6
	public bool CheckLostData()
	{
		if (!this.result.IsCreated())
		{
			this.result.Create();
			return true;
		}
		return false;
	}

	// Token: 0x060036BB RID: 14011 RVA: 0x001458D4 File Offset: 0x00143AD4
	public void Blend(Texture srcLut1, Texture dstLut1, float lerpLut1, Texture srcLut2, Texture dstLut2, float lerpLut2, float lerp, ClimateBlendTexture prevLut, float time)
	{
		this.material.SetTexture("_srcLut1", srcLut1);
		this.material.SetTexture("_dstLut1", dstLut1);
		this.material.SetTexture("_srcLut2", srcLut2);
		this.material.SetTexture("_dstLut2", dstLut2);
		this.material.SetTexture("_prevLut", prevLut);
		this.material.SetFloat("_lerpLut1", lerpLut1);
		this.material.SetFloat("_lerpLut2", lerpLut2);
		this.material.SetFloat("_lerp", lerp);
		this.material.SetFloat("_time", time);
		Graphics.Blit(null, this.result, this.material);
	}

	// Token: 0x060036BC RID: 14012 RVA: 0x00145998 File Offset: 0x00143B98
	public static void Swap(ref ClimateBlendTexture a, ref ClimateBlendTexture b)
	{
		ClimateBlendTexture climateBlendTexture = a;
		a = b;
		b = climateBlendTexture;
	}
}
