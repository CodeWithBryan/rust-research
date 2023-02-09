using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200092D RID: 2349
public class PrefabPool
{
	// Token: 0x17000439 RID: 1081
	// (get) Token: 0x060037F7 RID: 14327 RVA: 0x0014BA12 File Offset: 0x00149C12
	public int Count
	{
		get
		{
			return this.stack.Count;
		}
	}

	// Token: 0x060037F8 RID: 14328 RVA: 0x0014BA1F File Offset: 0x00149C1F
	public void Push(Poolable info)
	{
		this.stack.Push(info);
		info.EnterPool();
	}

	// Token: 0x060037F9 RID: 14329 RVA: 0x0014BA34 File Offset: 0x00149C34
	public void Push(GameObject instance)
	{
		Poolable component = instance.GetComponent<Poolable>();
		this.Push(component);
	}

	// Token: 0x060037FA RID: 14330 RVA: 0x0014BA50 File Offset: 0x00149C50
	public GameObject Pop(Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
	{
		while (this.stack.Count > 0)
		{
			Poolable poolable = this.stack.Pop();
			if (poolable)
			{
				poolable.transform.position = pos;
				poolable.transform.rotation = rot;
				poolable.LeavePool();
				return poolable.gameObject;
			}
		}
		return null;
	}

	// Token: 0x060037FB RID: 14331 RVA: 0x0014BAA8 File Offset: 0x00149CA8
	public void Clear()
	{
		foreach (Poolable poolable in this.stack)
		{
			if (poolable)
			{
				UnityEngine.Object.Destroy(poolable.gameObject);
			}
		}
		this.stack.Clear();
	}

	// Token: 0x04003219 RID: 12825
	public Stack<Poolable> stack = new Stack<Poolable>();
}
