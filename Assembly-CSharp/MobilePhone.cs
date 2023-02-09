using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009A RID: 154
public class MobilePhone : global::HeldEntity
{
	// Token: 0x06000DF0 RID: 3568 RVA: 0x00074BC4 File Offset: 0x00072DC4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MobilePhone.OnRpcMessage", 0))
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
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1529322558U, "AnswerPhone", this, player))
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
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2754362156U, "ClearCurrentUser", this, player))
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
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1095090232U, "InitiateCall", this, player))
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
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2606442785U, "Server_AddSavedNumber", this, player))
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
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1402406333U, "Server_RemoveSavedNumber", this, player))
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
			if (rpc == 2704491961U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestCurrentState ");
				}
				using (TimeWarning.New("Server_RequestCurrentState", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2704491961U, "Server_RequestCurrentState", this, player))
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
							this.Server_RequestCurrentState(msg7);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in Server_RequestCurrentState");
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
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(942544266U, "Server_RequestPhoneDirectory", this, player))
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
							this.Server_RequestPhoneDirectory(msg8);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogException(exception7);
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
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1240133378U, "ServerDeleteVoicemail", this, player))
						{
							return true;
						}
					}
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
							this.ServerDeleteVoicemail(msg9);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogException(exception8);
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
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1221129498U, "ServerHangUp", this, player))
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
							this.ServerHangUp(msg10);
						}
					}
					catch (Exception exception9)
					{
						Debug.LogException(exception9);
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
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(239260010U, "ServerPlayVoicemail", this, player))
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
							this.ServerPlayVoicemail(msg11);
						}
					}
					catch (Exception exception10)
					{
						Debug.LogException(exception10);
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
							global::BaseEntity.RPCMessage msg12 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerSendVoicemail(msg12);
						}
					}
					catch (Exception exception11)
					{
						Debug.LogException(exception11);
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
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2760189029U, "ServerStopVoicemail", this, player))
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
							this.ServerStopVoicemail(msg13);
						}
					}
					catch (Exception exception12)
					{
						Debug.LogException(exception12);
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
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3900772076U, "SetCurrentUser", this, player))
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
					catch (Exception exception13)
					{
						Debug.LogException(exception13);
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
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2760249627U, "UpdatePhoneName", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg14 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UpdatePhoneName(msg14);
						}
					}
					catch (Exception exception14)
					{
						Debug.LogException(exception14);
						player.Kick("RPC Error in UpdatePhoneName");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x00075F88 File Offset: 0x00074188
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
		if (!info.forDisk)
		{
			info.msg.telephone.usingPlayer = this.Controller.currentPlayerRef.uid;
		}
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x0007604D File Offset: 0x0007424D
	public override void ServerInit()
	{
		base.ServerInit();
		this.Controller.ServerInit();
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x00076060 File Offset: 0x00074260
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Controller.PostServerLoad();
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x00076073 File Offset: 0x00074273
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.Controller.DoServerDestroy();
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x00076086 File Offset: 0x00074286
	public override void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		this.Controller.OnParentChanged(newParent);
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x0007609C File Offset: 0x0007429C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ClearCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ClearCurrentUser(msg);
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x000760AA File Offset: 0x000742AA
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void SetCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.SetCurrentUser(msg);
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x000760B8 File Offset: 0x000742B8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void InitiateCall(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.InitiateCall(msg);
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x000760C6 File Offset: 0x000742C6
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void AnswerPhone(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.AnswerPhone(msg);
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x000760D4 File Offset: 0x000742D4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	private void ServerHangUp(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerHangUp(msg);
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x000760E2 File Offset: 0x000742E2
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.Controller.DestroyShared();
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x000760F5 File Offset: 0x000742F5
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void UpdatePhoneName(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.UpdatePhoneName(msg);
	}

	// Token: 0x06000DFD RID: 3581 RVA: 0x00076103 File Offset: 0x00074303
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_RequestPhoneDirectory(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_RequestPhoneDirectory(msg);
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x00076111 File Offset: 0x00074311
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_AddSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_AddSavedNumber(msg);
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x0007611F File Offset: 0x0007431F
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_RemoveSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_RemoveSavedNumber(msg);
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x0007612D File Offset: 0x0007432D
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void Server_RequestCurrentState(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.SetPhoneStateWithPlayer(this.Controller.serverState);
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x00076145 File Offset: 0x00074345
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ServerSendVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerSendVoicemail(msg);
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x00076153 File Offset: 0x00074353
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ServerPlayVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerPlayVoicemail(msg);
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x00076161 File Offset: 0x00074361
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ServerStopVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerStopVoicemail(msg);
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x0007616F File Offset: 0x0007436F
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ServerDeleteVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerDeleteVoicemail(msg);
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x00076180 File Offset: 0x00074380
	public void ToggleRinging(bool state)
	{
		MobileInventoryEntity associatedEntity = ItemModAssociatedEntity<MobileInventoryEntity>.GetAssociatedEntity(this.GetItem(), true);
		if (associatedEntity != null)
		{
			associatedEntity.ToggleRinging(state);
		}
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x000761AC File Offset: 0x000743AC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ProtoBuf.Entity msg = info.msg;
		if (((msg != null) ? msg.telephone : null) != null)
		{
			this.Controller.PhoneNumber = info.msg.telephone.phoneNumber;
			this.Controller.PhoneName = info.msg.telephone.phoneName;
			this.Controller.lastDialedNumber = info.msg.telephone.lastNumber;
			this.Controller.currentPlayerRef.uid = info.msg.telephone.usingPlayer;
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
		}
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x00076298 File Offset: 0x00074498
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(global::BaseEntity.Flags.Busy) != next.HasFlag(global::BaseEntity.Flags.Busy))
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
		this.Controller.OnFlagsChanged(old, next);
	}

	// Token: 0x0400091A RID: 2330
	public PhoneController Controller;
}
