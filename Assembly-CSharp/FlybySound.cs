using System;
using UnityEngine;

// Token: 0x02000212 RID: 530
public class FlybySound : MonoBehaviour, IClientComponent
{
	// Token: 0x04001320 RID: 4896
	public SoundDefinition flybySound;

	// Token: 0x04001321 RID: 4897
	public float flybySoundDistance = 7f;

	// Token: 0x04001322 RID: 4898
	public SoundDefinition closeFlybySound;

	// Token: 0x04001323 RID: 4899
	public float closeFlybyDistance = 3f;
}
