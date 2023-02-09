using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A52 RID: 2642
	[ExecuteAlways]
	[AddComponentMenu("Rendering/Post-process Volume", 1001)]
	public sealed class PostProcessVolume : MonoBehaviour
	{
		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06003E76 RID: 15990 RVA: 0x0016EBBC File Offset: 0x0016CDBC
		// (set) Token: 0x06003E77 RID: 15991 RVA: 0x0016EC50 File Offset: 0x0016CE50
		public PostProcessProfile profile
		{
			get
			{
				if (this.m_InternalProfile == null)
				{
					this.m_InternalProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
					if (this.sharedProfile != null)
					{
						foreach (PostProcessEffectSettings original in this.sharedProfile.settings)
						{
							PostProcessEffectSettings item = Object.Instantiate<PostProcessEffectSettings>(original);
							this.m_InternalProfile.settings.Add(item);
						}
					}
				}
				return this.m_InternalProfile;
			}
			set
			{
				this.m_InternalProfile = value;
			}
		}

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06003E78 RID: 15992 RVA: 0x0016EC59 File Offset: 0x0016CE59
		internal PostProcessProfile profileRef
		{
			get
			{
				if (!(this.m_InternalProfile == null))
				{
					return this.m_InternalProfile;
				}
				return this.sharedProfile;
			}
		}

		// Token: 0x06003E79 RID: 15993 RVA: 0x0016EC76 File Offset: 0x0016CE76
		public bool HasInstantiatedProfile()
		{
			return this.m_InternalProfile != null;
		}

		// Token: 0x06003E7A RID: 15994 RVA: 0x0016EC84 File Offset: 0x0016CE84
		private void OnEnable()
		{
			PostProcessManager.instance.Register(this);
			this.m_PreviousLayer = base.gameObject.layer;
		}

		// Token: 0x06003E7B RID: 15995 RVA: 0x0016ECA2 File Offset: 0x0016CEA2
		private void OnDisable()
		{
			PostProcessManager.instance.Unregister(this);
		}

		// Token: 0x06003E7C RID: 15996 RVA: 0x0016ECB0 File Offset: 0x0016CEB0
		private void Update()
		{
			int layer = base.gameObject.layer;
			if (layer != this.m_PreviousLayer)
			{
				PostProcessManager.instance.UpdateVolumeLayer(this, this.m_PreviousLayer, layer);
				this.m_PreviousLayer = layer;
			}
			if (this.priority != this.m_PreviousPriority)
			{
				PostProcessManager.instance.SetLayerDirty(layer);
				this.m_PreviousPriority = this.priority;
			}
		}

		// Token: 0x06003E7D RID: 15997 RVA: 0x0016ED10 File Offset: 0x0016CF10
		private void OnDrawGizmos()
		{
			if (this.isGlobal)
			{
				return;
			}
			Vector3 lossyScale = base.transform.lossyScale;
			Vector3 a = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, lossyScale);
			Gizmos.DrawCube(this.bounds.center, this.bounds.size);
			Gizmos.DrawWireCube(this.bounds.center, this.bounds.size + a * this.blendDistance * 4f);
			Gizmos.matrix = Matrix4x4.identity;
		}

		// Token: 0x0400377F RID: 14207
		public PostProcessProfile sharedProfile;

		// Token: 0x04003780 RID: 14208
		[Tooltip("Check this box to mark this volume as global. This volume's Profile will be applied to the whole Scene.")]
		public bool isGlobal;

		// Token: 0x04003781 RID: 14209
		public Bounds bounds;

		// Token: 0x04003782 RID: 14210
		[Min(0f)]
		[Tooltip("The distance (from the attached Collider) to start blending from. A value of 0 means there will be no blending and the Volume overrides will be applied immediatly upon entry to the attached Collider.")]
		public float blendDistance;

		// Token: 0x04003783 RID: 14211
		[Range(0f, 1f)]
		[Tooltip("The total weight of this Volume in the Scene. A value of 0 signifies that it will have no effect, 1 signifies full effect.")]
		public float weight = 1f;

		// Token: 0x04003784 RID: 14212
		[Tooltip("The volume priority in the stack. A higher value means higher priority. Negative values are supported.")]
		public float priority;

		// Token: 0x04003785 RID: 14213
		private int m_PreviousLayer;

		// Token: 0x04003786 RID: 14214
		private float m_PreviousPriority;

		// Token: 0x04003787 RID: 14215
		private PostProcessProfile m_InternalProfile;
	}
}
