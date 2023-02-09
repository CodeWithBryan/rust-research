using System;
using Facepunch;
using ProtoBuf;

// Token: 0x0200041F RID: 1055
public class PlayerBlueprints : EntityComponent<global::BasePlayer>
{
	// Token: 0x06002311 RID: 8977 RVA: 0x000DF464 File Offset: 0x000DD664
	internal void Reset()
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		if (persistantPlayerInfo.unlockedItems != null)
		{
			persistantPlayerInfo.unlockedItems.Clear();
		}
		else
		{
			persistantPlayerInfo.unlockedItems = Pool.GetList<int>();
		}
		base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x000DF4B8 File Offset: 0x000DD6B8
	internal void UnlockAll()
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		foreach (ItemBlueprint itemBlueprint in ItemManager.bpList)
		{
			if (itemBlueprint.userCraftable && !itemBlueprint.defaultBlueprint && !persistantPlayerInfo.unlockedItems.Contains(itemBlueprint.targetItem.itemid))
			{
				persistantPlayerInfo.unlockedItems.Add(itemBlueprint.targetItem.itemid);
			}
		}
		base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
		base.baseEntity.SendNetworkUpdateImmediate(false);
		base.baseEntity.ClientRPCPlayer<int>(null, base.baseEntity, "UnlockedBlueprint", 0);
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x000DF580 File Offset: 0x000DD780
	public bool IsUnlocked(ItemDefinition itemDef)
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		return persistantPlayerInfo.unlockedItems != null && persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid);
	}

	// Token: 0x06002314 RID: 8980 RVA: 0x000DF5B4 File Offset: 0x000DD7B4
	public void Unlock(ItemDefinition itemDef)
	{
		PersistantPlayer persistantPlayerInfo = base.baseEntity.PersistantPlayerInfo;
		if (!persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid))
		{
			persistantPlayerInfo.unlockedItems.Add(itemDef.itemid);
			base.baseEntity.PersistantPlayerInfo = persistantPlayerInfo;
			base.baseEntity.SendNetworkUpdateImmediate(false);
			base.baseEntity.ClientRPCPlayer<int>(null, base.baseEntity, "UnlockedBlueprint", itemDef.itemid);
			base.baseEntity.stats.Add("blueprint_studied", 1, (Stats)5);
		}
	}

	// Token: 0x06002315 RID: 8981 RVA: 0x000DF640 File Offset: 0x000DD840
	public bool HasUnlocked(ItemDefinition targetItem)
	{
		if (targetItem.Blueprint)
		{
			if (targetItem.Blueprint.NeedsSteamItem)
			{
				if (targetItem.steamItem != null && !this.steamInventory.HasItem(targetItem.steamItem.id))
				{
					return false;
				}
				if (targetItem.steamItem == null)
				{
					bool flag = false;
					foreach (ItemSkinDirectory.Skin skin in targetItem.skins)
					{
						if (this.steamInventory.HasItem(skin.id))
						{
							flag = true;
							break;
						}
					}
					if (!flag && targetItem.skins2 != null)
					{
						foreach (IPlayerItemDefinition playerItemDefinition in targetItem.skins2)
						{
							if (this.steamInventory.HasItem(playerItemDefinition.DefinitionId))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				return true;
			}
			else if (targetItem.Blueprint.NeedsSteamDLC && targetItem.steamDlc != null && targetItem.steamDlc.HasLicense(base.baseEntity.userID))
			{
				return true;
			}
		}
		int[] defaultBlueprints = ItemManager.defaultBlueprints;
		for (int i = 0; i < defaultBlueprints.Length; i++)
		{
			if (defaultBlueprints[i] == targetItem.itemid)
			{
				return true;
			}
		}
		return base.baseEntity.isServer && this.IsUnlocked(targetItem);
	}

	// Token: 0x06002316 RID: 8982 RVA: 0x000DF790 File Offset: 0x000DD990
	public bool CanCraft(int itemid, int skinItemId, ulong playerId)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemid);
		return !(itemDefinition == null) && (skinItemId == 0 || this.CheckSkinOwnership(skinItemId, playerId)) && base.baseEntity.currentCraftLevel >= (float)itemDefinition.Blueprint.workbenchLevelRequired && this.HasUnlocked(itemDefinition);
	}

	// Token: 0x06002317 RID: 8983 RVA: 0x000DF7E8 File Offset: 0x000DD9E8
	public bool CheckSkinOwnership(int skinItemId, ulong playerId)
	{
		ItemSkinDirectory.Skin skin = ItemSkinDirectory.FindByInventoryDefinitionId(skinItemId);
		return (skin.invItem != null && skin.invItem.HasUnlocked(playerId)) || this.steamInventory.HasItem(skinItemId);
	}

	// Token: 0x04001B96 RID: 7062
	public SteamInventory steamInventory;
}
