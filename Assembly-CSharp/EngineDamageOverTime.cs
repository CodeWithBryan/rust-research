using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000486 RID: 1158
public class EngineDamageOverTime
{
	// Token: 0x060025B8 RID: 9656 RVA: 0x000EBF0C File Offset: 0x000EA10C
	public EngineDamageOverTime(float triggerDamage, float maxSeconds, Action trigger)
	{
		this.triggerDamage = triggerDamage;
		this.maxSeconds = maxSeconds;
		this.trigger = trigger;
	}

	// Token: 0x060025B9 RID: 9657 RVA: 0x000EBF34 File Offset: 0x000EA134
	public void TakeDamage(float amount)
	{
		this.recentDamage.Add(new EngineDamageOverTime.RecentDamage(Time.time, amount));
		if (this.GetRecentDamage() > this.triggerDamage)
		{
			this.trigger();
			this.recentDamage.Clear();
		}
	}

	// Token: 0x060025BA RID: 9658 RVA: 0x000EBF70 File Offset: 0x000EA170
	private float GetRecentDamage()
	{
		float num = 0f;
		int i;
		for (i = this.recentDamage.Count - 1; i >= 0; i--)
		{
			EngineDamageOverTime.RecentDamage recentDamage = this.recentDamage[i];
			if (Time.time > recentDamage.time + this.maxSeconds)
			{
				break;
			}
			num += recentDamage.amount;
		}
		if (i > 0)
		{
			this.recentDamage.RemoveRange(0, i + 1);
		}
		return num;
	}

	// Token: 0x04001E72 RID: 7794
	private readonly List<EngineDamageOverTime.RecentDamage> recentDamage = new List<EngineDamageOverTime.RecentDamage>();

	// Token: 0x04001E73 RID: 7795
	private readonly float maxSeconds;

	// Token: 0x04001E74 RID: 7796
	private readonly float triggerDamage;

	// Token: 0x04001E75 RID: 7797
	private readonly Action trigger;

	// Token: 0x02000CB8 RID: 3256
	private struct RecentDamage
	{
		// Token: 0x06004D59 RID: 19801 RVA: 0x00197C7A File Offset: 0x00195E7A
		public RecentDamage(float time, float amount)
		{
			this.time = time;
			this.amount = amount;
		}

		// Token: 0x04004393 RID: 17299
		public readonly float time;

		// Token: 0x04004394 RID: 17300
		public readonly float amount;
	}
}
