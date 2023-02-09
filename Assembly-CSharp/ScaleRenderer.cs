using System;
using UnityEngine;

// Token: 0x02000331 RID: 817
public class ScaleRenderer : MonoBehaviour
{
	// Token: 0x06001DEA RID: 7658 RVA: 0x000CBA30 File Offset: 0x000C9C30
	private bool ScaleDifferent(float newScale)
	{
		return newScale != this.lastScale;
	}

	// Token: 0x06001DEB RID: 7659 RVA: 0x000CBA3E File Offset: 0x000C9C3E
	public void Start()
	{
		if (this.useRandomScale)
		{
			this.SetScale(UnityEngine.Random.Range(this.scaleMin, this.scaleMax));
		}
	}

	// Token: 0x06001DEC RID: 7660 RVA: 0x000CBA60 File Offset: 0x000C9C60
	public void SetScale(float scale)
	{
		if (!this.hasInitialValues)
		{
			this.GatherInitialValues();
		}
		if (this.ScaleDifferent(scale) || (scale > 0f && !this.myRenderer.enabled))
		{
			this.SetRendererEnabled(scale != 0f);
			this.SetScale_Internal(scale);
		}
	}

	// Token: 0x06001DED RID: 7661 RVA: 0x000CBAB1 File Offset: 0x000C9CB1
	public virtual void SetScale_Internal(float scale)
	{
		this.lastScale = scale;
	}

	// Token: 0x06001DEE RID: 7662 RVA: 0x000CBABA File Offset: 0x000C9CBA
	public virtual void SetRendererEnabled(bool isEnabled)
	{
		if (this.myRenderer && this.myRenderer.enabled != isEnabled)
		{
			this.myRenderer.enabled = isEnabled;
		}
	}

	// Token: 0x06001DEF RID: 7663 RVA: 0x000CBAE3 File Offset: 0x000C9CE3
	public virtual void GatherInitialValues()
	{
		this.hasInitialValues = true;
	}

	// Token: 0x040017A8 RID: 6056
	public bool useRandomScale;

	// Token: 0x040017A9 RID: 6057
	public float scaleMin = 1f;

	// Token: 0x040017AA RID: 6058
	public float scaleMax = 1f;

	// Token: 0x040017AB RID: 6059
	private float lastScale = -1f;

	// Token: 0x040017AC RID: 6060
	protected bool hasInitialValues;

	// Token: 0x040017AD RID: 6061
	public Renderer myRenderer;
}
