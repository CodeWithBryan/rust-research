using System;
using UnityEngine;

// Token: 0x02000961 RID: 2401
public class ExplosionsLightCurves : MonoBehaviour
{
	// Token: 0x060038A4 RID: 14500 RVA: 0x0014E30A File Offset: 0x0014C50A
	private void Awake()
	{
		this.lightSource = base.GetComponent<Light>();
		this.lightSource.intensity = this.LightCurve.Evaluate(0f);
	}

	// Token: 0x060038A5 RID: 14501 RVA: 0x0014E333 File Offset: 0x0014C533
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x060038A6 RID: 14502 RVA: 0x0014E348 File Offset: 0x0014C548
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			float intensity = this.LightCurve.Evaluate(num / this.GraphTimeMultiplier) * this.GraphIntensityMultiplier;
			this.lightSource.intensity = intensity;
		}
		if (num >= this.GraphTimeMultiplier)
		{
			this.canUpdate = false;
		}
	}

	// Token: 0x040032F2 RID: 13042
	public AnimationCurve LightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040032F3 RID: 13043
	public float GraphTimeMultiplier = 1f;

	// Token: 0x040032F4 RID: 13044
	public float GraphIntensityMultiplier = 1f;

	// Token: 0x040032F5 RID: 13045
	private bool canUpdate;

	// Token: 0x040032F6 RID: 13046
	private float startTime;

	// Token: 0x040032F7 RID: 13047
	private Light lightSource;
}
