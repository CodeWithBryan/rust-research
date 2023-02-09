using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007B RID: 123
public class HeldBoomBox : HeldEntity, ICassettePlayer
{
	// Token: 0x06000B9F RID: 2975 RVA: 0x00065630 File Offset: 0x00063830
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HeldBoomBox.OnRpcMessage", 0))
		{
			if (rpc == 1918716764U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_UpdateRadioIP ");
				}
				using (TimeWarning.New("Server_UpdateRadioIP", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(1918716764U, "Server_UpdateRadioIP", this, player, 2UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(1918716764U, "Server_UpdateRadioIP", this, player))
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
							this.Server_UpdateRadioIP(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_UpdateRadioIP");
					}
				}
				return true;
			}
			if (rpc == 1785864031U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerTogglePlay ");
				}
				using (TimeWarning.New("ServerTogglePlay", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(1785864031U, "ServerTogglePlay", this, player))
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
							this.ServerTogglePlay(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ServerTogglePlay");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000BA0 RID: 2976 RVA: 0x00002E37 File Offset: 0x00001037
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x00065944 File Offset: 0x00063B44
	public override void ServerInit()
	{
		base.ServerInit();
		this.BoxController.HurtCallback = new Action<float>(this.HurtCallback);
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x00065963 File Offset: 0x00063B63
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	public void ServerTogglePlay(BaseEntity.RPCMessage msg)
	{
		this.BoxController.ServerTogglePlay(msg);
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x00065971 File Offset: 0x00063B71
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void Server_UpdateRadioIP(BaseEntity.RPCMessage msg)
	{
		this.BoxController.Server_UpdateRadioIP(msg);
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x0006597F File Offset: 0x00063B7F
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		this.BoxController.Save(info);
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x00065994 File Offset: 0x00063B94
	public void OnCassetteInserted(Cassette c)
	{
		this.BoxController.OnCassetteInserted(c);
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x000659A2 File Offset: 0x00063BA2
	public void OnCassetteRemoved(Cassette c)
	{
		this.BoxController.OnCassetteRemoved(c);
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x000659B0 File Offset: 0x00063BB0
	public bool ClearRadioByUserId(ulong id)
	{
		return this.BoxController.ClearRadioByUserId(id);
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x000659BE File Offset: 0x00063BBE
	public void HurtCallback(float amount)
	{
		if (base.GetOwnerPlayer() != null && base.GetOwnerPlayer().IsSleeping())
		{
			this.BoxController.ServerTogglePlay(false);
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		item.LoseCondition(amount);
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x000659F9 File Offset: 0x00063BF9
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			this.BoxController.ServerTogglePlay(false);
		}
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x00065A15 File Offset: 0x00063C15
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		this.BoxController.Load(info);
		base.Load(info);
	}

	// Token: 0x0400076F RID: 1903
	public BoomBox BoxController;

	// Token: 0x04000770 RID: 1904
	public SwapKeycard cassetteSwapper;
}
