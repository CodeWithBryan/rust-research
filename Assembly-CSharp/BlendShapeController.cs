using System;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class BlendShapeController : MonoBehaviour
{
	// Token: 0x040014DF RID: 5343
	public SkinnedMeshRenderer TargetRenderer;

	// Token: 0x040014E0 RID: 5344
	public BlendShapeController.BlendState[] States;

	// Token: 0x040014E1 RID: 5345
	public float LerpSpeed = 0.25f;

	// Token: 0x040014E2 RID: 5346
	public BlendShapeController.BlendMode CurrentMode;

	// Token: 0x02000C3B RID: 3131
	public enum BlendMode
	{
		// Token: 0x0400416D RID: 16749
		Idle,
		// Token: 0x0400416E RID: 16750
		Happy,
		// Token: 0x0400416F RID: 16751
		Angry
	}

	// Token: 0x02000C3C RID: 3132
	[Serializable]
	public struct BlendState
	{
		// Token: 0x04004170 RID: 16752
		[Range(0f, 100f)]
		public float[] States;

		// Token: 0x04004171 RID: 16753
		public BlendShapeController.BlendMode Mode;
	}
}
