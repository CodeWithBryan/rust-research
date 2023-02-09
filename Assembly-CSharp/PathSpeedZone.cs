using System;
using UnityEngine;

// Token: 0x0200019D RID: 413
public class PathSpeedZone : MonoBehaviour
{
	// Token: 0x060017AC RID: 6060 RVA: 0x000B061E File Offset: 0x000AE81E
	public OBB WorldSpaceBounds()
	{
		return new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
	}

	// Token: 0x060017AD RID: 6061 RVA: 0x000B064C File Offset: 0x000AE84C
	public float GetMaxSpeed()
	{
		return this.maxVelocityPerSec;
	}

	// Token: 0x060017AE RID: 6062 RVA: 0x000B0654 File Offset: 0x000AE854
	public virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
		Gizmos.color = new Color(1f, 0.7f, 0f, 1f);
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x040010A2 RID: 4258
	public Bounds bounds;

	// Token: 0x040010A3 RID: 4259
	public OBB obbBounds;

	// Token: 0x040010A4 RID: 4260
	public float maxVelocityPerSec = 5f;
}
