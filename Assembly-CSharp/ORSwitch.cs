using System;
using UnityEngine;

// Token: 0x020004A7 RID: 1191
public class ORSwitch : IOEntity
{
	// Token: 0x06002696 RID: 9878 RVA: 0x00006C79 File Offset: 0x00004E79
	public override bool WantsPassthroughPower()
	{
		return base.IsOn();
	}

	// Token: 0x06002697 RID: 9879 RVA: 0x000EF728 File Offset: 0x000ED928
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int num = Mathf.Max(this.input1Amount, this.input2Amount);
		return Mathf.Max(0, num - this.ConsumptionAmount());
	}

	// Token: 0x06002698 RID: 9880 RVA: 0x000EF755 File Offset: 0x000ED955
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x06002699 RID: 9881 RVA: 0x00053CF9 File Offset: 0x00051EF9
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x0600269A RID: 9882 RVA: 0x000EF77C File Offset: 0x000ED97C
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (base.IsConnectedTo(this, slot, IOEntity.backtracking, false))
		{
			inputAmount = 0;
		}
		if (slot == 0)
		{
			this.input1Amount = inputAmount;
		}
		else if (slot == 1)
		{
			this.input2Amount = inputAmount;
		}
		int num = this.input1Amount + this.input2Amount;
		bool b = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, b, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 || this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
		base.UpdateFromInput(inputAmount, slot);
	}

	// Token: 0x04001F27 RID: 7975
	private int input1Amount;

	// Token: 0x04001F28 RID: 7976
	private int input2Amount;
}
