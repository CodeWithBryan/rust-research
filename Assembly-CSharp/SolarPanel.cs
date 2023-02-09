using System;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class SolarPanel : IOEntity
{
	// Token: 0x06001582 RID: 5506 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06001583 RID: 5507 RVA: 0x000A69D7 File Offset: 0x000A4BD7
	public override int MaximalPowerOutput()
	{
		return this.maximalPowerOutput;
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x00007074 File Offset: 0x00005274
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x000A69DF File Offset: 0x000A4BDF
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.SunUpdate), 1f, 5f, 2f);
	}

	// Token: 0x06001586 RID: 5510 RVA: 0x000A6A08 File Offset: 0x000A4C08
	public void SunUpdate()
	{
		int num = this.currentEnergy;
		if (TOD_Sky.Instance.IsNight)
		{
			num = 0;
		}
		else
		{
			Vector3 sunDirection = TOD_Sky.Instance.SunDirection;
			float value = Vector3.Dot(this.sunSampler.transform.forward, sunDirection);
			float num2 = Mathf.InverseLerp(this.dot_minimum, this.dot_maximum, value);
			if (num2 > 0f && !base.IsVisible(this.sunSampler.transform.position + sunDirection * 100f, 101f))
			{
				num2 = 0f;
			}
			num = Mathf.FloorToInt((float)this.maximalPowerOutput * num2 * base.healthFraction);
		}
		bool flag = this.currentEnergy != num;
		this.currentEnergy = num;
		if (flag)
		{
			this.MarkDirty();
		}
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x000620BF File Offset: 0x000602BF
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return this.currentEnergy;
	}

	// Token: 0x04000DD8 RID: 3544
	public Transform sunSampler;

	// Token: 0x04000DD9 RID: 3545
	private const int tickrateSeconds = 60;

	// Token: 0x04000DDA RID: 3546
	public int maximalPowerOutput = 10;

	// Token: 0x04000DDB RID: 3547
	public float dot_minimum = 0.1f;

	// Token: 0x04000DDC RID: 3548
	public float dot_maximum = 0.6f;
}
