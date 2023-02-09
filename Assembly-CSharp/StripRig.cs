using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000550 RID: 1360
public class StripRig : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06002975 RID: 10613 RVA: 0x000FBBE4 File Offset: 0x000F9DE4
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (this.root && ((serverside && this.fromServer) || (clientside && this.fromClient)))
		{
			SkinnedMeshRenderer component = base.GetComponent<SkinnedMeshRenderer>();
			this.Strip(preProcess, component);
		}
		preProcess.RemoveComponent(this);
	}

	// Token: 0x06002976 RID: 10614 RVA: 0x000FBC2C File Offset: 0x000F9E2C
	public void Strip(IPrefabProcessor preProcess, SkinnedMeshRenderer skinnedMeshRenderer)
	{
		List<Transform> list = Pool.GetList<Transform>();
		this.root.GetComponentsInChildren<Transform>(list);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (preProcess != null)
			{
				preProcess.NominateForDeletion(list[i].gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(list[i].gameObject);
			}
		}
		Pool.FreeList<Transform>(ref list);
	}

	// Token: 0x0400219B RID: 8603
	public Transform root;

	// Token: 0x0400219C RID: 8604
	public bool fromClient;

	// Token: 0x0400219D RID: 8605
	public bool fromServer;
}
