using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class Igniter : IOEntity
{
	// Token: 0x06001589 RID: 5513 RVA: 0x000A6AF6 File Offset: 0x000A4CF6
	public override int ConsumptionAmount()
	{
		return this.PowerConsumption;
	}

	// Token: 0x0600158A RID: 5514 RVA: 0x000A6B00 File Offset: 0x000A4D00
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputAmount > 0)
		{
			base.InvokeRepeating(new Action(this.IgniteInRange), this.IgniteStartDelay, this.IgniteFrequency);
			return;
		}
		if (base.IsInvoking(new Action(this.IgniteInRange)))
		{
			base.CancelInvoke(new Action(this.IgniteInRange));
		}
	}

	// Token: 0x0600158B RID: 5515 RVA: 0x000A6B60 File Offset: 0x000A4D60
	private void IgniteInRange()
	{
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(this.LineOfSightEyes.position, this.IgniteRange, list, 1236495121, QueryTriggerInteraction.Collide);
		int num = 0;
		foreach (BaseEntity baseEntity in list)
		{
			if (!baseEntity.HasFlag(BaseEntity.Flags.On) && baseEntity.IsVisible(this.LineOfSightEyes.position, float.PositiveInfinity))
			{
				IIgniteable igniteable;
				if (baseEntity.isServer && baseEntity is BaseOven)
				{
					(baseEntity as BaseOven).StartCooking();
					if (baseEntity.HasFlag(BaseEntity.Flags.On))
					{
						num++;
					}
				}
				else if (baseEntity.isServer && (igniteable = (baseEntity as IIgniteable)) != null && igniteable.CanIgnite())
				{
					igniteable.Ignite(base.transform.position);
					num++;
				}
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		base.Hurt(this.SelfDamagePerIgnite, DamageType.ElectricShock, this, false);
	}

	// Token: 0x04000DDD RID: 3549
	public float IgniteRange = 5f;

	// Token: 0x04000DDE RID: 3550
	public float IgniteFrequency = 1f;

	// Token: 0x04000DDF RID: 3551
	public float IgniteStartDelay;

	// Token: 0x04000DE0 RID: 3552
	public Transform LineOfSightEyes;

	// Token: 0x04000DE1 RID: 3553
	public float SelfDamagePerIgnite = 0.5f;

	// Token: 0x04000DE2 RID: 3554
	public int PowerConsumption = 2;
}
