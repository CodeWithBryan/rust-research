using System;
using UnityEngine;

// Token: 0x02000418 RID: 1048
public class LifeScale : BaseMonoBehaviour
{
	// Token: 0x060022FF RID: 8959 RVA: 0x000DEEF2 File Offset: 0x000DD0F2
	protected void Awake()
	{
		this.updateScaleAction = new Action(this.UpdateScale);
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x000DEF06 File Offset: 0x000DD106
	public void OnEnable()
	{
		this.Init();
		base.transform.localScale = this.initialScale;
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x000DEF1F File Offset: 0x000DD11F
	public void SetProgress(float progress)
	{
		this.Init();
		this.targetLerpScale = Vector3.Lerp(this.initialScale, this.finalScale, progress);
		base.InvokeRepeating(this.updateScaleAction, 0f, 0.015f);
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000DEF55 File Offset: 0x000DD155
	public void Init()
	{
		if (!this.initialized)
		{
			this.initialScale = base.transform.localScale;
			this.initialized = true;
		}
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x000DEF78 File Offset: 0x000DD178
	public void UpdateScale()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, this.targetLerpScale, Time.deltaTime);
		if (base.transform.localScale == this.targetLerpScale)
		{
			this.targetLerpScale = Vector3.zero;
			base.CancelInvoke(this.updateScaleAction);
		}
	}

	// Token: 0x04001B66 RID: 7014
	[NonSerialized]
	private bool initialized;

	// Token: 0x04001B67 RID: 7015
	[NonSerialized]
	private Vector3 initialScale;

	// Token: 0x04001B68 RID: 7016
	public Vector3 finalScale = Vector3.one;

	// Token: 0x04001B69 RID: 7017
	private Vector3 targetLerpScale = Vector3.zero;

	// Token: 0x04001B6A RID: 7018
	private Action updateScaleAction;
}
