using System;
using UnityEngine;

// Token: 0x02000332 RID: 818
public class ScaleTrailRenderer : ScaleRenderer
{
	// Token: 0x06001DF1 RID: 7665 RVA: 0x000CBB18 File Offset: 0x000C9D18
	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		if (this.myRenderer)
		{
			this.trailRenderer = this.myRenderer.GetComponent<TrailRenderer>();
		}
		else
		{
			this.trailRenderer = base.GetComponentInChildren<TrailRenderer>();
		}
		this.startWidth = this.trailRenderer.startWidth;
		this.endWidth = this.trailRenderer.endWidth;
		this.duration = this.trailRenderer.time;
		this.startMultiplier = this.trailRenderer.widthMultiplier;
	}

	// Token: 0x06001DF2 RID: 7666 RVA: 0x000CBB9C File Offset: 0x000C9D9C
	public override void SetScale_Internal(float scale)
	{
		if (scale == 0f)
		{
			this.trailRenderer.emitting = false;
			this.trailRenderer.enabled = false;
			this.trailRenderer.time = 0f;
			this.trailRenderer.Clear();
			return;
		}
		if (!this.trailRenderer.emitting)
		{
			this.trailRenderer.Clear();
		}
		this.trailRenderer.emitting = true;
		this.trailRenderer.enabled = true;
		base.SetScale_Internal(scale);
		this.trailRenderer.widthMultiplier = this.startMultiplier * scale;
		this.trailRenderer.time = this.duration * scale;
	}

	// Token: 0x040017AE RID: 6062
	private TrailRenderer trailRenderer;

	// Token: 0x040017AF RID: 6063
	[NonSerialized]
	private float startWidth;

	// Token: 0x040017B0 RID: 6064
	[NonSerialized]
	private float endWidth;

	// Token: 0x040017B1 RID: 6065
	[NonSerialized]
	private float duration;

	// Token: 0x040017B2 RID: 6066
	[NonSerialized]
	private float startMultiplier;
}
