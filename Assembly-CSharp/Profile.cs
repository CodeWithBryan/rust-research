using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200033C RID: 828
public class Profile
{
	// Token: 0x06001E1C RID: 7708 RVA: 0x000CC60B File Offset: 0x000CA80B
	public Profile(string cat, string nam, float WarnTime = 1f)
	{
		this.category = cat;
		this.name = nam;
		this.warnTime = WarnTime;
	}

	// Token: 0x06001E1D RID: 7709 RVA: 0x000CC633 File Offset: 0x000CA833
	public void Start()
	{
		this.watch.Reset();
		this.watch.Start();
	}

	// Token: 0x06001E1E RID: 7710 RVA: 0x000CC64C File Offset: 0x000CA84C
	public void Stop()
	{
		this.watch.Stop();
		if ((float)this.watch.Elapsed.Seconds > this.warnTime)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				this.category,
				".",
				this.name,
				": Took ",
				this.watch.Elapsed.Seconds,
				" seconds"
			}));
		}
	}

	// Token: 0x040017D8 RID: 6104
	public Stopwatch watch = new Stopwatch();

	// Token: 0x040017D9 RID: 6105
	public string category;

	// Token: 0x040017DA RID: 6106
	public string name;

	// Token: 0x040017DB RID: 6107
	public float warnTime;
}
