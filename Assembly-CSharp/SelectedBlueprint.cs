using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E9 RID: 2025
public class SelectedBlueprint : SingletonComponent<SelectedBlueprint>, IInventoryChanged
{
	// Token: 0x170003FC RID: 1020
	// (get) Token: 0x0600342D RID: 13357 RVA: 0x0013D50C File Offset: 0x0013B70C
	public static bool isOpen
	{
		get
		{
			return !(SingletonComponent<SelectedBlueprint>.Instance == null) && SingletonComponent<SelectedBlueprint>.Instance.blueprint != null;
		}
	}

	// Token: 0x04002CE0 RID: 11488
	public ItemBlueprint blueprint;

	// Token: 0x04002CE1 RID: 11489
	public InputField craftAmountText;

	// Token: 0x04002CE2 RID: 11490
	public GameObject ingredientGrid;

	// Token: 0x04002CE3 RID: 11491
	public IconSkinPicker skinPicker;

	// Token: 0x04002CE4 RID: 11492
	public Image iconImage;

	// Token: 0x04002CE5 RID: 11493
	public RustText titleText;

	// Token: 0x04002CE6 RID: 11494
	public RustText descriptionText;

	// Token: 0x04002CE7 RID: 11495
	public CanvasGroup CraftArea;

	// Token: 0x04002CE8 RID: 11496
	public Button CraftButton;

	// Token: 0x04002CE9 RID: 11497
	public RustText CraftingTime;

	// Token: 0x04002CEA RID: 11498
	public RustText CraftingAmount;

	// Token: 0x04002CEB RID: 11499
	public Sprite FavouriteOnSprite;

	// Token: 0x04002CEC RID: 11500
	public Sprite FavouriteOffSprite;

	// Token: 0x04002CED RID: 11501
	public Image FavouriteButtonStatusMarker;

	// Token: 0x04002CEE RID: 11502
	public GameObject[] workbenchReqs;

	// Token: 0x04002CEF RID: 11503
	private ItemInformationPanel[] informationPanels;
}
