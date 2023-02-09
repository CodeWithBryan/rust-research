using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	// Token: 0x0200098A RID: 2442
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[SelectionBase]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	public class VolumetricLightBeam : MonoBehaviour
	{
		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x060039CB RID: 14795 RVA: 0x00155643 File Offset: 0x00153843
		public float coneAngle
		{
			get
			{
				return Mathf.Atan2(this.coneRadiusEnd - this.coneRadiusStart, this.fadeEnd) * 57.29578f * 2f;
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x060039CC RID: 14796 RVA: 0x00155669 File Offset: 0x00153869
		public float coneRadiusEnd
		{
			get
			{
				return this.fadeEnd * Mathf.Tan(this.spotAngle * 0.017453292f * 0.5f);
			}
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x060039CD RID: 14797 RVA: 0x0015568C File Offset: 0x0015388C
		public float coneVolume
		{
			get
			{
				float num = this.coneRadiusStart;
				float coneRadiusEnd = this.coneRadiusEnd;
				return 1.0471976f * (num * num + num * coneRadiusEnd + coneRadiusEnd * coneRadiusEnd) * this.fadeEnd;
			}
		}

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x060039CE RID: 14798 RVA: 0x001556C0 File Offset: 0x001538C0
		public float coneApexOffsetZ
		{
			get
			{
				float num = this.coneRadiusStart / this.coneRadiusEnd;
				if (num != 1f)
				{
					return this.fadeEnd * num / (1f - num);
				}
				return float.MaxValue;
			}
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x060039CF RID: 14799 RVA: 0x001556F9 File Offset: 0x001538F9
		// (set) Token: 0x060039D0 RID: 14800 RVA: 0x00155715 File Offset: 0x00153915
		public int geomSides
		{
			get
			{
				if (this.geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSides;
				}
				return this.geomCustomSides;
			}
			set
			{
				this.geomCustomSides = value;
				Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSides is OBSOLETE and has been renamed to geomCustomSides.");
			}
		}

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x060039D1 RID: 14801 RVA: 0x00155728 File Offset: 0x00153928
		// (set) Token: 0x060039D2 RID: 14802 RVA: 0x00155744 File Offset: 0x00153944
		public int geomSegments
		{
			get
			{
				if (this.geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSegments;
				}
				return this.geomCustomSegments;
			}
			set
			{
				this.geomCustomSegments = value;
				Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSegments is OBSOLETE and has been renamed to geomCustomSegments.");
			}
		}

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x060039D3 RID: 14803 RVA: 0x00155757 File Offset: 0x00153957
		public float attenuationLerpLinearQuad
		{
			get
			{
				if (this.attenuationEquation == AttenuationEquation.Linear)
				{
					return 0f;
				}
				if (this.attenuationEquation == AttenuationEquation.Quadratic)
				{
					return 1f;
				}
				return this.attenuationCustomBlending;
			}
		}

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x060039D4 RID: 14804 RVA: 0x0015577C File Offset: 0x0015397C
		// (set) Token: 0x060039D5 RID: 14805 RVA: 0x00155784 File Offset: 0x00153984
		public int sortingLayerID
		{
			get
			{
				return this._SortingLayerID;
			}
			set
			{
				this._SortingLayerID = value;
				if (this.m_BeamGeom)
				{
					this.m_BeamGeom.sortingLayerID = value;
				}
			}
		}

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x060039D6 RID: 14806 RVA: 0x001557A6 File Offset: 0x001539A6
		// (set) Token: 0x060039D7 RID: 14807 RVA: 0x001557B3 File Offset: 0x001539B3
		public string sortingLayerName
		{
			get
			{
				return SortingLayer.IDToName(this.sortingLayerID);
			}
			set
			{
				this.sortingLayerID = SortingLayer.NameToID(value);
			}
		}

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x060039D8 RID: 14808 RVA: 0x001557C1 File Offset: 0x001539C1
		// (set) Token: 0x060039D9 RID: 14809 RVA: 0x001557C9 File Offset: 0x001539C9
		public int sortingOrder
		{
			get
			{
				return this._SortingOrder;
			}
			set
			{
				this._SortingOrder = value;
				if (this.m_BeamGeom)
				{
					this.m_BeamGeom.sortingOrder = value;
				}
			}
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x060039DA RID: 14810 RVA: 0x001557EB File Offset: 0x001539EB
		// (set) Token: 0x060039DB RID: 14811 RVA: 0x001557F3 File Offset: 0x001539F3
		public bool trackChangesDuringPlaytime
		{
			get
			{
				return this._TrackChangesDuringPlaytime;
			}
			set
			{
				this._TrackChangesDuringPlaytime = value;
				this.StartPlaytimeUpdateIfNeeded();
			}
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x060039DC RID: 14812 RVA: 0x00155802 File Offset: 0x00153A02
		public bool isCurrentlyTrackingChanges
		{
			get
			{
				return this.m_CoPlaytimeUpdate != null;
			}
		}

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x060039DD RID: 14813 RVA: 0x0015580D File Offset: 0x00153A0D
		public bool hasGeometry
		{
			get
			{
				return this.m_BeamGeom != null;
			}
		}

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x060039DE RID: 14814 RVA: 0x0015581B File Offset: 0x00153A1B
		public Bounds bounds
		{
			get
			{
				if (!(this.m_BeamGeom != null))
				{
					return new Bounds(Vector3.zero, Vector3.zero);
				}
				return this.m_BeamGeom.meshRenderer.bounds;
			}
		}

		// Token: 0x060039DF RID: 14815 RVA: 0x0015584B File Offset: 0x00153A4B
		public void SetClippingPlane(Plane planeWS)
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.SetClippingPlane(planeWS);
			}
			this.m_PlaneWS = planeWS;
		}

		// Token: 0x060039E0 RID: 14816 RVA: 0x0015586D File Offset: 0x00153A6D
		public void SetClippingPlaneOff()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.SetClippingPlaneOff();
			}
			this.m_PlaneWS = default(Plane);
		}

		// Token: 0x060039E1 RID: 14817 RVA: 0x00155894 File Offset: 0x00153A94
		public bool IsColliderHiddenByDynamicOccluder(Collider collider)
		{
			Debug.Assert(collider, "You should pass a valid Collider to VLB.VolumetricLightBeam.IsColliderHiddenByDynamicOccluder");
			return this.m_PlaneWS.IsValid() && !GeometryUtility.TestPlanesAABB(new Plane[]
			{
				this.m_PlaneWS
			}, collider.bounds);
		}

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x060039E2 RID: 14818 RVA: 0x001558E1 File Offset: 0x00153AE1
		public int blendingModeAsInt
		{
			get
			{
				return Mathf.Clamp((int)this.blendingMode, 0, Enum.GetValues(typeof(BlendingMode)).Length);
			}
		}

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x060039E3 RID: 14819 RVA: 0x00155903 File Offset: 0x00153B03
		public MeshRenderer Renderer
		{
			get
			{
				if (!(this.m_BeamGeom != null))
				{
					return null;
				}
				return this.m_BeamGeom.meshRenderer;
			}
		}

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x060039E4 RID: 14820 RVA: 0x00155920 File Offset: 0x00153B20
		public string meshStats
		{
			get
			{
				Mesh mesh = this.m_BeamGeom ? this.m_BeamGeom.coneMesh : null;
				if (mesh)
				{
					return string.Format("Cone angle: {0:0.0} degrees\nMesh: {1} vertices, {2} triangles", this.coneAngle, mesh.vertexCount, mesh.triangles.Length / 3);
				}
				return "no mesh available";
			}
		}

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x060039E5 RID: 14821 RVA: 0x00155986 File Offset: 0x00153B86
		public int meshVerticesCount
		{
			get
			{
				if (!this.m_BeamGeom || !this.m_BeamGeom.coneMesh)
				{
					return 0;
				}
				return this.m_BeamGeom.coneMesh.vertexCount;
			}
		}

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x060039E6 RID: 14822 RVA: 0x001559B9 File Offset: 0x00153BB9
		public int meshTrianglesCount
		{
			get
			{
				if (!this.m_BeamGeom || !this.m_BeamGeom.coneMesh)
				{
					return 0;
				}
				return this.m_BeamGeom.coneMesh.triangles.Length / 3;
			}
		}

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x060039E7 RID: 14823 RVA: 0x001559F0 File Offset: 0x00153BF0
		private Light lightSpotAttached
		{
			get
			{
				if (this._CachedLight == null)
				{
					this._CachedLight = base.GetComponent<Light>();
				}
				if (this._CachedLight && this._CachedLight.type == LightType.Spot)
				{
					return this._CachedLight;
				}
				return null;
			}
		}

		// Token: 0x060039E8 RID: 14824 RVA: 0x00155A2E File Offset: 0x00153C2E
		public float GetInsideBeamFactor(Vector3 posWS)
		{
			return this.GetInsideBeamFactorFromObjectSpacePos(base.transform.InverseTransformPoint(posWS));
		}

		// Token: 0x060039E9 RID: 14825 RVA: 0x00155A44 File Offset: 0x00153C44
		public float GetInsideBeamFactorFromObjectSpacePos(Vector3 posOS)
		{
			if (posOS.z < 0f)
			{
				return -1f;
			}
			Vector2 normalized = new Vector2(posOS.xy().magnitude, posOS.z + this.coneApexOffsetZ).normalized;
			return Mathf.Clamp((Mathf.Abs(Mathf.Sin(this.coneAngle * 0.017453292f / 2f)) - Mathf.Abs(normalized.x)) / 0.1f, -1f, 1f);
		}

		// Token: 0x060039EA RID: 14826 RVA: 0x00155ACA File Offset: 0x00153CCA
		[Obsolete("Use 'GenerateGeometry()' instead")]
		public void Generate()
		{
			this.GenerateGeometry();
		}

		// Token: 0x060039EB RID: 14827 RVA: 0x00155AD4 File Offset: 0x00153CD4
		public virtual void GenerateGeometry()
		{
			this.HandleBackwardCompatibility(this.pluginVersion, 1510);
			this.pluginVersion = 1510;
			this.ValidateProperties();
			if (this.m_BeamGeom == null)
			{
				Shader beamShader = Config.Instance.beamShader;
				if (!beamShader)
				{
					Debug.LogError("Invalid BeamShader set in VLB Config");
					return;
				}
				this.m_BeamGeom = Utils.NewWithComponent<BeamGeometry>("Beam Geometry");
				this.m_BeamGeom.Initialize(this, beamShader);
			}
			this.m_BeamGeom.RegenerateMesh();
			this.m_BeamGeom.visible = base.enabled;
		}

		// Token: 0x060039EC RID: 14828 RVA: 0x00155B68 File Offset: 0x00153D68
		public virtual void UpdateAfterManualPropertyChange()
		{
			this.ValidateProperties();
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.UpdateMaterialAndBounds();
			}
		}

		// Token: 0x060039ED RID: 14829 RVA: 0x00155ACA File Offset: 0x00153CCA
		private void Start()
		{
			this.GenerateGeometry();
		}

		// Token: 0x060039EE RID: 14830 RVA: 0x00155B88 File Offset: 0x00153D88
		private void OnEnable()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.visible = true;
			}
			this.StartPlaytimeUpdateIfNeeded();
		}

		// Token: 0x060039EF RID: 14831 RVA: 0x00155BA9 File Offset: 0x00153DA9
		private void OnDisable()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.visible = false;
			}
			this.m_CoPlaytimeUpdate = null;
		}

		// Token: 0x060039F0 RID: 14832 RVA: 0x000059DD File Offset: 0x00003BDD
		private void StartPlaytimeUpdateIfNeeded()
		{
		}

		// Token: 0x060039F1 RID: 14833 RVA: 0x00155BCB File Offset: 0x00153DCB
		private IEnumerator CoPlaytimeUpdate()
		{
			while (this.trackChangesDuringPlaytime && base.enabled)
			{
				this.UpdateAfterManualPropertyChange();
				yield return null;
			}
			this.m_CoPlaytimeUpdate = null;
			yield break;
		}

		// Token: 0x060039F2 RID: 14834 RVA: 0x00155BDA File Offset: 0x00153DDA
		private void OnDestroy()
		{
			this.DestroyBeam();
		}

		// Token: 0x060039F3 RID: 14835 RVA: 0x00155BE2 File Offset: 0x00153DE2
		private void DestroyBeam()
		{
			if (this.m_BeamGeom)
			{
				UnityEngine.Object.DestroyImmediate(this.m_BeamGeom.gameObject);
			}
			this.m_BeamGeom = null;
		}

		// Token: 0x060039F4 RID: 14836 RVA: 0x00155C08 File Offset: 0x00153E08
		private void AssignPropertiesFromSpotLight(Light lightSpot)
		{
			if (lightSpot && lightSpot.type == LightType.Spot)
			{
				if (this.fadeEndFromLight)
				{
					this.fadeEnd = lightSpot.range;
				}
				if (this.spotAngleFromLight)
				{
					this.spotAngle = lightSpot.spotAngle;
				}
				if (this.colorFromLight)
				{
					this.colorMode = ColorMode.Flat;
					this.color = lightSpot.color;
				}
			}
		}

		// Token: 0x060039F5 RID: 14837 RVA: 0x00155C68 File Offset: 0x00153E68
		private void ClampProperties()
		{
			this.alphaInside = Mathf.Clamp01(this.alphaInside);
			this.alphaOutside = Mathf.Clamp01(this.alphaOutside);
			this.attenuationCustomBlending = Mathf.Clamp01(this.attenuationCustomBlending);
			this.fadeEnd = Mathf.Max(0.01f, this.fadeEnd);
			this.fadeStart = Mathf.Clamp(this.fadeStart, 0f, this.fadeEnd - 0.01f);
			this.spotAngle = Mathf.Clamp(this.spotAngle, 0.1f, 179.9f);
			this.coneRadiusStart = Mathf.Max(this.coneRadiusStart, 0f);
			this.depthBlendDistance = Mathf.Max(this.depthBlendDistance, 0f);
			this.cameraClippingDistance = Mathf.Max(this.cameraClippingDistance, 0f);
			this.geomCustomSides = Mathf.Clamp(this.geomCustomSides, 3, 256);
			this.geomCustomSegments = Mathf.Clamp(this.geomCustomSegments, 0, 64);
			this.fresnelPow = Mathf.Max(0f, this.fresnelPow);
			this.glareBehind = Mathf.Clamp01(this.glareBehind);
			this.glareFrontal = Mathf.Clamp01(this.glareFrontal);
			this.noiseIntensity = Mathf.Clamp(this.noiseIntensity, 0f, 1f);
		}

		// Token: 0x060039F6 RID: 14838 RVA: 0x00155DBB File Offset: 0x00153FBB
		private void ValidateProperties()
		{
			this.AssignPropertiesFromSpotLight(this.lightSpotAttached);
			this.ClampProperties();
		}

		// Token: 0x060039F7 RID: 14839 RVA: 0x00155DCF File Offset: 0x00153FCF
		private void HandleBackwardCompatibility(int serializedVersion, int newVersion)
		{
			if (serializedVersion == -1)
			{
				return;
			}
			if (serializedVersion == newVersion)
			{
				return;
			}
			if (serializedVersion < 1301)
			{
				this.attenuationEquation = AttenuationEquation.Linear;
			}
			if (serializedVersion < 1501)
			{
				this.geomMeshType = MeshType.Custom;
				this.geomCustomSegments = 5;
			}
			Utils.MarkCurrentSceneDirty();
		}

		// Token: 0x04003458 RID: 13400
		public bool colorFromLight = true;

		// Token: 0x04003459 RID: 13401
		public ColorMode colorMode;

		// Token: 0x0400345A RID: 13402
		[ColorUsage(true, true)]
		[FormerlySerializedAs("colorValue")]
		public Color color = Consts.FlatColor;

		// Token: 0x0400345B RID: 13403
		public Gradient colorGradient;

		// Token: 0x0400345C RID: 13404
		[Range(0f, 1f)]
		public float alphaInside = 1f;

		// Token: 0x0400345D RID: 13405
		[FormerlySerializedAs("alpha")]
		[Range(0f, 1f)]
		public float alphaOutside = 1f;

		// Token: 0x0400345E RID: 13406
		public BlendingMode blendingMode;

		// Token: 0x0400345F RID: 13407
		[FormerlySerializedAs("angleFromLight")]
		public bool spotAngleFromLight = true;

		// Token: 0x04003460 RID: 13408
		[Range(0.1f, 179.9f)]
		public float spotAngle = 35f;

		// Token: 0x04003461 RID: 13409
		[FormerlySerializedAs("radiusStart")]
		public float coneRadiusStart = 0.1f;

		// Token: 0x04003462 RID: 13410
		public MeshType geomMeshType;

		// Token: 0x04003463 RID: 13411
		[FormerlySerializedAs("geomSides")]
		public int geomCustomSides = 18;

		// Token: 0x04003464 RID: 13412
		public int geomCustomSegments = 5;

		// Token: 0x04003465 RID: 13413
		public bool geomCap;

		// Token: 0x04003466 RID: 13414
		public bool fadeEndFromLight = true;

		// Token: 0x04003467 RID: 13415
		public AttenuationEquation attenuationEquation = AttenuationEquation.Quadratic;

		// Token: 0x04003468 RID: 13416
		[Range(0f, 1f)]
		public float attenuationCustomBlending = 0.5f;

		// Token: 0x04003469 RID: 13417
		public float fadeStart;

		// Token: 0x0400346A RID: 13418
		public float fadeEnd = 3f;

		// Token: 0x0400346B RID: 13419
		public float depthBlendDistance = 2f;

		// Token: 0x0400346C RID: 13420
		public float cameraClippingDistance = 0.5f;

		// Token: 0x0400346D RID: 13421
		[Range(0f, 1f)]
		public float glareFrontal = 0.5f;

		// Token: 0x0400346E RID: 13422
		[Range(0f, 1f)]
		public float glareBehind = 0.5f;

		// Token: 0x0400346F RID: 13423
		[Obsolete("Use 'glareFrontal' instead")]
		public float boostDistanceInside = 0.5f;

		// Token: 0x04003470 RID: 13424
		[Obsolete("This property has been merged with 'fresnelPow'")]
		public float fresnelPowInside = 6f;

		// Token: 0x04003471 RID: 13425
		[FormerlySerializedAs("fresnelPowOutside")]
		public float fresnelPow = 8f;

		// Token: 0x04003472 RID: 13426
		public bool noiseEnabled;

		// Token: 0x04003473 RID: 13427
		[Range(0f, 1f)]
		public float noiseIntensity = 0.5f;

		// Token: 0x04003474 RID: 13428
		public bool noiseScaleUseGlobal = true;

		// Token: 0x04003475 RID: 13429
		[Range(0.01f, 2f)]
		public float noiseScaleLocal = 0.5f;

		// Token: 0x04003476 RID: 13430
		public bool noiseVelocityUseGlobal = true;

		// Token: 0x04003477 RID: 13431
		public Vector3 noiseVelocityLocal = Consts.NoiseVelocityDefault;

		// Token: 0x04003478 RID: 13432
		private Plane m_PlaneWS;

		// Token: 0x04003479 RID: 13433
		[SerializeField]
		private int pluginVersion = -1;

		// Token: 0x0400347A RID: 13434
		[FormerlySerializedAs("trackChangesDuringPlaytime")]
		[SerializeField]
		private bool _TrackChangesDuringPlaytime;

		// Token: 0x0400347B RID: 13435
		[SerializeField]
		private int _SortingLayerID;

		// Token: 0x0400347C RID: 13436
		[SerializeField]
		private int _SortingOrder;

		// Token: 0x0400347D RID: 13437
		private BeamGeometry m_BeamGeom;

		// Token: 0x0400347E RID: 13438
		private Coroutine m_CoPlaytimeUpdate;

		// Token: 0x0400347F RID: 13439
		private Light _CachedLight;
	}
}
