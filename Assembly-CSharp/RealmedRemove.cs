using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000535 RID: 1333
public class RealmedRemove : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x060028BD RID: 10429 RVA: 0x000F7E74 File Offset: 0x000F6074
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside)
		{
			GameObject[] array = this.removedFromClient;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array[i], true);
			}
			Component[] array2 = this.removedComponentFromClient;
			for (int i = 0; i < array2.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array2[i], true);
			}
		}
		if (serverside)
		{
			GameObject[] array = this.removedFromServer;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array[i], true);
			}
			Component[] array2 = this.removedComponentFromServer;
			for (int i = 0; i < array2.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array2[i], true);
			}
		}
		if (!bundling)
		{
			process.RemoveComponent(this);
		}
	}

	// Token: 0x060028BE RID: 10430 RVA: 0x000F7F0C File Offset: 0x000F610C
	public bool ShouldDelete(Component comp, bool client, bool server)
	{
		return (!client || this.doNotRemoveFromClient == null || !this.doNotRemoveFromClient.Contains(comp)) && (!server || this.doNotRemoveFromServer == null || !this.doNotRemoveFromServer.Contains(comp));
	}

	// Token: 0x04002119 RID: 8473
	public GameObject[] removedFromClient;

	// Token: 0x0400211A RID: 8474
	public Component[] removedComponentFromClient;

	// Token: 0x0400211B RID: 8475
	public GameObject[] removedFromServer;

	// Token: 0x0400211C RID: 8476
	public Component[] removedComponentFromServer;

	// Token: 0x0400211D RID: 8477
	public Component[] doNotRemoveFromServer;

	// Token: 0x0400211E RID: 8478
	public Component[] doNotRemoveFromClient;
}
