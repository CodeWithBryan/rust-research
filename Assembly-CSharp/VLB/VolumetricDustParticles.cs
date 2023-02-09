using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x02000989 RID: 2441
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dustparticles/")]
	public class VolumetricDustParticles : MonoBehaviour
	{
		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x060039BA RID: 14778 RVA: 0x0015503E File Offset: 0x0015323E
		// (set) Token: 0x060039BB RID: 14779 RVA: 0x00155046 File Offset: 0x00153246
		public bool isCulled { get; private set; }

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x060039BC RID: 14780 RVA: 0x0015504F File Offset: 0x0015324F
		public bool particlesAreInstantiated
		{
			get
			{
				return this.m_Particles;
			}
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x060039BD RID: 14781 RVA: 0x0015505C File Offset: 0x0015325C
		public int particlesCurrentCount
		{
			get
			{
				if (!this.m_Particles)
				{
					return 0;
				}
				return this.m_Particles.particleCount;
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x060039BE RID: 14782 RVA: 0x00155078 File Offset: 0x00153278
		public int particlesMaxCount
		{
			get
			{
				if (!this.m_Particles)
				{
					return 0;
				}
				return this.m_Particles.main.maxParticles;
			}
		}

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x060039BF RID: 14783 RVA: 0x001550A8 File Offset: 0x001532A8
		public Camera mainCamera
		{
			get
			{
				if (!VolumetricDustParticles.ms_MainCamera)
				{
					VolumetricDustParticles.ms_MainCamera = Camera.main;
					if (!VolumetricDustParticles.ms_MainCamera && !VolumetricDustParticles.ms_NoMainCameraLogged)
					{
						Debug.LogErrorFormat(base.gameObject, "In order to use 'VolumetricDustParticles' culling, you must have a MainCamera defined in your scene.", Array.Empty<object>());
						VolumetricDustParticles.ms_NoMainCameraLogged = true;
					}
				}
				return VolumetricDustParticles.ms_MainCamera;
			}
		}

		// Token: 0x060039C0 RID: 14784 RVA: 0x001550FE File Offset: 0x001532FE
		private void Start()
		{
			this.isCulled = false;
			this.m_Master = base.GetComponent<VolumetricLightBeam>();
			Debug.Assert(this.m_Master);
			this.InstantiateParticleSystem();
			this.SetActiveAndPlay();
		}

		// Token: 0x060039C1 RID: 14785 RVA: 0x00155130 File Offset: 0x00153330
		private void InstantiateParticleSystem()
		{
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>(true);
			for (int i = componentsInChildren.Length - 1; i >= 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
			}
			this.m_Particles = Config.Instance.NewVolumetricDustParticles();
			if (this.m_Particles)
			{
				this.m_Particles.transform.SetParent(base.transform, false);
				this.m_Renderer = this.m_Particles.GetComponent<ParticleSystemRenderer>();
			}
		}

		// Token: 0x060039C2 RID: 14786 RVA: 0x001551A7 File Offset: 0x001533A7
		private void OnEnable()
		{
			this.SetActiveAndPlay();
		}

		// Token: 0x060039C3 RID: 14787 RVA: 0x001551AF File Offset: 0x001533AF
		private void SetActiveAndPlay()
		{
			if (this.m_Particles)
			{
				this.m_Particles.gameObject.SetActive(true);
				this.SetParticleProperties();
				this.m_Particles.Play(true);
			}
		}

		// Token: 0x060039C4 RID: 14788 RVA: 0x001551E1 File Offset: 0x001533E1
		private void OnDisable()
		{
			if (this.m_Particles)
			{
				this.m_Particles.gameObject.SetActive(false);
			}
		}

		// Token: 0x060039C5 RID: 14789 RVA: 0x00155201 File Offset: 0x00153401
		private void OnDestroy()
		{
			if (this.m_Particles)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Particles.gameObject);
			}
			this.m_Particles = null;
		}

		// Token: 0x060039C6 RID: 14790 RVA: 0x00155227 File Offset: 0x00153427
		private void Update()
		{
			if (Application.isPlaying)
			{
				this.UpdateCulling();
			}
			this.SetParticleProperties();
		}

		// Token: 0x060039C7 RID: 14791 RVA: 0x0015523C File Offset: 0x0015343C
		private void SetParticleProperties()
		{
			if (this.m_Particles && this.m_Particles.gameObject.activeSelf)
			{
				float t = Mathf.Clamp01(1f - this.m_Master.fresnelPow / 10f);
				float num = this.m_Master.fadeEnd * this.spawnMaxDistance;
				float num2 = num * this.density;
				int maxParticles = (int)(num2 * 4f);
				ParticleSystem.MainModule main = this.m_Particles.main;
				ParticleSystem.MinMaxCurve startLifetime = main.startLifetime;
				startLifetime.mode = ParticleSystemCurveMode.TwoConstants;
				startLifetime.constantMin = 4f;
				startLifetime.constantMax = 6f;
				main.startLifetime = startLifetime;
				ParticleSystem.MinMaxCurve startSize = main.startSize;
				startSize.mode = ParticleSystemCurveMode.TwoConstants;
				startSize.constantMin = this.size * 0.9f;
				startSize.constantMax = this.size * 1.1f;
				main.startSize = startSize;
				ParticleSystem.MinMaxGradient startColor = main.startColor;
				if (this.m_Master.colorMode == ColorMode.Flat)
				{
					startColor.mode = ParticleSystemGradientMode.Color;
					Color color = this.m_Master.color;
					color.a *= this.alpha;
					startColor.color = color;
				}
				else
				{
					startColor.mode = ParticleSystemGradientMode.Gradient;
					Gradient colorGradient = this.m_Master.colorGradient;
					GradientColorKey[] colorKeys = colorGradient.colorKeys;
					GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;
					for (int i = 0; i < alphaKeys.Length; i++)
					{
						GradientAlphaKey[] array = alphaKeys;
						int num3 = i;
						array[num3].alpha = array[num3].alpha * this.alpha;
					}
					Gradient gradient = new Gradient();
					gradient.SetKeys(colorKeys, alphaKeys);
					startColor.gradient = gradient;
				}
				main.startColor = startColor;
				ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
				startSpeed.constant = this.speed;
				main.startSpeed = startSpeed;
				main.maxParticles = maxParticles;
				ParticleSystem.ShapeModule shape = this.m_Particles.shape;
				shape.shapeType = ParticleSystemShapeType.ConeVolume;
				shape.radius = this.m_Master.coneRadiusStart * Mathf.Lerp(0.3f, 1f, t);
				shape.angle = this.m_Master.coneAngle * 0.5f * Mathf.Lerp(0.7f, 1f, t);
				shape.length = num;
				shape.arc = 360f;
				shape.randomDirectionAmount = ((this.direction == VolumetricDustParticles.Direction.Random) ? 1f : 0f);
				ParticleSystem.EmissionModule emission = this.m_Particles.emission;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				rateOverTime.constant = num2;
				emission.rateOverTime = rateOverTime;
				if (this.m_Renderer)
				{
					this.m_Renderer.sortingLayerID = this.m_Master.sortingLayerID;
					this.m_Renderer.sortingOrder = this.m_Master.sortingOrder;
				}
			}
		}

		// Token: 0x060039C8 RID: 14792 RVA: 0x00155500 File Offset: 0x00153700
		private void UpdateCulling()
		{
			if (this.m_Particles)
			{
				bool flag = true;
				if (this.cullingEnabled && this.m_Master.hasGeometry)
				{
					if (this.mainCamera)
					{
						float num = this.cullingMaxDistance * this.cullingMaxDistance;
						flag = (this.m_Master.bounds.SqrDistance(this.mainCamera.transform.position) <= num);
					}
					else
					{
						this.cullingEnabled = false;
					}
				}
				if (this.m_Particles.gameObject.activeSelf != flag)
				{
					this.m_Particles.gameObject.SetActive(flag);
					this.isCulled = !flag;
				}
				if (flag && !this.m_Particles.isPlaying)
				{
					this.m_Particles.Play();
				}
			}
		}

		// Token: 0x04003449 RID: 13385
		[Range(0f, 1f)]
		public float alpha = 0.5f;

		// Token: 0x0400344A RID: 13386
		[Range(0.0001f, 0.1f)]
		public float size = 0.01f;

		// Token: 0x0400344B RID: 13387
		public VolumetricDustParticles.Direction direction = VolumetricDustParticles.Direction.Random;

		// Token: 0x0400344C RID: 13388
		public float speed = 0.03f;

		// Token: 0x0400344D RID: 13389
		public float density = 5f;

		// Token: 0x0400344E RID: 13390
		[Range(0f, 1f)]
		public float spawnMaxDistance = 0.7f;

		// Token: 0x0400344F RID: 13391
		public bool cullingEnabled = true;

		// Token: 0x04003450 RID: 13392
		public float cullingMaxDistance = 10f;

		// Token: 0x04003452 RID: 13394
		public static bool isFeatureSupported = true;

		// Token: 0x04003453 RID: 13395
		private ParticleSystem m_Particles;

		// Token: 0x04003454 RID: 13396
		private ParticleSystemRenderer m_Renderer;

		// Token: 0x04003455 RID: 13397
		private static bool ms_NoMainCameraLogged = false;

		// Token: 0x04003456 RID: 13398
		private static Camera ms_MainCamera = null;

		// Token: 0x04003457 RID: 13399
		private VolumetricLightBeam m_Master;

		// Token: 0x02000E85 RID: 3717
		public enum Direction
		{
			// Token: 0x04004AE5 RID: 19173
			Beam,
			// Token: 0x04004AE6 RID: 19174
			Random
		}
	}
}
