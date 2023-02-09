using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E7 RID: 231
public class WaterWell : LiquidContainer
{
	// Token: 0x0600141C RID: 5148 RVA: 0x0009E944 File Offset: 0x0009CB44
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WaterWell.OnRpcMessage", 0))
		{
			if (rpc == 2538739344U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Pump ");
				}
				using (TimeWarning.New("RPC_Pump", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2538739344U, "RPC_Pump", this, player, 3f))
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
							this.RPC_Pump(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Pump");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x0009EAAC File Offset: 0x0009CCAC
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x0009EAD0 File Offset: 0x0009CCD0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Pump(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || player.IsDead() || player.IsSleeping())
		{
			return;
		}
		if (player.metabolism.calories.value < this.caloriesPerPump)
		{
			return;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		player.metabolism.calories.value -= this.caloriesPerPump;
		player.metabolism.SendChangesToClient();
		this.currentPressure = Mathf.Clamp01(this.currentPressure + this.pressurePerPump);
		base.Invoke(new Action(this.StopPump), 1.8f);
		if (this.currentPressure >= 0f)
		{
			base.CancelInvoke(new Action(this.Produce));
			base.Invoke(new Action(this.Produce), 1f);
		}
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x0009EBC7 File Offset: 0x0009CDC7
	public void StopPump()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x0009EBDE File Offset: 0x0009CDDE
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x0009EBEF File Offset: 0x0009CDEF
	public void Produce()
	{
		base.inventory.AddItem(this.defaultLiquid, this.waterPerPump, 0UL, global::ItemContainer.LimitStack.Existing);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, true, false, true);
		this.ScheduleTapOff();
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x0009EC26 File Offset: 0x0009CE26
	public void ScheduleTapOff()
	{
		base.CancelInvoke(new Action(this.TapOff));
		base.Invoke(new Action(this.TapOff), 1f);
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x0009EC51 File Offset: 0x0009CE51
	private void TapOff()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x0009EC64 File Offset: 0x0009CE64
	public void ReducePressure()
	{
		float num = UnityEngine.Random.Range(0.1f, 0.2f);
		this.currentPressure = Mathf.Clamp01(this.currentPressure - num);
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x0009EC94 File Offset: 0x0009CE94
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.waterwell = Facepunch.Pool.Get<ProtoBuf.WaterWell>();
		info.msg.waterwell.pressure = this.currentPressure;
		info.msg.waterwell.waterLevel = this.GetWaterAmount();
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x0009ECE4 File Offset: 0x0009CEE4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.waterwell != null)
		{
			this.currentPressure = info.msg.waterwell.pressure;
		}
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x0009ED10 File Offset: 0x0009CF10
	public float GetWaterAmount()
	{
		if (!base.isServer)
		{
			return 0f;
		}
		global::Item slot = base.inventory.GetSlot(0);
		if (slot == null)
		{
			return 0f;
		}
		return (float)slot.amount;
	}

	// Token: 0x04000CB4 RID: 3252
	public Animator animator;

	// Token: 0x04000CB5 RID: 3253
	private const global::BaseEntity.Flags Pumping = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000CB6 RID: 3254
	private const global::BaseEntity.Flags WaterFlow = global::BaseEntity.Flags.Reserved3;

	// Token: 0x04000CB7 RID: 3255
	public float caloriesPerPump = 5f;

	// Token: 0x04000CB8 RID: 3256
	public float pressurePerPump = 0.2f;

	// Token: 0x04000CB9 RID: 3257
	public float pressureForProduction = 1f;

	// Token: 0x04000CBA RID: 3258
	public float currentPressure;

	// Token: 0x04000CBB RID: 3259
	public int waterPerPump = 50;

	// Token: 0x04000CBC RID: 3260
	public GameObject waterLevelObj;

	// Token: 0x04000CBD RID: 3261
	public float waterLevelObjFullOffset;
}
