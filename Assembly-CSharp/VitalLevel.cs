using System;
using UnityEngine;

// Token: 0x020001FF RID: 511
[Serializable]
public struct VitalLevel
{
	// Token: 0x06001A6F RID: 6767 RVA: 0x000BAE7B File Offset: 0x000B907B
	internal void Add(float f)
	{
		this.Level += f;
		if (this.Level > 1f)
		{
			this.Level = 1f;
		}
		if (this.Level < 0f)
		{
			this.Level = 0f;
		}
	}

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x06001A70 RID: 6768 RVA: 0x000BAEBB File Offset: 0x000B90BB
	public float TimeSinceUsed
	{
		get
		{
			return Time.time - this.lastUsedTime;
		}
	}

	// Token: 0x06001A71 RID: 6769 RVA: 0x000BAECC File Offset: 0x000B90CC
	internal void Use(float f)
	{
		if (Mathf.Approximately(f, 0f))
		{
			return;
		}
		this.Level -= Mathf.Abs(f);
		if (this.Level < 0f)
		{
			this.Level = 0f;
		}
		this.lastUsedTime = Time.time;
	}

	// Token: 0x0400129F RID: 4767
	public float Level;

	// Token: 0x040012A0 RID: 4768
	private float lastUsedTime;
}
