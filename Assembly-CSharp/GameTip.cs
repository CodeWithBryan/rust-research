using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C0 RID: 1984
public class GameTip : SingletonComponent<GameTip>
{
	// Token: 0x04002BF8 RID: 11256
	public CanvasGroup canvasGroup;

	// Token: 0x04002BF9 RID: 11257
	public RustIcon icon;

	// Token: 0x04002BFA RID: 11258
	public Image background;

	// Token: 0x04002BFB RID: 11259
	public RustText text;

	// Token: 0x04002BFC RID: 11260
	public GameTip.Theme[] themes;

	// Token: 0x02000E21 RID: 3617
	public enum Styles
	{
		// Token: 0x04004953 RID: 18771
		Blue_Normal,
		// Token: 0x04004954 RID: 18772
		Red_Normal,
		// Token: 0x04004955 RID: 18773
		Blue_Long,
		// Token: 0x04004956 RID: 18774
		Blue_Short,
		// Token: 0x04004957 RID: 18775
		Server_Event
	}

	// Token: 0x02000E22 RID: 3618
	[Serializable]
	public struct Theme
	{
		// Token: 0x04004958 RID: 18776
		public Icons Icon;

		// Token: 0x04004959 RID: 18777
		public Color BackgroundColor;

		// Token: 0x0400495A RID: 18778
		public Color ForegroundColor;

		// Token: 0x0400495B RID: 18779
		public float duration;
	}
}
