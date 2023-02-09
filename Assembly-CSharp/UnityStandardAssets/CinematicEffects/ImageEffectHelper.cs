using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	// Token: 0x020009D1 RID: 2513
	public static class ImageEffectHelper
	{
		// Token: 0x06003B5C RID: 15196 RVA: 0x0015B5CC File Offset: 0x001597CC
		public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
		{
			if (s == null || !s.isSupported)
			{
				Debug.LogWarningFormat("Missing shader for image effect {0}", new object[]
				{
					effect
				});
				return false;
			}
			if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
			{
				Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", new object[]
				{
					effect
				});
				return false;
			}
			if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
			{
				Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", new object[]
				{
					effect
				});
				return false;
			}
			if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
			{
				Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", new object[]
				{
					effect
				});
				return false;
			}
			return true;
		}

		// Token: 0x06003B5D RID: 15197 RVA: 0x0014D7BC File Offset: 0x0014B9BC
		public static Material CheckShaderAndCreateMaterial(Shader s)
		{
			if (s == null || !s.isSupported)
			{
				return null;
			}
			return new Material(s)
			{
				hideFlags = HideFlags.DontSave
			};
		}

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x06003B5E RID: 15198 RVA: 0x0014D7DF File Offset: 0x0014B9DF
		public static bool supportsDX11
		{
			get
			{
				return SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
			}
		}
	}
}
