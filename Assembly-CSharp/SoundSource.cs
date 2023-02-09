using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class SoundSource : MonoBehaviour, IClientComponentEx, ILOD
{
	// Token: 0x06001AF0 RID: 6896 RVA: 0x000BCAAA File Offset: 0x000BACAA
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this);
	}

	// Token: 0x06001AF1 RID: 6897 RVA: 0x00007074 File Offset: 0x00005274
	public bool IsSyncedToParent()
	{
		return false;
	}

	// Token: 0x040013F5 RID: 5109
	[Header("Occlusion")]
	public bool handleOcclusionChecks;

	// Token: 0x040013F6 RID: 5110
	public LayerMask occlusionLayerMask;

	// Token: 0x040013F7 RID: 5111
	public List<SoundSource.OcclusionPoint> occlusionPoints = new List<SoundSource.OcclusionPoint>();

	// Token: 0x040013F8 RID: 5112
	public bool isOccluded;

	// Token: 0x040013F9 RID: 5113
	public float occlusionAmount;

	// Token: 0x040013FA RID: 5114
	public float lodDistance = 100f;

	// Token: 0x040013FB RID: 5115
	public bool inRange;

	// Token: 0x02000C2F RID: 3119
	[Serializable]
	public class OcclusionPoint
	{
		// Token: 0x04004124 RID: 16676
		public Vector3 offset = Vector3.zero;

		// Token: 0x04004125 RID: 16677
		public bool isOccluded;
	}
}
