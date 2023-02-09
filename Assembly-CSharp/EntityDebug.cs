using System;
using System.Diagnostics;

// Token: 0x020003A3 RID: 931
public class EntityDebug : EntityComponent<BaseEntity>
{
	// Token: 0x06002045 RID: 8261 RVA: 0x000D2FB8 File Offset: 0x000D11B8
	private void Update()
	{
		if (!base.baseEntity.IsValid() || !base.baseEntity.IsDebugging())
		{
			base.enabled = false;
			return;
		}
		if (this.stopwatch.Elapsed.TotalSeconds < 0.5)
		{
			return;
		}
		bool isClient = base.baseEntity.isClient;
		if (base.baseEntity.isServer)
		{
			base.baseEntity.DebugServer(1, (float)this.stopwatch.Elapsed.TotalSeconds);
		}
		this.stopwatch.Reset();
		this.stopwatch.Start();
	}

	// Token: 0x04001921 RID: 6433
	internal Stopwatch stopwatch = Stopwatch.StartNew();
}
