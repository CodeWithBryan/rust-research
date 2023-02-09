using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000920 RID: 2336
public struct Timing
{
	// Token: 0x060037A9 RID: 14249 RVA: 0x0014A197 File Offset: 0x00148397
	public static Timing Start(string name)
	{
		return new Timing(name);
	}

	// Token: 0x060037AA RID: 14250 RVA: 0x0014A1A0 File Offset: 0x001483A0
	public void End()
	{
		if (this.sw.Elapsed.TotalSeconds > 0.30000001192092896)
		{
			UnityEngine.Debug.Log("[" + this.sw.Elapsed.TotalSeconds.ToString("0.0") + "s] " + this.name);
		}
	}

	// Token: 0x060037AB RID: 14251 RVA: 0x0014A205 File Offset: 0x00148405
	public Timing(string name)
	{
		this.sw = Stopwatch.StartNew();
		this.name = name;
	}

	// Token: 0x040031E1 RID: 12769
	private Stopwatch sw;

	// Token: 0x040031E2 RID: 12770
	private string name;
}
