using System;

// Token: 0x02000380 RID: 896
public class MicrophoneStandIOEntity : IOEntity, IAudioConnectionSource
{
	// Token: 0x06001F44 RID: 8004 RVA: 0x000CF5A8 File Offset: 0x000CD7A8
	public override int DesiredPower()
	{
		return this.PowerCost;
	}

	// Token: 0x06001F45 RID: 8005 RVA: 0x000CF5B0 File Offset: 0x000CD7B0
	public override int MaximalPowerOutput()
	{
		if (this.IsStatic)
		{
			return 100;
		}
		return base.MaximalPowerOutput();
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x000CF5C3 File Offset: 0x000CD7C3
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (this.IsStatic)
		{
			return 100;
		}
		return base.CalculateCurrentEnergy(inputAmount, inputSlot);
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x000CF5D8 File Offset: 0x000CD7D8
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.IsStatic)
		{
			return 100;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x000CF5EC File Offset: 0x000CD7EC
	public override bool IsRootEntity()
	{
		return this.IsStatic || base.IsRootEntity();
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x00002E37 File Offset: 0x00001037
	public IOEntity ToEntity()
	{
		return this;
	}

	// Token: 0x040018AF RID: 6319
	public int PowerCost = 5;

	// Token: 0x040018B0 RID: 6320
	public TriggerBase InstrumentTrigger;

	// Token: 0x040018B1 RID: 6321
	public bool IsStatic;
}
