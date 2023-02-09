using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004FB RID: 1275
public static class OnPostNetworkUpdateEx
{
	// Token: 0x0600282E RID: 10286 RVA: 0x000F5D50 File Offset: 0x000F3F50
	public static void BroadcastOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnPostNetworkUpdate> list = Pool.GetList<IOnPostNetworkUpdate>();
		go.GetComponentsInChildren<IOnPostNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnPostNetworkUpdate(entity);
		}
		Pool.FreeList<IOnPostNetworkUpdate>(ref list);
	}

	// Token: 0x0600282F RID: 10287 RVA: 0x000F5D90 File Offset: 0x000F3F90
	public static void SendOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnPostNetworkUpdate> list = Pool.GetList<IOnPostNetworkUpdate>();
		go.GetComponents<IOnPostNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnPostNetworkUpdate(entity);
		}
		Pool.FreeList<IOnPostNetworkUpdate>(ref list);
	}
}
