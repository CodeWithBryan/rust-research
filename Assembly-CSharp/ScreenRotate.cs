using System;
using UnityEngine;

// Token: 0x02000338 RID: 824
public class ScreenRotate : BaseScreenShake
{
	// Token: 0x06001E08 RID: 7688 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void Setup()
	{
	}

	// Token: 0x06001E09 RID: 7689 RVA: 0x000CC11C File Offset: 0x000CA31C
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		Vector3 zero = Vector3.zero;
		zero.x = this.Pitch.Evaluate(delta);
		zero.y = this.Yaw.Evaluate(delta);
		zero.z = this.Roll.Evaluate(delta);
		if (cam)
		{
			cam.rotation *= Quaternion.Euler(zero * this.scale);
		}
		if (vm && this.useViewModelEffect)
		{
			vm.rotation *= Quaternion.Euler(zero * this.scale * -1f * (1f - this.ViewmodelEffect.Evaluate(delta)));
		}
	}

	// Token: 0x040017CA RID: 6090
	public AnimationCurve Pitch;

	// Token: 0x040017CB RID: 6091
	public AnimationCurve Yaw;

	// Token: 0x040017CC RID: 6092
	public AnimationCurve Roll;

	// Token: 0x040017CD RID: 6093
	public AnimationCurve ViewmodelEffect;

	// Token: 0x040017CE RID: 6094
	public float scale = 1f;

	// Token: 0x040017CF RID: 6095
	public bool useViewModelEffect = true;
}
