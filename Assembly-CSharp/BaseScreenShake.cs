using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000334 RID: 820
public abstract class BaseScreenShake : MonoBehaviour
{
	// Token: 0x06001DF7 RID: 7671 RVA: 0x000CBC88 File Offset: 0x000C9E88
	public static void Apply(Camera cam, BaseViewModel vm)
	{
		CachedTransform<Camera> cachedTransform = new CachedTransform<Camera>(cam);
		CachedTransform<BaseViewModel> cachedTransform2 = new CachedTransform<BaseViewModel>(vm);
		for (int i = 0; i < BaseScreenShake.list.Count; i++)
		{
			BaseScreenShake.list[i].Run(ref cachedTransform, ref cachedTransform2);
		}
		cachedTransform.Apply();
		cachedTransform2.Apply();
	}

	// Token: 0x06001DF8 RID: 7672 RVA: 0x000CBCDC File Offset: 0x000C9EDC
	protected void OnEnable()
	{
		BaseScreenShake.list.Add(this);
		this.timeTaken = 0f;
		this.Setup();
	}

	// Token: 0x06001DF9 RID: 7673 RVA: 0x000CBCFA File Offset: 0x000C9EFA
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		BaseScreenShake.list.Remove(this);
	}

	// Token: 0x06001DFA RID: 7674 RVA: 0x000CBD10 File Offset: 0x000C9F10
	public void Run(ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if (this.timeTaken > this.length)
		{
			return;
		}
		if (Time.frameCount != this.currentFrame)
		{
			this.timeTaken += Time.deltaTime;
			this.currentFrame = Time.frameCount;
		}
		float delta = Mathf.InverseLerp(0f, this.length, this.timeTaken);
		this.Run(delta, ref cam, ref vm);
	}

	// Token: 0x06001DFB RID: 7675
	public abstract void Setup();

	// Token: 0x06001DFC RID: 7676
	public abstract void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm);

	// Token: 0x040017B4 RID: 6068
	public static List<BaseScreenShake> list = new List<BaseScreenShake>();

	// Token: 0x040017B5 RID: 6069
	internal static float punchFadeScale = 0f;

	// Token: 0x040017B6 RID: 6070
	internal static float bobScale = 0f;

	// Token: 0x040017B7 RID: 6071
	internal static float animPunchMagnitude = 10f;

	// Token: 0x040017B8 RID: 6072
	public float length = 2f;

	// Token: 0x040017B9 RID: 6073
	internal float timeTaken;

	// Token: 0x040017BA RID: 6074
	private int currentFrame = -1;
}
