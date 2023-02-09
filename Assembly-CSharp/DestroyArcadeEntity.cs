using System;
using UnityEngine;

// Token: 0x02000145 RID: 325
public class DestroyArcadeEntity : BaseMonoBehaviour
{
	// Token: 0x06001622 RID: 5666 RVA: 0x000A8F22 File Offset: 0x000A7122
	private void Start()
	{
		base.Invoke(new Action(this.DestroyAction), this.TimeToDie + UnityEngine.Random.Range(this.TimeToDieVariance * -0.5f, this.TimeToDieVariance * 0.5f));
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x000A8F5A File Offset: 0x000A715A
	private void DestroyAction()
	{
		if (this.ent != null & this.ent.host)
		{
			UnityEngine.Object.Destroy(this.ent.gameObject);
		}
	}

	// Token: 0x04000EFA RID: 3834
	public ArcadeEntity ent;

	// Token: 0x04000EFB RID: 3835
	public float TimeToDie = 1f;

	// Token: 0x04000EFC RID: 3836
	public float TimeToDieVariance;
}
