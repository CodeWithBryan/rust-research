using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000088 RID: 136
public class KeyLock : BaseLock
{
	// Token: 0x06000CB9 RID: 3257 RVA: 0x0006C384 File Offset: 0x0006A584
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("KeyLock.OnRpcMessage", 0))
		{
			if (rpc == 4135414453U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_CreateKey ");
				}
				using (TimeWarning.New("RPC_CreateKey", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4135414453U, "RPC_CreateKey", this, player, 3f))
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
							this.RPC_CreateKey(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_CreateKey");
					}
				}
				return true;
			}
			if (rpc == 954115386U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Lock ");
				}
				using (TimeWarning.New("RPC_Lock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(954115386U, "RPC_Lock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Lock(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Lock");
					}
				}
				return true;
			}
			if (rpc == 1663222372U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Unlock ");
				}
				using (TimeWarning.New("RPC_Unlock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1663222372U, "RPC_Unlock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Unlock(rpc4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_Unlock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x0006C7E0 File Offset: 0x0006A9E0
	public override bool HasLockPermission(global::BasePlayer player)
	{
		if (player.IsDead())
		{
			return false;
		}
		if (player.userID == base.OwnerID)
		{
			return true;
		}
		foreach (global::Item key in player.inventory.FindItemIDs(this.keyItemType.itemid))
		{
			if (this.CanKeyUnlockUs(key))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x0006C868 File Offset: 0x0006AA68
	private bool CanKeyUnlockUs(global::Item key)
	{
		return key.instanceData != null && key.instanceData.dataInt == this.keyCode;
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x0006C88A File Offset: 0x0006AA8A
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.keyLock != null)
		{
			this.keyCode = info.msg.keyLock.code;
		}
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x0006C8B6 File Offset: 0x0006AAB6
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.OwnerID == 0UL && base.GetParentEntity())
		{
			base.OwnerID = base.GetParentEntity().OwnerID;
		}
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x0006C8E4 File Offset: 0x0006AAE4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.keyLock = Facepunch.Pool.Get<ProtoBuf.KeyLock>();
			info.msg.keyLock.code = this.keyCode;
		}
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x0006C91B File Offset: 0x0006AB1B
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		this.keyCode = UnityEngine.Random.Range(1, 100000);
		this.Lock(deployedBy);
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x0006C93E File Offset: 0x0006AB3E
	public override bool OnTryToOpen(global::BasePlayer player)
	{
		return this.HasLockPermission(player) || !base.IsLocked();
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x0006C93E File Offset: 0x0006AB3E
	public override bool OnTryToClose(global::BasePlayer player)
	{
		return this.HasLockPermission(player) || !base.IsLocked();
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x0006C954 File Offset: 0x0006AB54
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Unlock(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!base.IsLocked())
		{
			return;
		}
		if (!this.HasLockPermission(rpc.player))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x0006C98E File Offset: 0x0006AB8E
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Lock(global::BaseEntity.RPCMessage rpc)
	{
		this.Lock(rpc.player);
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x0006C99C File Offset: 0x0006AB9C
	private void Lock(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (!player.CanInteract())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (!this.HasLockPermission(player))
		{
			return;
		}
		this.LockLock(player);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x0006C9D4 File Offset: 0x0006ABD4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_CreateKey(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked() && !this.HasLockPermission(rpc.player))
		{
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(this.keyItemType.itemid);
		if (itemDefinition == null)
		{
			Debug.LogWarning("RPC_CreateKey: Itemdef is missing! " + this.keyItemType);
			return;
		}
		ItemBlueprint bp = ItemManager.FindBlueprint(itemDefinition);
		if (rpc.player.inventory.crafting.CanCraft(bp, 1, false))
		{
			ProtoBuf.Item.InstanceData instanceData = Facepunch.Pool.Get<ProtoBuf.Item.InstanceData>();
			instanceData.dataInt = this.keyCode;
			rpc.player.inventory.crafting.CraftItem(bp, rpc.player, instanceData, 1, 0, null, false);
			if (!this.firstKeyCreated)
			{
				this.LockLock(rpc.player);
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				this.firstKeyCreated = true;
			}
			return;
		}
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x0006CAAE File Offset: 0x0006ACAE
	public void LockLock(global::BasePlayer player)
	{
		base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
		if (player.IsValid())
		{
			player.GiveAchievement("LOCK_LOCK");
		}
	}

	// Token: 0x0400081C RID: 2076
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition keyItemType;

	// Token: 0x0400081D RID: 2077
	private int keyCode;

	// Token: 0x0400081E RID: 2078
	private bool firstKeyCreated;
}
