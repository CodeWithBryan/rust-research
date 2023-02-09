using System;
using UnityEngine;

// Token: 0x02000959 RID: 2393
public class FXAAPostEffectsBase : MonoBehaviour
{
	// Token: 0x06003875 RID: 14453 RVA: 0x0014D1B8 File Offset: 0x0014B3B8
	public Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		if (!s)
		{
			Debug.Log("Missing shader in " + this.ToString());
			base.enabled = false;
			return null;
		}
		if (s.isSupported && m2Create && m2Create.shader == s)
		{
			return m2Create;
		}
		if (!s.isSupported)
		{
			this.NotSupported();
			Debug.LogError(string.Concat(new string[]
			{
				"The shader ",
				s.ToString(),
				" on effect ",
				this.ToString(),
				" is not supported on this platform!"
			}));
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		if (m2Create)
		{
			return m2Create;
		}
		return null;
	}

	// Token: 0x06003876 RID: 14454 RVA: 0x0014D270 File Offset: 0x0014B470
	private Material CreateMaterial(Shader s, Material m2Create)
	{
		if (!s)
		{
			Debug.Log("Missing shader in " + this.ToString());
			return null;
		}
		if (m2Create && m2Create.shader == s && s.isSupported)
		{
			return m2Create;
		}
		if (!s.isSupported)
		{
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		if (m2Create)
		{
			return m2Create;
		}
		return null;
	}

	// Token: 0x06003877 RID: 14455 RVA: 0x0014D2E1 File Offset: 0x0014B4E1
	private void OnEnable()
	{
		this.isSupported = true;
	}

	// Token: 0x06003878 RID: 14456 RVA: 0x0014D2EA File Offset: 0x0014B4EA
	private bool CheckSupport()
	{
		return this.CheckSupport(false);
	}

	// Token: 0x06003879 RID: 14457 RVA: 0x0014D2F3 File Offset: 0x0014B4F3
	private bool CheckResources()
	{
		Debug.LogWarning("CheckResources () for " + this.ToString() + " should be overwritten.");
		return this.isSupported;
	}

	// Token: 0x0600387A RID: 14458 RVA: 0x0014D315 File Offset: 0x0014B515
	private void Start()
	{
		this.CheckResources();
	}

	// Token: 0x0600387B RID: 14459 RVA: 0x0014D320 File Offset: 0x0014B520
	public bool CheckSupport(bool needDepth)
	{
		this.isSupported = true;
		this.supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			this.NotSupported();
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			this.NotSupported();
			return false;
		}
		if (needDepth)
		{
			base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		}
		return true;
	}

	// Token: 0x0600387C RID: 14460 RVA: 0x0014D380 File Offset: 0x0014B580
	private bool CheckSupport(bool needDepth, bool needHdr)
	{
		if (!this.CheckSupport(needDepth))
		{
			return false;
		}
		if (needHdr && !this.supportHDRTextures)
		{
			this.NotSupported();
			return false;
		}
		return true;
	}

	// Token: 0x0600387D RID: 14461 RVA: 0x0014D3A1 File Offset: 0x0014B5A1
	private void ReportAutoDisable()
	{
		Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
	}

	// Token: 0x0600387E RID: 14462 RVA: 0x0014D3C0 File Offset: 0x0014B5C0
	private bool CheckShader(Shader s)
	{
		Debug.Log(string.Concat(new string[]
		{
			"The shader ",
			s.ToString(),
			" on effect ",
			this.ToString(),
			" is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package."
		}));
		if (!s.isSupported)
		{
			this.NotSupported();
			return false;
		}
		return false;
	}

	// Token: 0x0600387F RID: 14463 RVA: 0x0014D418 File Offset: 0x0014B618
	private void NotSupported()
	{
		base.enabled = false;
		this.isSupported = false;
	}

	// Token: 0x06003880 RID: 14464 RVA: 0x0014D428 File Offset: 0x0014B628
	private void DrawBorder(RenderTexture dest, Material material)
	{
		RenderTexture.active = dest;
		bool flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			float y;
			float y2;
			if (flag)
			{
				y = 1f;
				y2 = 0f;
			}
			else
			{
				y = 0f;
				y2 = 1f;
			}
			float x = 0f;
			float x2 = 0f + 1f / ((float)dest.width * 1f);
			float y3 = 0f;
			float y4 = 1f;
			GL.Begin(7);
			GL.TexCoord2(0f, y);
			GL.Vertex3(x, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x, y4, 0.1f);
			float x3 = 1f - 1f / ((float)dest.width * 1f);
			x2 = 1f;
			y3 = 0f;
			y4 = 1f;
			GL.TexCoord2(0f, y);
			GL.Vertex3(x3, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x3, y4, 0.1f);
			float x4 = 0f;
			x2 = 1f;
			y3 = 0f;
			y4 = 0f + 1f / ((float)dest.height * 1f);
			GL.TexCoord2(0f, y);
			GL.Vertex3(x4, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x4, y4, 0.1f);
			float x5 = 0f;
			x2 = 1f;
			y3 = 1f - 1f / ((float)dest.height * 1f);
			y4 = 1f;
			GL.TexCoord2(0f, y);
			GL.Vertex3(x5, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x5, y4, 0.1f);
			GL.End();
		}
		GL.PopMatrix();
	}

	// Token: 0x040032C7 RID: 12999
	protected bool supportHDRTextures = true;

	// Token: 0x040032C8 RID: 13000
	protected bool isSupported = true;
}
