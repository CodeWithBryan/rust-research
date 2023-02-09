using System;
using UnityEngine;

// Token: 0x0200072D RID: 1837
[CreateAssetMenu(menuName = "Rust/Recipe")]
public class Recipe : ScriptableObject
{
	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x060032E6 RID: 13030 RVA: 0x0013A807 File Offset: 0x00138A07
	public string DisplayName
	{
		get
		{
			if (this.ProducedItem != null)
			{
				return this.ProducedItem.displayName.translated;
			}
			if (this.SpawnedItem != null)
			{
				return this.SpawnedItemName;
			}
			return null;
		}
	}

	// Token: 0x170003F4 RID: 1012
	// (get) Token: 0x060032E7 RID: 13031 RVA: 0x0013A838 File Offset: 0x00138A38
	public string DisplayDescription
	{
		get
		{
			if (this.ProducedItem != null)
			{
				return this.ProducedItem.displayDescription.translated;
			}
			if (this.SpawnedItem != null)
			{
				return this.SpawnedItemDescription;
			}
			return null;
		}
	}

	// Token: 0x170003F5 RID: 1013
	// (get) Token: 0x060032E8 RID: 13032 RVA: 0x0013A869 File Offset: 0x00138A69
	public Sprite DisplayIcon
	{
		get
		{
			if (this.ProducedItem != null)
			{
				return this.ProducedItem.iconSprite;
			}
			if (this.SpawnedItem != null)
			{
				return this.SpawnedItemIcon;
			}
			return null;
		}
	}

	// Token: 0x060032E9 RID: 13033 RVA: 0x0013A898 File Offset: 0x00138A98
	public bool ContainsItem(Item item)
	{
		if (item == null)
		{
			return false;
		}
		if (this.Ingredients == null)
		{
			return false;
		}
		foreach (Recipe.RecipeIngredient recipeIngredient in this.Ingredients)
		{
			if (item.info == recipeIngredient.Ingredient)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400293A RID: 10554
	[Header("Produced Item")]
	public ItemDefinition ProducedItem;

	// Token: 0x0400293B RID: 10555
	public int ProducedItemCount = 1;

	// Token: 0x0400293C RID: 10556
	public bool CanQueueMultiple = true;

	// Token: 0x0400293D RID: 10557
	[Header("Spawned Item")]
	public GameObjectRef SpawnedItem;

	// Token: 0x0400293E RID: 10558
	public string SpawnedItemName;

	// Token: 0x0400293F RID: 10559
	public string SpawnedItemDescription;

	// Token: 0x04002940 RID: 10560
	public Sprite SpawnedItemIcon;

	// Token: 0x04002941 RID: 10561
	[Header("Misc")]
	public bool RequiresBlueprint;

	// Token: 0x04002942 RID: 10562
	public Recipe.RecipeIngredient[] Ingredients;

	// Token: 0x04002943 RID: 10563
	public float MixingDuration;

	// Token: 0x02000E10 RID: 3600
	[Serializable]
	public struct RecipeIngredient
	{
		// Token: 0x040048E0 RID: 18656
		public ItemDefinition Ingredient;

		// Token: 0x040048E1 RID: 18657
		public int Count;
	}
}
