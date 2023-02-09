using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x02000797 RID: 1943
public class TexasHoldEmUI : MonoBehaviour
{
	// Token: 0x04002AE7 RID: 10983
	[SerializeField]
	private Image[] holeCardImages;

	// Token: 0x04002AE8 RID: 10984
	[SerializeField]
	private Image[] holeCardBackings;

	// Token: 0x04002AE9 RID: 10985
	[FormerlySerializedAs("flopCardImages")]
	[SerializeField]
	private Image[] communityCardImages;

	// Token: 0x04002AEA RID: 10986
	[SerializeField]
	private Image[] communityCardBackings;

	// Token: 0x04002AEB RID: 10987
	[SerializeField]
	private RustText potText;

	// Token: 0x04002AEC RID: 10988
	[SerializeField]
	private CardGamePlayerWidget[] playerWidgets;

	// Token: 0x04002AED RID: 10989
	[SerializeField]
	private Translate.Phrase phraseWinningHand;

	// Token: 0x04002AEE RID: 10990
	[SerializeField]
	private Translate.Phrase foldPhrase;

	// Token: 0x04002AEF RID: 10991
	[SerializeField]
	private Translate.Phrase raisePhrase;

	// Token: 0x04002AF0 RID: 10992
	[SerializeField]
	private Translate.Phrase checkPhrase;

	// Token: 0x04002AF1 RID: 10993
	[SerializeField]
	private Translate.Phrase callPhrase;

	// Token: 0x04002AF2 RID: 10994
	[SerializeField]
	private Translate.Phrase phraseRoyalFlush;

	// Token: 0x04002AF3 RID: 10995
	[SerializeField]
	private Translate.Phrase phraseStraightFlush;

	// Token: 0x04002AF4 RID: 10996
	[SerializeField]
	private Translate.Phrase phraseFourOfAKind;

	// Token: 0x04002AF5 RID: 10997
	[SerializeField]
	private Translate.Phrase phraseFullHouse;

	// Token: 0x04002AF6 RID: 10998
	[SerializeField]
	private Translate.Phrase phraseFlush;

	// Token: 0x04002AF7 RID: 10999
	[SerializeField]
	private Translate.Phrase phraseStraight;

	// Token: 0x04002AF8 RID: 11000
	[SerializeField]
	private Translate.Phrase phraseThreeOfAKind;

	// Token: 0x04002AF9 RID: 11001
	[SerializeField]
	private Translate.Phrase phraseTwoPair;

	// Token: 0x04002AFA RID: 11002
	[SerializeField]
	private Translate.Phrase phrasePair;

	// Token: 0x04002AFB RID: 11003
	[SerializeField]
	private Translate.Phrase phraseHighCard;

	// Token: 0x04002AFC RID: 11004
	[SerializeField]
	private Translate.Phrase phraseRaiseAmount;

	// Token: 0x04002AFD RID: 11005
	[SerializeField]
	private Sprite dealerChip;

	// Token: 0x04002AFE RID: 11006
	[SerializeField]
	private Sprite smallBlindChip;

	// Token: 0x04002AFF RID: 11007
	[SerializeField]
	private Sprite bigBlindChip;

	// Token: 0x04002B00 RID: 11008
	[SerializeField]
	private Sprite noIcon;
}
