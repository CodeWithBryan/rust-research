using System;
using UnityEngine;

// Token: 0x020008C2 RID: 2242
public class BasePrefab : BaseMonoBehaviour, IPrefabPreProcess
{
	// Token: 0x17000412 RID: 1042
	// (get) Token: 0x06003619 RID: 13849 RVA: 0x001432D5 File Offset: 0x001414D5
	public bool isServer
	{
		get
		{
			return !this.isClient;
		}
	}

	// Token: 0x0600361A RID: 13850 RVA: 0x001432E0 File Offset: 0x001414E0
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.prefabID = StringPool.Get(name);
		this.isClient = clientside;
	}

	// Token: 0x04003111 RID: 12561
	[HideInInspector]
	public uint prefabID;

	// Token: 0x04003112 RID: 12562
	[HideInInspector]
	public bool isClient;
}
