using System;

// Token: 0x0200020C RID: 524
public class AmbienceZone : TriggerBase, IClientComponentEx
{
	// Token: 0x06001A91 RID: 6801 RVA: 0x000BB89A File Offset: 0x000B9A9A
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}

	// Token: 0x040012F0 RID: 4848
	public AmbienceDefinitionList baseAmbience;

	// Token: 0x040012F1 RID: 4849
	public AmbienceDefinitionList stings;

	// Token: 0x040012F2 RID: 4850
	public float priority;

	// Token: 0x040012F3 RID: 4851
	public bool overrideCrossfadeTime;

	// Token: 0x040012F4 RID: 4852
	public float crossfadeTime = 1f;
}
