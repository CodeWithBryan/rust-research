using System;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class ElectricalDFlipFlop : IOEntity
{
	// Token: 0x06001575 RID: 5493 RVA: 0x00082711 File Offset: 0x00080911
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x000A671C File Offset: 0x000A491C
	public bool GetDesiredState()
	{
		if (this.setAmount > 0 && this.resetAmount == 0)
		{
			return true;
		}
		if (this.setAmount > 0 && this.resetAmount > 0)
		{
			return true;
		}
		if (this.setAmount == 0 && this.resetAmount > 0)
		{
			return false;
		}
		if (this.toggleAmount > 0)
		{
			return !base.IsOn();
		}
		return this.setAmount == 0 && this.resetAmount == 0 && base.IsOn();
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x000A6790 File Offset: 0x000A4990
	public void UpdateState()
	{
		if (this.IsPowered())
		{
			bool flag = base.IsOn();
			bool desiredState = this.GetDesiredState();
			base.SetFlag(BaseEntity.Flags.On, desiredState, false, true);
			if (flag != base.IsOn())
			{
				base.MarkDirtyForceUpdateOutputs();
			}
		}
	}

	// Token: 0x06001578 RID: 5496 RVA: 0x000A67CC File Offset: 0x000A49CC
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1)
		{
			this.setAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 2)
		{
			this.resetAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 3)
		{
			this.toggleAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			this.UpdateState();
		}
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x000A6820 File Offset: 0x000A4A20
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x000A682C File Offset: 0x000A4A2C
	public override void UpdateOutputs()
	{
		if (!base.ShouldUpdateOutputs())
		{
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			int num = Mathf.Max(0, this.currentEnergy - 1);
			if (this.outputs[0].connectedTo.Get(true) != null)
			{
				this.outputs[0].connectedTo.Get(true).UpdateFromInput(base.IsOn() ? num : 0, this.outputs[0].connectedToSlot);
			}
			if (this.outputs[1].connectedTo.Get(true) != null)
			{
				this.outputs[1].connectedTo.Get(true).UpdateFromInput(base.IsOn() ? 0 : num, this.outputs[1].connectedToSlot);
			}
		}
	}

	// Token: 0x04000DD3 RID: 3539
	[NonSerialized]
	private int setAmount;

	// Token: 0x04000DD4 RID: 3540
	[NonSerialized]
	private int resetAmount;

	// Token: 0x04000DD5 RID: 3541
	[NonSerialized]
	private int toggleAmount;
}
