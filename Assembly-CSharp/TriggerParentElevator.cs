using System;

// Token: 0x02000565 RID: 1381
public class TriggerParentElevator : TriggerParentEnclosed
{
	// Token: 0x060029E6 RID: 10726 RVA: 0x000FDE3E File Offset: 0x000FC03E
	protected override bool IsClipping(BaseEntity ent)
	{
		return (!this.AllowHorsesToBypassClippingChecks || !(ent is BaseRidableAnimal)) && base.IsClipping(ent);
	}

	// Token: 0x040021E5 RID: 8677
	public bool AllowHorsesToBypassClippingChecks = true;
}
