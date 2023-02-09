using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002D7 RID: 727
public class ViewShake
{
	// Token: 0x17000228 RID: 552
	// (get) Token: 0x06001CD9 RID: 7385 RVA: 0x000C593A File Offset: 0x000C3B3A
	// (set) Token: 0x06001CDA RID: 7386 RVA: 0x000C5942 File Offset: 0x000C3B42
	public Vector3 PositionOffset { get; protected set; }

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x06001CDB RID: 7387 RVA: 0x000C594B File Offset: 0x000C3B4B
	// (set) Token: 0x06001CDC RID: 7388 RVA: 0x000C5953 File Offset: 0x000C3B53
	public Vector3 AnglesOffset { get; protected set; }

	// Token: 0x06001CDD RID: 7389 RVA: 0x000C595C File Offset: 0x000C3B5C
	public void AddShake(float amplitude, float frequency, float duration)
	{
		this.Entries.Add(new ViewShake.ShakeParameters
		{
			amplitude = amplitude,
			frequency = Mathf.Max(frequency, 0.01f),
			duration = duration,
			endTime = Time.time + duration,
			nextShake = 0f,
			angle = 0f,
			infinite = (duration <= 0f)
		});
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x000C59CC File Offset: 0x000C3BCC
	public void Update()
	{
		Vector3 a = Vector3.zero;
		Vector3 zero = Vector3.zero;
		this.Entries.RemoveAll((ViewShake.ShakeParameters i) => !i.infinite && Time.time > i.endTime);
		foreach (ViewShake.ShakeParameters shakeParameters in this.Entries)
		{
			if (Time.time > shakeParameters.nextShake)
			{
				shakeParameters.nextShake = Time.time + 1f / shakeParameters.frequency;
				shakeParameters.offset = new Vector3(UnityEngine.Random.Range(-shakeParameters.amplitude, shakeParameters.amplitude), UnityEngine.Random.Range(-shakeParameters.amplitude, shakeParameters.amplitude), UnityEngine.Random.Range(-shakeParameters.amplitude, shakeParameters.amplitude));
				shakeParameters.angle = UnityEngine.Random.Range(-shakeParameters.amplitude * 0.25f, shakeParameters.amplitude * 0.25f);
			}
			float num = 0f;
			float num2 = shakeParameters.infinite ? 1f : ((shakeParameters.endTime - Time.time) / shakeParameters.duration);
			if (num2 != 0f)
			{
				num = shakeParameters.frequency / num2;
			}
			num2 *= num2;
			float f = Time.time * num;
			num2 *= Mathf.Sin(f);
			a += shakeParameters.offset * num2;
			zero.z += shakeParameters.angle * num2;
			if (!shakeParameters.infinite)
			{
				shakeParameters.amplitude -= shakeParameters.amplitude * Time.deltaTime / (shakeParameters.duration * shakeParameters.frequency);
			}
		}
		this.PositionOffset = a * 0.01f;
		this.AnglesOffset = zero;
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x000C5BB8 File Offset: 0x000C3DB8
	public void Stop()
	{
		this.Entries.Clear();
		this.PositionOffset = Vector3.zero;
		this.AnglesOffset = Vector3.zero;
	}

	// Token: 0x0400168B RID: 5771
	protected List<ViewShake.ShakeParameters> Entries = new List<ViewShake.ShakeParameters>();

	// Token: 0x02000C4A RID: 3146
	protected class ShakeParameters
	{
		// Token: 0x040041A5 RID: 16805
		public float endTime;

		// Token: 0x040041A6 RID: 16806
		public float duration;

		// Token: 0x040041A7 RID: 16807
		public float amplitude;

		// Token: 0x040041A8 RID: 16808
		public float frequency;

		// Token: 0x040041A9 RID: 16809
		public float nextShake;

		// Token: 0x040041AA RID: 16810
		public float angle;

		// Token: 0x040041AB RID: 16811
		public Vector3 offset;

		// Token: 0x040041AC RID: 16812
		public bool infinite;
	}
}
