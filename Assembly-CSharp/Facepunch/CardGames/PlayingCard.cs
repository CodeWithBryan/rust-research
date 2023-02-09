using System;
using PokerEvaluator;

namespace Facepunch.CardGames
{
	// Token: 0x02000AB6 RID: 2742
	public class PlayingCard
	{
		// Token: 0x0600423F RID: 16959 RVA: 0x00183130 File Offset: 0x00181330
		private PlayingCard(Suit suit, Rank rank)
		{
			this.IsUnknownCard = false;
			this.Suit = suit;
			this.Rank = rank;
		}

		// Token: 0x06004240 RID: 16960 RVA: 0x0018314D File Offset: 0x0018134D
		private PlayingCard()
		{
			this.IsUnknownCard = true;
			this.Suit = Suit.Spades;
			this.Rank = Rank.Two;
		}

		// Token: 0x06004241 RID: 16961 RVA: 0x0018316A File Offset: 0x0018136A
		public static PlayingCard GetCard(Suit suit, Rank rank)
		{
			return PlayingCard.GetCard((int)suit, (int)rank);
		}

		// Token: 0x06004242 RID: 16962 RVA: 0x00183173 File Offset: 0x00181373
		public static PlayingCard GetCard(int suit, int rank)
		{
			return PlayingCard.cards[suit * 13 + rank];
		}

		// Token: 0x06004243 RID: 16963 RVA: 0x00183181 File Offset: 0x00181381
		public static PlayingCard GetCard(int index)
		{
			if (index == -1)
			{
				return PlayingCard.unknownCard;
			}
			return PlayingCard.cards[index];
		}

		// Token: 0x06004244 RID: 16964 RVA: 0x00183194 File Offset: 0x00181394
		public int GetIndex()
		{
			if (this.IsUnknownCard)
			{
				return -1;
			}
			return PlayingCard.GetIndex(this.Suit, this.Rank);
		}

		// Token: 0x06004245 RID: 16965 RVA: 0x001831B1 File Offset: 0x001813B1
		public static int GetIndex(Suit suit, Rank rank)
		{
			return (int)(suit * (Suit)13 + (int)rank);
		}

		// Token: 0x06004246 RID: 16966 RVA: 0x001831B9 File Offset: 0x001813B9
		public int GetPokerEvaluationValue()
		{
			return Arrays.primes[(int)this.Rank] | (int)((int)this.Rank << 8) | this.GetPokerSuitCode() | 1 << (int)(16 + this.Rank);
		}

		// Token: 0x06004247 RID: 16967 RVA: 0x001831E8 File Offset: 0x001813E8
		private int GetPokerSuitCode()
		{
			switch (this.Suit)
			{
			case Suit.Spades:
				return 4096;
			case Suit.Hearts:
				return 8192;
			case Suit.Diamonds:
				return 16384;
			case Suit.Clubs:
				return 32768;
			default:
				return 4096;
			}
		}

		// Token: 0x06004248 RID: 16968 RVA: 0x00183234 File Offset: 0x00181434
		private static PlayingCard[] GenerateAllCards()
		{
			PlayingCard[] array = new PlayingCard[52];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 13; j++)
				{
					array[i * 13 + j] = new PlayingCard((Suit)i, (Rank)j);
				}
			}
			return array;
		}

		// Token: 0x04003AAA RID: 15018
		public readonly bool IsUnknownCard;

		// Token: 0x04003AAB RID: 15019
		public readonly Suit Suit;

		// Token: 0x04003AAC RID: 15020
		public readonly Rank Rank;

		// Token: 0x04003AAD RID: 15021
		public static PlayingCard[] cards = PlayingCard.GenerateAllCards();

		// Token: 0x04003AAE RID: 15022
		public static PlayingCard unknownCard = new PlayingCard();
	}
}
