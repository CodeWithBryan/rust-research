using System;
using System.Collections.Generic;
using System.Globalization;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200002E RID: 46
public abstract class AppIOEntity : global::IOEntity
{
	// Token: 0x0600012C RID: 300 RVA: 0x0001F880 File Offset: 0x0001DA80
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("AppIOEntity.OnRpcMessage", 0))
		{
			if (rpc == 3018927126U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - PairWithApp ");
				}
				using (TimeWarning.New("PairWithApp", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3018927126U, "PairWithApp", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3018927126U, "PairWithApp", this, player, 3f))
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
							this.PairWithApp(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in PairWithApp");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x0600012D RID: 301
	public abstract AppEntityType Type { get; }

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600012E RID: 302 RVA: 0x00007074 File Offset: 0x00005274
	// (set) Token: 0x0600012F RID: 303 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual bool Value
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	// Token: 0x06000130 RID: 304 RVA: 0x0001FA40 File Offset: 0x0001DC40
	protected void BroadcastValueChange()
	{
		if (!this.IsValid())
		{
			return;
		}
		EntityTarget target = this.GetTarget();
		AppBroadcast appBroadcast = Facepunch.Pool.Get<AppBroadcast>();
		appBroadcast.entityChanged = Facepunch.Pool.Get<AppEntityChanged>();
		appBroadcast.entityChanged.entityId = this.net.ID;
		appBroadcast.entityChanged.payload = Facepunch.Pool.Get<AppEntityPayload>();
		this.FillEntityPayload(appBroadcast.entityChanged.payload);
		CompanionServer.Server.Broadcast(target, appBroadcast);
	}

	// Token: 0x06000131 RID: 305 RVA: 0x0001FAAA File Offset: 0x0001DCAA
	internal virtual void FillEntityPayload(AppEntityPayload payload)
	{
		payload.value = this.Value;
	}

	// Token: 0x06000132 RID: 306 RVA: 0x0001FAB8 File Offset: 0x0001DCB8
	public override BuildingPrivlidge GetBuildingPrivilege()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this._cacheTime > 5f)
		{
			this._cache = base.GetBuildingPrivilege();
			this._cacheTime = UnityEngine.Time.realtimeSinceStartup;
		}
		return this._cache;
	}

	// Token: 0x06000133 RID: 307 RVA: 0x0001FAEA File Offset: 0x0001DCEA
	public EntityTarget GetTarget()
	{
		return new EntityTarget(this.net.ID);
	}

	// Token: 0x06000134 RID: 308 RVA: 0x0001FAFC File Offset: 0x0001DCFC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public async void PairWithApp(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		Dictionary<string, string> playerPairingData = CompanionServer.Util.GetPlayerPairingData(player);
		playerPairingData.Add("entityId", this.net.ID.ToString("G", CultureInfo.InvariantCulture));
		playerPairingData.Add("entityType", ((int)this.Type).ToString("G", CultureInfo.InvariantCulture));
		playerPairingData.Add("entityName", base.GetDisplayName());
		NotificationSendResult notificationSendResult = await CompanionServer.Util.SendPairNotification("entity", player, base.GetDisplayName(), "Tap to pair with this device.", playerPairingData);
		if (notificationSendResult == NotificationSendResult.Sent)
		{
			this.OnPairedWithPlayer(msg.player);
		}
		else
		{
			player.ClientRPCPlayer<int>(null, player, "HandleCompanionPairingResult", (int)notificationSendResult);
		}
	}

	// Token: 0x06000135 RID: 309 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnPairedWithPlayer(global::BasePlayer player)
	{
	}

	// Token: 0x0400018D RID: 397
	private float _cacheTime;

	// Token: 0x0400018E RID: 398
	private BuildingPrivlidge _cache;
}
