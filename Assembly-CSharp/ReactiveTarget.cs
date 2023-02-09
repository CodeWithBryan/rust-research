using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B2 RID: 178
public class ReactiveTarget : IOEntity
{
	// Token: 0x06000FFD RID: 4093 RVA: 0x0008392C File Offset: 0x00081B2C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ReactiveTarget.OnRpcMessage", 0))
		{
			if (rpc == 1798082523U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Lower ");
				}
				using (TimeWarning.New("RPC_Lower", 0))
				{
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
							this.RPC_Lower(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Lower");
					}
				}
				return true;
			}
			if (rpc == 2169477377U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Reset ");
				}
				using (TimeWarning.New("RPC_Reset", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Reset(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Reset");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000FFE RID: 4094 RVA: 0x00083B8C File Offset: 0x00081D8C
	public void OnHitShared(HitInfo info)
	{
		if (this.IsKnockedDown())
		{
			return;
		}
		bool flag = info.HitBone == StringPool.Get("target_collider");
		bool flag2 = info.HitBone == StringPool.Get("target_collider_bullseye");
		if (!flag && !flag2)
		{
			return;
		}
		if (base.isServer)
		{
			float num = info.damageTypes.Total();
			if (flag2)
			{
				num *= 2f;
				Effect.server.Run(this.bullseyeEffect.resourcePath, this, StringPool.Get("target_collider_bullseye"), Vector3.zero, Vector3.zero, null, false);
			}
			this.knockdownHealth -= num;
			if (this.knockdownHealth <= 0f)
			{
				Effect.server.Run(this.knockdownEffect.resourcePath, this, StringPool.Get("target_collider_bullseye"), Vector3.zero, Vector3.zero, null, false);
				base.SetFlag(BaseEntity.Flags.On, false, false, true);
				this.QueueReset();
				this.SendPowerBurst();
				base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
			else
			{
				base.ClientRPC<uint>(null, "HitEffect", info.Initiator.net.ID);
			}
			base.Hurt(1f, DamageType.Suicide, info.Initiator, false);
		}
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x0005DC16 File Offset: 0x0005BE16
	public bool IsKnockedDown()
	{
		return !base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x00083CAB File Offset: 0x00081EAB
	public override void OnAttacked(HitInfo info)
	{
		this.OnHitShared(info);
		base.OnAttacked(info);
	}

	// Token: 0x06001001 RID: 4097 RVA: 0x00083CBB File Offset: 0x00081EBB
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && this.CanToggle();
	}

	// Token: 0x06001002 RID: 4098 RVA: 0x00083CCE File Offset: 0x00081ECE
	public bool CanToggle()
	{
		return UnityEngine.Time.time > this.lastToggleTime + 1f;
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x00083CE3 File Offset: 0x00081EE3
	public void QueueReset()
	{
		base.Invoke(new Action(this.ResetTarget), 6f);
	}

	// Token: 0x06001004 RID: 4100 RVA: 0x00083CFC File Offset: 0x00081EFC
	public void ResetTarget()
	{
		if (!this.IsKnockedDown() || !this.CanToggle())
		{
			return;
		}
		base.CancelInvoke(new Action(this.ResetTarget));
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.knockdownHealth = 100f;
		this.SendPowerBurst();
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x00083D3C File Offset: 0x00081F3C
	private void LowerTarget()
	{
		if (this.IsKnockedDown() || !this.CanToggle())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.SendPowerBurst();
	}

	// Token: 0x06001006 RID: 4102 RVA: 0x00083D5F File Offset: 0x00081F5F
	private void SendPowerBurst()
	{
		this.lastToggleTime = UnityEngine.Time.time;
		base.MarkDirtyForceUpdateOutputs();
		base.Invoke(new Action(base.MarkDirtyForceUpdateOutputs), this.activationPowerTime * 1.01f);
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x00003A54 File Offset: 0x00001C54
	public override int ConsumptionAmount()
	{
		return 1;
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x00083D90 File Offset: 0x00081F90
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			return;
		}
		if (inputAmount > 0)
		{
			if (inputSlot == 1)
			{
				this.ResetTarget();
				return;
			}
			if (inputSlot == 2)
			{
				this.LowerTarget();
			}
		}
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x00083DB7 File Offset: 0x00081FB7
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.IsKnockedDown())
		{
			if (this.IsPowered())
			{
				return base.GetPassthroughAmount(0);
			}
			if (UnityEngine.Time.time < this.lastToggleTime + this.activationPowerTime)
			{
				return this.activationPowerAmount;
			}
		}
		return 0;
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x00083DED File Offset: 0x00081FED
	[BaseEntity.RPC_Server]
	public void RPC_Reset(BaseEntity.RPCMessage msg)
	{
		this.ResetTarget();
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x00083DF5 File Offset: 0x00081FF5
	[BaseEntity.RPC_Server]
	public void RPC_Lower(BaseEntity.RPCMessage msg)
	{
		this.LowerTarget();
	}

	// Token: 0x04000A29 RID: 2601
	public Animator myAnimator;

	// Token: 0x04000A2A RID: 2602
	public GameObjectRef bullseyeEffect;

	// Token: 0x04000A2B RID: 2603
	public GameObjectRef knockdownEffect;

	// Token: 0x04000A2C RID: 2604
	public float activationPowerTime = 0.5f;

	// Token: 0x04000A2D RID: 2605
	public int activationPowerAmount = 1;

	// Token: 0x04000A2E RID: 2606
	private float lastToggleTime = float.NegativeInfinity;

	// Token: 0x04000A2F RID: 2607
	private float knockdownHealth = 100f;
}
