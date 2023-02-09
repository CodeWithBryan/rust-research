using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000406 RID: 1030
public class VisualStorageContainer : LootContainer
{
	// Token: 0x0600229C RID: 8860 RVA: 0x000DD774 File Offset: 0x000DB974
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x0009D19D File Offset: 0x0009B39D
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x000DD77C File Offset: 0x000DB97C
	public override void PopulateLoot()
	{
		base.PopulateLoot();
		for (int i = 0; i < this.inventorySlots; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				DroppedItem component = slot.Drop(this.displayNodes[i].transform.position + new Vector3(0f, 0.25f, 0f), Vector3.zero, this.displayNodes[i].transform.rotation).GetComponent<DroppedItem>();
				if (component)
				{
					base.ReceiveCollisionMessages(false);
					base.CancelInvoke(new Action(component.IdleDestroy));
					Rigidbody componentInChildren = component.GetComponentInChildren<Rigidbody>();
					if (componentInChildren)
					{
						componentInChildren.constraints = (RigidbodyConstraints)10;
					}
				}
			}
		}
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x000DD840 File Offset: 0x000DBA40
	public void ClearRigidBodies()
	{
		if (this.displayModels == null)
		{
			return;
		}
		foreach (VisualStorageContainer.DisplayModel displayModel in this.displayModels)
		{
			if (displayModel != null)
			{
				UnityEngine.Object.Destroy(displayModel.displayModel.GetComponentInChildren<Rigidbody>());
			}
		}
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x000DD884 File Offset: 0x000DBA84
	public void SetItemsVisible(bool vis)
	{
		if (this.displayModels == null)
		{
			return;
		}
		foreach (VisualStorageContainer.DisplayModel displayModel in this.displayModels)
		{
			if (displayModel != null)
			{
				LODGroup componentInChildren = displayModel.displayModel.GetComponentInChildren<LODGroup>();
				if (componentInChildren)
				{
					componentInChildren.localReferencePoint = (vis ? Vector3.zero : new Vector3(10000f, 10000f, 10000f));
				}
				else
				{
					Debug.Log("VisualStorageContainer item missing LODGroup" + displayModel.displayModel.gameObject.name);
				}
			}
		}
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000DD90F File Offset: 0x000DBB0F
	public void ItemUpdateComplete()
	{
		this.ClearRigidBodies();
		this.SetItemsVisible(true);
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x000DD920 File Offset: 0x000DBB20
	public void UpdateVisibleItems(ProtoBuf.ItemContainer msg)
	{
		for (int i = 0; i < this.displayModels.Length; i++)
		{
			VisualStorageContainer.DisplayModel displayModel = this.displayModels[i];
			if (displayModel != null)
			{
				UnityEngine.Object.Destroy(displayModel.displayModel);
				this.displayModels[i] = null;
			}
		}
		if (msg == null)
		{
			return;
		}
		foreach (ProtoBuf.Item item in msg.contents)
		{
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(item.itemid);
			GameObject gameObject;
			if (itemDefinition.worldModelPrefab != null && itemDefinition.worldModelPrefab.isValid)
			{
				gameObject = itemDefinition.worldModelPrefab.Instantiate(null);
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(this.defaultDisplayModel);
			}
			if (gameObject)
			{
				gameObject.transform.SetPositionAndRotation(this.displayNodes[item.slot].transform.position + new Vector3(0f, 0.25f, 0f), this.displayNodes[item.slot].transform.rotation);
				Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
				rigidbody.mass = 1f;
				rigidbody.drag = 0.1f;
				rigidbody.angularDrag = 0.1f;
				rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
				rigidbody.constraints = (RigidbodyConstraints)10;
				this.displayModels[item.slot].displayModel = gameObject;
				this.displayModels[item.slot].slot = item.slot;
				this.displayModels[item.slot].def = itemDefinition;
				gameObject.SetActive(true);
			}
		}
		this.SetItemsVisible(false);
		base.CancelInvoke(new Action(this.ItemUpdateComplete));
		base.Invoke(new Action(this.ItemUpdateComplete), 1f);
	}

	// Token: 0x04001B14 RID: 6932
	public VisualStorageContainerNode[] displayNodes;

	// Token: 0x04001B15 RID: 6933
	public VisualStorageContainer.DisplayModel[] displayModels;

	// Token: 0x04001B16 RID: 6934
	public Transform nodeParent;

	// Token: 0x04001B17 RID: 6935
	public GameObject defaultDisplayModel;

	// Token: 0x02000C8B RID: 3211
	[Serializable]
	public class DisplayModel
	{
		// Token: 0x040042D3 RID: 17107
		public GameObject displayModel;

		// Token: 0x040042D4 RID: 17108
		public ItemDefinition def;

		// Token: 0x040042D5 RID: 17109
		public int slot;
	}
}
