using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x02000986 RID: 2438
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-triggerzone/")]
	public class TriggerZone : MonoBehaviour
	{
		// Token: 0x060039A0 RID: 14752 RVA: 0x00154B80 File Offset: 0x00152D80
		private void Update()
		{
			VolumetricLightBeam component = base.GetComponent<VolumetricLightBeam>();
			if (component)
			{
				MeshCollider orAddComponent = base.gameObject.GetOrAddComponent<MeshCollider>();
				Debug.Assert(orAddComponent);
				float lengthZ = component.fadeEnd * this.rangeMultiplier;
				float radiusEnd = Mathf.LerpUnclamped(component.coneRadiusStart, component.coneRadiusEnd, this.rangeMultiplier);
				this.m_Mesh = MeshGenerator.GenerateConeZ_Radius(lengthZ, component.coneRadiusStart, radiusEnd, 8, 0, false);
				this.m_Mesh.hideFlags = Consts.ProceduralObjectsHideFlags;
				orAddComponent.sharedMesh = this.m_Mesh;
				if (this.setIsTrigger)
				{
					orAddComponent.convex = true;
					orAddComponent.isTrigger = true;
				}
				UnityEngine.Object.Destroy(this);
			}
		}

		// Token: 0x04003442 RID: 13378
		public bool setIsTrigger = true;

		// Token: 0x04003443 RID: 13379
		public float rangeMultiplier = 1f;

		// Token: 0x04003444 RID: 13380
		private const int kMeshColliderNumSides = 8;

		// Token: 0x04003445 RID: 13381
		private Mesh m_Mesh;
	}
}
