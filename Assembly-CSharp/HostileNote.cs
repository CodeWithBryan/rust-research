using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007BA RID: 1978
public class HostileNote : MonoBehaviour, IClientComponent
{
	// Token: 0x04002BD5 RID: 11221
	public CanvasGroup warnGroup;

	// Token: 0x04002BD6 RID: 11222
	public CanvasGroup group;

	// Token: 0x04002BD7 RID: 11223
	public CanvasGroup timerGroup;

	// Token: 0x04002BD8 RID: 11224
	public CanvasGroup smallWarning;

	// Token: 0x04002BD9 RID: 11225
	public Text timerText;

	// Token: 0x04002BDA RID: 11226
	public Text smallWarningText;

	// Token: 0x04002BDB RID: 11227
	public static float unhostileTime;

	// Token: 0x04002BDC RID: 11228
	public static float weaponDrawnDuration;

	// Token: 0x04002BDD RID: 11229
	public Color warnColor;

	// Token: 0x04002BDE RID: 11230
	public Color hostileColor;

	// Token: 0x04002BDF RID: 11231
	public float requireDistanceToSafeZone = 200f;
}
