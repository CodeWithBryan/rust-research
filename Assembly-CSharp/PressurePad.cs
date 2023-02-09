using System;

// Token: 0x0200010C RID: 268
public class PressurePad : BaseDetector
{
	// Token: 0x06001558 RID: 5464 RVA: 0x00003A54 File Offset: 0x00001C54
	public override int ConsumptionAmount()
	{
		return 1;
	}

	// Token: 0x06001559 RID: 5465 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x0600155A RID: 5466 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ShouldTrigger()
	{
		return true;
	}

	// Token: 0x0600155B RID: 5467 RVA: 0x000A6295 File Offset: 0x000A4495
	public override void OnDetectorTriggered()
	{
		base.OnDetectorTriggered();
		base.Invoke(new Action(this.UnpowerTime), this.pressPowerTime);
		base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
	}

	// Token: 0x0600155C RID: 5468 RVA: 0x000A62C3 File Offset: 0x000A44C3
	public override void OnDetectorReleased()
	{
		base.OnDetectorReleased();
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x000833AA File Offset: 0x000815AA
	public void UnpowerTime()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x000A62D9 File Offset: 0x000A44D9
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			if (base.HasFlag(BaseEntity.Flags.Reserved3))
			{
				return this.pressPowerAmount;
			}
			if (this.IsPowered())
			{
				return base.GetPassthroughAmount(0);
			}
		}
		return 0;
	}

	// Token: 0x04000DC7 RID: 3527
	public float pressPowerTime = 0.5f;

	// Token: 0x04000DC8 RID: 3528
	public int pressPowerAmount = 2;

	// Token: 0x04000DC9 RID: 3529
	public const BaseEntity.Flags Flag_EmittingPower = BaseEntity.Flags.Reserved3;
}
