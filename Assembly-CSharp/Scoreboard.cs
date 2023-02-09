using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000883 RID: 2179
public class Scoreboard : MonoBehaviour, IClientComponent
{
	// Token: 0x0400304A RID: 12362
	public static Scoreboard instance;

	// Token: 0x0400304B RID: 12363
	public RustText scoreboardTitle;

	// Token: 0x0400304C RID: 12364
	public RectTransform scoreboardRootContents;

	// Token: 0x0400304D RID: 12365
	public RustText scoreLimitText;

	// Token: 0x0400304E RID: 12366
	public GameObject teamPrefab;

	// Token: 0x0400304F RID: 12367
	public GameObject columnPrefab;

	// Token: 0x04003050 RID: 12368
	public GameObject dividerPrefab;

	// Token: 0x04003051 RID: 12369
	public Color localPlayerColor;

	// Token: 0x04003052 RID: 12370
	public Color otherPlayerColor;

	// Token: 0x04003053 RID: 12371
	public Scoreboard.TeamColumn[] teamColumns;

	// Token: 0x04003054 RID: 12372
	public GameObject[] TeamPanels;

	// Token: 0x02000E45 RID: 3653
	public class TeamColumn
	{
		// Token: 0x040049D6 RID: 18902
		public GameObject nameColumn;

		// Token: 0x040049D7 RID: 18903
		public GameObject[] activeColumns;
	}
}
