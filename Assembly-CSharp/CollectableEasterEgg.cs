using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000056 RID: 86
public class CollectableEasterEgg : BaseEntity
{
	// Token: 0x0600095A RID: 2394 RVA: 0x000576C8 File Offset: 0x000558C8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CollectableEasterEgg.OnRpcMessage", 0))
		{
			if (rpc == 2436818324U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PickUp ");
				}
				using (TimeWarning.New("RPC_PickUp", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2436818324U, "RPC_PickUp", this, player, 3f))
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
							this.RPC_PickUp(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_PickUp");
					}
				}
				return true;
			}
			if (rpc == 2243088389U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StartPickUp ");
				}
				using (TimeWarning.New("RPC_StartPickUp", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2243088389U, "RPC_StartPickUp", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StartPickUp(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_StartPickUp");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x000579C8 File Offset: 0x00055BC8
	public override void ServerInit()
	{
		int num = UnityEngine.Random.Range(0, 3);
		base.SetFlag(BaseEntity.Flags.Reserved1, num == 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, num == 1, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, num == 2, false, false);
		base.ServerInit();
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x00057A16 File Offset: 0x00055C16
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_StartPickUp(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		this.lastPickupStartTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x00057A34 File Offset: 0x00055C34
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_PickUp(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastPickupStartTime;
		if (!(msg.player.GetHeldEntity() as EasterBasket))
		{
			if (num > 2f)
			{
				return;
			}
			if (num < 0.8f)
			{
				return;
			}
		}
		if (EggHuntEvent.serverEvent)
		{
			if (!EggHuntEvent.serverEvent.IsEventActive())
			{
				return;
			}
			EggHuntEvent.serverEvent.EggCollected(msg.player);
			int iAmount = 1;
			msg.player.GiveItem(ItemManager.Create(this.itemToGive, iAmount, 0UL), BaseEntity.GiveItemReason.Generic);
		}
		Effect.server.Run(this.pickupEffect.resourcePath, base.transform.position + Vector3.up * 0.3f, Vector3.up, null, false);
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0400062E RID: 1582
	public Transform artwork;

	// Token: 0x0400062F RID: 1583
	public float bounceRange = 0.2f;

	// Token: 0x04000630 RID: 1584
	public float bounceSpeed = 1f;

	// Token: 0x04000631 RID: 1585
	public GameObjectRef pickupEffect;

	// Token: 0x04000632 RID: 1586
	public ItemDefinition itemToGive;

	// Token: 0x04000633 RID: 1587
	private float lastPickupStartTime;
}
