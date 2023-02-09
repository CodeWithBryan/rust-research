using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000AF RID: 175
public class PoweredRemoteControlEntity : global::IOEntity, IRemoteControllable
{
	// Token: 0x06000FCE RID: 4046 RVA: 0x00082E04 File Offset: 0x00081004
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PoweredRemoteControlEntity.OnRpcMessage", 0))
		{
			if (rpc == 1053317251U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetID ");
				}
				using (TimeWarning.New("Server_SetID", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1053317251U, "Server_SetID", this, player, 3f))
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
							this.Server_SetID(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_SetID");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x00082F6C File Offset: 0x0008116C
	public bool IsStatic()
	{
		return this.isStatic;
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x00082F74 File Offset: 0x00081174
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		this.UpdateRCAccess(this.IsPowered());
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x00082F8A File Offset: 0x0008118A
	public void UpdateRCAccess(bool isOnline)
	{
		if (isOnline)
		{
			RemoteControlEntity.InstallControllable(this);
			return;
		}
		RemoteControlEntity.RemoveControllable(this);
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x00082FA0 File Offset: 0x000811A0
	public override void Spawn()
	{
		base.Spawn();
		string text = "#ID";
		if (this.IsStatic() && this.rcIdentifier.Contains(text))
		{
			int length = this.rcIdentifier.IndexOf(text);
			int length2 = text.Length;
			string text2 = this.rcIdentifier.Substring(0, length);
			text2 += this.net.ID.ToString();
			this.UpdateIdentifier(text2, false);
		}
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x00083010 File Offset: 0x00081210
	public Transform GetEyes()
	{
		return this.viewEyes;
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x00083018 File Offset: 0x00081218
	public virtual bool CanControl()
	{
		return this.IsPowered() || this.IsStatic();
	}

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x06000FD5 RID: 4053 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool RequiresMouse
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void UserInput(InputState inputState, global::BasePlayer player)
	{
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseEntity GetEnt()
	{
		return this;
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x00007074 File Offset: 0x00005274
	public bool Occupied()
	{
		return false;
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void InitializeControl(global::BasePlayer controller)
	{
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void StopControl()
	{
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void RCSetup()
	{
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x00020C55 File Offset: 0x0001EE55
	public virtual void RCShutdown()
	{
		if (base.isServer)
		{
			RemoteControlEntity.RemoveControllable(this);
		}
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x0008302C File Offset: 0x0008122C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void Server_SetID(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsStatic())
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (!player.CanBuild() || !player.IsBuildingAuthed())
		{
			return;
		}
		string text = msg.read.String(256);
		if (!global::ComputerStation.IsValidIdentifier(text))
		{
			return;
		}
		string text2 = msg.read.String(256);
		if (!global::ComputerStation.IsValidIdentifier(text2))
		{
			return;
		}
		if (text == this.GetIdentifier())
		{
			this.UpdateIdentifier(text2, false);
		}
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x000830A4 File Offset: 0x000812A4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.rcEntity = Facepunch.Pool.Get<RCEntity>();
		info.msg.rcEntity.identifier = this.GetIdentifier();
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x000830D4 File Offset: 0x000812D4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null && global::ComputerStation.IsValidIdentifier(info.msg.rcEntity.identifier))
		{
			this.UpdateIdentifier(info.msg.rcEntity.identifier, false);
		}
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x00083123 File Offset: 0x00081323
	public void UpdateIdentifier(string newID, bool clientSend = false)
	{
		string text = this.rcIdentifier;
		if (base.isServer)
		{
			if (!RemoteControlEntity.IDInUse(newID))
			{
				this.rcIdentifier = newID;
			}
			else
			{
				Debug.Log("ID In use!" + newID);
			}
			if (!Rust.Application.isLoadingSave)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x00083163 File Offset: 0x00081363
	public string GetIdentifier()
	{
		return this.rcIdentifier;
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x0008316B File Offset: 0x0008136B
	public override void InitShared()
	{
		base.InitShared();
		this.RCSetup();
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x00083179 File Offset: 0x00081379
	public override void DestroyShared()
	{
		this.RCShutdown();
		base.DestroyShared();
	}

	// Token: 0x04000A14 RID: 2580
	public string rcIdentifier = "NONE";

	// Token: 0x04000A15 RID: 2581
	public Transform viewEyes;

	// Token: 0x04000A16 RID: 2582
	public GameObjectRef IDPanelPrefab;

	// Token: 0x04000A17 RID: 2583
	public bool isStatic;

	// Token: 0x04000A18 RID: 2584
	public bool appendEntityIDToIdentifier;
}
