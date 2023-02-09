using System;
using Rust.Interpolation;
using UnityEngine;

// Token: 0x020002C0 RID: 704
public class PositionLerp : IDisposable
{
	// Token: 0x17000224 RID: 548
	// (get) Token: 0x06001C8E RID: 7310 RVA: 0x000C40BD File Offset: 0x000C22BD
	// (set) Token: 0x06001C8F RID: 7311 RVA: 0x000C40C5 File Offset: 0x000C22C5
	public bool Enabled
	{
		get
		{
			return this.enabled;
		}
		set
		{
			this.enabled = value;
			if (this.enabled)
			{
				this.OnEnable();
				return;
			}
			this.OnDisable();
		}
	}

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x06001C90 RID: 7312 RVA: 0x000C40E3 File Offset: 0x000C22E3
	public static float LerpTime
	{
		get
		{
			return Time.time;
		}
	}

	// Token: 0x06001C91 RID: 7313 RVA: 0x000C40EA File Offset: 0x000C22EA
	private void OnEnable()
	{
		PositionLerp.InstanceList.Add(this);
		this.enabledTime = PositionLerp.LerpTime;
	}

	// Token: 0x06001C92 RID: 7314 RVA: 0x000C4102 File Offset: 0x000C2302
	private void OnDisable()
	{
		PositionLerp.InstanceList.Remove(this);
	}

	// Token: 0x06001C93 RID: 7315 RVA: 0x000C4110 File Offset: 0x000C2310
	public void Initialize(IPosLerpTarget target)
	{
		this.target = target;
		this.Enabled = true;
	}

	// Token: 0x06001C94 RID: 7316 RVA: 0x000C4120 File Offset: 0x000C2320
	public void Snapshot(Vector3 position, Quaternion rotation, float serverTime)
	{
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		float num = interpolationDelay + interpolationSmoothing + 1f;
		float num2 = PositionLerp.LerpTime;
		this.timeOffset0 = Mathf.Min(this.timeOffset0, num2 - serverTime);
		this.timeOffsetCount++;
		if (this.timeOffsetCount >= PositionLerp.TimeOffsetInterval / 4)
		{
			this.timeOffset3 = this.timeOffset2;
			this.timeOffset2 = this.timeOffset1;
			this.timeOffset1 = this.timeOffset0;
			this.timeOffset0 = float.MaxValue;
			this.timeOffsetCount = 0;
		}
		PositionLerp.TimeOffset = Mathx.Min(this.timeOffset0, this.timeOffset1, this.timeOffset2, this.timeOffset3);
		num2 = serverTime + PositionLerp.TimeOffset;
		if (PositionLerp.DebugLog && this.interpolator.list.Count > 0 && serverTime < this.lastServerTime)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				this.target.ToString(),
				" adding tick from the past: server time ",
				serverTime,
				" < ",
				this.lastServerTime
			}));
		}
		else if (PositionLerp.DebugLog && this.interpolator.list.Count > 0 && num2 < this.lastClientTime)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				this.target.ToString(),
				" adding tick from the past: client time ",
				num2,
				" < ",
				this.lastClientTime
			}));
		}
		else
		{
			this.lastClientTime = num2;
			this.lastServerTime = serverTime;
			this.interpolator.Add(new TransformSnapshot(num2, position, rotation));
		}
		this.interpolator.Cull(num2 - num);
	}

	// Token: 0x06001C95 RID: 7317 RVA: 0x000C42EA File Offset: 0x000C24EA
	public void Snapshot(Vector3 position, Quaternion rotation)
	{
		this.Snapshot(position, rotation, PositionLerp.LerpTime - PositionLerp.TimeOffset);
	}

	// Token: 0x06001C96 RID: 7318 RVA: 0x000C42FF File Offset: 0x000C24FF
	public void SnapTo(Vector3 position, Quaternion rotation, float serverTime)
	{
		this.interpolator.Clear();
		this.Snapshot(position, rotation, serverTime);
		this.target.SetNetworkPosition(position);
		this.target.SetNetworkRotation(rotation);
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x000C432D File Offset: 0x000C252D
	public void SnapTo(Vector3 position, Quaternion rotation)
	{
		this.interpolator.last = new TransformSnapshot(PositionLerp.LerpTime, position, rotation);
		this.Wipe();
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x000C434C File Offset: 0x000C254C
	public void SnapToEnd()
	{
		float interpolationDelay = this.target.GetInterpolationDelay();
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, interpolationDelay, 0f, 0f, ref PositionLerp.snapshotPrototype);
		this.target.SetNetworkPosition(segment.tick.pos);
		this.target.SetNetworkRotation(segment.tick.rot);
		this.Wipe();
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x000C43B8 File Offset: 0x000C25B8
	public void Wipe()
	{
		this.interpolator.Clear();
		this.timeOffsetCount = 0;
		this.timeOffset0 = float.MaxValue;
		this.timeOffset1 = float.MaxValue;
		this.timeOffset2 = float.MaxValue;
		this.timeOffset3 = float.MaxValue;
	}

	// Token: 0x06001C9A RID: 7322 RVA: 0x000C43F8 File Offset: 0x000C25F8
	public static void WipeAll()
	{
		foreach (PositionLerp positionLerp in PositionLerp.InstanceList)
		{
			positionLerp.Wipe();
		}
	}

	// Token: 0x06001C9B RID: 7323 RVA: 0x000C4448 File Offset: 0x000C2648
	protected void DoCycle()
	{
		if (this.target == null)
		{
			return;
		}
		float interpolationInertia = this.target.GetInterpolationInertia();
		float num = (interpolationInertia > 0f) ? Mathf.InverseLerp(0f, interpolationInertia, PositionLerp.LerpTime - this.enabledTime) : 1f;
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolation = this.target.GetInterpolationDelay() * num;
		float num2 = this.target.GetInterpolationSmoothing() * num;
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, interpolation, extrapolationTime, num2, ref PositionLerp.snapshotPrototype);
		if (segment.next.Time >= this.interpolator.last.Time)
		{
			this.extrapolatedTime = Mathf.Min(this.extrapolatedTime + Time.deltaTime, extrapolationTime);
		}
		else
		{
			this.extrapolatedTime = Mathf.Max(this.extrapolatedTime - Time.deltaTime, 0f);
		}
		if (this.extrapolatedTime > 0f && extrapolationTime > 0f && num2 > 0f)
		{
			float t = Time.deltaTime / (this.extrapolatedTime / extrapolationTime * num2);
			segment.tick.pos = Vector3.Lerp(this.target.GetNetworkPosition(), segment.tick.pos, t);
			segment.tick.rot = Quaternion.Slerp(this.target.GetNetworkRotation(), segment.tick.rot, t);
		}
		this.target.SetNetworkPosition(segment.tick.pos);
		this.target.SetNetworkRotation(segment.tick.rot);
		if (PositionLerp.DebugDraw)
		{
			this.target.DrawInterpolationState(segment, this.interpolator.list);
		}
		if (PositionLerp.LerpTime - this.lastClientTime > 10f)
		{
			if (this.idleDisable == null)
			{
				this.idleDisable = new Action(this.target.LerpIdleDisable);
			}
			InvokeHandler.Invoke(this.target as Behaviour, this.idleDisable, 0f);
		}
	}

	// Token: 0x06001C9C RID: 7324 RVA: 0x000C464C File Offset: 0x000C284C
	public void TransformEntries(Matrix4x4 matrix)
	{
		Quaternion rotation = matrix.rotation;
		for (int i = 0; i < this.interpolator.list.Count; i++)
		{
			TransformSnapshot transformSnapshot = this.interpolator.list[i];
			transformSnapshot.pos = matrix.MultiplyPoint3x4(transformSnapshot.pos);
			transformSnapshot.rot = rotation * transformSnapshot.rot;
			this.interpolator.list[i] = transformSnapshot;
		}
		this.interpolator.last.pos = matrix.MultiplyPoint3x4(this.interpolator.last.pos);
		this.interpolator.last.rot = rotation * this.interpolator.last.rot;
	}

	// Token: 0x06001C9D RID: 7325 RVA: 0x000C4714 File Offset: 0x000C2914
	public Quaternion GetEstimatedAngularVelocity()
	{
		if (this.target == null)
		{
			return Quaternion.identity;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing, ref PositionLerp.snapshotPrototype);
		TransformSnapshot next = segment.next;
		TransformSnapshot prev = segment.prev;
		if (next.Time == prev.Time)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler((prev.rot.eulerAngles - next.rot.eulerAngles) / (prev.Time - next.Time));
	}

	// Token: 0x06001C9E RID: 7326 RVA: 0x000C47C8 File Offset: 0x000C29C8
	public Vector3 GetEstimatedVelocity()
	{
		if (this.target == null)
		{
			return Vector3.zero;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing, ref PositionLerp.snapshotPrototype);
		TransformSnapshot next = segment.next;
		TransformSnapshot prev = segment.prev;
		if (next.Time == prev.Time)
		{
			return Vector3.zero;
		}
		return (prev.pos - next.pos) / (prev.Time - next.Time);
	}

	// Token: 0x06001C9F RID: 7327 RVA: 0x000C486C File Offset: 0x000C2A6C
	public void Dispose()
	{
		this.target = null;
		this.idleDisable = null;
		this.interpolator.Clear();
		this.timeOffset0 = float.MaxValue;
		this.timeOffset1 = float.MaxValue;
		this.timeOffset2 = float.MaxValue;
		this.timeOffset3 = float.MaxValue;
		this.lastClientTime = 0f;
		this.lastServerTime = 0f;
		this.extrapolatedTime = 0f;
		this.timeOffsetCount = 0;
		this.Enabled = false;
	}

	// Token: 0x06001CA0 RID: 7328 RVA: 0x000C48ED File Offset: 0x000C2AED
	public static void Clear()
	{
		PositionLerp.InstanceList.Clear();
	}

	// Token: 0x06001CA1 RID: 7329 RVA: 0x000C48FC File Offset: 0x000C2AFC
	public static void Cycle()
	{
		PositionLerp[] buffer = PositionLerp.InstanceList.Values.Buffer;
		int count = PositionLerp.InstanceList.Count;
		for (int i = 0; i < count; i++)
		{
			buffer[i].DoCycle();
		}
	}

	// Token: 0x040015F7 RID: 5623
	private static ListHashSet<PositionLerp> InstanceList = new ListHashSet<PositionLerp>(8);

	// Token: 0x040015F8 RID: 5624
	public static bool DebugLog = false;

	// Token: 0x040015F9 RID: 5625
	public static bool DebugDraw = false;

	// Token: 0x040015FA RID: 5626
	public static int TimeOffsetInterval = 16;

	// Token: 0x040015FB RID: 5627
	public static float TimeOffset = 0f;

	// Token: 0x040015FC RID: 5628
	public const int TimeOffsetIntervalMin = 4;

	// Token: 0x040015FD RID: 5629
	public const int TimeOffsetIntervalMax = 64;

	// Token: 0x040015FE RID: 5630
	private bool enabled = true;

	// Token: 0x040015FF RID: 5631
	private Action idleDisable;

	// Token: 0x04001600 RID: 5632
	private Interpolator<TransformSnapshot> interpolator = new Interpolator<TransformSnapshot>(32);

	// Token: 0x04001601 RID: 5633
	private IPosLerpTarget target;

	// Token: 0x04001602 RID: 5634
	private static TransformSnapshot snapshotPrototype = default(TransformSnapshot);

	// Token: 0x04001603 RID: 5635
	private float timeOffset0 = float.MaxValue;

	// Token: 0x04001604 RID: 5636
	private float timeOffset1 = float.MaxValue;

	// Token: 0x04001605 RID: 5637
	private float timeOffset2 = float.MaxValue;

	// Token: 0x04001606 RID: 5638
	private float timeOffset3 = float.MaxValue;

	// Token: 0x04001607 RID: 5639
	private int timeOffsetCount;

	// Token: 0x04001608 RID: 5640
	private float lastClientTime;

	// Token: 0x04001609 RID: 5641
	private float lastServerTime;

	// Token: 0x0400160A RID: 5642
	private float extrapolatedTime;

	// Token: 0x0400160B RID: 5643
	private float enabledTime;
}
