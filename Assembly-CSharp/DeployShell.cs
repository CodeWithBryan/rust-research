using System;
using UnityEngine;

// Token: 0x020004CA RID: 1226
public class DeployShell : PrefabAttribute
{
	// Token: 0x06002769 RID: 10089 RVA: 0x000F292E File Offset: 0x000F0B2E
	public OBB WorldSpaceBounds(Transform transform)
	{
		return new OBB(transform.position, transform.lossyScale, transform.rotation, this.bounds);
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000F294D File Offset: 0x000F0B4D
	public float LineOfSightPadding()
	{
		return 0.025f;
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000F2954 File Offset: 0x000F0B54
	protected override Type GetIndexedType()
	{
		return typeof(DeployShell);
	}

	// Token: 0x04001FB2 RID: 8114
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
}
