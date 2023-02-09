using System;
using System.Linq;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000AE3 RID: 2787
	public class EngineStorage : StorageContainer
	{
		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x060042ED RID: 17133 RVA: 0x00185722 File Offset: 0x00183922
		// (set) Token: 0x060042EE RID: 17134 RVA: 0x0018572A File Offset: 0x0018392A
		public bool isUsable { get; private set; }

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x060042EF RID: 17135 RVA: 0x00185733 File Offset: 0x00183933
		// (set) Token: 0x060042F0 RID: 17136 RVA: 0x0018573B File Offset: 0x0018393B
		public float accelerationBoostPercent { get; private set; }

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x060042F1 RID: 17137 RVA: 0x00185744 File Offset: 0x00183944
		// (set) Token: 0x060042F2 RID: 17138 RVA: 0x0018574C File Offset: 0x0018394C
		public float topSpeedBoostPercent { get; private set; }

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x060042F3 RID: 17139 RVA: 0x00185755 File Offset: 0x00183955
		// (set) Token: 0x060042F4 RID: 17140 RVA: 0x0018575D File Offset: 0x0018395D
		public float fuelEconomyBoostPercent { get; private set; }

		// Token: 0x060042F5 RID: 17141 RVA: 0x00185768 File Offset: 0x00183968
		public VehicleModuleEngine GetEngineModule()
		{
			global::BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity != null)
			{
				return parentEntity.GetComponent<VehicleModuleEngine>();
			}
			return null;
		}

		// Token: 0x060042F6 RID: 17142 RVA: 0x0018578D File Offset: 0x0018398D
		public float GetAveragedLoadoutPercent()
		{
			return (this.accelerationBoostPercent + this.topSpeedBoostPercent + this.fuelEconomyBoostPercent) / 3f;
		}

		// Token: 0x060042F7 RID: 17143 RVA: 0x001857AC File Offset: 0x001839AC
		public override void Load(global::BaseNetworkable.LoadInfo info)
		{
			base.Load(info);
			if (info.msg.engineStorage != null)
			{
				this.isUsable = info.msg.engineStorage.isUsable;
				this.accelerationBoostPercent = info.msg.engineStorage.accelerationBoost;
				this.topSpeedBoostPercent = info.msg.engineStorage.topSpeedBoost;
				this.fuelEconomyBoostPercent = info.msg.engineStorage.fuelEconomyBoost;
			}
			VehicleModuleEngine engineModule = this.GetEngineModule();
			if (engineModule == null)
			{
				return;
			}
			engineModule.RefreshPerformanceStats(this);
		}

		// Token: 0x060042F8 RID: 17144 RVA: 0x00185838 File Offset: 0x00183A38
		public override bool CanBeLooted(global::BasePlayer player)
		{
			VehicleModuleEngine engineModule = this.GetEngineModule();
			return engineModule != null && engineModule.CanBeLooted(player);
		}

		// Token: 0x060042F9 RID: 17145 RVA: 0x0018585E File Offset: 0x00183A5E
		public override int GetIdealSlot(global::BasePlayer player, global::Item item)
		{
			return this.GetValidSlot(item);
		}

		// Token: 0x060042FA RID: 17146 RVA: 0x00185868 File Offset: 0x00183A68
		private int GetValidSlot(global::Item item)
		{
			ItemModEngineItem component = item.info.GetComponent<ItemModEngineItem>();
			if (component == null)
			{
				return -1;
			}
			EngineStorage.EngineItemTypes engineItemType = component.engineItemType;
			for (int i = 0; i < this.inventorySlots; i++)
			{
				if (engineItemType == this.slotTypes[i] && !base.inventory.SlotTaken(item, i))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060042FB RID: 17147 RVA: 0x001858C1 File Offset: 0x00183AC1
		public override void OnInventoryFirstCreated(global::ItemContainer container)
		{
			this.RefreshLoadoutData();
		}

		// Token: 0x060042FC RID: 17148 RVA: 0x000059DD File Offset: 0x00003BDD
		public void NonUserSpawn()
		{
		}

		// Token: 0x060042FD RID: 17149 RVA: 0x001858C1 File Offset: 0x00183AC1
		public override void OnItemAddedOrRemoved(global::Item item, bool added)
		{
			this.RefreshLoadoutData();
		}

		// Token: 0x060042FE RID: 17150 RVA: 0x001858CC File Offset: 0x00183ACC
		public override bool ItemFilter(global::Item item, int targetSlot)
		{
			if (!base.ItemFilter(item, targetSlot))
			{
				return false;
			}
			if (targetSlot < 0 || targetSlot >= this.slotTypes.Length)
			{
				return false;
			}
			ItemModEngineItem component = item.info.GetComponent<ItemModEngineItem>();
			return component != null && component.engineItemType == this.slotTypes[targetSlot];
		}

		// Token: 0x060042FF RID: 17151 RVA: 0x00185920 File Offset: 0x00183B20
		public void RefreshLoadoutData()
		{
			bool isUsable;
			if (base.inventory.IsFull())
			{
				isUsable = base.inventory.itemList.All((global::Item item) => !item.isBroken);
			}
			else
			{
				isUsable = false;
			}
			this.isUsable = isUsable;
			this.accelerationBoostPercent = this.GetContainerItemsValueFor(new Func<EngineStorage.EngineItemTypes, bool>(EngineItemTypeEx.BoostsAcceleration)) / (float)this.accelerationBoostSlots;
			this.topSpeedBoostPercent = this.GetContainerItemsValueFor(new Func<EngineStorage.EngineItemTypes, bool>(EngineItemTypeEx.BoostsTopSpeed)) / (float)this.topSpeedBoostSlots;
			this.fuelEconomyBoostPercent = this.GetContainerItemsValueFor(new Func<EngineStorage.EngineItemTypes, bool>(EngineItemTypeEx.BoostsFuelEconomy)) / (float)this.fuelEconomyBoostSlots;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			VehicleModuleEngine engineModule = this.GetEngineModule();
			if (engineModule == null)
			{
				return;
			}
			engineModule.RefreshPerformanceStats(this);
		}

		// Token: 0x06004300 RID: 17152 RVA: 0x001859EC File Offset: 0x00183BEC
		public override void Save(global::BaseNetworkable.SaveInfo info)
		{
			base.Save(info);
			info.msg.engineStorage = Pool.Get<EngineStorage>();
			info.msg.engineStorage.isUsable = this.isUsable;
			info.msg.engineStorage.accelerationBoost = this.accelerationBoostPercent;
			info.msg.engineStorage.topSpeedBoost = this.topSpeedBoostPercent;
			info.msg.engineStorage.fuelEconomyBoost = this.fuelEconomyBoostPercent;
		}

		// Token: 0x06004301 RID: 17153 RVA: 0x00185A68 File Offset: 0x00183C68
		public void OnModuleDamaged(float damageTaken)
		{
			if (damageTaken <= 0f)
			{
				return;
			}
			damageTaken *= this.internalDamageMultiplier;
			float[] array = new float[base.inventory.capacity];
			float num = 0f;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = UnityEngine.Random.value;
				num += array[i];
			}
			float num2 = damageTaken / num;
			for (int j = 0; j < array.Length; j++)
			{
				global::Item slot = base.inventory.GetSlot(j);
				if (slot != null)
				{
					slot.condition -= array[j] * num2;
				}
			}
			this.RefreshLoadoutData();
		}

		// Token: 0x06004302 RID: 17154 RVA: 0x00185B00 File Offset: 0x00183D00
		public void AdminAddParts(int tier)
		{
			if (base.inventory == null)
			{
				Debug.LogWarning(base.GetType().Name + ": Null inventory on " + base.name);
				return;
			}
			for (int i = 0; i < base.inventory.capacity; i++)
			{
				global::Item slot = base.inventory.GetSlot(i);
				if (slot != null)
				{
					slot.RemoveFromContainer();
					slot.Remove(0f);
				}
			}
			for (int j = 0; j < base.inventory.capacity; j++)
			{
				ItemModEngineItem itemModEngineItem;
				if (base.inventory.GetSlot(j) == null && this.allEngineItems.TryGetItem(tier, this.slotTypes[j], out itemModEngineItem))
				{
					ItemDefinition component = itemModEngineItem.GetComponent<ItemDefinition>();
					global::Item item = ItemManager.Create(component, 1, 0UL);
					if (item != null)
					{
						item.condition = component.condition.max;
						item.MoveToContainer(base.inventory, j, false, false, null, true);
					}
					else
					{
						Debug.LogError(base.GetType().Name + ": Failed to create engine storage item.");
					}
				}
			}
		}

		// Token: 0x06004303 RID: 17155 RVA: 0x00185C0C File Offset: 0x00183E0C
		private float GetContainerItemsValueFor(Func<EngineStorage.EngineItemTypes, bool> boostConditional)
		{
			float num = 0f;
			foreach (global::Item item in base.inventory.itemList)
			{
				ItemModEngineItem component = item.info.GetComponent<ItemModEngineItem>();
				if (component != null && boostConditional(component.engineItemType) && !item.isBroken)
				{
					num += (float)item.amount * this.GetTierValue(component.tier);
				}
			}
			return num;
		}

		// Token: 0x06004304 RID: 17156 RVA: 0x00185CA8 File Offset: 0x00183EA8
		private float GetTierValue(int tier)
		{
			switch (tier)
			{
			case 1:
				return 0.6f;
			case 2:
				return 0.8f;
			case 3:
				return 1f;
			default:
				Debug.LogError(base.GetType().Name + ": Unrecognised item tier: " + tier);
				return 0f;
			}
		}

		// Token: 0x04003B90 RID: 15248
		[Header("Engine Storage")]
		public Sprite engineIcon;

		// Token: 0x04003B91 RID: 15249
		public float internalDamageMultiplier = 0.5f;

		// Token: 0x04003B92 RID: 15250
		public EngineStorage.EngineItemTypes[] slotTypes;

		// Token: 0x04003B93 RID: 15251
		[SerializeField]
		private VehicleModuleEngineItems allEngineItems;

		// Token: 0x04003B94 RID: 15252
		[SerializeField]
		[ReadOnly]
		private int accelerationBoostSlots;

		// Token: 0x04003B95 RID: 15253
		[SerializeField]
		[ReadOnly]
		private int topSpeedBoostSlots;

		// Token: 0x04003B96 RID: 15254
		[SerializeField]
		[ReadOnly]
		private int fuelEconomyBoostSlots;

		// Token: 0x02000F2C RID: 3884
		public enum EngineItemTypes
		{
			// Token: 0x04004D8A RID: 19850
			Crankshaft,
			// Token: 0x04004D8B RID: 19851
			Carburetor,
			// Token: 0x04004D8C RID: 19852
			SparkPlug,
			// Token: 0x04004D8D RID: 19853
			Piston,
			// Token: 0x04004D8E RID: 19854
			Valve
		}
	}
}
