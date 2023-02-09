using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200090F RID: 2319
public class LocalClock
{
	// Token: 0x0600370C RID: 14092 RVA: 0x0014774C File Offset: 0x0014594C
	public void Add(float delta, float variance, Action action)
	{
		LocalClock.TimedEvent item = default(LocalClock.TimedEvent);
		item.time = Time.time + delta + UnityEngine.Random.Range(-variance, variance);
		item.delta = delta;
		item.variance = variance;
		item.action = action;
		this.events.Add(item);
	}

	// Token: 0x0600370D RID: 14093 RVA: 0x0014779C File Offset: 0x0014599C
	public void Tick()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			LocalClock.TimedEvent timedEvent = this.events[i];
			if (Time.time > timedEvent.time)
			{
				float delta = timedEvent.delta;
				float variance = timedEvent.variance;
				timedEvent.action();
				timedEvent.time = Time.time + delta + UnityEngine.Random.Range(-variance, variance);
				this.events[i] = timedEvent;
			}
		}
	}

	// Token: 0x040031A6 RID: 12710
	public List<LocalClock.TimedEvent> events = new List<LocalClock.TimedEvent>();

	// Token: 0x02000E60 RID: 3680
	public struct TimedEvent
	{
		// Token: 0x04004A37 RID: 18999
		public float time;

		// Token: 0x04004A38 RID: 19000
		public float delta;

		// Token: 0x04004A39 RID: 19001
		public float variance;

		// Token: 0x04004A3A RID: 19002
		public Action action;
	}
}
