using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000829 RID: 2089
public class UIMixingTableItem : MonoBehaviour
{
	// Token: 0x06003486 RID: 13446 RVA: 0x0013E25C File Offset: 0x0013C45C
	public void Init(Recipe recipe)
	{
		if (recipe == null)
		{
			return;
		}
		this.ItemIcon.sprite = recipe.DisplayIcon;
		this.TextItemNameAndQuantity.text = recipe.ProducedItemCount.ToString() + " x " + recipe.DisplayName;
		this.ItemTooltip.Text = recipe.DisplayDescription;
		for (int i = 0; i < this.Ingredients.Length; i++)
		{
			if (i >= recipe.Ingredients.Length)
			{
				this.Ingredients[i].InitBlank();
			}
			else
			{
				this.Ingredients[i].Init(recipe.Ingredients[i]);
			}
		}
	}

	// Token: 0x04002E4E RID: 11854
	public Image ItemIcon;

	// Token: 0x04002E4F RID: 11855
	public Tooltip ItemTooltip;

	// Token: 0x04002E50 RID: 11856
	public RustText TextItemNameAndQuantity;

	// Token: 0x04002E51 RID: 11857
	public UIMixingTableItemIngredient[] Ingredients;
}
