using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AB3 RID: 2739
	public class StackOfCards
	{
		// Token: 0x0600423A RID: 16954 RVA: 0x00182F7C File Offset: 0x0018117C
		public StackOfCards(int numDecks)
		{
			this.cards = new List<PlayingCard>(52 * numDecks);
			for (int i = 0; i < numDecks; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 13; k++)
					{
						this.cards.Add(PlayingCard.GetCard(j, k));
					}
				}
			}
			this.ShuffleDeck();
		}

		// Token: 0x0600423B RID: 16955 RVA: 0x00182FDC File Offset: 0x001811DC
		public bool TryTakeCard(out PlayingCard card)
		{
			if (this.cards.Count == 0)
			{
				card = null;
				return false;
			}
			card = this.cards[this.cards.Count - 1];
			this.cards.RemoveAt(this.cards.Count - 1);
			return true;
		}

		// Token: 0x0600423C RID: 16956 RVA: 0x0018302E File Offset: 0x0018122E
		public void AddCard(PlayingCard card)
		{
			this.cards.Insert(0, card);
		}

		// Token: 0x0600423D RID: 16957 RVA: 0x00183040 File Offset: 0x00181240
		public void ShuffleDeck()
		{
			int i = this.cards.Count;
			while (i > 1)
			{
				i--;
				int index = UnityEngine.Random.Range(0, i);
				PlayingCard value = this.cards[index];
				this.cards[index] = this.cards[i];
				this.cards[i] = value;
			}
		}

		// Token: 0x0600423E RID: 16958 RVA: 0x001830A0 File Offset: 0x001812A0
		public void Print()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Cards in the deck: ");
			foreach (PlayingCard playingCard in this.cards)
			{
				stringBuilder.AppendLine(playingCard.Rank + " of " + playingCard.Suit);
			}
			Debug.Log(stringBuilder.ToString());
		}

		// Token: 0x04003A96 RID: 14998
		private List<PlayingCard> cards;
	}
}
