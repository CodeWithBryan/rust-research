using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000802 RID: 2050
public class LootPanel : MonoBehaviour
{
	// Token: 0x04002D6E RID: 11630
	public Text Title;

	// Token: 0x04002D6F RID: 11631
	public RustText TitleText;

	// Token: 0x04002D70 RID: 11632
	public bool hideInvalidIcons;

	// Token: 0x04002D71 RID: 11633
	[Tooltip("Only needed if hideInvalidIcons is true")]
	public CanvasGroup canvasGroup;

	// Token: 0x02000E28 RID: 3624
	public interface IHasLootPanel
	{
		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x06004FFB RID: 20475
		Translate.Phrase LootPanelTitle { get; }
	}
}
