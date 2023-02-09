using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000706 RID: 1798
[ExecuteInEditMode]
public class ReflectionProbeEx : MonoBehaviour
{
	// Token: 0x060031C2 RID: 12738 RVA: 0x00130F9E File Offset: 0x0012F19E
	private void CreateMeshes()
	{
		if (this.blitMesh == null)
		{
			this.blitMesh = ReflectionProbeEx.CreateBlitMesh();
		}
		if (this.skyboxMesh == null)
		{
			this.skyboxMesh = ReflectionProbeEx.CreateSkyboxMesh();
		}
	}

	// Token: 0x060031C3 RID: 12739 RVA: 0x00130FD4 File Offset: 0x0012F1D4
	private void DestroyMeshes()
	{
		if (this.blitMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(this.blitMesh);
			this.blitMesh = null;
		}
		if (this.skyboxMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(this.skyboxMesh);
			this.skyboxMesh = null;
		}
	}

	// Token: 0x060031C4 RID: 12740 RVA: 0x00131024 File Offset: 0x0012F224
	private static Mesh CreateBlitMesh()
	{
		return new Mesh
		{
			vertices = new Vector3[]
			{
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f, 1f, 0f),
				new Vector3(1f, 1f, 0f),
				new Vector3(1f, -1f, 0f)
			},
			uv = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f)
			},
			triangles = new int[]
			{
				0,
				1,
				2,
				0,
				2,
				3
			}
		};
	}

	// Token: 0x060031C5 RID: 12741 RVA: 0x0013112C File Offset: 0x0012F32C
	private static ReflectionProbeEx.CubemapSkyboxVertex SubDivVert(ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2)
	{
		Vector3 a = new Vector3(v1.x, v1.y, v1.z);
		Vector3 b = new Vector3(v2.x, v2.y, v2.z);
		Vector3 vector = Vector3.Normalize(Vector3.Lerp(a, b, 0.5f));
		ReflectionProbeEx.CubemapSkyboxVertex result;
		result.x = (result.tu = vector.x);
		result.y = (result.tv = vector.y);
		result.z = (result.tw = vector.z);
		result.color = Color.white;
		return result;
	}

	// Token: 0x060031C6 RID: 12742 RVA: 0x001311CC File Offset: 0x0012F3CC
	private static void Subdivide(List<ReflectionProbeEx.CubemapSkyboxVertex> destArray, ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2, ReflectionProbeEx.CubemapSkyboxVertex v3)
	{
		ReflectionProbeEx.CubemapSkyboxVertex item = ReflectionProbeEx.SubDivVert(v1, v2);
		ReflectionProbeEx.CubemapSkyboxVertex item2 = ReflectionProbeEx.SubDivVert(v2, v3);
		ReflectionProbeEx.CubemapSkyboxVertex item3 = ReflectionProbeEx.SubDivVert(v1, v3);
		destArray.Add(v1);
		destArray.Add(item);
		destArray.Add(item3);
		destArray.Add(item);
		destArray.Add(v2);
		destArray.Add(item2);
		destArray.Add(item2);
		destArray.Add(item3);
		destArray.Add(item);
		destArray.Add(v3);
		destArray.Add(item3);
		destArray.Add(item2);
	}

	// Token: 0x060031C7 RID: 12743 RVA: 0x00131248 File Offset: 0x0012F448
	private static void SubdivideYOnly(List<ReflectionProbeEx.CubemapSkyboxVertex> destArray, ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2, ReflectionProbeEx.CubemapSkyboxVertex v3)
	{
		float num = Mathf.Abs(v2.y - v1.y);
		float num2 = Mathf.Abs(v2.y - v3.y);
		float num3 = Mathf.Abs(v3.y - v1.y);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex;
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex2;
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex3;
		if (num < num2 && num < num3)
		{
			cubemapSkyboxVertex = v3;
			cubemapSkyboxVertex2 = v1;
			cubemapSkyboxVertex3 = v2;
		}
		else if (num2 < num && num2 < num3)
		{
			cubemapSkyboxVertex = v1;
			cubemapSkyboxVertex2 = v2;
			cubemapSkyboxVertex3 = v3;
		}
		else
		{
			cubemapSkyboxVertex = v2;
			cubemapSkyboxVertex2 = v3;
			cubemapSkyboxVertex3 = v1;
		}
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex4 = ReflectionProbeEx.SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex2);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex5 = ReflectionProbeEx.SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(cubemapSkyboxVertex4);
		destArray.Add(cubemapSkyboxVertex5);
		Vector3 vector = new Vector3(cubemapSkyboxVertex5.x - cubemapSkyboxVertex2.x, cubemapSkyboxVertex5.y - cubemapSkyboxVertex2.y, cubemapSkyboxVertex5.z - cubemapSkyboxVertex2.z);
		Vector3 vector2 = new Vector3(cubemapSkyboxVertex4.x - cubemapSkyboxVertex3.x, cubemapSkyboxVertex4.y - cubemapSkyboxVertex3.y, cubemapSkyboxVertex4.z - cubemapSkyboxVertex3.z);
		if (vector.x * vector.x + vector.y * vector.y + vector.z * vector.z > vector2.x * vector2.x + vector2.y * vector2.y + vector2.z * vector2.z)
		{
			destArray.Add(cubemapSkyboxVertex4);
			destArray.Add(cubemapSkyboxVertex2);
			destArray.Add(cubemapSkyboxVertex3);
			destArray.Add(cubemapSkyboxVertex5);
			destArray.Add(cubemapSkyboxVertex4);
			destArray.Add(cubemapSkyboxVertex3);
			return;
		}
		destArray.Add(cubemapSkyboxVertex5);
		destArray.Add(cubemapSkyboxVertex4);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex5);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex3);
	}

	// Token: 0x060031C8 RID: 12744 RVA: 0x00131410 File Offset: 0x0012F610
	private static Mesh CreateSkyboxMesh()
	{
		List<ReflectionProbeEx.CubemapSkyboxVertex> list = new List<ReflectionProbeEx.CubemapSkyboxVertex>();
		for (int i = 0; i < 24; i++)
		{
			ReflectionProbeEx.CubemapSkyboxVertex item = default(ReflectionProbeEx.CubemapSkyboxVertex);
			Vector3 vector = Vector3.Normalize(new Vector3(ReflectionProbeEx.octaVerts[i * 3], ReflectionProbeEx.octaVerts[i * 3 + 1], ReflectionProbeEx.octaVerts[i * 3 + 2]));
			item.x = (item.tu = vector.x);
			item.y = (item.tv = vector.y);
			item.z = (item.tw = vector.z);
			item.color = Color.white;
			list.Add(item);
		}
		for (int j = 0; j < 3; j++)
		{
			List<ReflectionProbeEx.CubemapSkyboxVertex> list2 = new List<ReflectionProbeEx.CubemapSkyboxVertex>(list.Count);
			list2.AddRange(list);
			int count = list2.Count;
			list.Clear();
			list.Capacity = count * 4;
			for (int k = 0; k < count; k += 3)
			{
				ReflectionProbeEx.Subdivide(list, list2[k], list2[k + 1], list2[k + 2]);
			}
		}
		for (int l = 0; l < 2; l++)
		{
			List<ReflectionProbeEx.CubemapSkyboxVertex> list3 = new List<ReflectionProbeEx.CubemapSkyboxVertex>(list.Count);
			list3.AddRange(list);
			int count2 = list3.Count;
			float num = Mathf.Pow(0.5f, (float)l + 1f);
			list.Clear();
			list.Capacity = count2 * 4;
			for (int m = 0; m < count2; m += 3)
			{
				if (Mathf.Max(Mathf.Max(Mathf.Abs(list3[m].y), Mathf.Abs(list3[m + 1].y)), Mathf.Abs(list3[m + 2].y)) > num)
				{
					list.Add(list3[m]);
					list.Add(list3[m + 1]);
					list.Add(list3[m + 2]);
				}
				else
				{
					ReflectionProbeEx.SubdivideYOnly(list, list3[m], list3[m + 1], list3[m + 2]);
				}
			}
		}
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[list.Count];
		Vector2[] array2 = new Vector2[list.Count];
		int[] array3 = new int[list.Count];
		for (int n = 0; n < list.Count; n++)
		{
			array[n] = new Vector3(list[n].x, list[n].y, list[n].z);
			array2[n] = new Vector3(list[n].tu, list[n].tv);
			array3[n] = n;
		}
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		return mesh;
	}

	// Token: 0x060031C9 RID: 12745 RVA: 0x00131718 File Offset: 0x0012F918
	private bool InitializeCubemapFaceMatrices()
	{
		GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
		if (graphicsDeviceType != GraphicsDeviceType.Direct3D11)
		{
			switch (graphicsDeviceType)
			{
			case GraphicsDeviceType.Metal:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
				goto IL_75;
			case GraphicsDeviceType.OpenGLCore:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatrices;
				goto IL_75;
			case GraphicsDeviceType.Direct3D12:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
				goto IL_75;
			case GraphicsDeviceType.Vulkan:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
				goto IL_75;
			}
			this.platformCubemapFaceMatrices = null;
		}
		else
		{
			this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
		}
		IL_75:
		if (this.platformCubemapFaceMatrices == null)
		{
			Debug.LogError("[ReflectionProbeEx] Initialization failed. No cubemap ortho basis defined for " + SystemInfo.graphicsDeviceType);
			return false;
		}
		return true;
	}

	// Token: 0x060031CA RID: 12746 RVA: 0x001317BE File Offset: 0x0012F9BE
	private int FastLog2(int value)
	{
		value |= value >> 1;
		value |= value >> 2;
		value |= value >> 4;
		value |= value >> 8;
		value |= value >> 16;
		return ReflectionProbeEx.tab32[(int)((uint)((long)value * 130329821L) >> 27)];
	}

	// Token: 0x060031CB RID: 12747 RVA: 0x001317F8 File Offset: 0x0012F9F8
	private uint ReverseBits(uint bits)
	{
		bits = (bits << 16 | bits >> 16);
		bits = ((bits & 16711935U) << 8 | (bits & 4278255360U) >> 8);
		bits = ((bits & 252645135U) << 4 | (bits & 4042322160U) >> 4);
		bits = ((bits & 858993459U) << 2 | (bits & 3435973836U) >> 2);
		bits = ((bits & 1431655765U) << 1 | (bits & 2863311530U) >> 1);
		return bits;
	}

	// Token: 0x060031CC RID: 12748 RVA: 0x00131865 File Offset: 0x0012FA65
	private void SafeCreateMaterial(ref Material mat, Shader shader)
	{
		if (mat == null)
		{
			mat = new Material(shader);
		}
	}

	// Token: 0x060031CD RID: 12749 RVA: 0x00131879 File Offset: 0x0012FA79
	private void SafeCreateMaterial(ref Material mat, string shaderName)
	{
		if (mat == null)
		{
			this.SafeCreateMaterial(ref mat, Shader.Find(shaderName));
		}
	}

	// Token: 0x060031CE RID: 12750 RVA: 0x00131894 File Offset: 0x0012FA94
	private void SafeCreateCubeRT(ref RenderTexture rt, string name, int size, int depth, bool mips, TextureDimension dim, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Linear)
	{
		if (rt == null || !rt.IsCreated())
		{
			this.SafeDestroy<RenderTexture>(ref rt);
			rt = new RenderTexture(size, size, depth, format, readWrite)
			{
				hideFlags = HideFlags.DontSave
			};
			rt.name = name;
			rt.dimension = dim;
			if (dim == TextureDimension.Tex2DArray)
			{
				rt.volumeDepth = 6;
			}
			rt.useMipMap = mips;
			rt.autoGenerateMips = false;
			rt.filterMode = filter;
			rt.anisoLevel = 0;
			rt.Create();
		}
	}

	// Token: 0x060031CF RID: 12751 RVA: 0x0013191A File Offset: 0x0012FB1A
	private void SafeCreateCB(ref CommandBuffer cb, string name)
	{
		if (cb == null)
		{
			cb = new CommandBuffer();
			cb.name = name;
		}
	}

	// Token: 0x060031D0 RID: 12752 RVA: 0x0013192F File Offset: 0x0012FB2F
	private void SafeDestroy<T>(ref T obj) where T : UnityEngine.Object
	{
		if (obj != null)
		{
			UnityEngine.Object.DestroyImmediate(obj);
			obj = default(T);
		}
	}

	// Token: 0x060031D1 RID: 12753 RVA: 0x0013195B File Offset: 0x0012FB5B
	private void SafeDispose<T>(ref T obj) where T : IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
			obj = default(T);
		}
	}

	// Token: 0x04002860 RID: 10336
	private Mesh blitMesh;

	// Token: 0x04002861 RID: 10337
	private Mesh skyboxMesh;

	// Token: 0x04002862 RID: 10338
	private static float[] octaVerts = new float[]
	{
		0f,
		1f,
		0f,
		0f,
		0f,
		-1f,
		1f,
		0f,
		0f,
		0f,
		1f,
		0f,
		1f,
		0f,
		0f,
		0f,
		0f,
		1f,
		0f,
		1f,
		0f,
		0f,
		0f,
		1f,
		-1f,
		0f,
		0f,
		0f,
		1f,
		0f,
		-1f,
		0f,
		0f,
		0f,
		0f,
		-1f,
		0f,
		-1f,
		0f,
		1f,
		0f,
		0f,
		0f,
		0f,
		-1f,
		0f,
		-1f,
		0f,
		0f,
		0f,
		1f,
		1f,
		0f,
		0f,
		0f,
		-1f,
		0f,
		-1f,
		0f,
		0f,
		0f,
		0f,
		1f,
		0f,
		-1f,
		0f,
		0f,
		0f,
		-1f,
		-1f,
		0f,
		0f
	};

	// Token: 0x04002863 RID: 10339
	private static readonly ReflectionProbeEx.CubemapFaceMatrices[] cubemapFaceMatrices = new ReflectionProbeEx.CubemapFaceMatrices[]
	{
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f))
	};

	// Token: 0x04002864 RID: 10340
	private static readonly ReflectionProbeEx.CubemapFaceMatrices[] cubemapFaceMatricesD3D11 = new ReflectionProbeEx.CubemapFaceMatrices[]
	{
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f), new Vector3(-1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, -1f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f))
	};

	// Token: 0x04002865 RID: 10341
	private static readonly ReflectionProbeEx.CubemapFaceMatrices[] shadowCubemapFaceMatrices = new ReflectionProbeEx.CubemapFaceMatrices[]
	{
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f))
	};

	// Token: 0x04002866 RID: 10342
	private ReflectionProbeEx.CubemapFaceMatrices[] platformCubemapFaceMatrices;

	// Token: 0x04002867 RID: 10343
	private static readonly int[] tab32 = new int[]
	{
		0,
		9,
		1,
		10,
		13,
		21,
		2,
		29,
		11,
		14,
		16,
		18,
		22,
		25,
		3,
		30,
		8,
		12,
		20,
		28,
		15,
		17,
		24,
		7,
		19,
		27,
		23,
		6,
		26,
		5,
		4,
		31
	};

	// Token: 0x04002868 RID: 10344
	public ReflectionProbeRefreshMode refreshMode = ReflectionProbeRefreshMode.EveryFrame;

	// Token: 0x04002869 RID: 10345
	public bool timeSlicing;

	// Token: 0x0400286A RID: 10346
	public int resolution = 128;

	// Token: 0x0400286B RID: 10347
	[global::InspectorName("HDR")]
	public bool hdr = true;

	// Token: 0x0400286C RID: 10348
	public float shadowDistance;

	// Token: 0x0400286D RID: 10349
	public ReflectionProbeClearFlags clearFlags = ReflectionProbeClearFlags.Skybox;

	// Token: 0x0400286E RID: 10350
	public Color background = new Color(0.192f, 0.301f, 0.474f);

	// Token: 0x0400286F RID: 10351
	public float nearClip = 0.3f;

	// Token: 0x04002870 RID: 10352
	public float farClip = 1000f;

	// Token: 0x04002871 RID: 10353
	public Transform attachToTarget;

	// Token: 0x04002872 RID: 10354
	public Light directionalLight;

	// Token: 0x04002873 RID: 10355
	public float textureMipBias = 2f;

	// Token: 0x04002874 RID: 10356
	public bool highPrecision;

	// Token: 0x04002875 RID: 10357
	public bool enableShadows;

	// Token: 0x04002876 RID: 10358
	public ReflectionProbeEx.ConvolutionQuality convolutionQuality;

	// Token: 0x04002877 RID: 10359
	public List<ReflectionProbeEx.RenderListEntry> staticRenderList = new List<ReflectionProbeEx.RenderListEntry>();

	// Token: 0x04002878 RID: 10360
	public Cubemap reflectionCubemap;

	// Token: 0x04002879 RID: 10361
	public float reflectionIntensity = 1f;

	// Token: 0x02000DE4 RID: 3556
	private struct CubemapSkyboxVertex
	{
		// Token: 0x04004848 RID: 18504
		public float x;

		// Token: 0x04004849 RID: 18505
		public float y;

		// Token: 0x0400484A RID: 18506
		public float z;

		// Token: 0x0400484B RID: 18507
		public Color color;

		// Token: 0x0400484C RID: 18508
		public float tu;

		// Token: 0x0400484D RID: 18509
		public float tv;

		// Token: 0x0400484E RID: 18510
		public float tw;
	}

	// Token: 0x02000DE5 RID: 3557
	private struct CubemapFaceMatrices
	{
		// Token: 0x06004F85 RID: 20357 RVA: 0x0019F8AC File Offset: 0x0019DAAC
		public CubemapFaceMatrices(Vector3 x, Vector3 y, Vector3 z)
		{
			this.worldToView = Matrix4x4.identity;
			this.worldToView[0, 0] = x[0];
			this.worldToView[0, 1] = x[1];
			this.worldToView[0, 2] = x[2];
			this.worldToView[1, 0] = y[0];
			this.worldToView[1, 1] = y[1];
			this.worldToView[1, 2] = y[2];
			this.worldToView[2, 0] = z[0];
			this.worldToView[2, 1] = z[1];
			this.worldToView[2, 2] = z[2];
			this.viewToWorld = this.worldToView.inverse;
		}

		// Token: 0x0400484F RID: 18511
		public Matrix4x4 worldToView;

		// Token: 0x04004850 RID: 18512
		public Matrix4x4 viewToWorld;
	}

	// Token: 0x02000DE6 RID: 3558
	[Serializable]
	public enum ConvolutionQuality
	{
		// Token: 0x04004852 RID: 18514
		Lowest,
		// Token: 0x04004853 RID: 18515
		Low,
		// Token: 0x04004854 RID: 18516
		Medium,
		// Token: 0x04004855 RID: 18517
		High,
		// Token: 0x04004856 RID: 18518
		VeryHigh
	}

	// Token: 0x02000DE7 RID: 3559
	[Serializable]
	public struct RenderListEntry
	{
		// Token: 0x06004F86 RID: 20358 RVA: 0x0019F992 File Offset: 0x0019DB92
		public RenderListEntry(Renderer renderer, bool alwaysEnabled)
		{
			this.renderer = renderer;
			this.alwaysEnabled = alwaysEnabled;
		}

		// Token: 0x04004857 RID: 18519
		public Renderer renderer;

		// Token: 0x04004858 RID: 18520
		public bool alwaysEnabled;
	}
}
