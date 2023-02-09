using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B7 RID: 183
public class RemoteControlEntity : BaseCombatEntity, IRemoteControllable
{
	// Token: 0x06001069 RID: 4201 RVA: 0x00086CBC File Offset: 0x00084EBC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RemoteControlEntity.OnRpcMessage", 0))
		{
			if (rpc == 1053317251U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
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

	// Token: 0x0600106A RID: 4202 RVA: 0x00086E24 File Offset: 0x00085024
	public Transform GetEyes()
	{
		return this.viewEyes;
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseEntity GetEnt()
	{
		return this;
	}

	// Token: 0x0600106C RID: 4204 RVA: 0x00007074 File Offset: 0x00005274
	public bool Occupied()
	{
		return false;
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x00086E2C File Offset: 0x0008502C
	public string GetIdentifier()
	{
		return this.rcIdentifier;
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void UserInput(InputState inputState, global::BasePlayer player)
	{
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x0600106F RID: 4207 RVA: 0x00086E34 File Offset: 0x00085034
	// (set) Token: 0x06001070 RID: 4208 RVA: 0x00086E3C File Offset: 0x0008503C
	public bool IsBeingControlled { get; private set; }

	// Token: 0x06001071 RID: 4209 RVA: 0x00086E45 File Offset: 0x00085045
	public virtual void InitializeControl(global::BasePlayer controller)
	{
		this.IsBeingControlled = true;
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x00086E4E File Offset: 0x0008504E
	public virtual void StopControl()
	{
		this.IsBeingControlled = false;
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x00086E58 File Offset: 0x00085058
	public void UpdateIdentifier(string newID, bool clientSend = false)
	{
		string text = this.rcIdentifier;
		if (base.isServer)
		{
			if (!RemoteControlEntity.IDInUse(newID))
			{
				this.rcIdentifier = newID;
				Debug.Log("Updated Identifier to : " + this.rcIdentifier);
			}
			else
			{
				Debug.Log("ID In use!" + newID);
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x00086EB1 File Offset: 0x000850B1
	public virtual void RCSetup()
	{
		if (base.isServer)
		{
			RemoteControlEntity.allControllables.Add(this);
		}
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x00086EC6 File Offset: 0x000850C6
	public virtual void RCShutdown()
	{
		if (base.isServer)
		{
			RemoteControlEntity.allControllables.Remove(this);
		}
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x00086EDC File Offset: 0x000850DC
	public override void InitShared()
	{
		base.InitShared();
		this.RCSetup();
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x00086EEA File Offset: 0x000850EA
	public override void DestroyShared()
	{
		this.RCShutdown();
		base.DestroyShared();
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool CanControl()
	{
		return true;
	}

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06001079 RID: 4217 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool RequiresMouse
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x00086EF8 File Offset: 0x000850F8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void Server_SetID(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanControl())
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
			Debug.Log("SetID success!");
			this.UpdateIdentifier(text2, false);
		}
	}

	// Token: 0x0600107B RID: 4219 RVA: 0x00086F62 File Offset: 0x00085162
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.rcEntity = Facepunch.Pool.Get<RCEntity>();
		info.msg.rcEntity.identifier = this.GetIdentifier();
	}

	// Token: 0x0600107C RID: 4220 RVA: 0x00086F94 File Offset: 0x00085194
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null && global::ComputerStation.IsValidIdentifier(info.msg.rcEntity.identifier))
		{
			this.UpdateIdentifier(info.msg.rcEntity.identifier, false);
		}
	}

	// Token: 0x0600107D RID: 4221 RVA: 0x00086FE3 File Offset: 0x000851E3
	public static bool IDInUse(string id)
	{
		return RemoteControlEntity.FindByID(id) != null;
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x00086FF0 File Offset: 0x000851F0
	public static IRemoteControllable FindByID(string id)
	{
		foreach (IRemoteControllable remoteControllable in RemoteControlEntity.allControllables)
		{
			if (remoteControllable != null && remoteControllable.GetIdentifier() == id)
			{
				return remoteControllable;
			}
		}
		return null;
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x00087054 File Offset: 0x00085254
	public static bool InstallControllable(IRemoteControllable newControllable)
	{
		if (RemoteControlEntity.allControllables.Contains(newControllable))
		{
			return false;
		}
		RemoteControlEntity.allControllables.Add(newControllable);
		return true;
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x00087071 File Offset: 0x00085271
	public static bool RemoveControllable(IRemoteControllable newControllable)
	{
		if (!RemoteControlEntity.allControllables.Contains(newControllable))
		{
			return false;
		}
		RemoteControlEntity.allControllables.Remove(newControllable);
		return true;
	}

	// Token: 0x04000A52 RID: 2642
	public static List<IRemoteControllable> allControllables = new List<IRemoteControllable>();

	// Token: 0x04000A53 RID: 2643
	[Header("RC Entity")]
	public string rcIdentifier = "NONE";

	// Token: 0x04000A54 RID: 2644
	public Transform viewEyes;

	// Token: 0x04000A55 RID: 2645
	public GameObjectRef IDPanelPrefab;
}
