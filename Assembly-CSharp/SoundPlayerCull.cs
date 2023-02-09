using System;
using UnityEngine;

// Token: 0x02000230 RID: 560
public class SoundPlayerCull : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x040013F1 RID: 5105
	public SoundPlayer soundPlayer;

	// Token: 0x040013F2 RID: 5106
	public float cullDistance = 100f;
}
