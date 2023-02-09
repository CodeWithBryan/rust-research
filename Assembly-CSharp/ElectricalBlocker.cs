using System;

// Token: 0x0200010F RID: 271
public class ElectricalBlocker : IOEntity
{
	// Token: 0x06001568 RID: 5480 RVA: 0x000A6507 File Offset: 0x000A4707
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return base.GetPassthroughAmount(outputSlot) * (base.IsOn() ? 0 : 1);
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x0004D07C File Offset: 0x0004B27C
	public override bool WantsPower()
	{
		return !base.IsOn();
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x000A651D File Offset: 0x000A471D
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x00053CF9 File Offset: 0x00051EF9
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x000A654C File Offset: 0x000A474C
	public virtual void UpdateBlocked()
	{
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved8, base.IsOn(), false, false);
		this.UpdateHasPower(this.input1Amount + this.input2Amount, 1);
		if (flag != base.IsOn())
		{
			this.MarkDirty();
		}
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x000A65A6 File Offset: 0x000A47A6
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1)
		{
			this.input1Amount = inputAmount;
			this.UpdateBlocked();
			return;
		}
		if (inputSlot == 0)
		{
			this.input2Amount = inputAmount;
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}

	// Token: 0x04000DCE RID: 3534
	protected int input1Amount;

	// Token: 0x04000DCF RID: 3535
	protected int input2Amount;
}
