using System;

// Token: 0x020004A6 RID: 1190
public class FuseBox : IOEntity
{
	// Token: 0x06002694 RID: 9876 RVA: 0x000EF70C File Offset: 0x000ED90C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		base.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
	}
}
