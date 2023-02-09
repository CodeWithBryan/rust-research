using System;
using UnityEngine;

// Token: 0x020004B2 RID: 1202
public class IndustrialStorageAdaptor : IndustrialEntity, IIndustrialStorage
{
	// Token: 0x17000316 RID: 790
	// (get) Token: 0x060026BC RID: 9916 RVA: 0x000EFAF6 File Offset: 0x000EDCF6
	public ItemContainer Container
	{
		get
		{
			StorageContainer storageContainer = base.GetParentEntity() as StorageContainer;
			if (storageContainer == null)
			{
				return null;
			}
			return storageContainer.inventory;
		}
	}

	// Token: 0x060026BD RID: 9917 RVA: 0x000EFB10 File Offset: 0x000EDD10
	public Vector2i InputSlotRange(int slotIndex)
	{
		IIndustrialStorage industrialStorage;
		if ((industrialStorage = (base.GetParentEntity() as IIndustrialStorage)) != null)
		{
			return industrialStorage.InputSlotRange(slotIndex);
		}
		Locker locker;
		if ((locker = (base.GetParentEntity() as Locker)) != null)
		{
			Vector3 localPosition = base.transform.localPosition;
			return locker.GetIndustrialSlotRange(localPosition);
		}
		return new Vector2i(0, this.Container.capacity - 1);
	}

	// Token: 0x060026BE RID: 9918 RVA: 0x000EFB6C File Offset: 0x000EDD6C
	public Vector2i OutputSlotRange(int slotIndex)
	{
		if (base.GetParentEntity() is DropBox)
		{
			return new Vector2i(0, this.Container.capacity - 2);
		}
		IIndustrialStorage industrialStorage;
		if ((industrialStorage = (base.GetParentEntity() as IIndustrialStorage)) != null)
		{
			return industrialStorage.OutputSlotRange(slotIndex);
		}
		Locker locker;
		if ((locker = (base.GetParentEntity() as Locker)) != null)
		{
			Vector3 localPosition = base.transform.localPosition;
			return locker.GetIndustrialSlotRange(localPosition);
		}
		return new Vector2i(0, this.Container.capacity - 1);
	}

	// Token: 0x060026BF RID: 9919 RVA: 0x000EFBE8 File Offset: 0x000EDDE8
	public void OnStorageItemTransferBegin()
	{
		VendingMachine vendingMachine;
		if ((vendingMachine = (base.GetParentEntity() as VendingMachine)) != null)
		{
			vendingMachine.OnIndustrialItemTransferBegins();
		}
	}

	// Token: 0x060026C0 RID: 9920 RVA: 0x000EFC0C File Offset: 0x000EDE0C
	public void OnStorageItemTransferEnd()
	{
		VendingMachine vendingMachine;
		if ((vendingMachine = (base.GetParentEntity() as VendingMachine)) != null)
		{
			vendingMachine.OnIndustrialItemTransferEnds();
		}
	}

	// Token: 0x17000317 RID: 791
	// (get) Token: 0x060026C1 RID: 9921 RVA: 0x00002E37 File Offset: 0x00001037
	public BaseEntity IndustrialEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x00007074 File Offset: 0x00005274
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x060026C3 RID: 9923 RVA: 0x000EFC2E File Offset: 0x000EDE2E
	public void ClientNotifyItemAddRemoved(bool add)
	{
		if (add)
		{
			this.GreenLight.SetActive(false);
			this.GreenLight.SetActive(true);
			return;
		}
		this.RedLight.SetActive(false);
		this.RedLight.SetActive(true);
	}

	// Token: 0x04001F40 RID: 8000
	public GameObject GreenLight;

	// Token: 0x04001F41 RID: 8001
	public GameObject RedLight;
}
