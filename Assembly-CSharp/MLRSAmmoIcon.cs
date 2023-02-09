using System;
using UnityEngine;

// Token: 0x0200045F RID: 1119
public class MLRSAmmoIcon : MonoBehaviour
{
	// Token: 0x060024C0 RID: 9408 RVA: 0x000E7FE4 File Offset: 0x000E61E4
	protected void Awake()
	{
		this.SetState(false);
	}

	// Token: 0x060024C1 RID: 9409 RVA: 0x000E7FED File Offset: 0x000E61ED
	public void SetState(bool filled)
	{
		this.fill.SetActive(filled);
	}

	// Token: 0x04001D49 RID: 7497
	[SerializeField]
	private GameObject fill;
}
