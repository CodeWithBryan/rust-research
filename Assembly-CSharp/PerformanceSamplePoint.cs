using System;

// Token: 0x020008DC RID: 2268
public struct PerformanceSamplePoint
{
	// Token: 0x0400315A RID: 12634
	public int UpdateCount;

	// Token: 0x0400315B RID: 12635
	public int FixedUpdateCount;

	// Token: 0x0400315C RID: 12636
	public int RenderCount;

	// Token: 0x0400315D RID: 12637
	public TimeSpan PreCull;

	// Token: 0x0400315E RID: 12638
	public TimeSpan Update;

	// Token: 0x0400315F RID: 12639
	public TimeSpan LateUpdate;

	// Token: 0x04003160 RID: 12640
	public TimeSpan Render;

	// Token: 0x04003161 RID: 12641
	public TimeSpan FixedUpdate;

	// Token: 0x04003162 RID: 12642
	public TimeSpan NetworkMessage;

	// Token: 0x04003163 RID: 12643
	public TimeSpan TotalCPU;

	// Token: 0x04003164 RID: 12644
	public int CpuUpdateCount;
}
