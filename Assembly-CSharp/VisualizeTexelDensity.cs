using System;
using UnityEngine;

// Token: 0x02000975 RID: 2421
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[AddComponentMenu("Rendering/Visualize Texture Density")]
public class VisualizeTexelDensity : MonoBehaviour
{
	// Token: 0x1700044B RID: 1099
	// (get) Token: 0x06003949 RID: 14665 RVA: 0x00152B94 File Offset: 0x00150D94
	public static VisualizeTexelDensity Instance
	{
		get
		{
			return VisualizeTexelDensity.instance;
		}
	}

	// Token: 0x0600394A RID: 14666 RVA: 0x00152B9B File Offset: 0x00150D9B
	private void Awake()
	{
		VisualizeTexelDensity.instance = this;
		this.mainCamera = base.GetComponent<Camera>();
	}

	// Token: 0x0600394B RID: 14667 RVA: 0x00152BAF File Offset: 0x00150DAF
	private void OnEnable()
	{
		this.mainCamera = base.GetComponent<Camera>();
		this.screenWidth = Screen.width;
		this.screenHeight = Screen.height;
		this.LoadResources();
		this.initialized = true;
	}

	// Token: 0x0600394C RID: 14668 RVA: 0x00152BE0 File Offset: 0x00150DE0
	private void OnDisable()
	{
		this.SafeDestroyViewTexelDensity();
		this.SafeDestroyViewTexelDensityRT();
		this.initialized = false;
	}

	// Token: 0x0600394D RID: 14669 RVA: 0x00152BF8 File Offset: 0x00150DF8
	private void LoadResources()
	{
		if (this.texelDensityGradTex == null)
		{
			this.texelDensityGradTex = (Resources.Load("TexelDensityGrad") as Texture);
		}
		if (this.texelDensityOverlayMat == null)
		{
			this.texelDensityOverlayMat = new Material(Shader.Find("Hidden/TexelDensityOverlay"))
			{
				hideFlags = HideFlags.DontSave
			};
		}
	}

	// Token: 0x0600394E RID: 14670 RVA: 0x00152C54 File Offset: 0x00150E54
	private void SafeDestroyViewTexelDensity()
	{
		if (this.texelDensityCamera != null)
		{
			UnityEngine.Object.DestroyImmediate(this.texelDensityCamera.gameObject);
			this.texelDensityCamera = null;
		}
		if (this.texelDensityGradTex != null)
		{
			Resources.UnloadAsset(this.texelDensityGradTex);
			this.texelDensityGradTex = null;
		}
		if (this.texelDensityOverlayMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.texelDensityOverlayMat);
			this.texelDensityOverlayMat = null;
		}
	}

	// Token: 0x0600394F RID: 14671 RVA: 0x00152CC6 File Offset: 0x00150EC6
	private void SafeDestroyViewTexelDensityRT()
	{
		if (this.texelDensityRT != null)
		{
			Graphics.SetRenderTarget(null);
			this.texelDensityRT.Release();
			UnityEngine.Object.DestroyImmediate(this.texelDensityRT);
			this.texelDensityRT = null;
		}
	}

	// Token: 0x06003950 RID: 14672 RVA: 0x00152CFC File Offset: 0x00150EFC
	private void UpdateViewTexelDensity(bool screenResized)
	{
		if (this.texelDensityCamera == null)
		{
			GameObject gameObject = new GameObject("Texel Density Camera", new Type[]
			{
				typeof(Camera)
			})
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			gameObject.transform.parent = this.mainCamera.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			this.texelDensityCamera = gameObject.GetComponent<Camera>();
			this.texelDensityCamera.CopyFrom(this.mainCamera);
			this.texelDensityCamera.renderingPath = RenderingPath.Forward;
			this.texelDensityCamera.allowMSAA = false;
			this.texelDensityCamera.allowHDR = false;
			this.texelDensityCamera.clearFlags = CameraClearFlags.Skybox;
			this.texelDensityCamera.depthTextureMode = DepthTextureMode.None;
			this.texelDensityCamera.SetReplacementShader(this.shader, this.shaderTag);
			this.texelDensityCamera.enabled = false;
		}
		if (this.texelDensityRT == null || screenResized || !this.texelDensityRT.IsCreated())
		{
			this.texelDensityCamera.targetTexture = null;
			this.SafeDestroyViewTexelDensityRT();
			this.texelDensityRT = new RenderTexture(this.screenWidth, this.screenHeight, 24, RenderTextureFormat.ARGB32)
			{
				hideFlags = HideFlags.DontSave
			};
			this.texelDensityRT.name = "TexelDensityRT";
			this.texelDensityRT.filterMode = FilterMode.Point;
			this.texelDensityRT.wrapMode = TextureWrapMode.Clamp;
			this.texelDensityRT.Create();
		}
		if (this.texelDensityCamera.targetTexture != this.texelDensityRT)
		{
			this.texelDensityCamera.targetTexture = this.texelDensityRT;
		}
		Shader.SetGlobalFloat("global_TexelsPerMeter", (float)this.texelsPerMeter);
		Shader.SetGlobalTexture("global_TexelDensityGrad", this.texelDensityGradTex);
		this.texelDensityCamera.fieldOfView = this.mainCamera.fieldOfView;
		this.texelDensityCamera.nearClipPlane = this.mainCamera.nearClipPlane;
		this.texelDensityCamera.farClipPlane = this.mainCamera.farClipPlane;
		this.texelDensityCamera.cullingMask = this.mainCamera.cullingMask;
	}

	// Token: 0x06003951 RID: 14673 RVA: 0x00152F19 File Offset: 0x00151119
	private bool CheckScreenResized(int width, int height)
	{
		if (this.screenWidth != width || this.screenHeight != height)
		{
			this.screenWidth = width;
			this.screenHeight = height;
			return true;
		}
		return false;
	}

	// Token: 0x06003952 RID: 14674 RVA: 0x00152F40 File Offset: 0x00151140
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.initialized)
		{
			this.UpdateViewTexelDensity(this.CheckScreenResized(source.width, source.height));
			this.texelDensityCamera.Render();
			this.texelDensityOverlayMat.SetTexture("_TexelDensityMap", this.texelDensityRT);
			this.texelDensityOverlayMat.SetFloat("_Opacity", this.overlayOpacity);
			Graphics.Blit(source, destination, this.texelDensityOverlayMat, 0);
			return;
		}
		Graphics.Blit(source, destination);
	}

	// Token: 0x06003953 RID: 14675 RVA: 0x00152FBC File Offset: 0x001511BC
	private void DrawGUIText(float x, float y, Vector2 size, string text, GUIStyle fontStyle)
	{
		fontStyle.normal.textColor = Color.black;
		GUI.Label(new Rect(x - 1f, y + 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x + 1f, y - 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x + 1f, y + 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x - 1f, y - 1f, size.x, size.y), text, fontStyle);
		fontStyle.normal.textColor = Color.white;
		GUI.Label(new Rect(x, y, size.x, size.y), text, fontStyle);
	}

	// Token: 0x06003954 RID: 14676 RVA: 0x001530A8 File Offset: 0x001512A8
	private void OnGUI()
	{
		if (this.initialized && this.showHUD)
		{
			string text = "Texels Per Meter";
			string text2 = "0";
			string text3 = this.texelsPerMeter.ToString();
			string text4 = (this.texelsPerMeter << 1).ToString() + "+";
			float num = (float)this.texelDensityGradTex.width;
			float num2 = (float)(this.texelDensityGradTex.height * 2);
			float num3 = (float)((Screen.width - this.texelDensityGradTex.width) / 2);
			float num4 = 32f;
			GL.PushMatrix();
			GL.LoadPixelMatrix(0f, (float)Screen.width, (float)Screen.height, 0f);
			Graphics.DrawTexture(new Rect(num3 - 2f, num4 - 2f, num + 4f, num2 + 4f), Texture2D.whiteTexture);
			Graphics.DrawTexture(new Rect(num3, num4, num, num2), this.texelDensityGradTex);
			GL.PopMatrix();
			GUIStyle guistyle = new GUIStyle();
			guistyle.fontSize = 13;
			Vector2 vector = guistyle.CalcSize(new GUIContent(text));
			Vector2 size = guistyle.CalcSize(new GUIContent(text2));
			Vector2 vector2 = guistyle.CalcSize(new GUIContent(text3));
			Vector2 vector3 = guistyle.CalcSize(new GUIContent(text4));
			this.DrawGUIText(((float)Screen.width - vector.x) / 2f, num4 - vector.y - 5f, vector, text, guistyle);
			this.DrawGUIText(num3, num4 + num2 + 6f, size, text2, guistyle);
			this.DrawGUIText(((float)Screen.width - vector2.x) / 2f, num4 + num2 + 6f, vector2, text3, guistyle);
			this.DrawGUIText(num3 + num - vector3.x, num4 + num2 + 6f, vector3, text4, guistyle);
		}
	}

	// Token: 0x040033B8 RID: 13240
	public Shader shader;

	// Token: 0x040033B9 RID: 13241
	public string shaderTag = "RenderType";

	// Token: 0x040033BA RID: 13242
	[Range(1f, 1024f)]
	public int texelsPerMeter = 256;

	// Token: 0x040033BB RID: 13243
	[Range(0f, 1f)]
	public float overlayOpacity = 0.5f;

	// Token: 0x040033BC RID: 13244
	public bool showHUD = true;

	// Token: 0x040033BD RID: 13245
	private Camera mainCamera;

	// Token: 0x040033BE RID: 13246
	private bool initialized;

	// Token: 0x040033BF RID: 13247
	private int screenWidth;

	// Token: 0x040033C0 RID: 13248
	private int screenHeight;

	// Token: 0x040033C1 RID: 13249
	private Camera texelDensityCamera;

	// Token: 0x040033C2 RID: 13250
	private RenderTexture texelDensityRT;

	// Token: 0x040033C3 RID: 13251
	private Texture texelDensityGradTex;

	// Token: 0x040033C4 RID: 13252
	private Material texelDensityOverlayMat;

	// Token: 0x040033C5 RID: 13253
	private static VisualizeTexelDensity instance;
}
