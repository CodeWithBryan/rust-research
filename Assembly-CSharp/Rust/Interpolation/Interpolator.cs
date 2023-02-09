using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Interpolation
{
	// Token: 0x02000AF0 RID: 2800
	public class Interpolator<T> where T : ISnapshot<T>, new()
	{
		// Token: 0x06004344 RID: 17220 RVA: 0x0018699B File Offset: 0x00184B9B
		public Interpolator(int listCount)
		{
			this.list = new List<T>(listCount);
		}

		// Token: 0x06004345 RID: 17221 RVA: 0x001869AF File Offset: 0x00184BAF
		public void Add(T tick)
		{
			this.last = tick;
			this.list.Add(tick);
		}

		// Token: 0x06004346 RID: 17222 RVA: 0x001869C4 File Offset: 0x00184BC4
		public void Cull(float beforeTime)
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				T t = this.list[i];
				if (t.Time < beforeTime)
				{
					this.list.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06004347 RID: 17223 RVA: 0x00186A14 File Offset: 0x00184C14
		public void Clear()
		{
			this.list.Clear();
		}

		// Token: 0x06004348 RID: 17224 RVA: 0x00186A24 File Offset: 0x00184C24
		public Interpolator<T>.Segment Query(float time, float interpolation, float extrapolation, float smoothing, ref T t)
		{
			Interpolator<T>.Segment result = default(Interpolator<T>.Segment);
			if (this.list.Count == 0)
			{
				result.prev = this.last;
				result.next = this.last;
				result.tick = this.last;
				return result;
			}
			float num = time - interpolation - smoothing * 0.5f;
			float num2 = Mathf.Min(time - interpolation, this.last.Time);
			float num3 = num2 - smoothing;
			T prev = this.list[0];
			T t2 = this.last;
			T prev2 = this.list[0];
			T t3 = this.last;
			foreach (T t4 in this.list)
			{
				if (t4.Time < num3)
				{
					prev = t4;
				}
				else if (t2.Time >= t4.Time)
				{
					t2 = t4;
				}
				if (t4.Time < num2)
				{
					prev2 = t4;
				}
				else if (t3.Time >= t4.Time)
				{
					t3 = t4;
				}
			}
			T @new = t.GetNew();
			if (t2.Time - prev.Time <= Mathf.Epsilon)
			{
				@new.Time = num3;
				@new.MatchValuesTo(t2);
			}
			else
			{
				@new.Time = num3;
				@new.Lerp(prev, t2, (num3 - prev.Time) / (t2.Time - prev.Time));
			}
			result.prev = @new;
			T new2 = t.GetNew();
			if (t3.Time - prev2.Time <= Mathf.Epsilon)
			{
				new2.Time = num2;
				new2.MatchValuesTo(t3);
			}
			else
			{
				new2.Time = num2;
				new2.Lerp(prev2, t3, (num2 - prev2.Time) / (t3.Time - prev2.Time));
			}
			result.next = new2;
			if (new2.Time - @new.Time <= Mathf.Epsilon)
			{
				result.prev = new2;
				result.tick = new2;
				return result;
			}
			if (num - new2.Time > extrapolation)
			{
				result.prev = new2;
				result.tick = new2;
				return result;
			}
			T new3 = t.GetNew();
			new3.Time = num;
			new3.Lerp(@new, new2, Mathf.Min(num - @new.Time, new2.Time + extrapolation - @new.Time) / (new2.Time - @new.Time));
			result.tick = new3;
			return result;
		}

		// Token: 0x04003BD7 RID: 15319
		public List<T> list;

		// Token: 0x04003BD8 RID: 15320
		public T last;

		// Token: 0x02000F31 RID: 3889
		public struct Segment
		{
			// Token: 0x04004D9C RID: 19868
			public T tick;

			// Token: 0x04004D9D RID: 19869
			public T prev;

			// Token: 0x04004D9E RID: 19870
			public T next;
		}
	}
}
