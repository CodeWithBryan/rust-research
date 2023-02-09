using System;
using UnityEngine;

// Token: 0x0200044C RID: 1100
public class BuoyancyPoint : MonoBehaviour
{
	// Token: 0x06002417 RID: 9239 RVA: 0x000E3E3C File Offset: 0x000E203C
	public void Start()
	{
		this.randomOffset = UnityEngine.Random.Range(0f, 20f);
	}

	// Token: 0x06002418 RID: 9240 RVA: 0x000E3E53 File Offset: 0x000E2053
	public void OnDrawGizmos()
	{
		Gizmos.color = BuoyancyPoint.gizmoColour;
		Gizmos.DrawSphere(base.transform.position, this.size * 0.5f);
	}

	// Token: 0x04001CA9 RID: 7337
	public float buoyancyForce = 10f;

	// Token: 0x04001CAA RID: 7338
	public float size = 0.1f;

	// Token: 0x04001CAB RID: 7339
	public float waveScale = 0.2f;

	// Token: 0x04001CAC RID: 7340
	public float waveFrequency = 1f;

	// Token: 0x04001CAD RID: 7341
	public bool doSplashEffects = true;

	// Token: 0x04001CAE RID: 7342
	[NonSerialized]
	public float randomOffset;

	// Token: 0x04001CAF RID: 7343
	[NonSerialized]
	public bool wasSubmergedLastFrame;

	// Token: 0x04001CB0 RID: 7344
	[NonSerialized]
	public float nexSplashTime;

	// Token: 0x04001CB1 RID: 7345
	private static readonly Color gizmoColour = new Color(1f, 0f, 0f, 0.25f);
}
