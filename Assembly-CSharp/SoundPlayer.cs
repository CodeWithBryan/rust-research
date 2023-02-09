using System;
using UnityEngine;

// Token: 0x0200022F RID: 559
public class SoundPlayer : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x040013EA RID: 5098
	public SoundDefinition soundDefinition;

	// Token: 0x040013EB RID: 5099
	public bool playImmediately = true;

	// Token: 0x040013EC RID: 5100
	public float minStartDelay;

	// Token: 0x040013ED RID: 5101
	public float maxStartDelay;

	// Token: 0x040013EE RID: 5102
	public bool debugRepeat;

	// Token: 0x040013EF RID: 5103
	public bool pending;

	// Token: 0x040013F0 RID: 5104
	public Vector3 soundOffset = Vector3.zero;
}
