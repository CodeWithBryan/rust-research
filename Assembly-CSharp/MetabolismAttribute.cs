using System;
using UnityEngine;

// Token: 0x020003DE RID: 990
[Serializable]
public class MetabolismAttribute
{
	// Token: 0x1700029A RID: 666
	// (get) Token: 0x06002195 RID: 8597 RVA: 0x000D7C39 File Offset: 0x000D5E39
	public float greatFraction
	{
		get
		{
			return Mathf.Floor(this.Fraction() / 0.1f) / 10f;
		}
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x000D7C52 File Offset: 0x000D5E52
	public void Reset()
	{
		this.value = Mathf.Clamp(UnityEngine.Random.Range(this.startMin, this.startMax), this.min, this.max);
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x000D7C7C File Offset: 0x000D5E7C
	public float Fraction()
	{
		return Mathf.InverseLerp(this.min, this.max, this.value);
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x000D7C95 File Offset: 0x000D5E95
	public float InverseFraction()
	{
		return 1f - this.Fraction();
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x000D7CA3 File Offset: 0x000D5EA3
	public void Add(float val)
	{
		this.value = Mathf.Clamp(this.value + val, this.min, this.max);
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x000D7CC4 File Offset: 0x000D5EC4
	public void Subtract(float val)
	{
		this.value = Mathf.Clamp(this.value - val, this.min, this.max);
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x000D7CE5 File Offset: 0x000D5EE5
	public void Increase(float fTarget)
	{
		fTarget = Mathf.Clamp(fTarget, this.min, this.max);
		if (fTarget <= this.value)
		{
			return;
		}
		this.value = fTarget;
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x000D7D0C File Offset: 0x000D5F0C
	public void MoveTowards(float fTarget, float fRate)
	{
		if (fRate == 0f)
		{
			return;
		}
		this.value = Mathf.Clamp(Mathf.MoveTowards(this.value, fTarget, fRate), this.min, this.max);
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x000D7D3B File Offset: 0x000D5F3B
	public bool HasChanged()
	{
		bool result = this.lastValue != this.value;
		this.lastValue = this.value;
		return result;
	}

	// Token: 0x0600219E RID: 8606 RVA: 0x000D7D5C File Offset: 0x000D5F5C
	public bool HasGreatlyChanged()
	{
		float greatFraction = this.greatFraction;
		bool result = this.lastGreatFraction != greatFraction;
		this.lastGreatFraction = greatFraction;
		return result;
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x000D7D83 File Offset: 0x000D5F83
	public void SetValue(float newValue)
	{
		this.value = newValue;
	}

	// Token: 0x040019FB RID: 6651
	public float startMin;

	// Token: 0x040019FC RID: 6652
	public float startMax;

	// Token: 0x040019FD RID: 6653
	public float min;

	// Token: 0x040019FE RID: 6654
	public float max;

	// Token: 0x040019FF RID: 6655
	public float value;

	// Token: 0x04001A00 RID: 6656
	internal float lastValue;

	// Token: 0x04001A01 RID: 6657
	internal float lastGreatFraction;

	// Token: 0x04001A02 RID: 6658
	private const float greatInterval = 0.1f;

	// Token: 0x02000C80 RID: 3200
	public enum Type
	{
		// Token: 0x04004290 RID: 17040
		Calories,
		// Token: 0x04004291 RID: 17041
		Hydration,
		// Token: 0x04004292 RID: 17042
		Heartrate,
		// Token: 0x04004293 RID: 17043
		Poison,
		// Token: 0x04004294 RID: 17044
		Radiation,
		// Token: 0x04004295 RID: 17045
		Bleeding,
		// Token: 0x04004296 RID: 17046
		Health,
		// Token: 0x04004297 RID: 17047
		HealthOverTime
	}
}
