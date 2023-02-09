using System;
using UnityEngine;

// Token: 0x02000963 RID: 2403
public class ExplosionsScaleCurves : MonoBehaviour
{
	// Token: 0x060038AB RID: 14507 RVA: 0x0014E3F1 File Offset: 0x0014C5F1
	private void Awake()
	{
		this.t = base.transform;
	}

	// Token: 0x060038AC RID: 14508 RVA: 0x0014E3FF File Offset: 0x0014C5FF
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.evalX = 0f;
		this.evalY = 0f;
		this.evalZ = 0f;
	}

	// Token: 0x060038AD RID: 14509 RVA: 0x0014E430 File Offset: 0x0014C630
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (num <= this.GraphTimeMultiplier.x)
		{
			this.evalX = this.ScaleCurveX.Evaluate(num / this.GraphTimeMultiplier.x) * this.GraphScaleMultiplier.x;
		}
		if (num <= this.GraphTimeMultiplier.y)
		{
			this.evalY = this.ScaleCurveY.Evaluate(num / this.GraphTimeMultiplier.y) * this.GraphScaleMultiplier.y;
		}
		if (num <= this.GraphTimeMultiplier.z)
		{
			this.evalZ = this.ScaleCurveZ.Evaluate(num / this.GraphTimeMultiplier.z) * this.GraphScaleMultiplier.z;
		}
		this.t.localScale = new Vector3(this.evalX, this.evalY, this.evalZ);
	}

	// Token: 0x040032F9 RID: 13049
	public AnimationCurve ScaleCurveX = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040032FA RID: 13050
	public AnimationCurve ScaleCurveY = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040032FB RID: 13051
	public AnimationCurve ScaleCurveZ = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040032FC RID: 13052
	public Vector3 GraphTimeMultiplier = Vector3.one;

	// Token: 0x040032FD RID: 13053
	public Vector3 GraphScaleMultiplier = Vector3.one;

	// Token: 0x040032FE RID: 13054
	private float startTime;

	// Token: 0x040032FF RID: 13055
	private Transform t;

	// Token: 0x04003300 RID: 13056
	private float evalX;

	// Token: 0x04003301 RID: 13057
	private float evalY;

	// Token: 0x04003302 RID: 13058
	private float evalZ;
}
