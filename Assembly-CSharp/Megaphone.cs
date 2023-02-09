using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000096 RID: 150
public class Megaphone : HeldEntity
{
	// Token: 0x06000D96 RID: 3478 RVA: 0x00072508 File Offset: 0x00070708
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Megaphone.OnRpcMessage", 0))
		{
			if (rpc == 4196056309U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_ToggleBroadcasting ");
				}
				using (TimeWarning.New("Server_ToggleBroadcasting", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(4196056309U, "Server_ToggleBroadcasting", this, player))
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
							this.Server_ToggleBroadcasting(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_ToggleBroadcasting");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000D97 RID: 3479 RVA: 0x0007266C File Offset: 0x0007086C
	// (set) Token: 0x06000D98 RID: 3480 RVA: 0x00072673 File Offset: 0x00070873
	[ReplicatedVar(Default = "100")]
	public static float MegaphoneVoiceRange { get; set; } = 100f;

	// Token: 0x06000D99 RID: 3481 RVA: 0x0007267C File Offset: 0x0007087C
	private void UpdateItemCondition()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null || !ownerItem.hasCondition)
		{
			return;
		}
		ownerItem.LoseCondition(this.VoiceDamageAmount);
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x000726A8 File Offset: 0x000708A8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private void Server_ToggleBroadcasting(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Int8() == 1;
		base.SetFlag(BaseEntity.Flags.On, flag, false, true);
		if (flag)
		{
			if (!base.IsInvoking(new Action(this.UpdateItemCondition)))
			{
				base.InvokeRepeating(new Action(this.UpdateItemCondition), 0f, this.VoiceDamageMinFrequency);
				return;
			}
		}
		else if (base.IsInvoking(new Action(this.UpdateItemCondition)))
		{
			base.CancelInvoke(new Action(this.UpdateItemCondition));
		}
	}

	// Token: 0x040008C5 RID: 2245
	[Header("Megaphone")]
	public VoiceProcessor voiceProcessor;

	// Token: 0x040008C6 RID: 2246
	public float VoiceDamageMinFrequency = 2f;

	// Token: 0x040008C7 RID: 2247
	public float VoiceDamageAmount = 1f;

	// Token: 0x040008C8 RID: 2248
	public AudioSource VoiceSource;

	// Token: 0x040008C9 RID: 2249
	public SoundDefinition StartBroadcastingSfx;

	// Token: 0x040008CA RID: 2250
	public SoundDefinition StopBroadcastingSfx;
}
