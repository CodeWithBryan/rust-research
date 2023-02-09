using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200078E RID: 1934
public class BlackjackMainScreenUI : FacepunchBehaviour
{
	// Token: 0x04002A4F RID: 10831
	[SerializeField]
	private Canvas inGameDisplay;

	// Token: 0x04002A50 RID: 10832
	[SerializeField]
	private Canvas notInGameDisplay;

	// Token: 0x04002A51 RID: 10833
	[SerializeField]
	private Sprite faceNeutral;

	// Token: 0x04002A52 RID: 10834
	[SerializeField]
	private Sprite faceShocked;

	// Token: 0x04002A53 RID: 10835
	[SerializeField]
	private Sprite faceSad;

	// Token: 0x04002A54 RID: 10836
	[SerializeField]
	private Sprite faceCool;

	// Token: 0x04002A55 RID: 10837
	[SerializeField]
	private Sprite faceHappy;

	// Token: 0x04002A56 RID: 10838
	[SerializeField]
	private Sprite faceLove;

	// Token: 0x04002A57 RID: 10839
	[SerializeField]
	private Image faceInGame;

	// Token: 0x04002A58 RID: 10840
	[SerializeField]
	private Image faceNotInGame;

	// Token: 0x04002A59 RID: 10841
	[SerializeField]
	private Sprite[] faceNeutralVariants;

	// Token: 0x04002A5A RID: 10842
	[SerializeField]
	private Sprite[] faceHalloweenVariants;

	// Token: 0x04002A5B RID: 10843
	[SerializeField]
	private RustText cardCountText;

	// Token: 0x04002A5C RID: 10844
	[SerializeField]
	private RustText payoutText;

	// Token: 0x04002A5D RID: 10845
	[SerializeField]
	private RustText insuranceText;

	// Token: 0x04002A5E RID: 10846
	[SerializeField]
	private Canvas placeBetsCanvas;

	// Token: 0x04002A5F RID: 10847
	[SerializeField]
	private HorizontalLayoutGroup cardsLayout;

	// Token: 0x04002A60 RID: 10848
	[SerializeField]
	private BlackjackScreenCardUI[] cards;

	// Token: 0x04002A61 RID: 10849
	[SerializeField]
	private Translate.Phrase phraseBust;
}
