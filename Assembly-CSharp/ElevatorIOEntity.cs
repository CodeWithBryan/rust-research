using System;

// Token: 0x020004A4 RID: 1188
public class ElevatorIOEntity : IOEntity
{
	// Token: 0x06002688 RID: 9864 RVA: 0x000EF60C File Offset: 0x000ED80C
	public override int ConsumptionAmount()
	{
		return this.Consumption;
	}

	// Token: 0x04001F22 RID: 7970
	public int Consumption = 5;
}
