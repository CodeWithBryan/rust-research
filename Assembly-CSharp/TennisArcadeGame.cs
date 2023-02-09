using System;
using UnityEngine;

// Token: 0x02000149 RID: 329
public class TennisArcadeGame : BaseArcadeGame
{
	// Token: 0x04000F01 RID: 3841
	public ArcadeEntity paddle1;

	// Token: 0x04000F02 RID: 3842
	public ArcadeEntity paddle2;

	// Token: 0x04000F03 RID: 3843
	public ArcadeEntity ball;

	// Token: 0x04000F04 RID: 3844
	public Transform paddle1Origin;

	// Token: 0x04000F05 RID: 3845
	public Transform paddle2Origin;

	// Token: 0x04000F06 RID: 3846
	public Transform paddle1Goal;

	// Token: 0x04000F07 RID: 3847
	public Transform paddle2Goal;

	// Token: 0x04000F08 RID: 3848
	public Transform ballSpawn;

	// Token: 0x04000F09 RID: 3849
	public float maxScore = 5f;

	// Token: 0x04000F0A RID: 3850
	public ArcadeEntity[] paddle1ScoreNodes;

	// Token: 0x04000F0B RID: 3851
	public ArcadeEntity[] paddle2ScoreNodes;

	// Token: 0x04000F0C RID: 3852
	public int paddle1Score;

	// Token: 0x04000F0D RID: 3853
	public int paddle2Score;

	// Token: 0x04000F0E RID: 3854
	public float sensitivity = 1f;

	// Token: 0x04000F0F RID: 3855
	public ArcadeEntity logo;

	// Token: 0x04000F10 RID: 3856
	public bool OnMainMenu;

	// Token: 0x04000F11 RID: 3857
	public bool GameActive;
}
