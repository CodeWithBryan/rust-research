using System;

// Token: 0x020004AA RID: 1194
public class Splitter : IOEntity
{
	// Token: 0x17000313 RID: 787
	// (get) Token: 0x060026A0 RID: 9888 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool BlockFluidDraining
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060026A1 RID: 9889 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x060026A2 RID: 9890 RVA: 0x000EF851 File Offset: 0x000EDA51
	public override void OnCircuitChanged(bool forceUpdate)
	{
		base.MarkDirtyForceUpdateOutputs();
	}
}
