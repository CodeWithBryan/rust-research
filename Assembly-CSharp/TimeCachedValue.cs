using System;
using UnityEngine;

// Token: 0x0200091F RID: 2335
public class TimeCachedValue<T>
{
	// Token: 0x060037A6 RID: 14246 RVA: 0x0014A0F8 File Offset: 0x001482F8
	public T Get(bool force)
	{
		if (this.cooldown < this.refreshCooldown && !force && this.hasRun && !this.forceNextRun)
		{
			return this.cachedValue;
		}
		this.hasRun = true;
		this.forceNextRun = false;
		this.cooldown = 0f - UnityEngine.Random.Range(0f, this.refreshRandomRange);
		if (this.updateValue != null)
		{
			this.cachedValue = this.updateValue();
		}
		else
		{
			this.cachedValue = default(T);
		}
		return this.cachedValue;
	}

	// Token: 0x060037A7 RID: 14247 RVA: 0x0014A18E File Offset: 0x0014838E
	public void ForceNextRun()
	{
		this.forceNextRun = true;
	}

	// Token: 0x040031DA RID: 12762
	public float refreshCooldown;

	// Token: 0x040031DB RID: 12763
	public float refreshRandomRange;

	// Token: 0x040031DC RID: 12764
	public Func<T> updateValue;

	// Token: 0x040031DD RID: 12765
	private T cachedValue;

	// Token: 0x040031DE RID: 12766
	private TimeSince cooldown;

	// Token: 0x040031DF RID: 12767
	private bool hasRun;

	// Token: 0x040031E0 RID: 12768
	private bool forceNextRun;
}
