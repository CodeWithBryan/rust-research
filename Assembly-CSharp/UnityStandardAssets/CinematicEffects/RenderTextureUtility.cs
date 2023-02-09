using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	// Token: 0x020009D3 RID: 2515
	public class RenderTextureUtility
	{
		// Token: 0x06003B60 RID: 15200 RVA: 0x0015B678 File Offset: 0x00159878
		public RenderTexture GetTemporaryRenderTexture(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.ARGBHalf, FilterMode filterMode = FilterMode.Bilinear)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format);
			temporary.filterMode = filterMode;
			temporary.wrapMode = TextureWrapMode.Clamp;
			temporary.name = "RenderTextureUtilityTempTexture";
			this.m_TemporaryRTs.Add(temporary);
			return temporary;
		}

		// Token: 0x06003B61 RID: 15201 RVA: 0x0015B6B8 File Offset: 0x001598B8
		public void ReleaseTemporaryRenderTexture(RenderTexture rt)
		{
			if (rt == null)
			{
				return;
			}
			if (!this.m_TemporaryRTs.Contains(rt))
			{
				Debug.LogErrorFormat("Attempting to remove texture that was not allocated: {0}", new object[]
				{
					rt
				});
				return;
			}
			this.m_TemporaryRTs.Remove(rt);
			RenderTexture.ReleaseTemporary(rt);
		}

		// Token: 0x06003B62 RID: 15202 RVA: 0x0015B708 File Offset: 0x00159908
		public void ReleaseAllTemporaryRenderTextures()
		{
			for (int i = 0; i < this.m_TemporaryRTs.Count; i++)
			{
				RenderTexture.ReleaseTemporary(this.m_TemporaryRTs[i]);
			}
			this.m_TemporaryRTs.Clear();
		}

		// Token: 0x0400353C RID: 13628
		private List<RenderTexture> m_TemporaryRTs = new List<RenderTexture>();
	}
}
