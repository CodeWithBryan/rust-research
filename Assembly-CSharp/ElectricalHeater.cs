using System;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class ElectricalHeater : IOEntity
{
	// Token: 0x06001560 RID: 5472 RVA: 0x00002E0E File Offset: 0x0000100E
	public override int ConsumptionAmount()
	{
		return 3;
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x000228A0 File Offset: 0x00020AA0
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x000A6328 File Offset: 0x000A4528
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = next.HasFlag(BaseEntity.Flags.Reserved8);
		if (old.HasFlag(BaseEntity.Flags.Reserved8) != flag && this.growableHeatSource != null)
		{
			this.growableHeatSource.ForceUpdateGrowablesInRange();
		}
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x000A6384 File Offset: 0x000A4584
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if (this.growableHeatSource != null)
		{
			this.growableHeatSource.ForceUpdateGrowablesInRange();
		}
	}

	// Token: 0x04000DCA RID: 3530
	public float fadeDuration = 1f;

	// Token: 0x04000DCB RID: 3531
	public Light sourceLight;

	// Token: 0x04000DCC RID: 3532
	public GrowableHeatSource growableHeatSource;
}
