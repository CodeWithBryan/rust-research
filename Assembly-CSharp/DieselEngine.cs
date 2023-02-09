using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000063 RID: 99
public class DieselEngine : StorageContainer
{
	// Token: 0x060009FC RID: 2556 RVA: 0x0005B554 File Offset: 0x00059754
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DieselEngine.OnRpcMessage", 0))
		{
			if (rpc == 578721460U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - EngineSwitch ");
				}
				using (TimeWarning.New("EngineSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(578721460U, "EngineSwitch", this, player, 6f))
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
							this.EngineSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in EngineSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x0005B6BC File Offset: 0x000598BC
	public override bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		return base.CanOpenLootPanel(player, panelName);
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x0005B6C8 File Offset: 0x000598C8
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.IsOn())
		{
			if (this.cachedFuelTime <= UnityEngine.Time.fixedDeltaTime && this.ConsumeFuelItem(1))
			{
				this.cachedFuelTime += this.runningTimePerFuelUnit;
			}
			this.cachedFuelTime -= UnityEngine.Time.fixedDeltaTime;
			if (this.cachedFuelTime <= 0f)
			{
				this.cachedFuelTime = 0f;
				this.EngineOff();
			}
		}
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x0005B740 File Offset: 0x00059940
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(6f)]
	public void EngineSwitch(global::BaseEntity.RPCMessage msg)
	{
		if (msg.read.Bit())
		{
			if (this.GetFuelAmount() > 0)
			{
				this.EngineOn();
				if (GameInfo.HasAchievements && msg.player != null)
				{
					msg.player.stats.Add("excavator_activated", 1, global::Stats.All);
					msg.player.stats.Save(true);
					return;
				}
			}
		}
		else
		{
			this.EngineOff();
		}
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x0005B7AD File Offset: 0x000599AD
	public void TimedShutdown()
	{
		this.EngineOff();
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x0005B7B8 File Offset: 0x000599B8
	public bool ConsumeFuelItem(int amount = 1)
	{
		global::Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < amount)
		{
			return false;
		}
		slot.UseItem(amount);
		this.UpdateHasFuelFlag();
		return true;
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x0005B7F0 File Offset: 0x000599F0
	public int GetFuelAmount()
	{
		global::Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x0005B81E File Offset: 0x00059A1E
	public void UpdateHasFuelFlag()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved3, this.GetFuelAmount() > 0, false, true);
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x0005B836 File Offset: 0x00059A36
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.UpdateHasFuelFlag();
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x0005B845 File Offset: 0x00059A45
	public void EngineOff()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.BroadcastEntityMessage("DieselEngineOff", 20f, 1218652417);
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x0005B866 File Offset: 0x00059A66
	public void EngineOn()
	{
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.BroadcastEntityMessage("DieselEngineOn", 20f, 1218652417);
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x0005B888 File Offset: 0x00059A88
	public void RescheduleEngineShutdown()
	{
		float time = 120f;
		base.Invoke(new Action(this.TimedShutdown), time);
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x0005B8AE File Offset: 0x00059AAE
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.IsOn())
		{
			base.BroadcastEntityMessage("DieselEngineOn", 20f, 1218652417);
			return;
		}
		base.BroadcastEntityMessage("DieselEngineOff", 20f, 1218652417);
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x0005B8E9 File Offset: 0x00059AE9
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.cachedFuelTime;
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x0005B918 File Offset: 0x00059B18
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.cachedFuelTime = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0002D546 File Offset: 0x0002B746
	public bool HasFuel()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved3);
	}

	// Token: 0x04000678 RID: 1656
	public GameObjectRef rumbleEffect;

	// Token: 0x04000679 RID: 1657
	public Transform rumbleOrigin;

	// Token: 0x0400067A RID: 1658
	public const global::BaseEntity.Flags Flag_HasFuel = global::BaseEntity.Flags.Reserved3;

	// Token: 0x0400067B RID: 1659
	public float runningTimePerFuelUnit = 120f;

	// Token: 0x0400067C RID: 1660
	private float cachedFuelTime;

	// Token: 0x0400067D RID: 1661
	private const float rumbleMaxDistSq = 100f;

	// Token: 0x0400067E RID: 1662
	private const string EXCAVATOR_ACTIVATED_STAT = "excavator_activated";
}
