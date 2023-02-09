using System;
using System.Collections.Generic;

// Token: 0x0200072E RID: 1838
public static class RecipeDictionary
{
	// Token: 0x060032EB RID: 13035 RVA: 0x0013A900 File Offset: 0x00138B00
	public static void CacheRecipes(RecipeList recipeList)
	{
		if (recipeList == null)
		{
			return;
		}
		Dictionary<int, List<Recipe>> dictionary;
		if (RecipeDictionary.recipeListsDict.TryGetValue(recipeList.FilenameStringId, out dictionary))
		{
			return;
		}
		Dictionary<int, List<Recipe>> dictionary2 = new Dictionary<int, List<Recipe>>();
		RecipeDictionary.recipeListsDict.Add(recipeList.FilenameStringId, dictionary2);
		foreach (Recipe recipe in recipeList.Recipes)
		{
			List<Recipe> list = null;
			if (!dictionary2.TryGetValue(recipe.Ingredients[0].Ingredient.itemid, out list))
			{
				list = new List<Recipe>();
				dictionary2.Add(recipe.Ingredients[0].Ingredient.itemid, list);
			}
			list.Add(recipe);
		}
	}

	// Token: 0x060032EC RID: 13036 RVA: 0x0013A9B0 File Offset: 0x00138BB0
	public static Recipe GetMatchingRecipeAndQuantity(RecipeList recipeList, List<Item> orderedIngredients, out int quantity)
	{
		quantity = 0;
		if (recipeList == null)
		{
			return null;
		}
		if (orderedIngredients == null || orderedIngredients.Count == 0)
		{
			return null;
		}
		List<Recipe> recipesByFirstIngredient = RecipeDictionary.GetRecipesByFirstIngredient(recipeList, orderedIngredients[0]);
		if (recipesByFirstIngredient == null)
		{
			return null;
		}
		foreach (Recipe recipe in recipesByFirstIngredient)
		{
			if (!(recipe == null) && recipe.Ingredients.Length == orderedIngredients.Count)
			{
				bool flag = true;
				int num = int.MaxValue;
				int num2 = 0;
				foreach (Recipe.RecipeIngredient recipeIngredient in recipe.Ingredients)
				{
					Item item = orderedIngredients[num2];
					if (recipeIngredient.Ingredient != item.info || item.amount < recipeIngredient.Count)
					{
						flag = false;
						break;
					}
					int num3 = item.amount / recipeIngredient.Count;
					if (num2 == 0)
					{
						num = num3;
					}
					else if (num3 < num)
					{
						num = num3;
					}
					num2++;
				}
				if (flag)
				{
					quantity = num;
					if (quantity > 1 && !recipe.CanQueueMultiple)
					{
						quantity = 1;
					}
					return recipe;
				}
			}
		}
		return null;
	}

	// Token: 0x060032ED RID: 13037 RVA: 0x0013AAFC File Offset: 0x00138CFC
	private static List<Recipe> GetRecipesByFirstIngredient(RecipeList recipeList, Item firstIngredient)
	{
		if (recipeList == null)
		{
			return null;
		}
		if (firstIngredient == null)
		{
			return null;
		}
		Dictionary<int, List<Recipe>> dictionary;
		if (!RecipeDictionary.recipeListsDict.TryGetValue(recipeList.FilenameStringId, out dictionary))
		{
			RecipeDictionary.CacheRecipes(recipeList);
		}
		if (dictionary == null)
		{
			return null;
		}
		List<Recipe> result;
		if (!dictionary.TryGetValue(firstIngredient.info.itemid, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x060032EE RID: 13038 RVA: 0x0013AB50 File Offset: 0x00138D50
	public static bool ValidIngredientForARecipe(Item ingredient, RecipeList recipeList)
	{
		if (ingredient == null)
		{
			return false;
		}
		if (recipeList == null)
		{
			return false;
		}
		foreach (Recipe recipe in recipeList.Recipes)
		{
			if (!(recipe == null) && recipe.ContainsItem(ingredient))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002944 RID: 10564
	private static Dictionary<uint, Dictionary<int, List<Recipe>>> recipeListsDict = new Dictionary<uint, Dictionary<int, List<Recipe>>>();
}
