using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Facepunch.CardGames
{
	// Token: 0x02000AB2 RID: 2738
	public class CardPlayerDataBlackjack : CardPlayerData
	{
		// Token: 0x0600422F RID: 16943 RVA: 0x00182D42 File Offset: 0x00180F42
		public CardPlayerDataBlackjack(int mountIndex, bool isServer) : base(mountIndex, isServer)
		{
			this.SplitCards = Pool.GetList<PlayingCard>();
		}

		// Token: 0x06004230 RID: 16944 RVA: 0x00182D57 File Offset: 0x00180F57
		public CardPlayerDataBlackjack(int scrapItemID, Func<int, StorageContainer> getStorage, int mountIndex, bool isServer) : base(scrapItemID, getStorage, mountIndex, isServer)
		{
			this.SplitCards = Pool.GetList<PlayingCard>();
		}

		// Token: 0x06004231 RID: 16945 RVA: 0x00182D6F File Offset: 0x00180F6F
		public override void Dispose()
		{
			base.Dispose();
			Pool.FreeList<PlayingCard>(ref this.SplitCards);
		}

		// Token: 0x06004232 RID: 16946 RVA: 0x00182D82 File Offset: 0x00180F82
		public override int GetTotalBetThisRound()
		{
			return this.betThisRound + this.splitBetThisRound + this.insuranceBetThisRound;
		}

		// Token: 0x06004233 RID: 16947 RVA: 0x00182D98 File Offset: 0x00180F98
		public override List<PlayingCard> GetSecondaryCards()
		{
			return this.SplitCards;
		}

		// Token: 0x06004234 RID: 16948 RVA: 0x00182DA0 File Offset: 0x00180FA0
		protected override void ClearPerRoundData()
		{
			base.ClearPerRoundData();
			this.SplitCards.Clear();
			this.splitBetThisRound = 0;
			this.insuranceBetThisRound = 0;
			this.playingSplitCards = false;
		}

		// Token: 0x06004235 RID: 16949 RVA: 0x00182DC8 File Offset: 0x00180FC8
		public override void LeaveCurrentRound(bool clearBets, bool leftRoundEarly)
		{
			if (!base.HasUserInCurrentRound)
			{
				return;
			}
			if (clearBets)
			{
				this.splitBetThisRound = 0;
				this.insuranceBetThisRound = 0;
			}
			base.LeaveCurrentRound(clearBets, leftRoundEarly);
		}

		// Token: 0x06004236 RID: 16950 RVA: 0x00182DEC File Offset: 0x00180FEC
		public override void LeaveGame()
		{
			base.LeaveGame();
			if (!base.HasUserInGame)
			{
				return;
			}
			this.SplitCards.Clear();
		}

		// Token: 0x06004237 RID: 16951 RVA: 0x00182E08 File Offset: 0x00181008
		public override void Save(CardGame syncData)
		{
			base.Save(syncData);
			CardGame.BlackjackCardPlayer blackjackCardPlayer = Pool.Get<CardGame.BlackjackCardPlayer>();
			blackjackCardPlayer.splitCards = Pool.GetList<int>();
			foreach (PlayingCard playingCard in this.SplitCards)
			{
				blackjackCardPlayer.splitCards.Add(base.SendCardDetails ? playingCard.GetIndex() : -1);
			}
			blackjackCardPlayer.splitBetThisRound = this.splitBetThisRound;
			blackjackCardPlayer.insuranceBetThisRound = this.insuranceBetThisRound;
			blackjackCardPlayer.playingSplitCards = this.playingSplitCards;
			if (syncData.blackjack.players == null)
			{
				syncData.blackjack.players = Pool.GetList<CardGame.BlackjackCardPlayer>();
			}
			syncData.blackjack.players.Add(blackjackCardPlayer);
		}

		// Token: 0x06004238 RID: 16952 RVA: 0x00182EDC File Offset: 0x001810DC
		public bool TrySwitchToSplitHand()
		{
			if (this.SplitCards.Count > 0 && !this.playingSplitCards)
			{
				this.SwapSplitCardsWithMain();
				this.playingSplitCards = true;
				return true;
			}
			return false;
		}

		// Token: 0x06004239 RID: 16953 RVA: 0x00182F04 File Offset: 0x00181104
		private void SwapSplitCardsWithMain()
		{
			List<PlayingCard> list = Pool.GetList<PlayingCard>();
			list.AddRange(this.Cards);
			this.Cards.Clear();
			this.Cards.AddRange(this.SplitCards);
			this.SplitCards.Clear();
			this.SplitCards.AddRange(list);
			Pool.FreeList<PlayingCard>(ref list);
			int betThisRound = this.betThisRound;
			int betThisRound2 = this.splitBetThisRound;
			this.splitBetThisRound = betThisRound;
			this.betThisRound = betThisRound2;
		}

		// Token: 0x04003A92 RID: 14994
		public List<PlayingCard> SplitCards;

		// Token: 0x04003A93 RID: 14995
		public int splitBetThisRound;

		// Token: 0x04003A94 RID: 14996
		public int insuranceBetThisRound;

		// Token: 0x04003A95 RID: 14997
		public bool playingSplitCards;
	}
}
