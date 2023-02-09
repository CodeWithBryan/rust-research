using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public class FishLookup : PrefabAttribute
{
	// Token: 0x060017C2 RID: 6082 RVA: 0x000B0C58 File Offset: 0x000AEE58
	public static void LoadFish()
	{
		if (FishLookup.AvailableFish != null)
		{
			if (FishLookup.lastShuffle > 5f)
			{
				FishLookup.AvailableFish.Shuffle((uint)UnityEngine.Random.Range(0, 10000));
			}
			return;
		}
		List<ItemModFishable> list = Pool.GetList<ItemModFishable>();
		List<ItemDefinition> list2 = Pool.GetList<ItemDefinition>();
		foreach (ItemDefinition itemDefinition in ItemManager.itemList)
		{
			ItemModFishable item;
			if (itemDefinition.TryGetComponent<ItemModFishable>(out item))
			{
				list.Add(item);
			}
			ItemModCompostable itemModCompostable;
			if (itemDefinition.TryGetComponent<ItemModCompostable>(out itemModCompostable) && itemModCompostable.BaitValue > 0f)
			{
				list2.Add(itemDefinition);
			}
		}
		FishLookup.AvailableFish = list.ToArray();
		FishLookup.BaitItems = list2.ToArray();
		Pool.FreeList<ItemModFishable>(ref list);
		Pool.FreeList<ItemDefinition>(ref list2);
	}

	// Token: 0x060017C3 RID: 6083 RVA: 0x000B0D34 File Offset: 0x000AEF34
	public ItemDefinition GetFish(Vector3 worldPos, WaterBody bodyType, ItemDefinition lure, out ItemModFishable fishable, ItemModFishable ignoreFish)
	{
		FishLookup.LoadFish();
		ItemModCompostable itemModCompostable;
		float num = lure.TryGetComponent<ItemModCompostable>(out itemModCompostable) ? itemModCompostable.BaitValue : 0f;
		WaterBody.FishingTag fishingTag = (bodyType != null) ? bodyType.FishingType : WaterBody.FishingTag.Ocean;
		float num2 = WaterLevel.GetOverallWaterDepth(worldPos, true, null, true);
		if (worldPos.y < -10f)
		{
			num2 = 10f;
		}
		int num3 = UnityEngine.Random.Range(0, FishLookup.AvailableFish.Length);
		for (int i = 0; i < FishLookup.AvailableFish.Length; i++)
		{
			num3++;
			if (num3 >= FishLookup.AvailableFish.Length)
			{
				num3 = 0;
			}
			ItemModFishable itemModFishable = FishLookup.AvailableFish[num3];
			if (itemModFishable.CanBeFished && itemModFishable.MinimumBaitLevel <= num && (itemModFishable.MaximumBaitLevel <= 0f || num <= itemModFishable.MaximumBaitLevel) && !(itemModFishable == ignoreFish) && (itemModFishable.RequiredTag == (WaterBody.FishingTag)(-1) || (itemModFishable.RequiredTag & fishingTag) != (WaterBody.FishingTag)0) && ((fishingTag & WaterBody.FishingTag.Ocean) != WaterBody.FishingTag.Ocean || ((itemModFishable.MinimumWaterDepth <= 0f || num2 >= itemModFishable.MinimumWaterDepth) && (itemModFishable.MaximumWaterDepth <= 0f || num2 <= itemModFishable.MaximumWaterDepth))) && UnityEngine.Random.Range(0f, 1f) - num * 3f * 0.01f <= itemModFishable.Chance)
			{
				fishable = itemModFishable;
				return itemModFishable.GetComponent<ItemDefinition>();
			}
		}
		fishable = this.FallbackFish;
		return this.FallbackFish.GetComponent<ItemDefinition>();
	}

	// Token: 0x060017C4 RID: 6084 RVA: 0x000B0EA8 File Offset: 0x000AF0A8
	public void CheckCatchAllAchievement(BasePlayer player)
	{
		FishLookup.LoadFish();
		int num = 0;
		foreach (ItemModFishable itemModFishable in FishLookup.AvailableFish)
		{
			if (!string.IsNullOrEmpty(itemModFishable.SteamStatName) && player.stats.steam.Get(itemModFishable.SteamStatName) > 0)
			{
				num++;
			}
		}
		if (num == 9)
		{
			player.GiveAchievement("PRO_ANGLER");
		}
	}

	// Token: 0x060017C5 RID: 6085 RVA: 0x000B0F0E File Offset: 0x000AF10E
	protected override Type GetIndexedType()
	{
		return typeof(FishLookup);
	}

	// Token: 0x040010C9 RID: 4297
	public ItemModFishable FallbackFish;

	// Token: 0x040010CA RID: 4298
	private static ItemModFishable[] AvailableFish;

	// Token: 0x040010CB RID: 4299
	public static ItemDefinition[] BaitItems;

	// Token: 0x040010CC RID: 4300
	private static TimeSince lastShuffle;

	// Token: 0x040010CD RID: 4301
	public const int ALL_FISH_COUNT = 9;

	// Token: 0x040010CE RID: 4302
	public const string ALL_FISH_ACHIEVEMENT_NAME = "PRO_ANGLER";
}
