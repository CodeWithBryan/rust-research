using System;
using UnityEngine;

// Token: 0x02000337 RID: 823
public class ScreenFov : BaseScreenShake
{
	// Token: 0x06001E05 RID: 7685 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void Setup()
	{
	}

	// Token: 0x06001E06 RID: 7686 RVA: 0x000CC0E6 File Offset: 0x000CA2E6
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if (cam)
		{
			cam.component.fieldOfView += this.FovAdjustment.Evaluate(delta);
		}
	}

	// Token: 0x040017C9 RID: 6089
	public AnimationCurve FovAdjustment;
}
