using System;
using UnityEngine;

// Token: 0x02000330 RID: 816
public class ScaleParticleSystem : ScaleRenderer
{
	// Token: 0x06001DE7 RID: 7655 RVA: 0x000CB970 File Offset: 0x000C9B70
	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		this.startGravity = this.pSystem.gravityModifier;
		this.startSpeed = this.pSystem.startSpeed;
		this.startSize = this.pSystem.startSize;
		this.startLifeTime = this.pSystem.startLifetime;
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x000CB9C8 File Offset: 0x000C9BC8
	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		this.pSystem.startSize = this.startSize * scale;
		this.pSystem.startLifetime = this.startLifeTime * scale;
		this.pSystem.startSpeed = this.startSpeed * scale;
		this.pSystem.gravityModifier = this.startGravity * scale;
	}

	// Token: 0x040017A2 RID: 6050
	public ParticleSystem pSystem;

	// Token: 0x040017A3 RID: 6051
	public bool scaleGravity;

	// Token: 0x040017A4 RID: 6052
	[NonSerialized]
	private float startSize;

	// Token: 0x040017A5 RID: 6053
	[NonSerialized]
	private float startLifeTime;

	// Token: 0x040017A6 RID: 6054
	[NonSerialized]
	private float startSpeed;

	// Token: 0x040017A7 RID: 6055
	[NonSerialized]
	private float startGravity;
}
