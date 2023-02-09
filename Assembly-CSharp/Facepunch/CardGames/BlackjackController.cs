using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ProtoBuf;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AAF RID: 2735
	public class BlackjackController : CardGameController
	{
		// Token: 0x0600417B RID: 16763 RVA: 0x00180512 File Offset: 0x0017E712
		public BlackjackController(BaseCardGameEntity owner) : base(owner)
		{
		}

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x0600417C RID: 16764 RVA: 0x00003A54 File Offset: 0x00001C54
		public override int MinPlayers
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x0600417D RID: 16765 RVA: 0x0001F1CE File Offset: 0x0001D3CE
		public override int MinBuyIn
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x0600417E RID: 16766 RVA: 0x00180532 File Offset: 0x0017E732
		public override int MaxBuyIn
		{
			get
			{
				return int.MaxValue;
			}
		}

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x0600417F RID: 16767 RVA: 0x00180539 File Offset: 0x0017E739
		public override int MinToPlay
		{
			get
			{
				return this.MinBuyIn;
			}
		}

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x06004180 RID: 16768 RVA: 0x00003A54 File Offset: 0x00001C54
		public override int EndRoundDelay
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x06004181 RID: 16769 RVA: 0x00180541 File Offset: 0x0017E741
		public override int TimeBetweenRounds
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x06004182 RID: 16770 RVA: 0x00180544 File Offset: 0x0017E744
		// (set) Token: 0x06004183 RID: 16771 RVA: 0x0018054C File Offset: 0x0017E74C
		public BlackjackController.BlackjackInputOption LastAction { get; private set; }

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x06004184 RID: 16772 RVA: 0x00180555 File Offset: 0x0017E755
		// (set) Token: 0x06004185 RID: 16773 RVA: 0x0018055D File Offset: 0x0017E75D
		public ulong LastActionTarget { get; private set; }

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x06004186 RID: 16774 RVA: 0x00180566 File Offset: 0x0017E766
		// (set) Token: 0x06004187 RID: 16775 RVA: 0x0018056E File Offset: 0x0017E76E
		public int LastActionValue { get; private set; }

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x06004188 RID: 16776 RVA: 0x00180578 File Offset: 0x0017E778
		public bool AllBetsPlaced
		{
			get
			{
				if (!base.HasRoundInProgressOrEnding)
				{
					return false;
				}
				using (IEnumerator<CardPlayerData> enumerator = base.PlayersInRound().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.betThisRound == 0)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x06004189 RID: 16777 RVA: 0x00007074 File Offset: 0x00005274
		protected override int GetFirstPlayerRelIndex(bool startOfRound)
		{
			return 0;
		}

		// Token: 0x0600418A RID: 16778 RVA: 0x001805D8 File Offset: 0x0017E7D8
		public override List<PlayingCard> GetTableCards()
		{
			return this.dealerCards;
		}

		// Token: 0x0600418B RID: 16779 RVA: 0x001805E0 File Offset: 0x0017E7E0
		public void InputsToList(int availableInputs, List<BlackjackController.BlackjackInputOption> result)
		{
			foreach (BlackjackController.BlackjackInputOption blackjackInputOption in (BlackjackController.BlackjackInputOption[])Enum.GetValues(typeof(BlackjackController.BlackjackInputOption)))
			{
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.None && (availableInputs & (int)blackjackInputOption) == (int)blackjackInputOption)
				{
					result.Add(blackjackInputOption);
				}
			}
		}

		// Token: 0x0600418C RID: 16780 RVA: 0x00180624 File Offset: 0x0017E824
		public bool WaitingForOtherPlayers(CardPlayerData pData)
		{
			if (!pData.HasUserInCurrentRound)
			{
				return false;
			}
			if (base.State == CardGameController.CardGameState.InGameRound && !pData.HasAvailableInputs)
			{
				foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
				{
					if (cardPlayerData != pData && cardPlayerData.HasAvailableInputs)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x0600418D RID: 16781 RVA: 0x00180698 File Offset: 0x0017E898
		public int GetCardsValue(List<PlayingCard> cards, BlackjackController.CardsValueMode mode)
		{
			int num = 0;
			foreach (PlayingCard playingCard in cards)
			{
				if (!playingCard.IsUnknownCard)
				{
					num += this.GetCardValue(playingCard, mode);
					if (playingCard.Rank == Rank.Ace)
					{
						mode = BlackjackController.CardsValueMode.Low;
					}
				}
			}
			return num;
		}

		// Token: 0x0600418E RID: 16782 RVA: 0x00180704 File Offset: 0x0017E904
		public int GetOptimalCardsValue(List<PlayingCard> cards)
		{
			int cardsValue = this.GetCardsValue(cards, BlackjackController.CardsValueMode.Low);
			int cardsValue2 = this.GetCardsValue(cards, BlackjackController.CardsValueMode.High);
			if (cardsValue2 <= 21)
			{
				return cardsValue2;
			}
			return cardsValue;
		}

		// Token: 0x0600418F RID: 16783 RVA: 0x0018072C File Offset: 0x0017E92C
		public int GetCardValue(PlayingCard card, BlackjackController.CardsValueMode mode)
		{
			int rank = (int)card.Rank;
			if (rank <= 8)
			{
				return rank + 2;
			}
			if (rank <= 11)
			{
				return 10;
			}
			if (mode != BlackjackController.CardsValueMode.Low)
			{
				return 11;
			}
			return 1;
		}

		// Token: 0x06004190 RID: 16784 RVA: 0x00180757 File Offset: 0x0017E957
		public bool Has21(List<PlayingCard> cards)
		{
			return this.GetOptimalCardsValue(cards) == 21;
		}

		// Token: 0x06004191 RID: 16785 RVA: 0x00180764 File Offset: 0x0017E964
		public bool HasBlackjack(List<PlayingCard> cards)
		{
			return this.GetCardsValue(cards, BlackjackController.CardsValueMode.High) == 21 && cards.Count == 2;
		}

		// Token: 0x06004192 RID: 16786 RVA: 0x0018077D File Offset: 0x0017E97D
		public bool HasBusted(List<PlayingCard> cards)
		{
			return this.GetCardsValue(cards, BlackjackController.CardsValueMode.Low) > 21;
		}

		// Token: 0x06004193 RID: 16787 RVA: 0x0018078C File Offset: 0x0017E98C
		private bool CanSplit(CardPlayerDataBlackjack pData)
		{
			if (pData.Cards.Count != 2)
			{
				return false;
			}
			if (this.HasSplit(pData))
			{
				return false;
			}
			int betThisRound = pData.betThisRound;
			return pData.GetScrapAmount() >= betThisRound && this.GetCardValue(pData.Cards[0], BlackjackController.CardsValueMode.Low) == this.GetCardValue(pData.Cards[1], BlackjackController.CardsValueMode.Low);
		}

		// Token: 0x06004194 RID: 16788 RVA: 0x001807F0 File Offset: 0x0017E9F0
		private bool HasAnyAces(List<PlayingCard> cards)
		{
			using (List<PlayingCard>.Enumerator enumerator = cards.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Rank == Rank.Ace)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004195 RID: 16789 RVA: 0x00180848 File Offset: 0x0017EA48
		private bool CanDoubleDown(CardPlayerDataBlackjack pData)
		{
			if (pData.Cards.Count != 2)
			{
				return false;
			}
			if (this.HasAnyAces(pData.Cards))
			{
				return false;
			}
			int betThisRound = pData.betThisRound;
			return pData.GetScrapAmount() >= betThisRound;
		}

		// Token: 0x06004196 RID: 16790 RVA: 0x00180888 File Offset: 0x0017EA88
		private bool CanTakeInsurance(CardPlayerDataBlackjack pData)
		{
			if (this.dealerCards.Count != 2)
			{
				return false;
			}
			if (this.dealerCards[1].Rank != Rank.Ace)
			{
				return false;
			}
			if (pData.insuranceBetThisRound > 0)
			{
				return false;
			}
			int num = Mathf.FloorToInt((float)pData.betThisRound / 2f);
			return pData.GetScrapAmount() >= num;
		}

		// Token: 0x06004197 RID: 16791 RVA: 0x001808E6 File Offset: 0x0017EAE6
		private bool HasSplit(CardPlayerDataBlackjack pData)
		{
			return pData.SplitCards.Count > 0;
		}

		// Token: 0x06004198 RID: 16792 RVA: 0x001808F6 File Offset: 0x0017EAF6
		protected override CardPlayerData GetNewCardPlayerData(int mountIndex)
		{
			if (base.IsServer)
			{
				return new CardPlayerDataBlackjack(base.ScrapItemID, new Func<int, StorageContainer>(base.Owner.GetPlayerStorage), mountIndex, base.IsServer);
			}
			return new CardPlayerDataBlackjack(mountIndex, base.IsServer);
		}

		// Token: 0x06004199 RID: 16793 RVA: 0x00180930 File Offset: 0x0017EB30
		public bool TryGetCardPlayerDataBlackjack(int index, out CardPlayerDataBlackjack cpBlackjack)
		{
			CardPlayerData cardPlayerData;
			bool result = base.TryGetCardPlayerData(index, out cardPlayerData);
			cpBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
			return result;
		}

		// Token: 0x0600419A RID: 16794 RVA: 0x0018094E File Offset: 0x0017EB4E
		public int ResultsToInt(BlackjackController.BlackjackRoundResult mainResult, BlackjackController.BlackjackRoundResult splitResult, int insurancePayout)
		{
			return (int)(mainResult + (int)((BlackjackController.BlackjackRoundResult)10 * splitResult) + 100 * insurancePayout);
		}

		// Token: 0x0600419B RID: 16795 RVA: 0x0018095B File Offset: 0x0017EB5B
		public void ResultsFromInt(int result, out BlackjackController.BlackjackRoundResult mainResult, out BlackjackController.BlackjackRoundResult splitResult, out int insurancePayout)
		{
			mainResult = (BlackjackController.BlackjackRoundResult)(result % 10);
			splitResult = (BlackjackController.BlackjackRoundResult)(result / 10 % 10);
			insurancePayout = (result - (int)mainResult - (int)splitResult) / 100;
		}

		// Token: 0x0600419C RID: 16796 RVA: 0x0018097C File Offset: 0x0017EB7C
		public override void Save(CardGame syncData)
		{
			syncData.blackjack = Pool.Get<CardGame.Blackjack>();
			syncData.blackjack.dealerCards = Pool.GetList<int>();
			syncData.lastActionId = (int)this.LastAction;
			syncData.lastActionTarget = this.LastActionTarget;
			syncData.lastActionValue = this.LastActionValue;
			for (int i = 0; i < this.dealerCards.Count; i++)
			{
				PlayingCard playingCard = this.dealerCards[i];
				if (base.HasActiveRound && i == 0)
				{
					syncData.blackjack.dealerCards.Add(-1);
				}
				else
				{
					syncData.blackjack.dealerCards.Add(playingCard.GetIndex());
				}
			}
			base.Save(syncData);
			this.ClearLastAction();
		}

		// Token: 0x0600419D RID: 16797 RVA: 0x00180A2C File Offset: 0x0017EC2C
		private void EditorMakeRandomMove(CardPlayerDataBlackjack pdBlackjack)
		{
			List<BlackjackController.BlackjackInputOption> list = Pool.GetList<BlackjackController.BlackjackInputOption>();
			this.InputsToList(pdBlackjack.availableInputs, list);
			if (list.Count == 0)
			{
				Debug.Log("No moves currently available.");
				Pool.FreeList<BlackjackController.BlackjackInputOption>(ref list);
				return;
			}
			BlackjackController.BlackjackInputOption blackjackInputOption = list[UnityEngine.Random.Range(0, list.Count)];
			if (this.AllBetsPlaced)
			{
				if (this.GetOptimalCardsValue(pdBlackjack.Cards) < 17 && list.Contains(BlackjackController.BlackjackInputOption.Hit))
				{
					blackjackInputOption = BlackjackController.BlackjackInputOption.Hit;
				}
				else if (list.Contains(BlackjackController.BlackjackInputOption.Stand))
				{
					blackjackInputOption = BlackjackController.BlackjackInputOption.Stand;
				}
			}
			else if (list.Contains(BlackjackController.BlackjackInputOption.SubmitBet))
			{
				blackjackInputOption = BlackjackController.BlackjackInputOption.SubmitBet;
			}
			if (list.Count > 0)
			{
				int num = 0;
				if (blackjackInputOption == BlackjackController.BlackjackInputOption.SubmitBet)
				{
					num = this.MinBuyIn;
				}
				Debug.Log(string.Concat(new object[]
				{
					pdBlackjack.UserID,
					" Taking random action: ",
					blackjackInputOption,
					" with value ",
					num
				}));
				base.ReceivedInputFromPlayer(pdBlackjack, (int)blackjackInputOption, true, num, true);
			}
			else
			{
				Debug.LogWarning(base.GetType().Name + ": No input options are available for the current player.");
			}
			Pool.FreeList<BlackjackController.BlackjackInputOption>(ref list);
		}

		// Token: 0x0600419E RID: 16798 RVA: 0x00180B3C File Offset: 0x0017ED3C
		protected override int GetAvailableInputsForPlayer(CardPlayerData pData)
		{
			BlackjackController.BlackjackInputOption blackjackInputOption = BlackjackController.BlackjackInputOption.None;
			CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)pData;
			if (cardPlayerDataBlackjack == null || this.isWaitingBetweenTurns || cardPlayerDataBlackjack.hasCompletedTurn || !cardPlayerDataBlackjack.HasUserInCurrentRound)
			{
				return (int)blackjackInputOption;
			}
			if (!base.HasActiveRound)
			{
				return (int)blackjackInputOption;
			}
			if (this.AllBetsPlaced)
			{
				blackjackInputOption |= BlackjackController.BlackjackInputOption.Stand;
				if (!this.Has21(cardPlayerDataBlackjack.Cards))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.Hit;
				}
				if (this.CanSplit(cardPlayerDataBlackjack))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.Split;
				}
				if (this.CanDoubleDown(cardPlayerDataBlackjack))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.DoubleDown;
				}
				if (this.CanTakeInsurance(cardPlayerDataBlackjack))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.Insurance;
				}
			}
			else
			{
				blackjackInputOption |= BlackjackController.BlackjackInputOption.SubmitBet;
				blackjackInputOption |= BlackjackController.BlackjackInputOption.MaxBet;
			}
			return (int)blackjackInputOption;
		}

		// Token: 0x0600419F RID: 16799 RVA: 0x00180BCC File Offset: 0x0017EDCC
		protected override void SubEndGameplay()
		{
			this.dealerCards.Clear();
		}

		// Token: 0x060041A0 RID: 16800 RVA: 0x00180BDC File Offset: 0x0017EDDC
		protected override void SubEndRound()
		{
			BlackjackController.<>c__DisplayClass59_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.dealerCardsVal = this.GetOptimalCardsValue(this.dealerCards);
			if (CS$<>8__locals1.dealerCardsVal > 21)
			{
				CS$<>8__locals1.dealerCardsVal = 0;
			}
			base.resultInfo.winningScore = CS$<>8__locals1.dealerCardsVal;
			if (base.NumPlayersInCurrentRound() == 0)
			{
				base.Owner.ClientRPC<CardGame.RoundResults>(null, "OnResultsDeclared", base.resultInfo);
				return;
			}
			CS$<>8__locals1.dealerHasBlackjack = this.HasBlackjack(this.dealerCards);
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
				int num = 0;
				int num2;
				BlackjackController.BlackjackRoundResult mainResult = this.<SubEndRound>g__CheckResult|59_0(cardPlayerDataBlackjack.Cards, cardPlayerDataBlackjack.betThisRound, out num2, ref CS$<>8__locals1);
				num += num2;
				BlackjackController.BlackjackRoundResult splitResult = this.<SubEndRound>g__CheckResult|59_0(cardPlayerDataBlackjack.SplitCards, cardPlayerDataBlackjack.splitBetThisRound, out num2, ref CS$<>8__locals1);
				num += num2;
				int num3 = cardPlayerDataBlackjack.betThisRound + cardPlayerDataBlackjack.splitBetThisRound + cardPlayerDataBlackjack.insuranceBetThisRound;
				int insurancePayout = 0;
				if (CS$<>8__locals1.dealerHasBlackjack && cardPlayerDataBlackjack.insuranceBetThisRound > 0)
				{
					int num4 = Mathf.FloorToInt((float)cardPlayerDataBlackjack.insuranceBetThisRound * 3f);
					num += num4;
					insurancePayout = num4;
				}
				int resultCode = this.ResultsToInt(mainResult, splitResult, insurancePayout);
				this.AddRoundResult(cardPlayerDataBlackjack, num - num3, resultCode);
				this.PayOut(cardPlayerDataBlackjack, num);
			}
			base.ClearPot();
			base.Owner.ClientRPC<CardGame.RoundResults>(null, "OnResultsDeclared", base.resultInfo);
		}

		// Token: 0x060041A1 RID: 16801 RVA: 0x00180D6C File Offset: 0x0017EF6C
		private int PayOut(CardPlayerData pData, int winnings)
		{
			if (winnings == 0)
			{
				return 0;
			}
			StorageContainer storage = pData.GetStorage();
			if (storage == null)
			{
				return 0;
			}
			storage.inventory.AddItem(base.Owner.scrapItemDef, winnings, 0UL, global::ItemContainer.LimitStack.None);
			return winnings;
		}

		// Token: 0x060041A2 RID: 16802 RVA: 0x00180DAB File Offset: 0x0017EFAB
		protected override void HandlePlayerLeavingDuringTheirTurn(CardPlayerData pData)
		{
			base.ReceivedInputFromPlayer(pData, 128, true, 0, false);
		}

		// Token: 0x060041A3 RID: 16803 RVA: 0x00180DBC File Offset: 0x0017EFBC
		protected override void SubReceivedInputFromPlayer(CardPlayerData pData, int input, int value, bool countAsAction)
		{
			if (!Enum.IsDefined(typeof(BlackjackController.BlackjackInputOption), input))
			{
				return;
			}
			BlackjackController.BlackjackInputOption lastAction = (BlackjackController.BlackjackInputOption)input;
			CardPlayerDataBlackjack pdBlackjack = (CardPlayerDataBlackjack)pData;
			if (!base.HasActiveRound)
			{
				this.LastActionTarget = pData.UserID;
				this.LastAction = lastAction;
				this.LastActionValue = 0;
				return;
			}
			int lastActionValue = 0;
			if (this.AllBetsPlaced)
			{
				this.DoInRoundPlayerInput(pdBlackjack, ref lastAction, ref lastActionValue);
			}
			else
			{
				this.DoBettingPhasePlayerInput(pdBlackjack, value, countAsAction, ref lastAction, ref lastActionValue);
			}
			this.LastActionTarget = pData.UserID;
			this.LastAction = lastAction;
			this.LastActionValue = lastActionValue;
			if (base.NumPlayersInCurrentRound() == 0)
			{
				base.EndGameplay();
				return;
			}
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			base.StartTurnTimer(pData, this.MaxTurnTime);
			base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x060041A4 RID: 16804 RVA: 0x00180E84 File Offset: 0x0017F084
		private void DoInRoundPlayerInput(CardPlayerDataBlackjack pdBlackjack, ref BlackjackController.BlackjackInputOption selectedMove, ref int selectedMoveValue)
		{
			if (selectedMove != BlackjackController.BlackjackInputOption.Abandon && (pdBlackjack.availableInputs & (int)selectedMove) != (int)selectedMove)
			{
				return;
			}
			BlackjackController.BlackjackInputOption blackjackInputOption = selectedMove;
			if (blackjackInputOption <= BlackjackController.BlackjackInputOption.Split)
			{
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.Hit)
				{
					if (blackjackInputOption != BlackjackController.BlackjackInputOption.Stand)
					{
						if (blackjackInputOption == BlackjackController.BlackjackInputOption.Split)
						{
							PlayingCard playingCard = pdBlackjack.Cards[1];
							bool flag = playingCard.Rank == Rank.Ace;
							pdBlackjack.SplitCards.Add(playingCard);
							pdBlackjack.Cards.Remove(playingCard);
							PlayingCard item;
							this.cardStack.TryTakeCard(out item);
							pdBlackjack.Cards.Add(item);
							this.cardStack.TryTakeCard(out item);
							pdBlackjack.SplitCards.Add(item);
							selectedMoveValue = this.TryMakeBet(pdBlackjack, pdBlackjack.betThisRound, BlackjackController.BetType.Split);
							if (flag)
							{
								pdBlackjack.SetHasCompletedTurn(true);
							}
						}
					}
					else if (!pdBlackjack.TrySwitchToSplitHand())
					{
						pdBlackjack.SetHasCompletedTurn(true);
					}
				}
				else
				{
					PlayingCard item2;
					this.cardStack.TryTakeCard(out item2);
					pdBlackjack.Cards.Add(item2);
				}
			}
			else if (blackjackInputOption != BlackjackController.BlackjackInputOption.DoubleDown)
			{
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.Insurance)
				{
					if (blackjackInputOption == BlackjackController.BlackjackInputOption.Abandon)
					{
						pdBlackjack.LeaveCurrentRound(false, true);
					}
				}
				else
				{
					int maxAmount = Mathf.FloorToInt((float)pdBlackjack.betThisRound / 2f);
					selectedMoveValue = this.TryMakeBet(pdBlackjack, maxAmount, BlackjackController.BetType.Insurance);
				}
			}
			else
			{
				if (pdBlackjack.playingSplitCards)
				{
					selectedMoveValue = this.TryMakeBet(pdBlackjack, pdBlackjack.splitBetThisRound, BlackjackController.BetType.Split);
				}
				else
				{
					selectedMoveValue = this.TryMakeBet(pdBlackjack, pdBlackjack.betThisRound, BlackjackController.BetType.Main);
				}
				PlayingCard item3;
				this.cardStack.TryTakeCard(out item3);
				pdBlackjack.Cards.Add(item3);
				if (!pdBlackjack.TrySwitchToSplitHand())
				{
					pdBlackjack.SetHasCompletedTurn(true);
				}
			}
			if (this.HasBusted(pdBlackjack.Cards) && !pdBlackjack.TrySwitchToSplitHand())
			{
				pdBlackjack.SetHasCompletedTurn(true);
			}
			if (this.Has21(pdBlackjack.Cards) && !this.CanTakeInsurance(pdBlackjack) && !this.CanDoubleDown(pdBlackjack) && !this.CanSplit(pdBlackjack) && !pdBlackjack.TrySwitchToSplitHand())
			{
				pdBlackjack.SetHasCompletedTurn(true);
			}
		}

		// Token: 0x060041A5 RID: 16805 RVA: 0x00181074 File Offset: 0x0017F274
		private void DoBettingPhasePlayerInput(CardPlayerDataBlackjack pdBlackjack, int value, bool countAsAction, ref BlackjackController.BlackjackInputOption selectedMove, ref int selectedMoveValue)
		{
			if (selectedMove != BlackjackController.BlackjackInputOption.Abandon && (pdBlackjack.availableInputs & (int)selectedMove) != (int)selectedMove)
			{
				return;
			}
			if (selectedMove == BlackjackController.BlackjackInputOption.SubmitBet)
			{
				selectedMoveValue = this.TryMakeBet(pdBlackjack, value, BlackjackController.BetType.Main);
				if (countAsAction)
				{
					pdBlackjack.SetHasCompletedTurn(true);
					return;
				}
			}
			else if (selectedMove == BlackjackController.BlackjackInputOption.MaxBet)
			{
				selectedMoveValue = this.TryMakeBet(pdBlackjack, BlackjackMachine.maxbet, BlackjackController.BetType.Main);
				if (countAsAction)
				{
					pdBlackjack.SetHasCompletedTurn(true);
					return;
				}
			}
			else if (selectedMove == BlackjackController.BlackjackInputOption.Abandon)
			{
				pdBlackjack.LeaveCurrentRound(false, true);
			}
		}

		// Token: 0x060041A6 RID: 16806 RVA: 0x001810F0 File Offset: 0x0017F2F0
		private int TryMakeBet(CardPlayerDataBlackjack pdBlackjack, int maxAmount, BlackjackController.BetType betType)
		{
			int num = base.TryMoveToPotStorage(pdBlackjack, maxAmount);
			switch (betType)
			{
			case BlackjackController.BetType.Main:
				pdBlackjack.betThisTurn += num;
				pdBlackjack.betThisRound += num;
				break;
			case BlackjackController.BetType.Split:
				pdBlackjack.splitBetThisRound += num;
				break;
			case BlackjackController.BetType.Insurance:
				pdBlackjack.insuranceBetThisRound += num;
				break;
			}
			return num;
		}

		// Token: 0x060041A7 RID: 16807 RVA: 0x00181158 File Offset: 0x0017F358
		protected override void SubStartRound()
		{
			this.dealerCards.Clear();
			this.cardStack = new StackOfCards(6);
			this.ClearLastAction();
			base.ServerPlaySound(CardGameSounds.SoundType.Shuffle);
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
				cardPlayerDataBlackjack.EnableSendingCards();
				cardPlayerDataBlackjack.availableInputs = this.GetAvailableInputsForPlayer(cardPlayerDataBlackjack);
				base.StartTurnTimer(cardPlayerDataBlackjack, this.MaxTurnTime);
			}
		}

		// Token: 0x060041A8 RID: 16808 RVA: 0x001811E8 File Offset: 0x0017F3E8
		protected override void OnTurnTimeout(CardPlayerData pData)
		{
			if (pData.HasUserInCurrentRound && !pData.hasCompletedTurn)
			{
				BlackjackController.BlackjackInputOption blackjackInputOption = BlackjackController.BlackjackInputOption.Abandon;
				int value = 0;
				if (this.AllBetsPlaced)
				{
					if ((pData.availableInputs & 4) == 4)
					{
						blackjackInputOption = BlackjackController.BlackjackInputOption.Stand;
						base.ReceivedInputFromPlayer(pData, 4, true, 0, false);
					}
				}
				else if ((pData.availableInputs & 1) == 1 && pData.GetScrapAmount() >= this.MinBuyIn)
				{
					blackjackInputOption = BlackjackController.BlackjackInputOption.SubmitBet;
					value = this.MinBuyIn;
				}
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.Abandon)
				{
					base.ReceivedInputFromPlayer(pData, (int)blackjackInputOption, true, value, false);
					return;
				}
				blackjackInputOption = BlackjackController.BlackjackInputOption.Abandon;
				base.ReceivedInputFromPlayer(pData, (int)blackjackInputOption, true, 0, false);
				pData.ClearAllData();
				if (base.HasActiveRound && base.NumPlayersInCurrentRound() < this.MinPlayers)
				{
					base.BeginRoundEnd();
				}
				if (pData.HasUserInGame)
				{
					base.Owner.ClientRPC<ulong>(null, "ClientOnPlayerLeft", pData.UserID);
				}
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}

		// Token: 0x060041A9 RID: 16809 RVA: 0x001812D0 File Offset: 0x0017F4D0
		protected override void StartNextCycle()
		{
			base.StartNextCycle();
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack pData = (CardPlayerDataBlackjack)cardPlayerData;
				base.StartTurnTimer(pData, this.MaxTurnTime);
			}
			base.UpdateAllAvailableInputs();
			base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x060041AA RID: 16810 RVA: 0x00181350 File Offset: 0x0017F550
		protected override bool ShouldEndCycle()
		{
			using (IEnumerator<CardPlayerData> enumerator = base.PlayersInRound().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.hasCompletedTurn)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060041AB RID: 16811 RVA: 0x001813A4 File Offset: 0x0017F5A4
		protected override void EndCycle()
		{
			CardPlayerData[] playerData = base.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].SetHasCompletedTurn(false);
			}
			if (this.dealerCards.Count == 0)
			{
				this.DealInitialCards();
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				base.QueueNextCycleInvoke();
				return;
			}
			bool flag = true;
			bool flag2 = true;
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
				if (!this.HasBusted(cardPlayerDataBlackjack.Cards))
				{
					flag = false;
				}
				if (!this.HasBlackjack(cardPlayerDataBlackjack.Cards))
				{
					flag2 = false;
				}
				if (cardPlayerDataBlackjack.SplitCards.Count > 0)
				{
					if (!this.HasBusted(cardPlayerDataBlackjack.SplitCards))
					{
						flag = false;
					}
					if (!this.HasBlackjack(cardPlayerDataBlackjack.SplitCards))
					{
						flag2 = false;
					}
				}
				if (!flag && !flag2)
				{
					break;
				}
			}
			base.ServerPlaySound(CardGameSounds.SoundType.Draw);
			if (base.NumPlayersInCurrentRound() > 0 && !flag && !flag2)
			{
				base.Owner.Invoke(new Action(this.DealerPlayInvoke), 1f);
				base.BeginRoundEnd();
				return;
			}
			base.EndRoundWithDelay();
		}

		// Token: 0x060041AC RID: 16812 RVA: 0x001814D8 File Offset: 0x0017F6D8
		private void DealerPlayInvoke()
		{
			int cardsValue = this.GetCardsValue(this.dealerCards, BlackjackController.CardsValueMode.High);
			if (this.GetCardsValue(this.dealerCards, BlackjackController.CardsValueMode.Low) < 17 && (cardsValue < 17 || cardsValue > 21))
			{
				PlayingCard item;
				this.cardStack.TryTakeCard(out item);
				this.dealerCards.Add(item);
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				base.Owner.Invoke(new Action(this.DealerPlayInvoke), 1f);
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
			base.EndRoundWithDelay();
		}

		// Token: 0x060041AD RID: 16813 RVA: 0x00181560 File Offset: 0x0017F760
		private void DealInitialCards()
		{
			if (!base.HasActiveRound)
			{
				return;
			}
			PlayingCard item;
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				this.cardStack.TryTakeCard(out item);
				cardPlayerData.Cards.Add(item);
			}
			this.cardStack.TryTakeCard(out item);
			this.dealerCards.Add(item);
			foreach (CardPlayerData cardPlayerData2 in base.PlayersInRound())
			{
				this.cardStack.TryTakeCard(out item);
				cardPlayerData2.Cards.Add(item);
				if (this.HasBlackjack(cardPlayerData2.Cards))
				{
					cardPlayerData2.SetHasCompletedTurn(true);
				}
			}
			this.cardStack.TryTakeCard(out item);
			this.dealerCards.Add(item);
		}

		// Token: 0x060041AE RID: 16814 RVA: 0x00181660 File Offset: 0x0017F860
		private void ClearLastAction()
		{
			this.LastAction = BlackjackController.BlackjackInputOption.None;
			this.LastActionTarget = 0UL;
			this.LastActionValue = 0;
		}

		// Token: 0x060041AF RID: 16815 RVA: 0x00181678 File Offset: 0x0017F878
		[CompilerGenerated]
		private BlackjackController.BlackjackRoundResult <SubEndRound>g__CheckResult|59_0(List<PlayingCard> cards, int betAmount, out int winnings, ref BlackjackController.<>c__DisplayClass59_0 A_4)
		{
			if (cards.Count == 0)
			{
				winnings = 0;
				return BlackjackController.BlackjackRoundResult.None;
			}
			int optimalCardsValue = this.GetOptimalCardsValue(cards);
			if (optimalCardsValue > 21)
			{
				winnings = 0;
				return BlackjackController.BlackjackRoundResult.Bust;
			}
			if (optimalCardsValue > base.resultInfo.winningScore)
			{
				base.resultInfo.winningScore = optimalCardsValue;
			}
			BlackjackController.BlackjackRoundResult blackjackRoundResult = BlackjackController.BlackjackRoundResult.Loss;
			bool flag = this.HasBlackjack(cards);
			if (A_4.dealerHasBlackjack)
			{
				if (flag)
				{
					blackjackRoundResult = BlackjackController.BlackjackRoundResult.Standoff;
				}
			}
			else if (optimalCardsValue > A_4.dealerCardsVal)
			{
				blackjackRoundResult = (flag ? BlackjackController.BlackjackRoundResult.BlackjackWin : BlackjackController.BlackjackRoundResult.Win);
			}
			else if (optimalCardsValue == A_4.dealerCardsVal)
			{
				if (flag)
				{
					blackjackRoundResult = BlackjackController.BlackjackRoundResult.BlackjackWin;
				}
				else
				{
					blackjackRoundResult = BlackjackController.BlackjackRoundResult.Standoff;
				}
			}
			if (blackjackRoundResult == BlackjackController.BlackjackRoundResult.BlackjackWin)
			{
				winnings = Mathf.FloorToInt((float)betAmount * 2.5f);
			}
			else if (blackjackRoundResult == BlackjackController.BlackjackRoundResult.Win)
			{
				winnings = Mathf.FloorToInt((float)betAmount * 2f);
			}
			else if (blackjackRoundResult == BlackjackController.BlackjackRoundResult.Standoff)
			{
				winnings = betAmount;
			}
			else
			{
				winnings = 0;
			}
			return blackjackRoundResult;
		}

		// Token: 0x04003A6F RID: 14959
		public List<PlayingCard> dealerCards = new List<PlayingCard>();

		// Token: 0x04003A70 RID: 14960
		public const float BLACKJACK_PAYOUT_RATIO = 1.5f;

		// Token: 0x04003A71 RID: 14961
		public const float INSURANCE_PAYOUT_RATIO = 2f;

		// Token: 0x04003A72 RID: 14962
		private const float DEALER_MOVE_TIME = 1f;

		// Token: 0x04003A76 RID: 14966
		private const int NUM_DECKS = 6;

		// Token: 0x04003A77 RID: 14967
		private StackOfCards cardStack = new StackOfCards(6);

		// Token: 0x02000F10 RID: 3856
		[Flags]
		public enum BlackjackInputOption
		{
			// Token: 0x04004D00 RID: 19712
			None = 0,
			// Token: 0x04004D01 RID: 19713
			SubmitBet = 1,
			// Token: 0x04004D02 RID: 19714
			Hit = 2,
			// Token: 0x04004D03 RID: 19715
			Stand = 4,
			// Token: 0x04004D04 RID: 19716
			Split = 8,
			// Token: 0x04004D05 RID: 19717
			DoubleDown = 16,
			// Token: 0x04004D06 RID: 19718
			Insurance = 32,
			// Token: 0x04004D07 RID: 19719
			MaxBet = 64,
			// Token: 0x04004D08 RID: 19720
			Abandon = 128
		}

		// Token: 0x02000F11 RID: 3857
		public enum BlackjackRoundResult
		{
			// Token: 0x04004D0A RID: 19722
			None,
			// Token: 0x04004D0B RID: 19723
			Bust,
			// Token: 0x04004D0C RID: 19724
			Loss,
			// Token: 0x04004D0D RID: 19725
			Standoff,
			// Token: 0x04004D0E RID: 19726
			Win,
			// Token: 0x04004D0F RID: 19727
			BlackjackWin
		}

		// Token: 0x02000F12 RID: 3858
		public enum CardsValueMode
		{
			// Token: 0x04004D11 RID: 19729
			Low,
			// Token: 0x04004D12 RID: 19730
			High
		}

		// Token: 0x02000F13 RID: 3859
		private enum BetType
		{
			// Token: 0x04004D14 RID: 19732
			Main,
			// Token: 0x04004D15 RID: 19733
			Split,
			// Token: 0x04004D16 RID: 19734
			Insurance
		}
	}
}
