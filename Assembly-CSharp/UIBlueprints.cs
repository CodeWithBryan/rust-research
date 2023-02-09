using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007EA RID: 2026
public class UIBlueprints : ListComponent<UIBlueprints>
{
	// Token: 0x04002CF0 RID: 11504
	public GameObjectRef buttonPrefab;

	// Token: 0x04002CF1 RID: 11505
	public ScrollRect scrollRect;

	// Token: 0x04002CF2 RID: 11506
	public CanvasGroup ScrollRectCanvasGroup;

	// Token: 0x04002CF3 RID: 11507
	public InputField searchField;

	// Token: 0x04002CF4 RID: 11508
	public GameObject searchFieldPlaceholder;

	// Token: 0x04002CF5 RID: 11509
	public GameObject listAvailable;

	// Token: 0x04002CF6 RID: 11510
	public GameObject listLocked;

	// Token: 0x04002CF7 RID: 11511
	public GameObject Categories;

	// Token: 0x04002CF8 RID: 11512
	public VerticalLayoutGroup CategoryVerticalLayoutGroup;

	// Token: 0x04002CF9 RID: 11513
	public BlueprintCategoryButton FavouriteCategoryButton;
}
