using System;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class TeslaCoil : IOEntity
{
	// Token: 0x0600159C RID: 5532 RVA: 0x000A70BB File Offset: 0x000A52BB
	public override int ConsumptionAmount()
	{
		return Mathf.CeilToInt(this.maxDamageOutput / this.powerToDamageRatio);
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x000A70CF File Offset: 0x000A52CF
	public bool CanDischarge()
	{
		return base.healthFraction >= 0.25f;
	}

	// Token: 0x0600159E RID: 5534 RVA: 0x000A70E4 File Offset: 0x000A52E4
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		base.UpdateFromInput(inputAmount, inputSlot);
		if (inputAmount > 0 && this.CanDischarge())
		{
			float num = Time.time - this.lastDischargeTime;
			if (num < 0f)
			{
				num = 0f;
			}
			float time = Mathf.Min(this.dischargeTickRate - num, this.dischargeTickRate);
			base.InvokeRepeating(new Action(this.Discharge), time, this.dischargeTickRate);
			base.SetFlag(BaseEntity.Flags.Reserved1, inputAmount < this.powerForHeavyShorting, false, false);
			base.SetFlag(BaseEntity.Flags.Reserved2, inputAmount >= this.powerForHeavyShorting, false, false);
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			return;
		}
		base.CancelInvoke(new Action(this.Discharge));
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, false);
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x0600159F RID: 5535 RVA: 0x000A71C8 File Offset: 0x000A53C8
	public void Discharge()
	{
		float damageAmount = Mathf.Clamp((float)this.currentEnergy * this.powerToDamageRatio, 0f, this.maxDamageOutput) * this.dischargeTickRate;
		this.lastDischargeTime = Time.time;
		if (this.targetTrigger.entityContents != null)
		{
			BaseEntity[] array = this.targetTrigger.entityContents.ToArray<BaseEntity>();
			if (array != null)
			{
				BaseEntity[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					BaseCombatEntity component = array2[i].GetComponent<BaseCombatEntity>();
					if (component && component.IsVisible(this.damageEyes.transform.position, component.CenterPoint(), float.PositiveInfinity))
					{
						component.OnAttacked(new HitInfo(this, component, DamageType.ElectricShock, damageAmount));
					}
				}
			}
		}
		float amount = this.dischargeTickRate / this.maxDischargeSelfDamageSeconds * this.MaxHealth();
		base.Hurt(amount, DamageType.ElectricShock, this, false);
		if (!this.CanDischarge())
		{
			this.MarkDirty();
		}
	}

	// Token: 0x04000DF4 RID: 3572
	public TargetTrigger targetTrigger;

	// Token: 0x04000DF5 RID: 3573
	public TriggerMovement movementTrigger;

	// Token: 0x04000DF6 RID: 3574
	public float powerToDamageRatio = 2f;

	// Token: 0x04000DF7 RID: 3575
	public float dischargeTickRate = 0.25f;

	// Token: 0x04000DF8 RID: 3576
	public float maxDischargeSelfDamageSeconds = 120f;

	// Token: 0x04000DF9 RID: 3577
	public float maxDamageOutput = 35f;

	// Token: 0x04000DFA RID: 3578
	public Transform damageEyes;

	// Token: 0x04000DFB RID: 3579
	public const BaseEntity.Flags Flag_WeakShorting = BaseEntity.Flags.Reserved1;

	// Token: 0x04000DFC RID: 3580
	public const BaseEntity.Flags Flag_StrongShorting = BaseEntity.Flags.Reserved2;

	// Token: 0x04000DFD RID: 3581
	public int powerForHeavyShorting = 10;

	// Token: 0x04000DFE RID: 3582
	private float lastDischargeTime;
}
