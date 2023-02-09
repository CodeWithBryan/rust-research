using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x020009FB RID: 2555
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[ImageEffectAllowedInSceneView]
	[AddComponentMenu("Rendering/Post-process Layer", 1000)]
	[RequireComponent(typeof(Camera))]
	public class PostProcessLayer : MonoBehaviour
	{
		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06003CDC RID: 15580 RVA: 0x001639B9 File Offset: 0x00161BB9
		// (set) Token: 0x06003CDD RID: 15581 RVA: 0x001639C1 File Offset: 0x00161BC1
		public Dictionary<PostProcessEvent, List<PostProcessLayer.SerializedBundleRef>> sortedBundles { get; private set; }

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06003CDE RID: 15582 RVA: 0x001639CA File Offset: 0x00161BCA
		// (set) Token: 0x06003CDF RID: 15583 RVA: 0x001639D2 File Offset: 0x00161BD2
		public bool haveBundlesBeenInited { get; private set; }

		// Token: 0x06003CE0 RID: 15584 RVA: 0x001639DC File Offset: 0x00161BDC
		private void OnEnable()
		{
			this.Init(null);
			if (!this.haveBundlesBeenInited)
			{
				this.InitBundles();
			}
			this.m_LogHistogram = new LogHistogram();
			this.m_PropertySheetFactory = new PropertySheetFactory();
			this.m_TargetPool = new TargetPool();
			this.debugLayer.OnEnable();
			if (RuntimeUtilities.scriptableRenderPipelineActive)
			{
				return;
			}
			this.InitLegacy();
		}

		// Token: 0x06003CE1 RID: 15585 RVA: 0x00163A38 File Offset: 0x00161C38
		private void InitLegacy()
		{
			this.m_LegacyCmdBufferBeforeReflections = new CommandBuffer
			{
				name = "Deferred Ambient Occlusion"
			};
			this.m_LegacyCmdBufferBeforeLighting = new CommandBuffer
			{
				name = "Deferred Ambient Occlusion"
			};
			this.m_LegacyCmdBufferOpaque = new CommandBuffer
			{
				name = "Opaque Only Post-processing"
			};
			this.m_LegacyCmdBuffer = new CommandBuffer
			{
				name = "Post-processing"
			};
			this.m_Camera = base.GetComponent<Camera>();
			this.m_Camera.AddCommandBuffer(CameraEvent.BeforeReflections, this.m_LegacyCmdBufferBeforeReflections);
			this.m_Camera.AddCommandBuffer(CameraEvent.BeforeLighting, this.m_LegacyCmdBufferBeforeLighting);
			this.m_Camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.m_LegacyCmdBufferOpaque);
			this.m_Camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, this.m_LegacyCmdBuffer);
			this.m_CurrentContext = new PostProcessRenderContext();
		}

		// Token: 0x06003CE2 RID: 15586 RVA: 0x00163AFF File Offset: 0x00161CFF
		[ImageEffectUsesCommandBuffer]
		private void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			if (this.finalBlitToCameraTarget)
			{
				RenderTexture.active = dst;
				return;
			}
			Graphics.Blit(src, dst);
		}

		// Token: 0x06003CE3 RID: 15587 RVA: 0x00163B18 File Offset: 0x00161D18
		public void Init(PostProcessResources resources)
		{
			if (resources != null)
			{
				this.m_Resources = resources;
			}
			RuntimeUtilities.CreateIfNull<TemporalAntialiasing>(ref this.temporalAntialiasing);
			RuntimeUtilities.CreateIfNull<SubpixelMorphologicalAntialiasing>(ref this.subpixelMorphologicalAntialiasing);
			RuntimeUtilities.CreateIfNull<FastApproximateAntialiasing>(ref this.fastApproximateAntialiasing);
			RuntimeUtilities.CreateIfNull<Dithering>(ref this.dithering);
			RuntimeUtilities.CreateIfNull<Fog>(ref this.fog);
			RuntimeUtilities.CreateIfNull<PostProcessDebugLayer>(ref this.debugLayer);
		}

		// Token: 0x06003CE4 RID: 15588 RVA: 0x00163B78 File Offset: 0x00161D78
		public void InitBundles()
		{
			if (this.haveBundlesBeenInited)
			{
				return;
			}
			RuntimeUtilities.CreateIfNull<List<PostProcessLayer.SerializedBundleRef>>(ref this.m_BeforeTransparentBundles);
			RuntimeUtilities.CreateIfNull<List<PostProcessLayer.SerializedBundleRef>>(ref this.m_BeforeStackBundles);
			RuntimeUtilities.CreateIfNull<List<PostProcessLayer.SerializedBundleRef>>(ref this.m_AfterStackBundles);
			this.m_Bundles = new Dictionary<Type, PostProcessBundle>();
			foreach (Type type in PostProcessManager.instance.settingsTypes.Keys)
			{
				PostProcessBundle value = new PostProcessBundle((PostProcessEffectSettings)ScriptableObject.CreateInstance(type));
				this.m_Bundles.Add(type, value);
			}
			this.UpdateBundleSortList(this.m_BeforeTransparentBundles, PostProcessEvent.BeforeTransparent);
			this.UpdateBundleSortList(this.m_BeforeStackBundles, PostProcessEvent.BeforeStack);
			this.UpdateBundleSortList(this.m_AfterStackBundles, PostProcessEvent.AfterStack);
			this.sortedBundles = new Dictionary<PostProcessEvent, List<PostProcessLayer.SerializedBundleRef>>(default(PostProcessEventComparer))
			{
				{
					PostProcessEvent.BeforeTransparent,
					this.m_BeforeTransparentBundles
				},
				{
					PostProcessEvent.BeforeStack,
					this.m_BeforeStackBundles
				},
				{
					PostProcessEvent.AfterStack,
					this.m_AfterStackBundles
				}
			};
			this.haveBundlesBeenInited = true;
		}

		// Token: 0x06003CE5 RID: 15589 RVA: 0x00163C90 File Offset: 0x00161E90
		private void UpdateBundleSortList(List<PostProcessLayer.SerializedBundleRef> sortedList, PostProcessEvent evt)
		{
			List<PostProcessBundle> effects = (from kvp in this.m_Bundles
			where kvp.Value.attribute.eventType == evt && !kvp.Value.attribute.builtinEffect
			select kvp.Value).ToList<PostProcessBundle>();
			sortedList.RemoveAll(delegate(PostProcessLayer.SerializedBundleRef x)
			{
				string searchStr = x.assemblyQualifiedName;
				return !effects.Exists((PostProcessBundle b) => b.settings.GetType().AssemblyQualifiedName == searchStr);
			});
			foreach (PostProcessBundle postProcessBundle in effects)
			{
				string typeName = postProcessBundle.settings.GetType().AssemblyQualifiedName;
				if (!sortedList.Exists((PostProcessLayer.SerializedBundleRef b) => b.assemblyQualifiedName == typeName))
				{
					PostProcessLayer.SerializedBundleRef item = new PostProcessLayer.SerializedBundleRef
					{
						assemblyQualifiedName = typeName
					};
					sortedList.Add(item);
				}
			}
			foreach (PostProcessLayer.SerializedBundleRef serializedBundleRef in sortedList)
			{
				string typeName = serializedBundleRef.assemblyQualifiedName;
				PostProcessBundle bundle = effects.Find((PostProcessBundle b) => b.settings.GetType().AssemblyQualifiedName == typeName);
				serializedBundleRef.bundle = bundle;
			}
		}

		// Token: 0x06003CE6 RID: 15590 RVA: 0x00163E00 File Offset: 0x00162000
		private void OnDisable()
		{
			if (this.m_Camera != null)
			{
				if (this.m_LegacyCmdBufferBeforeReflections != null)
				{
					this.m_Camera.RemoveCommandBuffer(CameraEvent.BeforeReflections, this.m_LegacyCmdBufferBeforeReflections);
				}
				if (this.m_LegacyCmdBufferBeforeLighting != null)
				{
					this.m_Camera.RemoveCommandBuffer(CameraEvent.BeforeLighting, this.m_LegacyCmdBufferBeforeLighting);
				}
				if (this.m_LegacyCmdBufferOpaque != null)
				{
					this.m_Camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.m_LegacyCmdBufferOpaque);
				}
				if (this.m_LegacyCmdBuffer != null)
				{
					this.m_Camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, this.m_LegacyCmdBuffer);
				}
			}
			this.temporalAntialiasing.Release();
			this.m_LogHistogram.Release();
			foreach (PostProcessBundle postProcessBundle in this.m_Bundles.Values)
			{
				postProcessBundle.Release();
			}
			this.m_Bundles.Clear();
			this.m_PropertySheetFactory.Release();
			if (this.debugLayer != null)
			{
				this.debugLayer.OnDisable();
			}
			TextureLerper.instance.Clear();
			this.m_Camera.ResetProjectionMatrix();
			this.m_Camera.nonJitteredProjectionMatrix = this.m_Camera.projectionMatrix;
			Shader.SetGlobalVector("_FrustumJitter", Vector2.zero);
			this.haveBundlesBeenInited = false;
		}

		// Token: 0x06003CE7 RID: 15591 RVA: 0x00163F54 File Offset: 0x00162154
		private void Reset()
		{
			this.volumeTrigger = base.transform;
		}

		// Token: 0x06003CE8 RID: 15592 RVA: 0x00163F64 File Offset: 0x00162164
		private void OnPreCull()
		{
			if (RuntimeUtilities.scriptableRenderPipelineActive)
			{
				return;
			}
			if (this.m_Camera == null || this.m_CurrentContext == null)
			{
				this.InitLegacy();
			}
			if (!this.m_Camera.usePhysicalProperties)
			{
				this.m_Camera.ResetProjectionMatrix();
			}
			this.m_Camera.nonJitteredProjectionMatrix = this.m_Camera.projectionMatrix;
			if (this.m_Camera.stereoEnabled)
			{
				this.m_Camera.ResetStereoProjectionMatrices();
			}
			else
			{
				Shader.SetGlobalFloat(ShaderIDs.RenderViewportScaleFactor, 1f);
			}
			this.BuildCommandBuffers();
			Shader.SetGlobalVector("_FrustumJitter", this.temporalAntialiasing.jitter);
		}

		// Token: 0x06003CE9 RID: 15593 RVA: 0x0016400C File Offset: 0x0016220C
		private void OnPreRender()
		{
			if (RuntimeUtilities.scriptableRenderPipelineActive || this.m_Camera.stereoActiveEye != Camera.MonoOrStereoscopicEye.Right)
			{
				return;
			}
			this.BuildCommandBuffers();
		}

		// Token: 0x06003CEA RID: 15594 RVA: 0x0016402A File Offset: 0x0016222A
		private RenderTextureFormat GetIntermediateFormat()
		{
			if (this.intermediateFormat != this.prevIntermediateFormat)
			{
				this.supportsIntermediateFormat = SystemInfo.SupportsRenderTextureFormat(this.intermediateFormat);
				this.prevIntermediateFormat = this.intermediateFormat;
			}
			if (!this.supportsIntermediateFormat)
			{
				return RenderTextureFormat.DefaultHDR;
			}
			return this.intermediateFormat;
		}

		// Token: 0x06003CEB RID: 15595 RVA: 0x00164068 File Offset: 0x00162268
		private static bool RequiresInitialBlit(Camera camera, PostProcessRenderContext context)
		{
			return camera.allowMSAA || RuntimeUtilities.scriptableRenderPipelineActive;
		}

		// Token: 0x06003CEC RID: 15596 RVA: 0x00164080 File Offset: 0x00162280
		private void UpdateSrcDstForOpaqueOnly(ref int src, ref int dst, PostProcessRenderContext context, RenderTargetIdentifier cameraTarget, int opaqueOnlyEffectsRemaining)
		{
			if (src > -1)
			{
				context.command.ReleaseTemporaryRT(src);
			}
			context.source = context.destination;
			src = dst;
			if (opaqueOnlyEffectsRemaining == 1)
			{
				context.destination = cameraTarget;
				return;
			}
			dst = this.m_TargetPool.Get();
			context.destination = dst;
			context.GetScreenSpaceTemporaryRT(context.command, dst, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, 0, 0);
		}

		// Token: 0x06003CED RID: 15597 RVA: 0x001640F0 File Offset: 0x001622F0
		private void BuildCommandBuffers()
		{
			PostProcessRenderContext currentContext = this.m_CurrentContext;
			RenderTextureFormat renderTextureFormat = this.GetIntermediateFormat();
			RenderTextureFormat renderTextureFormat2 = this.m_Camera.allowHDR ? renderTextureFormat : RenderTextureFormat.Default;
			if (!RuntimeUtilities.isFloatingPointFormat(renderTextureFormat2))
			{
				this.m_NaNKilled = true;
			}
			currentContext.Reset();
			currentContext.camera = this.m_Camera;
			currentContext.sourceFormat = renderTextureFormat2;
			this.m_LegacyCmdBufferBeforeReflections.Clear();
			this.m_LegacyCmdBufferBeforeLighting.Clear();
			this.m_LegacyCmdBufferOpaque.Clear();
			this.m_LegacyCmdBuffer.Clear();
			this.SetupContext(currentContext);
			currentContext.command = this.m_LegacyCmdBufferOpaque;
			TextureLerper.instance.BeginFrame(currentContext);
			this.UpdateVolumeSystem(currentContext.camera, currentContext.command);
			PostProcessBundle bundle = this.GetBundle<AmbientOcclusion>();
			AmbientOcclusion ambientOcclusion = bundle.CastSettings<AmbientOcclusion>();
			AmbientOcclusionRenderer ambientOcclusionRenderer = bundle.CastRenderer<AmbientOcclusionRenderer>();
			bool flag = ambientOcclusion.IsEnabledAndSupported(currentContext);
			bool flag2 = ambientOcclusionRenderer.IsAmbientOnly(currentContext);
			bool flag3 = flag && flag2;
			bool flag4 = flag && !flag2;
			PostProcessBundle bundle2 = this.GetBundle<ScreenSpaceReflections>();
			PostProcessEffectSettings settings = bundle2.settings;
			PostProcessEffectRenderer renderer = bundle2.renderer;
			bool flag5 = settings.IsEnabledAndSupported(currentContext);
			if (flag3)
			{
				IAmbientOcclusionMethod ambientOcclusionMethod = ambientOcclusionRenderer.Get();
				currentContext.command = this.m_LegacyCmdBufferBeforeReflections;
				ambientOcclusionMethod.RenderAmbientOnly(currentContext);
				currentContext.command = this.m_LegacyCmdBufferBeforeLighting;
				ambientOcclusionMethod.CompositeAmbientOnly(currentContext);
			}
			else if (flag4)
			{
				currentContext.command = this.m_LegacyCmdBufferOpaque;
				ambientOcclusionRenderer.Get().RenderAfterOpaque(currentContext);
			}
			bool flag6 = this.fog.IsEnabledAndSupported(currentContext);
			bool flag7 = this.HasOpaqueOnlyEffects(currentContext);
			int num = 0;
			num += (flag5 ? 1 : 0);
			num += (flag6 ? 1 : 0);
			num += (flag7 ? 1 : 0);
			RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
			if (num > 0)
			{
				CommandBuffer legacyCmdBufferOpaque = this.m_LegacyCmdBufferOpaque;
				currentContext.command = legacyCmdBufferOpaque;
				currentContext.source = renderTargetIdentifier;
				currentContext.destination = renderTargetIdentifier;
				int nameID = -1;
				int num2 = -1;
				this.UpdateSrcDstForOpaqueOnly(ref nameID, ref num2, currentContext, renderTargetIdentifier, num + 1);
				if (PostProcessLayer.RequiresInitialBlit(this.m_Camera, currentContext) || num == 1)
				{
					legacyCmdBufferOpaque.BuiltinBlit(currentContext.source, currentContext.destination, RuntimeUtilities.copyStdMaterial, this.stopNaNPropagation ? 1 : 0);
					this.UpdateSrcDstForOpaqueOnly(ref nameID, ref num2, currentContext, renderTargetIdentifier, num);
				}
				if (flag5)
				{
					renderer.Render(currentContext);
					num--;
					this.UpdateSrcDstForOpaqueOnly(ref nameID, ref num2, currentContext, renderTargetIdentifier, num);
				}
				if (flag6)
				{
					this.fog.Render(currentContext);
					num--;
					this.UpdateSrcDstForOpaqueOnly(ref nameID, ref num2, currentContext, renderTargetIdentifier, num);
				}
				if (flag7)
				{
					this.RenderOpaqueOnly(currentContext);
				}
				legacyCmdBufferOpaque.ReleaseTemporaryRT(nameID);
			}
			this.BuildPostEffectsOld(renderTextureFormat2, currentContext, renderTargetIdentifier);
		}

		// Token: 0x06003CEE RID: 15598 RVA: 0x00164380 File Offset: 0x00162580
		private void BuildPostEffectsOld(RenderTextureFormat sourceFormat, PostProcessRenderContext context, RenderTargetIdentifier cameraTarget)
		{
			int num = -1;
			bool flag = !this.m_NaNKilled && this.stopNaNPropagation && RuntimeUtilities.isFloatingPointFormat(sourceFormat);
			if (PostProcessLayer.RequiresInitialBlit(this.m_Camera, context) || flag)
			{
				num = this.m_TargetPool.Get();
				context.GetScreenSpaceTemporaryRT(this.m_LegacyCmdBuffer, num, 0, sourceFormat, RenderTextureReadWrite.sRGB, FilterMode.Bilinear, 0, 0);
				this.m_LegacyCmdBuffer.BuiltinBlit(cameraTarget, num, RuntimeUtilities.copyStdMaterial, this.stopNaNPropagation ? 1 : 0);
				if (!this.m_NaNKilled)
				{
					this.m_NaNKilled = this.stopNaNPropagation;
				}
				context.source = num;
			}
			else
			{
				context.source = cameraTarget;
			}
			context.destination = cameraTarget;
			if (this.finalBlitToCameraTarget && !RuntimeUtilities.scriptableRenderPipelineActive)
			{
				if (this.m_Camera.targetTexture)
				{
					context.destination = this.m_Camera.targetTexture.colorBuffer;
				}
				else
				{
					context.flip = true;
					context.destination = Display.main.colorBuffer;
				}
			}
			context.command = this.m_LegacyCmdBuffer;
			this.Render(context);
			if (num > -1)
			{
				this.m_LegacyCmdBuffer.ReleaseTemporaryRT(num);
			}
		}

		// Token: 0x06003CEF RID: 15599 RVA: 0x001644A8 File Offset: 0x001626A8
		private void OnPostRender()
		{
			if (RuntimeUtilities.scriptableRenderPipelineActive)
			{
				return;
			}
			if (this.m_CurrentContext.IsTemporalAntialiasingActive())
			{
				if (this.m_CurrentContext.physicalCamera)
				{
					this.m_Camera.usePhysicalProperties = true;
				}
				else
				{
					this.m_Camera.ResetProjectionMatrix();
				}
				if (this.m_CurrentContext.stereoActive && (RuntimeUtilities.isSinglePassStereoEnabled || this.m_Camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right))
				{
					this.m_Camera.ResetStereoProjectionMatrices();
				}
			}
		}

		// Token: 0x06003CF0 RID: 15600 RVA: 0x0016451D File Offset: 0x0016271D
		public PostProcessBundle GetBundle<T>() where T : PostProcessEffectSettings
		{
			return this.GetBundle(typeof(T));
		}

		// Token: 0x06003CF1 RID: 15601 RVA: 0x0016452F File Offset: 0x0016272F
		public PostProcessBundle GetBundle(Type settingsType)
		{
			Assert.IsTrue(this.m_Bundles.ContainsKey(settingsType), "Invalid type");
			return this.m_Bundles[settingsType];
		}

		// Token: 0x06003CF2 RID: 15602 RVA: 0x00164553 File Offset: 0x00162753
		public T GetSettings<T>() where T : PostProcessEffectSettings
		{
			return this.GetBundle<T>().CastSettings<T>();
		}

		// Token: 0x06003CF3 RID: 15603 RVA: 0x00164560 File Offset: 0x00162760
		public void BakeMSVOMap(CommandBuffer cmd, Camera camera, RenderTargetIdentifier destination, RenderTargetIdentifier? depthMap, bool invert, bool isMSAA = false)
		{
			MultiScaleVO multiScaleVO = this.GetBundle<AmbientOcclusion>().CastRenderer<AmbientOcclusionRenderer>().GetMultiScaleVO();
			multiScaleVO.SetResources(this.m_Resources);
			multiScaleVO.GenerateAOMap(cmd, camera, destination, depthMap, invert, isMSAA);
		}

		// Token: 0x06003CF4 RID: 15604 RVA: 0x0016458C File Offset: 0x0016278C
		internal void OverrideSettings(List<PostProcessEffectSettings> baseSettings, float interpFactor)
		{
			foreach (PostProcessEffectSettings postProcessEffectSettings in baseSettings)
			{
				if (postProcessEffectSettings.active)
				{
					PostProcessEffectSettings settings = this.GetBundle(postProcessEffectSettings.GetType()).settings;
					int count = postProcessEffectSettings.parameters.Count;
					for (int i = 0; i < count; i++)
					{
						ParameterOverride parameterOverride = postProcessEffectSettings.parameters[i];
						if (parameterOverride.overrideState)
						{
							ParameterOverride parameterOverride2 = settings.parameters[i];
							parameterOverride2.Interp(parameterOverride2, parameterOverride, interpFactor);
						}
					}
				}
			}
		}

		// Token: 0x06003CF5 RID: 15605 RVA: 0x00164638 File Offset: 0x00162838
		private void SetLegacyCameraFlags(PostProcessRenderContext context)
		{
			DepthTextureMode depthTextureMode = context.camera.depthTextureMode;
			foreach (KeyValuePair<Type, PostProcessBundle> keyValuePair in this.m_Bundles)
			{
				if (keyValuePair.Value.settings.IsEnabledAndSupported(context))
				{
					depthTextureMode |= keyValuePair.Value.renderer.GetCameraFlags();
				}
			}
			if (context.IsTemporalAntialiasingActive())
			{
				depthTextureMode |= this.temporalAntialiasing.GetCameraFlags();
			}
			if (this.fog.IsEnabledAndSupported(context))
			{
				depthTextureMode |= this.fog.GetCameraFlags();
			}
			if (this.debugLayer.debugOverlay != DebugOverlay.None)
			{
				depthTextureMode |= this.debugLayer.GetCameraFlags();
			}
			context.camera.depthTextureMode = depthTextureMode;
		}

		// Token: 0x06003CF6 RID: 15606 RVA: 0x00164714 File Offset: 0x00162914
		public void ResetHistory()
		{
			foreach (KeyValuePair<Type, PostProcessBundle> keyValuePair in this.m_Bundles)
			{
				keyValuePair.Value.ResetHistory();
			}
			this.temporalAntialiasing.ResetHistory();
		}

		// Token: 0x06003CF7 RID: 15607 RVA: 0x00164778 File Offset: 0x00162978
		public bool HasOpaqueOnlyEffects(PostProcessRenderContext context)
		{
			return this.HasActiveEffects(PostProcessEvent.BeforeTransparent, context);
		}

		// Token: 0x06003CF8 RID: 15608 RVA: 0x00164784 File Offset: 0x00162984
		public bool HasActiveEffects(PostProcessEvent evt, PostProcessRenderContext context)
		{
			foreach (PostProcessLayer.SerializedBundleRef serializedBundleRef in this.sortedBundles[evt])
			{
				bool flag = serializedBundleRef.bundle.settings.IsEnabledAndSupported(context);
				if (context.isSceneView)
				{
					if (serializedBundleRef.bundle.attribute.allowInSceneView && flag)
					{
						return true;
					}
				}
				else if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003CF9 RID: 15609 RVA: 0x00164814 File Offset: 0x00162A14
		private void SetupContext(PostProcessRenderContext context)
		{
			RuntimeUtilities.s_Resources = this.m_Resources;
			this.m_IsRenderingInSceneView = (context.camera.cameraType == CameraType.SceneView);
			context.isSceneView = this.m_IsRenderingInSceneView;
			context.resources = this.m_Resources;
			context.propertySheets = this.m_PropertySheetFactory;
			context.debugLayer = this.debugLayer;
			context.antialiasing = this.antialiasingMode;
			context.temporalAntialiasing = this.temporalAntialiasing;
			context.logHistogram = this.m_LogHistogram;
			context.physicalCamera = context.camera.usePhysicalProperties;
			this.SetLegacyCameraFlags(context);
			this.debugLayer.SetFrameSize(context.width, context.height);
			this.m_CurrentContext = context;
		}

		// Token: 0x06003CFA RID: 15610 RVA: 0x001648CC File Offset: 0x00162ACC
		public void UpdateVolumeSystem(Camera cam, CommandBuffer cmd)
		{
			if (this.m_SettingsUpdateNeeded)
			{
				cmd.BeginSample("VolumeBlending");
				PostProcessManager.instance.UpdateSettings(this, cam);
				cmd.EndSample("VolumeBlending");
				this.m_TargetPool.Reset();
				if (RuntimeUtilities.scriptableRenderPipelineActive)
				{
					Shader.SetGlobalFloat(ShaderIDs.RenderViewportScaleFactor, 1f);
				}
			}
			this.m_SettingsUpdateNeeded = false;
		}

		// Token: 0x06003CFB RID: 15611 RVA: 0x0016492C File Offset: 0x00162B2C
		public void RenderOpaqueOnly(PostProcessRenderContext context)
		{
			if (RuntimeUtilities.scriptableRenderPipelineActive)
			{
				this.SetupContext(context);
			}
			TextureLerper.instance.BeginFrame(context);
			this.UpdateVolumeSystem(context.camera, context.command);
			this.RenderList(this.sortedBundles[PostProcessEvent.BeforeTransparent], context, "OpaqueOnly");
		}

		// Token: 0x06003CFC RID: 15612 RVA: 0x0016497C File Offset: 0x00162B7C
		public void Render(PostProcessRenderContext context)
		{
			if (RuntimeUtilities.scriptableRenderPipelineActive)
			{
				this.SetupContext(context);
			}
			TextureLerper.instance.BeginFrame(context);
			CommandBuffer command = context.command;
			this.UpdateVolumeSystem(context.camera, context.command);
			int num = -1;
			RenderTargetIdentifier source = context.source;
			if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
			{
				command.SetSinglePassStereo(SinglePassStereoMode.None);
				command.DisableShaderKeyword("UNITY_SINGLE_PASS_STEREO");
			}
			for (int i = 0; i < context.numberOfEyes; i++)
			{
				bool flag = false;
				if (this.stopNaNPropagation && !this.m_NaNKilled)
				{
					num = this.m_TargetPool.Get();
					context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, 0, 0);
					if (context.stereoActive && context.numberOfEyes > 1)
					{
						if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
						{
							command.BlitFullscreenTriangleFromTexArray(context.source, num, RuntimeUtilities.copyFromTexArraySheet, 1, false, i);
							flag = true;
						}
						else if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
						{
							command.BlitFullscreenTriangleFromDoubleWide(context.source, num, RuntimeUtilities.copyStdFromDoubleWideMaterial, 1, i);
							flag = true;
						}
					}
					else
					{
						command.BlitFullscreenTriangle(context.source, num, RuntimeUtilities.copySheet, 1, false, null);
					}
					context.source = num;
					this.m_NaNKilled = true;
				}
				if (!flag && context.numberOfEyes > 1)
				{
					num = this.m_TargetPool.Get();
					context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, 0, 0);
					if (context.stereoActive)
					{
						if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
						{
							command.BlitFullscreenTriangleFromTexArray(context.source, num, RuntimeUtilities.copyFromTexArraySheet, 1, false, i);
						}
						else if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
						{
							command.BlitFullscreenTriangleFromDoubleWide(context.source, num, RuntimeUtilities.copyStdFromDoubleWideMaterial, this.stopNaNPropagation ? 1 : 0, i);
						}
					}
					context.source = num;
				}
				if (context.IsTemporalAntialiasingActive())
				{
					this.temporalAntialiasing.sampleCount = 8;
					if (!RuntimeUtilities.scriptableRenderPipelineActive)
					{
						if (context.stereoActive)
						{
							if (context.camera.stereoActiveEye != Camera.MonoOrStereoscopicEye.Right)
							{
								this.temporalAntialiasing.ConfigureStereoJitteredProjectionMatrices(context);
							}
						}
						else
						{
							this.temporalAntialiasing.ConfigureJitteredProjectionMatrix(context);
						}
					}
					int num2 = this.m_TargetPool.Get();
					RenderTargetIdentifier destination = context.destination;
					context.GetScreenSpaceTemporaryRT(command, num2, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, 0, 0);
					context.destination = num2;
					this.temporalAntialiasing.Render(context);
					context.source = num2;
					context.destination = destination;
					if (num > -1)
					{
						command.ReleaseTemporaryRT(num);
					}
					num = num2;
				}
				bool flag2 = this.HasActiveEffects(PostProcessEvent.BeforeStack, context);
				bool flag3 = this.HasActiveEffects(PostProcessEvent.AfterStack, context) && !this.breakBeforeColorGrading;
				bool flag4 = (flag3 || this.antialiasingMode == PostProcessLayer.Antialiasing.FastApproximateAntialiasing || (this.antialiasingMode == PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing && this.subpixelMorphologicalAntialiasing.IsSupported())) && !this.breakBeforeColorGrading;
				if (flag2)
				{
					num = this.RenderInjectionPoint(PostProcessEvent.BeforeStack, context, "BeforeStack", num);
				}
				num = this.RenderBuiltins(context, !flag4, num, i);
				if (flag3)
				{
					num = this.RenderInjectionPoint(PostProcessEvent.AfterStack, context, "AfterStack", num);
				}
				if (flag4)
				{
					this.RenderFinalPass(context, num, i);
				}
				if (context.stereoActive)
				{
					context.source = source;
				}
			}
			if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
			{
				command.SetSinglePassStereo(SinglePassStereoMode.SideBySide);
				command.EnableShaderKeyword("UNITY_SINGLE_PASS_STEREO");
			}
			this.debugLayer.RenderSpecialOverlays(context);
			this.debugLayer.RenderMonitors(context);
			TextureLerper.instance.EndFrame();
			this.debugLayer.EndFrame();
			this.m_SettingsUpdateNeeded = true;
			this.m_NaNKilled = false;
		}

		// Token: 0x06003CFD RID: 15613 RVA: 0x00164D30 File Offset: 0x00162F30
		private int RenderInjectionPoint(PostProcessEvent evt, PostProcessRenderContext context, string marker, int releaseTargetAfterUse = -1)
		{
			int num = this.m_TargetPool.Get();
			RenderTargetIdentifier destination = context.destination;
			CommandBuffer command = context.command;
			context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, 0, 0);
			context.destination = num;
			this.RenderList(this.sortedBundles[evt], context, marker);
			context.source = num;
			context.destination = destination;
			if (releaseTargetAfterUse > -1)
			{
				command.ReleaseTemporaryRT(releaseTargetAfterUse);
			}
			return num;
		}

		// Token: 0x06003CFE RID: 15614 RVA: 0x00164DAC File Offset: 0x00162FAC
		private void RenderList(List<PostProcessLayer.SerializedBundleRef> list, PostProcessRenderContext context, string marker)
		{
			CommandBuffer command = context.command;
			command.BeginSample(marker);
			this.m_ActiveEffects.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				PostProcessBundle bundle = list[i].bundle;
				if (bundle.settings.IsEnabledAndSupported(context) && (!context.isSceneView || (context.isSceneView && bundle.attribute.allowInSceneView)))
				{
					this.m_ActiveEffects.Add(bundle.renderer);
				}
			}
			int count = this.m_ActiveEffects.Count;
			if (count == 1)
			{
				this.m_ActiveEffects[0].Render(context);
			}
			else
			{
				this.m_Targets.Clear();
				this.m_Targets.Add(context.source);
				int num = this.m_TargetPool.Get();
				int num2 = this.m_TargetPool.Get();
				for (int j = 0; j < count - 1; j++)
				{
					this.m_Targets.Add((j % 2 == 0) ? num : num2);
				}
				this.m_Targets.Add(context.destination);
				context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, 0, 0);
				if (count > 2)
				{
					context.GetScreenSpaceTemporaryRT(command, num2, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, 0, 0);
				}
				for (int k = 0; k < count; k++)
				{
					context.source = this.m_Targets[k];
					context.destination = this.m_Targets[k + 1];
					this.m_ActiveEffects[k].Render(context);
				}
				command.ReleaseTemporaryRT(num);
				if (count > 2)
				{
					command.ReleaseTemporaryRT(num2);
				}
			}
			command.EndSample(marker);
		}

		// Token: 0x06003CFF RID: 15615 RVA: 0x00164F5A File Offset: 0x0016315A
		private void ApplyFlip(PostProcessRenderContext context, MaterialPropertyBlock properties)
		{
			if (context.flip && !context.isSceneView)
			{
				properties.SetVector(ShaderIDs.UVTransform, new Vector4(1f, 1f, 0f, 0f));
				return;
			}
			this.ApplyDefaultFlip(properties);
		}

		// Token: 0x06003D00 RID: 15616 RVA: 0x00164F98 File Offset: 0x00163198
		private void ApplyDefaultFlip(MaterialPropertyBlock properties)
		{
			properties.SetVector(ShaderIDs.UVTransform, SystemInfo.graphicsUVStartsAtTop ? new Vector4(1f, -1f, 0f, 1f) : new Vector4(1f, 1f, 0f, 0f));
		}

		// Token: 0x06003D01 RID: 15617 RVA: 0x00164FEC File Offset: 0x001631EC
		private int RenderBuiltins(PostProcessRenderContext context, bool isFinalPass, int releaseTargetAfterUse = -1, int eye = -1)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.uber);
			propertySheet.ClearKeywords();
			propertySheet.properties.Clear();
			context.uberSheet = propertySheet;
			context.bloomBufferNameID = -1;
			context.autoExposureTexture = RuntimeUtilities.whiteTexture;
			if (isFinalPass && context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
			{
				propertySheet.EnableKeyword("STEREO_INSTANCING_ENABLED");
			}
			CommandBuffer command = context.command;
			command.BeginSample("BuiltinStack");
			int num = -1;
			RenderTargetIdentifier destination = context.destination;
			if (!isFinalPass)
			{
				num = this.m_TargetPool.Get();
				context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, 0, 0);
				context.destination = num;
				if (this.antialiasingMode == PostProcessLayer.Antialiasing.FastApproximateAntialiasing && !this.fastApproximateAntialiasing.keepAlpha)
				{
					propertySheet.properties.SetFloat(ShaderIDs.LumaInAlpha, 1f);
				}
			}
			int num2 = this.RenderEffect<DepthOfFieldEffect>(context, true);
			int num3 = this.RenderEffect<MotionBlur>(context, true);
			if (this.ShouldGenerateLogHistogram(context))
			{
				this.m_LogHistogram.Generate(context);
			}
			this.RenderEffect<AutoExposure>(context, false);
			propertySheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);
			this.RenderEffect<LensDistortion>(context, false);
			this.RenderEffect<ChromaticAberration>(context, false);
			this.RenderEffect<Bloom>(context, false);
			this.RenderEffect<Vignette>(context, false);
			this.RenderEffect<Grain>(context, false);
			if (!this.breakBeforeColorGrading)
			{
				this.RenderEffect<ColorGrading>(context, false);
			}
			if (isFinalPass)
			{
				propertySheet.EnableKeyword("FINALPASS");
				this.dithering.Render(context);
				this.ApplyFlip(context, propertySheet.properties);
			}
			else
			{
				this.ApplyDefaultFlip(propertySheet.properties);
			}
			if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
			{
				propertySheet.properties.SetFloat(ShaderIDs.DepthSlice, (float)eye);
				command.BlitFullscreenTriangleToTexArray(context.source, context.destination, propertySheet, 0, false, eye);
			}
			else if (isFinalPass && context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
			{
				command.BlitFullscreenTriangleToDoubleWide(context.source, context.destination, propertySheet, 0, eye);
			}
			else
			{
				command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
			}
			context.source = context.destination;
			context.destination = destination;
			if (releaseTargetAfterUse > -1)
			{
				command.ReleaseTemporaryRT(releaseTargetAfterUse);
			}
			if (num3 > -1)
			{
				command.ReleaseTemporaryRT(num3);
			}
			if (num2 > -1)
			{
				command.ReleaseTemporaryRT(num2);
			}
			if (context.bloomBufferNameID > -1)
			{
				command.ReleaseTemporaryRT(context.bloomBufferNameID);
			}
			command.EndSample("BuiltinStack");
			return num;
		}

		// Token: 0x06003D02 RID: 15618 RVA: 0x00165278 File Offset: 0x00163478
		private void RenderFinalPass(PostProcessRenderContext context, int releaseTargetAfterUse = -1, int eye = -1)
		{
			CommandBuffer command = context.command;
			command.BeginSample("FinalPass");
			if (this.breakBeforeColorGrading)
			{
				PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.discardAlpha);
				if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
				{
					propertySheet.EnableKeyword("STEREO_INSTANCING_ENABLED");
				}
				if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
				{
					propertySheet.properties.SetFloat(ShaderIDs.DepthSlice, (float)eye);
					command.BlitFullscreenTriangleToTexArray(context.source, context.destination, propertySheet, 0, false, eye);
				}
				else if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
				{
					command.BlitFullscreenTriangleToDoubleWide(context.source, context.destination, propertySheet, 0, eye);
				}
				else
				{
					command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
				}
			}
			else
			{
				PropertySheet propertySheet2 = context.propertySheets.Get(context.resources.shaders.finalPass);
				propertySheet2.ClearKeywords();
				propertySheet2.properties.Clear();
				context.uberSheet = propertySheet2;
				int num = -1;
				if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
				{
					propertySheet2.EnableKeyword("STEREO_INSTANCING_ENABLED");
				}
				if (this.antialiasingMode == PostProcessLayer.Antialiasing.FastApproximateAntialiasing)
				{
					propertySheet2.EnableKeyword(this.fastApproximateAntialiasing.fastMode ? "FXAA_LOW" : "FXAA");
					if (this.fastApproximateAntialiasing.keepAlpha)
					{
						propertySheet2.EnableKeyword("FXAA_KEEP_ALPHA");
					}
				}
				else if (this.antialiasingMode == PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing && this.subpixelMorphologicalAntialiasing.IsSupported())
				{
					num = this.m_TargetPool.Get();
					RenderTargetIdentifier destination = context.destination;
					context.GetScreenSpaceTemporaryRT(context.command, num, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, 0, 0);
					context.destination = num;
					this.subpixelMorphologicalAntialiasing.Render(context);
					context.source = num;
					context.destination = destination;
				}
				this.dithering.Render(context);
				this.ApplyFlip(context, propertySheet2.properties);
				if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
				{
					propertySheet2.properties.SetFloat(ShaderIDs.DepthSlice, (float)eye);
					command.BlitFullscreenTriangleToTexArray(context.source, context.destination, propertySheet2, 0, false, eye);
				}
				else if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
				{
					command.BlitFullscreenTriangleToDoubleWide(context.source, context.destination, propertySheet2, 0, eye);
				}
				else
				{
					command.BlitFullscreenTriangle(context.source, context.destination, propertySheet2, 0, false, null);
				}
				if (num > -1)
				{
					command.ReleaseTemporaryRT(num);
				}
			}
			if (releaseTargetAfterUse > -1)
			{
				command.ReleaseTemporaryRT(releaseTargetAfterUse);
			}
			command.EndSample("FinalPass");
		}

		// Token: 0x06003D03 RID: 15619 RVA: 0x0016553C File Offset: 0x0016373C
		private int RenderEffect<T>(PostProcessRenderContext context, bool useTempTarget = false) where T : PostProcessEffectSettings
		{
			PostProcessBundle bundle = this.GetBundle<T>();
			if (!bundle.settings.IsEnabledAndSupported(context))
			{
				return -1;
			}
			if (this.m_IsRenderingInSceneView && !bundle.attribute.allowInSceneView)
			{
				return -1;
			}
			if (!useTempTarget)
			{
				bundle.renderer.Render(context);
				return -1;
			}
			RenderTargetIdentifier destination = context.destination;
			int num = this.m_TargetPool.Get();
			context.GetScreenSpaceTemporaryRT(context.command, num, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, 0, 0);
			context.destination = num;
			bundle.renderer.Render(context);
			context.source = num;
			context.destination = destination;
			return num;
		}

		// Token: 0x06003D04 RID: 15620 RVA: 0x001655E0 File Offset: 0x001637E0
		private bool ShouldGenerateLogHistogram(PostProcessRenderContext context)
		{
			bool flag = this.GetBundle<AutoExposure>().settings.IsEnabledAndSupported(context);
			bool flag2 = this.debugLayer.lightMeter.IsRequestedAndSupported(context);
			return flag || flag2;
		}

		// Token: 0x040035FD RID: 13821
		public Transform volumeTrigger;

		// Token: 0x040035FE RID: 13822
		public LayerMask volumeLayer;

		// Token: 0x040035FF RID: 13823
		public bool stopNaNPropagation = true;

		// Token: 0x04003600 RID: 13824
		public bool finalBlitToCameraTarget;

		// Token: 0x04003601 RID: 13825
		public PostProcessLayer.Antialiasing antialiasingMode;

		// Token: 0x04003602 RID: 13826
		public TemporalAntialiasing temporalAntialiasing;

		// Token: 0x04003603 RID: 13827
		public SubpixelMorphologicalAntialiasing subpixelMorphologicalAntialiasing;

		// Token: 0x04003604 RID: 13828
		public FastApproximateAntialiasing fastApproximateAntialiasing;

		// Token: 0x04003605 RID: 13829
		public Fog fog;

		// Token: 0x04003606 RID: 13830
		private Dithering dithering;

		// Token: 0x04003607 RID: 13831
		public PostProcessDebugLayer debugLayer;

		// Token: 0x04003608 RID: 13832
		public RenderTextureFormat intermediateFormat = RenderTextureFormat.DefaultHDR;

		// Token: 0x04003609 RID: 13833
		private RenderTextureFormat prevIntermediateFormat = RenderTextureFormat.DefaultHDR;

		// Token: 0x0400360A RID: 13834
		private bool supportsIntermediateFormat = true;

		// Token: 0x0400360B RID: 13835
		[SerializeField]
		private PostProcessResources m_Resources;

		// Token: 0x0400360C RID: 13836
		[Preserve]
		[SerializeField]
		private bool m_ShowToolkit;

		// Token: 0x0400360D RID: 13837
		[Preserve]
		[SerializeField]
		private bool m_ShowCustomSorter;

		// Token: 0x0400360E RID: 13838
		public bool breakBeforeColorGrading;

		// Token: 0x0400360F RID: 13839
		[SerializeField]
		private List<PostProcessLayer.SerializedBundleRef> m_BeforeTransparentBundles;

		// Token: 0x04003610 RID: 13840
		[SerializeField]
		private List<PostProcessLayer.SerializedBundleRef> m_BeforeStackBundles;

		// Token: 0x04003611 RID: 13841
		[SerializeField]
		private List<PostProcessLayer.SerializedBundleRef> m_AfterStackBundles;

		// Token: 0x04003614 RID: 13844
		private Dictionary<Type, PostProcessBundle> m_Bundles;

		// Token: 0x04003615 RID: 13845
		private PropertySheetFactory m_PropertySheetFactory;

		// Token: 0x04003616 RID: 13846
		private CommandBuffer m_LegacyCmdBufferBeforeReflections;

		// Token: 0x04003617 RID: 13847
		private CommandBuffer m_LegacyCmdBufferBeforeLighting;

		// Token: 0x04003618 RID: 13848
		private CommandBuffer m_LegacyCmdBufferOpaque;

		// Token: 0x04003619 RID: 13849
		private CommandBuffer m_LegacyCmdBuffer;

		// Token: 0x0400361A RID: 13850
		private Camera m_Camera;

		// Token: 0x0400361B RID: 13851
		private PostProcessRenderContext m_CurrentContext;

		// Token: 0x0400361C RID: 13852
		private LogHistogram m_LogHistogram;

		// Token: 0x0400361D RID: 13853
		private bool m_SettingsUpdateNeeded = true;

		// Token: 0x0400361E RID: 13854
		private bool m_IsRenderingInSceneView;

		// Token: 0x0400361F RID: 13855
		private TargetPool m_TargetPool;

		// Token: 0x04003620 RID: 13856
		private bool m_NaNKilled;

		// Token: 0x04003621 RID: 13857
		private readonly List<PostProcessEffectRenderer> m_ActiveEffects = new List<PostProcessEffectRenderer>();

		// Token: 0x04003622 RID: 13858
		private readonly List<RenderTargetIdentifier> m_Targets = new List<RenderTargetIdentifier>();

		// Token: 0x02000EB4 RID: 3764
		private enum ScalingMode
		{
			// Token: 0x04004BAC RID: 19372
			NATIVE,
			// Token: 0x04004BAD RID: 19373
			BILINEAR,
			// Token: 0x04004BAE RID: 19374
			DLSS
		}

		// Token: 0x02000EB5 RID: 3765
		public enum Antialiasing
		{
			// Token: 0x04004BB0 RID: 19376
			None,
			// Token: 0x04004BB1 RID: 19377
			FastApproximateAntialiasing,
			// Token: 0x04004BB2 RID: 19378
			SubpixelMorphologicalAntialiasing,
			// Token: 0x04004BB3 RID: 19379
			TemporalAntialiasing
		}

		// Token: 0x02000EB6 RID: 3766
		[Serializable]
		public sealed class SerializedBundleRef
		{
			// Token: 0x04004BB4 RID: 19380
			public string assemblyQualifiedName;

			// Token: 0x04004BB5 RID: 19381
			public PostProcessBundle bundle;
		}
	}
}
