using System;

// Token: 0x020004DB RID: 1243
[Flags]
public enum EnvironmentType
{
	// Token: 0x04001FD6 RID: 8150
	Underground = 1,
	// Token: 0x04001FD7 RID: 8151
	Building = 2,
	// Token: 0x04001FD8 RID: 8152
	Outdoor = 4,
	// Token: 0x04001FD9 RID: 8153
	Elevator = 8,
	// Token: 0x04001FDA RID: 8154
	PlayerConstruction = 16,
	// Token: 0x04001FDB RID: 8155
	TrainTunnels = 32,
	// Token: 0x04001FDC RID: 8156
	UnderwaterLab = 64,
	// Token: 0x04001FDD RID: 8157
	Submarine = 128,
	// Token: 0x04001FDE RID: 8158
	BuildingDark = 256,
	// Token: 0x04001FDF RID: 8159
	BuildingVeryDark = 512
}
