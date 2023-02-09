using System;

// Token: 0x020008C1 RID: 2241
public class AmbientLightLOD : FacepunchBehaviour, ILOD, IClientComponent
{
	// Token: 0x06003617 RID: 13847 RVA: 0x000C335A File Offset: 0x000C155A
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}

	// Token: 0x0400310D RID: 12557
	public bool isDynamic;

	// Token: 0x0400310E RID: 12558
	public float enabledRadius = 20f;

	// Token: 0x0400310F RID: 12559
	public bool toggleFade;

	// Token: 0x04003110 RID: 12560
	public float toggleFadeDuration = 0.5f;
}
