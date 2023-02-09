using System;
using ConVar;
using UnityEngine;

// Token: 0x02000435 RID: 1077
public class BushEntity : BaseEntity, IPrefabPreProcess
{
	// Token: 0x06002374 RID: 9076 RVA: 0x000E0DA0 File Offset: 0x000DEFA0
	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			DecorComponent[] components = PrefabAttribute.server.FindAll<DecorComponent>(this.prefabID);
			base.transform.ApplyDecorComponentsScaleOnly(components);
		}
	}

	// Token: 0x06002375 RID: 9077 RVA: 0x000E0DD8 File Offset: 0x000DEFD8
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.globalBillboard)
		{
			TreeManager.OnTreeSpawned(this);
		}
	}

	// Token: 0x06002376 RID: 9078 RVA: 0x000E0DEE File Offset: 0x000DEFEE
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.globalBillboard)
		{
			TreeManager.OnTreeDestroyed(this);
		}
	}

	// Token: 0x06002377 RID: 9079 RVA: 0x0009A237 File Offset: 0x00098437
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			this.globalBroadcast = ConVar.Tree.global_broadcast;
		}
	}

	// Token: 0x04001C2D RID: 7213
	public GameObjectRef prefab;

	// Token: 0x04001C2E RID: 7214
	public bool globalBillboard = true;
}
