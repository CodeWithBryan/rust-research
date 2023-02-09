using System;
using UnityEngine;

// Token: 0x02000900 RID: 2304
public class FixedRateStepped
{
	// Token: 0x060036DA RID: 14042 RVA: 0x00146520 File Offset: 0x00144720
	public bool ShouldStep()
	{
		if (this.nextCall > Time.time)
		{
			return false;
		}
		if (this.nextCall == 0f)
		{
			this.nextCall = Time.time;
		}
		if (this.nextCall + this.rate * (float)this.maxSteps < Time.time)
		{
			this.nextCall = Time.time - this.rate * (float)this.maxSteps;
		}
		this.nextCall += this.rate;
		return true;
	}

	// Token: 0x04003198 RID: 12696
	public float rate = 0.1f;

	// Token: 0x04003199 RID: 12697
	public int maxSteps = 3;

	// Token: 0x0400319A RID: 12698
	internal float nextCall;
}
