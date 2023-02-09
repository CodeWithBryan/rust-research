using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000898 RID: 2200
public class UIDeathScreen : SingletonComponent<UIDeathScreen>, IUIScreen
{
	// Token: 0x040030B7 RID: 12471
	public LifeInfographic previousLifeInfographic;

	// Token: 0x040030B8 RID: 12472
	public Animator screenAnimator;

	// Token: 0x040030B9 RID: 12473
	public bool fadeIn;

	// Token: 0x040030BA RID: 12474
	public Button ReportCheatButton;

	// Token: 0x040030BB RID: 12475
	public MapView View;

	// Token: 0x040030BC RID: 12476
	public List<SleepingBagButton> sleepingBagButtons = new List<SleepingBagButton>();

	// Token: 0x040030BD RID: 12477
	public UIDeathScreen.RespawnColourScheme[] RespawnColourSchemes;

	// Token: 0x040030BE RID: 12478
	public GameObject RespawnScrollGradient;

	// Token: 0x040030BF RID: 12479
	public ScrollRect RespawnScrollRect;

	// Token: 0x040030C0 RID: 12480
	public ExpandedLifeStats ExpandedStats;

	// Token: 0x040030C1 RID: 12481
	public CanvasGroup StreamerModeContainer;

	// Token: 0x02000E4C RID: 3660
	[Serializable]
	public struct RespawnColourScheme
	{
		// Token: 0x040049EC RID: 18924
		public Color BackgroundColour;

		// Token: 0x040049ED RID: 18925
		public Color CircleRimColour;

		// Token: 0x040049EE RID: 18926
		public Color CircleFillColour;
	}
}
