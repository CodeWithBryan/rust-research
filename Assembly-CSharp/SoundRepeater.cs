using System;
using UnityEngine;

// Token: 0x02000232 RID: 562
[RequireComponent(typeof(SoundPlayer))]
public class SoundRepeater : MonoBehaviour
{
	// Token: 0x040013F3 RID: 5107
	public float interval = 5f;

	// Token: 0x040013F4 RID: 5108
	public SoundPlayer player;
}
