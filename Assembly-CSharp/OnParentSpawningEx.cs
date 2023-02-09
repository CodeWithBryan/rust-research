using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004F9 RID: 1273
public static class OnParentSpawningEx
{
	// Token: 0x0600282B RID: 10283 RVA: 0x000F5CD0 File Offset: 0x000F3ED0
	public static void BroadcastOnParentSpawning(this GameObject go)
	{
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponentsInChildren<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
	}

	// Token: 0x0600282C RID: 10284 RVA: 0x000F5D10 File Offset: 0x000F3F10
	public static void SendOnParentSpawning(this GameObject go)
	{
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponents<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
	}
}
