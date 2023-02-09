using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000728 RID: 1832
[CreateAssetMenu(menuName = "Rust/Loot Spawn")]
public class LootSpawn : ScriptableObject
{
	// Token: 0x060032CD RID: 13005 RVA: 0x00139E42 File Offset: 0x00138042
	public ItemDefinition GetBlueprintBaseDef()
	{
		return ItemManager.FindItemDefinition("blueprintbase");
	}

	// Token: 0x060032CE RID: 13006 RVA: 0x00139E50 File Offset: 0x00138050
	public void SpawnIntoContainer(ItemContainer container)
	{
		if (this.subSpawn != null && this.subSpawn.Length != 0)
		{
			this.SubCategoryIntoContainer(container);
			return;
		}
		if (this.items != null)
		{
			foreach (ItemAmountRanged itemAmountRanged in this.items)
			{
				if (itemAmountRanged != null)
				{
					Item item2;
					if (itemAmountRanged.itemDef.spawnAsBlueprint)
					{
						ItemDefinition blueprintBaseDef = this.GetBlueprintBaseDef();
						if (blueprintBaseDef == null)
						{
							goto IL_EB;
						}
						Item item = ItemManager.Create(blueprintBaseDef, 1, 0UL);
						item.blueprintTarget = itemAmountRanged.itemDef.itemid;
						item2 = item;
					}
					else
					{
						item2 = ItemManager.CreateByItemID(itemAmountRanged.itemid, (int)itemAmountRanged.GetAmount(), 0UL);
					}
					if (item2 != null)
					{
						item2.OnVirginSpawn();
						if (!item2.MoveToContainer(container, -1, true, false, null, true))
						{
							if (container.playerOwner)
							{
								item2.Drop(container.playerOwner.GetDropPosition(), container.playerOwner.GetDropVelocity(), default(Quaternion));
							}
							else
							{
								item2.Remove(0f);
							}
						}
					}
				}
				IL_EB:;
			}
		}
	}

	// Token: 0x060032CF RID: 13007 RVA: 0x00139F58 File Offset: 0x00138158
	private void SubCategoryIntoContainer(ItemContainer container)
	{
		int num = this.subSpawn.Sum((LootSpawn.Entry x) => x.weight);
		int num2 = UnityEngine.Random.Range(0, num);
		for (int i = 0; i < this.subSpawn.Length; i++)
		{
			if (!(this.subSpawn[i].category == null))
			{
				num -= this.subSpawn[i].weight;
				if (num2 >= num)
				{
					for (int j = 0; j < 1 + this.subSpawn[i].extraSpawns; j++)
					{
						this.subSpawn[i].category.SpawnIntoContainer(container);
					}
					return;
				}
			}
		}
		Debug.LogWarning("SubCategoryIntoContainer: This should never happen!", this);
	}

	// Token: 0x0400291D RID: 10525
	public ItemAmountRanged[] items;

	// Token: 0x0400291E RID: 10526
	public LootSpawn.Entry[] subSpawn;

	// Token: 0x02000E0A RID: 3594
	[Serializable]
	public struct Entry
	{
		// Token: 0x040048D0 RID: 18640
		[Tooltip("If this category is chosen, we will spawn 1+ this amount")]
		public int extraSpawns;

		// Token: 0x040048D1 RID: 18641
		[Tooltip("If a subcategory exists we'll choose from there instead of any items specified")]
		public LootSpawn category;

		// Token: 0x040048D2 RID: 18642
		[Tooltip("The higher this number, the more likely this will be chosen")]
		public int weight;
	}
}
