using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007D6 RID: 2006
public class UIBuffs : SingletonComponent<UIBuffs>
{
	// Token: 0x06003412 RID: 13330 RVA: 0x0013D300 File Offset: 0x0013B500
	public void Refresh(PlayerModifiers modifiers)
	{
		if (!this.Enabled)
		{
			return;
		}
		this.RemoveAll();
		if (modifiers == null)
		{
			return;
		}
		using (List<Modifier>.Enumerator enumerator = modifiers.All.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				{
					UnityEngine.Object.Instantiate<Transform>(this.PrefabBuffIcon).SetParent(base.transform);
				}
			}
		}
	}

	// Token: 0x06003413 RID: 13331 RVA: 0x0013D37C File Offset: 0x0013B57C
	private void RemoveAll()
	{
		foreach (object obj in base.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
	}

	// Token: 0x04002C81 RID: 11393
	public bool Enabled = true;

	// Token: 0x04002C82 RID: 11394
	public Transform PrefabBuffIcon;
}
