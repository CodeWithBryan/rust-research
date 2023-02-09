using System;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AB1 RID: 2737
	public class CardPlayerData : IDisposable
	{
		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x06004209 RID: 16905 RVA: 0x001828C5 File Offset: 0x00180AC5
		// (set) Token: 0x0600420A RID: 16906 RVA: 0x001828CD File Offset: 0x00180ACD
		public ulong UserID { get; private set; }

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x0600420B RID: 16907 RVA: 0x001828D6 File Offset: 0x00180AD6
		// (set) Token: 0x0600420C RID: 16908 RVA: 0x001828DE File Offset: 0x00180ADE
		public CardPlayerData.CardPlayerState State { get; private set; }

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x0600420D RID: 16909 RVA: 0x001828E7 File Offset: 0x00180AE7
		public bool HasUser
		{
			get
			{
				return this.State >= CardPlayerData.CardPlayerState.WantsToPlay;
			}
		}

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x0600420E RID: 16910 RVA: 0x001828F5 File Offset: 0x00180AF5
		public bool HasUserInGame
		{
			get
			{
				return this.State >= CardPlayerData.CardPlayerState.InGame;
			}
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x0600420F RID: 16911 RVA: 0x00182903 File Offset: 0x00180B03
		public bool HasUserInCurrentRound
		{
			get
			{
				return this.State == CardPlayerData.CardPlayerState.InCurrentRound;
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x06004210 RID: 16912 RVA: 0x0018290E File Offset: 0x00180B0E
		public bool HasAvailableInputs
		{
			get
			{
				return this.availableInputs > 0;
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x06004211 RID: 16913 RVA: 0x00182919 File Offset: 0x00180B19
		private bool IsClient
		{
			get
			{
				return !this.isServer;
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06004212 RID: 16914 RVA: 0x00182924 File Offset: 0x00180B24
		// (set) Token: 0x06004213 RID: 16915 RVA: 0x0018292C File Offset: 0x00180B2C
		public bool LeftRoundEarly { get; private set; }

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06004214 RID: 16916 RVA: 0x00182935 File Offset: 0x00180B35
		// (set) Token: 0x06004215 RID: 16917 RVA: 0x0018293D File Offset: 0x00180B3D
		public bool SendCardDetails { get; private set; }

		// Token: 0x06004216 RID: 16918 RVA: 0x00182946 File Offset: 0x00180B46
		public CardPlayerData(int mountIndex, bool isServer)
		{
			this.isServer = isServer;
			this.mountIndex = mountIndex;
			this.Cards = Pool.GetList<PlayingCard>();
		}

		// Token: 0x06004217 RID: 16919 RVA: 0x00182967 File Offset: 0x00180B67
		public CardPlayerData(int scrapItemID, Func<int, StorageContainer> getStorage, int mountIndex, bool isServer) : this(mountIndex, isServer)
		{
			this.scrapItemID = scrapItemID;
			this.getStorage = getStorage;
		}

		// Token: 0x06004218 RID: 16920 RVA: 0x00182980 File Offset: 0x00180B80
		public virtual void Dispose()
		{
			Pool.FreeList<PlayingCard>(ref this.Cards);
			if (this.isServer)
			{
				this.CancelTurnTimer();
			}
		}

		// Token: 0x06004219 RID: 16921 RVA: 0x0018299C File Offset: 0x00180B9C
		public int GetScrapAmount()
		{
			if (this.isServer)
			{
				StorageContainer storage = this.GetStorage();
				if (storage != null)
				{
					return storage.inventory.GetAmount(this.scrapItemID, true);
				}
				Debug.LogError(base.GetType().Name + ": Couldn't get player storage.");
			}
			return 0;
		}

		// Token: 0x0600421A RID: 16922 RVA: 0x001829EF File Offset: 0x00180BEF
		public virtual int GetTotalBetThisRound()
		{
			return this.betThisRound;
		}

		// Token: 0x0600421B RID: 16923 RVA: 0x001829F7 File Offset: 0x00180BF7
		public virtual List<PlayingCard> GetMainCards()
		{
			return this.Cards;
		}

		// Token: 0x0600421C RID: 16924 RVA: 0x0002A0CF File Offset: 0x000282CF
		public virtual List<PlayingCard> GetSecondaryCards()
		{
			return null;
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x0600421D RID: 16925 RVA: 0x001829FF File Offset: 0x00180BFF
		// (set) Token: 0x0600421E RID: 16926 RVA: 0x00182A07 File Offset: 0x00180C07
		public bool hasCompletedTurn { get; private set; }

		// Token: 0x0600421F RID: 16927 RVA: 0x00182A10 File Offset: 0x00180C10
		public void SetHasCompletedTurn(bool hasActed)
		{
			this.hasCompletedTurn = hasActed;
			if (!hasActed)
			{
				this.betThisTurn = 0;
			}
		}

		// Token: 0x06004220 RID: 16928 RVA: 0x00182A23 File Offset: 0x00180C23
		public bool HasBeenIdleFor(int seconds)
		{
			return this.HasUserInGame && Time.unscaledTime > this.lastActionTime + (float)seconds;
		}

		// Token: 0x06004221 RID: 16929 RVA: 0x00182A3F File Offset: 0x00180C3F
		public StorageContainer GetStorage()
		{
			return this.getStorage(this.mountIndex);
		}

		// Token: 0x06004222 RID: 16930 RVA: 0x00182A52 File Offset: 0x00180C52
		public void AddUser(ulong userID)
		{
			this.ClearAllData();
			this.UserID = userID;
			this.State = CardPlayerData.CardPlayerState.WantsToPlay;
			this.lastActionTime = Time.unscaledTime;
		}

		// Token: 0x06004223 RID: 16931 RVA: 0x00182A73 File Offset: 0x00180C73
		public void ClearAllData()
		{
			this.UserID = 0UL;
			this.availableInputs = 0;
			this.State = CardPlayerData.CardPlayerState.None;
			this.ClearPerRoundData();
		}

		// Token: 0x06004224 RID: 16932 RVA: 0x00182A91 File Offset: 0x00180C91
		public void JoinRound()
		{
			if (!this.HasUser)
			{
				return;
			}
			this.State = CardPlayerData.CardPlayerState.InCurrentRound;
			this.ClearPerRoundData();
		}

		// Token: 0x06004225 RID: 16933 RVA: 0x00182AA9 File Offset: 0x00180CA9
		protected virtual void ClearPerRoundData()
		{
			this.Cards.Clear();
			this.betThisRound = 0;
			this.betThisTurn = 0;
			this.finalScore = 0;
			this.LeftRoundEarly = false;
			this.hasCompletedTurn = false;
			this.SendCardDetails = false;
		}

		// Token: 0x06004226 RID: 16934 RVA: 0x00182AE0 File Offset: 0x00180CE0
		public virtual void LeaveCurrentRound(bool clearBets, bool leftRoundEarly)
		{
			if (!this.HasUserInCurrentRound)
			{
				return;
			}
			this.availableInputs = 0;
			this.finalScore = 0;
			this.hasCompletedTurn = false;
			if (clearBets)
			{
				this.betThisRound = 0;
				this.betThisTurn = 0;
			}
			this.State = CardPlayerData.CardPlayerState.InGame;
			this.LeftRoundEarly = leftRoundEarly;
			this.CancelTurnTimer();
		}

		// Token: 0x06004227 RID: 16935 RVA: 0x00182B30 File Offset: 0x00180D30
		public virtual void LeaveGame()
		{
			if (!this.HasUserInGame)
			{
				return;
			}
			this.Cards.Clear();
			this.availableInputs = 0;
			this.finalScore = 0;
			this.SendCardDetails = false;
			this.LeftRoundEarly = false;
			this.State = CardPlayerData.CardPlayerState.WantsToPlay;
		}

		// Token: 0x06004228 RID: 16936 RVA: 0x00182B69 File Offset: 0x00180D69
		public void EnableSendingCards()
		{
			this.SendCardDetails = true;
		}

		// Token: 0x06004229 RID: 16937 RVA: 0x00182B72 File Offset: 0x00180D72
		public string HandToString()
		{
			return CardPlayerData.HandToString(this.Cards);
		}

		// Token: 0x0600422A RID: 16938 RVA: 0x00182B80 File Offset: 0x00180D80
		public static string HandToString(List<PlayingCard> cards)
		{
			string text = string.Empty;
			foreach (PlayingCard playingCard in cards)
			{
				text = text + "23456789TJQKA"[(int)playingCard.Rank].ToString() + "♠♥♦♣"[(int)playingCard.Suit].ToString() + " ";
			}
			return text;
		}

		// Token: 0x0600422B RID: 16939 RVA: 0x00182C0C File Offset: 0x00180E0C
		public virtual void Save(CardGame syncData)
		{
			CardGame.CardPlayer cardPlayer = Pool.Get<CardGame.CardPlayer>();
			cardPlayer.userid = this.UserID;
			cardPlayer.cards = Pool.GetList<int>();
			foreach (PlayingCard playingCard in this.Cards)
			{
				cardPlayer.cards.Add(this.SendCardDetails ? playingCard.GetIndex() : -1);
			}
			cardPlayer.scrap = this.GetScrapAmount();
			cardPlayer.state = (int)this.State;
			cardPlayer.availableInputs = this.availableInputs;
			cardPlayer.betThisRound = this.betThisRound;
			cardPlayer.betThisTurn = this.betThisTurn;
			cardPlayer.leftRoundEarly = this.LeftRoundEarly;
			cardPlayer.sendCardDetails = this.SendCardDetails;
			syncData.players.Add(cardPlayer);
		}

		// Token: 0x0600422C RID: 16940 RVA: 0x00182CF4 File Offset: 0x00180EF4
		public void StartTurnTimer(Action<CardPlayerData> callback, float maxTurnTime)
		{
			this.turnTimerCallback = callback;
			SingletonComponent<InvokeHandler>.Instance.Invoke(new Action(this.TimeoutTurn), maxTurnTime);
		}

		// Token: 0x0600422D RID: 16941 RVA: 0x00182D14 File Offset: 0x00180F14
		public void CancelTurnTimer()
		{
			SingletonComponent<InvokeHandler>.Instance.CancelInvoke(new Action(this.TimeoutTurn));
		}

		// Token: 0x0600422E RID: 16942 RVA: 0x00182D2C File Offset: 0x00180F2C
		public void TimeoutTurn()
		{
			if (this.turnTimerCallback != null)
			{
				this.turnTimerCallback(this);
			}
		}

		// Token: 0x04003A82 RID: 14978
		public List<PlayingCard> Cards;

		// Token: 0x04003A84 RID: 14980
		public readonly int mountIndex;

		// Token: 0x04003A85 RID: 14981
		private readonly bool isServer;

		// Token: 0x04003A86 RID: 14982
		public int availableInputs;

		// Token: 0x04003A87 RID: 14983
		public int betThisRound;

		// Token: 0x04003A88 RID: 14984
		public int betThisTurn;

		// Token: 0x04003A8B RID: 14987
		public int finalScore;

		// Token: 0x04003A8D RID: 14989
		public float lastActionTime;

		// Token: 0x04003A8E RID: 14990
		public int remainingToPayOut;

		// Token: 0x04003A8F RID: 14991
		private Func<int, StorageContainer> getStorage;

		// Token: 0x04003A90 RID: 14992
		private readonly int scrapItemID;

		// Token: 0x04003A91 RID: 14993
		private Action<CardPlayerData> turnTimerCallback;

		// Token: 0x02000F1A RID: 3866
		public enum CardPlayerState
		{
			// Token: 0x04004D2F RID: 19759
			None,
			// Token: 0x04004D30 RID: 19760
			WantsToPlay,
			// Token: 0x04004D31 RID: 19761
			InGame,
			// Token: 0x04004D32 RID: 19762
			InCurrentRound
		}
	}
}
