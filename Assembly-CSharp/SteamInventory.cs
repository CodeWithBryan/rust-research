using System;
using System.Linq;
using System.Threading.Tasks;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D4 RID: 212
public class SteamInventory : EntityComponent<BasePlayer>
{
	// Token: 0x06001263 RID: 4707 RVA: 0x00093914 File Offset: 0x00091B14
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SteamInventory.OnRpcMessage", 0))
		{
			if (rpc == 643458331U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UpdateSteamInventory ");
				}
				using (TimeWarning.New("UpdateSteamInventory", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(643458331U, "UpdateSteamInventory", this.GetBaseEntity(), player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UpdateSteamInventory(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in UpdateSteamInventory");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x00093A80 File Offset: 0x00091C80
	public bool HasItem(int itemid)
	{
		if (this.Items == null)
		{
			return false;
		}
		IPlayerItem[] items = this.Items;
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].DefinitionId == itemid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001265 RID: 4709 RVA: 0x00093ABC File Offset: 0x00091CBC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private async Task UpdateSteamInventory(BaseEntity.RPCMessage msg)
	{
		byte[] array = msg.read.BytesWithSize(10485760U);
		if (array == null)
		{
			Debug.LogWarning("UpdateSteamInventory: Data is null");
		}
		else
		{
			IPlayerInventory playerInventory = await PlatformService.Instance.DeserializeInventory(array);
			if (playerInventory == null)
			{
				Debug.LogWarning("UpdateSteamInventory: result is null");
			}
			else if (base.baseEntity == null)
			{
				Debug.LogWarning("UpdateSteamInventory: player is null");
			}
			else if (!playerInventory.BelongsTo(base.baseEntity.userID))
			{
				Debug.LogWarning(string.Format("UpdateSteamPlayer: inventory belongs to someone else (userID={0})", base.baseEntity.userID));
			}
			else if (base.gameObject)
			{
				this.Items = playerInventory.Items.ToArray<IPlayerItem>();
				playerInventory.Dispose();
			}
		}
	}

	// Token: 0x04000B86 RID: 2950
	private IPlayerItem[] Items;
}
