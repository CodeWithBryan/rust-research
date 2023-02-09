using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000792 RID: 1938
public class BlackjackUI : MonoBehaviour
{
	// Token: 0x04002A88 RID: 10888
	[SerializeField]
	private Image[] playerCardImages;

	// Token: 0x04002A89 RID: 10889
	[SerializeField]
	private Image[] dealerCardImages;

	// Token: 0x04002A8A RID: 10890
	[SerializeField]
	private Image[] splitCardImages;

	// Token: 0x04002A8B RID: 10891
	[SerializeField]
	private Image[] playerCardBackings;

	// Token: 0x04002A8C RID: 10892
	[SerializeField]
	private Image[] dealerCardBackings;

	// Token: 0x04002A8D RID: 10893
	[SerializeField]
	private Image[] splitCardBackings;

	// Token: 0x04002A8E RID: 10894
	[SerializeField]
	private CardGamePlayerWidget[] playerWidgets;

	// Token: 0x04002A8F RID: 10895
	[SerializeField]
	private GameObject dealerValueObj;

	// Token: 0x04002A90 RID: 10896
	[SerializeField]
	private RustText dealerValueText;

	// Token: 0x04002A91 RID: 10897
	[SerializeField]
	private GameObject yourValueObj;

	// Token: 0x04002A92 RID: 10898
	[SerializeField]
	private RustText yourValueText;

	// Token: 0x04002A93 RID: 10899
	[SerializeField]
	private Translate.Phrase phrasePlaceYourBet;

	// Token: 0x04002A94 RID: 10900
	[SerializeField]
	private Translate.Phrase phraseHit;

	// Token: 0x04002A95 RID: 10901
	[SerializeField]
	private Translate.Phrase phraseStand;

	// Token: 0x04002A96 RID: 10902
	[SerializeField]
	private Translate.Phrase phraseSplit;

	// Token: 0x04002A97 RID: 10903
	[SerializeField]
	private Translate.Phrase phraseDouble;

	// Token: 0x04002A98 RID: 10904
	[SerializeField]
	private Translate.Phrase phraseInsurance;

	// Token: 0x04002A99 RID: 10905
	[SerializeField]
	private Translate.Phrase phraseBust;

	// Token: 0x04002A9A RID: 10906
	[SerializeField]
	private Translate.Phrase phraseBlackjack;

	// Token: 0x04002A9B RID: 10907
	[SerializeField]
	private Translate.Phrase phraseStandoff;

	// Token: 0x04002A9C RID: 10908
	[SerializeField]
	private Translate.Phrase phraseYouWin;

	// Token: 0x04002A9D RID: 10909
	[SerializeField]
	private Translate.Phrase phraseYouLose;

	// Token: 0x04002A9E RID: 10910
	[SerializeField]
	private Translate.Phrase phraseWaitingForOtherPlayers;

	// Token: 0x04002A9F RID: 10911
	[SerializeField]
	private Translate.Phrase phraseHand;

	// Token: 0x04002AA0 RID: 10912
	[SerializeField]
	private Translate.Phrase phraseInsurancePaidOut;

	// Token: 0x04002AA1 RID: 10913
	[SerializeField]
	private Sprite insuranceIcon;

	// Token: 0x04002AA2 RID: 10914
	[SerializeField]
	private Sprite noIcon;

	// Token: 0x04002AA3 RID: 10915
	[SerializeField]
	private Color bustTextColour;
}
