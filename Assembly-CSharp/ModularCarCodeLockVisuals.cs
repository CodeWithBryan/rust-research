using System;
using UnityEngine;

// Token: 0x02000475 RID: 1141
[Serializable]
public class ModularCarCodeLockVisuals : MonoBehaviour
{
	// Token: 0x04001DD9 RID: 7641
	[SerializeField]
	private GameObject lockedVisuals;

	// Token: 0x04001DDA RID: 7642
	[SerializeField]
	private GameObject unlockedVisuals;

	// Token: 0x04001DDB RID: 7643
	[SerializeField]
	private GameObject blockedVisuals;

	// Token: 0x04001DDC RID: 7644
	[SerializeField]
	private GameObjectRef codelockEffectDenied;

	// Token: 0x04001DDD RID: 7645
	[SerializeField]
	private GameObjectRef codelockEffectShock;

	// Token: 0x04001DDE RID: 7646
	[SerializeField]
	private float xOffset = 0.91f;

	// Token: 0x04001DDF RID: 7647
	[SerializeField]
	private ParticleSystemContainer keycodeDestroyableFX;
}
