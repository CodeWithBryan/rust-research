using System;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class ReverbZoneTrigger : TriggerBase, IClientComponentEx, ILOD
{
	// Token: 0x06001AD4 RID: 6868 RVA: 0x000BC73C File Offset: 0x000BA93C
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this.trigger);
		p.RemoveComponent(this.reverbZone);
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}

	// Token: 0x06001AD5 RID: 6869 RVA: 0x00007074 File Offset: 0x00005274
	public bool IsSyncedToParent()
	{
		return false;
	}

	// Token: 0x04001394 RID: 5012
	public Collider trigger;

	// Token: 0x04001395 RID: 5013
	public AudioReverbZone reverbZone;

	// Token: 0x04001396 RID: 5014
	public float lodDistance = 100f;

	// Token: 0x04001397 RID: 5015
	public bool inRange;

	// Token: 0x04001398 RID: 5016
	public ReverbSettings reverbSettings;
}
