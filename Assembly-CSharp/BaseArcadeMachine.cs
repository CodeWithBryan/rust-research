using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x02000032 RID: 50
public class BaseArcadeMachine : global::BaseVehicle
{
	// Token: 0x060001FB RID: 507 RVA: 0x00024380 File Offset: 0x00022580
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseArcadeMachine.OnRpcMessage", 0))
		{
			if (rpc == 271542211U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BroadcastEntityMessage ");
				}
				using (TimeWarning.New("BroadcastEntityMessage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(271542211U, "BroadcastEntityMessage", this, player, 7UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(271542211U, "BroadcastEntityMessage", this, player, 3f))
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
							this.BroadcastEntityMessage(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in BroadcastEntityMessage");
					}
				}
				return true;
			}
			if (rpc == 1365277306U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DestroyMessageFromHost ");
				}
				using (TimeWarning.New("DestroyMessageFromHost", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1365277306U, "DestroyMessageFromHost", this, player, 3f))
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
							this.DestroyMessageFromHost(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in DestroyMessageFromHost");
					}
				}
				return true;
			}
			if (rpc == 2467852388U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - GetSnapshotFromClient ");
				}
				using (TimeWarning.New("GetSnapshotFromClient", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2467852388U, "GetSnapshotFromClient", this, player, 30UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2467852388U, "GetSnapshotFromClient", this, player, 3f))
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
							this.GetSnapshotFromClient(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in GetSnapshotFromClient");
					}
				}
				return true;
			}
			if (rpc == 2990871635U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestAddScore ");
				}
				using (TimeWarning.New("RequestAddScore", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2990871635U, "RequestAddScore", this, player, 3f))
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
							this.RequestAddScore(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RequestAddScore");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060001FC RID: 508 RVA: 0x00024970 File Offset: 0x00022B70
	public void AddScore(global::BasePlayer player, int score)
	{
		BaseArcadeMachine.ScoreEntry scoreEntry = new BaseArcadeMachine.ScoreEntry();
		scoreEntry.displayName = player.displayName;
		scoreEntry.score = score;
		scoreEntry.playerID = player.userID;
		this.scores.Add(scoreEntry);
		this.scores.Sort((BaseArcadeMachine.ScoreEntry a, BaseArcadeMachine.ScoreEntry b) => b.score.CompareTo(a.score));
		this.scores.TrimExcess();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060001FD RID: 509 RVA: 0x000249EC File Offset: 0x00022BEC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RequestAddScore(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.PlayerIsMounted(player))
		{
			return;
		}
		int score = msg.read.Int32();
		this.AddScore(player, score);
	}

	// Token: 0x060001FE RID: 510 RVA: 0x00024A28 File Offset: 0x00022C28
	public override void PlayerMounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		base.ClientRPCPlayer(null, player, "BeginHosting");
		base.SetFlag(global::BaseEntity.Flags.Reserved7, true, true, true);
	}

	// Token: 0x060001FF RID: 511 RVA: 0x00024A4D File Offset: 0x00022C4D
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		base.ClientRPCPlayer(null, player, "EndHosting");
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, true, true);
		if (!this.AnyMounted())
		{
			this.NearbyClientMessage("NoHost");
		}
	}

	// Token: 0x06000200 RID: 512 RVA: 0x00024A88 File Offset: 0x00022C88
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.arcadeMachine = Facepunch.Pool.Get<ArcadeMachine>();
		info.msg.arcadeMachine.scores = Facepunch.Pool.GetList<ArcadeMachine.ScoreEntry>();
		for (int i = 0; i < this.scores.Count; i++)
		{
			ArcadeMachine.ScoreEntry scoreEntry = Facepunch.Pool.Get<ArcadeMachine.ScoreEntry>();
			scoreEntry.displayName = this.scores[i].displayName;
			scoreEntry.playerID = this.scores[i].playerID;
			scoreEntry.score = this.scores[i].score;
			info.msg.arcadeMachine.scores.Add(scoreEntry);
		}
	}

	// Token: 0x06000201 RID: 513 RVA: 0x00024B38 File Offset: 0x00022D38
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.arcadeMachine != null && info.msg.arcadeMachine.scores != null)
		{
			this.scores.Clear();
			for (int i = 0; i < info.msg.arcadeMachine.scores.Count; i++)
			{
				BaseArcadeMachine.ScoreEntry scoreEntry = new BaseArcadeMachine.ScoreEntry();
				scoreEntry.displayName = info.msg.arcadeMachine.scores[i].displayName;
				scoreEntry.score = info.msg.arcadeMachine.scores[i].score;
				scoreEntry.playerID = info.msg.arcadeMachine.scores[i].playerID;
				this.scores.Add(scoreEntry);
			}
		}
	}

	// Token: 0x06000202 RID: 514 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return false;
	}

	// Token: 0x06000203 RID: 515 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
	}

	// Token: 0x06000204 RID: 516 RVA: 0x00024C14 File Offset: 0x00022E14
	public void NearbyClientMessage(string msg)
	{
		if (this.networkTrigger.entityContents != null)
		{
			foreach (global::BaseEntity baseEntity in this.networkTrigger.entityContents)
			{
				global::BasePlayer component = baseEntity.GetComponent<global::BasePlayer>();
				base.ClientRPCPlayer(null, component, msg);
			}
		}
	}

	// Token: 0x06000205 RID: 517 RVA: 0x00024C80 File Offset: 0x00022E80
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void DestroyMessageFromHost(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || base.GetDriver() != player)
		{
			return;
		}
		if (this.networkTrigger.entityContents != null)
		{
			uint arg = msg.read.UInt32();
			foreach (global::BaseEntity baseEntity in this.networkTrigger.entityContents)
			{
				global::BasePlayer component = baseEntity.GetComponent<global::BasePlayer>();
				base.ClientRPCPlayer<uint>(null, component, "DestroyEntity", arg);
			}
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x00024D1C File Offset: 0x00022F1C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(7UL)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void BroadcastEntityMessage(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || base.GetDriver() != player)
		{
			return;
		}
		if (this.networkTrigger.entityContents != null)
		{
			uint arg = msg.read.UInt32();
			string arg2 = msg.read.String(256);
			foreach (global::BaseEntity baseEntity in this.networkTrigger.entityContents)
			{
				global::BasePlayer component = baseEntity.GetComponent<global::BasePlayer>();
				base.ClientRPCPlayer<uint, string>(null, component, "GetEntityMessage", arg, arg2);
			}
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00024DCC File Offset: 0x00022FCC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(30UL)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void GetSnapshotFromClient(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || player != base.GetDriver())
		{
			return;
		}
		ArcadeGame arg = Facepunch.Pool.Get<ArcadeGame>();
		arg = ArcadeGame.Deserialize(msg.read);
		Connection sourceConnection = null;
		if (this.networkTrigger.entityContents != null)
		{
			foreach (global::BaseEntity baseEntity in this.networkTrigger.entityContents)
			{
				global::BasePlayer component = baseEntity.GetComponent<global::BasePlayer>();
				base.ClientRPCPlayer<ArcadeGame>(sourceConnection, component, "GetSnapshotFromServer", arg);
			}
		}
	}

	// Token: 0x04000201 RID: 513
	public BaseArcadeGame arcadeGamePrefab;

	// Token: 0x04000202 RID: 514
	public BaseArcadeGame activeGame;

	// Token: 0x04000203 RID: 515
	public ArcadeNetworkTrigger networkTrigger;

	// Token: 0x04000204 RID: 516
	public float broadcastRadius = 8f;

	// Token: 0x04000205 RID: 517
	public Transform gameScreen;

	// Token: 0x04000206 RID: 518
	public RawImage RTImage;

	// Token: 0x04000207 RID: 519
	public Transform leftJoystick;

	// Token: 0x04000208 RID: 520
	public Transform rightJoystick;

	// Token: 0x04000209 RID: 521
	public SoundPlayer musicPlayer;

	// Token: 0x0400020A RID: 522
	public const global::BaseEntity.Flags Flag_P1 = global::BaseEntity.Flags.Reserved7;

	// Token: 0x0400020B RID: 523
	public const global::BaseEntity.Flags Flag_P2 = global::BaseEntity.Flags.Reserved8;

	// Token: 0x0400020C RID: 524
	public List<BaseArcadeMachine.ScoreEntry> scores = new List<BaseArcadeMachine.ScoreEntry>(10);

	// Token: 0x0400020D RID: 525
	private const int inputFrameRate = 60;

	// Token: 0x0400020E RID: 526
	private const int snapshotFrameRate = 15;

	// Token: 0x02000B27 RID: 2855
	public class ScoreEntry
	{
		// Token: 0x04003CB0 RID: 15536
		public ulong playerID;

		// Token: 0x04003CB1 RID: 15537
		public int score;

		// Token: 0x04003CB2 RID: 15538
		public string displayName;
	}
}
