using System;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CA RID: 202
public class SmartSwitch : AppIOEntity
{
	// Token: 0x060011BF RID: 4543 RVA: 0x0008F688 File Offset: 0x0008D888
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SmartSwitch.OnRpcMessage", 0))
		{
			if (rpc == 2810053005U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ToggleSwitch ");
				}
				using (TimeWarning.New("ToggleSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2810053005U, "ToggleSwitch", this, player, 3UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2810053005U, "ToggleSwitch", this, player, 3f))
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
							this.ToggleSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ToggleSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060011C0 RID: 4544 RVA: 0x00006C79 File Offset: 0x00004E79
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x17000177 RID: 375
	// (get) Token: 0x060011C1 RID: 4545 RVA: 0x00003A54 File Offset: 0x00001C54
	public override AppEntityType Type
	{
		get
		{
			return AppEntityType.Switch;
		}
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x0005E493 File Offset: 0x0005C693
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x0005E440 File Offset: 0x0005C640
	public override int ConsumptionAmount()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x060011C4 RID: 4548 RVA: 0x0005E44D File Offset: 0x0005C64D
	public override void ResetIOState()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x060011C5 RID: 4549 RVA: 0x0005E459 File Offset: 0x0005C659
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x0008F848 File Offset: 0x0008DA48
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

	// Token: 0x060011C7 RID: 4551 RVA: 0x0008F870 File Offset: 0x0008DA70
	public void SetSwitch(bool wantsOn)
	{
		if (wantsOn == base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, wantsOn, false, true);
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.Unbusy), 0.5f);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.BroadcastValueChange();
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x0008F8C9 File Offset: 0x0008DAC9
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	public void ToggleSwitch(global::BaseEntity.RPCMessage msg)
	{
		if (!SmartSwitch.PlayerCanToggle(msg.player))
		{
			return;
		}
		this.SetSwitch(!base.IsOn());
	}

	// Token: 0x17000178 RID: 376
	// (get) Token: 0x060011C9 RID: 4553 RVA: 0x00006C79 File Offset: 0x00004E79
	// (set) Token: 0x060011CA RID: 4554 RVA: 0x0008F8E8 File Offset: 0x0008DAE8
	public override bool Value
	{
		get
		{
			return base.IsOn();
		}
		set
		{
			this.SetSwitch(value);
		}
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x0005E510 File Offset: 0x0005C710
	public void Unbusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x0008F8F1 File Offset: 0x0008DAF1
	private static bool PlayerCanToggle(global::BasePlayer player)
	{
		return player != null && player.CanBuild();
	}

	// Token: 0x04000B18 RID: 2840
	[Header("Smart Switch")]
	public Animator ReceiverAnimator;
}
