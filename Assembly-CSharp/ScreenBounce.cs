using System;
using UnityEngine;

// Token: 0x02000335 RID: 821
public class ScreenBounce : BaseScreenShake
{
	// Token: 0x06001DFF RID: 7679 RVA: 0x000CBDBA File Offset: 0x000C9FBA
	public override void Setup()
	{
		this.bounceTime = UnityEngine.Random.Range(0f, 1000f);
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x000CBDD4 File Offset: 0x000C9FD4
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		this.bounceTime += Time.deltaTime * this.bounceSpeed.Evaluate(delta);
		float num = this.bounceScale.Evaluate(delta) * 0.1f;
		this.bounceVelocity.x = Mathf.Sin(this.bounceTime * 20f) * num;
		this.bounceVelocity.y = Mathf.Cos(this.bounceTime * 25f) * num;
		this.bounceVelocity.z = 0f;
		Vector3 vector = Vector3.zero;
		vector += this.bounceVelocity.x * cam.right;
		vector += this.bounceVelocity.y * cam.up;
		if (cam)
		{
			cam.position += vector;
		}
		if (vm)
		{
			vm.position += vector * -1f * this.bounceViewmodel.Evaluate(delta);
		}
	}

	// Token: 0x040017BB RID: 6075
	public AnimationCurve bounceScale;

	// Token: 0x040017BC RID: 6076
	public AnimationCurve bounceSpeed;

	// Token: 0x040017BD RID: 6077
	public AnimationCurve bounceViewmodel;

	// Token: 0x040017BE RID: 6078
	private float bounceTime;

	// Token: 0x040017BF RID: 6079
	private Vector3 bounceVelocity = Vector3.zero;
}
