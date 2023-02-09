using System;
using Rust;
using Rust.Registry;
using UnityEngine;

// Token: 0x02000398 RID: 920
public class BaseEntityChild : MonoBehaviour
{
	// Token: 0x06001FDB RID: 8155 RVA: 0x000D1930 File Offset: 0x000CFB30
	public static void Setup(GameObject obj, BaseEntity parent)
	{
		using (TimeWarning.New("Registry.Entity.Register", 0))
		{
			Entity.Register(obj, parent);
		}
	}

	// Token: 0x06001FDC RID: 8156 RVA: 0x000D196C File Offset: 0x000CFB6C
	public void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		using (TimeWarning.New("Registry.Entity.Unregister", 0))
		{
			Entity.Unregister(base.gameObject);
		}
	}
}
