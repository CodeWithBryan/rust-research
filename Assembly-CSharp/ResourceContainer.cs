using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BA RID: 186
public class ResourceContainer : EntityComponent<BaseEntity>
{
	// Token: 0x060010A6 RID: 4262 RVA: 0x00088478 File Offset: 0x00086678
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ResourceContainer.OnRpcMessage", 0))
		{
			if (rpc == 548378753U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartLootingContainer ");
				}
				using (TimeWarning.New("StartLootingContainer", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(548378753U, "StartLootingContainer", this.GetBaseEntity(), player, 3f))
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
							this.StartLootingContainer(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in StartLootingContainer");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x060010A7 RID: 4263 RVA: 0x000885E8 File Offset: 0x000867E8
	public int accessedSecondsAgo
	{
		get
		{
			return (int)(UnityEngine.Time.realtimeSinceStartup - this.lastAccessTime);
		}
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x000885F8 File Offset: 0x000867F8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	private void StartLootingContainer(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		if (!this.lootable)
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(base.baseEntity, true))
		{
			this.lastAccessTime = UnityEngine.Time.realtimeSinceStartup;
			player.inventory.loot.AddContainer(this.container);
		}
	}

	// Token: 0x04000A64 RID: 2660
	public bool lootable = true;

	// Token: 0x04000A65 RID: 2661
	[NonSerialized]
	public ItemContainer container;

	// Token: 0x04000A66 RID: 2662
	[NonSerialized]
	public float lastAccessTime;
}
