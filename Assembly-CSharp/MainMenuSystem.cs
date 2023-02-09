using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200084A RID: 2122
public class MainMenuSystem : SingletonComponent<MainMenuSystem>
{
	// Token: 0x04002F41 RID: 12097
	public static bool isOpen = true;

	// Token: 0x04002F42 RID: 12098
	public static Action OnOpenStateChanged;

	// Token: 0x04002F43 RID: 12099
	public RustButton SessionButton;

	// Token: 0x04002F44 RID: 12100
	public GameObject SessionPanel;

	// Token: 0x04002F45 RID: 12101
	public GameObject NewsStoriesAlert;

	// Token: 0x04002F46 RID: 12102
	public GameObject ItemStoreAlert;

	// Token: 0x04002F47 RID: 12103
	public GameObject CompanionAlert;

	// Token: 0x04002F48 RID: 12104
	public GameObject DemoBrowser;

	// Token: 0x04002F49 RID: 12105
	public GameObject DemoBrowserButton;

	// Token: 0x04002F4A RID: 12106
	public GameObject SuicideButton;

	// Token: 0x04002F4B RID: 12107
	public GameObject EndDemoButton;

	// Token: 0x04002F4C RID: 12108
	public GameObject ReflexModeOption;

	// Token: 0x04002F4D RID: 12109
	public GameObject ReflexLatencyMarkerOption;
}
