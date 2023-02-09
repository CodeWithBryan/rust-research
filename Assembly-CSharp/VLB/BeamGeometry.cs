using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VLB
{
	// Token: 0x02000978 RID: 2424
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	public class BeamGeometry : MonoBehaviour
	{
		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06003959 RID: 14681 RVA: 0x001532C5 File Offset: 0x001514C5
		// (set) Token: 0x0600395A RID: 14682 RVA: 0x001532CD File Offset: 0x001514CD
		public MeshRenderer meshRenderer { get; private set; }

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x0600395B RID: 14683 RVA: 0x001532D6 File Offset: 0x001514D6
		// (set) Token: 0x0600395C RID: 14684 RVA: 0x001532DE File Offset: 0x001514DE
		public MeshFilter meshFilter { get; private set; }

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x0600395D RID: 14685 RVA: 0x001532E7 File Offset: 0x001514E7
		// (set) Token: 0x0600395E RID: 14686 RVA: 0x001532EF File Offset: 0x001514EF
		public Material material { get; private set; }

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x0600395F RID: 14687 RVA: 0x001532F8 File Offset: 0x001514F8
		// (set) Token: 0x06003960 RID: 14688 RVA: 0x00153300 File Offset: 0x00151500
		public Mesh coneMesh { get; private set; }

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06003961 RID: 14689 RVA: 0x00153309 File Offset: 0x00151509
		// (set) Token: 0x06003962 RID: 14690 RVA: 0x00153316 File Offset: 0x00151516
		public bool visible
		{
			get
			{
				return this.meshRenderer.enabled;
			}
			set
			{
				this.meshRenderer.enabled = value;
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06003963 RID: 14691 RVA: 0x00153324 File Offset: 0x00151524
		// (set) Token: 0x06003964 RID: 14692 RVA: 0x00153331 File Offset: 0x00151531
		public int sortingLayerID
		{
			get
			{
				return this.meshRenderer.sortingLayerID;
			}
			set
			{
				this.meshRenderer.sortingLayerID = value;
			}
		}

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06003965 RID: 14693 RVA: 0x0015333F File Offset: 0x0015153F
		// (set) Token: 0x06003966 RID: 14694 RVA: 0x0015334C File Offset: 0x0015154C
		public int sortingOrder
		{
			get
			{
				return this.meshRenderer.sortingOrder;
			}
			set
			{
				this.meshRenderer.sortingOrder = value;
			}
		}

		// Token: 0x06003967 RID: 14695 RVA: 0x000059DD File Offset: 0x00003BDD
		private void Start()
		{
		}

		// Token: 0x06003968 RID: 14696 RVA: 0x0015335A File Offset: 0x0015155A
		private void OnDestroy()
		{
			if (this.material)
			{
				UnityEngine.Object.DestroyImmediate(this.material);
				this.material = null;
			}
		}

		// Token: 0x06003969 RID: 14697 RVA: 0x0015337B File Offset: 0x0015157B
		private static bool IsUsingCustomRenderPipeline()
		{
			return RenderPipelineManager.currentPipeline != null || GraphicsSettings.renderPipelineAsset != null;
		}

		// Token: 0x0600396A RID: 14698 RVA: 0x00153391 File Offset: 0x00151591
		private void OnEnable()
		{
			if (BeamGeometry.IsUsingCustomRenderPipeline())
			{
				RenderPipelineManager.beginCameraRendering += this.OnBeginCameraRendering;
			}
		}

		// Token: 0x0600396B RID: 14699 RVA: 0x001533AB File Offset: 0x001515AB
		private void OnDisable()
		{
			if (BeamGeometry.IsUsingCustomRenderPipeline())
			{
				RenderPipelineManager.beginCameraRendering -= this.OnBeginCameraRendering;
			}
		}

		// Token: 0x0600396C RID: 14700 RVA: 0x001533C8 File Offset: 0x001515C8
		public void Initialize(VolumetricLightBeam master, Shader shader)
		{
			HideFlags proceduralObjectsHideFlags = Consts.ProceduralObjectsHideFlags;
			this.m_Master = master;
			base.transform.SetParent(master.transform, false);
			this.material = new Material(shader);
			this.material.hideFlags = proceduralObjectsHideFlags;
			this.meshRenderer = base.gameObject.GetOrAddComponent<MeshRenderer>();
			this.meshRenderer.hideFlags = proceduralObjectsHideFlags;
			this.meshRenderer.material = this.material;
			this.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			this.meshRenderer.receiveShadows = false;
			this.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			if (SortingLayer.IsValid(this.m_Master.sortingLayerID))
			{
				this.sortingLayerID = this.m_Master.sortingLayerID;
			}
			else
			{
				Debug.LogError(string.Format("Beam '{0}' has an invalid sortingLayerID ({1}). Please fix it by setting a valid layer.", Utils.GetPath(this.m_Master.transform), this.m_Master.sortingLayerID));
			}
			this.sortingOrder = this.m_Master.sortingOrder;
			this.meshFilter = base.gameObject.GetOrAddComponent<MeshFilter>();
			this.meshFilter.hideFlags = proceduralObjectsHideFlags;
			base.gameObject.hideFlags = proceduralObjectsHideFlags;
		}

		// Token: 0x0600396D RID: 14701 RVA: 0x001534EC File Offset: 0x001516EC
		public void RegenerateMesh()
		{
			Debug.Assert(this.m_Master);
			base.gameObject.layer = Config.Instance.geometryLayerID;
			base.gameObject.tag = Config.Instance.geometryTag;
			if (this.coneMesh && this.m_CurrentMeshType == MeshType.Custom)
			{
				UnityEngine.Object.DestroyImmediate(this.coneMesh);
			}
			this.m_CurrentMeshType = this.m_Master.geomMeshType;
			MeshType geomMeshType = this.m_Master.geomMeshType;
			if (geomMeshType != MeshType.Shared)
			{
				if (geomMeshType == MeshType.Custom)
				{
					this.coneMesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, this.m_Master.geomCustomSides, this.m_Master.geomCustomSegments, this.m_Master.geomCap);
					this.coneMesh.hideFlags = Consts.ProceduralObjectsHideFlags;
					this.meshFilter.mesh = this.coneMesh;
				}
				else
				{
					Debug.LogError("Unsupported MeshType");
				}
			}
			else
			{
				this.coneMesh = GlobalMesh.mesh;
				this.meshFilter.sharedMesh = this.coneMesh;
			}
			this.UpdateMaterialAndBounds();
		}

		// Token: 0x0600396E RID: 14702 RVA: 0x00153604 File Offset: 0x00151804
		private void ComputeLocalMatrix()
		{
			float num = Mathf.Max(this.m_Master.coneRadiusStart, this.m_Master.coneRadiusEnd);
			base.transform.localScale = new Vector3(num, num, this.m_Master.fadeEnd);
		}

		// Token: 0x0600396F RID: 14703 RVA: 0x0015364C File Offset: 0x0015184C
		public void UpdateMaterialAndBounds()
		{
			Debug.Assert(this.m_Master);
			this.material.renderQueue = Config.Instance.geometryRenderQueue;
			float f = this.m_Master.coneAngle * 0.017453292f / 2f;
			this.material.SetVector("_ConeSlopeCosSin", new Vector2(Mathf.Cos(f), Mathf.Sin(f)));
			Vector2 v = new Vector2(Mathf.Max(this.m_Master.coneRadiusStart, 0.0001f), Mathf.Max(this.m_Master.coneRadiusEnd, 0.0001f));
			this.material.SetVector("_ConeRadius", v);
			float value = Mathf.Sign(this.m_Master.coneApexOffsetZ) * Mathf.Max(Mathf.Abs(this.m_Master.coneApexOffsetZ), 0.0001f);
			this.material.SetFloat("_ConeApexOffsetZ", value);
			if (this.m_Master.colorMode == ColorMode.Gradient)
			{
				Utils.FloatPackingPrecision floatPackingPrecision = Utils.GetFloatPackingPrecision();
				this.material.EnableKeyword((floatPackingPrecision == Utils.FloatPackingPrecision.High) ? "VLB_COLOR_GRADIENT_MATRIX_HIGH" : "VLB_COLOR_GRADIENT_MATRIX_LOW");
				this.m_ColorGradientMatrix = this.m_Master.colorGradient.SampleInMatrix((int)floatPackingPrecision);
			}
			else
			{
				this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_HIGH");
				this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_LOW");
				this.material.SetColor("_ColorFlat", this.m_Master.color);
			}
			if (Consts.BlendingMode_AlphaAsBlack[this.m_Master.blendingModeAsInt])
			{
				this.material.EnableKeyword("ALPHA_AS_BLACK");
			}
			else
			{
				this.material.DisableKeyword("ALPHA_AS_BLACK");
			}
			this.material.SetInt("_BlendSrcFactor", (int)Consts.BlendingMode_SrcFactor[this.m_Master.blendingModeAsInt]);
			this.material.SetInt("_BlendDstFactor", (int)Consts.BlendingMode_DstFactor[this.m_Master.blendingModeAsInt]);
			this.material.SetFloat("_AlphaInside", this.m_Master.alphaInside);
			this.material.SetFloat("_AlphaOutside", this.m_Master.alphaOutside);
			this.material.SetFloat("_AttenuationLerpLinearQuad", this.m_Master.attenuationLerpLinearQuad);
			this.material.SetFloat("_DistanceFadeStart", this.m_Master.fadeStart);
			this.material.SetFloat("_DistanceFadeEnd", this.m_Master.fadeEnd);
			this.material.SetFloat("_DistanceCamClipping", this.m_Master.cameraClippingDistance);
			this.material.SetFloat("_FresnelPow", Mathf.Max(0.001f, this.m_Master.fresnelPow));
			this.material.SetFloat("_GlareBehind", this.m_Master.glareBehind);
			this.material.SetFloat("_GlareFrontal", this.m_Master.glareFrontal);
			this.material.SetFloat("_DrawCap", (float)(this.m_Master.geomCap ? 1 : 0));
			if (this.m_Master.depthBlendDistance > 0f)
			{
				this.material.EnableKeyword("VLB_DEPTH_BLEND");
				this.material.SetFloat("_DepthBlendDistance", this.m_Master.depthBlendDistance);
			}
			else
			{
				this.material.DisableKeyword("VLB_DEPTH_BLEND");
			}
			if (this.m_Master.noiseEnabled && this.m_Master.noiseIntensity > 0f && Noise3D.isSupported)
			{
				Noise3D.LoadIfNeeded();
				this.material.EnableKeyword("VLB_NOISE_3D");
				this.material.SetVector("_NoiseLocal", new Vector4(this.m_Master.noiseVelocityLocal.x, this.m_Master.noiseVelocityLocal.y, this.m_Master.noiseVelocityLocal.z, this.m_Master.noiseScaleLocal));
				this.material.SetVector("_NoiseParam", new Vector3(this.m_Master.noiseIntensity, this.m_Master.noiseVelocityUseGlobal ? 1f : 0f, this.m_Master.noiseScaleUseGlobal ? 1f : 0f));
			}
			else
			{
				this.material.DisableKeyword("VLB_NOISE_3D");
			}
			this.ComputeLocalMatrix();
		}

		// Token: 0x06003970 RID: 14704 RVA: 0x00153AAC File Offset: 0x00151CAC
		public void SetClippingPlane(Plane planeWS)
		{
			Vector3 normal = planeWS.normal;
			this.material.EnableKeyword("VLB_CLIPPING_PLANE");
			this.material.SetVector("_ClippingPlaneWS", new Vector4(normal.x, normal.y, normal.z, planeWS.distance));
		}

		// Token: 0x06003971 RID: 14705 RVA: 0x00153AFF File Offset: 0x00151CFF
		public void SetClippingPlaneOff()
		{
			this.material.DisableKeyword("VLB_CLIPPING_PLANE");
		}

		// Token: 0x06003972 RID: 14706 RVA: 0x00153B11 File Offset: 0x00151D11
		private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
		{
			this.UpdateCameraRelatedProperties(cam);
		}

		// Token: 0x06003973 RID: 14707 RVA: 0x00153B1C File Offset: 0x00151D1C
		private void OnWillRenderObject()
		{
			if (!BeamGeometry.IsUsingCustomRenderPipeline())
			{
				Camera current = Camera.current;
				if (current != null)
				{
					this.UpdateCameraRelatedProperties(current);
				}
			}
		}

		// Token: 0x06003974 RID: 14708 RVA: 0x00153B48 File Offset: 0x00151D48
		private void UpdateCameraRelatedProperties(Camera cam)
		{
			if (cam && this.m_Master)
			{
				if (this.material)
				{
					Vector3 vector = this.m_Master.transform.InverseTransformPoint(cam.transform.position);
					this.material.SetVector("_CameraPosObjectSpace", vector);
					Vector3 normalized = base.transform.InverseTransformDirection(cam.transform.forward).normalized;
					float w = cam.orthographic ? -1f : this.m_Master.GetInsideBeamFactorFromObjectSpacePos(vector);
					this.material.SetVector("_CameraParams", new Vector4(normalized.x, normalized.y, normalized.z, w));
					if (this.m_Master.colorMode == ColorMode.Gradient)
					{
						this.material.SetMatrix("_ColorGradientMatrix", this.m_ColorGradientMatrix);
					}
				}
				if (this.m_Master.depthBlendDistance > 0f)
				{
					cam.depthTextureMode |= DepthTextureMode.Depth;
				}
			}
		}

		// Token: 0x040033C9 RID: 13257
		private VolumetricLightBeam m_Master;

		// Token: 0x040033CA RID: 13258
		private Matrix4x4 m_ColorGradientMatrix;

		// Token: 0x040033CB RID: 13259
		private MeshType m_CurrentMeshType;
	}
}
