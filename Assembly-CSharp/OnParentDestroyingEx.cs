using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004F7 RID: 1271
public static class OnParentDestroyingEx
{
	// Token: 0x06002828 RID: 10280 RVA: 0x000F5C50 File Offset: 0x000F3E50
	public static void BroadcastOnParentDestroying(this GameObject go)
	{
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponentsInChildren<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
	}

	// Token: 0x06002829 RID: 10281 RVA: 0x000F5C90 File Offset: 0x000F3E90
	public static void SendOnParentDestroying(this GameObject go)
	{
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponents<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
	}
}
