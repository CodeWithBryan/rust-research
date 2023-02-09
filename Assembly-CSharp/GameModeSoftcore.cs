using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020004EA RID: 1258
public class GameModeSoftcore : GameModeVanilla
{
	// Token: 0x060027E7 RID: 10215 RVA: 0x000F4652 File Offset: 0x000F2852
	protected override void OnCreated()
	{
		base.OnCreated();
		SingletonComponent<ServerMgr>.Instance.CreateImportantEntity<ReclaimManager>(this.reclaimManagerPrefab.resourcePath);
	}

	// Token: 0x060027E8 RID: 10216 RVA: 0x000F4670 File Offset: 0x000F2870
	public void AddFractionOfContainer(ItemContainer from, ref List<Item> to, float fraction = 1f, bool takeLastItem = false)
	{
		if (from.itemList.Count == 0)
		{
			return;
		}
		fraction = Mathf.Clamp01(fraction);
		int count = from.itemList.Count;
		float num = Mathf.Ceil((float)count * fraction);
		if (count == 1 && num == 1f && !takeLastItem)
		{
			return;
		}
		List<int> list = Facepunch.Pool.GetList<int>();
		for (int i = 0; i < from.capacity; i++)
		{
			if (from.GetSlot(i) != null)
			{
				list.Add(i);
			}
		}
		if (list.Count == 0)
		{
			Facepunch.Pool.FreeList<int>(ref list);
			return;
		}
		int num2 = 0;
		while ((float)num2 < num)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			Item item = from.GetSlot(list[index]);
			if (item.MaxStackable() > 1)
			{
				foreach (Item item2 in from.itemList)
				{
					if (item2.info == item.info && item2.amount < item.amount && !to.Contains(item2))
					{
						item = item2;
						for (int j = 0; j < list.Count; j++)
						{
							if (list[j] == item2.position)
							{
								index = j;
							}
						}
					}
				}
			}
			to.Add(item);
			list.RemoveAt(index);
			num2++;
		}
		Facepunch.Pool.FreeList<int>(ref list);
	}

	// Token: 0x060027E9 RID: 10217 RVA: 0x000F47E4 File Offset: 0x000F29E4
	public List<Item> RemoveItemsFrom(ItemContainer itemContainer, ItemAmount[] types)
	{
		List<Item> list = Facepunch.Pool.GetList<Item>();
		foreach (ItemAmount itemAmount in types)
		{
			int num = 0;
			while ((float)num < itemAmount.amount)
			{
				Item item = itemContainer.FindItemByItemID(itemAmount.itemDef.itemid);
				if (item != null)
				{
					item.RemoveFromContainer();
					list.Add(item);
				}
				num++;
			}
		}
		return list;
	}

	// Token: 0x060027EA RID: 10218 RVA: 0x000F4848 File Offset: 0x000F2A48
	public void ReturnItemsTo(ref List<Item> source, ItemContainer itemContainer)
	{
		foreach (Item item in source)
		{
			item.MoveToContainer(itemContainer, -1, true, false, null, true);
		}
		Facepunch.Pool.FreeList<Item>(ref source);
	}

	// Token: 0x060027EB RID: 10219 RVA: 0x000F48A4 File Offset: 0x000F2AA4
	public override void OnPlayerDeath(BasePlayer instigator, BasePlayer victim, HitInfo deathInfo = null)
	{
		if (victim != null && !victim.IsNpc)
		{
			this.SetInventoryLocked(victim, false);
			int newID = 0;
			if (ReclaimManager.instance == null)
			{
				Debug.LogError("No reclaim manage for softcore");
				return;
			}
			List<Item> list = Facepunch.Pool.GetList<Item>();
			List<Item> list2 = this.RemoveItemsFrom(victim.inventory.containerBelt, this.startingGear);
			this.AddFractionOfContainer(victim.inventory.containerBelt, ref list, GameModeSoftcore.reclaim_fraction_belt, false);
			this.AddFractionOfContainer(victim.inventory.containerWear, ref list, GameModeSoftcore.reclaim_fraction_wear, false);
			this.AddFractionOfContainer(victim.inventory.containerMain, ref list, GameModeSoftcore.reclaim_fraction_main, false);
			if (list.Count > 0)
			{
				newID = ReclaimManager.instance.AddPlayerReclaim(victim.userID, list, (instigator == null) ? 0UL : instigator.userID, (instigator == null) ? "" : instigator.displayName, -1);
			}
			this.ReturnItemsTo(ref list2, victim.inventory.containerBelt);
			if (list.Count > 0)
			{
				Vector3 pos = victim.transform.position + Vector3.up * 0.25f;
				Quaternion rot = Quaternion.Euler(0f, victim.transform.eulerAngles.y, 0f);
				ReclaimBackpack component = GameManager.server.CreateEntity(this.reclaimBackpackPrefab.resourcePath, pos, rot, true).GetComponent<ReclaimBackpack>();
				component.InitForPlayer(victim.userID, newID);
				component.Spawn();
			}
			Facepunch.Pool.FreeList<Item>(ref list);
		}
		base.OnPlayerDeath(instigator, victim, deathInfo);
	}

	// Token: 0x060027EC RID: 10220 RVA: 0x000F4A35 File Offset: 0x000F2C35
	public override void OnPlayerRespawn(BasePlayer player)
	{
		base.OnPlayerRespawn(player);
		player.ShowToast(GameTip.Styles.Blue_Long, GameModeSoftcore.ReclaimToast, Array.Empty<string>());
	}

	// Token: 0x060027ED RID: 10221 RVA: 0x0002E34D File Offset: 0x0002C54D
	public override SleepingBag[] FindSleepingBagsForPlayer(ulong playerID, bool ignoreTimers)
	{
		return SleepingBag.FindForPlayer(playerID, ignoreTimers);
	}

	// Token: 0x060027EE RID: 10222 RVA: 0x000F4A50 File Offset: 0x000F2C50
	public override float CorpseRemovalTime(BaseCorpse corpse)
	{
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
		{
			if (monumentInfo != null && monumentInfo.IsSafeZone && monumentInfo.Bounds.Contains(corpse.transform.position))
			{
				return 30f;
			}
		}
		return Server.corpsedespawn;
	}

	// Token: 0x060027EF RID: 10223 RVA: 0x000F4AD8 File Offset: 0x000F2CD8
	public void SetInventoryLocked(BasePlayer player, bool wantsLocked)
	{
		player.inventory.containerMain.SetLocked(wantsLocked);
		player.inventory.containerBelt.SetLocked(wantsLocked);
		player.inventory.containerWear.SetLocked(wantsLocked);
	}

	// Token: 0x060027F0 RID: 10224 RVA: 0x000F4B0D File Offset: 0x000F2D0D
	public override void OnPlayerWounded(BasePlayer instigator, BasePlayer victim, HitInfo info)
	{
		base.OnPlayerWounded(instigator, victim, info);
		this.SetInventoryLocked(victim, true);
	}

	// Token: 0x060027F1 RID: 10225 RVA: 0x000F4B20 File Offset: 0x000F2D20
	public override void OnPlayerRevived(BasePlayer instigator, BasePlayer victim)
	{
		this.SetInventoryLocked(victim, false);
		base.OnPlayerRevived(instigator, victim);
	}

	// Token: 0x060027F2 RID: 10226 RVA: 0x000F4B32 File Offset: 0x000F2D32
	public override bool CanMoveItemsFrom(PlayerInventory inv, BaseEntity source, Item item)
	{
		if (item.parent != null && item.parent.HasFlag(ItemContainer.Flag.IsPlayer))
		{
			return !item.parent.IsLocked();
		}
		return base.CanMoveItemsFrom(inv, source, item);
	}

	// Token: 0x0400201F RID: 8223
	public GameObjectRef reclaimManagerPrefab;

	// Token: 0x04002020 RID: 8224
	public GameObjectRef reclaimBackpackPrefab;

	// Token: 0x04002021 RID: 8225
	public static readonly Translate.Phrase ReclaimToast = new Translate.Phrase("softcore.reclaim", "You can reclaim some of your lost items by visiting the Outpost or Bandit Town.");

	// Token: 0x04002022 RID: 8226
	public ItemAmount[] startingGear;

	// Token: 0x04002023 RID: 8227
	[ServerVar]
	public static float reclaim_fraction_belt = 0.5f;

	// Token: 0x04002024 RID: 8228
	[ServerVar]
	public static float reclaim_fraction_wear = 0f;

	// Token: 0x04002025 RID: 8229
	[ServerVar]
	public static float reclaim_fraction_main = 0.5f;
}
