using System;

// Token: 0x02000109 RID: 265
public class BaseDetector : IOEntity
{
	// Token: 0x0600154A RID: 5450 RVA: 0x000A6128 File Offset: 0x000A4328
	public override int ConsumptionAmount()
	{
		return base.ConsumptionAmount();
	}

	// Token: 0x0600154B RID: 5451 RVA: 0x000A6130 File Offset: 0x000A4330
	public virtual bool ShouldTrigger()
	{
		return this.IsPowered();
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x000A6138 File Offset: 0x000A4338
	public virtual void OnObjects()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		if (this.ShouldTrigger())
		{
			this.OnDetectorTriggered();
			this.MarkDirty();
		}
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x000A615C File Offset: 0x000A435C
	public virtual void OnEmpty()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		if (this.ShouldTrigger())
		{
			this.OnDetectorReleased();
			this.MarkDirty();
		}
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnDetectorTriggered()
	{
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnDetectorReleased()
	{
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x000A6180 File Offset: 0x000A4380
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			return 0;
		}
		return base.GetPassthroughAmount(0);
	}

	// Token: 0x04000DC4 RID: 3524
	public PlayerDetectionTrigger myTrigger;

	// Token: 0x04000DC5 RID: 3525
	public const BaseEntity.Flags Flag_HasContents = BaseEntity.Flags.Reserved1;
}
