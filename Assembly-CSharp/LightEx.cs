using System;
using UnityEngine;

// Token: 0x020002AA RID: 682
public class LightEx : UpdateBehaviour, IClientComponent
{
	// Token: 0x06001C3F RID: 7231 RVA: 0x000C335A File Offset: 0x000C155A
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}

	// Token: 0x06001C40 RID: 7232 RVA: 0x00007074 File Offset: 0x00005274
	public static bool CheckConflict(GameObject go)
	{
		return false;
	}

	// Token: 0x04001585 RID: 5509
	public bool alterColor;

	// Token: 0x04001586 RID: 5510
	public float colorTimeScale = 1f;

	// Token: 0x04001587 RID: 5511
	public Color colorA = Color.red;

	// Token: 0x04001588 RID: 5512
	public Color colorB = Color.yellow;

	// Token: 0x04001589 RID: 5513
	public AnimationCurve blendCurve = new AnimationCurve();

	// Token: 0x0400158A RID: 5514
	public bool loopColor = true;

	// Token: 0x0400158B RID: 5515
	public bool alterIntensity;

	// Token: 0x0400158C RID: 5516
	public float intensityTimeScale = 1f;

	// Token: 0x0400158D RID: 5517
	public AnimationCurve intenseCurve = new AnimationCurve();

	// Token: 0x0400158E RID: 5518
	public float intensityCurveScale = 3f;

	// Token: 0x0400158F RID: 5519
	public bool loopIntensity = true;

	// Token: 0x04001590 RID: 5520
	public bool randomOffset;

	// Token: 0x04001591 RID: 5521
	public float randomIntensityStartScale = -1f;
}
