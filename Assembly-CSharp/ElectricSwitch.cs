using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200006B RID: 107
public class ElectricSwitch : IOEntity
{
	// Token: 0x06000A6A RID: 2666 RVA: 0x0005E2D8 File Offset: 0x0005C4D8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ElectricSwitch.OnRpcMessage", 0))
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

	// Token: 0x06000A6B RID: 2667 RVA: 0x00006C79 File Offset: 0x00004E79
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x0005E440 File Offset: 0x0005C640
	public override int ConsumptionAmount()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x0005E44D File Offset: 0x0005C64D
	public override void ResetIOState()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x0005E459 File Offset: 0x0005C659
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x0005E46B File Offset: 0x0005C66B
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && inputAmount > 0)
		{
			this.SetSwitch(true);
		}
		if (inputSlot == 2 && inputAmount > 0)
		{
			this.SetSwitch(false);
		}
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x0005E493 File Offset: 0x0005C693
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x0005E4AC File Offset: 0x0005C6AC
	public virtual void SetSwitch(bool wantsOn)
	{
		if (wantsOn == base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
		base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.Unbusy), 0.5f);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x0005E4FF File Offset: 0x0005C6FF
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SVSwitch(BaseEntity.RPCMessage msg)
	{
		this.SetSwitch(!base.IsOn());
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x0005E510 File Offset: 0x0005C710
	public void Unbusy()
	{
		base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x040006B1 RID: 1713
	public bool isToggleSwitch;
}
