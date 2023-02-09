using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000AE5 RID: 2789
	public class ModularVehicleInventory : IDisposable
	{
		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x06004307 RID: 17159 RVA: 0x00185D83 File Offset: 0x00183F83
		public ItemContainer ModuleContainer { get; }

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x06004308 RID: 17160 RVA: 0x00185D8B File Offset: 0x00183F8B
		public ItemContainer ChassisContainer { get; }

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x06004309 RID: 17161 RVA: 0x00185D93 File Offset: 0x00183F93
		public uint UID
		{
			get
			{
				return this.ModuleContainer.uid;
			}
		}

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x0600430A RID: 17162 RVA: 0x00185DA0 File Offset: 0x00183FA0
		private int TotalSockets
		{
			get
			{
				return this.vehicle.TotalSockets;
			}
		}

		// Token: 0x0600430B RID: 17163 RVA: 0x00185DB0 File Offset: 0x00183FB0
		public ModularVehicleInventory(BaseModularVehicle vehicle, ItemDefinition chassisItemDef, bool giveUID)
		{
			this.vehicle = vehicle;
			this.ModuleContainer = this.CreateModuleInventory(vehicle, giveUID);
			this.ChassisContainer = this.CreateChassisInventory(vehicle, giveUID);
			vehicle.AssociatedItemInstance = ItemManager.Create(chassisItemDef, 1, 0UL);
			if (!Application.isLoadingSave)
			{
				vehicle.AssociatedItemInstance.MoveToContainer(this.ChassisContainer, 0, false, false, null, true);
			}
		}

		// Token: 0x0600430C RID: 17164 RVA: 0x00185E14 File Offset: 0x00184014
		public void Dispose()
		{
			foreach (Item item in this.ModuleContainer.itemList)
			{
				item.OnDirty -= this.OnModuleItemChanged;
			}
		}

		// Token: 0x0600430D RID: 17165 RVA: 0x00185E78 File Offset: 0x00184078
		public void GiveUIDs()
		{
			this.ModuleContainer.GiveUID();
			this.ChassisContainer.GiveUID();
		}

		// Token: 0x0600430E RID: 17166 RVA: 0x00185E90 File Offset: 0x00184090
		public bool SocketIsFree(int socketIndex, Item moduleItem = null)
		{
			Item item = null;
			int num = socketIndex;
			while (item == null && num >= 0)
			{
				item = this.ModuleContainer.GetSlot(num);
				if (item != null)
				{
					if (item == moduleItem)
					{
						return true;
					}
					ItemModVehicleModule component = item.info.GetComponent<ItemModVehicleModule>();
					return num + component.socketsTaken - 1 < socketIndex;
				}
				else
				{
					num--;
				}
			}
			return true;
		}

		// Token: 0x0600430F RID: 17167 RVA: 0x00185EDF File Offset: 0x001840DF
		public bool SocketIsTaken(int socketIndex)
		{
			return !this.SocketIsFree(socketIndex, null);
		}

		// Token: 0x06004310 RID: 17168 RVA: 0x00185EEC File Offset: 0x001840EC
		public bool TryAddModuleItem(Item moduleItem, int socketIndex)
		{
			if (moduleItem == null)
			{
				Debug.LogError(base.GetType().Name + ": Can't add null item.");
				return false;
			}
			return moduleItem.MoveToContainer(this.ModuleContainer, socketIndex, false, false, null, true);
		}

		// Token: 0x06004311 RID: 17169 RVA: 0x00185F1E File Offset: 0x0018411E
		public bool RemoveAndDestroy(Item itemToRemove)
		{
			bool result = this.ModuleContainer.Remove(itemToRemove);
			itemToRemove.Remove(0f);
			return result;
		}

		// Token: 0x06004312 RID: 17170 RVA: 0x00185F37 File Offset: 0x00184137
		public int TryGetFreeSocket(int socketsTaken)
		{
			return this.TryGetFreeSocket(null, socketsTaken);
		}

		// Token: 0x06004313 RID: 17171 RVA: 0x00185F44 File Offset: 0x00184144
		public int TryGetFreeSocket(Item moduleItem, int socketsTaken)
		{
			for (int i = 0; i <= this.TotalSockets - socketsTaken; i++)
			{
				if (this.SocketsAreFree(i, socketsTaken, moduleItem))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06004314 RID: 17172 RVA: 0x00185F74 File Offset: 0x00184174
		public bool SocketsAreFree(int firstIndex, int socketsTaken, Item moduleItem = null)
		{
			if (firstIndex < 0 || firstIndex + socketsTaken > this.TotalSockets)
			{
				return false;
			}
			for (int i = firstIndex; i < firstIndex + socketsTaken; i++)
			{
				if (!this.SocketIsFree(i, moduleItem))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004315 RID: 17173 RVA: 0x00185FB0 File Offset: 0x001841B0
		public bool TrySyncModuleInventory(BaseVehicleModule moduleEntity, int firstSocketIndex)
		{
			if (firstSocketIndex < 0)
			{
				Debug.LogError(string.Format("{0}: Invalid socket index ({1}) for new module entity.", base.GetType().Name, firstSocketIndex), this.vehicle.gameObject);
				return false;
			}
			Item slot = this.ModuleContainer.GetSlot(firstSocketIndex);
			int numSocketsTaken = moduleEntity.GetNumSocketsTaken();
			if (!this.SocketsAreFree(firstSocketIndex, numSocketsTaken, null) && (slot == null || moduleEntity.AssociatedItemInstance != slot))
			{
				Debug.LogError(string.Format("{0}: Sockets are not free for new module entity. First: {1} Taken: {2}", base.GetType().Name, firstSocketIndex, numSocketsTaken), this.vehicle.gameObject);
				return false;
			}
			if (slot != null)
			{
				return true;
			}
			Item item = ItemManager.Create(moduleEntity.AssociatedItemDef, 1, 0UL);
			item.condition = moduleEntity.health;
			moduleEntity.AssociatedItemInstance = item;
			bool flag = this.TryAddModuleItem(item, firstSocketIndex);
			if (flag)
			{
				this.vehicle.SetUpModule(moduleEntity, item);
				return flag;
			}
			item.Remove(0f);
			return flag;
		}

		// Token: 0x06004316 RID: 17174 RVA: 0x00186097 File Offset: 0x00184297
		private bool SocketIsUsed(Item item, int slotIndex)
		{
			return !this.SocketIsFree(slotIndex, item);
		}

		// Token: 0x06004317 RID: 17175 RVA: 0x001860A4 File Offset: 0x001842A4
		private ItemContainer CreateModuleInventory(BaseModularVehicle vehicle, bool giveUID)
		{
			if (this.ModuleContainer != null)
			{
				return this.ModuleContainer;
			}
			ItemContainer itemContainer = new ItemContainer
			{
				entityOwner = vehicle,
				allowedContents = ItemContainer.ContentsType.Generic,
				maxStackSize = 1
			};
			itemContainer.ServerInitialize(null, this.TotalSockets);
			if (giveUID)
			{
				itemContainer.GiveUID();
			}
			itemContainer.onItemAddedRemoved = new Action<Item, bool>(this.OnSocketInventoryAddRemove);
			itemContainer.canAcceptItem = new Func<Item, int, bool>(this.ItemFilter);
			itemContainer.slotIsReserved = new Func<Item, int, bool>(this.SocketIsUsed);
			return itemContainer;
		}

		// Token: 0x06004318 RID: 17176 RVA: 0x00186128 File Offset: 0x00184328
		private ItemContainer CreateChassisInventory(BaseModularVehicle vehicle, bool giveUID)
		{
			if (this.ChassisContainer != null)
			{
				return this.ChassisContainer;
			}
			ItemContainer itemContainer = new ItemContainer
			{
				entityOwner = vehicle,
				allowedContents = ItemContainer.ContentsType.Generic,
				maxStackSize = 1
			};
			itemContainer.ServerInitialize(null, 1);
			if (giveUID)
			{
				itemContainer.GiveUID();
			}
			return itemContainer;
		}

		// Token: 0x06004319 RID: 17177 RVA: 0x00186171 File Offset: 0x00184371
		private void OnSocketInventoryAddRemove(Item moduleItem, bool added)
		{
			if (added)
			{
				this.ModuleItemAdded(moduleItem, moduleItem.position);
				return;
			}
			this.ModuleItemRemoved(moduleItem);
		}

		// Token: 0x0600431A RID: 17178 RVA: 0x0018618C File Offset: 0x0018438C
		private void ModuleItemAdded(Item moduleItem, int socketIndex)
		{
			ItemModVehicleModule component = moduleItem.info.GetComponent<ItemModVehicleModule>();
			if (!Application.isLoadingSave && this.vehicle.GetModuleForItem(moduleItem) == null)
			{
				this.vehicle.CreatePhysicalModuleEntity(moduleItem, component, socketIndex);
			}
			moduleItem.OnDirty += this.OnModuleItemChanged;
		}

		// Token: 0x0600431B RID: 17179 RVA: 0x001861E4 File Offset: 0x001843E4
		private void ModuleItemRemoved(Item moduleItem)
		{
			if (moduleItem == null)
			{
				Debug.LogError("Null module item removed.", this.vehicle.gameObject);
				return;
			}
			moduleItem.OnDirty -= this.OnModuleItemChanged;
			BaseVehicleModule moduleForItem = this.vehicle.GetModuleForItem(moduleItem);
			if (moduleForItem != null)
			{
				if (!moduleForItem.IsFullySpawned())
				{
					Debug.LogError("Module entity being removed before it's fully spawned. This could cause errors.", this.vehicle.gameObject);
				}
				moduleForItem.Kill(BaseNetworkable.DestroyMode.None);
				return;
			}
			Debug.Log("Couldn't find entity for this item.");
		}

		// Token: 0x0600431C RID: 17180 RVA: 0x00186264 File Offset: 0x00184464
		private void OnModuleItemChanged(Item moduleItem)
		{
			BaseVehicleModule moduleForItem = this.vehicle.GetModuleForItem(moduleItem);
			if (moduleForItem != null)
			{
				moduleForItem.SetHealth(moduleItem.condition);
				if (moduleForItem.FirstSocketIndex != moduleItem.position)
				{
					this.ModuleItemRemoved(moduleItem);
					this.ModuleItemAdded(moduleItem, moduleItem.position);
				}
			}
		}

		// Token: 0x0600431D RID: 17181 RVA: 0x001862B8 File Offset: 0x001844B8
		private bool ItemFilter(Item item, int targetSlot)
		{
			string text;
			return this.vehicle.ModuleCanBeAdded(item, targetSlot, out text);
		}

		// Token: 0x04003BB2 RID: 15282
		private readonly BaseModularVehicle vehicle;
	}
}
