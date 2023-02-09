using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004FD RID: 1277
public static class OnSendNetworkUpdateEx
{
	// Token: 0x06002831 RID: 10289 RVA: 0x000F5DD0 File Offset: 0x000F3FD0
	public static void BroadcastOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnSendNetworkUpdate> list = Pool.GetList<IOnSendNetworkUpdate>();
		go.GetComponentsInChildren<IOnSendNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnSendNetworkUpdate(entity);
		}
		Pool.FreeList<IOnSendNetworkUpdate>(ref list);
	}

	// Token: 0x06002832 RID: 10290 RVA: 0x000F5E10 File Offset: 0x000F4010
	public static void SendOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnSendNetworkUpdate> list = Pool.GetList<IOnSendNetworkUpdate>();
		go.GetComponents<IOnSendNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnSendNetworkUpdate(entity);
		}
		Pool.FreeList<IOnSendNetworkUpdate>(ref list);
	}
}
