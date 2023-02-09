using System;
using ConVar;

// Token: 0x020003C5 RID: 965
public class DeployableDecay : global::Decay
{
	// Token: 0x060020FE RID: 8446 RVA: 0x000D5BFB File Offset: 0x000D3DFB
	public override float GetDecayDelay(BaseEntity entity)
	{
		return this.decayDelay * 60f * 60f;
	}

	// Token: 0x060020FF RID: 8447 RVA: 0x000D5C0F File Offset: 0x000D3E0F
	public override float GetDecayDuration(BaseEntity entity)
	{
		return this.decayDuration * 60f * 60f;
	}

	// Token: 0x06002100 RID: 8448 RVA: 0x000D5292 File Offset: 0x000D3492
	public override bool ShouldDecay(BaseEntity entity)
	{
		return ConVar.Decay.upkeep || entity.IsOutside();
	}

	// Token: 0x0400197F RID: 6527
	public float decayDelay = 8f;

	// Token: 0x04001980 RID: 6528
	public float decayDuration = 8f;
}
