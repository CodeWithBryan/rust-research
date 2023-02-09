using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000382 RID: 898
public class PhoneController : EntityComponent<global::BaseEntity>
{
	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06001F4F RID: 8015 RVA: 0x000CF639 File Offset: 0x000CD839
	// (set) Token: 0x06001F50 RID: 8016 RVA: 0x000CF641 File Offset: 0x000CD841
	public global::Telephone.CallState serverState { get; set; }

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x06001F51 RID: 8017 RVA: 0x000CF64C File Offset: 0x000CD84C
	public uint AnsweringMessageId
	{
		get
		{
			global::Telephone telephone;
			if ((telephone = (base.baseEntity as global::Telephone)) == null)
			{
				return 0U;
			}
			return telephone.AnsweringMessageId;
		}
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x000CF670 File Offset: 0x000CD870
	public void ServerInit()
	{
		if (this.PhoneNumber == 0 && !Rust.Application.isLoadingSave)
		{
			this.PhoneNumber = TelephoneManager.GetUnusedTelephoneNumber();
			if (this.AppendGridToName & !string.IsNullOrEmpty(this.PhoneName))
			{
				this.PhoneName = this.PhoneName + " " + PhoneController.PositionToGridCoord(base.transform.position);
			}
			TelephoneManager.RegisterTelephone(this, false);
		}
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x000CF6DB File Offset: 0x000CD8DB
	public void PostServerLoad()
	{
		this.currentPlayer = null;
		base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		TelephoneManager.RegisterTelephone(this, false);
	}

	// Token: 0x06001F54 RID: 8020 RVA: 0x000CF6FE File Offset: 0x000CD8FE
	public void DoServerDestroy()
	{
		TelephoneManager.DeregisterTelephone(this);
	}

	// Token: 0x06001F55 RID: 8021 RVA: 0x000CF706 File Offset: 0x000CD906
	public void ClearCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.ClearCurrentUser();
	}

	// Token: 0x06001F56 RID: 8022 RVA: 0x000CF70E File Offset: 0x000CD90E
	public void ClearCurrentUser()
	{
		if (this.currentPlayer != null)
		{
			this.currentPlayer.SetActiveTelephone(null);
			this.currentPlayer = null;
		}
		base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06001F57 RID: 8023 RVA: 0x000CF744 File Offset: 0x000CD944
	public void SetCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (this.currentPlayer == player)
		{
			return;
		}
		this.UpdateServerPlayer(player);
		if (this.serverState == global::Telephone.CallState.Dialing || this.serverState == global::Telephone.CallState.Ringing || this.serverState == global::Telephone.CallState.InProcess)
		{
			this.ServerHangUp(default(global::BaseEntity.RPCMessage));
		}
	}

	// Token: 0x06001F58 RID: 8024 RVA: 0x000CF798 File Offset: 0x000CD998
	private void UpdateServerPlayer(global::BasePlayer newPlayer)
	{
		if (this.currentPlayer == newPlayer)
		{
			return;
		}
		if (this.currentPlayer != null)
		{
			this.currentPlayer.SetActiveTelephone(null);
		}
		this.currentPlayer = newPlayer;
		base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, this.currentPlayer != null, false, true);
		if (this.currentPlayer != null)
		{
			this.currentPlayer.SetActiveTelephone(this);
		}
	}

	// Token: 0x06001F59 RID: 8025 RVA: 0x000CF810 File Offset: 0x000CDA10
	public void InitiateCall(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		int number = msg.read.Int32();
		this.CallPhone(number);
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x000CF844 File Offset: 0x000CDA44
	public void CallPhone(int number)
	{
		if (number == this.PhoneNumber)
		{
			this.OnDialFailed(global::Telephone.DialFailReason.CallSelf);
			return;
		}
		if (TelephoneManager.GetCurrentActiveCalls() + 1 > TelephoneManager.MaxConcurrentCalls)
		{
			this.OnDialFailed(global::Telephone.DialFailReason.NetworkBusy);
			return;
		}
		PhoneController telephone = TelephoneManager.GetTelephone(number);
		if (!(telephone != null))
		{
			this.OnDialFailed(global::Telephone.DialFailReason.WrongNumber);
			return;
		}
		if (telephone.serverState == global::Telephone.CallState.Idle && telephone.CanReceiveCall())
		{
			this.SetPhoneState(global::Telephone.CallState.Dialing);
			this.lastDialedNumber = number;
			this.activeCallTo = telephone;
			this.activeCallTo.ReceiveCallFrom(this);
			return;
		}
		this.OnDialFailed(global::Telephone.DialFailReason.Engaged);
		telephone.OnIncomingCallWhileBusy();
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x000CF8CF File Offset: 0x000CDACF
	private bool CanReceiveCall()
	{
		return (!this.RequirePower || this.IsPowered()) && (!this.RequireParent || base.baseEntity.HasParent());
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x000CF8FC File Offset: 0x000CDAFC
	public void AnswerPhone(global::BaseEntity.RPCMessage msg)
	{
		if (base.IsInvoking(new Action(this.TimeOutDialing)))
		{
			base.CancelInvoke(new Action(this.TimeOutDialing));
		}
		if (this.activeCallTo == null)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		this.UpdateServerPlayer(player);
		this.BeginCall();
		this.activeCallTo.BeginCall();
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x000CF95D File Offset: 0x000CDB5D
	public void ReceiveCallFrom(PhoneController t)
	{
		this.activeCallTo = t;
		this.SetPhoneState(global::Telephone.CallState.Ringing);
		base.Invoke(new Action(this.TimeOutDialing), this.CallWaitingTime);
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x000CF985 File Offset: 0x000CDB85
	private void TimeOutDialing()
	{
		if (this.activeCallTo != null)
		{
			this.activeCallTo.ServerPlayAnsweringMessage(this);
		}
		this.SetPhoneState(global::Telephone.CallState.Idle);
	}

	// Token: 0x06001F5F RID: 8031 RVA: 0x000CF9A8 File Offset: 0x000CDBA8
	public void OnDialFailed(global::Telephone.DialFailReason reason)
	{
		this.SetPhoneState(global::Telephone.CallState.Idle);
		base.baseEntity.ClientRPC<int>(null, "ClientOnDialFailed", (int)reason);
		this.activeCallTo = null;
		if (base.IsInvoking(new Action(this.TimeOutCall)))
		{
			base.CancelInvoke(new Action(this.TimeOutCall));
		}
		if (base.IsInvoking(new Action(this.TriggerTimeOut)))
		{
			base.CancelInvoke(new Action(this.TriggerTimeOut));
		}
		if (base.IsInvoking(new Action(this.TimeOutDialing)))
		{
			base.CancelInvoke(new Action(this.TimeOutDialing));
		}
	}

	// Token: 0x06001F60 RID: 8032 RVA: 0x000CFA48 File Offset: 0x000CDC48
	public void ServerPlayAnsweringMessage(PhoneController fromPhone)
	{
		uint num = 0U;
		uint num2 = 0U;
		uint arg = 0U;
		if (this.activeCallTo != null && this.activeCallTo.cachedCassette != null)
		{
			num = this.activeCallTo.cachedCassette.net.ID;
			num2 = this.activeCallTo.cachedCassette.AudioId;
			if (num2 == 0U)
			{
				arg = StringPool.Get(this.activeCallTo.cachedCassette.PreloadedAudio.name);
			}
		}
		if (num != 0U)
		{
			base.baseEntity.ClientRPC<uint, uint, uint, int, int>(null, "ClientPlayAnsweringMessage", num, num2, arg, fromPhone.HasVoicemailSlot() ? 1 : 0, this.activeCallTo.PhoneNumber);
			base.Invoke(new Action(this.TriggerTimeOut), this.activeCallTo.cachedCassette.MaxCassetteLength);
			return;
		}
		this.OnDialFailed(global::Telephone.DialFailReason.TimedOut);
	}

	// Token: 0x06001F61 RID: 8033 RVA: 0x000CFB19 File Offset: 0x000CDD19
	private void TriggerTimeOut()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.TimedOut);
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x000CFB24 File Offset: 0x000CDD24
	public void SetPhoneStateWithPlayer(global::Telephone.CallState state)
	{
		this.serverState = state;
		base.baseEntity.ClientRPC<int, int>(null, "SetClientState", (int)this.serverState, (this.activeCallTo != null) ? this.activeCallTo.PhoneNumber : 0);
		MobilePhone mobilePhone;
		if ((mobilePhone = (base.baseEntity as MobilePhone)) != null)
		{
			mobilePhone.ToggleRinging(state == global::Telephone.CallState.Ringing);
		}
	}

	// Token: 0x06001F63 RID: 8035 RVA: 0x000CFB84 File Offset: 0x000CDD84
	private void SetPhoneState(global::Telephone.CallState state)
	{
		if (state == global::Telephone.CallState.Idle && this.currentPlayer == null)
		{
			base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		}
		this.serverState = state;
		base.baseEntity.ClientRPC<int, int>(null, "SetClientState", (int)this.serverState, (this.activeCallTo != null) ? this.activeCallTo.PhoneNumber : 0);
		global::Telephone telephone;
		if ((telephone = (base.baseEntity as global::Telephone)) != null)
		{
			telephone.MarkDirtyForceUpdateOutputs();
		}
		MobilePhone mobilePhone;
		if ((mobilePhone = (base.baseEntity as MobilePhone)) != null)
		{
			mobilePhone.ToggleRinging(state == global::Telephone.CallState.Ringing);
		}
	}

	// Token: 0x06001F64 RID: 8036 RVA: 0x000CFC20 File Offset: 0x000CDE20
	public void BeginCall()
	{
		if (this.IsMobile && this.activeCallTo != null && !this.activeCallTo.RequirePower)
		{
			this.currentPlayer != null;
		}
		this.SetPhoneStateWithPlayer(global::Telephone.CallState.InProcess);
		base.Invoke(new Action(this.TimeOutCall), (float)TelephoneManager.MaxCallLength);
	}

	// Token: 0x06001F65 RID: 8037 RVA: 0x000CFC7C File Offset: 0x000CDE7C
	public void ServerHangUp(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		this.ServerHangUp();
	}

	// Token: 0x06001F66 RID: 8038 RVA: 0x000CFC98 File Offset: 0x000CDE98
	public void ServerHangUp()
	{
		if (this.activeCallTo != null)
		{
			this.activeCallTo.RemoteHangUp();
		}
		this.SelfHangUp();
	}

	// Token: 0x06001F67 RID: 8039 RVA: 0x000CFCB9 File Offset: 0x000CDEB9
	private void SelfHangUp()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.SelfHangUp);
	}

	// Token: 0x06001F68 RID: 8040 RVA: 0x000CFCC2 File Offset: 0x000CDEC2
	private void RemoteHangUp()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.RemoteHangUp);
	}

	// Token: 0x06001F69 RID: 8041 RVA: 0x000CFCCB File Offset: 0x000CDECB
	private void TimeOutCall()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.TimeOutDuringCall);
	}

	// Token: 0x06001F6A RID: 8042 RVA: 0x000CFCD4 File Offset: 0x000CDED4
	public void OnReceivedVoiceFromUser(byte[] data)
	{
		if (this.activeCallTo != null)
		{
			this.activeCallTo.OnReceivedDataFromConnectedPhone(data);
		}
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x000CFCF0 File Offset: 0x000CDEF0
	public void OnReceivedDataFromConnectedPhone(byte[] data)
	{
		base.baseEntity.ClientRPCEx<int, byte[]>(new SendInfo(global::BaseNetworkable.GetConnectionsWithin(base.transform.position, 15f))
		{
			priority = Priority.Immediate
		}, null, "OnReceivedVoice", data.Length, data);
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x000CFD37 File Offset: 0x000CDF37
	public void OnIncomingCallWhileBusy()
	{
		base.baseEntity.ClientRPC(null, "OnIncomingCallDuringCall");
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x000CFD4A File Offset: 0x000CDF4A
	public void DestroyShared()
	{
		if (this.isServer && this.serverState != global::Telephone.CallState.Idle && this.activeCallTo != null)
		{
			this.activeCallTo.RemoteHangUp();
		}
	}

	// Token: 0x06001F6E RID: 8046 RVA: 0x000CFD78 File Offset: 0x000CDF78
	public void UpdatePhoneName(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		string text = msg.read.String(256);
		if (text.Length > 20)
		{
			text = text.Substring(0, 20);
		}
		this.PhoneName = text;
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001F6F RID: 8047 RVA: 0x000CFDD4 File Offset: 0x000CDFD4
	public void Server_RequestPhoneDirectory(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		int page = msg.read.Int32();
		using (PhoneDirectory phoneDirectory = Pool.Get<PhoneDirectory>())
		{
			TelephoneManager.GetPhoneDirectory(this.PhoneNumber, page, 12, phoneDirectory);
			base.baseEntity.ClientRPC<PhoneDirectory>(null, "ReceivePhoneDirectory", phoneDirectory);
		}
	}

	// Token: 0x06001F70 RID: 8048 RVA: 0x000CFE44 File Offset: 0x000CE044
	public void Server_AddSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		if (this.savedNumbers == null)
		{
			this.savedNumbers = Pool.Get<PhoneDirectory>();
		}
		if (this.savedNumbers.entries == null)
		{
			this.savedNumbers.entries = Pool.GetList<PhoneDirectory.DirectoryEntry>();
		}
		int num = msg.read.Int32();
		string text = msg.read.String(256);
		if (!this.IsSavedContactValid(text, num))
		{
			return;
		}
		if (this.savedNumbers.entries.Count >= 10)
		{
			return;
		}
		PhoneDirectory.DirectoryEntry directoryEntry = Pool.Get<PhoneDirectory.DirectoryEntry>();
		directoryEntry.phoneName = text;
		directoryEntry.phoneNumber = num;
		directoryEntry.ShouldPool = false;
		this.savedNumbers.ShouldPool = false;
		this.savedNumbers.entries.Add(directoryEntry);
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001F71 RID: 8049 RVA: 0x000CFF18 File Offset: 0x000CE118
	public void Server_RemoveSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		uint number = msg.read.UInt32();
		if (this.savedNumbers.entries.RemoveAll((PhoneDirectory.DirectoryEntry p) => (long)p.phoneNumber == (long)((ulong)number)) > 0)
		{
			base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001F72 RID: 8050 RVA: 0x000CFF7B File Offset: 0x000CE17B
	public string GetDirectoryName()
	{
		return this.PhoneName;
	}

	// Token: 0x06001F73 RID: 8051 RVA: 0x000CFF84 File Offset: 0x000CE184
	private static string PositionToGridCoord(Vector3 position)
	{
		Vector2 a = new Vector2(TerrainMeta.NormalizeX(position.x), TerrainMeta.NormalizeZ(position.z));
		float num = TerrainMeta.Size.x / 1024f;
		int num2 = 7;
		Vector2 vector = a * num * (float)num2;
		float num3 = Mathf.Floor(vector.x) + 1f;
		float num4 = Mathf.Floor(num * (float)num2 - vector.y);
		string text = string.Empty;
		float num5 = num3 / 26f;
		float num6 = num3 % 26f;
		if (num6 == 0f)
		{
			num6 = 26f;
		}
		if (num5 > 1f)
		{
			text += Convert.ToChar(64 + (int)num5).ToString();
		}
		text += Convert.ToChar(64 + (int)num6).ToString();
		return string.Format("{0}{1}", text, num4);
	}

	// Token: 0x06001F74 RID: 8052 RVA: 0x000D006C File Offset: 0x000CE26C
	public void WatchForDisconnects()
	{
		bool flag = false;
		if (this.currentPlayer != null)
		{
			if (this.currentPlayer.IsSleeping())
			{
				flag = true;
			}
			if (this.currentPlayer.IsDead())
			{
				flag = true;
			}
			if (Vector3.Distance(base.transform.position, this.currentPlayer.transform.position) > 5f)
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			this.ServerHangUp();
			this.ClearCurrentUser();
		}
	}

	// Token: 0x06001F75 RID: 8053 RVA: 0x000D00E3 File Offset: 0x000CE2E3
	public void OnParentChanged(global::BaseEntity newParent)
	{
		if (newParent != null && newParent is global::BasePlayer)
		{
			TelephoneManager.RegisterTelephone(this, true);
			return;
		}
		TelephoneManager.DeregisterTelephone(this);
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x000D010A File Offset: 0x000CE30A
	private bool HasVoicemailSlot()
	{
		return this.MaxVoicemailSlots > 0;
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x000D0118 File Offset: 0x000CE318
	public void ServerSendVoicemail(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		byte[] data = msg.read.BytesWithSize(10485760U);
		PhoneController telephone = TelephoneManager.GetTelephone(msg.read.Int32());
		if (telephone == null)
		{
			return;
		}
		if (!global::Cassette.IsOggValid(data, telephone.cachedCassette))
		{
			return;
		}
		telephone.SaveVoicemail(data, msg.player.displayName);
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x000D0184 File Offset: 0x000CE384
	public void SaveVoicemail(byte[] data, string playerName)
	{
		uint audioId = FileStorage.server.Store(data, FileStorage.Type.ogg, base.baseEntity.net.ID, 0U);
		if (this.savedVoicemail == null)
		{
			this.savedVoicemail = Pool.GetList<ProtoBuf.VoicemailEntry>();
		}
		ProtoBuf.VoicemailEntry voicemailEntry = Pool.Get<ProtoBuf.VoicemailEntry>();
		voicemailEntry.audioId = audioId;
		voicemailEntry.timestamp = DateTime.Now.ToBinary();
		voicemailEntry.userName = playerName;
		voicemailEntry.ShouldPool = false;
		this.savedVoicemail.Add(voicemailEntry);
		while (this.savedVoicemail.Count > this.MaxVoicemailSlots)
		{
			FileStorage.server.Remove(this.savedVoicemail[0].audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
			this.savedVoicemail.RemoveAt(0);
		}
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x000D0255 File Offset: 0x000CE455
	public void ServerPlayVoicemail(global::BaseEntity.RPCMessage msg)
	{
		base.baseEntity.ClientRPC<int, uint>(null, "ClientToggleVoicemail", 1, msg.read.UInt32());
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x000D0274 File Offset: 0x000CE474
	public void ServerStopVoicemail(global::BaseEntity.RPCMessage msg)
	{
		base.baseEntity.ClientRPC<int, uint>(null, "ClientToggleVoicemail", 0, msg.read.UInt32());
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x000D0294 File Offset: 0x000CE494
	public void ServerDeleteVoicemail(global::BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		for (int i = 0; i < this.savedVoicemail.Count; i++)
		{
			if (this.savedVoicemail[i].audioId == num)
			{
				ProtoBuf.VoicemailEntry voicemailEntry = this.savedVoicemail[i];
				FileStorage.server.Remove(voicemailEntry.audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
				voicemailEntry.ShouldPool = true;
				Pool.Free<ProtoBuf.VoicemailEntry>(ref voicemailEntry);
				this.savedVoicemail.RemoveAt(i);
				base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
		}
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x000D032C File Offset: 0x000CE52C
	public void DeleteAllVoicemail()
	{
		if (this.savedVoicemail == null)
		{
			return;
		}
		foreach (ProtoBuf.VoicemailEntry voicemailEntry in this.savedVoicemail)
		{
			voicemailEntry.ShouldPool = true;
			FileStorage.server.Remove(voicemailEntry.audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
		}
		Pool.FreeList<ProtoBuf.VoicemailEntry>(ref this.savedVoicemail);
	}

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x06001F7D RID: 8061 RVA: 0x000D03B4 File Offset: 0x000CE5B4
	public int MaxVoicemailSlots
	{
		get
		{
			if (!(this.cachedCassette != null))
			{
				return 0;
			}
			return this.cachedCassette.MaximumVoicemailSlots;
		}
	}

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x06001F7E RID: 8062 RVA: 0x000D03D1 File Offset: 0x000CE5D1
	// (set) Token: 0x06001F7F RID: 8063 RVA: 0x000D03FE File Offset: 0x000CE5FE
	public global::BasePlayer currentPlayer
	{
		get
		{
			if (this.currentPlayerRef.IsValid(this.isServer))
			{
				return this.currentPlayerRef.Get(this.isServer).ToPlayer();
			}
			return null;
		}
		set
		{
			this.currentPlayerRef.Set(value);
		}
	}

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x06001F80 RID: 8064 RVA: 0x000D040C File Offset: 0x000CE60C
	private bool isServer
	{
		get
		{
			return base.baseEntity != null && base.baseEntity.isServer;
		}
	}

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06001F81 RID: 8065 RVA: 0x000D0429 File Offset: 0x000CE629
	// (set) Token: 0x06001F82 RID: 8066 RVA: 0x000D0431 File Offset: 0x000CE631
	public int lastDialedNumber { get; set; }

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06001F83 RID: 8067 RVA: 0x000D043A File Offset: 0x000CE63A
	// (set) Token: 0x06001F84 RID: 8068 RVA: 0x000D0442 File Offset: 0x000CE642
	public PhoneDirectory savedNumbers { get; set; }

	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06001F85 RID: 8069 RVA: 0x000CF226 File Offset: 0x000CD426
	public global::BaseEntity ParentEntity
	{
		get
		{
			return base.baseEntity;
		}
	}

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06001F86 RID: 8070 RVA: 0x000D044C File Offset: 0x000CE64C
	private global::Cassette cachedCassette
	{
		get
		{
			global::Telephone telephone;
			if (!(base.baseEntity != null) || (telephone = (base.baseEntity as global::Telephone)) == null)
			{
				return null;
			}
			return telephone.cachedCassette;
		}
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x000D0480 File Offset: 0x000CE680
	private bool IsPowered()
	{
		global::IOEntity ioentity;
		return base.baseEntity != null && (ioentity = (base.baseEntity as global::IOEntity)) != null && ioentity.IsPowered();
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x000D04B2 File Offset: 0x000CE6B2
	public bool IsSavedContactValid(string contactName, int contactNumber)
	{
		return contactName.Length > 0 && contactName.Length <= 20 && contactNumber >= 10000000 && contactNumber < 100000000;
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
	}

	// Token: 0x040018B7 RID: 6327
	private PhoneController activeCallTo;

	// Token: 0x040018B8 RID: 6328
	public int PhoneNumber;

	// Token: 0x040018B9 RID: 6329
	public string PhoneName;

	// Token: 0x040018BA RID: 6330
	public bool CanModifyPhoneName = true;

	// Token: 0x040018BB RID: 6331
	public bool CanSaveNumbers = true;

	// Token: 0x040018BC RID: 6332
	public bool RequirePower = true;

	// Token: 0x040018BD RID: 6333
	public bool RequireParent;

	// Token: 0x040018BE RID: 6334
	public float CallWaitingTime = 12f;

	// Token: 0x040018BF RID: 6335
	public bool AppendGridToName;

	// Token: 0x040018C0 RID: 6336
	public bool IsMobile;

	// Token: 0x040018C1 RID: 6337
	public bool CanSaveVoicemail;

	// Token: 0x040018C2 RID: 6338
	public GameObjectRef PhoneDialog;

	// Token: 0x040018C3 RID: 6339
	public VoiceProcessor VProcessor;

	// Token: 0x040018C4 RID: 6340
	public PreloadedCassetteContent PreloadedContent;

	// Token: 0x040018C5 RID: 6341
	public SoundDefinition DialToneSfx;

	// Token: 0x040018C6 RID: 6342
	public SoundDefinition RingingSfx;

	// Token: 0x040018C7 RID: 6343
	public SoundDefinition ErrorSfx;

	// Token: 0x040018C8 RID: 6344
	public SoundDefinition CallIncomingWhileBusySfx;

	// Token: 0x040018C9 RID: 6345
	public SoundDefinition PickupHandsetSfx;

	// Token: 0x040018CA RID: 6346
	public SoundDefinition PutDownHandsetSfx;

	// Token: 0x040018CB RID: 6347
	public SoundDefinition FailedWrongNumber;

	// Token: 0x040018CC RID: 6348
	public SoundDefinition FailedNoAnswer;

	// Token: 0x040018CD RID: 6349
	public SoundDefinition FailedNetworkBusy;

	// Token: 0x040018CE RID: 6350
	public SoundDefinition FailedEngaged;

	// Token: 0x040018CF RID: 6351
	public SoundDefinition FailedRemoteHangUp;

	// Token: 0x040018D0 RID: 6352
	public SoundDefinition FailedSelfHangUp;

	// Token: 0x040018D1 RID: 6353
	public Light RingingLight;

	// Token: 0x040018D2 RID: 6354
	public float RingingLightFrequency = 0.4f;

	// Token: 0x040018D3 RID: 6355
	public AudioSource answeringMachineSound;

	// Token: 0x040018D4 RID: 6356
	public EntityRef currentPlayerRef;

	// Token: 0x040018D7 RID: 6359
	public List<ProtoBuf.VoicemailEntry> savedVoicemail;
}
