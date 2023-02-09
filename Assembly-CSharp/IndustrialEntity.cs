using System;

// Token: 0x020004AE RID: 1198
public class IndustrialEntity : IOEntity
{
	// Token: 0x060026B1 RID: 9905 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void RunJob()
	{
	}

	// Token: 0x04001F30 RID: 7984
	public static IndustrialEntity.IndustrialProcessQueue Queue = new IndustrialEntity.IndustrialProcessQueue();

	// Token: 0x02000CC7 RID: 3271
	public class IndustrialProcessQueue : ObjectWorkQueue<IndustrialEntity>
	{
		// Token: 0x06004D67 RID: 19815 RVA: 0x00197CD4 File Offset: 0x00195ED4
		protected override void RunJob(IndustrialEntity job)
		{
			if (job != null)
			{
				job.RunJob();
			}
		}
	}
}
