using System;

// Token: 0x020005C3 RID: 1475
public interface IAirSupply
{
	// Token: 0x17000368 RID: 872
	// (get) Token: 0x06002BC6 RID: 11206
	ItemModGiveOxygen.AirSupplyType AirType { get; }

	// Token: 0x06002BC7 RID: 11207
	float GetAirTimeRemaining();
}
