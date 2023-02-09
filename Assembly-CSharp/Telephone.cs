using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D7 RID: 215
public class Telephone : ContainerIOEntity, ICassettePlayer
{
	// Token: 0x0600129D RID: 4765 RVA: 0x0009461C File Offset: 0x0009281C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Telephone.OnRpcMessage", 0))
		{
			if (rpc == 1529322558U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AnswerPhone ");
				}
				using (TimeWarning.New("AnswerPhone", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1529322558U, "AnswerPhone", this, player, 3f))
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
							this.AnswerPhone(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AnswerPhone");
					}
				}
				return true;
			}
			if (rpc == 2754362156U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ClearCurrentUser ");
				}
				using (TimeWarning.New("ClearCurrentUser", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2754362156U, "ClearCurrentUser", this, player, 9f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ClearCurrentUser(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ClearCurrentUser");
					}
				}
				return true;
			}
			if (rpc == 1095090232U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - InitiateCall ");
				}
				using (TimeWarning.New("InitiateCall", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1095090232U, "InitiateCall", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.InitiateCall(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in InitiateCall");
					}
				}
				return true;
			}
			if (rpc == 2606442785U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_AddSavedNumber ");
				}
				using (TimeWarning.New("Server_AddSavedNumber", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2606442785U, "Server_AddSavedNumber", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2606442785U, "Server_AddSavedNumber", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_AddSavedNumber(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in Server_AddSavedNumber");
					}
				}
				return true;
			}
			if (rpc == 1402406333U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RemoveSavedNumber ");
				}
				using (TimeWarning.New("Server_RemoveSavedNumber", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1402406333U, "Server_RemoveSavedNumber", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1402406333U, "Server_RemoveSavedNumber", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RemoveSavedNumber(msg6);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in Server_RemoveSavedNumber");
					}
				}
				return true;
			}
			if (rpc == 942544266U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestPhoneDirectory ");
				}
				using (TimeWarning.New("Server_RequestPhoneDirectory", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(942544266U, "Server_RequestPhoneDirectory", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(942544266U, "Server_RequestPhoneDirectory", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RequestPhoneDirectory(msg7);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in Server_RequestPhoneDirectory");
					}
				}
				return true;
			}
			if (rpc == 1240133378U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerDeleteVoicemail ");
				}
				using (TimeWarning.New("ServerDeleteVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1240133378U, "ServerDeleteVoicemail", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1240133378U, "ServerDeleteVoicemail", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg8 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerDeleteVoicemail(msg8);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogException(exception7);
						player.Kick("RPC Error in ServerDeleteVoicemail");
					}
				}
				return true;
			}
			if (rpc == 1221129498U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerHangUp ");
				}
				using (TimeWarning.New("ServerHangUp", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg9 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerHangUp(msg9);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogException(exception8);
						player.Kick("RPC Error in ServerHangUp");
					}
				}
				return true;
			}
			if (rpc == 239260010U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerPlayVoicemail ");
				}
				using (TimeWarning.New("ServerPlayVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(239260010U, "ServerPlayVoicemail", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(239260010U, "ServerPlayVoicemail", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg10 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerPlayVoicemail(msg10);
						}
					}
					catch (Exception exception9)
					{
						Debug.LogException(exception9);
						player.Kick("RPC Error in ServerPlayVoicemail");
					}
				}
				return true;
			}
			if (rpc == 189198880U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerSendVoicemail ");
				}
				using (TimeWarning.New("ServerSendVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(189198880U, "ServerSendVoicemail", this, player, 5UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg11 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerSendVoicemail(msg11);
						}
					}
					catch (Exception exception10)
					{
						Debug.LogException(exception10);
						player.Kick("RPC Error in ServerSendVoicemail");
					}
				}
				return true;
			}
			if (rpc == 2760189029U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerStopVoicemail ");
				}
				using (TimeWarning.New("ServerStopVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2760189029U, "ServerStopVoicemail", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2760189029U, "ServerStopVoicemail", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg12 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerStopVoicemail(msg12);
						}
					}
					catch (Exception exception11)
					{
						Debug.LogException(exception11);
						player.Kick("RPC Error in ServerStopVoicemail");
					}
				}
				return true;
			}
			if (rpc == 3900772076U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetCurrentUser ");
				}
				using (TimeWarning.New("SetCurrentUser", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3900772076U, "SetCurrentUser", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage currentUser = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetCurrentUser(currentUser);
						}
					}
					catch (Exception exception12)
					{
						Debug.LogException(exception12);
						player.Kick("RPC Error in SetCurrentUser");
					}
				}
				return true;
			}
			if (rpc == 2760249627U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UpdatePhoneName ");
				}
				using (TimeWarning.New("UpdatePhoneName", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2760249627U, "UpdatePhoneName", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2760249627U, "UpdatePhoneName", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg13 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UpdatePhoneName(msg13);
						}
					}
					catch (Exception exception13)
					{
						Debug.LogException(exception13);
						player.Kick("RPC Error in UpdatePhoneName");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x0600129E RID: 4766 RVA: 0x00095878 File Offset: 0x00093A78
	public uint AnsweringMessageId
	{
		get
		{
			if (!(this.cachedCassette != null))
			{
				return 0U;
			}
			return this.cachedCassette.AudioId;
		}
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x00095898 File Offset: 0x00093A98
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.telephone == null)
		{
			info.msg.telephone = Facepunch.Pool.Get<ProtoBuf.Telephone>();
		}
		info.msg.telephone.phoneNumber = this.Controller.PhoneNumber;
		info.msg.telephone.phoneName = this.Controller.PhoneName;
		info.msg.telephone.lastNumber = this.Controller.lastDialedNumber;
		info.msg.telephone.savedNumbers = this.Controller.savedNumbers;
		if (this.Controller.savedVoicemail != null)
		{
			info.msg.telephone.voicemail = Facepunch.Pool.GetList<ProtoBuf.VoicemailEntry>();
			foreach (ProtoBuf.VoicemailEntry item in this.Controller.savedVoicemail)
			{
				info.msg.telephone.voicemail.Add(item);
			}
		}
		if (!info.forDisk)
		{
			info.msg.telephone.usingPlayer = this.Controller.currentPlayerRef.uid;
		}
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x000959DC File Offset: 0x00093BDC
	public override void ServerInit()
	{
		base.ServerInit();
		this.Controller.ServerInit();
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x00095A16 File Offset: 0x00093C16
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Controller.PostServerLoad();
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x00095A29 File Offset: 0x00093C29
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.Controller.DoServerDestroy();
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x00095A3C File Offset: 0x00093C3C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(9f)]
	public void ClearCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ClearCurrentUser(msg);
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x00095A4A File Offset: 0x00093C4A
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void SetCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.SetCurrentUser(msg);
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x00095A58 File Offset: 0x00093C58
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void InitiateCall(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.InitiateCall(msg);
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x00095A66 File Offset: 0x00093C66
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void AnswerPhone(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.AnswerPhone(msg);
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x00095A74 File Offset: 0x00093C74
	[global::BaseEntity.RPC_Server]
	private void ServerHangUp(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerHangUp(msg);
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x00095A82 File Offset: 0x00093C82
	public void OnCassetteInserted(global::Cassette c)
	{
		this.cachedCassette = c;
		base.ClientRPC<uint>(null, "ClientOnCassetteChanged", c.net.ID);
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x00095AA2 File Offset: 0x00093CA2
	public void OnCassetteRemoved(global::Cassette c)
	{
		this.cachedCassette = null;
		this.Controller.DeleteAllVoicemail();
		base.ClientRPC<int>(null, "ClientOnCassetteChanged", 0);
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x00095AC4 File Offset: 0x00093CC4
	private bool CanAcceptItem(global::Item item, int targetSlot)
	{
		ItemDefinition[] validCassettes = this.ValidCassettes;
		for (int i = 0; i < validCassettes.Length; i++)
		{
			if (validCassettes[i] == item.info)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x00095AF9 File Offset: 0x00093CF9
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.Controller.DestroyShared();
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x00095B0C File Offset: 0x00093D0C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void UpdatePhoneName(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.UpdatePhoneName(msg);
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x00095B1A File Offset: 0x00093D1A
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_RequestPhoneDirectory(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_RequestPhoneDirectory(msg);
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x00095B28 File Offset: 0x00093D28
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_AddSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_AddSavedNumber(msg);
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x00095B36 File Offset: 0x00093D36
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_RemoveSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_RemoveSavedNumber(msg);
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x00095B44 File Offset: 0x00093D44
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ServerSendVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerSendVoicemail(msg);
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x00095B52 File Offset: 0x00093D52
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ServerPlayVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerPlayVoicemail(msg);
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x00095B60 File Offset: 0x00093D60
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ServerStopVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerStopVoicemail(msg);
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x00095B6E File Offset: 0x00093D6E
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ServerDeleteVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerDeleteVoicemail(msg);
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x00095B7C File Offset: 0x00093D7C
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.Controller.serverState == global::Telephone.CallState.Ringing || this.Controller.serverState == global::Telephone.CallState.InProcess)
		{
			return base.GetPassthroughAmount(outputSlot);
		}
		return 0;
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x060012B5 RID: 4789 RVA: 0x00095BA3 File Offset: 0x00093DA3
	// (set) Token: 0x060012B6 RID: 4790 RVA: 0x00095BAB File Offset: 0x00093DAB
	public global::Cassette cachedCassette { get; private set; }

	// Token: 0x060012B7 RID: 4791 RVA: 0x00095BB4 File Offset: 0x00093DB4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ProtoBuf.Entity msg = info.msg;
		if (((msg != null) ? msg.telephone : null) != null)
		{
			this.Controller.PhoneNumber = info.msg.telephone.phoneNumber;
			this.Controller.PhoneName = info.msg.telephone.phoneName;
			this.Controller.lastDialedNumber = info.msg.telephone.lastNumber;
			this.Controller.savedVoicemail = Facepunch.Pool.GetList<ProtoBuf.VoicemailEntry>();
			foreach (ProtoBuf.VoicemailEntry voicemailEntry in info.msg.telephone.voicemail)
			{
				this.Controller.savedVoicemail.Add(voicemailEntry);
				voicemailEntry.ShouldPool = false;
			}
			if (!info.fromDisk)
			{
				this.Controller.currentPlayerRef.uid = info.msg.telephone.usingPlayer;
			}
			PhoneDirectory savedNumbers = this.Controller.savedNumbers;
			if (savedNumbers != null)
			{
				savedNumbers.ResetToPool();
			}
			this.Controller.savedNumbers = info.msg.telephone.savedNumbers;
			if (this.Controller.savedNumbers != null)
			{
				this.Controller.savedNumbers.ShouldPool = false;
			}
			if (info.fromDisk)
			{
				base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			}
		}
	}

	// Token: 0x060012B8 RID: 4792 RVA: 0x00095D30 File Offset: 0x00093F30
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && this.Controller.currentPlayer == null;
	}

	// Token: 0x1700018D RID: 397
	// (get) Token: 0x060012B9 RID: 4793 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x00095D50 File Offset: 0x00093F50
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			if (this.Controller.RequirePower && next.HasFlag(global::BaseEntity.Flags.Busy) && !next.HasFlag(global::BaseEntity.Flags.Reserved8))
			{
				this.Controller.ServerHangUp();
			}
			if (old.HasFlag(global::BaseEntity.Flags.Busy) != next.HasFlag(global::BaseEntity.Flags.Busy))
			{
				if (next.HasFlag(global::BaseEntity.Flags.Busy))
				{
					if (!base.IsInvoking(new Action(this.Controller.WatchForDisconnects)))
					{
						base.InvokeRepeating(new Action(this.Controller.WatchForDisconnects), 0f, 0.1f);
					}
				}
				else
				{
					base.CancelInvoke(new Action(this.Controller.WatchForDisconnects));
				}
			}
		}
		this.Controller.OnFlagsChanged(old, next);
	}

	// Token: 0x04000B9F RID: 2975
	public const int MaxPhoneNameLength = 20;

	// Token: 0x04000BA0 RID: 2976
	public const int MaxSavedNumbers = 10;

	// Token: 0x04000BA1 RID: 2977
	public Transform PhoneHotspot;

	// Token: 0x04000BA2 RID: 2978
	public Transform AnsweringMachineHotspot;

	// Token: 0x04000BA3 RID: 2979
	public Transform[] HandsetRoots;

	// Token: 0x04000BA4 RID: 2980
	public ItemDefinition[] ValidCassettes;

	// Token: 0x04000BA5 RID: 2981
	public Transform ParentedHandsetTransform;

	// Token: 0x04000BA6 RID: 2982
	public LineRenderer CableLineRenderer;

	// Token: 0x04000BA7 RID: 2983
	public Transform CableStartPoint;

	// Token: 0x04000BA8 RID: 2984
	public Transform CableEndPoint;

	// Token: 0x04000BA9 RID: 2985
	public float LineDroopAmount = 0.25f;

	// Token: 0x04000BAB RID: 2987
	public PhoneController Controller;

	// Token: 0x02000BBE RID: 3006
	public enum CallState
	{
		// Token: 0x04003F5F RID: 16223
		Idle,
		// Token: 0x04003F60 RID: 16224
		Dialing,
		// Token: 0x04003F61 RID: 16225
		Ringing,
		// Token: 0x04003F62 RID: 16226
		InProcess
	}

	// Token: 0x02000BBF RID: 3007
	public enum DialFailReason
	{
		// Token: 0x04003F64 RID: 16228
		TimedOut,
		// Token: 0x04003F65 RID: 16229
		Engaged,
		// Token: 0x04003F66 RID: 16230
		WrongNumber,
		// Token: 0x04003F67 RID: 16231
		CallSelf,
		// Token: 0x04003F68 RID: 16232
		RemoteHangUp,
		// Token: 0x04003F69 RID: 16233
		NetworkBusy,
		// Token: 0x04003F6A RID: 16234
		TimeOutDuringCall,
		// Token: 0x04003F6B RID: 16235
		SelfHangUp
	}
}
