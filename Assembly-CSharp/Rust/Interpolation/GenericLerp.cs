using System;
using UnityEngine;

namespace Rust.Interpolation
{
	// Token: 0x02000AEE RID: 2798
	public class GenericLerp<T> : IDisposable where T : ISnapshot<T>, new()
	{
		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x06004334 RID: 17204 RVA: 0x001865C5 File Offset: 0x001847C5
		private int TimeOffsetInterval
		{
			get
			{
				return PositionLerp.TimeOffsetInterval;
			}
		}

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x06004335 RID: 17205 RVA: 0x001865CC File Offset: 0x001847CC
		private float LerpTime
		{
			get
			{
				return PositionLerp.LerpTime;
			}
		}

		// Token: 0x06004336 RID: 17206 RVA: 0x001865D4 File Offset: 0x001847D4
		public GenericLerp(IGenericLerpTarget<T> target, int listCount)
		{
			this.target = target;
			this.interpolator = new Interpolator<T>(listCount);
		}

		// Token: 0x06004337 RID: 17207 RVA: 0x00186628 File Offset: 0x00184828
		public void Tick()
		{
			if (this.target == null)
			{
				return;
			}
			float extrapolationTime = this.target.GetExtrapolationTime();
			float interpolationDelay = this.target.GetInterpolationDelay();
			float interpolationSmoothing = this.target.GetInterpolationSmoothing();
			Interpolator<T>.Segment segment = this.interpolator.Query(this.LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing, ref GenericLerp<T>.snapshotPrototype);
			if (segment.next.Time >= this.interpolator.last.Time)
			{
				this.extrapolatedTime = Mathf.Min(this.extrapolatedTime + Time.deltaTime, extrapolationTime);
			}
			else
			{
				this.extrapolatedTime = Mathf.Max(this.extrapolatedTime - Time.deltaTime, 0f);
			}
			if (this.extrapolatedTime > 0f && extrapolationTime > 0f && interpolationSmoothing > 0f)
			{
				float delta = Time.deltaTime / (this.extrapolatedTime / extrapolationTime * interpolationSmoothing);
				segment.tick.Lerp(this.target.GetCurrentState(), segment.tick, delta);
			}
			this.target.SetFrom(segment.tick);
		}

		// Token: 0x06004338 RID: 17208 RVA: 0x00186744 File Offset: 0x00184944
		public void Snapshot(T snapshot)
		{
			float interpolationDelay = this.target.GetInterpolationDelay();
			float interpolationSmoothing = this.target.GetInterpolationSmoothing();
			float num = interpolationDelay + interpolationSmoothing + 1f;
			float num2 = this.LerpTime;
			this.timeOffset0 = Mathf.Min(this.timeOffset0, num2 - snapshot.Time);
			this.timeOffsetCount++;
			if (this.timeOffsetCount >= this.TimeOffsetInterval / 4)
			{
				this.timeOffset3 = this.timeOffset2;
				this.timeOffset2 = this.timeOffset1;
				this.timeOffset1 = this.timeOffset0;
				this.timeOffset0 = float.MaxValue;
				this.timeOffsetCount = 0;
			}
			GenericLerp<T>.TimeOffset = Mathx.Min(this.timeOffset0, this.timeOffset1, this.timeOffset2, this.timeOffset3);
			num2 = snapshot.Time + GenericLerp<T>.TimeOffset;
			snapshot.Time = num2;
			this.interpolator.Add(snapshot);
			this.interpolator.Cull(num2 - num);
		}

		// Token: 0x06004339 RID: 17209 RVA: 0x0018684A File Offset: 0x00184A4A
		public void SnapTo(T snapshot)
		{
			this.interpolator.Clear();
			this.Snapshot(snapshot);
			this.target.SetFrom(snapshot);
		}

		// Token: 0x0600433A RID: 17210 RVA: 0x0018686A File Offset: 0x00184A6A
		public void SnapToNow(T snapshot)
		{
			snapshot.Time = this.LerpTime;
			this.interpolator.last = snapshot;
			this.Wipe();
		}

		// Token: 0x0600433B RID: 17211 RVA: 0x00186894 File Offset: 0x00184A94
		public void SnapToEnd()
		{
			float interpolationDelay = this.target.GetInterpolationDelay();
			Interpolator<T>.Segment segment = this.interpolator.Query(this.LerpTime, interpolationDelay, 0f, 0f, ref GenericLerp<T>.snapshotPrototype);
			this.target.SetFrom(segment.tick);
			this.Wipe();
		}

		// Token: 0x0600433C RID: 17212 RVA: 0x001868E8 File Offset: 0x00184AE8
		public void Dispose()
		{
			this.target = null;
			this.interpolator.Clear();
			this.timeOffset0 = float.MaxValue;
			this.timeOffset1 = float.MaxValue;
			this.timeOffset2 = float.MaxValue;
			this.timeOffset3 = float.MaxValue;
			this.extrapolatedTime = 0f;
			this.timeOffsetCount = 0;
		}

		// Token: 0x0600433D RID: 17213 RVA: 0x00186945 File Offset: 0x00184B45
		private void Wipe()
		{
			this.interpolator.Clear();
			this.timeOffsetCount = 0;
			this.timeOffset0 = float.MaxValue;
			this.timeOffset1 = float.MaxValue;
			this.timeOffset2 = float.MaxValue;
			this.timeOffset3 = float.MaxValue;
		}

		// Token: 0x04003BCD RID: 15309
		private Interpolator<T> interpolator;

		// Token: 0x04003BCE RID: 15310
		private IGenericLerpTarget<T> target;

		// Token: 0x04003BCF RID: 15311
		private static T snapshotPrototype = Activator.CreateInstance<T>();

		// Token: 0x04003BD0 RID: 15312
		private static float TimeOffset = 0f;

		// Token: 0x04003BD1 RID: 15313
		private float timeOffset0 = float.MaxValue;

		// Token: 0x04003BD2 RID: 15314
		private float timeOffset1 = float.MaxValue;

		// Token: 0x04003BD3 RID: 15315
		private float timeOffset2 = float.MaxValue;

		// Token: 0x04003BD4 RID: 15316
		private float timeOffset3 = float.MaxValue;

		// Token: 0x04003BD5 RID: 15317
		private int timeOffsetCount;

		// Token: 0x04003BD6 RID: 15318
		private float extrapolatedTime;
	}
}
