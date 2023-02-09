using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000707 RID: 1799
public static class CommandBufferEx
{
	// Token: 0x060031D4 RID: 12756 RVA: 0x00131F70 File Offset: 0x00130170
	public static void BlitArray(this CommandBuffer cb, Mesh blitMesh, RenderTargetIdentifier source, Material mat, int slice, int pass = 0)
	{
		cb.SetGlobalTexture("_Source", source);
		cb.SetGlobalFloat("_SourceMip", 0f);
		if (slice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)slice);
			cb.SetGlobalInt("_TargetSlice", slice);
		}
		cb.DrawMesh(blitMesh, Matrix4x4.identity, mat, 0, pass);
	}

	// Token: 0x060031D5 RID: 12757 RVA: 0x00131FCC File Offset: 0x001301CC
	public static void BlitArray(this CommandBuffer cb, Mesh blitMesh, RenderTargetIdentifier source, Texture target, Material mat, int slice, int pass = 0)
	{
		cb.SetRenderTarget(target, 0, CubemapFace.PositiveX, -1);
		cb.SetGlobalTexture("_Source", source);
		cb.SetGlobalFloat("_SourceMip", 0f);
		if (slice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)slice);
			cb.SetGlobalInt("_TargetSlice", slice);
		}
		cb.DrawMesh(blitMesh, Matrix4x4.identity, mat, 0, pass);
	}

	// Token: 0x060031D6 RID: 12758 RVA: 0x00132038 File Offset: 0x00130238
	public static void BlitArrayMip(this CommandBuffer cb, Mesh blitMesh, Texture source, int sourceMip, int sourceSlice, Texture target, int targetMip, int targetSlice, Material mat, int pass = 0)
	{
		int num = source.width >> sourceMip;
		int num2 = source.height >> sourceMip;
		Vector4 value = new Vector4(1f / (float)num, 1f / (float)num2, (float)num, (float)num2);
		int num3 = target.width >> targetMip;
		int num4 = target.height >> targetMip;
		Vector4 value2 = new Vector4(1f / (float)num3, 1f / (float)num4, (float)num3, (float)num4);
		cb.SetGlobalTexture("_Source", source);
		cb.SetGlobalVector("_Source_TexelSize", value);
		cb.SetGlobalVector("_Target_TexelSize", value2);
		cb.SetGlobalFloat("_SourceMip", (float)sourceMip);
		if (sourceSlice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)sourceSlice);
			cb.SetGlobalInt("_TargetSlice", targetSlice);
		}
		cb.SetRenderTarget(target, targetMip, CubemapFace.PositiveX, -1);
		cb.DrawMesh(blitMesh, Matrix4x4.identity, mat, 0, pass);
	}

	// Token: 0x060031D7 RID: 12759 RVA: 0x0013212C File Offset: 0x0013032C
	public static void BlitMip(this CommandBuffer cb, Mesh blitMesh, Texture source, Texture target, int mip, int slice, Material mat, int pass = 0)
	{
		cb.BlitArrayMip(blitMesh, source, mip, slice, target, mip, slice, mat, pass);
	}
}
