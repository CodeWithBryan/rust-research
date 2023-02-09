using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008F RID: 143
public class LootableCorpse : BaseCorpse, LootPanel.IHasLootPanel
{
	// Token: 0x06000D26 RID: 3366 RVA: 0x0006EF60 File Offset: 0x0006D160
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LootableCorpse.OnRpcMessage", 0))
		{
			if (rpc == 2278459738U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_LootCorpse ");
				}
				using (TimeWarning.New("RPC_LootCorpse", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2278459738U, "RPC_LootCorpse", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_LootCorpse(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_LootCorpse");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000D27 RID: 3367 RVA: 0x0006F0C8 File Offset: 0x0006D2C8
	// (set) Token: 0x06000D28 RID: 3368 RVA: 0x0006F0DB File Offset: 0x0006D2DB
	public virtual string playerName
	{
		get
		{
			return NameHelper.Get(this.playerSteamID, this._playerName);
		}
		set
		{
			this._playerName = value;
		}
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000D29 RID: 3369 RVA: 0x0006F0E4 File Offset: 0x0006D2E4
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.playerName;
		}
	}

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000D2A RID: 3370 RVA: 0x0006F0F1 File Offset: 0x0006D2F1
	public Translate.Phrase LootPanelName
	{
		get
		{
			return "N/A";
		}
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x0006F0FD File Offset: 0x0006D2FD
	public override void ResetState()
	{
		this.firstLooted = false;
		base.ResetState();
	}

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000D2C RID: 3372 RVA: 0x0006F10C File Offset: 0x0006D30C
	// (set) Token: 0x06000D2D RID: 3373 RVA: 0x0006F114 File Offset: 0x0006D314
	public bool blockBagDrop { get; set; }

	// Token: 0x06000D2E RID: 3374 RVA: 0x0006F11D File Offset: 0x0006D31D
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x0006F128 File Offset: 0x0006D328
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (!this.blockBagDrop)
		{
			this.PreDropItems();
			this.DropItems();
		}
		this.blockBagDrop = false;
		if (this.containers != null)
		{
			global::ItemContainer[] array = this.containers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill();
			}
		}
		this.containers = null;
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x0006F184 File Offset: 0x0006D384
	public void TakeFrom(params global::ItemContainer[] source)
	{
		Assert.IsTrue(this.containers == null, "Initializing Twice");
		using (TimeWarning.New("Corpse.TakeFrom", 0))
		{
			this.containers = new global::ItemContainer[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				this.containers[i] = new global::ItemContainer();
				this.containers[i].ServerInitialize(null, source[i].capacity);
				this.containers[i].GiveUID();
				this.containers[i].entityOwner = this;
				foreach (global::Item item in source[i].itemList.ToArray())
				{
					if (!item.MoveToContainer(this.containers[i], -1, true, false, null, true))
					{
						item.DropAndTossUpwards(base.transform.position, 2f);
					}
				}
			}
			base.ResetRemovalTime();
		}
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x0006F284 File Offset: 0x0006D484
	public override bool CanRemove()
	{
		return !base.IsOpen();
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool CanLoot()
	{
		return true;
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x0006F28F File Offset: 0x0006D48F
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		if (!this.firstLooted)
		{
			ulong num = this.playerSteamID;
			this.firstLooted = true;
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x00003A54 File Offset: 0x00001C54
	protected virtual bool CanLootContainer(global::ItemContainer c, int index)
	{
		return true;
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x0006F2B8 File Offset: 0x0006D4B8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_LootCorpse(global::BaseEntity.RPCMessage rpc)
	{
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		if (!this.CanLoot())
		{
			return;
		}
		if (this.containers == null)
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(this, true))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			for (int i = 0; i < this.containers.Length; i++)
			{
				global::ItemContainer itemContainer = this.containers[i];
				if (this.CanLootContainer(itemContainer, i))
				{
					player.inventory.loot.AddContainer(itemContainer);
				}
			}
			player.inventory.loot.SendImmediate();
			base.ClientRPCPlayer(null, player, "RPC_ClientLootCorpse");
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x0006F368 File Offset: 0x0006D568
	public void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.ResetRemovalTime();
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void PreDropItems()
	{
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x0006F384 File Offset: 0x0006D584
	public void DropItems()
	{
		if (Global.disableBagDropping)
		{
			return;
		}
		if (this.containers != null)
		{
			DroppedItemContainer droppedItemContainer = global::ItemContainer.Drop("assets/prefabs/misc/item drop/item_drop_backpack.prefab", base.transform.position, Quaternion.identity, this.containers);
			if (droppedItemContainer != null)
			{
				droppedItemContainer.playerName = this.playerName;
				droppedItemContainer.playerSteamID = this.playerSteamID;
			}
		}
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x0006F3E4 File Offset: 0x0006D5E4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Facepunch.Pool.Get<ProtoBuf.LootableCorpse>();
		info.msg.lootableCorpse.playerName = this.playerName;
		info.msg.lootableCorpse.playerID = this.playerSteamID;
		if (info.forDisk && this.containers != null)
		{
			info.msg.lootableCorpse.privateData = Facepunch.Pool.Get<ProtoBuf.LootableCorpse.Private>();
			info.msg.lootableCorpse.privateData.container = Facepunch.Pool.GetList<ProtoBuf.ItemContainer>();
			foreach (global::ItemContainer itemContainer in this.containers)
			{
				if (itemContainer != null)
				{
					ProtoBuf.ItemContainer itemContainer2 = itemContainer.Save();
					if (itemContainer2 != null)
					{
						info.msg.lootableCorpse.privateData.container.Add(itemContainer2);
					}
				}
			}
		}
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x0006F4B4 File Offset: 0x0006D6B4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerName = info.msg.lootableCorpse.playerName;
			this.playerSteamID = info.msg.lootableCorpse.playerID;
			if (info.fromDisk && info.msg.lootableCorpse.privateData != null && info.msg.lootableCorpse.privateData.container != null)
			{
				int count = info.msg.lootableCorpse.privateData.container.Count;
				this.containers = new global::ItemContainer[count];
				for (int i = 0; i < count; i++)
				{
					this.containers[i] = new global::ItemContainer();
					this.containers[i].ServerInitialize(null, info.msg.lootableCorpse.privateData.container[i].slots);
					this.containers[i].GiveUID();
					this.containers[i].entityOwner = this;
					this.containers[i].Load(info.msg.lootableCorpse.privateData.container[i]);
				}
			}
		}
	}

	// Token: 0x0400085D RID: 2141
	public string lootPanelName = "generic";

	// Token: 0x0400085E RID: 2142
	[NonSerialized]
	public ulong playerSteamID;

	// Token: 0x0400085F RID: 2143
	[NonSerialized]
	public string _playerName;

	// Token: 0x04000861 RID: 2145
	[NonSerialized]
	public global::ItemContainer[] containers;

	// Token: 0x04000862 RID: 2146
	[NonSerialized]
	private bool firstLooted;
}
