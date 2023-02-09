using System;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class SoundFollowCollider : MonoBehaviour, IClientComponent
{
	// Token: 0x040013E2 RID: 5090
	public SoundDefinition soundDefinition;

	// Token: 0x040013E3 RID: 5091
	public Sound sound;

	// Token: 0x040013E4 RID: 5092
	public Bounds soundFollowBounds;

	// Token: 0x040013E5 RID: 5093
	public bool startImmediately;
}
