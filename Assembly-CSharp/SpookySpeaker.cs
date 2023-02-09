using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CD RID: 205
public class SpookySpeaker : BaseCombatEntity
{
	// Token: 0x0600120C RID: 4620 RVA: 0x00090C74 File Offset: 0x0008EE74
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SpookySpeaker.OnRpcMessage", 0))
		{
			if (rpc == 2523893445U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetWantsOn ");
				}
				using (TimeWarning.New("SetWantsOn", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2523893445U, "SetWantsOn", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage wantsOn = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetWantsOn(wantsOn);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetWantsOn");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x00090DDC File Offset: 0x0008EFDC
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateInvokes();
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x00090DEC File Offset: 0x0008EFEC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetWantsOn(BaseEntity.RPCMessage msg)
	{
		bool b = msg.read.Bit();
		base.SetFlag(BaseEntity.Flags.On, b, false, true);
		this.UpdateInvokes();
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x00090E18 File Offset: 0x0008F018
	public void UpdateInvokes()
	{
		if (base.IsOn())
		{
			base.InvokeRandomized(new Action(this.SendPlaySound), this.soundSpacing, this.soundSpacing, this.soundSpacingRand);
			base.Invoke(new Action(this.DelayedOff), 7200f);
			return;
		}
		base.CancelInvoke(new Action(this.SendPlaySound));
		base.CancelInvoke(new Action(this.DelayedOff));
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x00090E8D File Offset: 0x0008F08D
	public void SendPlaySound()
	{
		base.ClientRPC(null, "PlaySpookySound");
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x0005E44D File Offset: 0x0005C64D
	public void DelayedOff()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x04000B53 RID: 2899
	public SoundPlayer soundPlayer;

	// Token: 0x04000B54 RID: 2900
	public float soundSpacing = 12f;

	// Token: 0x04000B55 RID: 2901
	public float soundSpacingRand = 5f;
}
