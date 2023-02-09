using System;
using Facepunch.CardGames;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000794 RID: 1940
public class CardGameUI : UIDialog
{
	// Token: 0x04002AB8 RID: 10936
	[Header("Card Game")]
	[SerializeField]
	private CardGameUI.InfoTextUI primaryInfo;

	// Token: 0x04002AB9 RID: 10937
	[SerializeField]
	private CardGameUI.InfoTextUI secondaryInfo;

	// Token: 0x04002ABA RID: 10938
	[SerializeField]
	private CardGameUI.InfoTextUI playerLeaveInfo;

	// Token: 0x04002ABB RID: 10939
	[SerializeField]
	private GameObject playingUI;

	// Token: 0x04002ABC RID: 10940
	[SerializeField]
	private CardGameUI.PlayingCardImage[] cardImages;

	// Token: 0x04002ABD RID: 10941
	[SerializeField]
	private CardInputWidget[] inputWidgets;

	// Token: 0x04002ABE RID: 10942
	[SerializeField]
	private RustSlider dismountProgressSlider;

	// Token: 0x04002ABF RID: 10943
	[SerializeField]
	private Translate.Phrase phraseLoading;

	// Token: 0x04002AC0 RID: 10944
	[SerializeField]
	private Translate.Phrase phraseWaitingForNextRound;

	// Token: 0x04002AC1 RID: 10945
	[SerializeField]
	private Translate.Phrase phraseNotEnoughPlayers;

	// Token: 0x04002AC2 RID: 10946
	[SerializeField]
	private Translate.Phrase phrasePlayerLeftGame;

	// Token: 0x04002AC3 RID: 10947
	[SerializeField]
	private Translate.Phrase phraseNotEnoughBuyIn;

	// Token: 0x04002AC4 RID: 10948
	[SerializeField]
	private Translate.Phrase phraseTooMuchBuyIn;

	// Token: 0x04002AC5 RID: 10949
	public Translate.Phrase phraseYourTurn;

	// Token: 0x04002AC6 RID: 10950
	public Translate.Phrase phraseYouWinTheRound;

	// Token: 0x04002AC7 RID: 10951
	public Translate.Phrase phraseRoundWinner;

	// Token: 0x04002AC8 RID: 10952
	public Translate.Phrase phraseRoundWinners;

	// Token: 0x04002AC9 RID: 10953
	public Translate.Phrase phraseScrapWon;

	// Token: 0x04002ACA RID: 10954
	public Translate.Phrase phraseScrapReturned;

	// Token: 0x04002ACB RID: 10955
	public Translate.Phrase phraseChangeBetAmount;

	// Token: 0x04002ACC RID: 10956
	public Translate.Phrase phraseBet;

	// Token: 0x04002ACD RID: 10957
	public Translate.Phrase phraseBetAdd;

	// Token: 0x04002ACE RID: 10958
	public Translate.Phrase phraseAllIn;

	// Token: 0x04002ACF RID: 10959
	public GameObject amountChangeRoot;

	// Token: 0x04002AD0 RID: 10960
	public RustText amountChangeText;

	// Token: 0x04002AD1 RID: 10961
	public Color colourNeutralUI;

	// Token: 0x04002AD2 RID: 10962
	public Color colourGoodUI;

	// Token: 0x04002AD3 RID: 10963
	public Color colourBadUI;

	// Token: 0x04002AD4 RID: 10964
	[SerializeField]
	private CanvasGroup timerCanvas;

	// Token: 0x04002AD5 RID: 10965
	[SerializeField]
	private RustSlider timerSlider;

	// Token: 0x04002AD6 RID: 10966
	[SerializeField]
	private UIChat chat;

	// Token: 0x04002AD7 RID: 10967
	[SerializeField]
	private HudElement Hunger;

	// Token: 0x04002AD8 RID: 10968
	[SerializeField]
	private HudElement Thirst;

	// Token: 0x04002AD9 RID: 10969
	[SerializeField]
	private HudElement Health;

	// Token: 0x04002ADA RID: 10970
	[SerializeField]
	private HudElement PendingHealth;

	// Token: 0x04002ADB RID: 10971
	public Sprite cardNone;

	// Token: 0x04002ADC RID: 10972
	public Sprite cardBackLarge;

	// Token: 0x04002ADD RID: 10973
	public Sprite cardBackSmall;

	// Token: 0x04002ADE RID: 10974
	private static Sprite cardBackLargeStatic;

	// Token: 0x04002ADF RID: 10975
	private static Sprite cardBackSmallStatic;

	// Token: 0x04002AE0 RID: 10976
	[SerializeField]
	private TexasHoldEmUI texasHoldEmUI;

	// Token: 0x04002AE1 RID: 10977
	[SerializeField]
	private BlackjackUI blackjackUI;

	// Token: 0x02000E1B RID: 3611
	[Serializable]
	public class PlayingCardImage
	{
		// Token: 0x04004942 RID: 18754
		public Rank rank;

		// Token: 0x04004943 RID: 18755
		public Suit suit;

		// Token: 0x04004944 RID: 18756
		public Sprite image;

		// Token: 0x04004945 RID: 18757
		public Sprite imageSmall;

		// Token: 0x04004946 RID: 18758
		public Sprite imageTransparent;
	}

	// Token: 0x02000E1C RID: 3612
	[Serializable]
	public class InfoTextUI
	{
		// Token: 0x04004947 RID: 18759
		public GameObject gameObj;

		// Token: 0x04004948 RID: 18760
		public RustText rustText;

		// Token: 0x04004949 RID: 18761
		public Image background;

		// Token: 0x02000F6C RID: 3948
		public enum Attitude
		{
			// Token: 0x04004E3D RID: 20029
			Neutral,
			// Token: 0x04004E3E RID: 20030
			Good,
			// Token: 0x04004E3F RID: 20031
			Bad
		}
	}

	// Token: 0x02000E1D RID: 3613
	public interface ICardGameSubUI
	{
		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x06004FF0 RID: 20464
		int DynamicBetAmount { get; }

		// Token: 0x06004FF1 RID: 20465
		void UpdateInGameUI(CardGameUI ui, CardGameController game);

		// Token: 0x06004FF2 RID: 20466
		string GetSecondaryInfo(CardGameUI ui, CardGameController game, out CardGameUI.InfoTextUI.Attitude attitude);

		// Token: 0x06004FF3 RID: 20467
		void UpdateInGameUI_NoPlayer(CardGameUI ui);
	}
}
