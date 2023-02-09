using System;

namespace VLB
{
	// Token: 0x02000980 RID: 2432
	public enum RenderQueue
	{
		// Token: 0x04003431 RID: 13361
		Custom,
		// Token: 0x04003432 RID: 13362
		Background = 1000,
		// Token: 0x04003433 RID: 13363
		Geometry = 2000,
		// Token: 0x04003434 RID: 13364
		AlphaTest = 2450,
		// Token: 0x04003435 RID: 13365
		GeometryLast = 2500,
		// Token: 0x04003436 RID: 13366
		Transparent = 3000,
		// Token: 0x04003437 RID: 13367
		Overlay = 4000
	}
}
