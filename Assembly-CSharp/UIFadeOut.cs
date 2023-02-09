using System;
using UnityEngine;

// Token: 0x020007D9 RID: 2009
public class UIFadeOut : MonoBehaviour
{
	// Token: 0x06003417 RID: 13335 RVA: 0x0013D3EF File Offset: 0x0013B5EF
	private void Start()
	{
		this.timeStarted = Time.realtimeSinceStartup;
	}

	// Token: 0x06003418 RID: 13336 RVA: 0x0013D3FC File Offset: 0x0013B5FC
	private void Update()
	{
		float num = this.timeStarted;
		this.targetGroup.alpha = Mathf.InverseLerp(num + this.secondsToFadeOut, num, Time.realtimeSinceStartup - this.fadeDelay);
		if (this.destroyOnFaded && Time.realtimeSinceStartup - this.fadeDelay > this.timeStarted + this.secondsToFadeOut)
		{
			GameManager.Destroy(base.gameObject, 0f);
		}
	}

	// Token: 0x04002C8B RID: 11403
	public float secondsToFadeOut = 3f;

	// Token: 0x04002C8C RID: 11404
	public bool destroyOnFaded = true;

	// Token: 0x04002C8D RID: 11405
	public CanvasGroup targetGroup;

	// Token: 0x04002C8E RID: 11406
	public float fadeDelay;

	// Token: 0x04002C8F RID: 11407
	private float timeStarted;
}
