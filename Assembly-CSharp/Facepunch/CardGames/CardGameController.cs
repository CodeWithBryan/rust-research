using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using ProtoBuf;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AB0 RID: 2736
	public abstract class CardGameController : IDisposable
	{
		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x060041B0 RID: 16816 RVA: 0x00181736 File Offset: 0x0017F936
		// (set) Token: 0x060041B1 RID: 16817 RVA: 0x0018173E File Offset: 0x0017F93E
		public CardGameController.CardGameState State { get; private set; }

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x060041B2 RID: 16818 RVA: 0x00181747 File Offset: 0x0017F947
		public bool HasGameInProgress
		{
			get
			{
				return this.State >= CardGameController.CardGameState.InGameBetweenRounds;
			}
		}

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x060041B3 RID: 16819 RVA: 0x00181755 File Offset: 0x0017F955
		public bool HasRoundInProgressOrEnding
		{
			get
			{
				return this.State == CardGameController.CardGameState.InGameRound || this.State == CardGameController.CardGameState.InGameRoundEnding;
			}
		}

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x060041B4 RID: 16820 RVA: 0x0018176B File Offset: 0x0017F96B
		public bool HasActiveRound
		{
			get
			{
				return this.State == CardGameController.CardGameState.InGameRound;
			}
		}

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x060041B5 RID: 16821 RVA: 0x00181776 File Offset: 0x0017F976
		// (set) Token: 0x060041B6 RID: 16822 RVA: 0x0018177E File Offset: 0x0017F97E
		public CardPlayerData[] PlayerData { get; private set; }

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x060041B7 RID: 16823
		public abstract int MinPlayers { get; }

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x060041B8 RID: 16824
		public abstract int MinBuyIn { get; }

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x060041B9 RID: 16825
		public abstract int MaxBuyIn { get; }

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x060041BA RID: 16826
		public abstract int MinToPlay { get; }

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x060041BB RID: 16827 RVA: 0x00181787 File Offset: 0x0017F987
		public virtual float MaxTurnTime
		{
			get
			{
				return 30f;
			}
		}

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x060041BC RID: 16828 RVA: 0x00007074 File Offset: 0x00005274
		public virtual int EndRoundDelay
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x060041BD RID: 16829 RVA: 0x000B76EB File Offset: 0x000B58EB
		public virtual int TimeBetweenRounds
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x060041BE RID: 16830 RVA: 0x000062DD File Offset: 0x000044DD
		protected virtual float TimeBetweenTurns
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x060041BF RID: 16831 RVA: 0x0018178E File Offset: 0x0017F98E
		// (set) Token: 0x060041C0 RID: 16832 RVA: 0x00181796 File Offset: 0x0017F996
		private protected BaseCardGameEntity Owner { protected get; private set; }

		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x060041C1 RID: 16833 RVA: 0x0018179F File Offset: 0x0017F99F
		protected int ScrapItemID
		{
			get
			{
				return this.Owner.ScrapItemID;
			}
		}

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x060041C2 RID: 16834 RVA: 0x001817AC File Offset: 0x0017F9AC
		protected bool IsServer
		{
			get
			{
				return this.Owner.isServer;
			}
		}

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x060041C3 RID: 16835 RVA: 0x001817B9 File Offset: 0x0017F9B9
		protected bool IsClient
		{
			get
			{
				return this.Owner.isClient;
			}
		}

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x060041C4 RID: 16836 RVA: 0x001817C6 File Offset: 0x0017F9C6
		// (set) Token: 0x060041C5 RID: 16837 RVA: 0x001817CE File Offset: 0x0017F9CE
		public CardGame.RoundResults resultInfo { get; private set; }

		// Token: 0x060041C6 RID: 16838 RVA: 0x001817D8 File Offset: 0x0017F9D8
		public CardGameController(BaseCardGameEntity owner)
		{
			this.Owner = owner;
			this.PlayerData = new CardPlayerData[this.MaxPlayersAtTable()];
			this.resultInfo = Pool.Get<CardGame.RoundResults>();
			this.resultInfo.results = Pool.GetList<CardGame.RoundResults.Result>();
			this.localPlayerCards = Pool.Get<CardGame.CardList>();
			this.localPlayerCards.cards = Pool.GetList<int>();
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				this.PlayerData[i] = this.GetNewCardPlayerData(i);
			}
		}

		// Token: 0x060041C7 RID: 16839 RVA: 0x0018185B File Offset: 0x0017FA5B
		public IEnumerable<CardPlayerData> PlayersInRound()
		{
			foreach (CardPlayerData cardPlayerData in this.PlayerData)
			{
				if (cardPlayerData.HasUserInCurrentRound)
				{
					yield return cardPlayerData;
				}
			}
			CardPlayerData[] array = null;
			yield break;
		}

		// Token: 0x060041C8 RID: 16840
		protected abstract int GetFirstPlayerRelIndex(bool startOfRound);

		// Token: 0x060041C9 RID: 16841 RVA: 0x0018186C File Offset: 0x0017FA6C
		public void Dispose()
		{
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				this.PlayerData[i].Dispose();
			}
			this.localPlayerCards.Dispose();
			this.resultInfo.Dispose();
		}

		// Token: 0x060041CA RID: 16842 RVA: 0x001818B0 File Offset: 0x0017FAB0
		public int NumPlayersAllowedToPlay(CardPlayerData ignore = null)
		{
			int num = 0;
			foreach (CardPlayerData cardPlayerData in this.PlayerData)
			{
				if (cardPlayerData != ignore && this.IsAllowedToPlay(cardPlayerData))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060041CB RID: 16843 RVA: 0x001818EC File Offset: 0x0017FAEC
		public CardGameController.Playability GetPlayabilityStatus(CardPlayerData cpd)
		{
			if (!cpd.HasUser)
			{
				return CardGameController.Playability.NoPlayer;
			}
			int scrapAmount = cpd.GetScrapAmount();
			if (cpd.HasUserInGame)
			{
				if (scrapAmount < this.MinToPlay)
				{
					return CardGameController.Playability.RanOutOfScrap;
				}
			}
			else
			{
				if (scrapAmount < this.MinBuyIn)
				{
					return CardGameController.Playability.NotEnoughBuyIn;
				}
				if (scrapAmount > this.MaxBuyIn)
				{
					return CardGameController.Playability.TooMuchBuyIn;
				}
			}
			return CardGameController.Playability.OK;
		}

		// Token: 0x060041CC RID: 16844 RVA: 0x00181934 File Offset: 0x0017FB34
		public bool TryGetActivePlayer(out CardPlayerData activePlayer)
		{
			return this.ToCardPlayerData(this.activePlayerIndex, false, out activePlayer);
		}

		// Token: 0x060041CD RID: 16845 RVA: 0x00181944 File Offset: 0x0017FB44
		protected bool ToCardPlayerData(int relIndex, bool includeOutOfRound, out CardPlayerData result)
		{
			if (!this.HasRoundInProgressOrEnding)
			{
				Debug.LogWarning(base.GetType().Name + ": Tried to call ToCardPlayerData while no round was in progress. Returning null.");
				result = null;
				return false;
			}
			int num = includeOutOfRound ? this.NumPlayersInGame() : this.NumPlayersInCurrentRound();
			int index = this.RelToAbsIndex(relIndex % num, includeOutOfRound);
			return this.TryGetCardPlayerData(index, out result);
		}

		// Token: 0x060041CE RID: 16846 RVA: 0x001819A0 File Offset: 0x0017FBA0
		public int RelToAbsIndex(int relIndex, bool includeFolded)
		{
			if (!this.HasRoundInProgressOrEnding)
			{
				Debug.LogError(base.GetType().Name + ": Called RelToAbsIndex outside of a round. No-one is playing. Returning -1.");
				return -1;
			}
			int num = 0;
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				if (includeFolded ? this.PlayerData[i].HasUserInGame : this.PlayerData[i].HasUserInCurrentRound)
				{
					if (num == relIndex)
					{
						return i;
					}
					num++;
				}
			}
			Debug.LogError(string.Format("{0}: No absolute index found for relative index {1}. Only {2} total players are in the round. Returning -1.", base.GetType().Name, relIndex, this.NumPlayersInCurrentRound()));
			return -1;
		}

		// Token: 0x060041CF RID: 16847 RVA: 0x00181A40 File Offset: 0x0017FC40
		public int GameToRoundIndex(int gameRelIndex)
		{
			if (!this.HasRoundInProgressOrEnding)
			{
				Debug.LogError(base.GetType().Name + ": Called GameToRoundIndex outside of a round. No-one is playing. Returning 0.");
				return 0;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				if (this.PlayerData[i].HasUserInCurrentRound)
				{
					if (num == gameRelIndex)
					{
						return num2;
					}
					num++;
					num2++;
				}
				else if (this.PlayerData[i].HasUserInGame)
				{
					if (num == gameRelIndex)
					{
						return num2;
					}
					num++;
				}
			}
			Debug.LogError(string.Format("{0}: No round index found for game index {1}. Only {2} total players are in the round. Returning 0.", base.GetType().Name, gameRelIndex, this.NumPlayersInCurrentRound()));
			return 0;
		}

		// Token: 0x060041D0 RID: 16848 RVA: 0x00181AEC File Offset: 0x0017FCEC
		public int NumPlayersInGame()
		{
			int num = 0;
			CardPlayerData[] playerData = this.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				if (playerData[i].HasUserInGame)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060041D1 RID: 16849 RVA: 0x00181B20 File Offset: 0x0017FD20
		public int NumPlayersInCurrentRound()
		{
			int num = 0;
			CardPlayerData[] playerData = this.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				if (playerData[i].HasUserInCurrentRound)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060041D2 RID: 16850 RVA: 0x00181B53 File Offset: 0x0017FD53
		public int MaxPlayersAtTable()
		{
			return this.Owner.mountPoints.Count;
		}

		// Token: 0x060041D3 RID: 16851 RVA: 0x00181B68 File Offset: 0x0017FD68
		public bool PlayerIsInGame(global::BasePlayer player)
		{
			return this.PlayerData.Any((CardPlayerData data) => data.HasUserInGame && data.UserID == player.userID);
		}

		// Token: 0x060041D4 RID: 16852 RVA: 0x00181B99 File Offset: 0x0017FD99
		public bool IsAtTable(global::BasePlayer player)
		{
			return this.IsAtTable(player.userID);
		}

		// Token: 0x060041D5 RID: 16853 RVA: 0x0002A0CF File Offset: 0x000282CF
		public virtual List<PlayingCard> GetTableCards()
		{
			return null;
		}

		// Token: 0x060041D6 RID: 16854 RVA: 0x00181BA7 File Offset: 0x0017FDA7
		public void StartTurnTimer(CardPlayerData pData, float turnTime)
		{
			if (this.IsServer)
			{
				pData.StartTurnTimer(new Action<CardPlayerData>(this.OnTurnTimeout), turnTime);
				this.Owner.ClientRPC<int, float>(null, "ClientStartTurnTimer", pData.mountIndex, turnTime);
			}
		}

		// Token: 0x060041D7 RID: 16855 RVA: 0x00181BE0 File Offset: 0x0017FDE0
		private bool IsAtTable(ulong userID)
		{
			return this.PlayerData.Any((CardPlayerData data) => data.UserID == userID);
		}

		// Token: 0x060041D8 RID: 16856 RVA: 0x00181C14 File Offset: 0x0017FE14
		public int GetScrapInPot()
		{
			if (!this.IsServer)
			{
				return 0;
			}
			StorageContainer pot = this.Owner.GetPot();
			if (pot != null)
			{
				return pot.inventory.GetAmount(this.ScrapItemID, true);
			}
			return 0;
		}

		// Token: 0x060041D9 RID: 16857 RVA: 0x00181C54 File Offset: 0x0017FE54
		public bool TryGetCardPlayerData(int index, out CardPlayerData cardPlayer)
		{
			if (index >= 0 && index < this.PlayerData.Length)
			{
				cardPlayer = this.PlayerData[index];
				return true;
			}
			cardPlayer = null;
			return false;
		}

		// Token: 0x060041DA RID: 16858 RVA: 0x00181C78 File Offset: 0x0017FE78
		public bool TryGetCardPlayerData(ulong forPlayer, out CardPlayerData cardPlayer)
		{
			foreach (CardPlayerData cardPlayerData in this.PlayerData)
			{
				if (cardPlayerData.UserID == forPlayer)
				{
					cardPlayer = cardPlayerData;
					return true;
				}
			}
			cardPlayer = null;
			return false;
		}

		// Token: 0x060041DB RID: 16859 RVA: 0x00181CB0 File Offset: 0x0017FEB0
		public bool TryGetCardPlayerData(global::BasePlayer forPlayer, out CardPlayerData cardPlayer)
		{
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				if (this.PlayerData[i].UserID == forPlayer.userID)
				{
					cardPlayer = this.PlayerData[i];
					return true;
				}
			}
			cardPlayer = null;
			return false;
		}

		// Token: 0x060041DC RID: 16860 RVA: 0x00181CF5 File Offset: 0x0017FEF5
		public bool IsAllowedToPlay(CardPlayerData cpd)
		{
			return this.GetPlayabilityStatus(cpd) == CardGameController.Playability.OK;
		}

		// Token: 0x060041DD RID: 16861 RVA: 0x00181D04 File Offset: 0x0017FF04
		protected void ClearResultsInfo()
		{
			if (this.resultInfo.results != null)
			{
				foreach (CardGame.RoundResults.Result result in this.resultInfo.results)
				{
					if (result != null)
					{
						result.Dispose();
					}
				}
				this.resultInfo.results.Clear();
			}
		}

		// Token: 0x060041DE RID: 16862
		protected abstract CardPlayerData GetNewCardPlayerData(int mountIndex);

		// Token: 0x060041DF RID: 16863
		protected abstract void OnTurnTimeout(CardPlayerData playerData);

		// Token: 0x060041E0 RID: 16864
		protected abstract void SubStartRound();

		// Token: 0x060041E1 RID: 16865
		protected abstract void SubReceivedInputFromPlayer(CardPlayerData playerData, int input, int value, bool countAsAction);

		// Token: 0x060041E2 RID: 16866
		protected abstract int GetAvailableInputsForPlayer(CardPlayerData playerData);

		// Token: 0x060041E3 RID: 16867
		protected abstract void HandlePlayerLeavingDuringTheirTurn(CardPlayerData pData);

		// Token: 0x060041E4 RID: 16868
		protected abstract void SubEndRound();

		// Token: 0x060041E5 RID: 16869
		protected abstract void SubEndGameplay();

		// Token: 0x060041E6 RID: 16870
		protected abstract void EndCycle();

		// Token: 0x060041E7 RID: 16871
		protected abstract bool ShouldEndCycle();

		// Token: 0x060041E8 RID: 16872 RVA: 0x000059DD File Offset: 0x00003BDD
		public void EditorMakeRandomMove()
		{
		}

		// Token: 0x060041E9 RID: 16873 RVA: 0x00181D7C File Offset: 0x0017FF7C
		public void JoinTable(global::BasePlayer player)
		{
			this.JoinTable(player.userID);
		}

		// Token: 0x060041EA RID: 16874 RVA: 0x00181D8C File Offset: 0x0017FF8C
		protected void SyncAllLocalPlayerCards()
		{
			foreach (CardPlayerData pData in this.PlayerData)
			{
				this.SyncLocalPlayerCards(pData);
			}
		}

		// Token: 0x060041EB RID: 16875 RVA: 0x00181DBC File Offset: 0x0017FFBC
		protected void SyncLocalPlayerCards(CardPlayerData pData)
		{
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(pData.UserID);
			if (basePlayer == null)
			{
				return;
			}
			this.localPlayerCards.cards.Clear();
			foreach (PlayingCard playingCard in pData.Cards)
			{
				this.localPlayerCards.cards.Add(playingCard.GetIndex());
			}
			this.Owner.ClientRPCPlayer<CardGame.CardList>(null, basePlayer, "ReceiveCardsForPlayer", this.localPlayerCards);
		}

		// Token: 0x060041EC RID: 16876 RVA: 0x00181E5C File Offset: 0x0018005C
		private void JoinTable(ulong userID)
		{
			if (this.IsAtTable(userID))
			{
				return;
			}
			if (this.NumPlayersAllowedToPlay(null) >= this.MaxPlayersAtTable())
			{
				return;
			}
			int mountPointIndex = this.Owner.GetMountPointIndex(userID);
			if (mountPointIndex < 0)
			{
				return;
			}
			this.PlayerData[mountPointIndex].AddUser(userID);
			if (!this.HasGameInProgress)
			{
				if (!this.TryStartNewRound())
				{
					this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
					return;
				}
			}
			else
			{
				this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}

		// Token: 0x060041ED RID: 16877 RVA: 0x00181ECC File Offset: 0x001800CC
		public void LeaveTable(ulong userID)
		{
			CardPlayerData pData;
			if (this.TryGetCardPlayerData(userID, out pData))
			{
				this.LeaveTable(pData);
			}
		}

		// Token: 0x060041EE RID: 16878 RVA: 0x00181EEC File Offset: 0x001800EC
		public void LeaveTable(CardPlayerData pData)
		{
			CardPlayerData cardPlayerData;
			if (this.HasActiveRound && this.TryGetActivePlayer(out cardPlayerData))
			{
				if (pData == cardPlayerData)
				{
					this.HandlePlayerLeavingDuringTheirTurn(cardPlayerData);
				}
				else if (pData.HasUserInCurrentRound && pData.mountIndex < cardPlayerData.mountIndex && this.activePlayerIndex > 0)
				{
					this.activePlayerIndex--;
				}
			}
			pData.ClearAllData();
			if (this.HasActiveRound && this.NumPlayersInCurrentRound() < this.MinPlayers)
			{
				this.EndRoundWithDelay();
			}
			if (pData.HasUserInGame)
			{
				this.Owner.ClientRPC<ulong>(null, "ClientOnPlayerLeft", pData.UserID);
			}
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x060041EF RID: 16879 RVA: 0x00181F94 File Offset: 0x00180194
		protected int TryAddBet(CardPlayerData playerData, int maxAmount)
		{
			int num = this.TryMoveToPotStorage(playerData, maxAmount);
			playerData.betThisRound += num;
			playerData.betThisTurn += num;
			return num;
		}

		// Token: 0x060041F0 RID: 16880 RVA: 0x00181FC8 File Offset: 0x001801C8
		protected int GoAllIn(CardPlayerData playerData)
		{
			int num = this.TryMoveToPotStorage(playerData, 999999);
			playerData.betThisRound += num;
			playerData.betThisTurn += num;
			return num;
		}

		// Token: 0x060041F1 RID: 16881 RVA: 0x00182000 File Offset: 0x00180200
		protected int TryMoveToPotStorage(CardPlayerData playerData, int maxAmount)
		{
			int num = 0;
			StorageContainer storage = playerData.GetStorage();
			StorageContainer pot = this.Owner.GetPot();
			if (storage != null && pot != null)
			{
				List<global::Item> list = Pool.GetList<global::Item>();
				num = storage.inventory.Take(list, this.ScrapItemID, maxAmount);
				if (num > 0)
				{
					foreach (global::Item item in list)
					{
						if (!item.MoveToContainer(pot.inventory, -1, true, true, null, true))
						{
							item.MoveToContainer(storage.inventory, -1, true, false, null, true);
						}
					}
				}
				Pool.FreeList<global::Item>(ref list);
			}
			else
			{
				Debug.LogError(base.GetType().Name + ": TryAddToPot: Null storage.");
			}
			return num;
		}

		// Token: 0x060041F2 RID: 16882 RVA: 0x001820DC File Offset: 0x001802DC
		protected int PayOutFromPot(CardPlayerData playerData, int maxAmount)
		{
			int num = 0;
			StorageContainer storage = playerData.GetStorage();
			StorageContainer pot = this.Owner.GetPot();
			if (storage != null && pot != null)
			{
				List<global::Item> list = Pool.GetList<global::Item>();
				num = pot.inventory.Take(list, this.ScrapItemID, maxAmount);
				if (num > 0)
				{
					foreach (global::Item item in list)
					{
						if (!item.MoveToContainer(storage.inventory, -1, true, true, null, true))
						{
							item.MoveToContainer(pot.inventory, -1, true, false, null, true);
						}
					}
				}
				Pool.FreeList<global::Item>(ref list);
			}
			else
			{
				Debug.LogError(base.GetType().Name + ": PayOut: Null storage.");
			}
			return num;
		}

		// Token: 0x060041F3 RID: 16883 RVA: 0x001821B8 File Offset: 0x001803B8
		protected int PayOutAllFromPot(CardPlayerData playerData)
		{
			return this.PayOutFromPot(playerData, int.MaxValue);
		}

		// Token: 0x060041F4 RID: 16884 RVA: 0x001821C8 File Offset: 0x001803C8
		protected void ClearPot()
		{
			StorageContainer pot = this.Owner.GetPot();
			if (pot != null)
			{
				pot.inventory.Clear();
			}
		}

		// Token: 0x060041F5 RID: 16885 RVA: 0x001821F8 File Offset: 0x001803F8
		protected int RemoveScrapFromStorage(CardPlayerData data)
		{
			StorageContainer storage = data.GetStorage();
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(data.UserID);
			int num = 0;
			if (basePlayer != null)
			{
				List<global::Item> list = Pool.GetList<global::Item>();
				num = storage.inventory.Take(list, this.ScrapItemID, int.MaxValue);
				if (num > 0)
				{
					foreach (global::Item item in list)
					{
						if (!item.MoveToContainer(basePlayer.inventory.containerMain, -1, true, true, null, true))
						{
							item.MoveToContainer(storage.inventory, -1, true, false, null, true);
						}
					}
				}
				Pool.FreeList<global::Item>(ref list);
			}
			return num;
		}

		// Token: 0x060041F6 RID: 16886 RVA: 0x001822B8 File Offset: 0x001804B8
		public virtual void Save(CardGame syncData)
		{
			syncData.players = Pool.GetList<CardGame.CardPlayer>();
			syncData.state = (int)this.State;
			syncData.activePlayerIndex = this.activePlayerIndex;
			CardPlayerData[] playerData = this.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].Save(syncData);
			}
			syncData.pot = this.GetScrapInPot();
		}

		// Token: 0x060041F7 RID: 16887 RVA: 0x00182312 File Offset: 0x00180512
		private void InvokeStartNewRound()
		{
			this.TryStartNewRound();
		}

		// Token: 0x060041F8 RID: 16888 RVA: 0x0018231C File Offset: 0x0018051C
		private bool TryStartNewRound()
		{
			if (this.HasRoundInProgressOrEnding)
			{
				return false;
			}
			foreach (CardPlayerData cardPlayerData in this.PlayerData)
			{
				global::BasePlayer basePlayer;
				if (this.State == CardGameController.CardGameState.NotPlaying)
				{
					cardPlayerData.lastActionTime = Time.unscaledTime;
				}
				else if (cardPlayerData.HasBeenIdleFor(240) && global::BasePlayer.TryFindByID(cardPlayerData.UserID, out basePlayer))
				{
					basePlayer.GetMounted().DismountPlayer(basePlayer, false);
				}
			}
			if (this.NumPlayersAllowedToPlay(null) < this.MinPlayers)
			{
				this.EndGameplay();
				return false;
			}
			foreach (CardPlayerData cardPlayerData2 in this.PlayerData)
			{
				if (this.IsAllowedToPlay(cardPlayerData2))
				{
					cardPlayerData2.JoinRound();
				}
				else
				{
					cardPlayerData2.LeaveGame();
				}
			}
			this.State = CardGameController.CardGameState.InGameRound;
			this.SubStartRound();
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return true;
		}

		// Token: 0x060041F9 RID: 16889 RVA: 0x001823EF File Offset: 0x001805EF
		protected void BeginRoundEnd()
		{
			this.State = CardGameController.CardGameState.InGameRoundEnding;
			this.CancelNextCycleInvoke();
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x060041FA RID: 16890 RVA: 0x0018240A File Offset: 0x0018060A
		protected void EndRoundWithDelay()
		{
			this.State = CardGameController.CardGameState.InGameRoundEnding;
			this.CancelNextCycleInvoke();
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.Owner.Invoke(new Action(this.EndRound), (float)this.EndRoundDelay);
		}

		// Token: 0x060041FB RID: 16891 RVA: 0x00182444 File Offset: 0x00180644
		private void EndRound()
		{
			this.State = CardGameController.CardGameState.InGameBetweenRounds;
			this.CancelNextCycleInvoke();
			this.ClearResultsInfo();
			this.SubEndRound();
			foreach (CardPlayerData cardPlayerData in this.PlayersInRound())
			{
				global::BasePlayer basePlayer = global::BasePlayer.FindByID(cardPlayerData.UserID);
				if (basePlayer != null && basePlayer.metabolism.CanConsume())
				{
					basePlayer.metabolism.MarkConsumption();
					basePlayer.metabolism.ApplyChange(MetabolismAttribute.Type.Calories, 2f, 0f);
					basePlayer.metabolism.ApplyChange(MetabolismAttribute.Type.Hydration, 2f, 0f);
				}
				cardPlayerData.LeaveCurrentRound(true, false);
			}
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.Owner.Invoke(new Action(this.InvokeStartNewRound), (float)this.TimeBetweenRounds);
		}

		// Token: 0x060041FC RID: 16892 RVA: 0x0018252C File Offset: 0x0018072C
		protected virtual void AddRoundResult(CardPlayerData pData, int winnings, int resultCode)
		{
			foreach (CardGame.RoundResults.Result result in this.resultInfo.results)
			{
				if (result.ID == pData.UserID)
				{
					result.winnings += winnings;
					return;
				}
			}
			CardGame.RoundResults.Result result2 = Pool.Get<CardGame.RoundResults.Result>();
			result2.ID = pData.UserID;
			result2.winnings = winnings;
			result2.resultCode = resultCode;
			this.resultInfo.results.Add(result2);
		}

		// Token: 0x060041FD RID: 16893 RVA: 0x001825CC File Offset: 0x001807CC
		protected void EndGameplay()
		{
			if (!this.HasGameInProgress)
			{
				return;
			}
			this.CancelNextCycleInvoke();
			this.SubEndGameplay();
			this.State = CardGameController.CardGameState.NotPlaying;
			CardPlayerData[] playerData = this.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].LeaveGame();
			}
			this.SyncAllLocalPlayerCards();
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x060041FE RID: 16894 RVA: 0x00182624 File Offset: 0x00180824
		public void ReceivedInputFromPlayer(global::BasePlayer player, int input, bool countAsAction, int value = 0)
		{
			if (player == null)
			{
				return;
			}
			player.ResetInputIdleTime();
			CardPlayerData pData;
			if (this.TryGetCardPlayerData(player, out pData))
			{
				this.ReceivedInputFromPlayer(pData, input, countAsAction, value, true);
			}
		}

		// Token: 0x060041FF RID: 16895 RVA: 0x00182658 File Offset: 0x00180858
		protected void ReceivedInputFromPlayer(CardPlayerData pData, int input, bool countAsAction, int value = 0, bool playerInitiated = true)
		{
			if (!this.HasGameInProgress)
			{
				return;
			}
			if (pData == null)
			{
				return;
			}
			if (playerInitiated)
			{
				pData.lastActionTime = Time.unscaledTime;
			}
			this.SubReceivedInputFromPlayer(pData, input, value, countAsAction);
			if (this.HasActiveRound)
			{
				this.UpdateAllAvailableInputs();
				this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}

		// Token: 0x06004200 RID: 16896 RVA: 0x001826A8 File Offset: 0x001808A8
		protected void UpdateAllAvailableInputs()
		{
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				this.PlayerData[i].availableInputs = this.GetAvailableInputsForPlayer(this.PlayerData[i]);
			}
		}

		// Token: 0x06004201 RID: 16897 RVA: 0x001826E3 File Offset: 0x001808E3
		public void PlayerStorageChanged()
		{
			if (!this.HasGameInProgress)
			{
				this.TryStartNewRound();
			}
		}

		// Token: 0x06004202 RID: 16898 RVA: 0x001826F4 File Offset: 0x001808F4
		protected void ServerPlaySound(CardGameSounds.SoundType type)
		{
			this.Owner.ClientRPC<int>(null, "ClientPlaySound", (int)type);
		}

		// Token: 0x06004203 RID: 16899 RVA: 0x00182708 File Offset: 0x00180908
		public void GetConnectionsInGame(List<Connection> connections)
		{
			foreach (CardPlayerData cardPlayerData in this.PlayerData)
			{
				global::BasePlayer basePlayer;
				if (cardPlayerData.HasUserInGame && global::BasePlayer.TryFindByID(cardPlayerData.UserID, out basePlayer))
				{
					connections.Add(basePlayer.net.connection);
				}
			}
		}

		// Token: 0x06004204 RID: 16900 RVA: 0x00182758 File Offset: 0x00180958
		public virtual void OnTableDestroyed()
		{
			if (this.HasGameInProgress)
			{
				foreach (CardPlayerData cardPlayerData in this.PlayerData)
				{
					if (cardPlayerData.HasUserInGame)
					{
						this.PayOutFromPot(cardPlayerData, cardPlayerData.GetTotalBetThisRound());
					}
				}
				if (this.GetScrapInPot() > 0)
				{
					int maxAmount = this.GetScrapInPot() / this.NumPlayersInGame();
					foreach (CardPlayerData cardPlayerData2 in this.PlayerData)
					{
						if (cardPlayerData2.HasUserInGame)
						{
							this.PayOutFromPot(cardPlayerData2, maxAmount);
						}
					}
				}
			}
			foreach (CardPlayerData cardPlayerData3 in this.PlayerData)
			{
				if (cardPlayerData3.HasUser)
				{
					this.RemoveScrapFromStorage(cardPlayerData3);
				}
			}
		}

		// Token: 0x06004205 RID: 16901 RVA: 0x0018280C File Offset: 0x00180A0C
		protected bool TryMoveToNextPlayerWithInputs(int startIndex, out CardPlayerData newActivePlayer)
		{
			this.activePlayerIndex = startIndex;
			this.TryGetActivePlayer(out newActivePlayer);
			int num = 0;
			bool flag = false;
			while (this.GetAvailableInputsForPlayer(newActivePlayer) == 0)
			{
				if (num == this.NumPlayersInCurrentRound())
				{
					flag = true;
					break;
				}
				this.activePlayerIndex = (this.activePlayerIndex + 1) % this.NumPlayersInCurrentRound();
				this.TryGetActivePlayer(out newActivePlayer);
				num++;
			}
			return !flag;
		}

		// Token: 0x06004206 RID: 16902 RVA: 0x0018286A File Offset: 0x00180A6A
		protected virtual void StartNextCycle()
		{
			this.isWaitingBetweenTurns = false;
		}

		// Token: 0x06004207 RID: 16903 RVA: 0x00182873 File Offset: 0x00180A73
		protected void QueueNextCycleInvoke()
		{
			SingletonComponent<InvokeHandler>.Instance.Invoke(new Action(this.StartNextCycle), this.TimeBetweenTurns);
			this.isWaitingBetweenTurns = true;
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x06004208 RID: 16904 RVA: 0x001828A5 File Offset: 0x00180AA5
		private void CancelNextCycleInvoke()
		{
			SingletonComponent<InvokeHandler>.Instance.CancelInvoke(new Action(this.StartNextCycle));
			this.isWaitingBetweenTurns = false;
		}

		// Token: 0x04003A7A RID: 14970
		public const int IDLE_KICK_SECONDS = 240;

		// Token: 0x04003A7D RID: 14973
		private CardGame.CardList localPlayerCards;

		// Token: 0x04003A7E RID: 14974
		protected int activePlayerIndex;

		// Token: 0x04003A7F RID: 14975
		public const int STD_RAISE_INCREMENTS = 5;

		// Token: 0x04003A80 RID: 14976
		protected bool isWaitingBetweenTurns;

		// Token: 0x02000F15 RID: 3861
		public enum CardGameState
		{
			// Token: 0x04004D1B RID: 19739
			NotPlaying,
			// Token: 0x04004D1C RID: 19740
			InGameBetweenRounds,
			// Token: 0x04004D1D RID: 19741
			InGameRound,
			// Token: 0x04004D1E RID: 19742
			InGameRoundEnding
		}

		// Token: 0x02000F16 RID: 3862
		public enum Playability
		{
			// Token: 0x04004D20 RID: 19744
			OK,
			// Token: 0x04004D21 RID: 19745
			NoPlayer,
			// Token: 0x04004D22 RID: 19746
			NotEnoughBuyIn,
			// Token: 0x04004D23 RID: 19747
			TooMuchBuyIn,
			// Token: 0x04004D24 RID: 19748
			RanOutOfScrap,
			// Token: 0x04004D25 RID: 19749
			Idle
		}
	}
}
