using System;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class ElectricalCombiner : IOEntity
{
	// Token: 0x0600156F RID: 5487 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x06001570 RID: 5488 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool BlockFluidDraining
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x000A65CC File Offset: 0x000A47CC
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int num = this.input1Amount + this.input2Amount + this.input3Amount;
		Mathf.Clamp(num - 1, 0, num);
		return num;
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x000A65FA File Offset: 0x000A47FA
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x000A6620 File Offset: 0x000A4820
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (inputAmount > 0 && base.IsConnectedTo(this, slot, IOEntity.backtracking * 2, true))
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
		else if (slot == 2)
		{
			this.input3Amount = inputAmount;
		}
		int num = this.input1Amount + this.input2Amount + this.input3Amount;
		bool b = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, b, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 || this.input2Amount > 0 || this.input3Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
		base.UpdateFromInput(num, slot);
	}

	// Token: 0x04000DD0 RID: 3536
	public int input1Amount;

	// Token: 0x04000DD1 RID: 3537
	public int input2Amount;

	// Token: 0x04000DD2 RID: 3538
	public int input3Amount;
}
