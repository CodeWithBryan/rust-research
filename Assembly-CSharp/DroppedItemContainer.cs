using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000067 RID: 103
public class DroppedItemContainer : BaseCombatEntity, LootPanel.IHasLootPanel, IContainerSounds
{
	// Token: 0x06000A37 RID: 2615 RVA: 0x0005D0C4 File Offset: 0x0005B2C4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DroppedItemContainer.OnRpcMessage", 0))
		{
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
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
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenLoot(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06000A38 RID: 2616 RVA: 0x0005D22C File Offset: 0x0005B42C
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.playerName;
		}
	}

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000A39 RID: 2617 RVA: 0x0005D239 File Offset: 0x0005B439
	// (set) Token: 0x06000A3A RID: 2618 RVA: 0x0005D24C File Offset: 0x0005B44C
	public string playerName
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

	// Token: 0x06000A3B RID: 2619 RVA: 0x0005D255 File Offset: 0x0005B455
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		return (!baseEntity.InSafeZone() || baseEntity.userID == this.playerSteamID) && (!this.onlyOwnerLoot || baseEntity.userID == this.playerSteamID) && base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x0005D28E File Offset: 0x0005B48E
	public override void ServerInit()
	{
		this.ResetRemovalTime();
		base.ServerInit();
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0005D29C File Offset: 0x0005B49C
	public void RemoveMe()
	{
		if (base.IsOpen())
		{
			this.ResetRemovalTime();
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x0005D2B4 File Offset: 0x0005B4B4
	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning.New("ResetRemovalTime", 0))
		{
			base.Invoke(new Action(this.RemoveMe), dur);
		}
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x0005D2FC File Offset: 0x0005B4FC
	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.CalculateRemovalTime());
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0005D30C File Offset: 0x0005B50C
	public float CalculateRemovalTime()
	{
		if (!this.ItemBasedDespawn)
		{
			return ConVar.Server.itemdespawn * 16f * ConVar.Server.itemdespawn_container_scale;
		}
		float num = ConVar.Server.itemdespawn_quick;
		if (this.inventory != null)
		{
			foreach (global::Item item in this.inventory.itemList)
			{
				num = Mathf.Max(num, item.GetDespawnDuration());
			}
		}
		return num;
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0005D394 File Offset: 0x0005B594
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.inventory != null)
		{
			this.inventory.Kill();
			this.inventory = null;
		}
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x0005D3B8 File Offset: 0x0005B5B8
	public void TakeFrom(params global::ItemContainer[] source)
	{
		Assert.IsTrue(this.inventory == null, "Initializing Twice");
		using (TimeWarning.New("DroppedItemContainer.TakeFrom", 0))
		{
			int num = 0;
			foreach (global::ItemContainer itemContainer in source)
			{
				num += itemContainer.itemList.Count;
			}
			this.inventory = new global::ItemContainer();
			this.inventory.ServerInitialize(null, Mathf.Min(num, this.maxItemCount));
			this.inventory.GiveUID();
			this.inventory.entityOwner = this;
			this.inventory.SetFlag(global::ItemContainer.Flag.NoItemInput, true);
			for (int i = 0; i < source.Length; i++)
			{
				foreach (global::Item item in source[i].itemList.ToArray())
				{
					if (!item.MoveToContainer(this.inventory, -1, true, false, null, true))
					{
						item.DropAndTossUpwards(base.transform.position, 2f);
					}
				}
			}
			this.ResetRemovalTime();
		}
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x0005D4DC File Offset: 0x0005B6DC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage rpc)
	{
		if (this.inventory == null)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(this, true))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			player.inventory.loot.AddContainer(this.inventory);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", this.lootPanelName);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x0005D568 File Offset: 0x0005B768
	public void PlayerStoppedLooting(global::BasePlayer player)
	{
		if (this.inventory == null || this.inventory.itemList == null || this.inventory.itemList.Count == 0)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			return;
		}
		this.ResetRemovalTime();
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x0005D5BB File Offset: 0x0005B7BB
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.ServerInitialize(null, 0);
		this.inventory.SetFlag(global::ItemContainer.Flag.NoItemInput, true);
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x0005D5F8 File Offset: 0x0005B7F8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Facepunch.Pool.Get<ProtoBuf.LootableCorpse>();
		info.msg.lootableCorpse.playerName = this.playerName;
		info.msg.lootableCorpse.playerID = this.playerSteamID;
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning("Dropped item container without inventory: " + this.ToString());
		}
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0005D69C File Offset: 0x0005B89C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerName = info.msg.lootableCorpse.playerName;
			this.playerSteamID = info.msg.lootableCorpse.playerID;
		}
		if (info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				return;
			}
			Debug.LogWarning("Dropped item container without inventory: " + this.ToString());
		}
	}

	// Token: 0x0400069C RID: 1692
	public string lootPanelName = "generic";

	// Token: 0x0400069D RID: 1693
	public int maxItemCount = 36;

	// Token: 0x0400069E RID: 1694
	[NonSerialized]
	public ulong playerSteamID;

	// Token: 0x0400069F RID: 1695
	[NonSerialized]
	public string _playerName;

	// Token: 0x040006A0 RID: 1696
	public bool ItemBasedDespawn;

	// Token: 0x040006A1 RID: 1697
	public bool onlyOwnerLoot;

	// Token: 0x040006A2 RID: 1698
	public SoundDefinition openSound;

	// Token: 0x040006A3 RID: 1699
	public SoundDefinition closeSound;

	// Token: 0x040006A4 RID: 1700
	public global::ItemContainer inventory;
}
