using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200082A RID: 2090
public class UIMixingTableItemIngredient : MonoBehaviour
{
	// Token: 0x06003488 RID: 13448 RVA: 0x0013E304 File Offset: 0x0013C504
	public void Init(Recipe.RecipeIngredient ingredient)
	{
		this.ItemIcon.sprite = ingredient.Ingredient.iconSprite;
		this.ItemCount.text = ingredient.Count.ToString();
		this.ItemIcon.enabled = true;
		this.ItemCount.enabled = true;
		this.ToolTip.Text = ingredient.Count.ToString() + " x " + ingredient.Ingredient.displayName.translated;
		this.ToolTip.enabled = true;
	}

	// Token: 0x06003489 RID: 13449 RVA: 0x0013E393 File Offset: 0x0013C593
	public void InitBlank()
	{
		this.ItemIcon.enabled = false;
		this.ItemCount.enabled = false;
		this.ToolTip.enabled = false;
	}

	// Token: 0x04002E52 RID: 11858
	public Image ItemIcon;

	// Token: 0x04002E53 RID: 11859
	public Text ItemCount;

	// Token: 0x04002E54 RID: 11860
	public Tooltip ToolTip;
}
