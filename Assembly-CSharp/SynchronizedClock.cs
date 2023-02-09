using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200091C RID: 2332
public class SynchronizedClock
{
	// Token: 0x17000435 RID: 1077
	// (get) Token: 0x06003794 RID: 14228 RVA: 0x000DC6EF File Offset: 0x000DA8EF
	private static float CurrentTime
	{
		get
		{
			return Time.realtimeSinceStartup;
		}
	}

	// Token: 0x06003795 RID: 14229 RVA: 0x00149C70 File Offset: 0x00147E70
	public void Add(float delta, float variance, Action<uint> action)
	{
		SynchronizedClock.TimedEvent item = default(SynchronizedClock.TimedEvent);
		item.time = SynchronizedClock.CurrentTime;
		item.delta = delta;
		item.variance = variance;
		item.action = action;
		this.events.Add(item);
	}

	// Token: 0x06003796 RID: 14230 RVA: 0x00149CB8 File Offset: 0x00147EB8
	public void Tick()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			SynchronizedClock.TimedEvent timedEvent = this.events[i];
			float time = timedEvent.time;
			float currentTime = SynchronizedClock.CurrentTime;
			float delta = timedEvent.delta;
			float num = time - time % delta;
			uint obj = (uint)(time / delta);
			SeedRandom.Wanghash(ref obj);
			SeedRandom.Wanghash(ref obj);
			SeedRandom.Wanghash(ref obj);
			float num2 = SeedRandom.Range(ref obj, -timedEvent.variance, timedEvent.variance);
			float num3 = num + delta + num2;
			if (time < num3 && currentTime >= num3)
			{
				timedEvent.action(obj);
				timedEvent.time = currentTime;
			}
			else if (currentTime > time || currentTime < num - 5f)
			{
				timedEvent.time = currentTime;
			}
			this.events[i] = timedEvent;
		}
	}

	// Token: 0x040031D2 RID: 12754
	public List<SynchronizedClock.TimedEvent> events = new List<SynchronizedClock.TimedEvent>();

	// Token: 0x02000E63 RID: 3683
	public struct TimedEvent
	{
		// Token: 0x04004A42 RID: 19010
		public float time;

		// Token: 0x04004A43 RID: 19011
		public float delta;

		// Token: 0x04004A44 RID: 19012
		public float variance;

		// Token: 0x04004A45 RID: 19013
		public Action<uint> action;
	}
}
