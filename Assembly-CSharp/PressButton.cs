using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B0 RID: 176
public class PressButton : IOEntity
{
	// Token: 0x06000FE5 RID: 4069 RVA: 0x0008319C File Offset: 0x0008139C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PressButton.OnRpcMessage", 0))
		{
			if (rpc == 3778543711U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Press ");
				}
				using (TimeWarning.New("Press", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3778543711U, "Press", this, player, 3f))
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
							this.Press(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Press");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x00083304 File Offset: 0x00081504
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		base.CancelInvoke(new Action(this.Unpress));
		base.CancelInvoke(new Action(this.UnpowerTime));
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x00083354 File Offset: 0x00081554
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		if (!(this.sourceItem != null) && !this.smallBurst)
		{
			return base.GetPassthroughAmount(0);
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved3))
		{
			return Mathf.Max(this.pressPowerAmount, base.GetPassthroughAmount(0));
		}
		return 0;
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x000833AA File Offset: 0x000815AA
	public void UnpowerTime()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x000833C0 File Offset: 0x000815C0
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000FEA RID: 4074 RVA: 0x000833D4 File Offset: 0x000815D4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void Press(BaseEntity.RPCMessage msg)
	{
		if (base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.Invoke(new Action(this.UnpowerTime), this.pressPowerTime);
		base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.Invoke(new Action(this.Unpress), this.pressDuration);
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x00053D03 File Offset: 0x00051F03
	public void Unpress()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x0008343F File Offset: 0x0008163F
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericFloat1 = this.pressDuration;
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x0008345E File Offset: 0x0008165E
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.pressDuration = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x04000A19 RID: 2585
	public float pressDuration = 5f;

	// Token: 0x04000A1A RID: 2586
	public float pressPowerTime = 0.5f;

	// Token: 0x04000A1B RID: 2587
	public int pressPowerAmount = 2;

	// Token: 0x04000A1C RID: 2588
	public const BaseEntity.Flags Flag_EmittingPower = BaseEntity.Flags.Reserved3;

	// Token: 0x04000A1D RID: 2589
	public bool smallBurst;
}
