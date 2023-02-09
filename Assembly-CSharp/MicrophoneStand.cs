using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

// Token: 0x02000097 RID: 151
public class MicrophoneStand : BaseMountable
{
	// Token: 0x06000D9D RID: 3485 RVA: 0x00072754 File Offset: 0x00070954
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MicrophoneStand.OnRpcMessage", 0))
		{
			if (rpc == 1420522459U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetMode ");
				}
				using (TimeWarning.New("SetMode", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage mode = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetMode(mode);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetMode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x00072878 File Offset: 0x00070A78
	[global::BaseEntity.RPC_Server]
	public void SetMode(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this._mounted)
		{
			return;
		}
		global::MicrophoneStand.SpeechMode speechMode = (global::MicrophoneStand.SpeechMode)msg.read.Int32();
		if (speechMode != this.currentSpeechMode)
		{
			this.currentSpeechMode = speechMode;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x000728BC File Offset: 0x00070ABC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.microphoneStand == null)
		{
			info.msg.microphoneStand = Facepunch.Pool.Get<ProtoBuf.MicrophoneStand>();
		}
		info.msg.microphoneStand.microphoneMode = (int)this.currentSpeechMode;
		info.msg.microphoneStand.IORef = this.ioEntity.uid;
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x00072920 File Offset: 0x00070B20
	public void SpawnChildEntity()
	{
		MicrophoneStandIOEntity microphoneStandIOEntity = GameManager.server.CreateEntity(this.IOSubEntity.resourcePath, this.IOSubEntitySpawnPos.localPosition, this.IOSubEntitySpawnPos.localRotation, true) as MicrophoneStandIOEntity;
		microphoneStandIOEntity.enableSaving = this.enableSaving;
		microphoneStandIOEntity.SetParent(this, false, false);
		microphoneStandIOEntity.Spawn();
		this.ioEntity.Set(microphoneStandIOEntity);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x0007298D File Offset: 0x00070B8D
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		this.SpawnChildEntity();
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x000729A0 File Offset: 0x00070BA0
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		if (this.IsStatic)
		{
			this.SpawnChildEntity();
			int num = 128;
			List<global::ConnectedSpeaker> list = Facepunch.Pool.GetList<global::ConnectedSpeaker>();
			GamePhysics.OverlapSphere<global::ConnectedSpeaker>(base.transform.position, (float)num, list, 256, QueryTriggerInteraction.Ignore);
			global::IOEntity ioentity = this.ioEntity.Get(true);
			List<global::MicrophoneStand> list2 = Facepunch.Pool.GetList<global::MicrophoneStand>();
			int num2 = 0;
			foreach (global::ConnectedSpeaker connectedSpeaker in list)
			{
				bool flag = true;
				list2.Clear();
				GamePhysics.OverlapSphere<global::MicrophoneStand>(connectedSpeaker.transform.position, (float)num, list2, 256, QueryTriggerInteraction.Ignore);
				if (list2.Count > 1)
				{
					float num3 = base.Distance(connectedSpeaker);
					foreach (global::MicrophoneStand microphoneStand in list2)
					{
						if (!microphoneStand.isClient && microphoneStand.Distance(connectedSpeaker) < num3)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					ioentity.outputs[0].connectedTo.Set(connectedSpeaker);
					connectedSpeaker.inputs[0].connectedTo.Set(ioentity);
					ioentity = connectedSpeaker;
					num2++;
				}
			}
			Facepunch.Pool.FreeList<global::ConnectedSpeaker>(ref list);
			Facepunch.Pool.FreeList<global::MicrophoneStand>(ref list2);
		}
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x00072B10 File Offset: 0x00070D10
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.microphoneStand != null)
		{
			this.currentSpeechMode = (global::MicrophoneStand.SpeechMode)info.msg.microphoneStand.microphoneMode;
			this.ioEntity.uid = info.msg.microphoneStand.IORef;
		}
	}

	// Token: 0x040008CC RID: 2252
	public VoiceProcessor VoiceProcessor;

	// Token: 0x040008CD RID: 2253
	public AudioSource VoiceSource;

	// Token: 0x040008CE RID: 2254
	private global::MicrophoneStand.SpeechMode currentSpeechMode;

	// Token: 0x040008CF RID: 2255
	public AudioMixerGroup NormalMix;

	// Token: 0x040008D0 RID: 2256
	public AudioMixerGroup HighPitchMix;

	// Token: 0x040008D1 RID: 2257
	public AudioMixerGroup LowPitchMix;

	// Token: 0x040008D2 RID: 2258
	public Translate.Phrase NormalPhrase = new Translate.Phrase("microphone_normal", "Normal");

	// Token: 0x040008D3 RID: 2259
	public Translate.Phrase NormalDescPhrase = new Translate.Phrase("microphone_normal_desc", "No voice effect");

	// Token: 0x040008D4 RID: 2260
	public Translate.Phrase HighPitchPhrase = new Translate.Phrase("microphone_high", "High Pitch");

	// Token: 0x040008D5 RID: 2261
	public Translate.Phrase HighPitchDescPhrase = new Translate.Phrase("microphone_high_desc", "High pitch voice");

	// Token: 0x040008D6 RID: 2262
	public Translate.Phrase LowPitchPhrase = new Translate.Phrase("microphone_low", "Low");

	// Token: 0x040008D7 RID: 2263
	public Translate.Phrase LowPitchDescPhrase = new Translate.Phrase("microphone_low_desc", "Low pitch voice");

	// Token: 0x040008D8 RID: 2264
	public GameObjectRef IOSubEntity;

	// Token: 0x040008D9 RID: 2265
	public Transform IOSubEntitySpawnPos;

	// Token: 0x040008DA RID: 2266
	public bool IsStatic;

	// Token: 0x040008DB RID: 2267
	public EntityRef<global::IOEntity> ioEntity;

	// Token: 0x02000B98 RID: 2968
	public enum SpeechMode
	{
		// Token: 0x04003EDF RID: 16095
		Normal,
		// Token: 0x04003EE0 RID: 16096
		HighPitch,
		// Token: 0x04003EE1 RID: 16097
		LowPitch
	}
}
