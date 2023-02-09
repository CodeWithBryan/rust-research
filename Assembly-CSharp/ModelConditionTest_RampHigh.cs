using System;
using UnityEngine;

// Token: 0x02000246 RID: 582
public class ModelConditionTest_RampHigh : ModelConditionTest
{
	// Token: 0x06001B38 RID: 6968 RVA: 0x000BE598 File Offset: 0x000BC798
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, 0.75f, 0f), new Vector3(3f, 1.5f, 3f));
	}

	// Token: 0x06001B39 RID: 6969 RVA: 0x000BE5EC File Offset: 0x000BC7EC
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("ramp/sockets/block-male/1");
		return entityLink != null && entityLink.IsEmpty();
	}

	// Token: 0x04001444 RID: 5188
	private const string socket = "ramp/sockets/block-male/1";
}
