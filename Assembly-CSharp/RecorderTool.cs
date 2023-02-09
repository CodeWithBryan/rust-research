using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B4 RID: 180
public class RecorderTool : ThrownWeapon, ICassettePlayer
{
	// Token: 0x06001018 RID: 4120 RVA: 0x0008427C File Offset: 0x0008247C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RecorderTool.OnRpcMessage", 0))
		{
			if (rpc == 3075830603U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_TogglePlaying ");
				}
				using (TimeWarning.New("Server_TogglePlaying", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(3075830603U, "Server_TogglePlaying", this, player))
						{
							return true;
						}
					}
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
							this.Server_TogglePlaying(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_TogglePlaying");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06001019 RID: 4121 RVA: 0x000843E0 File Offset: 0x000825E0
	// (set) Token: 0x0600101A RID: 4122 RVA: 0x000843E8 File Offset: 0x000825E8
	public Cassette cachedCassette { get; private set; }

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x0600101B RID: 4123 RVA: 0x000843F1 File Offset: 0x000825F1
	public Sprite LoadedCassetteIcon
	{
		get
		{
			if (!(this.cachedCassette != null))
			{
				return null;
			}
			return this.cachedCassette.HudSprite;
		}
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x0008440E File Offset: 0x0008260E
	private bool HasCassette()
	{
		return this.cachedCassette != null;
	}

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x0600101D RID: 4125 RVA: 0x00002E37 File Offset: 0x00001037
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x0008441C File Offset: 0x0008261C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	public void Server_TogglePlaying(BaseEntity.RPCMessage msg)
	{
		bool b = msg.read.ReadByte() == 1;
		base.SetFlag(BaseEntity.Flags.On, b, false, true);
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x00084442 File Offset: 0x00082642
	public void OnCassetteInserted(Cassette c)
	{
		this.cachedCassette = c;
		base.ClientRPC<uint>(null, "Client_OnCassetteInserted", c.net.ID);
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x00084462 File Offset: 0x00082662
	public void OnCassetteRemoved(Cassette c)
	{
		this.cachedCassette = null;
		base.ClientRPC(null, "Client_OnCassetteRemoved");
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x00084478 File Offset: 0x00082678
	protected override void SetUpThrownWeapon(BaseEntity ent)
	{
		base.SetUpThrownWeapon(ent);
		if (base.GetOwnerPlayer() != null)
		{
			ent.OwnerID = base.GetOwnerPlayer().userID;
		}
		DeployedRecorder deployedRecorder;
		if (this.cachedCassette != null && (deployedRecorder = (ent as DeployedRecorder)) != null)
		{
			this.GetItem().contents.itemList[0].SetParent(deployedRecorder.inventory);
		}
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x000844E4 File Offset: 0x000826E4
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}

	// Token: 0x04000A32 RID: 2610
	[ClientVar(Saved = true)]
	public static bool debugRecording;

	// Token: 0x04000A33 RID: 2611
	public AudioSource RecorderAudioSource;

	// Token: 0x04000A34 RID: 2612
	public SoundDefinition RecordStartSfx;

	// Token: 0x04000A35 RID: 2613
	public SoundDefinition RewindSfx;

	// Token: 0x04000A36 RID: 2614
	public SoundDefinition RecordFinishedSfx;

	// Token: 0x04000A37 RID: 2615
	public SoundDefinition PlayTapeSfx;

	// Token: 0x04000A38 RID: 2616
	public SoundDefinition StopTapeSfx;

	// Token: 0x04000A39 RID: 2617
	public float ThrowScale = 3f;
}
