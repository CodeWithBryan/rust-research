using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000059 RID: 89
public class CompoundBowWeapon : BowWeapon
{
	// Token: 0x0600096B RID: 2411 RVA: 0x000580BC File Offset: 0x000562BC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CompoundBowWeapon.OnRpcMessage", 0))
		{
			if (rpc == 618693016U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StringHoldStatus ");
				}
				using (TimeWarning.New("RPC_StringHoldStatus", 0))
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
							this.RPC_StringHoldStatus(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_StringHoldStatus");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x000581E0 File Offset: 0x000563E0
	public void UpdateMovementPenalty(float delta)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		bool flag = false;
		if (base.isServer)
		{
			if (ownerPlayer == null)
			{
				return;
			}
			flag = (ownerPlayer.estimatedSpeed > 0.1f);
		}
		if (flag)
		{
			this.movementPenalty += delta * (1f / this.movementPenaltyRampUpTime);
		}
		else
		{
			this.movementPenalty -= delta * (1f / this.stringHoldDurationMax);
		}
		this.movementPenalty = Mathf.Clamp01(this.movementPenalty);
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x00058264 File Offset: 0x00056464
	public void UpdateConditionLoss()
	{
		if (this.stringHoldTimeStart != 0f && UnityEngine.Time.time - this.stringHoldTimeStart > this.conditionLossHeldDelay && this.GetStringBonusScale() > 0f)
		{
			Item ownerItem = base.GetOwnerItem();
			if (ownerItem == null)
			{
				return;
			}
			ownerItem.LoseCondition(this.conditionLossCheckTickRate * this.conditionLossPerSecondHeld);
		}
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x000582C2 File Offset: 0x000564C2
	public void ServerMovementCheck()
	{
		this.UpdateMovementPenalty(this.serverMovementCheckTickRate);
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x000582D0 File Offset: 0x000564D0
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			base.CancelInvoke(new Action(this.ServerMovementCheck));
			base.CancelInvoke(new Action(this.UpdateConditionLoss));
			return;
		}
		base.InvokeRepeating(new Action(this.ServerMovementCheck), 0f, this.serverMovementCheckTickRate);
		base.InvokeRepeating(new Action(this.UpdateConditionLoss), 0f, this.conditionLossCheckTickRate);
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x0005834A File Offset: 0x0005654A
	[BaseEntity.RPC_Server]
	public void RPC_StringHoldStatus(BaseEntity.RPCMessage msg)
	{
		if (msg.read.Bit())
		{
			this.stringHoldTimeStart = UnityEngine.Time.time;
			return;
		}
		this.stringHoldTimeStart = 0f;
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x00058370 File Offset: 0x00056570
	public override void DidAttackServerside()
	{
		base.DidAttackServerside();
		this.stringHoldTimeStart = 0f;
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x00058383 File Offset: 0x00056583
	public float GetLastPlayerMovementTime()
	{
		bool isServer = base.isServer;
		return 0f;
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x00058391 File Offset: 0x00056591
	public float GetStringBonusScale()
	{
		if (this.stringHoldTimeStart == 0f)
		{
			return 0f;
		}
		return Mathf.Clamp01(Mathf.Clamp01((UnityEngine.Time.time - this.stringHoldTimeStart) / this.stringHoldDurationMax) - this.movementPenalty);
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x000583CC File Offset: 0x000565CC
	public override float GetDamageScale(bool getMax = false)
	{
		float num = getMax ? 1f : this.GetStringBonusScale();
		return this.damageScale + this.stringBonusDamage * num;
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x000583FC File Offset: 0x000565FC
	public override float GetDistanceScale(bool getMax = false)
	{
		float num = getMax ? 1f : this.GetStringBonusScale();
		return this.distanceScale + this.stringBonusDistance * num;
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x0005842C File Offset: 0x0005662C
	public override float GetProjectileVelocityScale(bool getMax = false)
	{
		float num = getMax ? 1f : this.GetStringBonusScale();
		return this.projectileVelocityScale + this.stringBonusVelocity * num;
	}

	// Token: 0x0400063A RID: 1594
	public float stringHoldDurationMax = 3f;

	// Token: 0x0400063B RID: 1595
	public float stringBonusDamage = 1f;

	// Token: 0x0400063C RID: 1596
	public float stringBonusDistance = 0.5f;

	// Token: 0x0400063D RID: 1597
	public float stringBonusVelocity = 1f;

	// Token: 0x0400063E RID: 1598
	public float movementPenaltyRampUpTime = 0.5f;

	// Token: 0x0400063F RID: 1599
	public float conditionLossPerSecondHeld = 1f;

	// Token: 0x04000640 RID: 1600
	public float conditionLossHeldDelay = 3f;

	// Token: 0x04000641 RID: 1601
	public SoundDefinition chargeUpSoundDef;

	// Token: 0x04000642 RID: 1602
	public SoundDefinition stringHeldSoundDef;

	// Token: 0x04000643 RID: 1603
	public SoundDefinition drawFinishSoundDef;

	// Token: 0x04000644 RID: 1604
	private Sound chargeUpSound;

	// Token: 0x04000645 RID: 1605
	private Sound stringHeldSound;

	// Token: 0x04000646 RID: 1606
	protected float movementPenalty;

	// Token: 0x04000647 RID: 1607
	internal float stringHoldTimeStart;

	// Token: 0x04000648 RID: 1608
	protected float conditionLossCheckTickRate = 0.5f;

	// Token: 0x04000649 RID: 1609
	protected float serverMovementCheckTickRate = 0.1f;
}
