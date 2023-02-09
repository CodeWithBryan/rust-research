using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200002F RID: 47
public class AudioVisualisationEntity : global::IOEntity
{
	// Token: 0x06000137 RID: 311 RVA: 0x0001FB48 File Offset: 0x0001DD48
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("AudioVisualisationEntity.OnRpcMessage", 0))
		{
			if (rpc == 4002266471U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerUpdateSettings ");
				}
				using (TimeWarning.New("ServerUpdateSettings", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4002266471U, "ServerUpdateSettings", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4002266471U, "ServerUpdateSettings", this, player, 3f))
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
							this.ServerUpdateSettings(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerUpdateSettings");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000138 RID: 312 RVA: 0x0001FD08 File Offset: 0x0001DF08
	// (set) Token: 0x06000139 RID: 313 RVA: 0x0001FD10 File Offset: 0x0001DF10
	public AudioVisualisationEntity.LightColour currentColour { get; private set; }

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x0600013A RID: 314 RVA: 0x0001FD19 File Offset: 0x0001DF19
	// (set) Token: 0x0600013B RID: 315 RVA: 0x0001FD21 File Offset: 0x0001DF21
	public AudioVisualisationEntity.VolumeSensitivity currentVolumeSensitivity { get; private set; } = AudioVisualisationEntity.VolumeSensitivity.Medium;

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x0600013C RID: 316 RVA: 0x0001FD2A File Offset: 0x0001DF2A
	// (set) Token: 0x0600013D RID: 317 RVA: 0x0001FD32 File Offset: 0x0001DF32
	public AudioVisualisationEntity.Speed currentSpeed { get; private set; } = AudioVisualisationEntity.Speed.Medium;

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x0600013E RID: 318 RVA: 0x0001FD3B File Offset: 0x0001DF3B
	// (set) Token: 0x0600013F RID: 319 RVA: 0x0001FD43 File Offset: 0x0001DF43
	public int currentGradient { get; private set; }

	// Token: 0x06000140 RID: 320 RVA: 0x0001FD4C File Offset: 0x0001DF4C
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(global::BaseEntity.Flags.Reserved8) != next.HasFlag(global::BaseEntity.Flags.Reserved8) && next.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			global::IOEntity audioSource = this.GetAudioSource(this, global::BoomBox.BacktrackLength);
			if (audioSource != null)
			{
				base.ClientRPC<uint>(null, "Client_PlayAudioFrom", audioSource.net.ID);
			}
			this.connectedTo.Set(audioSource);
		}
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0001FDE8 File Offset: 0x0001DFE8
	private global::IOEntity GetAudioSource(global::IOEntity entity, int depth)
	{
		if (depth <= 0)
		{
			return null;
		}
		global::IOEntity.IOSlot[] inputs = entity.inputs;
		for (int i = 0; i < inputs.Length; i++)
		{
			global::IOEntity ioentity = inputs[i].connectedTo.Get(base.isServer);
			if (ioentity == this)
			{
				return null;
			}
			IAudioConnectionSource audioConnectionSource;
			if (ioentity != null && ioentity.TryGetComponent<IAudioConnectionSource>(out audioConnectionSource))
			{
				return ioentity;
			}
			AudioVisualisationEntity audioVisualisationEntity;
			if (ioentity != null && ioentity.TryGetComponent<AudioVisualisationEntity>(out audioVisualisationEntity) && audioVisualisationEntity.connectedTo.IsSet)
			{
				return audioVisualisationEntity.connectedTo.Get(base.isServer) as global::IOEntity;
			}
			if (ioentity != null)
			{
				ioentity = this.GetAudioSource(ioentity, depth - 1);
				IAudioConnectionSource audioConnectionSource2;
				if (ioentity != null && ioentity.TryGetComponent<IAudioConnectionSource>(out audioConnectionSource2))
				{
					return ioentity;
				}
			}
		}
		return null;
	}

	// Token: 0x06000142 RID: 322 RVA: 0x0001FEB0 File Offset: 0x0001E0B0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.connectedSpeaker == null)
		{
			info.msg.connectedSpeaker = Facepunch.Pool.Get<ProtoBuf.ConnectedSpeaker>();
		}
		info.msg.connectedSpeaker.connectedTo = this.connectedTo.uid;
		if (info.msg.audioEntity == null)
		{
			info.msg.audioEntity = Facepunch.Pool.Get<AudioEntity>();
		}
		info.msg.audioEntity.colourMode = (int)this.currentColour;
		info.msg.audioEntity.volumeRange = (int)this.currentVolumeSensitivity;
		info.msg.audioEntity.speed = (int)this.currentSpeed;
		info.msg.audioEntity.gradient = this.currentGradient;
	}

	// Token: 0x06000143 RID: 323 RVA: 0x0001FF74 File Offset: 0x0001E174
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerUpdateSettings(global::BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		int num3 = msg.read.Int32();
		int num4 = msg.read.Int32();
		if (this.currentColour != (AudioVisualisationEntity.LightColour)num || this.currentVolumeSensitivity != (AudioVisualisationEntity.VolumeSensitivity)num2 || this.currentSpeed != (AudioVisualisationEntity.Speed)num3 || this.currentGradient != num4)
		{
			this.currentColour = (AudioVisualisationEntity.LightColour)num;
			this.currentVolumeSensitivity = (AudioVisualisationEntity.VolumeSensitivity)num2;
			this.currentSpeed = (AudioVisualisationEntity.Speed)num3;
			this.currentGradient = num4;
			this.MarkDirty();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000144 RID: 324 RVA: 0x00020000 File Offset: 0x0001E200
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.audioEntity != null)
		{
			this.currentColour = (AudioVisualisationEntity.LightColour)info.msg.audioEntity.colourMode;
			this.currentVolumeSensitivity = (AudioVisualisationEntity.VolumeSensitivity)info.msg.audioEntity.volumeRange;
			this.currentSpeed = (AudioVisualisationEntity.Speed)info.msg.audioEntity.speed;
			this.currentGradient = info.msg.audioEntity.gradient;
		}
		if (info.msg.connectedSpeaker != null)
		{
			this.connectedTo.uid = info.msg.connectedSpeaker.connectedTo;
		}
	}

	// Token: 0x0400018F RID: 399
	private EntityRef<global::BaseEntity> connectedTo;

	// Token: 0x04000193 RID: 403
	public GameObjectRef SettingsDialog;

	// Token: 0x02000B10 RID: 2832
	public enum LightColour
	{
		// Token: 0x04003C8C RID: 15500
		Red,
		// Token: 0x04003C8D RID: 15501
		Green,
		// Token: 0x04003C8E RID: 15502
		Blue,
		// Token: 0x04003C8F RID: 15503
		Yellow,
		// Token: 0x04003C90 RID: 15504
		Pink
	}

	// Token: 0x02000B11 RID: 2833
	public enum VolumeSensitivity
	{
		// Token: 0x04003C92 RID: 15506
		Small,
		// Token: 0x04003C93 RID: 15507
		Medium,
		// Token: 0x04003C94 RID: 15508
		Large
	}

	// Token: 0x02000B12 RID: 2834
	public enum Speed
	{
		// Token: 0x04003C96 RID: 15510
		Low,
		// Token: 0x04003C97 RID: 15511
		Medium,
		// Token: 0x04003C98 RID: 15512
		High
	}
}
