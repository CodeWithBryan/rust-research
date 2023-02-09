using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020005DD RID: 1501
public class ItemManager
{
	// Token: 0x06002C17 RID: 11287 RVA: 0x00108890 File Offset: 0x00106A90
	public static void InvalidateWorkshopSkinCache()
	{
		if (ItemManager.itemList == null)
		{
			return;
		}
		foreach (ItemDefinition itemDefinition in ItemManager.itemList)
		{
			itemDefinition.InvalidateWorkshopSkinCache();
		}
	}

	// Token: 0x06002C18 RID: 11288 RVA: 0x001088E8 File Offset: 0x00106AE8
	public static void Initialize()
	{
		if (ItemManager.itemList != null)
		{
			return;
		}
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		GameObject[] array = FileSystem.LoadAllFromBundle<GameObject>("items.preload.bundle", "l:ItemDefinition");
		if (array.Length == 0)
		{
			throw new Exception("items.preload.bundle has no items!");
		}
		if (stopwatch.Elapsed.TotalSeconds > 1.0)
		{
			UnityEngine.Debug.Log("Loading Items Took: " + (stopwatch.Elapsed.TotalMilliseconds / 1000.0).ToString() + " seconds");
		}
		List<ItemDefinition> list = (from x in array
		select x.GetComponent<ItemDefinition>() into x
		where x != null
		select x).ToList<ItemDefinition>();
		List<ItemBlueprint> list2 = (from x in array
		select x.GetComponent<ItemBlueprint>() into x
		where x != null && x.userCraftable
		select x).ToList<ItemBlueprint>();
		Dictionary<int, ItemDefinition> dictionary = new Dictionary<int, ItemDefinition>();
		Dictionary<string, ItemDefinition> dictionary2 = new Dictionary<string, ItemDefinition>(StringComparer.OrdinalIgnoreCase);
		foreach (ItemDefinition itemDefinition in list)
		{
			itemDefinition.Initialize(list);
			if (dictionary.ContainsKey(itemDefinition.itemid))
			{
				ItemDefinition itemDefinition2 = dictionary[itemDefinition.itemid];
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					"Item ID duplicate ",
					itemDefinition.itemid,
					" (",
					itemDefinition.name,
					") - have you given your items unique shortnames?"
				}), itemDefinition.gameObject);
				UnityEngine.Debug.LogWarning("Other item is " + itemDefinition2.name, itemDefinition2);
			}
			else if (string.IsNullOrEmpty(itemDefinition.shortname))
			{
				UnityEngine.Debug.LogWarning(string.Format("{0} has a null short name! id: {1} {2}", itemDefinition, itemDefinition.itemid, itemDefinition.displayName.english));
			}
			else
			{
				dictionary.Add(itemDefinition.itemid, itemDefinition);
				dictionary2.Add(itemDefinition.shortname, itemDefinition);
			}
		}
		stopwatch.Stop();
		if (stopwatch.Elapsed.TotalSeconds > 1.0)
		{
			UnityEngine.Debug.Log(string.Concat(new string[]
			{
				"Building Items Took: ",
				(stopwatch.Elapsed.TotalMilliseconds / 1000.0).ToString(),
				" seconds / Items: ",
				list.Count.ToString(),
				" / Blueprints: ",
				list2.Count.ToString()
			}));
		}
		ItemManager.defaultBlueprints = (from x in list2
		where !x.NeedsSteamItem && !x.NeedsSteamDLC && x.defaultBlueprint
		select x.targetItem.itemid).ToArray<int>();
		ItemManager.itemList = list;
		ItemManager.bpList = list2;
		ItemManager.itemDictionary = dictionary;
		ItemManager.itemDictionaryByName = dictionary2;
		ItemManager.blueprintBaseDef = ItemManager.FindItemDefinition("blueprintbase");
	}

	// Token: 0x06002C19 RID: 11289 RVA: 0x00108C54 File Offset: 0x00106E54
	public static global::Item CreateByName(string strName, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == strName);
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.CreateByItemID(itemDefinition.itemid, iAmount, skin);
	}

	// Token: 0x06002C1A RID: 11290 RVA: 0x00108CA0 File Offset: 0x00106EA0
	public static global::Item CreateByPartialName(string strName, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == strName);
		if (itemDefinition == null)
		{
			itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname.Contains(strName, CompareOptions.IgnoreCase));
		}
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.CreateByItemID(itemDefinition.itemid, iAmount, skin);
	}

	// Token: 0x06002C1B RID: 11291 RVA: 0x00108D0C File Offset: 0x00106F0C
	public static ItemDefinition FindDefinitionByPartialName(string strName, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname == strName);
		if (itemDefinition == null)
		{
			itemDefinition = ItemManager.itemList.Find((ItemDefinition x) => x.shortname.Contains(strName, CompareOptions.IgnoreCase));
		}
		return itemDefinition;
	}

	// Token: 0x06002C1C RID: 11292 RVA: 0x00108D60 File Offset: 0x00106F60
	public static global::Item CreateByItemID(int itemID, int iAmount = 1, ulong skin = 0UL)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemID);
		if (itemDefinition == null)
		{
			return null;
		}
		return ItemManager.Create(itemDefinition, iAmount, skin);
	}

	// Token: 0x06002C1D RID: 11293 RVA: 0x00108D88 File Offset: 0x00106F88
	public static global::Item Create(ItemDefinition template, int iAmount = 1, ulong skin = 0UL)
	{
		ItemManager.TrySkinChangeItem(ref template, ref skin);
		if (template == null)
		{
			UnityEngine.Debug.LogWarning("Creating invalid/missing item!");
			return null;
		}
		global::Item item = new global::Item();
		item.isServer = true;
		if (iAmount <= 0)
		{
			UnityEngine.Debug.LogError("Creating item with less than 1 amount! (" + template.displayName.english + ")");
			return null;
		}
		item.info = template;
		item.amount = iAmount;
		item.skin = skin;
		item.Initialize(template);
		return item;
	}

	// Token: 0x06002C1E RID: 11294 RVA: 0x00108E04 File Offset: 0x00107004
	private static void TrySkinChangeItem(ref ItemDefinition template, ref ulong skinId)
	{
		if (skinId == 0UL)
		{
			return;
		}
		ItemSkinDirectory.Skin skin = ItemSkinDirectory.FindByInventoryDefinitionId((int)skinId);
		if (skin.id == 0)
		{
			return;
		}
		ItemSkin itemSkin = skin.invItem as ItemSkin;
		if (itemSkin == null)
		{
			return;
		}
		if (itemSkin.Redirect == null)
		{
			return;
		}
		template = itemSkin.Redirect;
		skinId = 0UL;
	}

	// Token: 0x06002C1F RID: 11295 RVA: 0x00108E5C File Offset: 0x0010705C
	public static global::Item Load(ProtoBuf.Item load, global::Item created, bool isServer)
	{
		if (created == null)
		{
			created = new global::Item();
		}
		created.isServer = isServer;
		created.Load(load);
		if (created.info == null)
		{
			UnityEngine.Debug.LogWarning("Item loading failed - item is invalid");
			return null;
		}
		if (created.info == ItemManager.blueprintBaseDef && created.blueprintTargetDef == null)
		{
			UnityEngine.Debug.LogWarning("Blueprint item loading failed - invalid item target");
			return null;
		}
		return created;
	}

	// Token: 0x06002C20 RID: 11296 RVA: 0x00108EC8 File Offset: 0x001070C8
	public static ItemDefinition FindItemDefinition(int itemID)
	{
		ItemManager.Initialize();
		ItemDefinition result;
		if (ItemManager.itemDictionary.TryGetValue(itemID, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06002C21 RID: 11297 RVA: 0x00108EEC File Offset: 0x001070EC
	public static ItemDefinition FindItemDefinition(string shortName)
	{
		ItemManager.Initialize();
		ItemDefinition result;
		if (ItemManager.itemDictionaryByName.TryGetValue(shortName, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06002C22 RID: 11298 RVA: 0x00108F10 File Offset: 0x00107110
	public static ItemBlueprint FindBlueprint(ItemDefinition item)
	{
		return item.GetComponent<ItemBlueprint>();
	}

	// Token: 0x06002C23 RID: 11299 RVA: 0x00108F18 File Offset: 0x00107118
	public static List<ItemDefinition> GetItemDefinitions()
	{
		ItemManager.Initialize();
		return ItemManager.itemList;
	}

	// Token: 0x06002C24 RID: 11300 RVA: 0x00108F24 File Offset: 0x00107124
	public static List<ItemBlueprint> GetBlueprints()
	{
		ItemManager.Initialize();
		return ItemManager.bpList;
	}

	// Token: 0x06002C25 RID: 11301 RVA: 0x00108F30 File Offset: 0x00107130
	public static void DoRemoves()
	{
		using (TimeWarning.New("DoRemoves", 0))
		{
			for (int i = 0; i < ItemManager.ItemRemoves.Count; i++)
			{
				if (ItemManager.ItemRemoves[i].time <= Time.time)
				{
					global::Item item = ItemManager.ItemRemoves[i].item;
					ItemManager.ItemRemoves.RemoveAt(i--);
					item.DoRemove();
				}
			}
		}
	}

	// Token: 0x06002C26 RID: 11302 RVA: 0x00108FB8 File Offset: 0x001071B8
	public static void Heartbeat()
	{
		ItemManager.DoRemoves();
	}

	// Token: 0x06002C27 RID: 11303 RVA: 0x00108FC0 File Offset: 0x001071C0
	public static void RemoveItem(global::Item item, float fTime = 0f)
	{
		Assert.IsTrue(item.isServer, "RemoveItem: Removing a client item!");
		ItemManager.ItemRemove item2 = default(ItemManager.ItemRemove);
		item2.item = item;
		item2.time = Time.time + fTime;
		ItemManager.ItemRemoves.Add(item2);
	}

	// Token: 0x040023F5 RID: 9205
	public static List<ItemDefinition> itemList;

	// Token: 0x040023F6 RID: 9206
	public static Dictionary<int, ItemDefinition> itemDictionary;

	// Token: 0x040023F7 RID: 9207
	public static Dictionary<string, ItemDefinition> itemDictionaryByName;

	// Token: 0x040023F8 RID: 9208
	public static List<ItemBlueprint> bpList;

	// Token: 0x040023F9 RID: 9209
	public static int[] defaultBlueprints;

	// Token: 0x040023FA RID: 9210
	public static ItemDefinition blueprintBaseDef;

	// Token: 0x040023FB RID: 9211
	private static List<ItemManager.ItemRemove> ItemRemoves = new List<ItemManager.ItemRemove>();

	// Token: 0x02000D28 RID: 3368
	private struct ItemRemove
	{
		// Token: 0x0400453F RID: 17727
		public global::Item item;

		// Token: 0x04004540 RID: 17728
		public float time;
	}
}
