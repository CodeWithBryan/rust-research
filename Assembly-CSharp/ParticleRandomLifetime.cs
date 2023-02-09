using System;
using UnityEngine;

// Token: 0x020002B8 RID: 696
public class ParticleRandomLifetime : MonoBehaviour
{
	// Token: 0x06001C73 RID: 7283 RVA: 0x000C3D1C File Offset: 0x000C1F1C
	public void Awake()
	{
		if (!this.mySystem)
		{
			return;
		}
		float startLifetime = UnityEngine.Random.Range(this.minScale, this.maxScale);
		this.mySystem.startLifetime = startLifetime;
	}

	// Token: 0x040015E8 RID: 5608
	public ParticleSystem mySystem;

	// Token: 0x040015E9 RID: 5609
	public float minScale = 0.5f;

	// Token: 0x040015EA RID: 5610
	public float maxScale = 1f;
}
