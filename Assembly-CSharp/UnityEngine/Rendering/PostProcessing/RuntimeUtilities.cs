using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5A RID: 2650
	public static class RuntimeUtilities
	{
		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06003EB0 RID: 16048 RVA: 0x0016FBFC File Offset: 0x0016DDFC
		public static Texture2D whiteTexture
		{
			get
			{
				if (RuntimeUtilities.m_WhiteTexture == null)
				{
					RuntimeUtilities.m_WhiteTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
					{
						name = "White Texture"
					};
					RuntimeUtilities.m_WhiteTexture.SetPixel(0, 0, Color.white);
					RuntimeUtilities.m_WhiteTexture.Apply();
				}
				return RuntimeUtilities.m_WhiteTexture;
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06003EB1 RID: 16049 RVA: 0x0016FC50 File Offset: 0x0016DE50
		public static Texture3D whiteTexture3D
		{
			get
			{
				if (RuntimeUtilities.m_WhiteTexture3D == null)
				{
					RuntimeUtilities.m_WhiteTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false)
					{
						name = "White Texture 3D"
					};
					RuntimeUtilities.m_WhiteTexture3D.SetPixels(new Color[]
					{
						Color.white
					});
					RuntimeUtilities.m_WhiteTexture3D.Apply();
				}
				return RuntimeUtilities.m_WhiteTexture3D;
			}
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06003EB2 RID: 16050 RVA: 0x0016FCB0 File Offset: 0x0016DEB0
		public static Texture2D blackTexture
		{
			get
			{
				if (RuntimeUtilities.m_BlackTexture == null)
				{
					RuntimeUtilities.m_BlackTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
					{
						name = "Black Texture"
					};
					RuntimeUtilities.m_BlackTexture.SetPixel(0, 0, Color.black);
					RuntimeUtilities.m_BlackTexture.Apply();
				}
				return RuntimeUtilities.m_BlackTexture;
			}
		}

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x06003EB3 RID: 16051 RVA: 0x0016FD04 File Offset: 0x0016DF04
		public static Texture3D blackTexture3D
		{
			get
			{
				if (RuntimeUtilities.m_BlackTexture3D == null)
				{
					RuntimeUtilities.m_BlackTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false)
					{
						name = "Black Texture 3D"
					};
					RuntimeUtilities.m_BlackTexture3D.SetPixels(new Color[]
					{
						Color.black
					});
					RuntimeUtilities.m_BlackTexture3D.Apply();
				}
				return RuntimeUtilities.m_BlackTexture3D;
			}
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06003EB4 RID: 16052 RVA: 0x0016FD64 File Offset: 0x0016DF64
		public static Texture2D transparentTexture
		{
			get
			{
				if (RuntimeUtilities.m_TransparentTexture == null)
				{
					RuntimeUtilities.m_TransparentTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
					{
						name = "Transparent Texture"
					};
					RuntimeUtilities.m_TransparentTexture.SetPixel(0, 0, Color.clear);
					RuntimeUtilities.m_TransparentTexture.Apply();
				}
				return RuntimeUtilities.m_TransparentTexture;
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06003EB5 RID: 16053 RVA: 0x0016FDB8 File Offset: 0x0016DFB8
		public static Texture3D transparentTexture3D
		{
			get
			{
				if (RuntimeUtilities.m_TransparentTexture3D == null)
				{
					RuntimeUtilities.m_TransparentTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false)
					{
						name = "Transparent Texture 3D"
					};
					RuntimeUtilities.m_TransparentTexture3D.SetPixels(new Color[]
					{
						Color.clear
					});
					RuntimeUtilities.m_TransparentTexture3D.Apply();
				}
				return RuntimeUtilities.m_TransparentTexture3D;
			}
		}

		// Token: 0x06003EB6 RID: 16054 RVA: 0x0016FE18 File Offset: 0x0016E018
		public static Texture2D GetLutStrip(int size)
		{
			Texture2D texture2D;
			if (!RuntimeUtilities.m_LutStrips.TryGetValue(size, out texture2D))
			{
				int num = size * size;
				Color[] array = new Color[num * size];
				float num2 = 1f / ((float)size - 1f);
				for (int i = 0; i < size; i++)
				{
					int num3 = i * size;
					float b = (float)i * num2;
					for (int j = 0; j < size; j++)
					{
						float g = (float)j * num2;
						for (int k = 0; k < size; k++)
						{
							float r = (float)k * num2;
							array[j * num + num3 + k] = new Color(r, g, b);
						}
					}
				}
				TextureFormat textureFormat = TextureFormat.RGBAHalf;
				if (!textureFormat.IsSupported())
				{
					textureFormat = TextureFormat.ARGB32;
				}
				texture2D = new Texture2D(size * size, size, textureFormat, false, true)
				{
					name = "Strip Lut" + size,
					hideFlags = HideFlags.DontSave,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					anisoLevel = 0
				};
				texture2D.SetPixels(array);
				texture2D.Apply();
				RuntimeUtilities.m_LutStrips.Add(size, texture2D);
			}
			return texture2D;
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06003EB7 RID: 16055 RVA: 0x0016FF2C File Offset: 0x0016E12C
		public static Mesh fullscreenTriangle
		{
			get
			{
				if (RuntimeUtilities.s_FullscreenTriangle != null)
				{
					return RuntimeUtilities.s_FullscreenTriangle;
				}
				RuntimeUtilities.s_FullscreenTriangle = new Mesh
				{
					name = "Fullscreen Triangle"
				};
				RuntimeUtilities.s_FullscreenTriangle.SetVertices(new List<Vector3>
				{
					new Vector3(-1f, -1f, 0f),
					new Vector3(-1f, 3f, 0f),
					new Vector3(3f, -1f, 0f)
				});
				RuntimeUtilities.s_FullscreenTriangle.SetIndices(new int[]
				{
					0,
					1,
					2
				}, MeshTopology.Triangles, 0, false);
				RuntimeUtilities.s_FullscreenTriangle.UploadMeshData(false);
				return RuntimeUtilities.s_FullscreenTriangle;
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06003EB8 RID: 16056 RVA: 0x0016FFEC File Offset: 0x0016E1EC
		public static Material copyStdMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyStdMaterial != null)
				{
					return RuntimeUtilities.s_CopyStdMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyStdMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copyStd)
				{
					name = "PostProcess - CopyStd",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyStdMaterial;
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06003EB9 RID: 16057 RVA: 0x00170048 File Offset: 0x0016E248
		public static Material copyStdFromDoubleWideMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyStdFromDoubleWideMaterial != null)
				{
					return RuntimeUtilities.s_CopyStdFromDoubleWideMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyStdFromDoubleWideMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copyStdFromDoubleWide)
				{
					name = "PostProcess - CopyStdFromDoubleWide",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyStdFromDoubleWideMaterial;
			}
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06003EBA RID: 16058 RVA: 0x001700A4 File Offset: 0x0016E2A4
		public static Material copyMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyMaterial != null)
				{
					return RuntimeUtilities.s_CopyMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copy)
				{
					name = "PostProcess - Copy",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyMaterial;
			}
		}

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06003EBB RID: 16059 RVA: 0x00170100 File Offset: 0x0016E300
		public static Material copyFromTexArrayMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyFromTexArrayMaterial != null)
				{
					return RuntimeUtilities.s_CopyFromTexArrayMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyFromTexArrayMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copyStdFromTexArray)
				{
					name = "PostProcess - CopyFromTexArray",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyFromTexArrayMaterial;
			}
		}

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x06003EBC RID: 16060 RVA: 0x0017015B File Offset: 0x0016E35B
		public static PropertySheet copySheet
		{
			get
			{
				if (RuntimeUtilities.s_CopySheet == null)
				{
					RuntimeUtilities.s_CopySheet = new PropertySheet(RuntimeUtilities.copyMaterial);
				}
				return RuntimeUtilities.s_CopySheet;
			}
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06003EBD RID: 16061 RVA: 0x00170178 File Offset: 0x0016E378
		public static PropertySheet copyFromTexArraySheet
		{
			get
			{
				if (RuntimeUtilities.s_CopyFromTexArraySheet == null)
				{
					RuntimeUtilities.s_CopyFromTexArraySheet = new PropertySheet(RuntimeUtilities.copyFromTexArrayMaterial);
				}
				return RuntimeUtilities.s_CopyFromTexArraySheet;
			}
		}

		// Token: 0x06003EBE RID: 16062 RVA: 0x00170195 File Offset: 0x0016E395
		public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd, RenderTargetIdentifier rt, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction)
		{
			cmd.SetRenderTarget(rt, loadAction, storeAction);
		}

		// Token: 0x06003EBF RID: 16063 RVA: 0x001701A0 File Offset: 0x0016E3A0
		public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd, RenderTargetIdentifier color, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depth, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			cmd.SetRenderTarget(color, colorLoadAction, colorStoreAction, depth, depthLoadAction, depthStoreAction);
		}

		// Token: 0x06003EC0 RID: 16064 RVA: 0x001701B4 File Offset: 0x0016E3B4
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, bool clear = false, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTargetWithLoadStoreAction(destination, (viewport == null) ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, RuntimeUtilities.copyMaterial, 0, 0);
		}

		// Token: 0x06003EC1 RID: 16065 RVA: 0x00170220 File Offset: 0x0016E420
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, RenderBufferLoadAction loadAction, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			bool flag = loadAction == RenderBufferLoadAction.Clear;
			if (flag)
			{
				loadAction = RenderBufferLoadAction.DontCare;
			}
			cmd.SetRenderTargetWithLoadStoreAction(destination, (viewport == null) ? loadAction : RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			if (flag)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x06003EC2 RID: 16066 RVA: 0x0017029E File Offset: 0x0016E49E
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
		{
			cmd.BlitFullscreenTriangle(source, destination, propertySheet, pass, clear ? RenderBufferLoadAction.Clear : RenderBufferLoadAction.DontCare, viewport);
		}

		// Token: 0x06003EC3 RID: 16067 RVA: 0x001702B8 File Offset: 0x0016E4B8
		public static void BlitFullscreenTriangleFromDoubleWide(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int pass, int eye)
		{
			Vector4 value = new Vector4(0.5f, 1f, 0f, 0f);
			if (eye == 1)
			{
				value.z = 0.5f;
			}
			cmd.SetGlobalVector(ShaderIDs.UVScaleOffset, value);
			cmd.BuiltinBlit(source, destination, material, pass);
		}

		// Token: 0x06003EC4 RID: 16068 RVA: 0x00170308 File Offset: 0x0016E508
		public static void BlitFullscreenTriangleToDoubleWide(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, int eye)
		{
			Vector4 value = new Vector4(0.5f, 1f, -0.5f, 0f);
			if (eye == 1)
			{
				value.z = 0.5f;
			}
			propertySheet.EnableKeyword("STEREO_DOUBLEWIDE_TARGET");
			propertySheet.properties.SetVector(ShaderIDs.PosScaleOffset, value);
			cmd.BlitFullscreenTriangle(source, destination, propertySheet, 0, false, null);
		}

		// Token: 0x06003EC5 RID: 16069 RVA: 0x00170374 File Offset: 0x0016E574
		public static void BlitFullscreenTriangleFromTexArray(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, int depthSlice = -1)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetGlobalFloat(ShaderIDs.DepthSlice, (float)depthSlice);
			cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x06003EC6 RID: 16070 RVA: 0x001703D4 File Offset: 0x0016E5D4
		public static void BlitFullscreenTriangleToTexArray(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, int depthSlice = -1)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetGlobalFloat(ShaderIDs.DepthSlice, (float)depthSlice);
			cmd.SetRenderTarget(destination, 0, CubemapFace.Unknown, -1);
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x06003EC7 RID: 16071 RVA: 0x00170438 File Offset: 0x0016E638
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			RenderBufferLoadAction renderBufferLoadAction = (viewport == null) ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load;
			if (clear)
			{
				cmd.SetRenderTargetWithLoadStoreAction(destination, renderBufferLoadAction, RenderBufferStoreAction.Store, depth, renderBufferLoadAction, RenderBufferStoreAction.Store);
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			else
			{
				cmd.SetRenderTargetWithLoadStoreAction(destination, renderBufferLoadAction, RenderBufferStoreAction.Store, depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
			}
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x06003EC8 RID: 16072 RVA: 0x001704C4 File Offset: 0x0016E6C4
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier[] destinations, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTarget(destinations, depth);
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x06003EC9 RID: 16073 RVA: 0x0017052D File Offset: 0x0016E72D
		public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
		{
			cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			destination = BuiltinRenderTextureType.CurrentActive;
			cmd.Blit(source, destination);
		}

		// Token: 0x06003ECA RID: 16074 RVA: 0x00170548 File Offset: 0x0016E748
		public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material mat, int pass = 0)
		{
			cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			destination = BuiltinRenderTextureType.CurrentActive;
			cmd.Blit(source, destination, mat, pass);
		}

		// Token: 0x06003ECB RID: 16075 RVA: 0x00170568 File Offset: 0x0016E768
		public static void CopyTexture(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
		{
			if (SystemInfo.copyTextureSupport > CopyTextureSupport.None)
			{
				cmd.CopyTexture(source, destination);
				return;
			}
			cmd.BlitFullscreenTriangle(source, destination, false, null);
		}

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06003ECC RID: 16076 RVA: 0x00170598 File Offset: 0x0016E798
		public static bool scriptableRenderPipelineActive
		{
			get
			{
				return GraphicsSettings.renderPipelineAsset != null;
			}
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x06003ECD RID: 16077 RVA: 0x001705A5 File Offset: 0x0016E7A5
		public static bool supportsDeferredShading
		{
			get
			{
				return RuntimeUtilities.scriptableRenderPipelineActive || GraphicsSettings.GetShaderMode(BuiltinShaderType.DeferredShading) > BuiltinShaderMode.Disabled;
			}
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06003ECE RID: 16078 RVA: 0x001705B9 File Offset: 0x0016E7B9
		public static bool supportsDepthNormals
		{
			get
			{
				return RuntimeUtilities.scriptableRenderPipelineActive || GraphicsSettings.GetShaderMode(BuiltinShaderType.DepthNormals) > BuiltinShaderMode.Disabled;
			}
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06003ECF RID: 16079 RVA: 0x00007074 File Offset: 0x00005274
		public static bool isSinglePassStereoEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x06003ED0 RID: 16080 RVA: 0x00007074 File Offset: 0x00005274
		public static bool isVREnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x06003ED1 RID: 16081 RVA: 0x001705CD File Offset: 0x0016E7CD
		public static bool isAndroidOpenGL
		{
			get
			{
				return Application.platform == RuntimePlatform.Android && SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan;
			}
		}

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x06003ED2 RID: 16082 RVA: 0x0003D345 File Offset: 0x0003B545
		public static RenderTextureFormat defaultHDRRenderTextureFormat
		{
			get
			{
				return RenderTextureFormat.DefaultHDR;
			}
		}

		// Token: 0x06003ED3 RID: 16083 RVA: 0x001705E6 File Offset: 0x0016E7E6
		public static bool isFloatingPointFormat(RenderTextureFormat format)
		{
			return format == RenderTextureFormat.DefaultHDR || format == RenderTextureFormat.ARGBHalf || format == RenderTextureFormat.ARGBFloat || format == RenderTextureFormat.RGFloat || format == RenderTextureFormat.RGHalf || format == RenderTextureFormat.RFloat || format == RenderTextureFormat.RHalf || format == RenderTextureFormat.RGB111110Float;
		}

		// Token: 0x06003ED4 RID: 16084 RVA: 0x00170611 File Offset: 0x0016E811
		public static void Destroy(Object obj)
		{
			if (obj != null)
			{
				Object.Destroy(obj);
			}
		}

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06003ED5 RID: 16085 RVA: 0x00170622 File Offset: 0x0016E822
		public static bool isLinearColorSpace
		{
			get
			{
				return QualitySettings.activeColorSpace == ColorSpace.Linear;
			}
		}

		// Token: 0x06003ED6 RID: 16086 RVA: 0x0017062C File Offset: 0x0016E82C
		public static bool IsResolvedDepthAvailable(Camera camera)
		{
			GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
			return camera.actualRenderingPath == RenderingPath.DeferredShading && (graphicsDeviceType == GraphicsDeviceType.Direct3D11 || graphicsDeviceType == GraphicsDeviceType.Direct3D12 || graphicsDeviceType == GraphicsDeviceType.XboxOne);
		}

		// Token: 0x06003ED7 RID: 16087 RVA: 0x0017065C File Offset: 0x0016E85C
		public static void DestroyProfile(PostProcessProfile profile, bool destroyEffects)
		{
			if (destroyEffects)
			{
				foreach (PostProcessEffectSettings obj in profile.settings)
				{
					RuntimeUtilities.Destroy(obj);
				}
			}
			RuntimeUtilities.Destroy(profile);
		}

		// Token: 0x06003ED8 RID: 16088 RVA: 0x001706B8 File Offset: 0x0016E8B8
		public static void DestroyVolume(PostProcessVolume volume, bool destroyProfile, bool destroyGameObject = false)
		{
			if (destroyProfile)
			{
				RuntimeUtilities.DestroyProfile(volume.profileRef, true);
			}
			GameObject gameObject = volume.gameObject;
			RuntimeUtilities.Destroy(volume);
			if (destroyGameObject)
			{
				RuntimeUtilities.Destroy(gameObject);
			}
		}

		// Token: 0x06003ED9 RID: 16089 RVA: 0x001706EA File Offset: 0x0016E8EA
		public static bool IsPostProcessingActive(PostProcessLayer layer)
		{
			return layer != null && layer.enabled;
		}

		// Token: 0x06003EDA RID: 16090 RVA: 0x001706FD File Offset: 0x0016E8FD
		public static bool IsTemporalAntialiasingActive(PostProcessLayer layer)
		{
			return RuntimeUtilities.IsPostProcessingActive(layer) && layer.antialiasingMode == PostProcessLayer.Antialiasing.TemporalAntialiasing && layer.temporalAntialiasing.IsSupported();
		}

		// Token: 0x06003EDB RID: 16091 RVA: 0x0017071D File Offset: 0x0016E91D
		public static IEnumerable<T> GetAllSceneObjects<T>() where T : Component
		{
			Queue<Transform> queue = new Queue<Transform>();
			GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				queue.Enqueue(gameObject.transform);
				T component = gameObject.GetComponent<T>();
				if (component != null)
				{
					yield return component;
				}
			}
			GameObject[] array = null;
			while (queue.Count > 0)
			{
				foreach (object obj in queue.Dequeue())
				{
					Transform transform = (Transform)obj;
					queue.Enqueue(transform);
					T component2 = transform.GetComponent<T>();
					if (component2 != null)
					{
						yield return component2;
					}
				}
				IEnumerator enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06003EDC RID: 16092 RVA: 0x00170726 File Offset: 0x0016E926
		public static void CreateIfNull<T>(ref T obj) where T : class, new()
		{
			if (obj == null)
			{
				obj = Activator.CreateInstance<T>();
			}
		}

		// Token: 0x06003EDD RID: 16093 RVA: 0x00170740 File Offset: 0x0016E940
		public static float Exp2(float x)
		{
			return Mathf.Exp(x * 0.6931472f);
		}

		// Token: 0x06003EDE RID: 16094 RVA: 0x00170750 File Offset: 0x0016E950
		public static Matrix4x4 GetJitteredPerspectiveProjectionMatrix(Camera camera, Vector2 offset)
		{
			float nearClipPlane = camera.nearClipPlane;
			float farClipPlane = camera.farClipPlane;
			float num = Mathf.Tan(0.008726646f * camera.fieldOfView) * nearClipPlane;
			float num2 = num * camera.aspect;
			offset.x *= num2 / (0.5f * (float)camera.pixelWidth);
			offset.y *= num / (0.5f * (float)camera.pixelHeight);
			Matrix4x4 projectionMatrix = camera.projectionMatrix;
			ref Matrix4x4 ptr = ref projectionMatrix;
			ptr[0, 2] = ptr[0, 2] + offset.x / num2;
			ptr = ref projectionMatrix;
			ptr[1, 2] = ptr[1, 2] + offset.y / num;
			return projectionMatrix;
		}

		// Token: 0x06003EDF RID: 16095 RVA: 0x00170804 File Offset: 0x0016EA04
		public static Matrix4x4 GetJitteredOrthographicProjectionMatrix(Camera camera, Vector2 offset)
		{
			float orthographicSize = camera.orthographicSize;
			float num = orthographicSize * camera.aspect;
			offset.x *= num / (0.5f * (float)camera.pixelWidth);
			offset.y *= orthographicSize / (0.5f * (float)camera.pixelHeight);
			float left = offset.x - num;
			float right = offset.x + num;
			float top = offset.y + orthographicSize;
			float bottom = offset.y - orthographicSize;
			return Matrix4x4.Ortho(left, right, bottom, top, camera.nearClipPlane, camera.farClipPlane);
		}

		// Token: 0x06003EE0 RID: 16096 RVA: 0x00170890 File Offset: 0x0016EA90
		public static Matrix4x4 GenerateJitteredProjectionMatrixFromOriginal(PostProcessRenderContext context, Matrix4x4 origProj, Vector2 jitter)
		{
			FrustumPlanes decomposeProjection = origProj.decomposeProjection;
			float num = Math.Abs(decomposeProjection.top) + Math.Abs(decomposeProjection.bottom);
			float num2 = Math.Abs(decomposeProjection.left) + Math.Abs(decomposeProjection.right);
			Vector2 vector = new Vector2(jitter.x * num2 / (float)context.screenWidth, jitter.y * num / (float)context.screenHeight);
			decomposeProjection.left += vector.x;
			decomposeProjection.right += vector.x;
			decomposeProjection.top += vector.y;
			decomposeProjection.bottom += vector.y;
			return Matrix4x4.Frustum(decomposeProjection);
		}

		// Token: 0x06003EE1 RID: 16097 RVA: 0x00170948 File Offset: 0x0016EB48
		public static IEnumerable<Type> GetAllAssemblyTypes()
		{
			if (RuntimeUtilities.m_AssemblyTypes == null)
			{
				RuntimeUtilities.m_AssemblyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(delegate(Assembly t)
				{
					Type[] result = new Type[0];
					try
					{
						result = t.GetTypes();
					}
					catch
					{
					}
					return result;
				});
			}
			return RuntimeUtilities.m_AssemblyTypes;
		}

		// Token: 0x06003EE2 RID: 16098 RVA: 0x00170994 File Offset: 0x0016EB94
		public static T GetAttribute<T>(this Type type) where T : Attribute
		{
			Assert.IsTrue(type.IsDefined(typeof(T), false), "Attribute not found");
			return (T)((object)type.GetCustomAttributes(typeof(T), false)[0]);
		}

		// Token: 0x06003EE3 RID: 16099 RVA: 0x001709CC File Offset: 0x0016EBCC
		public static Attribute[] GetMemberAttributes<TType, TValue>(Expression<Func<TType, TValue>> expr)
		{
			Expression expression = expr;
			if (expression is LambdaExpression)
			{
				expression = ((LambdaExpression)expression).Body;
			}
			ExpressionType nodeType = expression.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				return ((FieldInfo)((MemberExpression)expression).Member).GetCustomAttributes(false).Cast<Attribute>().ToArray<Attribute>();
			}
			throw new InvalidOperationException();
		}

		// Token: 0x06003EE4 RID: 16100 RVA: 0x00170A24 File Offset: 0x0016EC24
		public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
		{
			ExpressionType nodeType = expr.Body.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				MemberExpression memberExpression = expr.Body as MemberExpression;
				List<string> list = new List<string>();
				while (memberExpression != null)
				{
					list.Add(memberExpression.Member.Name);
					memberExpression = (memberExpression.Expression as MemberExpression);
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = list.Count - 1; i >= 0; i--)
				{
					stringBuilder.Append(list[i]);
					if (i > 0)
					{
						stringBuilder.Append('.');
					}
				}
				return stringBuilder.ToString();
			}
			throw new InvalidOperationException();
		}

		// Token: 0x0400379E RID: 14238
		private static Texture2D m_WhiteTexture;

		// Token: 0x0400379F RID: 14239
		private static Texture3D m_WhiteTexture3D;

		// Token: 0x040037A0 RID: 14240
		private static Texture2D m_BlackTexture;

		// Token: 0x040037A1 RID: 14241
		private static Texture3D m_BlackTexture3D;

		// Token: 0x040037A2 RID: 14242
		private static Texture2D m_TransparentTexture;

		// Token: 0x040037A3 RID: 14243
		private static Texture3D m_TransparentTexture3D;

		// Token: 0x040037A4 RID: 14244
		private static Dictionary<int, Texture2D> m_LutStrips = new Dictionary<int, Texture2D>();

		// Token: 0x040037A5 RID: 14245
		internal static PostProcessResources s_Resources;

		// Token: 0x040037A6 RID: 14246
		private static Mesh s_FullscreenTriangle;

		// Token: 0x040037A7 RID: 14247
		private static Material s_CopyStdMaterial;

		// Token: 0x040037A8 RID: 14248
		private static Material s_CopyStdFromDoubleWideMaterial;

		// Token: 0x040037A9 RID: 14249
		private static Material s_CopyMaterial;

		// Token: 0x040037AA RID: 14250
		private static Material s_CopyFromTexArrayMaterial;

		// Token: 0x040037AB RID: 14251
		private static PropertySheet s_CopySheet;

		// Token: 0x040037AC RID: 14252
		private static PropertySheet s_CopyFromTexArraySheet;

		// Token: 0x040037AD RID: 14253
		private static IEnumerable<Type> m_AssemblyTypes;
	}
}
