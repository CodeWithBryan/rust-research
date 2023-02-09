using System;
using UnityEngine;

// Token: 0x020002B7 RID: 695
public class ParticleDisableOnParentDestroy : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x06001C71 RID: 7281 RVA: 0x000C3CE4 File Offset: 0x000C1EE4
	public void OnParentDestroying()
	{
		base.transform.parent = null;
		base.GetComponent<ParticleSystem>().enableEmission = false;
		if (this.destroyAfterSeconds > 0f)
		{
			GameManager.Destroy(base.gameObject, this.destroyAfterSeconds);
		}
	}

	// Token: 0x040015E7 RID: 5607
	public float destroyAfterSeconds;
}
