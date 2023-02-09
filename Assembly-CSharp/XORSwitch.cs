using System;
using UnityEngine;

// Token: 0x020004AB RID: 1195
public class XORSwitch : IOEntity
{
	// Token: 0x060026A4 RID: 9892 RVA: 0x000EF859 File Offset: 0x000EDA59
	public override void ResetState()
	{
		base.ResetState();
		this.firstRun = true;
	}

	// Token: 0x060026A5 RID: 9893 RVA: 0x000EF868 File Offset: 0x000EDA68
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.input1Amount > 0 && this.input2Amount > 0)
		{
			return 0;
		}
		int num = Mathf.Max(this.input1Amount, this.input2Amount);
		return Mathf.Max(0, num - this.ConsumptionAmount());
	}

	// Token: 0x060026A6 RID: 9894 RVA: 0x000EF8A9 File Offset: 0x000EDAA9
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x060026A7 RID: 9895 RVA: 0x00053CF9 File Offset: 0x00051EF9
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x060026A8 RID: 9896 RVA: 0x000EF8D0 File Offset: 0x000EDAD0
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (inputAmount > 0 && base.IsConnectedTo(this, slot, IOEntity.backtracking, false))
		{
			inputAmount = 0;
			base.SetFlag(BaseEntity.Flags.Reserved7, true, false, true);
		}
		else
		{
			base.SetFlag(BaseEntity.Flags.Reserved7, false, false, true);
		}
		if (slot == 0)
		{
			this.input1Amount = inputAmount;
		}
		else if (slot == 1)
		{
			this.input2Amount = inputAmount;
		}
		if (this.firstRun)
		{
			if (!base.IsInvoking(new Action(this.UpdateFlags)))
			{
				base.Invoke(new Action(this.UpdateFlags), 0.1f);
			}
		}
		else
		{
			this.UpdateFlags();
		}
		this.firstRun = false;
		base.UpdateFromInput(inputAmount, slot);
	}

	// Token: 0x060026A9 RID: 9897 RVA: 0x000EF974 File Offset: 0x000EDB74
	private void UpdateFlags()
	{
		int num = (this.input1Amount > 0 && this.input2Amount > 0) ? 0 : Mathf.Max(this.input1Amount, this.input2Amount);
		bool b = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, b, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 || this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
	}

	// Token: 0x04001F29 RID: 7977
	private int input1Amount;

	// Token: 0x04001F2A RID: 7978
	private int input2Amount;

	// Token: 0x04001F2B RID: 7979
	private bool firstRun = true;
}
