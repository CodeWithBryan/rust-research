using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000108 RID: 264
public class ElectricBattery : global::IOEntity, IInstanceDataReceiver
{
	// Token: 0x0600152E RID: 5422 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x00007074 File Offset: 0x00005274
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x06001530 RID: 5424 RVA: 0x000A5B20 File Offset: 0x000A3D20
	public override int MaximalPowerOutput()
	{
		return this.maxOutput;
	}

	// Token: 0x06001531 RID: 5425 RVA: 0x000A5B28 File Offset: 0x000A3D28
	public int GetActiveDrain()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.activeDrain;
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x000A5B3A File Offset: 0x000A3D3A
	public void ReceiveInstanceData(ProtoBuf.Item.InstanceData data)
	{
		this.rustWattSeconds = (float)data.dataInt;
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x000A5B49 File Offset: 0x000A3D49
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.wasLoaded = true;
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x000A5B58 File Offset: 0x000A3D58
	public override void OnPickedUp(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUp(createdItem, player);
		if (createdItem.instanceData == null)
		{
			createdItem.instanceData = new ProtoBuf.Item.InstanceData();
		}
		createdItem.instanceData.ShouldPool = false;
		createdItem.instanceData.dataInt = Mathf.FloorToInt(this.rustWattSeconds);
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x000A5B97 File Offset: 0x000A3D97
	public override int GetCurrentEnergy()
	{
		return this.currentEnergy;
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x000A5B9F File Offset: 0x000A3D9F
	public override int DesiredPower()
	{
		if (this.rustWattSeconds >= this.maxCapactiySeconds)
		{
			return 0;
		}
		return Mathf.FloorToInt((float)this.maxOutput * this.maximumInboundEnergyRatio);
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x000A5BC4 File Offset: 0x000A3DC4
	public override void SendAdditionalData(global::BasePlayer player, int slot, bool input)
	{
		int passthroughAmountForAnySlot = base.GetPassthroughAmountForAnySlot(slot, input);
		base.ClientRPCPlayer<int, int, float, float>(null, player, "Client_ReceiveAdditionalData", this.currentEnergy, passthroughAmountForAnySlot, this.rustWattSeconds, (float)this.activeDrain);
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x000A5BFB File Offset: 0x000A3DFB
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.CheckDischarge), UnityEngine.Random.Range(0f, 1f), 1f, 0.1f);
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x00007074 File Offset: 0x00005274
	public int GetDrainFor(global::IOEntity ent)
	{
		return 0;
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x000A5C30 File Offset: 0x000A3E30
	public void AddConnectedRecursive(global::IOEntity root, ref HashSet<global::IOEntity> listToUse)
	{
		listToUse.Add(root);
		if (root.WantsPassthroughPower())
		{
			for (int i = 0; i < root.outputs.Length; i++)
			{
				if (root.AllowDrainFrom(i))
				{
					global::IOEntity ioentity = root.outputs[i].connectedTo.Get(true);
					if (ioentity != null)
					{
						bool flag = ioentity.WantsPower();
						if (!listToUse.Contains(ioentity))
						{
							if (flag)
							{
								this.AddConnectedRecursive(ioentity, ref listToUse);
							}
							else
							{
								listToUse.Add(ioentity);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600153B RID: 5435 RVA: 0x000A5CB0 File Offset: 0x000A3EB0
	public int GetDrain()
	{
		this.connectedList.Clear();
		global::IOEntity ioentity = this.outputs[0].connectedTo.Get(true);
		if (ioentity)
		{
			this.AddConnectedRecursive(ioentity, ref this.connectedList);
		}
		int num = 0;
		foreach (global::IOEntity ioentity2 in this.connectedList)
		{
			if (ioentity2.ShouldDrainBattery(this))
			{
				num += ioentity2.DesiredPower();
				if (num >= this.maxOutput)
				{
					num = this.maxOutput;
					break;
				}
			}
		}
		return num;
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x000A5D58 File Offset: 0x000A3F58
	public override void OnCircuitChanged(bool forceUpdate)
	{
		base.OnCircuitChanged(forceUpdate);
		int drain = this.GetDrain();
		this.activeDrain = drain;
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x000A5D7C File Offset: 0x000A3F7C
	public void CheckDischarge()
	{
		if (this.rustWattSeconds < 5f)
		{
			this.SetDischarging(false);
			return;
		}
		global::IOEntity ioentity = this.outputs[0].connectedTo.Get(true);
		int drain = this.GetDrain();
		this.activeDrain = drain;
		if (ioentity)
		{
			this.SetDischarging(ioentity.WantsPower());
			return;
		}
		this.SetDischarging(false);
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x000A5DDC File Offset: 0x000A3FDC
	public void SetDischarging(bool wantsOn)
	{
		this.SetPassthroughOn(wantsOn);
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x000A5DE5 File Offset: 0x000A3FE5
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (base.IsOn())
		{
			return Mathf.FloorToInt((float)this.maxOutput * ((this.rustWattSeconds >= 1f) ? 1f : 0f));
		}
		return 0;
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x000A5E17 File Offset: 0x000A4017
	public override bool WantsPower()
	{
		return this.rustWattSeconds < this.maxCapactiySeconds;
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x000A5E28 File Offset: 0x000A4028
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (inputSlot == 0)
		{
			if (!this.IsPowered())
			{
				if (this.rechargable)
				{
					base.CancelInvoke(new Action(this.AddCharge));
					return;
				}
			}
			else if (this.rechargable && !base.IsInvoking(new Action(this.AddCharge)))
			{
				base.InvokeRandomized(new Action(this.AddCharge), 1f, 1f, 0.1f);
			}
		}
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x000A5EA0 File Offset: 0x000A40A0
	public void TickUsage()
	{
		float oldCharge = this.rustWattSeconds;
		bool flag = this.rustWattSeconds > 0f;
		if (this.rustWattSeconds >= 1f)
		{
			float num = 1f * (float)this.activeDrain;
			this.rustWattSeconds -= num;
		}
		if (this.rustWattSeconds <= 0f)
		{
			this.rustWattSeconds = 0f;
		}
		bool flag2 = this.rustWattSeconds > 0f;
		this.ChargeChanged(oldCharge);
		if (flag != flag2)
		{
			this.MarkDirty();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x000A5F28 File Offset: 0x000A4128
	public virtual void ChargeChanged(float oldCharge)
	{
		float num = this.rustWattSeconds;
		bool flag = this.rustWattSeconds > this.maxCapactiySeconds * 0.25f;
		bool flag2 = this.rustWattSeconds > this.maxCapactiySeconds * 0.75f;
		if (base.HasFlag(global::BaseEntity.Flags.Reserved5) != flag || base.HasFlag(global::BaseEntity.Flags.Reserved6) != flag2)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, flag, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved6, flag2, false, false);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x000A5FA8 File Offset: 0x000A41A8
	public void AddCharge()
	{
		float oldCharge = this.rustWattSeconds;
		float num = (float)Mathf.Min(this.currentEnergy, this.DesiredPower()) * 1f * this.chargeRatio;
		this.rustWattSeconds += num;
		this.rustWattSeconds = Mathf.Clamp(this.rustWattSeconds, 0f, this.maxCapactiySeconds);
		this.ChargeChanged(oldCharge);
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x000A6010 File Offset: 0x000A4210
	public void SetPassthroughOn(bool wantsOn)
	{
		if (wantsOn == base.IsOn() && !this.wasLoaded)
		{
			return;
		}
		this.wasLoaded = false;
		base.SetFlag(global::BaseEntity.Flags.On, wantsOn, false, true);
		if (base.IsOn())
		{
			if (!base.IsInvoking(new Action(this.TickUsage)))
			{
				base.InvokeRandomized(new Action(this.TickUsage), 1f, 1f, 0.1f);
			}
		}
		else
		{
			base.CancelInvoke(new Action(this.TickUsage));
		}
		this.MarkDirty();
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x0005E510 File Offset: 0x0005C710
	public void Unbusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x000A6097 File Offset: 0x000A4297
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericFloat1 = this.rustWattSeconds;
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x000A60D3 File Offset: 0x000A42D3
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.rustWattSeconds = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x04000DB8 RID: 3512
	public int maxOutput;

	// Token: 0x04000DB9 RID: 3513
	public float maxCapactiySeconds;

	// Token: 0x04000DBA RID: 3514
	public float rustWattSeconds;

	// Token: 0x04000DBB RID: 3515
	private int activeDrain;

	// Token: 0x04000DBC RID: 3516
	public bool rechargable;

	// Token: 0x04000DBD RID: 3517
	[Tooltip("How much energy we can request from power sources for charging is this value * our maxOutput")]
	public float maximumInboundEnergyRatio = 4f;

	// Token: 0x04000DBE RID: 3518
	public float chargeRatio = 0.25f;

	// Token: 0x04000DBF RID: 3519
	private const float tickRateSeconds = 1f;

	// Token: 0x04000DC0 RID: 3520
	public const global::BaseEntity.Flags Flag_HalfFull = global::BaseEntity.Flags.Reserved5;

	// Token: 0x04000DC1 RID: 3521
	public const global::BaseEntity.Flags Flag_VeryFull = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000DC2 RID: 3522
	private bool wasLoaded;

	// Token: 0x04000DC3 RID: 3523
	private HashSet<global::IOEntity> connectedList = new HashSet<global::IOEntity>();
}
