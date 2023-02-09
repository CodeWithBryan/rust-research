using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000015 RID: 21
public class PatternFirework : MortarFirework, IUGCBrowserEntity
{
	// Token: 0x0600003B RID: 59 RVA: 0x00002B36 File Offset: 0x00000D36
	public override void DestroyShared()
	{
		base.DestroyShared();
		ProtoBuf.PatternFirework.Design design = this.Design;
		if (design != null)
		{
			design.Dispose();
		}
		this.Design = null;
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00002B56 File Offset: 0x00000D56
	public override void ServerInit()
	{
		base.ServerInit();
		this.ShellFuseLength = global::PatternFirework.FuseLength.Medium;
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00002B65 File Offset: 0x00000D65
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void StartOpenDesigner(global::BaseEntity.RPCMessage rpc)
	{
		if (!this.PlayerCanModify(rpc.player))
		{
			return;
		}
		base.ClientRPCPlayer(null, rpc.player, "OpenDesigner");
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00002B88 File Offset: 0x00000D88
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void ServerSetFireworkDesign(global::BaseEntity.RPCMessage rpc)
	{
		if (!this.PlayerCanModify(rpc.player))
		{
			return;
		}
		ProtoBuf.PatternFirework.Design design = ProtoBuf.PatternFirework.Design.Deserialize(rpc.read);
		if (((design != null) ? design.stars : null) != null)
		{
			while (design.stars.Count > this.MaxStars)
			{
				int index = design.stars.Count - 1;
				design.stars[index].Dispose();
				design.stars.RemoveAt(index);
			}
			foreach (ProtoBuf.PatternFirework.Star star in design.stars)
			{
				star.position = new Vector2(Mathf.Clamp(star.position.x, -1f, 1f), Mathf.Clamp(star.position.y, -1f, 1f));
				star.color = new Color(Mathf.Clamp01(star.color.r), Mathf.Clamp01(star.color.g), Mathf.Clamp01(star.color.b), 1f);
			}
			design.editedBy = rpc.player.userID;
		}
		ProtoBuf.PatternFirework.Design design2 = this.Design;
		if (design2 != null)
		{
			design2.Dispose();
		}
		this.Design = design;
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00002CF8 File Offset: 0x00000EF8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void SetShellFuseLength(global::BaseEntity.RPCMessage rpc)
	{
		if (!this.PlayerCanModify(rpc.player))
		{
			return;
		}
		this.ShellFuseLength = (global::PatternFirework.FuseLength)Mathf.Clamp(rpc.read.Int32(), 0, 2);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00002D28 File Offset: 0x00000F28
	private bool PlayerCanModify(global::BasePlayer player)
	{
		if (player == null || !player.CanInteract())
		{
			return false;
		}
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		return !(buildingPrivilege != null) || buildingPrivilege.CanAdministrate(player);
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00002D64 File Offset: 0x00000F64
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.patternFirework = Facepunch.Pool.Get<ProtoBuf.PatternFirework>();
		ProtoBuf.PatternFirework patternFirework = info.msg.patternFirework;
		ProtoBuf.PatternFirework.Design design = this.Design;
		patternFirework.design = ((design != null) ? design.Copy() : null);
		info.msg.patternFirework.shellFuseLength = (int)this.ShellFuseLength;
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x06000042 RID: 66 RVA: 0x00002DC0 File Offset: 0x00000FC0
	public uint[] GetContentCRCs
	{
		get
		{
			if (this.Design == null || this.Design.stars.Count <= 0)
			{
				return Array.Empty<uint>();
			}
			return new uint[]
			{
				1U
			};
		}
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00002DED File Offset: 0x00000FED
	public void ClearContent()
	{
		ProtoBuf.PatternFirework.Design design = this.Design;
		if (design != null)
		{
			design.Dispose();
		}
		this.Design = null;
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000044 RID: 68 RVA: 0x00002E0E File Offset: 0x0000100E
	public UGCType ContentType
	{
		get
		{
			return UGCType.PatternBoomer;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000045 RID: 69 RVA: 0x00002E11 File Offset: 0x00001011
	public List<ulong> EditingHistory
	{
		get
		{
			if (this.Design == null)
			{
				return new List<ulong>();
			}
			return new List<ulong>
			{
				this.Design.editedBy
			};
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x06000046 RID: 70 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00002E3C File Offset: 0x0000103C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.patternFirework != null)
		{
			ProtoBuf.PatternFirework.Design design = this.Design;
			if (design != null)
			{
				design.Dispose();
			}
			ProtoBuf.PatternFirework.Design design2 = info.msg.patternFirework.design;
			this.Design = ((design2 != null) ? design2.Copy() : null);
			this.ShellFuseLength = (global::PatternFirework.FuseLength)info.msg.patternFirework.shellFuseLength;
		}
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00002EA8 File Offset: 0x000010A8
	private float GetShellFuseLength()
	{
		switch (this.ShellFuseLength)
		{
		case global::PatternFirework.FuseLength.Short:
			return this.ShellFuseLengthShort;
		case global::PatternFirework.FuseLength.Medium:
			return this.ShellFuseLengthMed;
		case global::PatternFirework.FuseLength.Long:
			return this.ShellFuseLengthLong;
		default:
			return this.ShellFuseLengthMed;
		}
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00002EEC File Offset: 0x000010EC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PatternFirework.OnRpcMessage", 0))
		{
			if (rpc == 3850129568U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerSetFireworkDesign ");
				}
				using (TimeWarning.New("ServerSetFireworkDesign", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3850129568U, "ServerSetFireworkDesign", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3850129568U, "ServerSetFireworkDesign", this, player, 3f))
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
							this.ServerSetFireworkDesign(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerSetFireworkDesign");
					}
				}
				return true;
			}
			if (rpc == 2132764204U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetShellFuseLength ");
				}
				using (TimeWarning.New("SetShellFuseLength", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2132764204U, "SetShellFuseLength", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2132764204U, "SetShellFuseLength", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage shellFuseLength = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetShellFuseLength(shellFuseLength);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SetShellFuseLength");
					}
				}
				return true;
			}
			if (rpc == 2760408151U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartOpenDesigner ");
				}
				using (TimeWarning.New("StartOpenDesigner", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2760408151U, "StartOpenDesigner", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2760408151U, "StartOpenDesigner", this, player, 3f))
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
							this.StartOpenDesigner(rpc3);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in StartOpenDesigner");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x04000047 RID: 71
	public const int CurrentVersion = 1;

	// Token: 0x04000048 RID: 72
	[Header("PatternFirework")]
	public GameObjectRef FireworkDesignerDialog;

	// Token: 0x04000049 RID: 73
	public int MaxStars = 25;

	// Token: 0x0400004A RID: 74
	public float ShellFuseLengthShort = 3f;

	// Token: 0x0400004B RID: 75
	public float ShellFuseLengthMed = 5.5f;

	// Token: 0x0400004C RID: 76
	public float ShellFuseLengthLong = 8f;

	// Token: 0x0400004D RID: 77
	[NonSerialized]
	public ProtoBuf.PatternFirework.Design Design;

	// Token: 0x0400004E RID: 78
	[NonSerialized]
	public global::PatternFirework.FuseLength ShellFuseLength;

	// Token: 0x02000B09 RID: 2825
	public enum FuseLength
	{
		// Token: 0x04003C6D RID: 15469
		Short,
		// Token: 0x04003C6E RID: 15470
		Medium,
		// Token: 0x04003C6F RID: 15471
		Long,
		// Token: 0x04003C70 RID: 15472
		Max = 2
	}
}
