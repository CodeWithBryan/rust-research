using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D9 RID: 217
public class TimerSwitch : IOEntity
{
	// Token: 0x060012C4 RID: 4804 RVA: 0x00096970 File Offset: 0x00094B70
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TimerSwitch.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4167839872U, "SVSwitch", this, player, 3f))
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
							this.SVSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060012C5 RID: 4805 RVA: 0x00096AD8 File Offset: 0x00094CD8
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (base.IsInvoking(new Action(this.AdvanceTime)))
		{
			this.EndTimer();
		}
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x00096B04 File Offset: 0x00094D04
	public override bool WantsPassthroughPower()
	{
		return this.IsPowered() && base.IsOn();
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x00096B16 File Offset: 0x00094D16
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!this.IsPowered() || !base.IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(0);
	}

	// Token: 0x060012C8 RID: 4808 RVA: 0x00096B34 File Offset: 0x00094D34
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, inputAmount > 0, false, false);
		}
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x00096B4C File Offset: 0x00094D4C
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			if (!this.IsPowered() && base.IsInvoking(new Action(this.AdvanceTime)))
			{
				this.EndTimer();
				return;
			}
			if (this.timePassed != -1f)
			{
				base.SetFlag(BaseEntity.Flags.On, false, false, false);
				this.SwitchPressed();
				return;
			}
		}
		else if (inputSlot == 1 && inputAmount > 0)
		{
			this.SwitchPressed();
		}
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x00096BB3 File Offset: 0x00094DB3
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SVSwitch(BaseEntity.RPCMessage msg)
	{
		this.SwitchPressed();
	}

	// Token: 0x060012CB RID: 4811 RVA: 0x00096BBC File Offset: 0x00094DBC
	public void SwitchPressed()
	{
		if (base.IsOn())
		{
			return;
		}
		if (!this.IsPowered())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.MarkDirty();
		base.InvokeRepeating(new Action(this.AdvanceTime), 0f, 0.1f);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x00096C10 File Offset: 0x00094E10
	public void AdvanceTime()
	{
		if (this.timePassed < 0f)
		{
			this.timePassed = 0f;
		}
		this.timePassed += 0.1f;
		if (this.timePassed >= this.timerLength)
		{
			this.EndTimer();
			return;
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x00096C63 File Offset: 0x00094E63
	public void EndTimer()
	{
		base.CancelInvoke(new Action(this.AdvanceTime));
		this.timePassed = -1f;
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x00096C99 File Offset: 0x00094E99
	public float GetPassedTime()
	{
		return this.timePassed;
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x00096CA1 File Offset: 0x00094EA1
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.timePassed == -1f)
		{
			if (base.IsOn())
			{
				base.SetFlag(BaseEntity.Flags.On, false, false, true);
				return;
			}
		}
		else
		{
			this.SwitchPressed();
		}
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x00096CCF File Offset: 0x00094ECF
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericFloat1 = this.GetPassedTime();
		info.msg.ioEntity.genericFloat2 = this.timerLength;
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x00096D04 File Offset: 0x00094F04
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.timerLength = info.msg.ioEntity.genericFloat2;
			this.timePassed = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x04000BB2 RID: 2994
	public float timerLength = 10f;

	// Token: 0x04000BB3 RID: 2995
	public Transform timerDrum;

	// Token: 0x04000BB4 RID: 2996
	private float timePassed = -1f;
}
