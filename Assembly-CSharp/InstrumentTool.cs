using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000082 RID: 130
public class InstrumentTool : HeldEntity
{
	// Token: 0x06000C3D RID: 3133 RVA: 0x000693A4 File Offset: 0x000675A4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("InstrumentTool.OnRpcMessage", 0))
		{
			if (rpc == 1625188589U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_PlayNote ");
				}
				using (TimeWarning.New("Server_PlayNote", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_PlayNote(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_PlayNote");
					}
				}
				return true;
			}
			if (rpc == 705843933U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StopNote ");
				}
				using (TimeWarning.New("Server_StopNote", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_StopNote(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_StopNote");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x00069604 File Offset: 0x00067804
	[BaseEntity.RPC_Server]
	private void Server_PlayNote(BaseEntity.RPCMessage msg)
	{
		int arg = msg.read.Int32();
		int arg2 = msg.read.Int32();
		int arg3 = msg.read.Int32();
		float arg4 = msg.read.Float();
		this.KeyController.ProcessServerPlayedNote(base.GetOwnerPlayer());
		base.ClientRPC<int, int, int, float>(null, "Client_PlayNote", arg, arg2, arg3, arg4);
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x00069664 File Offset: 0x00067864
	[BaseEntity.RPC_Server]
	private void Server_StopNote(BaseEntity.RPCMessage msg)
	{
		int arg = msg.read.Int32();
		int arg2 = msg.read.Int32();
		int arg3 = msg.read.Int32();
		base.ClientRPC<int, int, int>(null, "Client_StopNote", arg, arg2, arg3);
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x000696A4 File Offset: 0x000678A4
	public override void ServerUse()
	{
		base.ServerUse();
		if (base.IsInvoking(new Action(this.StopAfterTime)))
		{
			return;
		}
		this.lastPlayedTurretData = this.KeyController.Bindings.BaseBindings[UnityEngine.Random.Range(0, this.KeyController.Bindings.BaseBindings.Length)];
		base.ClientRPC<int, int, int, float>(null, "Client_PlayNote", (int)this.lastPlayedTurretData.Note, (int)this.lastPlayedTurretData.Type, this.lastPlayedTurretData.NoteOctave, 1f);
		base.Invoke(new Action(this.StopAfterTime), 0.2f);
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x00069748 File Offset: 0x00067948
	private void StopAfterTime()
	{
		base.ClientRPC<int, int, int>(null, "Client_StopNote", (int)this.lastPlayedTurretData.Note, (int)this.lastPlayedTurretData.Type, this.lastPlayedTurretData.NoteOctave);
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000C42 RID: 3138 RVA: 0x00069777 File Offset: 0x00067977
	public override bool IsUsableByTurret
	{
		get
		{
			return this.UsableByAutoTurrets;
		}
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000C43 RID: 3139 RVA: 0x0006977F File Offset: 0x0006797F
	public override Transform MuzzleTransform
	{
		get
		{
			return this.MuzzleT;
		}
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsInstrument()
	{
		return true;
	}

	// Token: 0x040007D8 RID: 2008
	public InstrumentKeyController KeyController;

	// Token: 0x040007D9 RID: 2009
	public SoundDefinition DeploySound;

	// Token: 0x040007DA RID: 2010
	public Vector2 PitchClamp = new Vector2(-90f, 90f);

	// Token: 0x040007DB RID: 2011
	public bool UseAnimationSlotEvents;

	// Token: 0x040007DC RID: 2012
	public Transform MuzzleT;

	// Token: 0x040007DD RID: 2013
	public bool UsableByAutoTurrets;

	// Token: 0x040007DE RID: 2014
	private NoteBindingCollection.NoteData lastPlayedTurretData;
}
