using System;
using UnityEngine;

// Token: 0x0200092C RID: 2348
public static class PoolableEx
{
	// Token: 0x060037F4 RID: 14324 RVA: 0x0014B9A4 File Offset: 0x00149BA4
	public static bool SupportsPoolingInParent(this GameObject gameObject)
	{
		Poolable componentInParent = gameObject.GetComponentInParent<Poolable>();
		return componentInParent != null && componentInParent.prefabID > 0U;
	}

	// Token: 0x060037F5 RID: 14325 RVA: 0x0014B9CC File Offset: 0x00149BCC
	public static bool SupportsPooling(this GameObject gameObject)
	{
		Poolable component = gameObject.GetComponent<Poolable>();
		return component != null && component.prefabID > 0U;
	}

	// Token: 0x060037F6 RID: 14326 RVA: 0x0014B9F4 File Offset: 0x00149BF4
	public static void AwakeFromInstantiate(this GameObject gameObject)
	{
		if (gameObject.activeSelf)
		{
			gameObject.GetComponent<Poolable>().SetBehaviourEnabled(true);
			return;
		}
		gameObject.SetActive(true);
	}
}
