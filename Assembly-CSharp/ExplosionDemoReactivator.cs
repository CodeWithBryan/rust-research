using System;
using UnityEngine;

// Token: 0x0200095C RID: 2396
public class ExplosionDemoReactivator : MonoBehaviour
{
	// Token: 0x06003890 RID: 14480 RVA: 0x0014DF2C File Offset: 0x0014C12C
	private void Start()
	{
		base.InvokeRepeating("Reactivate", 0f, this.TimeDelayToReactivate);
	}

	// Token: 0x06003891 RID: 14481 RVA: 0x0014DF44 File Offset: 0x0014C144
	private void Reactivate()
	{
		foreach (Transform transform in base.GetComponentsInChildren<Transform>())
		{
			transform.gameObject.SetActive(false);
			transform.gameObject.SetActive(true);
		}
	}

	// Token: 0x040032DD RID: 13021
	public float TimeDelayToReactivate = 3f;
}
