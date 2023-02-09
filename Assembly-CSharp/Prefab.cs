using System;
using UnityEngine;

// Token: 0x02000527 RID: 1319
public class Prefab<T> : Prefab, IComparable<Prefab<T>> where T : Component
{
	// Token: 0x06002875 RID: 10357 RVA: 0x000F67B0 File Offset: 0x000F49B0
	public Prefab(string name, GameObject prefab, T component, GameManager manager, PrefabAttribute.Library attribute) : base(name, prefab, manager, attribute)
	{
		this.Component = component;
	}

	// Token: 0x06002876 RID: 10358 RVA: 0x000F67C5 File Offset: 0x000F49C5
	public int CompareTo(Prefab<T> that)
	{
		return base.CompareTo(that);
	}

	// Token: 0x040020EE RID: 8430
	public T Component;
}
