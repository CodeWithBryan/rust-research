using System;
using System.Collections.Generic;
using PokerEvaluator;
using ProtoBuf;
using Rust;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AB7 RID: 2743
	public class TexasHoldEmController : CardGameController
	{
		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x0600424A RID: 16970 RVA: 0x0004AF67 File Offset: 0x00049167
		public override int MinPlayers
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x0600424B RID: 16971 RVA: 0x000066E9 File Offset: 0x000048E9
		public override int MinBuyIn
		{
			get
			{
				return 100;
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x0600424C RID: 16972 RVA: 0x00183288 File Offset: 0x00181488
		public override int MaxBuyIn
		{
			get
			{
				return 1000;
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x0600424D RID: 16973 RVA: 0x00020C77 File Offset: 0x0001EE77
		public override int MinToPlay
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x0600424E RID: 16974 RVA: 0x0018328F File Offset: 0x0018148F
		// (set) Token: 0x0600424F RID: 16975 RVA: 0x00183297 File Offset: 0x00181497
		public TexasHoldEmController.PokerInputOption LastAction { get; private set; }

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x06004250 RID: 16976 RVA: 0x001832A0 File Offset: 0x001814A0
		// (set) Token: 0x06004251 RID: 16977 RVA: 0x001832A8 File Offset: 0x001814A8
		public ulong LastActionTarget { get; private set; }

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x06004252 RID: 16978 RVA: 0x001832B1 File Offset: 0x001814B1
		// (set) Token: 0x06004253 RID: 16979 RVA: 0x001832B9 File Offset: 0x001814B9
		public int LastActionValue { get; private set; }

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x06004254 RID: 16980 RVA: 0x001832C2 File Offset: 0x001814C2
		// (set) Token: 0x06004255 RID: 16981 RVA: 0x001832CA File Offset: 0x001814CA
		public int BiggestRaiseThisTurn { get; private set; }

		// Token: 0x06004256 RID: 16982 RVA: 0x001832D3 File Offset: 0x001814D3
		public TexasHoldEmController(BaseCardGameEntity owner) : base(owner)
		{
		}

		// Token: 0x06004257 RID: 16983 RVA: 0x001832F4 File Offset: 0x001814F4
		public int GetCurrentBet()
		{
			int num = 0;
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				num = Mathf.Max(num, cardPlayerData.betThisTurn);
			}
			return num;
		}

		// Token: 0x06004258 RID: 16984 RVA: 0x0018334C File Offset: 0x0018154C
		public bool TryGetDealer(out CardPlayerData dealer)
		{
			return base.ToCardPlayerData(this.dealerIndex, true, out dealer);
		}

		// Token: 0x06004259 RID: 16985 RVA: 0x0018335C File Offset: 0x0018155C
		public bool TryGetSmallBlind(out CardPlayerData smallBlind)
		{
			int relIndex = (base.NumPlayersInGame() < 3) ? this.dealerIndex : (this.dealerIndex + 1);
			return base.ToCardPlayerData(relIndex, true, out smallBlind);
		}

		// Token: 0x0600425A RID: 16986 RVA: 0x0018338C File Offset: 0x0018158C
		public bool TryGetBigBlind(out CardPlayerData bigBlind)
		{
			int relIndex = (base.NumPlayersInGame() < 3) ? (this.dealerIndex + 1) : (this.dealerIndex + 2);
			return base.ToCardPlayerData(relIndex, true, out bigBlind);
		}

		// Token: 0x0600425B RID: 16987 RVA: 0x001833C0 File Offset: 0x001815C0
		protected override int GetFirstPlayerRelIndex(bool startOfRound)
		{
			int num = base.NumPlayersInGame();
			if (startOfRound && num == 2)
			{
				return this.dealerIndex;
			}
			return (this.dealerIndex + 1) % num;
		}

		// Token: 0x0600425C RID: 16988 RVA: 0x001833EC File Offset: 0x001815EC
		public static ushort EvaluatePokerHand(List<PlayingCard> cards)
		{
			ushort result = 0;
			int[] array = new int[cards.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = cards[i].GetPokerEvaluationValue();
			}
			if (cards.Count == 5)
			{
				result = PokerLib.Eval5Hand(array);
			}
			else if (cards.Count == 7)
			{
				result = PokerLib.Eval7Hand(array);
			}
			else
			{
				Debug.LogError("Currently we can only evaluate five or seven card hands.");
			}
			return result;
		}

		// Token: 0x0600425D RID: 16989 RVA: 0x00183453 File Offset: 0x00181653
		public int GetCurrentMinRaise(CardPlayerData playerData)
		{
			return Mathf.Max(10, this.GetCurrentBet() - playerData.betThisTurn + this.BiggestRaiseThisTurn);
		}

		// Token: 0x0600425E RID: 16990 RVA: 0x00183470 File Offset: 0x00181670
		public override List<PlayingCard> GetTableCards()
		{
			return this.communityCards;
		}

		// Token: 0x0600425F RID: 16991 RVA: 0x00183478 File Offset: 0x00181678
		public void InputsToList(int availableInputs, List<TexasHoldEmController.PokerInputOption> result)
		{
			foreach (TexasHoldEmController.PokerInputOption pokerInputOption in (TexasHoldEmController.PokerInputOption[])Enum.GetValues(typeof(TexasHoldEmController.PokerInputOption)))
			{
				if (pokerInputOption != TexasHoldEmController.PokerInputOption.None && (availableInputs & (int)pokerInputOption) == (int)pokerInputOption)
				{
					result.Add(pokerInputOption);
				}
			}
		}

		// Token: 0x06004260 RID: 16992 RVA: 0x001834BC File Offset: 0x001816BC
		protected override CardPlayerData GetNewCardPlayerData(int mountIndex)
		{
			if (base.IsServer)
			{
				return new CardPlayerData(base.ScrapItemID, new Func<int, StorageContainer>(base.Owner.GetPlayerStorage), mountIndex, base.IsServer);
			}
			return new CardPlayerData(mountIndex, base.IsServer);
		}

		// Token: 0x06004261 RID: 16993 RVA: 0x001834F8 File Offset: 0x001816F8
		public override void Save(CardGame syncData)
		{
			base.Save(syncData);
			syncData.texasHoldEm = Pool.Get<CardGame.TexasHoldEm>();
			syncData.texasHoldEm.dealerIndex = this.dealerIndex;
			syncData.texasHoldEm.communityCards = Pool.GetList<int>();
			syncData.texasHoldEm.biggestRaiseThisTurn = this.BiggestRaiseThisTurn;
			syncData.lastActionId = (int)this.LastAction;
			syncData.lastActionTarget = this.LastActionTarget;
			syncData.lastActionValue = this.LastActionValue;
			foreach (PlayingCard playingCard in this.communityCards)
			{
				syncData.texasHoldEm.communityCards.Add(playingCard.GetIndex());
			}
			this.ClearLastAction();
		}

		// Token: 0x06004262 RID: 16994 RVA: 0x001835C8 File Offset: 0x001817C8
		protected override void SubStartRound()
		{
			this.communityCards.Clear();
			this.deck = new StackOfCards(1);
			this.BiggestRaiseThisTurn = 0;
			this.ClearLastAction();
			this.IncrementDealer();
			this.DealHoleCards();
			this.activePlayerIndex = this.GetFirstPlayerRelIndex(true);
			base.ServerPlaySound(CardGameSounds.SoundType.Shuffle);
			CardPlayerData cardPlayerData;
			base.TryGetActivePlayer(out cardPlayerData);
			cardPlayerData.availableInputs = this.GetAvailableInputsForPlayer(cardPlayerData);
			if ((cardPlayerData.availableInputs & 32) == 32)
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 32, false, 5, false);
			}
			else
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 4, false, 5, false);
			}
			base.TryGetActivePlayer(out cardPlayerData);
			cardPlayerData.availableInputs = this.GetAvailableInputsForPlayer(cardPlayerData);
			if ((cardPlayerData.availableInputs & 16) == 16)
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 16, false, 10, false);
				return;
			}
			base.ReceivedInputFromPlayer(cardPlayerData, 4, false, 10, false);
		}

		// Token: 0x06004263 RID: 16995 RVA: 0x00183694 File Offset: 0x00181894
		protected override void SubEndRound()
		{
			int num = 0;
			List<CardPlayerData> list = Pool.GetList<CardPlayerData>();
			foreach (CardPlayerData cardPlayerData in base.PlayerData)
			{
				if (cardPlayerData.betThisRound > 0)
				{
					list.Add(cardPlayerData);
				}
				if (cardPlayerData.HasUserInCurrentRound)
				{
					num++;
				}
			}
			if (list.Count == 0)
			{
				base.Owner.GetPot().inventory.Clear();
				return;
			}
			bool flag = num > 1;
			int num2 = base.GetScrapInPot();
			foreach (CardPlayerData cardPlayerData2 in base.PlayerData)
			{
				if (cardPlayerData2.HasUserInGame)
				{
					num2 -= cardPlayerData2.betThisRound;
				}
			}
			bool flag2 = true;
			foreach (CardPlayerData cardPlayerData3 in base.PlayerData)
			{
				cardPlayerData3.remainingToPayOut = cardPlayerData3.betThisRound;
			}
			while (list.Count > 1)
			{
				int num3 = int.MaxValue;
				int num4 = 0;
				foreach (CardPlayerData cardPlayerData4 in base.PlayerData)
				{
					if (cardPlayerData4.betThisRound > 0)
					{
						if (cardPlayerData4.betThisRound < num3)
						{
							num3 = cardPlayerData4.betThisRound;
						}
						num4++;
					}
				}
				int num5 = num3 * num4;
				foreach (CardPlayerData cardPlayerData5 in list)
				{
					cardPlayerData5.betThisRound -= num3;
				}
				int num6 = int.MaxValue;
				foreach (CardPlayerData cardPlayerData6 in base.PlayersInRound())
				{
					if (cardPlayerData6.finalScore < num6)
					{
						num6 = cardPlayerData6.finalScore;
					}
				}
				if (flag2)
				{
					base.resultInfo.winningScore = num6;
				}
				int num7 = 0;
				using (IEnumerator<CardPlayerData> enumerator2 = base.PlayersInRound().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.finalScore == num6)
						{
							num7++;
						}
					}
				}
				int num8 = Mathf.CeilToInt((float)(num5 + num2) / (float)num7);
				num2 = 0;
				foreach (CardPlayerData cardPlayerData7 in base.PlayersInRound())
				{
					if (cardPlayerData7.finalScore == num6)
					{
						if (flag)
						{
							cardPlayerData7.EnableSendingCards();
						}
						base.PayOutFromPot(cardPlayerData7, num8);
						TexasHoldEmController.PokerRoundResult resultCode = flag2 ? TexasHoldEmController.PokerRoundResult.PrimaryWinner : TexasHoldEmController.PokerRoundResult.SecondaryWinner;
						this.AddRoundResult(cardPlayerData7, num8, (int)resultCode);
					}
				}
				for (int j = list.Count - 1; j >= 0; j--)
				{
					if (list[j].betThisRound == 0)
					{
						list.RemoveAt(j);
					}
				}
				flag2 = false;
			}
			if (list.Count == 1)
			{
				int num9 = list[0].betThisRound + num2;
				num2 = 0;
				base.PayOutFromPot(list[0], num9);
				TexasHoldEmController.PokerRoundResult resultCode2 = (base.resultInfo.results.Count == 0) ? TexasHoldEmController.PokerRoundResult.PrimaryWinner : TexasHoldEmController.PokerRoundResult.SecondaryWinner;
				this.AddRoundResult(list[0], num9, (int)resultCode2);
			}
			base.Owner.ClientRPC<CardGame.RoundResults>(null, "OnResultsDeclared", base.resultInfo);
			StorageContainer pot = base.Owner.GetPot();
			int amount = pot.inventory.GetAmount(base.ScrapItemID, true);
			if (amount > 0)
			{
				Debug.LogError(string.Format("{0}: Something went wrong in the winner calculation. Pot still has {1} scrap left over after payouts. Expected 0. Clearing it.", base.GetType().Name, amount));
				pot.inventory.Clear();
			}
			Pool.FreeList<CardPlayerData>(ref list);
		}

		// Token: 0x06004264 RID: 16996 RVA: 0x00183A54 File Offset: 0x00181C54
		protected override void AddRoundResult(CardPlayerData pData, int winnings, int winState)
		{
			base.AddRoundResult(pData, winnings, winState);
			if (GameInfo.HasAchievements)
			{
				global::BasePlayer basePlayer = base.Owner.IDToPlayer(pData.UserID);
				if (basePlayer != null)
				{
					basePlayer.stats.Add("won_hand_texas_holdem", 1, Stats.Steam);
					basePlayer.stats.Save(true);
				}
			}
		}

		// Token: 0x06004265 RID: 16997 RVA: 0x00183AAA File Offset: 0x00181CAA
		protected override void SubEndGameplay()
		{
			this.communityCards.Clear();
		}

		// Token: 0x06004266 RID: 16998 RVA: 0x00183AB8 File Offset: 0x00181CB8
		private void IncrementDealer()
		{
			int num = base.NumPlayersInGame();
			if (num == 0)
			{
				this.dealerIndex = 0;
				return;
			}
			this.dealerIndex = Mathf.Clamp(this.dealerIndex, 0, num - 1);
			int num2 = this.dealerIndex + 1;
			this.dealerIndex = num2;
			this.dealerIndex = num2 % num;
		}

		// Token: 0x06004267 RID: 16999 RVA: 0x00183B08 File Offset: 0x00181D08
		private void DealHoleCards()
		{
			for (int i = 0; i < 2; i++)
			{
				foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
				{
					PlayingCard item;
					if (this.deck.TryTakeCard(out item))
					{
						cardPlayerData.Cards.Add(item);
					}
					else
					{
						Debug.LogError(base.GetType().Name + ": No more cards in the deck to deal!");
					}
				}
			}
			base.SyncAllLocalPlayerCards();
		}

		// Token: 0x06004268 RID: 17000 RVA: 0x00183B98 File Offset: 0x00181D98
		private bool DealCommunityCards()
		{
			if (!base.HasActiveRound)
			{
				return false;
			}
			if (this.communityCards.Count == 0)
			{
				for (int i = 0; i < 3; i++)
				{
					PlayingCard item;
					if (this.deck.TryTakeCard(out item))
					{
						this.communityCards.Add(item);
					}
				}
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				return true;
			}
			if (this.communityCards.Count == 3 || this.communityCards.Count == 4)
			{
				PlayingCard item2;
				if (this.deck.TryTakeCard(out item2))
				{
					this.communityCards.Add(item2);
				}
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				return true;
			}
			return false;
		}

		// Token: 0x06004269 RID: 17001 RVA: 0x00183C2D File Offset: 0x00181E2D
		private void ClearLastAction()
		{
			this.LastAction = TexasHoldEmController.PokerInputOption.None;
			this.LastActionTarget = 0UL;
			this.LastActionValue = 0;
		}

		// Token: 0x0600426A RID: 17002 RVA: 0x00183C48 File Offset: 0x00181E48
		protected override void OnTurnTimeout(CardPlayerData pData)
		{
			CardPlayerData cardPlayerData;
			if (base.TryGetActivePlayer(out cardPlayerData) && cardPlayerData == pData)
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 1, true, 0, false);
			}
		}

		// Token: 0x0600426B RID: 17003 RVA: 0x00183C70 File Offset: 0x00181E70
		protected override void SubReceivedInputFromPlayer(CardPlayerData playerData, int input, int value, bool countAsAction)
		{
			if (!Enum.IsDefined(typeof(TexasHoldEmController.PokerInputOption), input))
			{
				return;
			}
			if (!base.HasActiveRound)
			{
				if (input == 64)
				{
					playerData.EnableSendingCards();
				}
				this.LastActionTarget = playerData.UserID;
				this.LastAction = (TexasHoldEmController.PokerInputOption)input;
				this.LastActionValue = 0;
				return;
			}
			CardPlayerData cardPlayerData;
			if (!base.TryGetActivePlayer(out cardPlayerData))
			{
				return;
			}
			if (cardPlayerData != playerData)
			{
				return;
			}
			bool flag = false;
			if ((playerData.availableInputs & input) != input)
			{
				return;
			}
			if (input <= 8)
			{
				switch (input)
				{
				case 1:
					playerData.LeaveCurrentRound(false, true);
					flag = true;
					this.LastActionValue = 0;
					break;
				case 2:
				{
					int currentBet = this.GetCurrentBet();
					int num = base.TryAddBet(playerData, currentBet - playerData.betThisTurn);
					this.LastActionValue = num;
					break;
				}
				case 3:
					break;
				case 4:
				{
					int currentBet = this.GetCurrentBet();
					int num = base.GoAllIn(playerData);
					this.BiggestRaiseThisTurn = Mathf.Max(this.BiggestRaiseThisTurn, num - currentBet);
					this.LastActionValue = num;
					break;
				}
				default:
					if (input == 8)
					{
						this.LastActionValue = 0;
					}
					break;
				}
			}
			else if (input == 16 || input == 32)
			{
				int currentBet = this.GetCurrentBet();
				int biggestRaiseThisTurn = this.BiggestRaiseThisTurn;
				if (playerData.betThisTurn + value < currentBet + biggestRaiseThisTurn)
				{
					value = currentBet + biggestRaiseThisTurn - playerData.betThisTurn;
				}
				int num = base.TryAddBet(playerData, value);
				this.BiggestRaiseThisTurn = Mathf.Max(this.BiggestRaiseThisTurn, num - currentBet);
				this.LastActionValue = num;
			}
			if (countAsAction && input != 0)
			{
				playerData.SetHasCompletedTurn(true);
			}
			this.LastActionTarget = playerData.UserID;
			this.LastAction = (TexasHoldEmController.PokerInputOption)input;
			if (flag && base.NumPlayersInCurrentRound() == 1)
			{
				base.EndRoundWithDelay();
				return;
			}
			int startIndex = this.activePlayerIndex;
			if (flag)
			{
				if (this.activePlayerIndex > base.NumPlayersInCurrentRound() - 1)
				{
					startIndex = 0;
				}
			}
			else
			{
				startIndex = (this.activePlayerIndex + 1) % base.NumPlayersInCurrentRound();
			}
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			CardPlayerData pData;
			if (base.TryMoveToNextPlayerWithInputs(startIndex, out pData))
			{
				base.StartTurnTimer(pData, this.MaxTurnTime);
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
			this.EndCycle();
		}

		// Token: 0x0600426C RID: 17004 RVA: 0x00183E84 File Offset: 0x00182084
		protected override void StartNextCycle()
		{
			base.StartNextCycle();
			int num = this.GetFirstPlayerRelIndex(false);
			int num2 = base.NumPlayersInGame();
			int num3 = 0;
			CardPlayerData cardPlayerData;
			while (!base.ToCardPlayerData(num, true, out cardPlayerData) || !cardPlayerData.HasUserInCurrentRound)
			{
				num = (num + 1) % num2;
				num3++;
				if (num3 > num2)
				{
					Debug.LogError(base.GetType().Name + ": This should never happen. Ended turn with no players in game?.");
					base.EndRoundWithDelay();
					return;
				}
			}
			int num4 = base.GameToRoundIndex(num);
			if (num4 < 0 || num4 > base.NumPlayersInCurrentRound())
			{
				Debug.LogError(string.Format("StartNextCycle NewActiveIndex is out of range: {0}. Clamping it to between 0 and {1}.", num4, base.NumPlayersInCurrentRound()));
				num4 = Mathf.Clamp(num4, 0, base.NumPlayersInCurrentRound());
			}
			int startIndex = num4;
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			CardPlayerData pData;
			if (base.TryMoveToNextPlayerWithInputs(startIndex, out pData))
			{
				base.StartTurnTimer(pData, this.MaxTurnTime);
				base.UpdateAllAvailableInputs();
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
			this.EndCycle();
		}

		// Token: 0x0600426D RID: 17005 RVA: 0x00183F7C File Offset: 0x0018217C
		protected override bool ShouldEndCycle()
		{
			int num = 0;
			using (IEnumerator<CardPlayerData> enumerator = base.PlayersInRound().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetScrapAmount() > 0)
					{
						num++;
					}
				}
			}
			if (num == 1)
			{
				return true;
			}
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				if (cardPlayerData.GetScrapAmount() > 0 && (cardPlayerData.betThisTurn != this.GetCurrentBet() || !cardPlayerData.hasCompletedTurn))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600426E RID: 17006 RVA: 0x00184030 File Offset: 0x00182230
		protected override void EndCycle()
		{
			CardPlayerData[] playerData = base.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].SetHasCompletedTurn(false);
			}
			this.BiggestRaiseThisTurn = 0;
			if (this.DealCommunityCards())
			{
				base.QueueNextCycleInvoke();
				return;
			}
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				List<PlayingCard> list = Pool.GetList<PlayingCard>();
				list.AddRange(cardPlayerData.Cards);
				list.AddRange(this.communityCards);
				ushort finalScore = TexasHoldEmController.EvaluatePokerHand(list);
				Pool.FreeList<PlayingCard>(ref list);
				cardPlayerData.finalScore = (int)finalScore;
			}
			base.EndRoundWithDelay();
		}

		// Token: 0x0600426F RID: 17007 RVA: 0x001840EC File Offset: 0x001822EC
		protected override int GetAvailableInputsForPlayer(CardPlayerData playerData)
		{
			TexasHoldEmController.PokerInputOption pokerInputOption = TexasHoldEmController.PokerInputOption.None;
			if (playerData == null || this.isWaitingBetweenTurns)
			{
				return (int)pokerInputOption;
			}
			if (!base.HasActiveRound)
			{
				if (!playerData.LeftRoundEarly && playerData.Cards.Count > 0 && !playerData.SendCardDetails)
				{
					pokerInputOption |= TexasHoldEmController.PokerInputOption.RevealHand;
				}
				return (int)pokerInputOption;
			}
			CardPlayerData cardPlayerData;
			if (!base.TryGetActivePlayer(out cardPlayerData) || playerData != cardPlayerData)
			{
				return (int)pokerInputOption;
			}
			int scrapAmount = playerData.GetScrapAmount();
			if (scrapAmount > 0)
			{
				pokerInputOption |= TexasHoldEmController.PokerInputOption.AllIn;
				pokerInputOption |= TexasHoldEmController.PokerInputOption.Fold;
				int currentBet = this.GetCurrentBet();
				if (playerData.betThisTurn >= currentBet)
				{
					pokerInputOption |= TexasHoldEmController.PokerInputOption.Check;
				}
				if (currentBet > playerData.betThisTurn && scrapAmount >= currentBet - playerData.betThisTurn)
				{
					pokerInputOption |= TexasHoldEmController.PokerInputOption.Call;
				}
				if (scrapAmount >= this.GetCurrentMinRaise(playerData))
				{
					if (this.BiggestRaiseThisTurn == 0)
					{
						pokerInputOption |= TexasHoldEmController.PokerInputOption.Bet;
					}
					else
					{
						pokerInputOption |= TexasHoldEmController.PokerInputOption.Raise;
					}
				}
			}
			return (int)pokerInputOption;
		}

		// Token: 0x06004270 RID: 17008 RVA: 0x001841A3 File Offset: 0x001823A3
		protected override void HandlePlayerLeavingDuringTheirTurn(CardPlayerData pData)
		{
			base.ReceivedInputFromPlayer(pData, 1, true, 0, false);
		}

		// Token: 0x04003AAF RID: 15023
		public List<PlayingCard> communityCards = new List<PlayingCard>();

		// Token: 0x04003AB0 RID: 15024
		public const int SMALL_BLIND = 5;

		// Token: 0x04003AB1 RID: 15025
		public const int BIG_BLIND = 10;

		// Token: 0x04003AB2 RID: 15026
		public const string WON_HAND_STAT = "won_hand_texas_holdem";

		// Token: 0x04003AB7 RID: 15031
		private int dealerIndex;

		// Token: 0x04003AB8 RID: 15032
		private StackOfCards deck = new StackOfCards(1);

		// Token: 0x02000F1B RID: 3867
		[Flags]
		public enum PokerInputOption
		{
			// Token: 0x04004D34 RID: 19764
			None = 0,
			// Token: 0x04004D35 RID: 19765
			Fold = 1,
			// Token: 0x04004D36 RID: 19766
			Call = 2,
			// Token: 0x04004D37 RID: 19767
			AllIn = 4,
			// Token: 0x04004D38 RID: 19768
			Check = 8,
			// Token: 0x04004D39 RID: 19769
			Raise = 16,
			// Token: 0x04004D3A RID: 19770
			Bet = 32,
			// Token: 0x04004D3B RID: 19771
			RevealHand = 64
		}

		// Token: 0x02000F1C RID: 3868
		public enum PokerRoundResult
		{
			// Token: 0x04004D3D RID: 19773
			Loss,
			// Token: 0x04004D3E RID: 19774
			PrimaryWinner,
			// Token: 0x04004D3F RID: 19775
			SecondaryWinner
		}
	}
}
