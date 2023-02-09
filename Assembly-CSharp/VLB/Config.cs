using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	// Token: 0x02000979 RID: 2425
	[HelpURL("http://saladgamer.com/vlb-doc/config/")]
	public class Config : ScriptableObject
	{
		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06003976 RID: 14710 RVA: 0x00153C59 File Offset: 0x00151E59
		public Shader beamShader
		{
			get
			{
				if (!this.forceSinglePass)
				{
					return this.beamShader2Pass;
				}
				return this.beamShader1Pass;
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06003977 RID: 14711 RVA: 0x00153C70 File Offset: 0x00151E70
		public Vector4 globalNoiseParam
		{
			get
			{
				return new Vector4(this.globalNoiseVelocity.x, this.globalNoiseVelocity.y, this.globalNoiseVelocity.z, this.globalNoiseScale);
			}
		}

		// Token: 0x06003978 RID: 14712 RVA: 0x00153CA0 File Offset: 0x00151EA0
		public void Reset()
		{
			this.geometryLayerID = 1;
			this.geometryTag = "Untagged";
			this.geometryRenderQueue = 3000;
			this.beamShader1Pass = Shader.Find("Hidden/VolumetricLightBeam1Pass");
			this.beamShader2Pass = Shader.Find("Hidden/VolumetricLightBeam2Pass");
			this.sharedMeshSides = 24;
			this.sharedMeshSegments = 5;
			this.globalNoiseScale = 0.5f;
			this.globalNoiseVelocity = Consts.NoiseVelocityDefault;
			this.noise3DData = (Resources.Load("Noise3D_64x64x64") as TextAsset);
			this.noise3DSize = 64;
			this.dustParticlesPrefab = (Resources.Load("DustParticles", typeof(ParticleSystem)) as ParticleSystem);
		}

		// Token: 0x06003979 RID: 14713 RVA: 0x00153D4C File Offset: 0x00151F4C
		public ParticleSystem NewVolumetricDustParticles()
		{
			if (!this.dustParticlesPrefab)
			{
				if (Application.isPlaying)
				{
					Debug.LogError("Failed to instantiate VolumetricDustParticles prefab.");
				}
				return null;
			}
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate<ParticleSystem>(this.dustParticlesPrefab);
			particleSystem.useAutoRandomSeed = false;
			particleSystem.name = "Dust Particles";
			particleSystem.gameObject.hideFlags = Consts.ProceduralObjectsHideFlags;
			particleSystem.gameObject.SetActive(true);
			return particleSystem;
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x0600397A RID: 14714 RVA: 0x00153DB4 File Offset: 0x00151FB4
		public static Config Instance
		{
			get
			{
				if (Config.m_Instance == null)
				{
					Config[] array = Resources.LoadAll<Config>("Config");
					Debug.Assert(array.Length != 0, string.Format("Can't find any resource of type '{0}'. Make sure you have a ScriptableObject of this type in a 'Resources' folder.", typeof(Config)));
					Config.m_Instance = array[0];
				}
				return Config.m_Instance;
			}
		}

		// Token: 0x040033D0 RID: 13264
		public int geometryLayerID = 1;

		// Token: 0x040033D1 RID: 13265
		public string geometryTag = "Untagged";

		// Token: 0x040033D2 RID: 13266
		public int geometryRenderQueue = 3000;

		// Token: 0x040033D3 RID: 13267
		public bool forceSinglePass;

		// Token: 0x040033D4 RID: 13268
		[SerializeField]
		[HighlightNull]
		private Shader beamShader1Pass;

		// Token: 0x040033D5 RID: 13269
		[FormerlySerializedAs("BeamShader")]
		[FormerlySerializedAs("beamShader")]
		[SerializeField]
		[HighlightNull]
		private Shader beamShader2Pass;

		// Token: 0x040033D6 RID: 13270
		public int sharedMeshSides = 24;

		// Token: 0x040033D7 RID: 13271
		public int sharedMeshSegments = 5;

		// Token: 0x040033D8 RID: 13272
		[Range(0.01f, 2f)]
		public float globalNoiseScale = 0.5f;

		// Token: 0x040033D9 RID: 13273
		public Vector3 globalNoiseVelocity = Consts.NoiseVelocityDefault;

		// Token: 0x040033DA RID: 13274
		[HighlightNull]
		public TextAsset noise3DData;

		// Token: 0x040033DB RID: 13275
		public int noise3DSize = 64;

		// Token: 0x040033DC RID: 13276
		[HighlightNull]
		public ParticleSystem dustParticlesPrefab;

		// Token: 0x040033DD RID: 13277
		private static Config m_Instance;
	}
}
