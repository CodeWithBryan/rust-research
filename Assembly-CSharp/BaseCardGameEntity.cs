using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.CardGames;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000033 RID: 51
public abstract class BaseCardGameEntity : global::BaseVehicle
{
	// Token: 0x06000209 RID: 521 RVA: 0x00024E94 File Offset: 0x00023094
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseCardGameEntity.OnRpcMessage", 0))
		{
			if (rpc == 2395020190U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Editor_MakeRandomMove ");
				}
				using (TimeWarning.New("RPC_Editor_MakeRandomMove", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2395020190U, "RPC_Editor_MakeRandomMove", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Editor_MakeRandomMove(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Editor_MakeRandomMove");
					}
				}
				return true;
			}
			if (rpc == 1608700874U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Editor_SpawnTestPlayer ");
				}
				using (TimeWarning.New("RPC_Editor_SpawnTestPlayer", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1608700874U, "RPC_Editor_SpawnTestPlayer", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Editor_SpawnTestPlayer(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Editor_SpawnTestPlayer");
					}
				}
				return true;
			}
			if (rpc == 1499640189U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_LeaveTable ");
				}
				using (TimeWarning.New("RPC_LeaveTable", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1499640189U, "RPC_LeaveTable", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_LeaveTable(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_LeaveTable");
					}
				}
				return true;
			}
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLoot ");
				}
				using (TimeWarning.New("RPC_OpenLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(331989034U, "RPC_OpenLoot", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenLoot(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
			if (rpc == 2847205856U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Play ");
				}
				using (TimeWarning.New("RPC_Play", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2847205856U, "RPC_Play", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Play(msg6);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in RPC_Play");
					}
				}
				return true;
			}
			if (rpc == 2495306863U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PlayerInput ");
				}
				using (TimeWarning.New("RPC_PlayerInput", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2495306863U, "RPC_PlayerInput", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_PlayerInput(msg7);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in RPC_PlayerInput");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x0600020A RID: 522 RVA: 0x00025700 File Offset: 0x00023900
	public int ScrapItemID
	{
		get
		{
			return this.scrapItemDef.itemid;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x0600020B RID: 523 RVA: 0x0002570D File Offset: 0x0002390D
	public CardGameController GameController
	{
		get
		{
			if (this._gameCont == null)
			{
				this._gameCont = this.GetGameController();
			}
			return this._gameCont;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x0600020C RID: 524
	protected abstract float MaxStorageInteractionDist { get; }

	// Token: 0x0600020D RID: 525 RVA: 0x00025729 File Offset: 0x00023929
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer)
		{
			this.PotInstance.uid = info.msg.cardGame.potRef;
		}
	}

	// Token: 0x0600020E RID: 526 RVA: 0x00025758 File Offset: 0x00023958
	private CardGameController GetGameController()
	{
		BaseCardGameEntity.CardGameOption cardGameOption = this.gameOption;
		if (cardGameOption == BaseCardGameEntity.CardGameOption.TexasHoldEm)
		{
			return new TexasHoldEmController(this);
		}
		if (cardGameOption != BaseCardGameEntity.CardGameOption.Blackjack)
		{
			return new TexasHoldEmController(this);
		}
		return new BlackjackController(this);
	}

	// Token: 0x0600020F RID: 527 RVA: 0x00025789 File Offset: 0x00023989
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.GameController.Dispose();
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x06000210 RID: 528 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool CanSwapSeats
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0002579C File Offset: 0x0002399C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.cardGame = Facepunch.Pool.Get<CardGame>();
		info.msg.cardGame.potRef = this.PotInstance.uid;
		if (!info.forDisk && this.storageLinked)
		{
			this.GameController.Save(info.msg.cardGame);
		}
	}

	// Token: 0x06000212 RID: 530 RVA: 0x00025804 File Offset: 0x00023A04
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		int num = 0;
		int num2 = 0;
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardGamePlayerStorage cardGamePlayerStorage;
				if ((cardGamePlayerStorage = (enumerator.Current as CardGamePlayerStorage)) != null)
				{
					this.playerStoragePoints[num].storageInstance.Set(cardGamePlayerStorage);
					if (!cardGamePlayerStorage.inventory.IsEmpty())
					{
						num2++;
					}
					num++;
				}
			}
		}
		this.storageLinked = true;
		bool flag = true;
		StorageContainer pot = this.GetPot();
		if (pot == null)
		{
			flag = false;
		}
		else
		{
			int num3 = (num2 > 0) ? num2 : this.playerStoragePoints.Length;
			int iAmount = Mathf.CeilToInt((float)(pot.inventory.GetAmount(this.ScrapItemID, true) / num3));
			BaseCardGameEntity.PlayerStorageInfo[] array = this.playerStoragePoints;
			for (int i = 0; i < array.Length; i++)
			{
				CardGamePlayerStorage cardGamePlayerStorage2 = array[i].storageInstance.Get(base.isServer) as CardGamePlayerStorage;
				if (cardGamePlayerStorage2.IsValid() && (!cardGamePlayerStorage2.inventory.IsEmpty() || num2 == 0))
				{
					List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
					if (pot.inventory.Take(list, this.ScrapItemID, iAmount) > 0)
					{
						foreach (global::Item item in list)
						{
							if (!item.MoveToContainer(cardGamePlayerStorage2.inventory, -1, true, true, null, true))
							{
								item.Remove(0f);
							}
						}
					}
					Facepunch.Pool.FreeList<global::Item>(ref list);
				}
			}
		}
		if (flag)
		{
			BaseCardGameEntity.PlayerStorageInfo[] array = this.playerStoragePoints;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].storageInstance.IsValid(base.isServer))
				{
					flag = false;
					break;
				}
			}
		}
		if (!flag)
		{
			Debug.LogWarning(base.GetType().Name + ": Card game storage didn't load in. Destroying the card game (and parent entity if there is one).");
			global::BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity != null)
			{
				parentEntity.Invoke(new Action(parentEntity.KillMessage), 0f);
				return;
			}
			base.Invoke(new Action(base.KillMessage), 0f);
		}
	}

	// Token: 0x06000213 RID: 531 RVA: 0x00025A50 File Offset: 0x00023C50
	internal override void DoServerDestroy()
	{
		CardGameController gameController = this.GameController;
		if (gameController != null)
		{
			gameController.OnTableDestroyed();
		}
		BaseCardGameEntity.PlayerStorageInfo[] array = this.playerStoragePoints;
		for (int i = 0; i < array.Length; i++)
		{
			CardGamePlayerStorage storage = array[i].GetStorage();
			if (storage != null)
			{
				storage.DropItems(null);
			}
		}
		StorageContainer pot = this.GetPot();
		if (pot != null)
		{
			pot.DropItems(null);
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000214 RID: 532 RVA: 0x00025ABC File Offset: 0x00023CBC
	public override void PrePlayerDismount(global::BasePlayer player, BaseMountable seat)
	{
		base.PrePlayerDismount(player, seat);
		if (!Rust.Application.isLoadingSave)
		{
			CardGamePlayerStorage playerStorage = this.GetPlayerStorage(player.userID);
			if (playerStorage != null)
			{
				global::Item slot = playerStorage.inventory.GetSlot(0);
				if (slot != null)
				{
					slot.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true);
				}
			}
		}
	}

	// Token: 0x06000215 RID: 533 RVA: 0x00025B15 File Offset: 0x00023D15
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		this.GameController.LeaveTable(player.userID);
	}

	// Token: 0x06000216 RID: 534 RVA: 0x00025B30 File Offset: 0x00023D30
	public StorageContainer GetPot()
	{
		global::BaseEntity baseEntity = this.PotInstance.Get(true);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x06000217 RID: 535 RVA: 0x00025B64 File Offset: 0x00023D64
	public global::BasePlayer IDToPlayer(ulong id)
	{
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() != null && mountPointInfo.mountable.GetMounted().userID == id)
			{
				return mountPointInfo.mountable.GetMounted();
			}
		}
		return null;
	}

	// Token: 0x06000218 RID: 536 RVA: 0x00025BF8 File Offset: 0x00023DF8
	public virtual void PlayerStorageChanged()
	{
		this.GameController.PlayerStorageChanged();
	}

	// Token: 0x06000219 RID: 537 RVA: 0x00025C05 File Offset: 0x00023E05
	public CardGamePlayerStorage GetPlayerStorage(int storageIndex)
	{
		return this.playerStoragePoints[storageIndex].GetStorage();
	}

	// Token: 0x0600021A RID: 538 RVA: 0x00025C14 File Offset: 0x00023E14
	public CardGamePlayerStorage GetPlayerStorage(ulong playerID)
	{
		int mountPointIndex = this.GetMountPointIndex(playerID);
		if (mountPointIndex < 0)
		{
			return null;
		}
		return this.playerStoragePoints[mountPointIndex].GetStorage();
	}

	// Token: 0x0600021B RID: 539 RVA: 0x00025C3C File Offset: 0x00023E3C
	public int GetMountPointIndex(ulong playerID)
	{
		int num = -1;
		for (int i = 0; i < this.mountPoints.Count; i++)
		{
			BaseMountable mountable = this.mountPoints[i].mountable;
			if (mountable != null)
			{
				global::BasePlayer mounted = mountable.GetMounted();
				if (mounted != null && mounted.userID == playerID)
				{
					num = i;
				}
			}
		}
		if (num < 0)
		{
			Debug.LogError(base.GetType().Name + ": Couldn't find mount point for this player.");
		}
		return num;
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00025CB8 File Offset: 0x00023EB8
	public override void SpawnSubEntities()
	{
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.potPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		StorageContainer storageContainer = baseEntity as StorageContainer;
		if (storageContainer != null)
		{
			storageContainer.SetParent(this, false, false);
			storageContainer.Spawn();
			this.PotInstance.Set(baseEntity);
		}
		else
		{
			Debug.LogError(base.GetType().Name + ": Spawned prefab is not a StorageContainer as expected.");
		}
		foreach (BaseCardGameEntity.PlayerStorageInfo playerStorageInfo in this.playerStoragePoints)
		{
			baseEntity = GameManager.server.CreateEntity(this.playerStoragePrefab.resourcePath, playerStorageInfo.storagePos.localPosition, playerStorageInfo.storagePos.localRotation, true);
			CardGamePlayerStorage cardGamePlayerStorage = baseEntity as CardGamePlayerStorage;
			if (cardGamePlayerStorage != null)
			{
				cardGamePlayerStorage.SetCardTable(this);
				cardGamePlayerStorage.SetParent(this, false, false);
				cardGamePlayerStorage.Spawn();
				playerStorageInfo.storageInstance.Set(baseEntity);
				this.storageLinked = true;
			}
			else
			{
				Debug.LogError(base.GetType().Name + ": Spawned prefab is not a CardTablePlayerStorage as expected.");
			}
		}
		base.SpawnSubEntities();
	}

	// Token: 0x0600021D RID: 541 RVA: 0x00025DDD File Offset: 0x00023FDD
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_PlayerInput(global::BaseEntity.RPCMessage msg)
	{
		this.GameController.ReceivedInputFromPlayer(msg.player, msg.read.Int32(), true, msg.read.Int32());
	}

	// Token: 0x0600021E RID: 542 RVA: 0x00025E07 File Offset: 0x00024007
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_LeaveTable(global::BaseEntity.RPCMessage msg)
	{
		this.GameController.LeaveTable(msg.player.userID);
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00025E20 File Offset: 0x00024020
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player != null && this.PlayerIsMounted(player))
		{
			this.GetPlayerStorage(player.userID).PlayerOpenLoot(player, "", true);
		}
	}

	// Token: 0x06000220 RID: 544 RVA: 0x00025E60 File Offset: 0x00024060
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Editor_SpawnTestPlayer(global::BaseEntity.RPCMessage msg)
	{
		if (!UnityEngine.Application.isEditor)
		{
			return;
		}
		int num = this.GameController.MaxPlayersAtTable();
		if (this.GameController.NumPlayersAllowedToPlay(null) >= num || base.NumMounted() >= num)
		{
			return;
		}
		Debug.Log("Adding test NPC for card game");
		global::BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		global::BasePlayer basePlayer = (global::BasePlayer)baseEntity;
		this.AttemptMount(basePlayer, false);
		this.GameController.JoinTable(basePlayer);
		CardPlayerData cardPlayerData;
		if (this.GameController.TryGetCardPlayerData(basePlayer, out cardPlayerData))
		{
			int scrapAmount = cardPlayerData.GetScrapAmount();
			if (scrapAmount < 400)
			{
				StorageContainer storage = cardPlayerData.GetStorage();
				if (storage != null)
				{
					storage.inventory.AddItem(this.scrapItemDef, 400 - scrapAmount, 0UL, global::ItemContainer.LimitStack.Existing);
					return;
				}
				Debug.LogError("Couldn't get storage for NPC.");
				return;
			}
		}
		else
		{
			Debug.Log("Couldn't find player data for NPC. No scrap given.");
		}
	}

	// Token: 0x06000221 RID: 545 RVA: 0x00025F47 File Offset: 0x00024147
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Editor_MakeRandomMove(global::BaseEntity.RPCMessage msg)
	{
		if (!UnityEngine.Application.isEditor)
		{
			return;
		}
		this.GameController.EditorMakeRandomMove();
	}

	// Token: 0x06000222 RID: 546 RVA: 0x00025F5C File Offset: 0x0002415C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_Play(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player != null && this.PlayerIsMounted(player))
		{
			this.GameController.JoinTable(player);
		}
	}

	// Token: 0x0400020F RID: 527
	[Header("Card Game")]
	[SerializeField]
	private GameObjectRef uiPrefab;

	// Token: 0x04000210 RID: 528
	public ItemDefinition scrapItemDef;

	// Token: 0x04000211 RID: 529
	[SerializeField]
	private GameObjectRef potPrefab;

	// Token: 0x04000212 RID: 530
	public BaseCardGameEntity.PlayerStorageInfo[] playerStoragePoints;

	// Token: 0x04000213 RID: 531
	[SerializeField]
	private GameObjectRef playerStoragePrefab;

	// Token: 0x04000214 RID: 532
	private CardGameController _gameCont;

	// Token: 0x04000215 RID: 533
	public BaseCardGameEntity.CardGameOption gameOption;

	// Token: 0x04000216 RID: 534
	public EntityRef PotInstance;

	// Token: 0x04000217 RID: 535
	private bool storageLinked;

	// Token: 0x02000B29 RID: 2857
	[Serializable]
	public class PlayerStorageInfo
	{
		// Token: 0x06004A36 RID: 18998 RVA: 0x0018F844 File Offset: 0x0018DA44
		public CardGamePlayerStorage GetStorage()
		{
			global::BaseEntity baseEntity = this.storageInstance.Get(true);
			if (baseEntity != null && baseEntity.IsValid())
			{
				return baseEntity as CardGamePlayerStorage;
			}
			return null;
		}

		// Token: 0x04003CB5 RID: 15541
		public Transform storagePos;

		// Token: 0x04003CB6 RID: 15542
		public EntityRef storageInstance;
	}

	// Token: 0x02000B2A RID: 2858
	public enum CardGameOption
	{
		// Token: 0x04003CB8 RID: 15544
		TexasHoldEm,
		// Token: 0x04003CB9 RID: 15545
		Blackjack
	}
}
