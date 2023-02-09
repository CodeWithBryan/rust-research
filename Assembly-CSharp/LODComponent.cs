using System;
using UnityEngine;

// Token: 0x0200050A RID: 1290
public abstract class LODComponent : BaseMonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x040020A5 RID: 8357
	public LODDistanceMode DistanceMode;

	// Token: 0x040020A6 RID: 8358
	public LODComponent.OccludeeParameters OccludeeParams = new LODComponent.OccludeeParameters
	{
		isDynamic = false,
		dynamicUpdateInterval = 0.2f,
		shadowRangeScale = 3f,
		showBounds = false
	};

	// Token: 0x02000CE5 RID: 3301
	[Serializable]
	public struct OccludeeParameters
	{
		// Token: 0x04004436 RID: 17462
		[Tooltip("Is Occludee dynamic or static?")]
		public bool isDynamic;

		// Token: 0x04004437 RID: 17463
		[Tooltip("Dynamic occludee update interval in seconds; 0 = every frame")]
		public float dynamicUpdateInterval;

		// Token: 0x04004438 RID: 17464
		[Tooltip("Distance scale combined with occludee max bounds size at which culled occludee shadows are still visible")]
		public float shadowRangeScale;

		// Token: 0x04004439 RID: 17465
		[Tooltip("Show culling bounds via gizmos; editor only")]
		public bool showBounds;
	}
}
