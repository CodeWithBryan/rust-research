using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000897 RID: 2199
public class UIConversationScreen : SingletonComponent<UIConversationScreen>, IUIScreen
{
	// Token: 0x040030AF RID: 12463
	public NeedsCursor needsCursor;

	// Token: 0x040030B0 RID: 12464
	public RectTransform conversationPanel;

	// Token: 0x040030B1 RID: 12465
	public RustText conversationSpeechBody;

	// Token: 0x040030B2 RID: 12466
	public RustText conversationProviderName;

	// Token: 0x040030B3 RID: 12467
	public RustButton[] responseButtons;

	// Token: 0x040030B4 RID: 12468
	public RectTransform letterBoxTop;

	// Token: 0x040030B5 RID: 12469
	public RectTransform letterBoxBottom;

	// Token: 0x040030B6 RID: 12470
	protected CanvasGroup canvasGroup;
}
