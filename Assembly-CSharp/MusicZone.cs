using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021E RID: 542
public class MusicZone : MonoBehaviour, IClientComponent
{
	// Token: 0x04001383 RID: 4995
	public List<MusicTheme> themes;

	// Token: 0x04001384 RID: 4996
	public float priority;

	// Token: 0x04001385 RID: 4997
	public bool suppressAutomaticMusic;
}
