using System;
using UnityEngine;

// Token: 0x020004A5 RID: 1189
public class FluidSwitch : ElectricSwitch
{
	// Token: 0x0600268A RID: 9866 RVA: 0x000228A0 File Offset: 0x00020AA0
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x0600268B RID: 9867 RVA: 0x000EF624 File Offset: 0x000ED824
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && this.lastToggleInput != inputAmount)
		{
			this.lastToggleInput = inputAmount;
			this.SetSwitch(inputAmount > 0);
		}
		if (inputSlot == 2)
		{
			bool flag = this.pumpEnabled;
			this.pumpEnabled = (inputAmount > 0);
			if (flag != this.pumpEnabled)
			{
				this.lastPassthroughEnergy = -1;
				base.SetFlag(this.Flag_PumpPowered, this.pumpEnabled, false, true);
				this.SendChangedToRoot(true);
			}
		}
	}

	// Token: 0x0600268C RID: 9868 RVA: 0x000EF68D File Offset: 0x000ED88D
	public override void SetSwitch(bool wantsOn)
	{
		base.SetSwitch(wantsOn);
		base.Invoke(new Action(this.DelayedSendChanged), IOEntity.responsetime * 2f);
	}

	// Token: 0x0600268D RID: 9869 RVA: 0x000EF6B3 File Offset: 0x000ED8B3
	private void DelayedSendChanged()
	{
		this.SendChangedToRoot(true);
	}

	// Token: 0x0600268E RID: 9870 RVA: 0x000EF6BC File Offset: 0x000ED8BC
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x0600268F RID: 9871 RVA: 0x00007074 File Offset: 0x00005274
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x17000311 RID: 785
	// (get) Token: 0x06002690 RID: 9872 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsGravitySource
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000312 RID: 786
	// (get) Token: 0x06002691 RID: 9873 RVA: 0x000EF6D3 File Offset: 0x000ED8D3
	protected override bool DisregardGravityRestrictionsOnLiquid
	{
		get
		{
			return base.HasFlag(this.Flag_PumpPowered);
		}
	}

	// Token: 0x06002692 RID: 9874 RVA: 0x000EF6E1 File Offset: 0x000ED8E1
	public override bool AllowLiquidPassthrough(IOEntity fromSource, Vector3 sourceWorldPosition, bool forPlacement = false)
	{
		return (forPlacement || base.IsOn()) && base.AllowLiquidPassthrough(fromSource, sourceWorldPosition, false);
	}

	// Token: 0x04001F23 RID: 7971
	private BaseEntity.Flags Flag_PumpPowered = BaseEntity.Flags.Reserved6;

	// Token: 0x04001F24 RID: 7972
	public Animator PumpAnimator;

	// Token: 0x04001F25 RID: 7973
	private bool pumpEnabled;

	// Token: 0x04001F26 RID: 7974
	private int lastToggleInput;
}
