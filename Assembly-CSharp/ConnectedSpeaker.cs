using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class ConnectedSpeaker : global::IOEntity
{
	// Token: 0x06000991 RID: 2449 RVA: 0x00059500 File Offset: 0x00057700
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ConnectedSpeaker.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x00059540 File Offset: 0x00057740
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(global::BaseEntity.Flags.Reserved8) != next.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			if (next.HasFlag(global::BaseEntity.Flags.Reserved8))
			{
				IAudioConnectionSource connectionSource = this.GetConnectionSource(this, global::BoomBox.BacktrackLength);
				if (connectionSource != null)
				{
					base.ClientRPC<uint>(null, "Client_PlayAudioFrom", connectionSource.ToEntity().net.ID);
					this.connectedTo.Set(connectionSource.ToEntity());
					return;
				}
			}
			else if (this.connectedTo.IsSet)
			{
				base.ClientRPC<uint>(null, "Client_StopPlayingAudio", this.connectedTo.uid);
				this.connectedTo.Set(null);
			}
		}
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x00059614 File Offset: 0x00057814
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.connectedSpeaker != null)
		{
			this.connectedTo.uid = info.msg.connectedSpeaker.connectedTo;
		}
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x00059648 File Offset: 0x00057848
	private IAudioConnectionSource GetConnectionSource(global::IOEntity entity, int depth)
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
			IAudioConnectionSource result;
			if (ioentity != null && (result = (ioentity as IAudioConnectionSource)) != null)
			{
				return result;
			}
			if (ioentity != null)
			{
				IAudioConnectionSource connectionSource = this.GetConnectionSource(ioentity, depth - 1);
				if (connectionSource != null)
				{
					return connectionSource;
				}
			}
		}
		return null;
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x000596C4 File Offset: 0x000578C4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.connectedSpeaker == null)
		{
			info.msg.connectedSpeaker = Pool.Get<ProtoBuf.ConnectedSpeaker>();
		}
		info.msg.connectedSpeaker.connectedTo = this.connectedTo.uid;
	}

	// Token: 0x04000656 RID: 1622
	public AudioSource SoundSource;

	// Token: 0x04000657 RID: 1623
	private EntityRef<global::IOEntity> connectedTo;

	// Token: 0x04000658 RID: 1624
	public VoiceProcessor VoiceProcessor;
}
