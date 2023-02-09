using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000019 RID: 25
public class FogMachine : ContainerIOEntity
{
	// Token: 0x06000053 RID: 83 RVA: 0x000035EB File Offset: 0x000017EB
	public bool IsEmitting()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved6);
	}

	// Token: 0x06000054 RID: 84 RVA: 0x000035F8 File Offset: 0x000017F8
	public bool HasJuice()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5);
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00003608 File Offset: 0x00001808
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetFogOn(BaseEntity.RPCMessage msg)
	{
		if (this.IsEmitting())
		{
			return;
		}
		if (base.IsOn())
		{
			return;
		}
		if (!this.HasFuel())
		{
			return;
		}
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.InvokeRepeating(new Action(this.StartFogging), 0f, this.fogLength - 1f);
	}

	// Token: 0x06000056 RID: 86 RVA: 0x0000366B File Offset: 0x0000186B
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetFogOff(BaseEntity.RPCMessage msg)
	{
		if (!base.IsOn())
		{
			return;
		}
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.CancelInvoke(new Action(this.StartFogging));
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000057 RID: 87 RVA: 0x000036A0 File Offset: 0x000018A0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetMotionDetection(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved9, flag, false, true);
		if (flag)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
		this.UpdateMotionMode();
	}

	// Token: 0x06000058 RID: 88 RVA: 0x000036E8 File Offset: 0x000018E8
	public void UpdateMotionMode()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved9))
		{
			base.InvokeRandomized(new Action(this.CheckTrigger), UnityEngine.Random.Range(0f, 0.5f), 0.5f, 0.1f);
			return;
		}
		base.CancelInvoke(new Action(this.CheckTrigger));
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00003740 File Offset: 0x00001940
	public void CheckTrigger()
	{
		if (this.IsEmitting())
		{
			return;
		}
		if (BasePlayer.AnyPlayersVisibleToEntity(base.transform.position + base.transform.forward * 3f, 3f, this, base.transform.position + Vector3.up * 0.1f, true))
		{
			this.StartFogging();
		}
	}

	// Token: 0x0600005A RID: 90 RVA: 0x000037B0 File Offset: 0x000019B0
	public void StartFogging()
	{
		if (!this.UseFuel(1f))
		{
			base.CancelInvoke(new Action(this.StartFogging));
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved6, true, false, true);
		base.Invoke(new Action(this.EnableFogField), 1f);
		base.Invoke(new Action(this.DisableNozzle), this.nozzleBlastDuration);
		base.Invoke(new Action(this.FinishFogging), this.fogLength);
	}

	// Token: 0x0600005B RID: 91 RVA: 0x0000383E File Offset: 0x00001A3E
	public virtual void EnableFogField()
	{
		base.SetFlag(BaseEntity.Flags.Reserved10, true, false, true);
	}

	// Token: 0x0600005C RID: 92 RVA: 0x0000384E File Offset: 0x00001A4E
	public void DisableNozzle()
	{
		base.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
	}

	// Token: 0x0600005D RID: 93 RVA: 0x0000385E File Offset: 0x00001A5E
	public virtual void FinishFogging()
	{
		base.SetFlag(BaseEntity.Flags.Reserved10, false, false, true);
	}

	// Token: 0x0600005E RID: 94 RVA: 0x00003870 File Offset: 0x00001A70
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.Reserved10, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved5, this.HasFuel(), false, true);
		if (base.IsOn())
		{
			base.InvokeRepeating(new Action(this.StartFogging), 0f, this.fogLength - 1f);
		}
		this.UpdateMotionMode();
	}

	// Token: 0x0600005F RID: 95 RVA: 0x000038E3 File Offset: 0x00001AE3
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.SetFlag(BaseEntity.Flags.Reserved5, this.HasFuel(), false, true);
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00003900 File Offset: 0x00001B00
	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000061 RID: 97 RVA: 0x0000392E File Offset: 0x00001B2E
	public bool HasFuel()
	{
		return this.GetFuelAmount() >= 1;
	}

	// Token: 0x06000062 RID: 98 RVA: 0x0000393C File Offset: 0x00001B3C
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

	// Token: 0x06000063 RID: 99 RVA: 0x000039AC File Offset: 0x00001BAC
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		bool flag = false;
		if (inputSlot == 0)
		{
			flag = (inputAmount > 0);
		}
		else if (inputSlot == 1)
		{
			if (inputAmount == 0)
			{
				return;
			}
			flag = true;
		}
		else if (inputSlot == 2)
		{
			if (inputAmount == 0)
			{
				return;
			}
			flag = false;
		}
		if (flag)
		{
			if (this.IsEmitting())
			{
				return;
			}
			if (base.IsOn())
			{
				return;
			}
			if (!this.HasFuel())
			{
				return;
			}
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			base.InvokeRepeating(new Action(this.StartFogging), 0f, this.fogLength - 1f);
			return;
		}
		else
		{
			if (!base.IsOn())
			{
				return;
			}
			base.CancelInvoke(new Action(this.StartFogging));
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			return;
		}
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool MotionModeEnabled()
	{
		return true;
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00003A58 File Offset: 0x00001C58
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FogMachine.OnRpcMessage", 0))
		{
			if (rpc == 2788115565U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetFogOff ");
				}
				using (TimeWarning.New("SetFogOff", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2788115565U, "SetFogOff", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage fogOff = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetFogOff(fogOff);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetFogOff");
					}
				}
				return true;
			}
			if (rpc == 3905831928U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetFogOn ");
				}
				using (TimeWarning.New("SetFogOn", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3905831928U, "SetFogOn", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage fogOn = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetFogOn(fogOn);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SetFogOn");
					}
				}
				return true;
			}
			if (rpc == 1773639087U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetMotionDetection ");
				}
				using (TimeWarning.New("SetMotionDetection", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1773639087U, "SetMotionDetection", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage motionDetection = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetMotionDetection(motionDetection);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in SetMotionDetection");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0400005C RID: 92
	public const BaseEntity.Flags FogFieldOn = BaseEntity.Flags.Reserved10;

	// Token: 0x0400005D RID: 93
	public const BaseEntity.Flags MotionMode = BaseEntity.Flags.Reserved9;

	// Token: 0x0400005E RID: 94
	public const BaseEntity.Flags Emitting = BaseEntity.Flags.Reserved6;

	// Token: 0x0400005F RID: 95
	public const BaseEntity.Flags Flag_HasJuice = BaseEntity.Flags.Reserved5;

	// Token: 0x04000060 RID: 96
	public float fogLength = 60f;

	// Token: 0x04000061 RID: 97
	public float nozzleBlastDuration = 5f;

	// Token: 0x04000062 RID: 98
	public float fuelPerSec = 1f;

	// Token: 0x04000063 RID: 99
	private float pendingFuel;
}
