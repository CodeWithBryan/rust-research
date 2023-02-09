using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000492 RID: 1170
[CreateAssetMenu(menuName = "Rust/Vehicles/Train Wagon Loot Data", fileName = "Train Wagon Loot Data")]
public class TrainWagonLootData : ScriptableObject
{
	// Token: 0x0600260A RID: 9738 RVA: 0x000ED8F1 File Offset: 0x000EBAF1
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		TrainWagonLootData.instance = Resources.Load<TrainWagonLootData>("Train Wagon Loot Data");
	}

	// Token: 0x0600260B RID: 9739 RVA: 0x000ED904 File Offset: 0x000EBB04
	public TrainWagonLootData.LootOption GetLootOption(TrainCarUnloadable.WagonType wagonType, out int index)
	{
		if (wagonType == TrainCarUnloadable.WagonType.Lootboxes)
		{
			index = 1000;
			return this.lootWagonContent;
		}
		if (wagonType != TrainCarUnloadable.WagonType.Fuel)
		{
			float num = 0f;
			foreach (TrainWagonLootData.LootOption lootOption in this.oreOptions)
			{
				num += lootOption.spawnWeighting;
			}
			float num2 = UnityEngine.Random.value * num;
			for (index = 0; index < this.oreOptions.Length; index++)
			{
				if ((num2 -= this.oreOptions[index].spawnWeighting) < 0f)
				{
					return this.oreOptions[index];
				}
			}
			return this.oreOptions[index];
		}
		index = 1001;
		return this.fuelWagonContent;
	}

	// Token: 0x0600260C RID: 9740 RVA: 0x000ED9AC File Offset: 0x000EBBAC
	public bool TryGetLootFromIndex(int index, out TrainWagonLootData.LootOption lootOption)
	{
		if (index == 1000)
		{
			lootOption = this.lootWagonContent;
			return true;
		}
		if (index != 1001)
		{
			index = Mathf.Clamp(index, 0, this.oreOptions.Length - 1);
			lootOption = this.oreOptions[index];
			return true;
		}
		lootOption = this.fuelWagonContent;
		return true;
	}

	// Token: 0x0600260D RID: 9741 RVA: 0x000EDA00 File Offset: 0x000EBC00
	public bool TryGetIndexFromLoot(TrainWagonLootData.LootOption lootOption, out int index)
	{
		if (lootOption == this.lootWagonContent)
		{
			index = 1000;
			return true;
		}
		if (lootOption == this.fuelWagonContent)
		{
			index = 1001;
			return true;
		}
		for (index = 0; index < this.oreOptions.Length; index++)
		{
			if (this.oreOptions[index] == lootOption)
			{
				return true;
			}
		}
		index = -1;
		return false;
	}

	// Token: 0x0600260E RID: 9742 RVA: 0x000EDA5C File Offset: 0x000EBC5C
	public static float GetOrePercent(int lootTypeIndex, StorageContainer sc)
	{
		TrainWagonLootData.LootOption lootOption;
		if (TrainWagonLootData.instance.TryGetLootFromIndex(lootTypeIndex, out lootOption))
		{
			return TrainWagonLootData.GetOrePercent(lootOption, sc);
		}
		return 0f;
	}

	// Token: 0x0600260F RID: 9743 RVA: 0x000EDA88 File Offset: 0x000EBC88
	public static float GetOrePercent(TrainWagonLootData.LootOption lootOption, StorageContainer sc)
	{
		float result = 0f;
		if (sc.IsValid())
		{
			int maxLootAmount = lootOption.maxLootAmount;
			if ((float)maxLootAmount == 0f)
			{
				result = 0f;
			}
			else
			{
				result = Mathf.Clamp01((float)sc.inventory.GetAmount(lootOption.lootItem.itemid, false) / (float)maxLootAmount);
			}
		}
		return result;
	}

	// Token: 0x04001EE6 RID: 7910
	[SerializeField]
	private TrainWagonLootData.LootOption[] oreOptions;

	// Token: 0x04001EE7 RID: 7911
	[SerializeField]
	[ReadOnly]
	private TrainWagonLootData.LootOption lootWagonContent;

	// Token: 0x04001EE8 RID: 7912
	[SerializeField]
	private TrainWagonLootData.LootOption fuelWagonContent;

	// Token: 0x04001EE9 RID: 7913
	public static TrainWagonLootData instance;

	// Token: 0x04001EEA RID: 7914
	private const int LOOT_WAGON_INDEX = 1000;

	// Token: 0x04001EEB RID: 7915
	private const int FUEL_WAGON_INDEX = 1001;

	// Token: 0x02000CC1 RID: 3265
	[Serializable]
	public class LootOption
	{
		// Token: 0x040043AD RID: 17325
		public bool showsFX = true;

		// Token: 0x040043AE RID: 17326
		public ItemDefinition lootItem;

		// Token: 0x040043AF RID: 17327
		[FormerlySerializedAs("lootAmount")]
		public int maxLootAmount;

		// Token: 0x040043B0 RID: 17328
		public int minLootAmount;

		// Token: 0x040043B1 RID: 17329
		public Material lootMaterial;

		// Token: 0x040043B2 RID: 17330
		public float spawnWeighting = 1f;

		// Token: 0x040043B3 RID: 17331
		public Color fxTint;

		// Token: 0x040043B4 RID: 17332
		[FormerlySerializedAs("indoorFXTint")]
		public Color particleFXTint;
	}
}
