using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000076 RID: 118
public class FuelGenerator : ContainerIOEntity
{
	// Token: 0x06000B10 RID: 2832 RVA: 0x00061ED4 File Offset: 0x000600D4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FuelGenerator.OnRpcMessage", 0))
		{
			if (rpc == 1401355317U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_EngineSwitch ");
				}
				using (TimeWarning.New("RPC_EngineSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1401355317U, "RPC_EngineSwitch", this, player, 3f))
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
							this.RPC_EngineSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_EngineSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0006203C File Offset: 0x0006023C
	public override int MaximalPowerOutput()
	{
		return this.outputEnergy;
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x00007074 File Offset: 0x00005274
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x00062044 File Offset: 0x00060244
	public override void Init()
	{
		if (base.IsOn())
		{
			this.UpdateCurrentEnergy();
			base.InvokeRepeating(new Action(this.FuelConsumption), this.fuelTickRate, this.fuelTickRate);
		}
		base.Init();
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x00062078 File Offset: 0x00060278
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0 && inputAmount > 0)
		{
			this.TurnOn();
		}
		if (inputSlot == 1 && inputAmount > 0)
		{
			this.TurnOff();
		}
		base.UpdateFromInput(inputAmount, inputSlot);
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x0006209D File Offset: 0x0006029D
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.outputEnergy;
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x000620AF File Offset: 0x000602AF
	public void UpdateCurrentEnergy()
	{
		this.currentEnergy = this.CalculateCurrentEnergy(0, 0);
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x000620BF File Offset: 0x000602BF
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return this.currentEnergy;
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x000620CC File Offset: 0x000602CC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_EngineSwitch(BaseEntity.RPCMessage msg)
	{
		bool generatorState = msg.read.Bit();
		this.SetGeneratorState(generatorState);
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x000620EC File Offset: 0x000602EC
	public void SetGeneratorState(bool wantsOn)
	{
		if (wantsOn)
		{
			this.TurnOn();
			return;
		}
		this.TurnOff();
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x00062100 File Offset: 0x00060300
	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0006212E File Offset: 0x0006032E
	public bool HasFuel()
	{
		return this.GetFuelAmount() >= 1;
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0006213C File Offset: 0x0006033C
	public bool UseFuel(float seconds)
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		this.pendingFuel += seconds * this.fuelPerSec;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			this.pendingFuel -= (float)num;
		}
		return true;
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x000621AC File Offset: 0x000603AC
	public void TurnOn()
	{
		if (base.IsOn())
		{
			return;
		}
		if (this.UseFuel(1f))
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			this.UpdateCurrentEnergy();
			this.MarkDirty();
			base.InvokeRepeating(new Action(this.FuelConsumption), this.fuelTickRate, this.fuelTickRate);
		}
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x00062203 File Offset: 0x00060403
	public void FuelConsumption()
	{
		if (!this.UseFuel(this.fuelTickRate))
		{
			this.TurnOff();
		}
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x00062219 File Offset: 0x00060419
	public void TurnOff()
	{
		if (!base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.UpdateCurrentEnergy();
		this.MarkDirty();
		base.CancelInvoke(new Action(this.FuelConsumption));
	}

	// Token: 0x04000729 RID: 1833
	public int outputEnergy = 35;

	// Token: 0x0400072A RID: 1834
	public float fuelPerSec = 1f;

	// Token: 0x0400072B RID: 1835
	protected float fuelTickRate = 3f;

	// Token: 0x0400072C RID: 1836
	private float pendingFuel;
}
