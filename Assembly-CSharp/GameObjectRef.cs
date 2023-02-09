using System;
using Facepunch;
using UnityEngine;

// Token: 0x02000932 RID: 2354
[Serializable]
public class GameObjectRef : ResourceRef<GameObject>
{
	// Token: 0x0600380F RID: 14351 RVA: 0x0014BDDC File Offset: 0x00149FDC
	public GameObject Instantiate(Transform parent = null)
	{
		return Facepunch.Instantiate.GameObject(base.Get(), parent);
	}

	// Token: 0x06003810 RID: 14352 RVA: 0x0014BDEA File Offset: 0x00149FEA
	public BaseEntity GetEntity()
	{
		return base.Get().GetComponent<BaseEntity>();
	}
}
