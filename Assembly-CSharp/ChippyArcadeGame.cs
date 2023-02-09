using System;
using UnityEngine;

// Token: 0x02000140 RID: 320
public class ChippyArcadeGame : BaseArcadeGame
{
	// Token: 0x04000ED5 RID: 3797
	public ChippyMainCharacter mainChar;

	// Token: 0x04000ED6 RID: 3798
	public SpriteArcadeEntity mainCharAim;

	// Token: 0x04000ED7 RID: 3799
	public ChippyBoss currentBoss;

	// Token: 0x04000ED8 RID: 3800
	public ChippyBoss[] bossPrefabs;

	// Token: 0x04000ED9 RID: 3801
	public SpriteArcadeEntity mainMenuLogo;

	// Token: 0x04000EDA RID: 3802
	public Transform respawnPoint;

	// Token: 0x04000EDB RID: 3803
	public Vector2 mouseAim = new Vector2(0f, 1f);

	// Token: 0x04000EDC RID: 3804
	public TextArcadeEntity levelIndicator;

	// Token: 0x04000EDD RID: 3805
	public TextArcadeEntity gameOverIndicator;

	// Token: 0x04000EDE RID: 3806
	public TextArcadeEntity playGameButton;

	// Token: 0x04000EDF RID: 3807
	public TextArcadeEntity highScoresButton;

	// Token: 0x04000EE0 RID: 3808
	public bool OnMainMenu;

	// Token: 0x04000EE1 RID: 3809
	public bool GameActive;

	// Token: 0x04000EE2 RID: 3810
	public int level;

	// Token: 0x04000EE3 RID: 3811
	public TextArcadeEntity[] scoreDisplays;

	// Token: 0x04000EE4 RID: 3812
	public MenuButtonArcadeEntity[] mainMenuButtons;

	// Token: 0x04000EE5 RID: 3813
	public int selectedButtonIndex;

	// Token: 0x04000EE6 RID: 3814
	public bool OnHighScores;
}
