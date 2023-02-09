using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200003B RID: 59
public class BaseLock : BaseEntity
{
	// Token: 0x060003D3 RID: 979 RVA: 0x0002FF08 File Offset: 0x0002E108
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseLock.OnRpcMessage", 0))
		{
			if (rpc == 3572556655U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TakeLock ");
				}
				using (TimeWarning.New("RPC_TakeLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(3572556655U, "RPC_TakeLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_TakeLock(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_TakeLock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x00030070 File Offset: 0x0002E270
	public virtual bool GetPlayerLockPermission(BasePlayer player)
	{
		return this.OnTryToOpen(player);
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x00030079 File Offset: 0x0002E279
	public virtual bool OnTryToOpen(BasePlayer player)
	{
		return !base.IsLocked();
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool OnTryToClose(BasePlayer player)
	{
		return true;
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool HasLockPermission(BasePlayer player)
	{
		return true;
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x00030084 File Offset: 0x0002E284
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_TakeLock(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		Item item = ItemManager.Create(this.itemType, 1, this.skinID);
		if (item != null)
		{
			rpc.player.GiveItem(item, BaseEntity.GiveItemReason.Generic);
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x000300D2 File Offset: 0x0002E2D2
	public override float BoundsPadding()
	{
		return 2f;
	}

	// Token: 0x04000303 RID: 771
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemType;
}
