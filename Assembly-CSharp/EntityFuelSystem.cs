using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003C8 RID: 968
public class EntityFuelSystem
{
	// Token: 0x06002108 RID: 8456 RVA: 0x000D5C74 File Offset: 0x000D3E74
	public EntityFuelSystem(bool isServer, GameObjectRef fuelStoragePrefab, List<BaseEntity> children, bool editorGiveFreeFuel = true)
	{
		this.isServer = isServer;
		this.editorGiveFreeFuel = editorGiveFreeFuel;
		this.fuelStorageID = fuelStoragePrefab.GetEntity().prefabID;
		if (isServer)
		{
			foreach (BaseEntity child in children)
			{
				this.CheckNewChild(child);
			}
		}
	}

	// Token: 0x06002109 RID: 8457 RVA: 0x000D5CEC File Offset: 0x000D3EEC
	public bool IsInFuelInteractionRange(BasePlayer player)
	{
		StorageContainer fuelContainer = this.GetFuelContainer();
		if (fuelContainer != null)
		{
			float num = 0f;
			if (this.isServer)
			{
				num = 3f;
			}
			return fuelContainer.Distance(player.eyes.position) <= num;
		}
		return false;
	}

	// Token: 0x0600210A RID: 8458 RVA: 0x000D5D38 File Offset: 0x000D3F38
	private StorageContainer GetFuelContainer()
	{
		StorageContainer storageContainer = this.fuelStorageInstance.Get(this.isServer);
		if (storageContainer.IsValid())
		{
			return storageContainer;
		}
		return null;
	}

	// Token: 0x0600210B RID: 8459 RVA: 0x000D5D62 File Offset: 0x000D3F62
	public void CheckNewChild(BaseEntity child)
	{
		if (child.prefabID == this.fuelStorageID)
		{
			this.fuelStorageInstance.Set((StorageContainer)child);
		}
	}

	// Token: 0x0600210C RID: 8460 RVA: 0x000D5D84 File Offset: 0x000D3F84
	public Item GetFuelItem()
	{
		StorageContainer fuelContainer = this.GetFuelContainer();
		if (fuelContainer == null)
		{
			return null;
		}
		return fuelContainer.inventory.GetSlot(0);
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x000D5DB0 File Offset: 0x000D3FB0
	public int GetFuelAmount()
	{
		Item fuelItem = this.GetFuelItem();
		if (fuelItem == null || fuelItem.amount < 1)
		{
			return 0;
		}
		return fuelItem.amount;
	}

	// Token: 0x0600210E RID: 8462 RVA: 0x000D5DD8 File Offset: 0x000D3FD8
	public float GetFuelFraction()
	{
		Item fuelItem = this.GetFuelItem();
		if (fuelItem == null || fuelItem.amount < 1)
		{
			return 0f;
		}
		return Mathf.Clamp01((float)fuelItem.amount / (float)fuelItem.MaxStackable());
	}

	// Token: 0x0600210F RID: 8463 RVA: 0x000D5E14 File Offset: 0x000D4014
	public bool HasFuel(bool forceCheck = false)
	{
		if (Time.time > this.nextFuelCheckTime || forceCheck)
		{
			this.cachedHasFuel = ((float)this.GetFuelAmount() > 0f);
			this.nextFuelCheckTime = Time.time + UnityEngine.Random.Range(1f, 2f);
		}
		return this.cachedHasFuel;
	}

	// Token: 0x06002110 RID: 8464 RVA: 0x000D5E68 File Offset: 0x000D4068
	public int TryUseFuel(float seconds, float fuelUsedPerSecond)
	{
		StorageContainer fuelContainer = this.GetFuelContainer();
		if (fuelContainer == null)
		{
			return 0;
		}
		Item slot = fuelContainer.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		this.pendingFuel += seconds * fuelUsedPerSecond;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			this.pendingFuel -= (float)num;
			return num;
		}
		return 0;
	}

	// Token: 0x06002111 RID: 8465 RVA: 0x000D5EE4 File Offset: 0x000D40E4
	public void LootFuel(BasePlayer player)
	{
		if (this.IsInFuelInteractionRange(player))
		{
			this.GetFuelContainer().PlayerOpenLoot(player, "", true);
		}
	}

	// Token: 0x06002112 RID: 8466 RVA: 0x000D5F04 File Offset: 0x000D4104
	public void AddStartingFuel(float amount = -1f)
	{
		amount = ((amount == -1f) ? ((float)this.GetFuelContainer().allowedItem.stackable * 0.2f) : amount);
		this.GetFuelContainer().inventory.AddItem(this.GetFuelContainer().allowedItem, Mathf.FloorToInt(amount), 0UL, ItemContainer.LimitStack.Existing);
	}

	// Token: 0x06002113 RID: 8467 RVA: 0x000D5F59 File Offset: 0x000D4159
	public void AdminAddFuel()
	{
		this.GetFuelContainer().inventory.AddItem(this.GetFuelContainer().allowedItem, this.GetFuelContainer().allowedItem.stackable, 0UL, ItemContainer.LimitStack.Existing);
	}

	// Token: 0x04001982 RID: 6530
	private readonly bool isServer;

	// Token: 0x04001983 RID: 6531
	private readonly bool editorGiveFreeFuel;

	// Token: 0x04001984 RID: 6532
	private readonly uint fuelStorageID;

	// Token: 0x04001985 RID: 6533
	public EntityRef<StorageContainer> fuelStorageInstance;

	// Token: 0x04001986 RID: 6534
	private float nextFuelCheckTime;

	// Token: 0x04001987 RID: 6535
	private bool cachedHasFuel;

	// Token: 0x04001988 RID: 6536
	private float pendingFuel;
}
