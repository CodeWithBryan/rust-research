using System;

// Token: 0x020004A9 RID: 1193
public class SimpleLight : IOEntity
{
	// Token: 0x0600269D RID: 9885 RVA: 0x000EF83F File Offset: 0x000EDA3F
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x0600269E RID: 9886 RVA: 0x000EF70C File Offset: 0x000ED90C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		base.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
	}
}
