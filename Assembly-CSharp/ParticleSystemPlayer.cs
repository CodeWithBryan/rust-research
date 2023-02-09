using System;
using UnityEngine;

// Token: 0x020008DB RID: 2267
public class ParticleSystemPlayer : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x0600365F RID: 13919 RVA: 0x00144034 File Offset: 0x00142234
	protected void OnEnable()
	{
		base.GetComponent<ParticleSystem>().enableEmission = true;
	}

	// Token: 0x06003660 RID: 13920 RVA: 0x00144042 File Offset: 0x00142242
	public void OnParentDestroying()
	{
		base.GetComponent<ParticleSystem>().enableEmission = false;
	}
}
