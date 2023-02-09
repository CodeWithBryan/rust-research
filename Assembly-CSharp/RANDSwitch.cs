using System;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class RANDSwitch : ElectricalBlocker
{
	// Token: 0x0600157C RID: 5500 RVA: 0x000A68F4 File Offset: 0x000A4AF4
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return base.GetPassthroughAmount(outputSlot) * (base.IsOn() ? 0 : 1);
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x000A690C File Offset: 0x000A4B0C
	public override void UpdateBlocked()
	{
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.rand, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved8, this.rand, false, false);
		this.UpdateHasPower(this.input1Amount + this.input2Amount, 1);
		if (flag != base.IsOn())
		{
			this.MarkDirty();
		}
	}

	// Token: 0x0600157E RID: 5502 RVA: 0x000A6963 File Offset: 0x000A4B63
	public bool RandomRoll()
	{
		return UnityEngine.Random.Range(0, 2) == 1;
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x000A6970 File Offset: 0x000A4B70
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && inputAmount > 0)
		{
			this.input1Amount = inputAmount;
			this.rand = this.RandomRoll();
			this.UpdateBlocked();
		}
		if (inputSlot == 2)
		{
			if (inputAmount > 0)
			{
				this.rand = false;
				this.UpdateBlocked();
				return;
			}
		}
		else
		{
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}

	// Token: 0x04000DD6 RID: 3542
	private bool rand;
}
