using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200092E RID: 2350
public class PrefabPoolCollection
{
	// Token: 0x060037FD RID: 14333 RVA: 0x0014BB28 File Offset: 0x00149D28
	public void Push(GameObject instance)
	{
		Poolable component = instance.GetComponent<Poolable>();
		PrefabPool prefabPool;
		if (!this.storage.TryGetValue(component.prefabID, out prefabPool))
		{
			prefabPool = new PrefabPool();
			this.storage.Add(component.prefabID, prefabPool);
		}
		prefabPool.Push(component);
	}

	// Token: 0x060037FE RID: 14334 RVA: 0x0014BB70 File Offset: 0x00149D70
	public GameObject Pop(uint id, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
	{
		PrefabPool prefabPool;
		if (this.storage.TryGetValue(id, out prefabPool))
		{
			return prefabPool.Pop(pos, rot);
		}
		return null;
	}

	// Token: 0x060037FF RID: 14335 RVA: 0x0014BB98 File Offset: 0x00149D98
	public void Clear()
	{
		foreach (KeyValuePair<uint, PrefabPool> keyValuePair in this.storage)
		{
			keyValuePair.Value.Clear();
		}
		this.storage.Clear();
	}

	// Token: 0x0400321A RID: 12826
	public Dictionary<uint, PrefabPool> storage = new Dictionary<uint, PrefabPool>();
}
