using System;
using UnityEngine;

// Token: 0x020002D5 RID: 725
public class UnparentOnDestroy : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x06001CD5 RID: 7381 RVA: 0x000C58DA File Offset: 0x000C3ADA
	public void OnParentDestroying()
	{
		base.transform.parent = null;
		GameManager.Destroy(base.gameObject, (this.destroyAfterSeconds <= 0f) ? 1f : this.destroyAfterSeconds);
	}

	// Token: 0x06001CD6 RID: 7382 RVA: 0x000C590D File Offset: 0x000C3B0D
	protected void OnValidate()
	{
		if (this.destroyAfterSeconds <= 0f)
		{
			this.destroyAfterSeconds = 1f;
		}
	}

	// Token: 0x04001687 RID: 5767
	public float destroyAfterSeconds = 1f;
}
