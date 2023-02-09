using System;
using UnityEngine;

// Token: 0x02000336 RID: 822
public class ScreenBounceFade : BaseScreenShake
{
	// Token: 0x06001E02 RID: 7682 RVA: 0x000CBF15 File Offset: 0x000CA115
	public override void Setup()
	{
		this.bounceTime = UnityEngine.Random.Range(0f, 1000f);
	}

	// Token: 0x06001E03 RID: 7683 RVA: 0x000CBF2C File Offset: 0x000CA12C
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		float value = Vector3.Distance(cam.position, base.transform.position);
		float num = 1f - Mathf.InverseLerp(0f, this.maxDistance, value);
		this.bounceTime += Time.deltaTime * this.bounceSpeed.Evaluate(delta);
		float num2 = this.distanceFalloff.Evaluate(num);
		float num3 = this.bounceScale.Evaluate(delta) * 0.1f * num2 * this.scale * this.timeFalloff.Evaluate(delta);
		this.bounceVelocity.x = Mathf.Sin(this.bounceTime * 20f) * num3;
		this.bounceVelocity.y = Mathf.Cos(this.bounceTime * 25f) * num3;
		this.bounceVelocity.z = 0f;
		Vector3 vector = Vector3.zero;
		vector += this.bounceVelocity.x * cam.right;
		vector += this.bounceVelocity.y * cam.up;
		vector *= num;
		if (cam)
		{
			cam.position += vector;
		}
		if (vm)
		{
			vm.position += vector * -1f * this.bounceViewmodel.Evaluate(delta);
		}
	}

	// Token: 0x040017C0 RID: 6080
	public AnimationCurve bounceScale;

	// Token: 0x040017C1 RID: 6081
	public AnimationCurve bounceSpeed;

	// Token: 0x040017C2 RID: 6082
	public AnimationCurve bounceViewmodel;

	// Token: 0x040017C3 RID: 6083
	public AnimationCurve distanceFalloff;

	// Token: 0x040017C4 RID: 6084
	public AnimationCurve timeFalloff;

	// Token: 0x040017C5 RID: 6085
	private float bounceTime;

	// Token: 0x040017C6 RID: 6086
	private Vector3 bounceVelocity = Vector3.zero;

	// Token: 0x040017C7 RID: 6087
	public float maxDistance = 10f;

	// Token: 0x040017C8 RID: 6088
	public float scale = 1f;
}
