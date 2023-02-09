using System;
using UnityEngine;

// Token: 0x0200049D RID: 1181
public class ANDSwitch : IOEntity
{
	// Token: 0x0600264B RID: 9803 RVA: 0x000EEA29 File Offset: 0x000ECC29
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.input1Amount <= 0 || this.input2Amount <= 0)
		{
			return 0;
		}
		return Mathf.Max(this.input1Amount, this.input2Amount);
	}

	// Token: 0x0600264C RID: 9804 RVA: 0x000EEA50 File Offset: 0x000ECC50
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x0600264D RID: 9805 RVA: 0x000EEA74 File Offset: 0x000ECC74
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (slot == 0)
		{
			this.input1Amount = inputAmount;
		}
		else if (slot == 1)
		{
			this.input2Amount = inputAmount;
		}
		int num = (this.input1Amount > 0 && this.input2Amount > 0) ? (this.input1Amount + this.input2Amount) : 0;
		bool b = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, b, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 && this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
		base.UpdateFromInput(inputAmount, slot);
	}

	// Token: 0x04001F14 RID: 7956
	private int input1Amount;

	// Token: 0x04001F15 RID: 7957
	private int input2Amount;
}
