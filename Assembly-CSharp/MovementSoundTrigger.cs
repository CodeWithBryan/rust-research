using System;
using UnityEngine;

// Token: 0x02000217 RID: 535
public class MovementSoundTrigger : TriggerBase, IClientComponentEx, ILOD
{
	// Token: 0x06001AA9 RID: 6825 RVA: 0x000BBE21 File Offset: 0x000BA021
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this.collider);
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}

	// Token: 0x0400134A RID: 4938
	public SoundDefinition softSound;

	// Token: 0x0400134B RID: 4939
	public SoundDefinition medSound;

	// Token: 0x0400134C RID: 4940
	public SoundDefinition hardSound;

	// Token: 0x0400134D RID: 4941
	public Collider collider;
}
