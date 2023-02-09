using System;
using UnityEngine.Serialization;

// Token: 0x020004D7 RID: 1239
public class EffectRecycle : BaseMonoBehaviour, IClientComponent, IRagdollInhert, IEffectRecycle
{
	// Token: 0x04001FCF RID: 8143
	[FormerlySerializedAs("lifeTime")]
	[ReadOnly]
	public float detachTime;

	// Token: 0x04001FD0 RID: 8144
	[FormerlySerializedAs("lifeTime")]
	[ReadOnly]
	public float recycleTime;

	// Token: 0x04001FD1 RID: 8145
	public EffectRecycle.PlayMode playMode;

	// Token: 0x04001FD2 RID: 8146
	public EffectRecycle.ParentDestroyBehaviour onParentDestroyed;

	// Token: 0x02000CD9 RID: 3289
	public enum PlayMode
	{
		// Token: 0x04004402 RID: 17410
		Once,
		// Token: 0x04004403 RID: 17411
		Looped
	}

	// Token: 0x02000CDA RID: 3290
	public enum ParentDestroyBehaviour
	{
		// Token: 0x04004405 RID: 17413
		Detach,
		// Token: 0x04004406 RID: 17414
		Destroy,
		// Token: 0x04004407 RID: 17415
		DetachWaitDestroy
	}
}
