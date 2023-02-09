using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000055 RID: 85
public class CodeLock : BaseLock
{
	// Token: 0x0600094A RID: 2378 RVA: 0x00056974 File Offset: 0x00054B74
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CodeLock.OnRpcMessage", 0))
		{
			if (rpc == 4013784361U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_ChangeCode ");
				}
				using (TimeWarning.New("RPC_ChangeCode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4013784361U, "RPC_ChangeCode", this, player, 3f))
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
							this.RPC_ChangeCode(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_ChangeCode");
					}
				}
				return true;
			}
			if (rpc == 2626067433U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TryLock ");
				}
				using (TimeWarning.New("TryLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2626067433U, "TryLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TryLock(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in TryLock");
					}
				}
				return true;
			}
			if (rpc == 1718262U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TryUnlock ");
				}
				using (TimeWarning.New("TryUnlock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1718262U, "TryUnlock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TryUnlock(rpc4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in TryUnlock");
					}
				}
				return true;
			}
			if (rpc == 418605506U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UnlockWithCode ");
				}
				using (TimeWarning.New("UnlockWithCode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(418605506U, "UnlockWithCode", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UnlockWithCode(rpc5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in UnlockWithCode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x00056F2C File Offset: 0x0005512C
	public bool IsCodeEntryBlocked()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved11);
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x00056F3C File Offset: 0x0005513C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.codeLock != null)
		{
			this.hasCode = info.msg.codeLock.hasCode;
			this.hasGuestCode = info.msg.codeLock.hasGuestCode;
			if (info.msg.codeLock.pv != null)
			{
				this.code = info.msg.codeLock.pv.code;
				this.whitelistPlayers = info.msg.codeLock.pv.users;
				this.guestCode = info.msg.codeLock.pv.guestCode;
				this.guestPlayers = info.msg.codeLock.pv.guestUsers;
			}
		}
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x0005700A File Offset: 0x0005520A
	internal void DoEffect(string effect)
	{
		Effect.server.Run(effect, this, 0U, Vector3.zero, Vector3.forward, null, false);
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x00057020 File Offset: 0x00055220
	public override bool OnTryToOpen(global::BasePlayer player)
	{
		if (!base.IsLocked())
		{
			return true;
		}
		if (this.whitelistPlayers.Contains(player.userID) || this.guestPlayers.Contains(player.userID))
		{
			this.DoEffect(this.effectUnlocked.resourcePath);
			return true;
		}
		this.DoEffect(this.effectDenied.resourcePath);
		return false;
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x00057084 File Offset: 0x00055284
	public override bool OnTryToClose(global::BasePlayer player)
	{
		if (!base.IsLocked())
		{
			return true;
		}
		if (this.whitelistPlayers.Contains(player.userID) || this.guestPlayers.Contains(player.userID))
		{
			this.DoEffect(this.effectUnlocked.resourcePath);
			return true;
		}
		this.DoEffect(this.effectDenied.resourcePath);
		return false;
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x00007074 File Offset: 0x00005274
	public override bool CanUseNetworkCache(Connection connection)
	{
		return false;
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x000570E8 File Offset: 0x000552E8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.codeLock = Facepunch.Pool.Get<ProtoBuf.CodeLock>();
		info.msg.codeLock.hasGuestCode = (this.guestCode.Length > 0);
		info.msg.codeLock.hasCode = (this.code.Length > 0);
		if (!info.forDisk && info.forConnection != null)
		{
			info.msg.codeLock.hasAuth = (this.whitelistPlayers.Contains(info.forConnection.userid) || this.guestPlayers.Contains(info.forConnection.userid));
		}
		if (info.forDisk)
		{
			info.msg.codeLock.pv = Facepunch.Pool.Get<ProtoBuf.CodeLock.Private>();
			info.msg.codeLock.pv.code = this.code;
			info.msg.codeLock.pv.users = Facepunch.Pool.Get<List<ulong>>();
			info.msg.codeLock.pv.users.AddRange(this.whitelistPlayers);
			info.msg.codeLock.pv.guestCode = this.guestCode;
			info.msg.codeLock.pv.guestUsers = Facepunch.Pool.Get<List<ulong>>();
			info.msg.codeLock.pv.guestUsers.AddRange(this.guestPlayers);
		}
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x00057264 File Offset: 0x00055464
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_ChangeCode(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		string text = rpc.read.String(256);
		bool flag = rpc.read.Bit();
		if (base.IsLocked())
		{
			return;
		}
		if (text.Length != 4)
		{
			return;
		}
		if (!text.IsNumeric())
		{
			return;
		}
		if (!this.hasCode && flag)
		{
			return;
		}
		if (!this.hasCode && !flag)
		{
			base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
		}
		if (!flag)
		{
			this.code = text;
			this.hasCode = (this.code.Length > 0);
			this.whitelistPlayers.Clear();
			this.whitelistPlayers.Add(rpc.player.userID);
		}
		else
		{
			this.guestCode = text;
			this.hasGuestCode = (this.guestCode.Length > 0);
			this.guestPlayers.Clear();
			this.guestPlayers.Add(rpc.player.userID);
		}
		this.DoEffect(this.effectCodeChanged.resourcePath);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x00057374 File Offset: 0x00055574
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void TryUnlock(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!base.IsLocked())
		{
			return;
		}
		if (this.IsCodeEntryBlocked())
		{
			return;
		}
		if (!this.whitelistPlayers.Contains(rpc.player.userID))
		{
			return;
		}
		this.DoEffect(this.effectUnlocked.resourcePath);
		base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x000573E0 File Offset: 0x000555E0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void TryLock(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (this.code.Length != 4)
		{
			return;
		}
		if (!this.whitelistPlayers.Contains(rpc.player.userID))
		{
			return;
		}
		this.DoEffect(this.effectLocked.resourcePath);
		base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x0005744F File Offset: 0x0005564F
	public void ClearCodeEntryBlocked()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved11, false, false, true);
		this.wrongCodes = 0;
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x00057468 File Offset: 0x00055668
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void UnlockWithCode(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!base.IsLocked())
		{
			return;
		}
		if (this.IsCodeEntryBlocked())
		{
			return;
		}
		string a = rpc.read.String(256);
		bool flag = a == this.guestCode;
		bool flag2 = a == this.code;
		if (!(a == this.code) && (!this.hasGuestCode || !(a == this.guestCode)))
		{
			if (UnityEngine.Time.realtimeSinceStartup > this.lastWrongTime + 60f)
			{
				this.wrongCodes = 0;
			}
			this.DoEffect(this.effectDenied.resourcePath);
			this.DoEffect(this.effectShock.resourcePath);
			rpc.player.Hurt((float)(this.wrongCodes + 1) * 5f, DamageType.ElectricShock, this, false);
			this.wrongCodes++;
			if (this.wrongCodes > 5)
			{
				rpc.player.ShowToast(GameTip.Styles.Red_Normal, global::CodeLock.blockwarning, Array.Empty<string>());
			}
			if ((float)this.wrongCodes >= global::CodeLock.maxFailedAttempts)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved11, true, false, true);
				base.Invoke(new Action(this.ClearCodeEntryBlocked), global::CodeLock.lockoutCooldown);
			}
			this.lastWrongTime = UnityEngine.Time.realtimeSinceStartup;
			return;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (flag2)
		{
			if (!this.whitelistPlayers.Contains(rpc.player.userID))
			{
				this.DoEffect(this.effectCodeChanged.resourcePath);
				this.whitelistPlayers.Add(rpc.player.userID);
				this.wrongCodes = 0;
				return;
			}
		}
		else if (flag && !this.guestPlayers.Contains(rpc.player.userID))
		{
			this.DoEffect(this.effectCodeChanged.resourcePath);
			this.guestPlayers.Add(rpc.player.userID);
		}
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x00057646 File Offset: 0x00055846
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.Reserved11, false, false, true);
	}

	// Token: 0x0400061C RID: 1564
	public GameObjectRef keyEnterDialog;

	// Token: 0x0400061D RID: 1565
	public GameObjectRef effectUnlocked;

	// Token: 0x0400061E RID: 1566
	public GameObjectRef effectLocked;

	// Token: 0x0400061F RID: 1567
	public GameObjectRef effectDenied;

	// Token: 0x04000620 RID: 1568
	public GameObjectRef effectCodeChanged;

	// Token: 0x04000621 RID: 1569
	public GameObjectRef effectShock;

	// Token: 0x04000622 RID: 1570
	private bool hasCode;

	// Token: 0x04000623 RID: 1571
	public const global::BaseEntity.Flags Flag_CodeEntryBlocked = global::BaseEntity.Flags.Reserved11;

	// Token: 0x04000624 RID: 1572
	public static readonly Translate.Phrase blockwarning = new Translate.Phrase("codelock.blockwarning", "Further failed attempts will block code entry for some time");

	// Token: 0x04000625 RID: 1573
	[ServerVar]
	public static float maxFailedAttempts = 8f;

	// Token: 0x04000626 RID: 1574
	[ServerVar]
	public static float lockoutCooldown = 900f;

	// Token: 0x04000627 RID: 1575
	private bool hasGuestCode;

	// Token: 0x04000628 RID: 1576
	private string code = string.Empty;

	// Token: 0x04000629 RID: 1577
	private string guestCode = string.Empty;

	// Token: 0x0400062A RID: 1578
	public List<ulong> whitelistPlayers = new List<ulong>();

	// Token: 0x0400062B RID: 1579
	public List<ulong> guestPlayers = new List<ulong>();

	// Token: 0x0400062C RID: 1580
	private int wrongCodes;

	// Token: 0x0400062D RID: 1581
	private float lastWrongTime = float.NegativeInfinity;
}
