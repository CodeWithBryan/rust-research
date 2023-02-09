using System;

// Token: 0x02000464 RID: 1124
public class MLRSServerProjectile : ServerProjectile
{
	// Token: 0x170002CA RID: 714
	// (get) Token: 0x060024CE RID: 9422 RVA: 0x00007074 File Offset: 0x00005274
	public override bool HasRangeLimit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x060024CF RID: 9423 RVA: 0x000E81AB File Offset: 0x000E63AB
	protected override int mask
	{
		get
		{
			return 1235430161;
		}
	}

	// Token: 0x060024D0 RID: 9424 RVA: 0x000E81B2 File Offset: 0x000E63B2
	protected override bool IsAValidHit(BaseEntity hitEnt)
	{
		return base.IsAValidHit(hitEnt) && (!hitEnt.IsValid() || !(hitEnt is MLRS));
	}
}
