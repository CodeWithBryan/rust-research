using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000791 RID: 1937
public class BlackjackSmallScreenUI : FacepunchBehaviour
{
	// Token: 0x04002A6E RID: 10862
	[SerializeField]
	private Canvas notInGameDisplay;

	// Token: 0x04002A6F RID: 10863
	[SerializeField]
	private Canvas inGameDisplay;

	// Token: 0x04002A70 RID: 10864
	[SerializeField]
	private RustText cardCountText;

	// Token: 0x04002A71 RID: 10865
	[SerializeField]
	private RustText betText;

	// Token: 0x04002A72 RID: 10866
	[SerializeField]
	private RustText splitBetText;

	// Token: 0x04002A73 RID: 10867
	[SerializeField]
	private RustText insuranceText;

	// Token: 0x04002A74 RID: 10868
	[SerializeField]
	private RustText bankText;

	// Token: 0x04002A75 RID: 10869
	[SerializeField]
	private RustText splitText;

	// Token: 0x04002A76 RID: 10870
	[SerializeField]
	private Canvas infoTextCanvas;

	// Token: 0x04002A77 RID: 10871
	[SerializeField]
	private RustText inGameText;

	// Token: 0x04002A78 RID: 10872
	[SerializeField]
	private RustText notInGameText;

	// Token: 0x04002A79 RID: 10873
	[SerializeField]
	private HorizontalLayoutGroup cardsLayout;

	// Token: 0x04002A7A RID: 10874
	[SerializeField]
	private BlackjackScreenCardUI[] cards;

	// Token: 0x04002A7B RID: 10875
	[SerializeField]
	private BlackjackScreenInputUI[] inputs;

	// Token: 0x04002A7C RID: 10876
	[SerializeField]
	private Translate.Phrase phraseBust;

	// Token: 0x04002A7D RID: 10877
	[SerializeField]
	private Translate.Phrase phraseBet;

	// Token: 0x04002A7E RID: 10878
	[SerializeField]
	private Translate.Phrase phrasePlaceYourBet;

	// Token: 0x04002A7F RID: 10879
	[SerializeField]
	private Translate.Phrase phraseStandoff;

	// Token: 0x04002A80 RID: 10880
	[SerializeField]
	private Translate.Phrase phraseYouWin;

	// Token: 0x04002A81 RID: 10881
	[SerializeField]
	private Translate.Phrase phraseYouLose;

	// Token: 0x04002A82 RID: 10882
	[SerializeField]
	private Translate.Phrase phraseWaitingForOtherPlayers;

	// Token: 0x04002A83 RID: 10883
	[SerializeField]
	private Translate.Phrase phraseAddFunds;

	// Token: 0x04002A84 RID: 10884
	[SerializeField]
	private Translate.Phrase phraseWaitingForPlayer;

	// Token: 0x04002A85 RID: 10885
	[SerializeField]
	private Translate.Phrase phraseSplitStored;

	// Token: 0x04002A86 RID: 10886
	[SerializeField]
	private Translate.Phrase phraseSplitActive;

	// Token: 0x04002A87 RID: 10887
	[SerializeField]
	private Translate.Phrase phraseHand;
}
