using System;

// Token: 0x0200011A RID: 282
public class CableTunnel : IOEntity
{
	// Token: 0x060015A1 RID: 5537 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool WantsPower()
	{
		return true;
	}

	// Token: 0x060015A2 RID: 5538 RVA: 0x000A72F0 File Offset: 0x000A54F0
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		int num = this.inputAmounts[inputSlot];
		this.inputAmounts[inputSlot] = inputAmount;
		if (inputAmount != num)
		{
			this.ensureOutputsUpdated = true;
		}
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x000A7324 File Offset: 0x000A5524
	public override void UpdateOutputs()
	{
		if (!base.ShouldUpdateOutputs())
		{
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			for (int i = 0; i < 4; i++)
			{
				IOEntity.IOSlot ioslot = this.outputs[i];
				if (ioslot.connectedTo.Get(true) != null)
				{
					ioslot.connectedTo.Get(true).UpdateFromInput(this.inputAmounts[i], ioslot.connectedToSlot);
				}
			}
		}
	}

	// Token: 0x04000DFF RID: 3583
	private const int numChannels = 4;

	// Token: 0x04000E00 RID: 3584
	private int[] inputAmounts = new int[4];
}
