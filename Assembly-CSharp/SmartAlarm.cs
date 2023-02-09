using System;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C9 RID: 201
public class SmartAlarm : AppIOEntity, ISubscribable
{
	// Token: 0x060011AF RID: 4527 RVA: 0x0008EFFC File Offset: 0x0008D1FC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SmartAlarm.OnRpcMessage", 0))
		{
			if (rpc == 3292290572U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetNotificationTextImpl ");
				}
				using (TimeWarning.New("SetNotificationTextImpl", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3292290572U, "SetNotificationTextImpl", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3292290572U, "SetNotificationTextImpl", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage notificationTextImpl = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetNotificationTextImpl(notificationTextImpl);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetNotificationTextImpl");
					}
				}
				return true;
			}
			if (rpc == 4207149767U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartSetupNotification ");
				}
				using (TimeWarning.New("StartSetupNotification", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4207149767U, "StartSetupNotification", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4207149767U, "StartSetupNotification", this, player, 3f))
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
							this.StartSetupNotification(rpc2);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in StartSetupNotification");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000175 RID: 373
	// (get) Token: 0x060011B0 RID: 4528 RVA: 0x0004AF67 File Offset: 0x00049167
	public override AppEntityType Type
	{
		get
		{
			return AppEntityType.Alarm;
		}
	}

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x060011B1 RID: 4529 RVA: 0x0008F334 File Offset: 0x0008D534
	// (set) Token: 0x060011B2 RID: 4530 RVA: 0x0008F33C File Offset: 0x0008D53C
	public override bool Value { get; set; }

	// Token: 0x060011B3 RID: 4531 RVA: 0x0008F345 File Offset: 0x0008D545
	public bool AddSubscription(ulong steamId)
	{
		return this._subscriptions.AddSubscription(steamId);
	}

	// Token: 0x060011B4 RID: 4532 RVA: 0x0008F353 File Offset: 0x0008D553
	public bool RemoveSubscription(ulong steamId)
	{
		return this._subscriptions.RemoveSubscription(steamId);
	}

	// Token: 0x060011B5 RID: 4533 RVA: 0x0008F361 File Offset: 0x0008D561
	public bool HasSubscription(ulong steamId)
	{
		return this._subscriptions.HasSubscription(steamId);
	}

	// Token: 0x060011B6 RID: 4534 RVA: 0x0008F36F File Offset: 0x0008D56F
	public override void InitShared()
	{
		base.InitShared();
		this._notificationTitle = global::SmartAlarm.DefaultNotificationTitle.translated;
		this._notificationBody = global::SmartAlarm.DefaultNotificationBody.translated;
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x0008F398 File Offset: 0x0008D598
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		this.Value = (inputAmount > 0);
		if (this.Value == base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, this.Value, false, true);
		base.BroadcastValueChange();
		float num = Mathf.Max(App.alarmcooldown, 15f);
		if (this.Value && UnityEngine.Time.realtimeSinceStartup - this._lastSentTime >= num)
		{
			BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
			if (buildingPrivilege != null)
			{
				this._subscriptions.IntersectWith(buildingPrivilege.authorizedPlayers);
			}
			this._subscriptions.SendNotification(NotificationChannel.SmartAlarm, this._notificationTitle, this._notificationBody, "alarm");
			this._lastSentTime = UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x0008F448 File Offset: 0x0008D648
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.smartAlarm = Facepunch.Pool.Get<ProtoBuf.SmartAlarm>();
			info.msg.smartAlarm.notificationTitle = this._notificationTitle;
			info.msg.smartAlarm.notificationBody = this._notificationBody;
			info.msg.smartAlarm.subscriptions = this._subscriptions.ToList();
		}
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x0008F4BC File Offset: 0x0008D6BC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.smartAlarm != null)
		{
			this._notificationTitle = info.msg.smartAlarm.notificationTitle;
			this._notificationBody = info.msg.smartAlarm.notificationBody;
			this._subscriptions.LoadFrom(info.msg.smartAlarm.subscriptions);
		}
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x0008F52C File Offset: 0x0008D72C
	protected override void OnPairedWithPlayer(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (!this.AddSubscription(player.userID))
		{
			player.ClientRPCPlayer<int>(null, player, "HandleCompanionPairingResult", 7);
		}
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0008F554 File Offset: 0x0008D754
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void StartSetupNotification(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		if (buildingPrivilege != null && !buildingPrivilege.CanAdministrate(rpc.player))
		{
			return;
		}
		base.ClientRPCPlayer<string, string>(null, rpc.player, "SetupNotification", this._notificationTitle, this._notificationBody);
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x0008F5AC File Offset: 0x0008D7AC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void SetNotificationTextImpl(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		if (buildingPrivilege != null && !buildingPrivilege.CanAdministrate(rpc.player))
		{
			return;
		}
		string text = rpc.read.String(128);
		string text2 = rpc.read.String(512);
		if (!string.IsNullOrWhiteSpace(text))
		{
			this._notificationTitle = text;
		}
		if (!string.IsNullOrWhiteSpace(text2))
		{
			this._notificationBody = text2;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, true, false, true);
	}

	// Token: 0x04000B0E RID: 2830
	public const global::BaseEntity.Flags Flag_HasCustomMessage = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000B0F RID: 2831
	public static readonly Translate.Phrase DefaultNotificationTitle = new Translate.Phrase("app.alarm.title", "Alarm");

	// Token: 0x04000B10 RID: 2832
	public static readonly Translate.Phrase DefaultNotificationBody = new Translate.Phrase("app.alarm.body", "Your base is under attack!");

	// Token: 0x04000B11 RID: 2833
	[Header("Smart Alarm")]
	public GameObjectRef SetupNotificationDialog;

	// Token: 0x04000B12 RID: 2834
	public Animator Animator;

	// Token: 0x04000B14 RID: 2836
	private readonly NotificationList _subscriptions = new NotificationList();

	// Token: 0x04000B15 RID: 2837
	private string _notificationTitle = "";

	// Token: 0x04000B16 RID: 2838
	private string _notificationBody = "";

	// Token: 0x04000B17 RID: 2839
	private float _lastSentTime;
}
