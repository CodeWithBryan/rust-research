using System;
using UnityEngine;

// Token: 0x02000247 RID: 583
public class ModelConditionTest_RampLow : ModelConditionTest
{
	// Token: 0x06001B3B RID: 6971 RVA: 0x000BE610 File Offset: 0x000BC810
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, 0.375f, 0f), new Vector3(3f, 0.75f, 3f));
	}

	// Token: 0x06001B3C RID: 6972 RVA: 0x000BE664 File Offset: 0x000BC864
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("ramp/sockets/block-male/1");
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x04001445 RID: 5189
	private const string socket = "ramp/sockets/block-male/1";
}
