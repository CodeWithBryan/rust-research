using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RustNative;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200096D RID: 2413
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class OcclusionCulling : MonoBehaviour
{
	// Token: 0x060038DA RID: 14554 RVA: 0x0014F13A File Offset: 0x0014D33A
	public static bool DebugFilterIsDynamic(int filter)
	{
		return filter == 1 || filter == 4;
	}

	// Token: 0x060038DB RID: 14555 RVA: 0x0014F146 File Offset: 0x0014D346
	public static bool DebugFilterIsStatic(int filter)
	{
		return filter == 2 || filter == 4;
	}

	// Token: 0x060038DC RID: 14556 RVA: 0x0014F152 File Offset: 0x0014D352
	public static bool DebugFilterIsGrid(int filter)
	{
		return filter == 3 || filter == 4;
	}

	// Token: 0x060038DD RID: 14557 RVA: 0x0014F15E File Offset: 0x0014D35E
	private void DebugInitialize()
	{
		this.debugMipMat = new Material(Shader.Find("Hidden/OcclusionCulling/DebugMip"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	// Token: 0x060038DE RID: 14558 RVA: 0x0014F17D File Offset: 0x0014D37D
	private void DebugShutdown()
	{
		if (this.debugMipMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.debugMipMat);
			this.debugMipMat = null;
		}
	}

	// Token: 0x060038DF RID: 14559 RVA: 0x0014F19F File Offset: 0x0014D39F
	private void DebugUpdate()
	{
		if (this.HiZReady)
		{
			this.debugSettings.showMainLod = Mathf.Clamp(this.debugSettings.showMainLod, 0, this.hiZLevels.Length - 1);
		}
	}

	// Token: 0x060038E0 RID: 14560 RVA: 0x000059DD File Offset: 0x00003BDD
	private void DebugDraw()
	{
	}

	// Token: 0x060038E1 RID: 14561 RVA: 0x0014F1D0 File Offset: 0x0014D3D0
	public static void NormalizePlane(ref Vector4 plane)
	{
		float num = Mathf.Sqrt(plane.x * plane.x + plane.y * plane.y + plane.z * plane.z);
		plane.x /= num;
		plane.y /= num;
		plane.z /= num;
		plane.w /= num;
	}

	// Token: 0x060038E2 RID: 14562 RVA: 0x0014F238 File Offset: 0x0014D438
	public static void ExtractFrustum(Matrix4x4 viewProjMatrix, ref Vector4[] planes)
	{
		planes[0].x = viewProjMatrix.m30 + viewProjMatrix.m00;
		planes[0].y = viewProjMatrix.m31 + viewProjMatrix.m01;
		planes[0].z = viewProjMatrix.m32 + viewProjMatrix.m02;
		planes[0].w = viewProjMatrix.m33 + viewProjMatrix.m03;
		OcclusionCulling.NormalizePlane(ref planes[0]);
		planes[1].x = viewProjMatrix.m30 - viewProjMatrix.m00;
		planes[1].y = viewProjMatrix.m31 - viewProjMatrix.m01;
		planes[1].z = viewProjMatrix.m32 - viewProjMatrix.m02;
		planes[1].w = viewProjMatrix.m33 - viewProjMatrix.m03;
		OcclusionCulling.NormalizePlane(ref planes[1]);
		planes[2].x = viewProjMatrix.m30 - viewProjMatrix.m10;
		planes[2].y = viewProjMatrix.m31 - viewProjMatrix.m11;
		planes[2].z = viewProjMatrix.m32 - viewProjMatrix.m12;
		planes[2].w = viewProjMatrix.m33 - viewProjMatrix.m13;
		OcclusionCulling.NormalizePlane(ref planes[2]);
		planes[3].x = viewProjMatrix.m30 + viewProjMatrix.m10;
		planes[3].y = viewProjMatrix.m31 + viewProjMatrix.m11;
		planes[3].z = viewProjMatrix.m32 + viewProjMatrix.m12;
		planes[3].w = viewProjMatrix.m33 + viewProjMatrix.m13;
		OcclusionCulling.NormalizePlane(ref planes[3]);
		planes[4].x = viewProjMatrix.m20;
		planes[4].y = viewProjMatrix.m21;
		planes[4].z = viewProjMatrix.m22;
		planes[4].w = viewProjMatrix.m23;
		OcclusionCulling.NormalizePlane(ref planes[4]);
		planes[5].x = viewProjMatrix.m30 - viewProjMatrix.m20;
		planes[5].y = viewProjMatrix.m31 - viewProjMatrix.m21;
		planes[5].z = viewProjMatrix.m32 - viewProjMatrix.m22;
		planes[5].w = viewProjMatrix.m33 - viewProjMatrix.m23;
		OcclusionCulling.NormalizePlane(ref planes[5]);
	}

	// Token: 0x17000445 RID: 1093
	// (get) Token: 0x060038E3 RID: 14563 RVA: 0x0014F4E7 File Offset: 0x0014D6E7
	public bool HiZReady
	{
		get
		{
			return this.hiZTexture != null && this.hiZWidth > 0 && this.hiZHeight > 0;
		}
	}

	// Token: 0x060038E4 RID: 14564 RVA: 0x0014F50C File Offset: 0x0014D70C
	public void CheckResizeHiZMap()
	{
		int pixelWidth = this.camera.pixelWidth;
		int pixelHeight = this.camera.pixelHeight;
		if (pixelWidth > 0 && pixelHeight > 0)
		{
			int num = pixelWidth / 4;
			int num2 = pixelHeight / 4;
			if (this.hiZLevels == null || this.hiZWidth != num || this.hiZHeight != num2)
			{
				this.InitializeHiZMap(num, num2);
				this.hiZWidth = num;
				this.hiZHeight = num2;
				if (this.debugSettings.log)
				{
					Debug.Log(string.Concat(new object[]
					{
						"[OcclusionCulling] Resized HiZ Map to ",
						this.hiZWidth,
						" x ",
						this.hiZHeight
					}));
				}
			}
		}
	}

	// Token: 0x060038E5 RID: 14565 RVA: 0x0014F5C0 File Offset: 0x0014D7C0
	private void InitializeHiZMap()
	{
		Shader shader = Shader.Find("Hidden/OcclusionCulling/DepthDownscale");
		Shader shader2 = Shader.Find("Hidden/OcclusionCulling/BlitCopy");
		this.downscaleMat = new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.blitCopyMat = new Material(shader2)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.CheckResizeHiZMap();
	}

	// Token: 0x060038E6 RID: 14566 RVA: 0x0014F614 File Offset: 0x0014D814
	private void FinalizeHiZMap()
	{
		this.DestroyHiZMap();
		if (this.downscaleMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.downscaleMat);
			this.downscaleMat = null;
		}
		if (this.blitCopyMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.blitCopyMat);
			this.blitCopyMat = null;
		}
	}

	// Token: 0x060038E7 RID: 14567 RVA: 0x0014F668 File Offset: 0x0014D868
	private void InitializeHiZMap(int width, int height)
	{
		this.DestroyHiZMap();
		width = Mathf.Clamp(width, 1, 65536);
		height = Mathf.Clamp(height, 1, 65536);
		int num = Mathf.Min(width, height);
		this.hiZLevelCount = (int)(Mathf.Log((float)num, 2f) + 1f);
		this.hiZLevels = new RenderTexture[this.hiZLevelCount];
		this.depthTexture = this.CreateDepthTexture("DepthTex", width, height, false);
		this.hiZTexture = this.CreateDepthTexture("HiZMapTex", width, height, true);
		for (int i = 0; i < this.hiZLevelCount; i++)
		{
			this.hiZLevels[i] = this.CreateDepthTextureMip("HiZMap" + i, width, height, i);
		}
	}

	// Token: 0x060038E8 RID: 14568 RVA: 0x0014F724 File Offset: 0x0014D924
	private void DestroyHiZMap()
	{
		if (this.depthTexture != null)
		{
			RenderTexture.active = null;
			UnityEngine.Object.DestroyImmediate(this.depthTexture);
			this.depthTexture = null;
		}
		if (this.hiZTexture != null)
		{
			RenderTexture.active = null;
			UnityEngine.Object.DestroyImmediate(this.hiZTexture);
			this.hiZTexture = null;
		}
		if (this.hiZLevels != null)
		{
			for (int i = 0; i < this.hiZLevels.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(this.hiZLevels[i]);
			}
			this.hiZLevels = null;
		}
	}

	// Token: 0x060038E9 RID: 14569 RVA: 0x0014F7AC File Offset: 0x0014D9AC
	private RenderTexture CreateDepthTexture(string name, int width, int height, bool mips = false)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		renderTexture.name = name;
		renderTexture.useMipMap = mips;
		renderTexture.autoGenerateMips = false;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x060038EA RID: 14570 RVA: 0x0014F7E4 File Offset: 0x0014D9E4
	private RenderTexture CreateDepthTextureMip(string name, int width, int height, int mip)
	{
		int width2 = width >> mip;
		int height2 = height >> mip;
		RenderTexture renderTexture = new RenderTexture(width2, height2, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		renderTexture.name = name;
		renderTexture.useMipMap = false;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x060038EB RID: 14571 RVA: 0x0014F82D File Offset: 0x0014DA2D
	public void GrabDepthTexture()
	{
		if (this.depthTexture != null)
		{
			UnityEngine.Graphics.Blit(null, this.depthTexture, this.depthCopyMat, 0);
		}
	}

	// Token: 0x060038EC RID: 14572 RVA: 0x0014F850 File Offset: 0x0014DA50
	public void GenerateHiZMipChain()
	{
		if (this.HiZReady)
		{
			bool flag = true;
			this.depthCopyMat.SetMatrix("_CameraReprojection", this.prevViewProjMatrix * this.invViewProjMatrix);
			this.depthCopyMat.SetFloat("_FrustumNoDataDepth", flag ? 1f : 0f);
			UnityEngine.Graphics.Blit(this.depthTexture, this.hiZLevels[0], this.depthCopyMat, 1);
			for (int i = 1; i < this.hiZLevels.Length; i++)
			{
				RenderTexture renderTexture = this.hiZLevels[i - 1];
				RenderTexture dest = this.hiZLevels[i];
				int pass = ((renderTexture.width & 1) == 0 && (renderTexture.height & 1) == 0) ? 0 : 1;
				this.downscaleMat.SetTexture("_MainTex", renderTexture);
				UnityEngine.Graphics.Blit(renderTexture, dest, this.downscaleMat, pass);
			}
			for (int j = 0; j < this.hiZLevels.Length; j++)
			{
				UnityEngine.Graphics.SetRenderTarget(this.hiZTexture, j);
				UnityEngine.Graphics.Blit(this.hiZLevels[j], this.blitCopyMat);
			}
		}
	}

	// Token: 0x060038ED RID: 14573 RVA: 0x0014F960 File Offset: 0x0014DB60
	private void DebugDrawGizmos()
	{
		Camera component = base.GetComponent<Camera>();
		Gizmos.color = new Color(0.75f, 0.75f, 0f, 0.5f);
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		Gizmos.DrawFrustum(Vector3.zero, component.fieldOfView, component.farClipPlane, component.nearClipPlane, component.aspect);
		Gizmos.color = Color.red;
		Gizmos.matrix = Matrix4x4.identity;
		Matrix4x4 worldToCameraMatrix = component.worldToCameraMatrix;
		Matrix4x4 matrix4x = GL.GetGPUProjectionMatrix(component.projectionMatrix, false) * worldToCameraMatrix;
		Vector4[] array = new Vector4[6];
		OcclusionCulling.ExtractFrustum(matrix4x, ref array);
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 a = new Vector3(array[i].x, array[i].y, array[i].z);
			float w = array[i].w;
			Vector3 vector = -a * w;
			Gizmos.DrawLine(vector, vector * 2f);
		}
	}

	// Token: 0x060038EE RID: 14574 RVA: 0x0014FA78 File Offset: 0x0014DC78
	private static int floor(float x)
	{
		int num = (int)x;
		if (x >= (float)num)
		{
			return num;
		}
		return num - 1;
	}

	// Token: 0x060038EF RID: 14575 RVA: 0x0014FA94 File Offset: 0x0014DC94
	public static OcclusionCulling.Cell RegisterToGrid(OccludeeState occludee)
	{
		int num = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num2 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num3 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		int num4 = Mathf.Clamp(num, -1048575, 1048575);
		int num5 = Mathf.Clamp(num2, -1048575, 1048575);
		int num6 = Mathf.Clamp(num3, -1048575, 1048575);
		ulong num7 = (ulong)((long)((num4 >= 0) ? num4 : (num4 + 1048575)));
		ulong num8 = (ulong)((long)((num5 >= 0) ? num5 : (num5 + 1048575)));
		ulong num9 = (ulong)((long)((num6 >= 0) ? num6 : (num6 + 1048575)));
		ulong key = num7 << 42 | num8 << 21 | num9;
		OcclusionCulling.Cell cell;
		bool flag = OcclusionCulling.grid.TryGetValue(key, out cell);
		if (!flag)
		{
			Vector3 center = default(Vector3);
			center.x = (float)num * 100f + 50f;
			center.y = (float)num2 * 100f + 50f;
			center.z = (float)num3 * 100f + 50f;
			Vector3 size = new Vector3(100f, 100f, 100f);
			cell = OcclusionCulling.grid.Add(key, 16).Initialize(num, num2, num3, new Bounds(center, size));
		}
		OcclusionCulling.SmartList smartList = occludee.isStatic ? cell.staticBucket : cell.dynamicBucket;
		if (!flag || !smartList.Contains(occludee))
		{
			occludee.cell = cell;
			smartList.Add(occludee, 16);
			OcclusionCulling.gridChanged.Enqueue(cell);
		}
		return cell;
	}

	// Token: 0x060038F0 RID: 14576 RVA: 0x0014FC74 File Offset: 0x0014DE74
	public static void UpdateInGrid(OccludeeState occludee)
	{
		int num = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num2 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num3 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		if (num != occludee.cell.x || num2 != occludee.cell.y || num3 != occludee.cell.z)
		{
			OcclusionCulling.UnregisterFromGrid(occludee);
			OcclusionCulling.RegisterToGrid(occludee);
		}
	}

	// Token: 0x060038F1 RID: 14577 RVA: 0x0014FD3C File Offset: 0x0014DF3C
	public static void UnregisterFromGrid(OccludeeState occludee)
	{
		OcclusionCulling.Cell cell = occludee.cell;
		OcclusionCulling.SmartList smartList = occludee.isStatic ? cell.staticBucket : cell.dynamicBucket;
		OcclusionCulling.gridChanged.Enqueue(cell);
		smartList.Remove(occludee);
		if (cell.staticBucket.Count == 0 && cell.dynamicBucket.Count == 0)
		{
			OcclusionCulling.grid.Remove(cell);
			cell.Reset();
		}
		occludee.cell = null;
	}

	// Token: 0x060038F2 RID: 14578 RVA: 0x0014FDAC File Offset: 0x0014DFAC
	public void UpdateGridBuffers()
	{
		if (OcclusionCulling.gridSet.CheckResize(OcclusionCulling.grid.Size, 256))
		{
			if (this.debugSettings.log)
			{
				Debug.Log("[OcclusionCulling] Resized grid to " + OcclusionCulling.grid.Size);
			}
			for (int i = 0; i < OcclusionCulling.grid.Size; i++)
			{
				if (OcclusionCulling.grid[i] != null)
				{
					OcclusionCulling.gridChanged.Enqueue(OcclusionCulling.grid[i]);
				}
			}
		}
		bool flag = OcclusionCulling.gridChanged.Count > 0;
		while (OcclusionCulling.gridChanged.Count > 0)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.gridChanged.Dequeue();
			OcclusionCulling.gridSet.inputData[cell.hashedPoolIndex] = cell.sphereBounds;
		}
		if (flag)
		{
			OcclusionCulling.gridSet.UploadData();
		}
	}

	// Token: 0x17000446 RID: 1094
	// (get) Token: 0x060038F3 RID: 14579 RVA: 0x0014FE8B File Offset: 0x0014E08B
	public static OcclusionCulling Instance
	{
		get
		{
			return OcclusionCulling.instance;
		}
	}

	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x060038F4 RID: 14580 RVA: 0x0014FE92 File Offset: 0x0014E092
	public static bool Supported
	{
		get
		{
			return OcclusionCulling.supportedDeviceTypes.Contains(SystemInfo.graphicsDeviceType);
		}
	}

	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x060038F5 RID: 14581 RVA: 0x0014FEA3 File Offset: 0x0014E0A3
	// (set) Token: 0x060038F6 RID: 14582 RVA: 0x0014FEAA File Offset: 0x0014E0AA
	public static bool Enabled
	{
		get
		{
			return OcclusionCulling._enabled;
		}
		set
		{
			OcclusionCulling._enabled = value;
			if (OcclusionCulling.instance != null)
			{
				OcclusionCulling.instance.enabled = value;
			}
		}
	}

	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x060038F7 RID: 14583 RVA: 0x0014FECA File Offset: 0x0014E0CA
	// (set) Token: 0x060038F8 RID: 14584 RVA: 0x0014FED1 File Offset: 0x0014E0D1
	public static bool SafeMode
	{
		get
		{
			return OcclusionCulling._safeMode;
		}
		set
		{
			OcclusionCulling._safeMode = value;
		}
	}

	// Token: 0x1700044A RID: 1098
	// (get) Token: 0x060038F9 RID: 14585 RVA: 0x0014FED9 File Offset: 0x0014E0D9
	// (set) Token: 0x060038FA RID: 14586 RVA: 0x0014FEE0 File Offset: 0x0014E0E0
	public static OcclusionCulling.DebugFilter DebugShow
	{
		get
		{
			return OcclusionCulling._debugShow;
		}
		set
		{
			OcclusionCulling._debugShow = value;
		}
	}

	// Token: 0x060038FB RID: 14587 RVA: 0x0014FEE8 File Offset: 0x0014E0E8
	private static void GrowStatePool()
	{
		for (int i = 0; i < 2048; i++)
		{
			OcclusionCulling.statePool.Enqueue(new OccludeeState());
		}
	}

	// Token: 0x060038FC RID: 14588 RVA: 0x0014FF14 File Offset: 0x0014E114
	private static OccludeeState Allocate()
	{
		if (OcclusionCulling.statePool.Count == 0)
		{
			OcclusionCulling.GrowStatePool();
		}
		return OcclusionCulling.statePool.Dequeue();
	}

	// Token: 0x060038FD RID: 14589 RVA: 0x0014FF31 File Offset: 0x0014E131
	private static void Release(OccludeeState state)
	{
		OcclusionCulling.statePool.Enqueue(state);
	}

	// Token: 0x060038FE RID: 14590 RVA: 0x0014FF40 File Offset: 0x0014E140
	private void Awake()
	{
		OcclusionCulling.instance = this;
		this.camera = base.GetComponent<Camera>();
		for (int i = 0; i < 6; i++)
		{
			this.frustumPropNames[i] = "_FrustumPlane" + i;
		}
	}

	// Token: 0x060038FF RID: 14591 RVA: 0x0014FF84 File Offset: 0x0014E184
	private void OnEnable()
	{
		if (!OcclusionCulling.Enabled)
		{
			OcclusionCulling.Enabled = false;
			return;
		}
		if (!OcclusionCulling.Supported)
		{
			Debug.LogWarning("[OcclusionCulling] Disabled due to graphics device type " + SystemInfo.graphicsDeviceType + " not supported.");
			OcclusionCulling.Enabled = false;
			return;
		}
		this.usePixelShaderFallback = (this.usePixelShaderFallback || !SystemInfo.supportsComputeShaders || this.computeShader == null || !this.computeShader.HasKernel("compute_cull"));
		this.useNativePath = (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 && this.SupportsNativePath());
		this.useAsyncReadAPI = (!this.useNativePath && SystemInfo.supportsAsyncGPUReadback);
		if (!this.useNativePath && !this.useAsyncReadAPI)
		{
			Debug.LogWarning("[OcclusionCulling] Disabled due to unsupported Async GPU Reads on device " + SystemInfo.graphicsDeviceType);
			OcclusionCulling.Enabled = false;
			return;
		}
		for (int i = 0; i < OcclusionCulling.staticOccludees.Count; i++)
		{
			OcclusionCulling.staticChanged.Add(i);
		}
		for (int j = 0; j < OcclusionCulling.dynamicOccludees.Count; j++)
		{
			OcclusionCulling.dynamicChanged.Add(j);
		}
		if (this.usePixelShaderFallback)
		{
			this.fallbackMat = new Material(Shader.Find("Hidden/OcclusionCulling/Culling"))
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
		OcclusionCulling.staticSet.Attach(this);
		OcclusionCulling.dynamicSet.Attach(this);
		OcclusionCulling.gridSet.Attach(this);
		this.depthCopyMat = new Material(Shader.Find("Hidden/OcclusionCulling/DepthCopy"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.InitializeHiZMap();
		this.UpdateCameraMatrices(true);
	}

	// Token: 0x06003900 RID: 14592 RVA: 0x00150114 File Offset: 0x0014E314
	private bool SupportsNativePath()
	{
		bool result = true;
		try
		{
			OccludeeState.State state = default(OccludeeState.State);
			Color32 color = new Color32(0, 0, 0, 0);
			Vector4 zero = Vector4.zero;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			OcclusionCulling.ProcessOccludees_Native(ref state, ref num, 0, ref color, 0, ref num2, ref num3, ref zero, 0f, 0U);
		}
		catch (EntryPointNotFoundException)
		{
			Debug.Log("[OcclusionCulling] Fast native path not available. Reverting to managed fallback.");
			result = false;
		}
		return result;
	}

	// Token: 0x06003901 RID: 14593 RVA: 0x00150184 File Offset: 0x0014E384
	private void OnDisable()
	{
		if (this.fallbackMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.fallbackMat);
			this.fallbackMat = null;
		}
		if (this.depthCopyMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.depthCopyMat);
			this.depthCopyMat = null;
		}
		OcclusionCulling.staticSet.Dispose(true);
		OcclusionCulling.dynamicSet.Dispose(true);
		OcclusionCulling.gridSet.Dispose(true);
		this.FinalizeHiZMap();
	}

	// Token: 0x06003902 RID: 14594 RVA: 0x001501F8 File Offset: 0x0014E3F8
	public static void MakeAllVisible()
	{
		for (int i = 0; i < OcclusionCulling.staticOccludees.Count; i++)
		{
			if (OcclusionCulling.staticOccludees[i] != null)
			{
				OcclusionCulling.staticOccludees[i].MakeVisible();
			}
		}
		for (int j = 0; j < OcclusionCulling.dynamicOccludees.Count; j++)
		{
			if (OcclusionCulling.dynamicOccludees[j] != null)
			{
				OcclusionCulling.dynamicOccludees[j].MakeVisible();
			}
		}
	}

	// Token: 0x06003903 RID: 14595 RVA: 0x00150269 File Offset: 0x0014E469
	private void Update()
	{
		if (!OcclusionCulling.Enabled)
		{
			base.enabled = false;
			return;
		}
		this.CheckResizeHiZMap();
		this.DebugUpdate();
		this.DebugDraw();
	}

	// Token: 0x06003904 RID: 14596 RVA: 0x0015028C File Offset: 0x0014E48C
	public static void RecursiveAddOccludees<T>(Transform transform, float minTimeVisible = 0.1f, bool isStatic = true, bool stickyGizmos = false) where T : Occludee
	{
		Renderer component = transform.GetComponent<Renderer>();
		Collider component2 = transform.GetComponent<Collider>();
		if (component != null && component2 != null)
		{
			T t = component.gameObject.GetComponent<T>();
			t = ((t == null) ? component.gameObject.AddComponent<T>() : t);
			t.minTimeVisible = minTimeVisible;
			t.isStatic = isStatic;
			t.stickyGizmos = stickyGizmos;
			t.Register();
		}
		foreach (object obj in transform)
		{
			OcclusionCulling.RecursiveAddOccludees<T>((Transform)obj, minTimeVisible, isStatic, stickyGizmos);
		}
	}

	// Token: 0x06003905 RID: 14597 RVA: 0x0015035C File Offset: 0x0014E55C
	private static int FindFreeSlot(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, Queue<int> recycled)
	{
		int result;
		if (recycled.Count > 0)
		{
			result = recycled.Dequeue();
		}
		else
		{
			if (occludees.Count == occludees.Capacity)
			{
				int num = Mathf.Min(occludees.Capacity + 2048, 1048576);
				if (num > 0)
				{
					occludees.Capacity = num;
					states.Capacity = num;
				}
			}
			if (occludees.Count < occludees.Capacity)
			{
				result = occludees.Count;
				occludees.Add(null);
				states.Add(default(OccludeeState.State));
			}
			else
			{
				result = -1;
			}
		}
		return result;
	}

	// Token: 0x06003906 RID: 14598 RVA: 0x001503E4 File Offset: 0x0014E5E4
	public static OccludeeState GetStateById(int id)
	{
		if (id < 0 || id >= 2097152)
		{
			return null;
		}
		bool flag = id < 1048576;
		int index = flag ? id : (id - 1048576);
		if (flag)
		{
			return OcclusionCulling.staticOccludees[index];
		}
		return OcclusionCulling.dynamicOccludees[index];
	}

	// Token: 0x06003907 RID: 14599 RVA: 0x00150430 File Offset: 0x0014E630
	public static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged = null)
	{
		int num;
		if (isStatic)
		{
			num = OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged, OcclusionCulling.staticSet, OcclusionCulling.staticVisibilityChanged);
		}
		else
		{
			num = OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicVisibilityChanged);
		}
		if (num >= 0 && !isStatic)
		{
			return num + 1048576;
		}
		return num;
	}

	// Token: 0x06003908 RID: 14600 RVA: 0x001504B4 File Offset: 0x0014E6B4
	private static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged, OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, Queue<int> recycled, List<int> changed, OcclusionCulling.BufferSet set, OcclusionCulling.SimpleList<int> visibilityChanged)
	{
		int num = OcclusionCulling.FindFreeSlot(occludees, states, recycled);
		if (num >= 0)
		{
			Vector4 sphereBounds = new Vector4(center.x, center.y, center.z, radius);
			OccludeeState occludeeState = OcclusionCulling.Allocate().Initialize(states, set, num, sphereBounds, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged);
			occludeeState.cell = OcclusionCulling.RegisterToGrid(occludeeState);
			occludees[num] = occludeeState;
			changed.Add(num);
			if (states.array[num].isVisible > 0 != occludeeState.cell.isVisible)
			{
				visibilityChanged.Add(num);
			}
		}
		return num;
	}

	// Token: 0x06003909 RID: 14601 RVA: 0x0015054C File Offset: 0x0014E74C
	public static void UnregisterOccludee(int id)
	{
		if (id >= 0 && id < 2097152)
		{
			bool flag = id < 1048576;
			int slot = flag ? id : (id - 1048576);
			if (flag)
			{
				OcclusionCulling.UnregisterOccludee(slot, OcclusionCulling.staticOccludees, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged);
				return;
			}
			OcclusionCulling.UnregisterOccludee(slot, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged);
		}
	}

	// Token: 0x0600390A RID: 14602 RVA: 0x001505A8 File Offset: 0x0014E7A8
	private static void UnregisterOccludee(int slot, OcclusionCulling.SimpleList<OccludeeState> occludees, Queue<int> recycled, List<int> changed)
	{
		OccludeeState occludeeState = occludees[slot];
		OcclusionCulling.UnregisterFromGrid(occludeeState);
		recycled.Enqueue(slot);
		changed.Add(slot);
		OcclusionCulling.Release(occludeeState);
		occludees[slot] = null;
		occludeeState.Invalidate();
	}

	// Token: 0x0600390B RID: 14603 RVA: 0x001505D8 File Offset: 0x0014E7D8
	public static void UpdateDynamicOccludee(int id, Vector3 center, float radius)
	{
		int num = id - 1048576;
		if (num >= 0 && num < 1048576)
		{
			OcclusionCulling.dynamicStates.array[num].sphereBounds = new Vector4(center.x, center.y, center.z, radius);
			OcclusionCulling.dynamicChanged.Add(num);
		}
	}

	// Token: 0x0600390C RID: 14604 RVA: 0x00150634 File Offset: 0x0014E834
	private void UpdateBuffers(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.BufferSet set, List<int> changed, bool isStatic)
	{
		int count = occludees.Count;
		bool flag = changed.Count > 0;
		set.CheckResize(count, 2048);
		for (int i = 0; i < changed.Count; i++)
		{
			int num = changed[i];
			OccludeeState occludeeState = occludees[num];
			if (occludeeState != null)
			{
				if (!isStatic)
				{
					OcclusionCulling.UpdateInGrid(occludeeState);
				}
				set.inputData[num] = states[num].sphereBounds;
			}
			else
			{
				set.inputData[num] = Vector4.zero;
			}
		}
		changed.Clear();
		if (flag)
		{
			set.UploadData();
		}
	}

	// Token: 0x0600390D RID: 14605 RVA: 0x001506D8 File Offset: 0x0014E8D8
	private void UpdateCameraMatrices(bool starting = false)
	{
		if (!starting)
		{
			this.prevViewProjMatrix = this.viewProjMatrix;
		}
		Matrix4x4 proj = Matrix4x4.Perspective(this.camera.fieldOfView, this.camera.aspect, this.camera.nearClipPlane, this.camera.farClipPlane);
		this.viewMatrix = this.camera.worldToCameraMatrix;
		this.projMatrix = GL.GetGPUProjectionMatrix(proj, false);
		this.viewProjMatrix = this.projMatrix * this.viewMatrix;
		this.invViewProjMatrix = Matrix4x4.Inverse(this.viewProjMatrix);
		if (starting)
		{
			this.prevViewProjMatrix = this.viewProjMatrix;
		}
	}

	// Token: 0x0600390E RID: 14606 RVA: 0x0015077C File Offset: 0x0014E97C
	private void OnPreCull()
	{
		this.UpdateCameraMatrices(false);
		this.GenerateHiZMipChain();
		this.PrepareAndDispatch();
		this.IssueRead();
		if (OcclusionCulling.grid.Size <= OcclusionCulling.gridSet.resultData.Length)
		{
			this.RetrieveAndApplyVisibility();
			return;
		}
		Debug.LogWarning(string.Concat(new object[]
		{
			"[OcclusionCulling] Grid size and result capacity are out of sync: ",
			OcclusionCulling.grid.Size,
			", ",
			OcclusionCulling.gridSet.resultData.Length
		}));
	}

	// Token: 0x0600390F RID: 14607 RVA: 0x00150808 File Offset: 0x0014EA08
	private void OnPostRender()
	{
		bool sRGBWrite = GL.sRGBWrite;
		RenderBuffer activeColorBuffer = UnityEngine.Graphics.activeColorBuffer;
		RenderBuffer activeDepthBuffer = UnityEngine.Graphics.activeDepthBuffer;
		this.GrabDepthTexture();
		UnityEngine.Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
		GL.sRGBWrite = sRGBWrite;
	}

	// Token: 0x06003910 RID: 14608 RVA: 0x00150838 File Offset: 0x0014EA38
	private float[] MatrixToFloatArray(Matrix4x4 m)
	{
		int i = 0;
		int num = 0;
		while (i < 4)
		{
			for (int j = 0; j < 4; j++)
			{
				this.matrixToFloatTemp[num++] = m[j, i];
			}
			i++;
		}
		return this.matrixToFloatTemp;
	}

	// Token: 0x06003911 RID: 14609 RVA: 0x0015087C File Offset: 0x0014EA7C
	private void PrepareAndDispatch()
	{
		Vector2 v = new Vector2((float)this.hiZWidth, (float)this.hiZHeight);
		OcclusionCulling.ExtractFrustum(this.viewProjMatrix, ref this.frustumPlanes);
		bool flag = true;
		if (this.usePixelShaderFallback)
		{
			this.fallbackMat.SetTexture("_HiZMap", this.hiZTexture);
			this.fallbackMat.SetFloat("_HiZMaxLod", (float)(this.hiZLevelCount - 1));
			this.fallbackMat.SetMatrix("_ViewMatrix", this.viewMatrix);
			this.fallbackMat.SetMatrix("_ProjMatrix", this.projMatrix);
			this.fallbackMat.SetMatrix("_ViewProjMatrix", this.viewProjMatrix);
			this.fallbackMat.SetVector("_CameraWorldPos", base.transform.position);
			this.fallbackMat.SetVector("_ViewportSize", v);
			this.fallbackMat.SetFloat("_FrustumCull", flag ? 0f : 1f);
			for (int i = 0; i < 6; i++)
			{
				this.fallbackMat.SetVector(this.frustumPropNames[i], this.frustumPlanes[i]);
			}
		}
		else
		{
			this.computeShader.SetTexture(0, "_HiZMap", this.hiZTexture);
			this.computeShader.SetFloat("_HiZMaxLod", (float)(this.hiZLevelCount - 1));
			this.computeShader.SetFloats("_ViewMatrix", this.MatrixToFloatArray(this.viewMatrix));
			this.computeShader.SetFloats("_ProjMatrix", this.MatrixToFloatArray(this.projMatrix));
			this.computeShader.SetFloats("_ViewProjMatrix", this.MatrixToFloatArray(this.viewProjMatrix));
			this.computeShader.SetVector("_CameraWorldPos", base.transform.position);
			this.computeShader.SetVector("_ViewportSize", v);
			this.computeShader.SetFloat("_FrustumCull", flag ? 0f : 1f);
			for (int j = 0; j < 6; j++)
			{
				this.computeShader.SetVector(this.frustumPropNames[j], this.frustumPlanes[j]);
			}
		}
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			this.UpdateBuffers(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticSet, OcclusionCulling.staticChanged, true);
			OcclusionCulling.staticSet.Dispatch(OcclusionCulling.staticOccludees.Count);
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			this.UpdateBuffers(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicChanged, false);
			OcclusionCulling.dynamicSet.Dispatch(OcclusionCulling.dynamicOccludees.Count);
		}
		this.UpdateGridBuffers();
		OcclusionCulling.gridSet.Dispatch(OcclusionCulling.grid.Size);
	}

	// Token: 0x06003912 RID: 14610 RVA: 0x00150B48 File Offset: 0x0014ED48
	private void IssueRead()
	{
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			OcclusionCulling.staticSet.IssueRead();
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			OcclusionCulling.dynamicSet.IssueRead();
		}
		if (OcclusionCulling.grid.Count > 0)
		{
			OcclusionCulling.gridSet.IssueRead();
		}
		GL.IssuePluginEvent(RustNative.Graphics.GetRenderEventFunc(), 2);
	}

	// Token: 0x06003913 RID: 14611 RVA: 0x00150BA8 File Offset: 0x0014EDA8
	public void ResetTiming(OcclusionCulling.SmartList bucket)
	{
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState occludeeState = bucket[i];
			if (occludeeState != null)
			{
				occludeeState.states.array[occludeeState.slot].waitTime = 0f;
			}
		}
	}

	// Token: 0x06003914 RID: 14612 RVA: 0x00150BF4 File Offset: 0x0014EDF4
	public void ResetTiming()
	{
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null)
			{
				this.ResetTiming(cell.staticBucket);
				this.ResetTiming(cell.dynamicBucket);
			}
		}
	}

	// Token: 0x06003915 RID: 14613 RVA: 0x00150C40 File Offset: 0x0014EE40
	private static bool FrustumCull(Vector4[] planes, Vector4 testSphere)
	{
		for (int i = 0; i < 6; i++)
		{
			if (planes[i].x * testSphere.x + planes[i].y * testSphere.y + planes[i].z * testSphere.z + planes[i].w < -testSphere.w)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003916 RID: 14614 RVA: 0x00150CB0 File Offset: 0x0014EEB0
	private static int ProcessOccludees_Safe(OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.SmartList bucket, Color32[] results, OcclusionCulling.SimpleList<int> changed, Vector4[] frustumPlanes, float time, uint frame)
	{
		int num = 0;
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState occludeeState = bucket[i];
			if (occludeeState != null && occludeeState.slot < results.Length)
			{
				int slot = occludeeState.slot;
				OccludeeState.State state = states[slot];
				bool flag = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
				bool flag2 = results[slot].r > 0 && flag;
				if (flag2 || frame < state.waitFrame)
				{
					state.waitTime = time + state.minTimeVisible;
				}
				if (!flag2)
				{
					flag2 = (time < state.waitTime);
				}
				if (flag2 != state.isVisible > 0)
				{
					if (state.callback != 0)
					{
						changed.Add(slot);
					}
					else
					{
						state.isVisible = (flag2 ? 1 : 0);
					}
				}
				states[slot] = state;
				num += (int)state.isVisible;
			}
		}
		return num;
	}

	// Token: 0x06003917 RID: 14615 RVA: 0x00150D9C File Offset: 0x0014EF9C
	private static int ProcessOccludees_Fast(OccludeeState.State[] states, int[] bucket, int bucketCount, Color32[] results, int resultCount, int[] changed, ref int changedCount, Vector4[] frustumPlanes, float time, uint frame)
	{
		int num = 0;
		for (int i = 0; i < bucketCount; i++)
		{
			int num2 = bucket[i];
			if (num2 >= 0 && num2 < resultCount && states[num2].active != 0)
			{
				OccludeeState.State state = states[num2];
				bool flag = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
				bool flag2 = results[num2].r > 0 && flag;
				if (flag2 || frame < state.waitFrame)
				{
					state.waitTime = time + state.minTimeVisible;
				}
				if (!flag2)
				{
					flag2 = (time < state.waitTime);
				}
				if (flag2 != state.isVisible > 0)
				{
					if (state.callback != 0)
					{
						int num3 = changedCount;
						changedCount = num3 + 1;
						changed[num3] = num2;
					}
					else
					{
						state.isVisible = (flag2 ? 1 : 0);
					}
				}
				states[num2] = state;
				num += (flag2 ? 0 : 1);
			}
		}
		return num;
	}

	// Token: 0x06003918 RID: 14616
	[DllImport("Renderer", EntryPoint = "CULL_ProcessOccludees")]
	private static extern int ProcessOccludees_Native(ref OccludeeState.State states, ref int bucket, int bucketCount, ref Color32 results, int resultCount, ref int changed, ref int changedCount, ref Vector4 frustumPlanes, float time, uint frame);

	// Token: 0x06003919 RID: 14617 RVA: 0x00150E88 File Offset: 0x0014F088
	private void ApplyVisibility_Safe(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.staticStates, cell.staticBucket, OcclusionCulling.staticSet.resultData, OcclusionCulling.staticVisibilityChanged, this.frustumPlanes, time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.dynamicStates, cell.dynamicBucket, OcclusionCulling.dynamicSet.resultData, OcclusionCulling.dynamicVisibilityChanged, this.frustumPlanes, time, frame);
					}
					cell.isVisible = (flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count);
				}
			}
		}
	}

	// Token: 0x0600391A RID: 14618 RVA: 0x00150FCC File Offset: 0x0014F1CC
	private void ApplyVisibility_Fast(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.staticStates.array, cell.staticBucket.Slots, cell.staticBucket.Size, OcclusionCulling.staticSet.resultData, OcclusionCulling.staticSet.resultData.Length, OcclusionCulling.staticVisibilityChanged.array, ref OcclusionCulling.staticVisibilityChanged.count, this.frustumPlanes, time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.dynamicStates.array, cell.dynamicBucket.Slots, cell.dynamicBucket.Size, OcclusionCulling.dynamicSet.resultData, OcclusionCulling.dynamicSet.resultData.Length, OcclusionCulling.dynamicVisibilityChanged.array, ref OcclusionCulling.dynamicVisibilityChanged.count, this.frustumPlanes, time, frame);
					}
					cell.isVisible = (flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count);
				}
			}
		}
	}

	// Token: 0x0600391B RID: 14619 RVA: 0x00151170 File Offset: 0x0014F370
	private void ApplyVisibility_Native(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.staticStates.array[0], ref cell.staticBucket.Slots[0], cell.staticBucket.Size, ref OcclusionCulling.staticSet.resultData[0], OcclusionCulling.staticSet.resultData.Length, ref OcclusionCulling.staticVisibilityChanged.array[0], ref OcclusionCulling.staticVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.dynamicStates.array[0], ref cell.dynamicBucket.Slots[0], cell.dynamicBucket.Size, ref OcclusionCulling.dynamicSet.resultData[0], OcclusionCulling.dynamicSet.resultData.Length, ref OcclusionCulling.dynamicVisibilityChanged.array[0], ref OcclusionCulling.dynamicVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
					}
					cell.isVisible = (flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count);
				}
			}
		}
	}

	// Token: 0x0600391C RID: 14620 RVA: 0x00151358 File Offset: 0x0014F558
	private void ProcessCallbacks(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.SimpleList<int> changed)
	{
		for (int i = 0; i < changed.Count; i++)
		{
			int num = changed[i];
			OccludeeState occludeeState = occludees[num];
			if (occludeeState != null)
			{
				bool flag = states.array[num].isVisible == 0;
				OcclusionCulling.OnVisibilityChanged onVisibilityChanged = occludeeState.onVisibilityChanged;
				if (onVisibilityChanged != null && (UnityEngine.Object)onVisibilityChanged.Target != null)
				{
					onVisibilityChanged(flag);
				}
				if (occludeeState.slot >= 0)
				{
					states.array[occludeeState.slot].isVisible = (flag ? 1 : 0);
				}
			}
		}
		changed.Clear();
	}

	// Token: 0x0600391D RID: 14621 RVA: 0x001513F8 File Offset: 0x0014F5F8
	public void RetrieveAndApplyVisibility()
	{
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			OcclusionCulling.staticSet.GetResults();
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			OcclusionCulling.dynamicSet.GetResults();
		}
		if (OcclusionCulling.grid.Count > 0)
		{
			OcclusionCulling.gridSet.GetResults();
		}
		if (this.debugSettings.showAllVisible)
		{
			for (int i = 0; i < OcclusionCulling.staticSet.resultData.Length; i++)
			{
				OcclusionCulling.staticSet.resultData[i].r = 1;
			}
			for (int j = 0; j < OcclusionCulling.dynamicSet.resultData.Length; j++)
			{
				OcclusionCulling.dynamicSet.resultData[j].r = 1;
			}
			for (int k = 0; k < OcclusionCulling.gridSet.resultData.Length; k++)
			{
				OcclusionCulling.gridSet.resultData[k].r = 1;
			}
		}
		OcclusionCulling.staticVisibilityChanged.EnsureCapacity(OcclusionCulling.staticOccludees.Count);
		OcclusionCulling.dynamicVisibilityChanged.EnsureCapacity(OcclusionCulling.dynamicOccludees.Count);
		float time = Time.time;
		uint frameCount = (uint)Time.frameCount;
		if (this.useNativePath)
		{
			this.ApplyVisibility_Native(time, frameCount);
		}
		else
		{
			this.ApplyVisibility_Fast(time, frameCount);
		}
		this.ProcessCallbacks(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticVisibilityChanged);
		this.ProcessCallbacks(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicVisibilityChanged);
	}

	// Token: 0x04003343 RID: 13123
	public OcclusionCulling.DebugSettings debugSettings = new OcclusionCulling.DebugSettings();

	// Token: 0x04003344 RID: 13124
	private Material debugMipMat;

	// Token: 0x04003345 RID: 13125
	private const float debugDrawDuration = 0.0334f;

	// Token: 0x04003346 RID: 13126
	private Material downscaleMat;

	// Token: 0x04003347 RID: 13127
	private Material blitCopyMat;

	// Token: 0x04003348 RID: 13128
	private int hiZLevelCount;

	// Token: 0x04003349 RID: 13129
	private int hiZWidth;

	// Token: 0x0400334A RID: 13130
	private int hiZHeight;

	// Token: 0x0400334B RID: 13131
	private RenderTexture depthTexture;

	// Token: 0x0400334C RID: 13132
	private RenderTexture hiZTexture;

	// Token: 0x0400334D RID: 13133
	private RenderTexture[] hiZLevels;

	// Token: 0x0400334E RID: 13134
	private const int GridCellsPerAxis = 2097152;

	// Token: 0x0400334F RID: 13135
	private const int GridHalfCellsPerAxis = 1048576;

	// Token: 0x04003350 RID: 13136
	private const int GridMinHalfCellsPerAxis = -1048575;

	// Token: 0x04003351 RID: 13137
	private const int GridMaxHalfCellsPerAxis = 1048575;

	// Token: 0x04003352 RID: 13138
	private const float GridCellSize = 100f;

	// Token: 0x04003353 RID: 13139
	private const float GridHalfCellSize = 50f;

	// Token: 0x04003354 RID: 13140
	private const float GridRcpCellSize = 0.01f;

	// Token: 0x04003355 RID: 13141
	private const int GridPoolCapacity = 16384;

	// Token: 0x04003356 RID: 13142
	private const int GridPoolGranularity = 4096;

	// Token: 0x04003357 RID: 13143
	private static OcclusionCulling.HashedPool<OcclusionCulling.Cell> grid = new OcclusionCulling.HashedPool<OcclusionCulling.Cell>(16384, 4096);

	// Token: 0x04003358 RID: 13144
	private static Queue<OcclusionCulling.Cell> gridChanged = new Queue<OcclusionCulling.Cell>();

	// Token: 0x04003359 RID: 13145
	public ComputeShader computeShader;

	// Token: 0x0400335A RID: 13146
	public bool usePixelShaderFallback = true;

	// Token: 0x0400335B RID: 13147
	public bool useAsyncReadAPI;

	// Token: 0x0400335C RID: 13148
	private Camera camera;

	// Token: 0x0400335D RID: 13149
	private const int ComputeThreadsPerGroup = 64;

	// Token: 0x0400335E RID: 13150
	private const int InputBufferStride = 16;

	// Token: 0x0400335F RID: 13151
	private const int ResultBufferStride = 4;

	// Token: 0x04003360 RID: 13152
	private const int OccludeeMaxSlotsPerPool = 1048576;

	// Token: 0x04003361 RID: 13153
	private const int OccludeePoolGranularity = 2048;

	// Token: 0x04003362 RID: 13154
	private const int StateBufferGranularity = 2048;

	// Token: 0x04003363 RID: 13155
	private const int GridBufferGranularity = 256;

	// Token: 0x04003364 RID: 13156
	private static Queue<OccludeeState> statePool = new Queue<OccludeeState>();

	// Token: 0x04003365 RID: 13157
	private static OcclusionCulling.SimpleList<OccludeeState> staticOccludees = new OcclusionCulling.SimpleList<OccludeeState>(2048);

	// Token: 0x04003366 RID: 13158
	private static OcclusionCulling.SimpleList<OccludeeState.State> staticStates = new OcclusionCulling.SimpleList<OccludeeState.State>(2048);

	// Token: 0x04003367 RID: 13159
	private static OcclusionCulling.SimpleList<int> staticVisibilityChanged = new OcclusionCulling.SimpleList<int>(1024);

	// Token: 0x04003368 RID: 13160
	private static OcclusionCulling.SimpleList<OccludeeState> dynamicOccludees = new OcclusionCulling.SimpleList<OccludeeState>(2048);

	// Token: 0x04003369 RID: 13161
	private static OcclusionCulling.SimpleList<OccludeeState.State> dynamicStates = new OcclusionCulling.SimpleList<OccludeeState.State>(2048);

	// Token: 0x0400336A RID: 13162
	private static OcclusionCulling.SimpleList<int> dynamicVisibilityChanged = new OcclusionCulling.SimpleList<int>(1024);

	// Token: 0x0400336B RID: 13163
	private static List<int> staticChanged = new List<int>(256);

	// Token: 0x0400336C RID: 13164
	private static Queue<int> staticRecycled = new Queue<int>();

	// Token: 0x0400336D RID: 13165
	private static List<int> dynamicChanged = new List<int>(1024);

	// Token: 0x0400336E RID: 13166
	private static Queue<int> dynamicRecycled = new Queue<int>();

	// Token: 0x0400336F RID: 13167
	private static OcclusionCulling.BufferSet staticSet = new OcclusionCulling.BufferSet();

	// Token: 0x04003370 RID: 13168
	private static OcclusionCulling.BufferSet dynamicSet = new OcclusionCulling.BufferSet();

	// Token: 0x04003371 RID: 13169
	private static OcclusionCulling.BufferSet gridSet = new OcclusionCulling.BufferSet();

	// Token: 0x04003372 RID: 13170
	private Vector4[] frustumPlanes = new Vector4[6];

	// Token: 0x04003373 RID: 13171
	private string[] frustumPropNames = new string[6];

	// Token: 0x04003374 RID: 13172
	private float[] matrixToFloatTemp = new float[16];

	// Token: 0x04003375 RID: 13173
	private Material fallbackMat;

	// Token: 0x04003376 RID: 13174
	private Material depthCopyMat;

	// Token: 0x04003377 RID: 13175
	private Matrix4x4 viewMatrix;

	// Token: 0x04003378 RID: 13176
	private Matrix4x4 projMatrix;

	// Token: 0x04003379 RID: 13177
	private Matrix4x4 viewProjMatrix;

	// Token: 0x0400337A RID: 13178
	private Matrix4x4 prevViewProjMatrix;

	// Token: 0x0400337B RID: 13179
	private Matrix4x4 invViewProjMatrix;

	// Token: 0x0400337C RID: 13180
	private bool useNativePath = true;

	// Token: 0x0400337D RID: 13181
	private static OcclusionCulling instance;

	// Token: 0x0400337E RID: 13182
	private static GraphicsDeviceType[] supportedDeviceTypes = new GraphicsDeviceType[]
	{
		GraphicsDeviceType.Direct3D11
	};

	// Token: 0x0400337F RID: 13183
	private static bool _enabled = false;

	// Token: 0x04003380 RID: 13184
	private static bool _safeMode = false;

	// Token: 0x04003381 RID: 13185
	private static OcclusionCulling.DebugFilter _debugShow = OcclusionCulling.DebugFilter.Off;

	// Token: 0x02000E77 RID: 3703
	public class BufferSet
	{
		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x060050A4 RID: 20644 RVA: 0x001A224F File Offset: 0x001A044F
		public bool Ready
		{
			get
			{
				return this.resultData.Length != 0;
			}
		}

		// Token: 0x060050A5 RID: 20645 RVA: 0x001A225B File Offset: 0x001A045B
		public void Attach(OcclusionCulling culling)
		{
			this.culling = culling;
		}

		// Token: 0x060050A6 RID: 20646 RVA: 0x001A2264 File Offset: 0x001A0464
		public void Dispose(bool data = true)
		{
			if (this.inputBuffer != null)
			{
				this.inputBuffer.Dispose();
				this.inputBuffer = null;
			}
			if (this.resultBuffer != null)
			{
				this.resultBuffer.Dispose();
				this.resultBuffer = null;
			}
			if (this.inputTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.inputTexture);
				this.inputTexture = null;
			}
			if (this.resultTexture != null)
			{
				RenderTexture.active = null;
				this.resultTexture.Release();
				UnityEngine.Object.DestroyImmediate(this.resultTexture);
				this.resultTexture = null;
			}
			if (this.resultReadTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.resultReadTexture);
				this.resultReadTexture = null;
			}
			if (this.readbackInst != IntPtr.Zero)
			{
				RustNative.Graphics.BufferReadback.Destroy(this.readbackInst);
				this.readbackInst = IntPtr.Zero;
			}
			if (data)
			{
				this.inputData = new Color[0];
				this.resultData = new Color32[0];
				this.capacity = 0;
				this.count = 0;
			}
		}

		// Token: 0x060050A7 RID: 20647 RVA: 0x001A2368 File Offset: 0x001A0568
		public bool CheckResize(int count, int granularity)
		{
			if (count > this.capacity || (this.culling.usePixelShaderFallback && this.resultTexture != null && !this.resultTexture.IsCreated()))
			{
				this.Dispose(false);
				int num = this.capacity;
				int num2 = count / granularity * granularity + granularity;
				if (this.culling.usePixelShaderFallback)
				{
					this.width = Mathf.CeilToInt(Mathf.Sqrt((float)num2));
					this.height = Mathf.CeilToInt((float)num2 / (float)this.width);
					this.inputTexture = new Texture2D(this.width, this.height, TextureFormat.RGBAFloat, false, true);
					this.inputTexture.name = "_Input";
					this.inputTexture.filterMode = FilterMode.Point;
					this.inputTexture.wrapMode = TextureWrapMode.Clamp;
					this.resultTexture = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
					this.resultTexture.name = "_Result";
					this.resultTexture.filterMode = FilterMode.Point;
					this.resultTexture.wrapMode = TextureWrapMode.Clamp;
					this.resultTexture.useMipMap = false;
					this.resultTexture.Create();
					this.resultReadTexture = new Texture2D(this.width, this.height, TextureFormat.ARGB32, false, true);
					this.resultReadTexture.name = "_ResultRead";
					this.resultReadTexture.filterMode = FilterMode.Point;
					this.resultReadTexture.wrapMode = TextureWrapMode.Clamp;
					if (!this.culling.useAsyncReadAPI)
					{
						this.readbackInst = RustNative.Graphics.BufferReadback.CreateForTexture(this.resultTexture.GetNativeTexturePtr(), (uint)this.width, (uint)this.height, (uint)this.resultTexture.format);
					}
					this.capacity = this.width * this.height;
				}
				else
				{
					this.inputBuffer = new ComputeBuffer(num2, 16);
					this.resultBuffer = new ComputeBuffer(num2, 4);
					if (!this.culling.useAsyncReadAPI)
					{
						uint size = (uint)(this.capacity * 4);
						this.readbackInst = RustNative.Graphics.BufferReadback.CreateForBuffer(this.resultBuffer.GetNativeBufferPtr(), size);
					}
					this.capacity = num2;
				}
				Array.Resize<Color>(ref this.inputData, this.capacity);
				Array.Resize<Color32>(ref this.resultData, this.capacity);
				Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				for (int i = num; i < this.capacity; i++)
				{
					this.resultData[i] = color;
				}
				this.count = count;
				return true;
			}
			return false;
		}

		// Token: 0x060050A8 RID: 20648 RVA: 0x001A25DC File Offset: 0x001A07DC
		public void UploadData()
		{
			if (this.culling.usePixelShaderFallback)
			{
				this.inputTexture.SetPixels(this.inputData);
				this.inputTexture.Apply();
				return;
			}
			this.inputBuffer.SetData(this.inputData);
		}

		// Token: 0x060050A9 RID: 20649 RVA: 0x001A2619 File Offset: 0x001A0819
		private int AlignDispatchSize(int dispatchSize)
		{
			return (dispatchSize + 63) / 64;
		}

		// Token: 0x060050AA RID: 20650 RVA: 0x001A2624 File Offset: 0x001A0824
		public void Dispatch(int count)
		{
			if (this.culling.usePixelShaderFallback)
			{
				RenderBuffer activeColorBuffer = UnityEngine.Graphics.activeColorBuffer;
				RenderBuffer activeDepthBuffer = UnityEngine.Graphics.activeDepthBuffer;
				this.culling.fallbackMat.SetTexture("_Input", this.inputTexture);
				UnityEngine.Graphics.Blit(this.inputTexture, this.resultTexture, this.culling.fallbackMat, 0);
				UnityEngine.Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
				return;
			}
			if (this.inputBuffer != null)
			{
				this.culling.computeShader.SetBuffer(0, "_Input", this.inputBuffer);
				this.culling.computeShader.SetBuffer(0, "_Result", this.resultBuffer);
				this.culling.computeShader.Dispatch(0, this.AlignDispatchSize(count), 1, 1);
			}
		}

		// Token: 0x060050AB RID: 20651 RVA: 0x001A26E4 File Offset: 0x001A08E4
		public void IssueRead()
		{
			if (!OcclusionCulling.SafeMode)
			{
				if (this.culling.useAsyncReadAPI)
				{
					if (this.asyncRequests.Count < 10)
					{
						AsyncGPUReadbackRequest item;
						if (this.culling.usePixelShaderFallback)
						{
							item = AsyncGPUReadback.Request(this.resultTexture, 0, null);
						}
						else
						{
							item = AsyncGPUReadback.Request(this.resultBuffer, null);
						}
						this.asyncRequests.Enqueue(item);
						return;
					}
				}
				else if (this.readbackInst != IntPtr.Zero)
				{
					RustNative.Graphics.BufferReadback.IssueRead(this.readbackInst);
				}
			}
		}

		// Token: 0x060050AC RID: 20652 RVA: 0x001A2768 File Offset: 0x001A0968
		public void GetResults()
		{
			if (this.resultData != null && this.resultData.Length != 0)
			{
				if (!OcclusionCulling.SafeMode)
				{
					if (this.culling.useAsyncReadAPI)
					{
						while (this.asyncRequests.Count > 0)
						{
							AsyncGPUReadbackRequest asyncGPUReadbackRequest = this.asyncRequests.Peek();
							if (asyncGPUReadbackRequest.hasError)
							{
								this.asyncRequests.Dequeue();
							}
							else
							{
								if (!asyncGPUReadbackRequest.done)
								{
									return;
								}
								NativeArray<Color32> data = asyncGPUReadbackRequest.GetData<Color32>(0);
								for (int i = 0; i < data.Length; i++)
								{
									this.resultData[i] = data[i];
								}
								this.asyncRequests.Dequeue();
							}
						}
						return;
					}
					if (this.readbackInst != IntPtr.Zero)
					{
						RustNative.Graphics.BufferReadback.GetData(this.readbackInst, ref this.resultData[0]);
						return;
					}
				}
				else
				{
					if (this.culling.usePixelShaderFallback)
					{
						RenderTexture.active = this.resultTexture;
						this.resultReadTexture.ReadPixels(new Rect(0f, 0f, (float)this.width, (float)this.height), 0, 0);
						this.resultReadTexture.Apply();
						Array.Copy(this.resultReadTexture.GetPixels32(), this.resultData, this.resultData.Length);
						return;
					}
					this.resultBuffer.GetData(this.resultData);
				}
			}
		}

		// Token: 0x04004A99 RID: 19097
		public ComputeBuffer inputBuffer;

		// Token: 0x04004A9A RID: 19098
		public ComputeBuffer resultBuffer;

		// Token: 0x04004A9B RID: 19099
		public int width;

		// Token: 0x04004A9C RID: 19100
		public int height;

		// Token: 0x04004A9D RID: 19101
		public int capacity;

		// Token: 0x04004A9E RID: 19102
		public int count;

		// Token: 0x04004A9F RID: 19103
		public Texture2D inputTexture;

		// Token: 0x04004AA0 RID: 19104
		public RenderTexture resultTexture;

		// Token: 0x04004AA1 RID: 19105
		public Texture2D resultReadTexture;

		// Token: 0x04004AA2 RID: 19106
		public Color[] inputData = new Color[0];

		// Token: 0x04004AA3 RID: 19107
		public Color32[] resultData = new Color32[0];

		// Token: 0x04004AA4 RID: 19108
		private OcclusionCulling culling;

		// Token: 0x04004AA5 RID: 19109
		private const int MaxAsyncGPUReadbackRequests = 10;

		// Token: 0x04004AA6 RID: 19110
		private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();

		// Token: 0x04004AA7 RID: 19111
		public IntPtr readbackInst = IntPtr.Zero;
	}

	// Token: 0x02000E78 RID: 3704
	public enum DebugFilter
	{
		// Token: 0x04004AA9 RID: 19113
		Off,
		// Token: 0x04004AAA RID: 19114
		Dynamic,
		// Token: 0x04004AAB RID: 19115
		Static,
		// Token: 0x04004AAC RID: 19116
		Grid,
		// Token: 0x04004AAD RID: 19117
		All
	}

	// Token: 0x02000E79 RID: 3705
	[Flags]
	public enum DebugMask
	{
		// Token: 0x04004AAF RID: 19119
		Off = 0,
		// Token: 0x04004AB0 RID: 19120
		Dynamic = 1,
		// Token: 0x04004AB1 RID: 19121
		Static = 2,
		// Token: 0x04004AB2 RID: 19122
		Grid = 4,
		// Token: 0x04004AB3 RID: 19123
		All = 7
	}

	// Token: 0x02000E7A RID: 3706
	[Serializable]
	public class DebugSettings
	{
		// Token: 0x04004AB4 RID: 19124
		public bool log;

		// Token: 0x04004AB5 RID: 19125
		public bool showAllVisible;

		// Token: 0x04004AB6 RID: 19126
		public bool showMipChain;

		// Token: 0x04004AB7 RID: 19127
		public bool showMain;

		// Token: 0x04004AB8 RID: 19128
		public int showMainLod;

		// Token: 0x04004AB9 RID: 19129
		public bool showFallback;

		// Token: 0x04004ABA RID: 19130
		public bool showStats;

		// Token: 0x04004ABB RID: 19131
		public bool showScreenBounds;

		// Token: 0x04004ABC RID: 19132
		public OcclusionCulling.DebugMask showMask;

		// Token: 0x04004ABD RID: 19133
		public LayerMask layerFilter = -1;
	}

	// Token: 0x02000E7B RID: 3707
	public class HashedPoolValue
	{
		// Token: 0x04004ABE RID: 19134
		public ulong hashedPoolKey = ulong.MaxValue;

		// Token: 0x04004ABF RID: 19135
		public int hashedPoolIndex = -1;
	}

	// Token: 0x02000E7C RID: 3708
	public class HashedPool<ValueType> where ValueType : OcclusionCulling.HashedPoolValue, new()
	{
		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x060050B0 RID: 20656 RVA: 0x001A2929 File Offset: 0x001A0B29
		public int Size
		{
			get
			{
				return this.list.Count;
			}
		}

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x060050B1 RID: 20657 RVA: 0x001A2936 File Offset: 0x001A0B36
		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		// Token: 0x17000694 RID: 1684
		public ValueType this[int i]
		{
			get
			{
				return this.list[i];
			}
			set
			{
				this.list[i] = value;
			}
		}

		// Token: 0x060050B4 RID: 20660 RVA: 0x001A2960 File Offset: 0x001A0B60
		public HashedPool(int capacity, int granularity)
		{
			this.granularity = granularity;
			this.dict = new Dictionary<ulong, ValueType>(capacity);
			this.pool = new List<ValueType>(capacity);
			this.list = new List<ValueType>(capacity);
			this.recycled = new Queue<ValueType>();
		}

		// Token: 0x060050B5 RID: 20661 RVA: 0x001A299E File Offset: 0x001A0B9E
		public void Clear()
		{
			this.dict.Clear();
			this.pool.Clear();
			this.list.Clear();
			this.recycled.Clear();
		}

		// Token: 0x060050B6 RID: 20662 RVA: 0x001A29CC File Offset: 0x001A0BCC
		public ValueType Add(ulong key, int capacityGranularity = 16)
		{
			ValueType valueType;
			if (this.recycled.Count > 0)
			{
				valueType = this.recycled.Dequeue();
				this.list[valueType.hashedPoolIndex] = valueType;
			}
			else
			{
				int count = this.pool.Count;
				if (count == this.pool.Capacity)
				{
					this.pool.Capacity += this.granularity;
				}
				valueType = Activator.CreateInstance<ValueType>();
				valueType.hashedPoolIndex = count;
				this.pool.Add(valueType);
				this.list.Add(valueType);
			}
			valueType.hashedPoolKey = key;
			this.dict.Add(key, valueType);
			return valueType;
		}

		// Token: 0x060050B7 RID: 20663 RVA: 0x001A2A84 File Offset: 0x001A0C84
		public void Remove(ValueType value)
		{
			this.dict.Remove(value.hashedPoolKey);
			this.list[value.hashedPoolIndex] = default(ValueType);
			this.recycled.Enqueue(value);
			value.hashedPoolKey = ulong.MaxValue;
		}

		// Token: 0x060050B8 RID: 20664 RVA: 0x001A2AE0 File Offset: 0x001A0CE0
		public bool TryGetValue(ulong key, out ValueType value)
		{
			return this.dict.TryGetValue(key, out value);
		}

		// Token: 0x060050B9 RID: 20665 RVA: 0x001A2AEF File Offset: 0x001A0CEF
		public bool ContainsKey(ulong key)
		{
			return this.dict.ContainsKey(key);
		}

		// Token: 0x04004AC0 RID: 19136
		private int granularity;

		// Token: 0x04004AC1 RID: 19137
		private Dictionary<ulong, ValueType> dict;

		// Token: 0x04004AC2 RID: 19138
		private List<ValueType> pool;

		// Token: 0x04004AC3 RID: 19139
		private List<ValueType> list;

		// Token: 0x04004AC4 RID: 19140
		private Queue<ValueType> recycled;
	}

	// Token: 0x02000E7D RID: 3709
	public class SimpleList<T>
	{
		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x060050BA RID: 20666 RVA: 0x001A2AFD File Offset: 0x001A0CFD
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x060050BB RID: 20667 RVA: 0x001A2B05 File Offset: 0x001A0D05
		// (set) Token: 0x060050BC RID: 20668 RVA: 0x001A2B10 File Offset: 0x001A0D10
		public int Capacity
		{
			get
			{
				return this.array.Length;
			}
			set
			{
				if (value != this.array.Length)
				{
					if (value > 0)
					{
						T[] destinationArray = new T[value];
						if (this.count > 0)
						{
							Array.Copy(this.array, 0, destinationArray, 0, this.count);
						}
						this.array = destinationArray;
						return;
					}
					this.array = OcclusionCulling.SimpleList<T>.emptyArray;
				}
			}
		}

		// Token: 0x17000697 RID: 1687
		public T this[int index]
		{
			get
			{
				return this.array[index];
			}
			set
			{
				this.array[index] = value;
			}
		}

		// Token: 0x060050BF RID: 20671 RVA: 0x001A2B80 File Offset: 0x001A0D80
		public SimpleList()
		{
			this.array = OcclusionCulling.SimpleList<T>.emptyArray;
		}

		// Token: 0x060050C0 RID: 20672 RVA: 0x001A2B93 File Offset: 0x001A0D93
		public SimpleList(int capacity)
		{
			this.array = ((capacity == 0) ? OcclusionCulling.SimpleList<T>.emptyArray : new T[capacity]);
		}

		// Token: 0x060050C1 RID: 20673 RVA: 0x001A2BB4 File Offset: 0x001A0DB4
		public void Add(T item)
		{
			if (this.count == this.array.Length)
			{
				this.EnsureCapacity(this.count + 1);
			}
			T[] array = this.array;
			int num = this.count;
			this.count = num + 1;
			array[num] = item;
		}

		// Token: 0x060050C2 RID: 20674 RVA: 0x001A2BFC File Offset: 0x001A0DFC
		public void Clear()
		{
			if (this.count > 0)
			{
				Array.Clear(this.array, 0, this.count);
				this.count = 0;
			}
		}

		// Token: 0x060050C3 RID: 20675 RVA: 0x001A2C20 File Offset: 0x001A0E20
		public bool Contains(T item)
		{
			for (int i = 0; i < this.count; i++)
			{
				if (this.array[i].Equals(item))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060050C4 RID: 20676 RVA: 0x001A2C62 File Offset: 0x001A0E62
		public void CopyTo(T[] array)
		{
			Array.Copy(this.array, 0, array, 0, this.count);
		}

		// Token: 0x060050C5 RID: 20677 RVA: 0x001A2C78 File Offset: 0x001A0E78
		public void EnsureCapacity(int min)
		{
			if (this.array.Length < min)
			{
				int num = (this.array.Length == 0) ? 16 : (this.array.Length * 2);
				num = ((num < min) ? min : num);
				this.Capacity = num;
			}
		}

		// Token: 0x04004AC5 RID: 19141
		private const int defaultCapacity = 16;

		// Token: 0x04004AC6 RID: 19142
		private static readonly T[] emptyArray = new T[0];

		// Token: 0x04004AC7 RID: 19143
		public T[] array;

		// Token: 0x04004AC8 RID: 19144
		public int count;
	}

	// Token: 0x02000E7E RID: 3710
	public class SmartListValue
	{
		// Token: 0x04004AC9 RID: 19145
		public int hashedListIndex = -1;
	}

	// Token: 0x02000E7F RID: 3711
	public class SmartList
	{
		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x060050C8 RID: 20680 RVA: 0x001A2CD4 File Offset: 0x001A0ED4
		public OccludeeState[] List
		{
			get
			{
				return this.list;
			}
		}

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x060050C9 RID: 20681 RVA: 0x001A2CDC File Offset: 0x001A0EDC
		public int[] Slots
		{
			get
			{
				return this.slots;
			}
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x060050CA RID: 20682 RVA: 0x001A2CE4 File Offset: 0x001A0EE4
		public int Size
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x060050CB RID: 20683 RVA: 0x001A2CEC File Offset: 0x001A0EEC
		public int Count
		{
			get
			{
				return this.count - this.recycled.Count;
			}
		}

		// Token: 0x1700069C RID: 1692
		public OccludeeState this[int i]
		{
			get
			{
				return this.list[i];
			}
			set
			{
				this.list[i] = value;
			}
		}

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x060050CE RID: 20686 RVA: 0x001A2D15 File Offset: 0x001A0F15
		// (set) Token: 0x060050CF RID: 20687 RVA: 0x001A2D20 File Offset: 0x001A0F20
		public int Capacity
		{
			get
			{
				return this.list.Length;
			}
			set
			{
				if (value != this.list.Length)
				{
					if (value > 0)
					{
						OccludeeState[] destinationArray = new OccludeeState[value];
						int[] destinationArray2 = new int[value];
						if (this.count > 0)
						{
							Array.Copy(this.list, destinationArray, this.count);
							Array.Copy(this.slots, destinationArray2, this.count);
						}
						this.list = destinationArray;
						this.slots = destinationArray2;
						return;
					}
					this.list = OcclusionCulling.SmartList.emptyList;
					this.slots = OcclusionCulling.SmartList.emptySlots;
				}
			}
		}

		// Token: 0x060050D0 RID: 20688 RVA: 0x001A2D9C File Offset: 0x001A0F9C
		public SmartList(int capacity)
		{
			this.list = new OccludeeState[capacity];
			this.slots = new int[capacity];
			this.recycled = new Queue<int>();
			this.count = 0;
		}

		// Token: 0x060050D1 RID: 20689 RVA: 0x001A2DD0 File Offset: 0x001A0FD0
		public void Add(OccludeeState value, int capacityGranularity = 16)
		{
			int num;
			if (this.recycled.Count > 0)
			{
				num = this.recycled.Dequeue();
				this.list[num] = value;
				this.slots[num] = value.slot;
			}
			else
			{
				num = this.count;
				if (num == this.list.Length)
				{
					this.EnsureCapacity(this.count + 1);
				}
				this.list[num] = value;
				this.slots[num] = value.slot;
				this.count++;
			}
			value.hashedListIndex = num;
		}

		// Token: 0x060050D2 RID: 20690 RVA: 0x001A2E5C File Offset: 0x001A105C
		public void Remove(OccludeeState value)
		{
			int hashedListIndex = value.hashedListIndex;
			this.list[hashedListIndex] = null;
			this.slots[hashedListIndex] = -1;
			this.recycled.Enqueue(hashedListIndex);
			value.hashedListIndex = -1;
		}

		// Token: 0x060050D3 RID: 20691 RVA: 0x001A2E98 File Offset: 0x001A1098
		public bool Contains(OccludeeState value)
		{
			int hashedListIndex = value.hashedListIndex;
			return hashedListIndex >= 0 && this.list[hashedListIndex] != null;
		}

		// Token: 0x060050D4 RID: 20692 RVA: 0x001A2EC0 File Offset: 0x001A10C0
		public void EnsureCapacity(int min)
		{
			if (this.list.Length < min)
			{
				int num = (this.list.Length == 0) ? 16 : (this.list.Length * 2);
				num = ((num < min) ? min : num);
				this.Capacity = num;
			}
		}

		// Token: 0x04004ACA RID: 19146
		private const int defaultCapacity = 16;

		// Token: 0x04004ACB RID: 19147
		private static readonly OccludeeState[] emptyList = new OccludeeState[0];

		// Token: 0x04004ACC RID: 19148
		private static readonly int[] emptySlots = new int[0];

		// Token: 0x04004ACD RID: 19149
		private OccludeeState[] list;

		// Token: 0x04004ACE RID: 19150
		private int[] slots;

		// Token: 0x04004ACF RID: 19151
		private Queue<int> recycled;

		// Token: 0x04004AD0 RID: 19152
		private int count;
	}

	// Token: 0x02000E80 RID: 3712
	[Serializable]
	public class Cell : OcclusionCulling.HashedPoolValue
	{
		// Token: 0x060050D6 RID: 20694 RVA: 0x001A2F18 File Offset: 0x001A1118
		public void Reset()
		{
			this.x = (this.y = (this.z = 0));
			this.bounds = default(Bounds);
			this.sphereBounds = Vector4.zero;
			this.isVisible = true;
			this.staticBucket = null;
			this.dynamicBucket = null;
		}

		// Token: 0x060050D7 RID: 20695 RVA: 0x001A2F6C File Offset: 0x001A116C
		public OcclusionCulling.Cell Initialize(int x, int y, int z, Bounds bounds)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.bounds = bounds;
			this.sphereBounds = new Vector4(bounds.center.x, bounds.center.y, bounds.center.z, bounds.extents.magnitude);
			this.isVisible = true;
			this.staticBucket = new OcclusionCulling.SmartList(32);
			this.dynamicBucket = new OcclusionCulling.SmartList(32);
			return this;
		}

		// Token: 0x04004AD1 RID: 19153
		public int x;

		// Token: 0x04004AD2 RID: 19154
		public int y;

		// Token: 0x04004AD3 RID: 19155
		public int z;

		// Token: 0x04004AD4 RID: 19156
		public Bounds bounds;

		// Token: 0x04004AD5 RID: 19157
		public Vector4 sphereBounds;

		// Token: 0x04004AD6 RID: 19158
		public bool isVisible;

		// Token: 0x04004AD7 RID: 19159
		public OcclusionCulling.SmartList staticBucket;

		// Token: 0x04004AD8 RID: 19160
		public OcclusionCulling.SmartList dynamicBucket;
	}

	// Token: 0x02000E81 RID: 3713
	public struct Sphere
	{
		// Token: 0x060050D9 RID: 20697 RVA: 0x001A2FFE File Offset: 0x001A11FE
		public bool IsValid()
		{
			return this.radius > 0f;
		}

		// Token: 0x060050DA RID: 20698 RVA: 0x001A300D File Offset: 0x001A120D
		public Sphere(Vector3 position, float radius)
		{
			this.position = position;
			this.radius = radius;
		}

		// Token: 0x04004AD9 RID: 19161
		public Vector3 position;

		// Token: 0x04004ADA RID: 19162
		public float radius;
	}

	// Token: 0x02000E82 RID: 3714
	// (Invoke) Token: 0x060050DC RID: 20700
	public delegate void OnVisibilityChanged(bool visible);
}
