using System;
using UnityEngine;

// Token: 0x020001FE RID: 510
[Serializable]
public struct StateTimer
{
	// Token: 0x06001A6D RID: 6765 RVA: 0x000BAE39 File Offset: 0x000B9039
	public void Activate(float seconds, Action onFinished = null)
	{
		this.ReleaseTime = Time.time + seconds;
		this.OnFinished = onFinished;
	}

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x06001A6E RID: 6766 RVA: 0x000BAE4F File Offset: 0x000B904F
	public bool IsActive
	{
		get
		{
			bool flag = this.ReleaseTime > Time.time;
			if (!flag && this.OnFinished != null)
			{
				this.OnFinished();
				this.OnFinished = null;
			}
			return flag;
		}
	}

	// Token: 0x0400129D RID: 4765
	public float ReleaseTime;

	// Token: 0x0400129E RID: 4766
	public Action OnFinished;
}
