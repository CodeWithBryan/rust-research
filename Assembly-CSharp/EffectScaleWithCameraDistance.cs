using System;
using UnityEngine;

// Token: 0x0200031D RID: 797
public class EffectScaleWithCameraDistance : MonoBehaviour, IEffect
{
	// Token: 0x0400174A RID: 5962
	public float minScale = 1f;

	// Token: 0x0400174B RID: 5963
	public float maxScale = 2.5f;

	// Token: 0x0400174C RID: 5964
	public float scaleStartDistance = 50f;

	// Token: 0x0400174D RID: 5965
	public float scaleEndDistance = 150f;
}
